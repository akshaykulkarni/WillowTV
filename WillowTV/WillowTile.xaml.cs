using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Xml.Linq;
using System.IO;

namespace WillowTV
{
    public partial class WillowTile : UserControl
    {
        public WillowTile()
        {
            InitializeComponent();
        }

        private void btnLiveVideo_Click(object sender, RoutedEventArgs e)
        {
            if (WillowUtils.IsUserLoggedIn())
                loadLiveVideo(WillowUtils.getUserId());
            else
                MainPage.mainPageInstance.NavigationService.Navigate(new Uri("/Login.xaml", UriKind.Relative));
        }

        private void loadLiveVideo(string userId)
        {
            string wsUrl = WillowUtils.wsBase + "FetchVideoXML.asp?uid=" + userId + "&mid=999999";
            WillowUtils.startWebRequest(wsUrl, playLiveVideo);
        }

        private void playLiveVideo(string xmlResp)
        {
            XDocument xdoc = XDocument.Load(new StringReader(xmlResp));
            IEnumerator<XElement> iEnum = xdoc.Descendants("item").GetEnumerator();
            string liveURL = string.Empty;
            while (iEnum.MoveNext())
            {
                XElement elem = iEnum.Current;
                liveURL = elem.Descendants("url").First().Value;
            }
            WillowVideo.matchInfo = new MatchInfo("999999",true, WillowLinkType.liveVideo, liveURL);
            Dispatcher.BeginInvoke(() =>
            {
                MainPage.mainPageInstance.NavigationService.Navigate(new Uri("/WillowVideo.xaml", UriKind.Relative));
            });
        }
    }
}
