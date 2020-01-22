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
            Console.WriteLine("Hello World!");
            var endPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 9876);
            SocketServer server = new SocketServer(endPoint);
            server.HelloMessage += HelloMessage;
            server.Start();
            Console.ReadKey();
        }

        private static void HelloMessage(Socket socket, string content)
        {
            Console.WriteLine(content);
            socket.Send(Encoding.UTF8.GetBytes(
                MessageType.Hello.ToString() + "|I love U!"));
        }
    }
}
