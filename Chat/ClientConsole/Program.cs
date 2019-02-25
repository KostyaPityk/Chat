using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Threading;


namespace ClientConsole
{
    class Program
    {
        static string userName;
        private const string host = "127.0.0.1";
        private const int port = 8888;
        static TcpClient client;
        static NetworkStream stream;

        static void Main(string[] args)
        {
            while(true)
            { 
                try
                {
                        userName = Guid.NewGuid().ToString();
                        client = new TcpClient();
                        client.Connect(host, port);
                        stream = client.GetStream();

                        string message = userName;
                        byte[] data = Encoding.Unicode.GetBytes(message);
                        stream.Write(data, 0, data.Length);


                        Task receiveThread = new Task(ReceiveMessage);
                        receiveThread.Start();
                        Console.WriteLine("Добро пожаловать, {0}", userName);
                        SendMessage();    
                       
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                finally
                { 
                    Disconnect();
                }
                Thread.Sleep(1000);
            }
        }

        static void SendMessage()
        {

            string message = string.Empty;
            var messages = GetMessages();

            foreach (var item in messages)
            {
                message = item;
                Console.WriteLine(message);
                byte[] data = Encoding.Unicode.GetBytes(message);
                stream.Write(data, 0, data.Length);
                Thread.Sleep(1000);
            }
        }

        static void ReceiveMessage()
        {
            while (true)
            {
                try
                {
                    byte[] data = new byte[64];
                    StringBuilder builder = new StringBuilder();
                    int bytes = 0;
                    do
                    {
                        bytes = stream.Read(data, 0, data.Length);
                        builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
                    }
                    while (stream.DataAvailable);

                    string message = builder.ToString();
                    Console.WriteLine(message);
                }
                catch
                {
                }
            }
        }

        static void Disconnect()
        {
            if (stream != null)
                stream.Close();
            if (client != null)
                client.Close();
        }
        private static List<string> GetMessages(string fileName = "Message.txt")
        {
            List<string> result = new List<string>();
            List<string> messages = new List<string>();

            using (StreamReader file = new StreamReader(fileName))
            {
                while (!file.EndOfStream)
                {
                    messages.Add(file.ReadLine());
                }
            }
            Random random = new Random();
            int countMessage = random.Next(1, messages.Count());

            for (int i = 0; i < countMessage; i++)
            {
                result.Add(messages[random.Next(1, messages.Count())]);
            }
            return result;
        }
    }
}
