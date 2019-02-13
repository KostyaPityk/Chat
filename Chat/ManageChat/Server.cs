using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ManageChat
{
    public class Server
    {
        static TcpListener tcpListener;
        List<Client> clients = new List<Client>();
        public List<string> collectionsMessage = new List<string>(10);

        public void AddConnection(Client clientObject)
        {
            clients.Add(clientObject);
        }
        public void RemoveConnection(string id)
        {
 
            Client client = clients.FirstOrDefault(c => c.Id == id);

            if (client != null)
                clients.Remove(client);
        }

        public void Listen()
        {
            try
            {
                tcpListener = new TcpListener(IPAddress.Any, 8888);
                tcpListener.Start();
                Console.WriteLine("Сервер запущен. Ожидание подключений...");

                while (true)
                {
                    TcpClient tcpClient = tcpListener.AcceptTcpClient();

                    Client clientObject = new Client(tcpClient, this);

                    if (collectionsMessage.Count != 0)
                    {
                        ShowLastMessageForNewClient(clientObject.Id);
                    }

                    Task Start = new Task(clientObject.Process);
                    Start.Start();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Disconnect();
            }
        }

        public void ShowLastMessageForNewClient(string Id)
        {
            byte[] data = Encoding.Unicode.GetBytes(GetLastMessage());

            for (int i = 0; i < clients.Count; i++)
            {
                if (clients[i].Id == Id) 
                {
                    clients[i].Stream.Write(data, 0, data.Length); 
                }
            }
        }

        public void BroadcastMessage(string message, string id)
        {
            byte[] data = Encoding.Unicode.GetBytes(message);
            for (int i = 0; i < clients.Count; i++)
            {
                if (clients[i].Id != id) 
                {
                    clients[i].Stream.Write(data, 0, data.Length); 
                }
            }
        }

        public void Disconnect()
        {
            tcpListener.Stop(); 

            for (int i = 0; i < clients.Count; i++)
            {
                clients[i].Close(); 
            }
            Environment.Exit(0); 
        }

        public string GetLastMessage()
        {
            string result = string.Empty;

            foreach(var i in this.collectionsMessage)
            {
                result += i + "\n";
            }

            return result;
        }
    }
}
