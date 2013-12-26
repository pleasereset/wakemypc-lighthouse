using ree7.WakeMyPC.ProbeServer.Commands;
using ree7.WakeMyPC.ProbeServer.Utils;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading;

namespace ree7.WakeMyPC.ProbeServer
{
	public class Server
	{
		const char PasswordSeparator = '/';

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

				listener.Start();

				while (m_Continue)
				{
					Log.d("Waiting for connection...");
					TcpClient client = listener.AcceptTcpClient();

					Log.d("Connection accepted.");
					NetworkStream ns = client.GetStream();

					try
					{
						// Sends the HELLO message
						string sWelcomeMsg = GetHello();
						Byte[] sendBytes = Encoding.UTF8.GetBytes(sWelcomeMsg);
						ns.Write(sendBytes, 0, sendBytes.Length);
						Log.d("HELLO sent.");

						// Reads NetworkStream into a byte buffer.
						byte[] bytes = new byte[client.ReceiveBufferSize];

						// Read can return anything from 0 to numBytesToRead. 
						// This method blocks until at least one byte is read.
						Log.d("Waiting for data...");
						ns.Read(bytes, 0, (int)client.ReceiveBufferSize);
						string returndata = Encoding.UTF8.GetString(bytes);
						OnData(returndata, ns);
						Log.d("Finished reading data.");

						ns.Close();
						client.Close();
						Log.d("Connection closed.");
					}
					catch (Exception e)
					{
						Log.d(e.ToString());
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
			m_Commands.Add(new PasswordCommand());
			m_Commands.Add(new ShutdownCommand());
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

		private void OnData(string data, NetworkStream ns)
		{
			string answer = "KO UNKNOWN";
			CommandBase action = null;
			data = data.TrimEnd('\0');

			// Check that the password appended to the command string is correct
			int separatorPosition = data.LastIndexOf(PasswordSeparator);
			string commandText = data.Substring(0, separatorPosition);
			string passwordText = data.Substring(separatorPosition);

			if (Account.CheckPassword(passwordText, m_Password) == false)
			{
				answer = "KO WRONG_PASSWORD";
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

			// Send answer
			Byte[] sendBytes = Encoding.UTF8.GetBytes(answer);
			ns.Write(sendBytes, 0, sendBytes.Length);

			// Do action
			if(action != null) action.Execute(commandText);
		}

		
	}
}
