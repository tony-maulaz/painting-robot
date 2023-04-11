using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace RobotPainting
{
    class UdpHelper
    {
        private const int listenPort = 9400;

        public static MainWindow super;

        public static bool _continue { get; private set; }

        public static void Start()
        {
            _continue = true;
            StartListener();
        }

        public static void Stop()
        {
            _continue = false;
        }

        private static void StartListener()
        {
            UdpClient listener = new UdpClient(listenPort);
            IPEndPoint groupEP = new IPEndPoint(IPAddress.Any, listenPort);

            try
            {
                while (_continue)
                {
                    Console.WriteLine("Waiting for broadcast");
                    byte[] bytes = listener.Receive(ref groupEP);

                    Console.WriteLine($"Received broadcast from {groupEP} :");
                    
                    string mess = Encoding.ASCII.GetString(bytes, 0, bytes.Length);
                    super.message.Dispatcher.BeginInvoke((Action)(() => super.message.Content = mess));
                    
                    Console.WriteLine($" {Encoding.ASCII.GetString(bytes, 0, bytes.Length)}");
                }
            }
            catch (SocketException e)
            {
                Console.WriteLine(e);
            }
            finally
            {
                listener.Close();
            }
        }
        
    }
}
