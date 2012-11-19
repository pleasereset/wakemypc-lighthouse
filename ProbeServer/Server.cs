using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using ree7.WakeMyPC.ProbeServer.Utils;

namespace ree7.WakeMyPC.ProbeServer
{
    public class Server
    {
        volatile bool _continue;
        Thread worker;
        int port;
        string password;

        public Server(int port, string password)
        {
            // Reads configuration data
            try
            {
                this.port = port;
                this.password = password;
            }
            catch (Exception ex)
            {
                Log.d("Exception at Server() : " + ex);
                throw new InvalidOperationException();
            }

            Log.d("Lighthouse listening on " + port + " with password " + password);
        }

        public bool isRunning
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

            _continue = true;
            worker = new Thread(() =>
            {
                TcpListener listener = new TcpListener(IPAddress.Any, port);

                listener.Start();

                while (_continue)
                {
                    Log.d("Waiting for connection...");
                    TcpClient client = listener.AcceptTcpClient();

                    Log.d("Connection accepted.");
                    NetworkStream ns = client.GetStream();

                    try
                    {
                        // Sends the HELLO message
                        string sWelcomeMsg = "HELLO WAKE MY PC " + "0.1" + " (ree7,Windows)";
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

            worker.Start();
        }

        public void Stop()
        {
            _continue = false;
            if (isWorkerAlive()) worker.Abort();
        }

        private bool isWorkerAlive()
        {
            return worker != null && worker.ThreadState == ThreadState.Running;
        }

        private const string CommandPassword = "PASSWORD/";
        private const string CommandShutdown = "SHUTDOWN/";

        private void OnData(string data, NetworkStream ns)
        {
            string answer = "KO";
            Action action = null;

            data = data.TrimEnd('\0');

            if(data.StartsWith(CommandPassword))
            {
                if(CheckPassword(data.Substring(CommandPassword.Length)))
                {
                    answer = "OK PASSWORD";
                }
            }
            else if (data.StartsWith(CommandShutdown))
            {
                if (CheckPassword(data.Substring(CommandShutdown.Length)))
                {
                    answer = "OK SHUTDOWN";
                    // System call to shutdown
                    action = () =>
                    {
                        Utils.System.Shutdown();
                    };
                }
            }

            // Send answer
            Byte[] sendBytes = Encoding.UTF8.GetBytes(answer);
            ns.Write(sendBytes, 0, sendBytes.Length);

            // Do action
            if(action != null) action();
        }

        private bool CheckPassword(string password)
        {
            Log.d("Checking password : " + password);
            if (password[password.Length - 1] == '\n') password = password.Substring(0, password.Length - 1);
            return this.password == password;
        }
    }
}
