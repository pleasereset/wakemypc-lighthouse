using ree7.WakeMyPC.LighthouseCore.Commands;
using ree7.WakeMyPC.LighthouseCore.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading;

namespace ree7.WakeMyPC.LighthouseCore
{
	public class Server
	{
		const char PASSWORD_SEPARATOR = '/';
		const int SOCKET_READ_TIMEOUT = 5000;

		volatile bool m_Continue;
		List<CommandBase> m_Commands;
		Thread m_Worker;
		int m_Port;
		string m_Password;

		public Server(int port, string password)
		{
			// Reads configuration data
			try
			{
				this.m_Port = port;
				this.m_Password = password;
			}
			catch (Exception ex)
			{
				Log.d("Exception at Server() : " + ex);
				throw new InvalidOperationException();
			}

			registerCommands();
			Log.d("WMP Lighthouse ready on " + port + " with password " + password);
		}

		public bool IsRunning
		{
			get { return this.isWorkerAlive(); }
		}

		public void Start()
		{
			// Check if a worker isn't already running
			if (isWorkerAlive())
			{
				Log.d("Could not start server, an instance is already running.");
				return;
			}

			m_Continue = true;
			m_Worker = new Thread(() =>
			{
				TcpListener listener = new TcpListener(IPAddress.Any, m_Port);
				listener.AllowNatTraversal(true);
				listener.ExclusiveAddressUse = false;
				listener.Server.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);

				listener.Start();

				while (m_Continue)
				{
					Log.d("Waiting for connection...");
					TcpClient client = listener.AcceptTcpClient();

					Log.d("Connection accepted.");
					NetworkStream ns = client.GetStream();
#if !DEBUG
					ns.ReadTimeout = SOCKET_READ_TIMEOUT;
#endif

					try
					{
						// Sends the HELLO message
						Byte[] sendBytes = Encoding.UTF8.GetBytes(GetHello());
						ns.Write(sendBytes, 0, sendBytes.Length);
						ns.Flush();
						Log.d("HELLO sent.");
						

						// Reads NetworkStream into a byte buffer.
						byte[] bytes = new byte[client.ReceiveBufferSize];

						// Read can return anything from 0 to numBytesToRead. 
						// This method blocks until at least one byte is read.
						StringBuilder readString = null;
						do
						{
							Log.d("Waiting for data...");

							readString = new StringBuilder();
							int numberOfBytesRead = 0;

							// Incoming message may be larger than the buffer size.
							do
							{
								numberOfBytesRead = ns.Read(bytes, 0, bytes.Length);
								readString.AppendFormat("{0}", Encoding.UTF8.GetString(bytes, 0, numberOfBytesRead).TrimEnd('\0', '\r', '\n'));		
							}
							while(ns.DataAvailable);
							
							Log.d("Received : " + readString.ToString() + " - " + numberOfBytesRead.ToString() + " bytes");

							
						}
						while (OnData(readString.ToString(), ns));
					}
					catch (Exception e)
					{
						Log.d(e.ToString());
					}
					finally
					{
						ns.Close();
						client.Close();
						Log.d("Connection closed.");
					}
				}

				listener.Stop();
			});

			m_Worker.Start();
		}

		public void Stop()
		{
			m_Continue = false;
			if (isWorkerAlive()) m_Worker.Abort();
		}

		private void registerCommands()
		{
			m_Commands = new List<CommandBase>();

			m_Commands.Add(new MuteCommand());
			m_Commands.Add(new PasswordCommand());
			m_Commands.Add(new SessionCloseCommand());
			m_Commands.Add(new SessionLockCommand());
			m_Commands.Add(new ShutdownCommand());
			m_Commands.Add(new HibernateCommand());
			m_Commands.Add(new StandbyCommand());
			m_Commands.Add(new ResetCommand());
		}

		private bool isWorkerAlive()
		{
			return m_Worker != null && m_Worker.ThreadState == ThreadState.Running;
		}

		private string GetHello()
		{
			string userAgent = String.Format("ree7, Windows {0}, CLR {1}, Protocol {2}",
				Environment.OSVersion.Version,
				Environment.Version,
				LighthouseProtocolCommands.V);

			return String.Format("HELLO WAKE MY PC {0} ({1})",
							Assembly.GetExecutingAssembly().GetName().Version.ToString(), userAgent);
		}

		private bool OnData(string data, NetworkStream ns)
		{
			if(String.IsNullOrEmpty(data))
			{
				// http://msdn.microsoft.com/fr-fr/library/system.net.sockets.networkstream.read(v=vs.110).aspx
				Log.d("The client has closed the socket, no answer needed.");
				return false;
			}


			bool shouldContinue = true;
			string answer = "KO UNKNOWN";
			CommandBase action = null;

			// Check that the password appended to the command string is correct
			string commandText = null, passwordText = null;

			try
			{
				int separatorPosition = data.LastIndexOf(PASSWORD_SEPARATOR);
				commandText = data.Substring(0, separatorPosition);
				passwordText = data.Substring(separatorPosition+1);
			}
			catch(Exception ex)
			{
				answer = "KO SYNTAX";
				shouldContinue = false;
				Log.d("Error in decoding the command : " + ex.ToString());
				goto SendResponse;
			}

			if (Account.CheckPassword(passwordText, m_Password) == false)
			{
				answer = "KO WRONG_PASSWORD";
				shouldContinue = false;
			}
			else
			{
				// Locate the corresponding command
				foreach (var command in m_Commands)
				{
					if (command.Check(data))
					{
						action = command;
						answer = "OK " + commandText;
						break;
					}
				}
			}

			SendResponse:
			// Send answer
			Byte[] sendBytes = Encoding.UTF8.GetBytes(answer);
			ns.Write(sendBytes, 0, sendBytes.Length);
			ns.Flush();
			Log.d("Sent : " + answer);

			// Do action
			if(action != null) action.Execute(commandText);

			return shouldContinue;
		}

		
	}
}
