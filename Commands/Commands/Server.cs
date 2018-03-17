using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.IO;
using System.Net;
using System.Net.Sockets;

namespace Server
{
    class Server
    {
        /// Alle Nachrichten die empfangen worden sind
        public Dictionary<string, List<Tuple<string, string>>> ReceivedMessages { get; private set; }

        /// Alle verbundenen Clients
        public List<ServerClient> Clients { get; private set; }

        /// EndPoint des Servers
        public IPEndPoint EndPoint { get; private set; }

        /// TcpListener zum Warten auf Anfragen
        public TcpListener Listener { get; private set; }

        /// Thread zum Annehmen der Anfragen
        public Thread AcceptThread { get; private set; }

        public Server()
        {
            ReceivedMessages = new Dictionary<string, List<Tuple<string, string>>>();
            EndPoint = new IPEndPoint(IPAddress.Loopback, 1025);
            Clients = new List<ServerClient>();
            Listener = new TcpListener(EndPoint);
            Listener.Start();
            AcceptThread = new Thread(Accept);
            AcceptThread.IsBackground = true;
            AcceptThread.Start();
        }

        /// sofern der Server nicht voll ist werden Verbindungsanfragen angenommen
        private void Accept()
        {
            while (true)
            {
                if (Clients.Count < 10)
                {
                    TcpClient client = Listener.AcceptTcpClient();
                    Clients.Add(new ServerClient(this, client));
                    Console.WriteLine("Client angenommen! IP: " + client.Client.LocalEndPoint); /// Wird angezeigt, wenn ein Client angenommen worden ist
                }
            }
        }

        /// Die Daten von den Clients werden verarbeitet
        public void HandleData(ServerClient client, Command command)
        {
            Console.WriteLine("Daten erhalten! ID: " + command.Identifier); /// Wird angezeigt, wenn ein Daten erhalten worden sind
            if (command.Identifier == "sendMessage")
            {
                List<Tuple<string, string>> messageDatas = new List<Tuple<string, string>>();
                if (ReceivedMessages.ContainsKey(client.Name))
                {
                    messageDatas = ReceivedMessages[client.Name];
                    ReceivedMessages.Remove(client.Name);
                }
                messageDatas.Add(new Tuple<string, string>(command.Arguments[0], command.Arguments[1]));
                ReceivedMessages.Add(client.Name, messageDatas);
                ServerClient receiver = GetClientByName(command.Arguments[0]);
                if (receiver != null)
                {
                    receiver.Send(new Command("sendText", client.Name, command.Arguments[1]));
                }
            }
            else if (command.Identifier == "sendName")
            {
                client.Name = command.Arguments[0];
            }
        }

        //Wird aufgerufen, wenn ein Client den Server verlässt
        public void HandleDisconnect(ServerClient client)
        {
            Clients.Remove(client);
            Console.WriteLine("Client hat den Server verlassen! Name: " + client.Name);         /// Wird angezeigt, wenn ein Client den Server verlässt
        }

        /// Gibt das ServerClient Objekt zum gegebenen Namen zurück
        private ServerClient GetClientByName(string name)
        {
            foreach (ServerClient client in Clients)
            {
                if (client.Name == name)
                {
                    return client;
                }
            }
            return null;
        }
    }
}