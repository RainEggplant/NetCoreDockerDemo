using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace NetCoreDockerDemo
{
    /// <summary>
    /// 消息类型。
    /// </summary>
    public enum MessageType
    {
        Hello, Error
    }

    public class SocketServer : IDisposable
    {
        private Socket listener;
        private Thread listenThread;

        /// <summary>
        /// 获取 IP 与端口号。
        /// </summary>
        public IPEndPoint EndPoint { get; }

        /// <summary>
        /// 初始化 SocketConnection 类的新实例。
        /// </summary>
        /// <param name="endPoint">监听的 IP 和 端口号</param>
        public SocketServer(IPEndPoint endPoint)
        {
            EndPoint = endPoint;
        }


        /// <summary>
        /// 启动服务端。
        /// </summary>
        public void Start()
        {
            listenThread = new Thread(new ThreadStart(Listen))
            {
                IsBackground = true
            };
            listenThread.Start();
        }

        /// <summary>
        /// 关闭服务端。
        /// </summary>
        public void Stop()
        {
            if (listener != null)
            {
                listener.Close();
                listener = null;
            }
            if (listenThread != null)
            {
                if (listenThread.IsAlive) listenThread.Abort();
                listenThread = null;
            }
        }

        /// <summary>
        /// 监听线程的入口函数。
        /// </summary>
        private void Listen()
        {
            listener = new Socket(AddressFamily.InterNetwork,
                SocketType.Stream, ProtocolType.Tcp);

            try
            {
                listener.Bind(EndPoint);
                listener.Listen(10);
                
                // 开始监听
                while (true)
                {
                    Socket handler = listener.Accept();
                    var task = new Task(() => HandleRequest(handler));
                    task.Start();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        /// <summary>
        /// 处理接收到的请求。
        /// </summary>
        private void HandleRequest(Socket handler)
        {
            try
            {
                byte[] bytes = new byte[1024];
                int bytesRec = handler.Receive(bytes);
                string data = Encoding.UTF8.GetString(bytes, 0, bytesRec);
                string[] tokens = data.Trim().Split('|');
                switch (tokens[0])
                {
                    case "Hello":
                        HelloMessage(handler, tokens[1]);
                        break;
                    default:
                        byte[] msg = Encoding.UTF8.GetBytes(
                            MessageType.Error.ToString() + "|Bad request");
                        handler.Send(msg);
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                handler.Shutdown(SocketShutdown.Both);
                handler.Close();
            }
        }

        #region IDisposable Support
        private bool disposedValue = false;  // 要检测冗余调用

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // 释放托管状态(托管对象)。                   
                }

                Stop();
                disposedValue = true;
            }
        }

        // the finalizer.
        ~SocketServer()
        {
            Dispose(false);
        }

        /// <summary>
        /// 关闭连接并释放由 SocketConnection 类使用的所有资源。
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion

        #region 委托与事件
        /// <summary>
        /// 代表用于处理 HelloMessage 事件的方法。
        /// </summary>
        /// <param name="content">打招呼内容</param>
        public delegate void HelloMessageEventHandler(Socket socket, string content);

        /// <summary>
        /// 当接收到远程的 Hello 指令时发生。
        /// </summary>
        public event HelloMessageEventHandler HelloMessage;
        #endregion
    }
}
