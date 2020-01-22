using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace NetCoreDockerDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Program started.");
            var endPoint = new IPEndPoint(IPAddress.Parse("0.0.0.0"), 3000);
            SocketServer server = new SocketServer(endPoint);
            server.HelloMessage += HelloMessage;
            server.Start();
            Console.ReadKey();
        }

        private static void HelloMessage(Socket socket, string content)
        {
            Console.WriteLine(content + ", I love U!");
            socket.Send(Encoding.UTF8.GetBytes(
                MessageType.Hello.ToString() + "|I love U!"));
        }
    }
}
