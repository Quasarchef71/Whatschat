using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.IO;
using System.Net.Sockets;

namespace Server
{
    class ServerClient
    {
        /// Reader des Clients
        public StreamReader Reader { get; private set; }

        /// Writer des Clients
        public StreamWriter Writer { get; private set; }

        /// Network Client
        public TcpClient Client { get; private set; }

        /// Name des Clients
        public string Name { get; set; }

        /// Thread welcher auf Data wartet
        public Thread ReceiveThread { get; private set; }

        private Server server;

        public ServerClient(Server server, TcpClient client)
        {
            this.server = server;
            Client = client;
            Reader = new StreamReader(client.GetStream());
            Writer = new StreamWriter(client.GetStream());
            ReceiveThread = new Thread(ReceiveData);
            ReceiveThread.IsBackground = true;
            ReceiveThread.Start();
        }

        /// Wartet auf Daten und bearbeitet diese
        private void ReceiveData()
        {
            try
            {
                while (true)
                {
                    string receivedData = null;
                    while ((receivedData = Reader.ReadLine()) != null)
                    {
                        Command command = Command.ToCommand(receivedData);
                        server.HandleData(this, command);
                    }
                }
            }
            catch
            {
                server.HandleDisconnect(this);
            }
        }

        /// Sendet Daten an den Client
        public void Send(Command command)
        {
            Writer.WriteLine(command.ToString());
            Writer.Flush();
        }

        /// Stoppt den Client und räumt auf
        public void Close()
        {
            Client.GetStream().Close();
            Client.Close();
            ReceiveThread.Interrupt();
            Reader.Close();
            Writer.Close();
        }
    }
}

