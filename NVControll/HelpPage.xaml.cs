using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Tasks;

namespace NVControll
{
    public partial class HelpPage : PhoneApplicationPage
    {
        public HelpPage()
        {
            InitializeComponent();
        }

        private void Send_Feedback(object sender, RoutedEventArgs e)
        {
            var task = new EmailComposeTask { To = "dweisden@hotmail.com", Subject = "Feedback for Smartball" };
            task.Show();
        }

        private void Browse_Patch(object sender, RoutedEventArgs e)
        {
             WebBrowserTask webBrowserTask = new WebBrowserTask();
             webBrowserTask.Uri = new Uri("https://github.com/jfietkau/neverball-fbiuhh");
             webBrowserTask.Show();            
        }
    }
}