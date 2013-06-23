using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using NVControll.Resources;
using Microsoft.Devices.Sensors;
using Microsoft.Xna.Framework;
using System.Net;
using System.Net.Sockets;
using System;



namespace NVControll
{
    public partial class MainPage : PhoneApplicationPage
    {
        Accelerometer accelerometer;
        Socket sock;
        IPAddress ipo;
        IPEndPoint ipEo;

        short x, z, r, c;

        byte[] message = new byte[8];
        // Constructor
        public MainPage()
        {
            InitializeComponent();
        }

        public void initializeValues()
        {
            x = z = r = c = 0;
            sock = new Socket(AddressFamily.InterNetwork,
                         SocketType.Stream,
                         ProtocolType.Tcp);
        }

        int sign(int v)
        {
            return v > 0 ? 1 : (v < 0 ? -1 : 0);
        }

        private void startButton_Click(object sender, RoutedEventArgs e)
        {
            this.initializeValues();
            if (IPTextField.Text.Trim().Length > 0 && IPTextField.Text.Trim().Length < 18)
            {
                const int Port = 33333;
                string IPv4 = IPTextField.Text;

                try
                {
                    ipo = IPAddress.Parse(IPv4);
                }
                catch (SystemException s)
                {
                    MessageBoxResult result = MessageBox.Show("You entered a non valid IP adress, correct format is xxx.yyy.nnn.sss. You must restart the App",
                    "You Must Enter A Valid IP Adress", MessageBoxButton.OK);
                }

                ipEo = new IPEndPoint(ipo, Port);
                SocketAsyncEventArgs socketEventArg = new SocketAsyncEventArgs();
                socketEventArg.RemoteEndPoint = ipEo;

                socketEventArg.Completed += new EventHandler<SocketAsyncEventArgs>(delegate(object s, SocketAsyncEventArgs f)
                {
                    // Retrieve the result of this request
                    if (f.SocketError != SocketError.Success)
                    {
                        statusTextBlock.Text = "Error Socket!";
                    }
                });

                sock.ConnectAsync(socketEventArg);

                if (accelerometer == null)
                {
                    // Instantiate the Accelerometer.
                    accelerometer = new Accelerometer();
                    accelerometer.TimeBetweenUpdates = TimeSpan.FromMilliseconds(80);
                    accelerometer.CurrentValueChanged +=
                         new EventHandler<SensorReadingEventArgs<AccelerometerReading>>(accelerometer_CurrentValueChanged);
                }
                try
                {
                    accelerometer.Start();
                }
                catch (InvalidOperationException ex)
                {
                    statusTextBlock.Text = "unable to start accelerometer.";
                }
            }//if programs runs proper
            else
            {
                MessageBoxResult result = MessageBox.Show("Please provide the host's IP below first",
               "You Must Enter the IP of the Host First", MessageBoxButton.OK);
            }
        }//start button

        void accelerometer_CurrentValueChanged(object sender, SensorReadingEventArgs<AccelerometerReading> e)
        {
            // Call UpdateUI on the UI thread and pass the AccelerometerReading.
            Dispatcher.BeginInvoke(() => UpdateUI(e.SensorReading));
        }

        private void UpdateUI(AccelerometerReading accelerometerReading)
        {
            //  statusTextBlock.Text = "getting data";

            Vector3 acceleration = accelerometerReading.Acceleration;

            // Show the numeric values.
            xTextBlock.Text = "X: " + acceleration.X.ToString("0.00");
            yTextBlock.Text = "Y: " + acceleration.Y.ToString("0.00");
            zTextBlock.Text = "Z: " + acceleration.Z.ToString("0.00");

            // Show the values graphically.
            xLine.X2 = xLine.X1 + acceleration.X * 200;
            yLine.Y2 = yLine.Y1 - acceleration.Y * 200;
            zLine.X2 = zLine.X1 - acceleration.Z * 100;
            zLine.Y2 = zLine.Y1 + acceleration.Z * 100;

            float acc_x, acc_y;
            acc_x = acceleration.X;
            acc_y = acceleration.Y;

            if (acceleration.X > 1)
            {
                acc_x = 1;
            }
            else if (acceleration.X < -1)
            {
                acc_x = -1;
            }

            if (acceleration.Y > 1)
            {
                acc_y = 1;
            }
            else if (acceleration.Y < -1)
            {
                acc_y = -1;
            }

            x = (short)(32767 * acc_y);
            z = (short)(32767 * acc_x);

            //generation messages for neverball
            message[0] = (byte)x;
            message[1] = (byte)(x >> 8);
            message[2] = (byte)z;
            message[3] = (byte)(z >> 8);

            SocketAsyncEventArgs completeArgs = new SocketAsyncEventArgs();
            completeArgs.SetBuffer(message, 0, 8);
            completeArgs.RemoteEndPoint = ipEo;
            sock.SendAsync(completeArgs);

        }

        private void stopButton_Click(object sender, RoutedEventArgs e)
        {
            if (accelerometer != null)
            {
                // Stop the accelerometer.
                accelerometer.Stop();
                statusTextBlock.Text = "accelerometer stopped.";
            }
            sock.Shutdown(SocketShutdown.Both);
        }

        public void IPTextField_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox tb = (TextBox)sender;
            tb.Text = string.Empty;
            tb.GotFocus -= IPTextField_GotFocus;
        }
    }
}