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
    public partial class Statistics : PhoneApplicationPage
    {
        public Statistics()
        {
            InitializeComponent();

            statisticsControl.Loaded += WebBrowser_OnLoaded;
        }

        private void WebBrowser_OnLoaded(object sender, RoutedEventArgs e)
        {
            statisticsControl.Navigate(new Uri("http://www.willow.tv/EventMgmt/YTWidget/Charts/WillowChartsContainer.asp?MatchId=2737", UriKind.Absolute));
        }
    }
}