using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace RobotPainting
{
    public class Robot
    {
        public Robot(int sizeX, int sizeY)
        {
            width = sizeX;
            height = sizeY;
        }

        int width;
        int height;

        public int X { get; set; }
        public int Y { get; set; }
        public int Angle { get; set; }

        int speedX { get; set; }
        int speedY { get; set; }

        int speed = 5;
        int margin = 40;

        public void Rotate(int angle)
        {
            Angle = angle;
            speedY = (int)(Math.Sin(angle * 2.0 * 3.14 / 360.0) * speed);
            speedX = (int)(Math.Cos(angle * 2.0 * 3.14 / 360.0) * speed);
        }

        public void UpdatePosition()
        {
            if( X < width - margin && X > margin)
                X += speedX;

            if( Y > margin && Y < height - margin)
                Y -= speedY;
        }
    };
    
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Line lineInProgress { get; set; }
        
        public Robot robotPos = new Robot(500,700);

        public bool moveEnable = false;
        public bool rotate = false;
        public bool printEnable = false;
        public bool updatePrint = false;
        
        Thread t = new Thread(new ThreadStart(UdpHelper.Start));

        public MainWindow()
        {
            InitializeComponent();
            UdpHelper.super = this;
            init();
        }

        void init()
        {
            moveEnable = false;
            rotate = false;
            printEnable = false;
            updatePrint = false;
            robotPos.X = 100;
            robotPos.Y = 200;
            robotPos.Rotate(0);
            MoveTo();
            if( printEnable )
                start_line();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if(!UdpHelper._continue)
                t.Start();
        }

        private void start_line()
        {
            {
                Line myLine = new Line();
                myLine.Stroke = System.Windows.Media.Brushes.LightSteelBlue;
                myLine.X1 = robotPos.X;
                myLine.X2 = robotPos.X;
                myLine.Y1 = robotPos.Y;
                myLine.Y2 = robotPos.Y;
                myLine.HorizontalAlignment = HorizontalAlignment.Left;
                myLine.VerticalAlignment = VerticalAlignment.Center;
                myLine.StrokeThickness = 4;

                PaintingZone.Children.Add(myLine);

                lineInProgress = myLine;
            }
        }

        private void update_line(int x, int y)
        {
            lineInProgress.X2 = x;
            lineInProgress.Y2 = y;
        }

        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            for (int i = PaintingZone.Children.Count - 1; i >= 0; i--)
            {
                if (PaintingZone.Children[i] is Line)
                {
                    PaintingZone.Children.RemoveAt(i);
                }
            }
            init();
        }

        private void MoveTo()
        {
            Canvas.SetLeft(Robot, robotPos.X-30);
            Canvas.SetTop(Robot, robotPos.Y-50);
        }


        public void Update()
        {
            if( rotate)
            {
                rotate = false;
                start_line();
            }

            if (updatePrint && printEnable)
                start_line();

            if( moveEnable)
            {
                robotPos.UpdatePosition();
                MoveTo();

                if(printEnable)
                    update_line(robotPos.X, robotPos.Y);
            }

            Positions.Content = "Pos X : " + robotPos.X + " / Y : " + robotPos.Y + " / angle : " + robotPos.Angle + " / Print : " + printEnable;
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            if (UdpHelper._continue)
            {
                UdpHelper.Stop();
                t.Join();
            }
        }
    }
}
