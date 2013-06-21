using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Windows.Media;

namespace WillowTV
{
    public partial class ReplayTile : UserControl
    {
        private string replayUrl;
     
        public ReplayTile()
        {
            InitializeComponent();
        }

        public ReplayTile(string url, string priority)
        {
            InitializeComponent();
            ReplayPriority = priority; ;
            ReplayURL = url;
        }

        public string ReplayURL
        {
            get { return replayUrl; }
            set { replayUrl = value; }
        }

        public string ReplayPriority
        {
            get { return replayTitle.Text; }
            set { replayTitle.Text = value; }
        }

        private string _matchId = string.Empty;
        public string MatchId
        {
            get { return _matchId; }
            set { _matchId = value; }
        }

        private void UserControl_Tap_1(object sender, System.Windows.Input.GestureEventArgs e)
        {
            bool isdayTile = (sender as ReplayTile).Name.Contains("Day");
            if (isdayTile) return;
            bool isreplayAllTile = (sender as ReplayTile).Name.Contains("playAll");
            if (isreplayAllTile)
            {
                string tileName = ((sender as ReplayTile).Name);
                int dayNumber = (tileName[tileName.Length-1]-'0');
                List<string> replayUrls = ReplayList.getUrlsForParticularDay(dayNumber);
                if (replayUrls != null)
                {
                    WillowVideo.matchInfo = new MatchInfo(MatchId, false, WillowLinkType.onDemandVideo, replayUrls.First());
                    WillowVideo.playlist = replayUrls;
                    MainPage.mainPageInstance.NavigationService.Navigate(new Uri("/WillowVideo.xaml", UriKind.Relative));
                }
            }
            else
            {
                if (string.IsNullOrEmpty(replayUrl) == false)
                {
                    WillowVideo.matchInfo = new MatchInfo(MatchId, false, WillowLinkType.onDemandVideo, replayUrl);
                    WillowVideo.playlist = new List<string>();
                    WillowVideo.playlist.Add(replayUrl);
                    MainPage.mainPageInstance.NavigationService.Navigate(new Uri("/WillowVideo.xaml", UriKind.Relative));
                }
            }
        }
    }
}
