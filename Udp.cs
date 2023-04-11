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
                    if (listener.Available > 0) { 
                        byte[] bytes = listener.Receive(ref groupEP);

                        Console.WriteLine($"Received broadcast from {groupEP} :");

                        string mess = Encoding.ASCII.GetString(bytes, 0, bytes.Length);

                        if (mess == "Start") {
                            super.moveEnable = true;
                        }
                        else if (mess == "Stop")
                        {
                            super.moveEnable = false;
                        }
                        else if(mess == "Print")
                        {
                            super.printEnable = true;
                            super.updatePrint = true;
                        }
                        else if(mess == "StopPrint")
                        {
                            super.updatePrint = true;
                            super.printEnable = false;
                        }
                        else if (mess.Contains("Rot"))
                        {
                            int angle = int.Parse(mess.Split(":")[1]);
                            super.robotPos.Rotate(angle);
                            super.rotate = true;
                        }

                        //super.message.Dispatcher.BeginInvoke((Action)(() => super.message.Content = mess));
                    }
                    super.PaintingZone.Dispatcher.BeginInvoke((Action)(() => super.Update()) );

                    System.Threading.Thread.Sleep(100);
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
