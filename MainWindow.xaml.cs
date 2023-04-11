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
    public class Position
    {
        public double X { get; set; }
        public double Y { get; set; }
    };
    
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Line lineInProgress { get; set; }
        int a = 0;

        Position robotPos;
        
        Thread t = new Thread(new ThreadStart(UdpHelper.Start));

        public MainWindow()
        {
            InitializeComponent();
            UdpHelper.super = this;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            t.Start();
            MoveTo(Robot, 300, 200);
        }

        private void Button_Click1(object sender, RoutedEventArgs e)
        {
            if (a == 0)
                start_line(10, 10);
            else
                update_line(a*20, a*40);

            if( a == 3)
            {
                start_line(50, 50);
            }
            a++;
        }

        private void start_line(int x, int y)
        {
            {
                Line myLine = new Line();
                myLine.Stroke = System.Windows.Media.Brushes.LightSteelBlue;
                myLine.X1 = x;
                myLine.X2 = x;
                myLine.Y1 = y;
                myLine.Y2 = y;
                myLine.HorizontalAlignment = HorizontalAlignment.Left;
                myLine.VerticalAlignment = VerticalAlignment.Center;
                myLine.StrokeThickness = 2;

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
        }

        private void MoveTo(Image target, double newX, double newY)
        {
            Canvas.SetLeft(target, newX);
            Canvas.SetTop(target, newY);
        }
    }
}
