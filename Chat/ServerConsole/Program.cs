using System;
using System.Net.Sockets;
using System.Threading.Tasks;
using ManageChat;

namespace ServerConsole
{
    class Program
    {
        static Server server;
        static Task listenThread; 
        static void Main(string[] args)
        {
            try
            {
                server = new Server();
                listenThread = new Task(server.Listen);
                listenThread.Start(); 
            }
            catch (Exception ex)
            {
                server.Disconnect();
                Console.WriteLine(ex.Message);
            }
            listenThread.Wait();
        }
    }
}
