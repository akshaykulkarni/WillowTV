using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace WillowTV
{
    public partial class Scorecard : PhoneApplicationPage
    {
        public Scorecard()
        {
            InitializeComponent();

            scorecardControl.Loaded += WebBrowser_OnLoaded;
        }

        private void WebBrowser_OnLoaded(object sender, RoutedEventArgs e)
        {
            scorecardControl.Navigate(new Uri("http://cscore.willow.tv/Match/2725_v.html", UriKind.Absolute));
        }

        private void PhoneApplicationPage_Loaded_1(object sender, RoutedEventArgs e)
        {
        }
    }
}