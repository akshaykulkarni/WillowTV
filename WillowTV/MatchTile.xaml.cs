using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using System.Windows.Media.Imaging;
using Microsoft.Phone.Shell;
using System.Xml.Linq;
using System.IO;

namespace WillowTV
{
    public partial class MatchTile : UserControl
    {
        public MatchInfo matchInfo;

        private string matchId;

        public MatchTile()
        {
            InitializeComponent();
        }

        public MatchTile(string mid, string mName, string t1Logo, string t2Logo, string t1Name, string t2Name, string score, Boolean isLive, Boolean replayPresent, Boolean highlightPresent, Boolean scorecardPresent)
        {
            InitializeComponent();

            sMatchId = mid;
            sMatchName = mName;
            sTeam1Logo = t1Logo;
            sTeam2Logo = t2Logo;
            sTeam1Name = t1Name;
            sTeam2Name = t2Name;
            sMatchScore = score;

            if (!isLive)
            {
                gridLiveButtons.Visibility = Visibility.Collapsed;
                LayoutRoot.Height = 340;
            }
            else
            {
                gridLiveButtons.Visibility = Visibility.Visible;
            }

            gridScorecards.Visibility = Visibility.Collapsed;
        }

        public string sMatchName
        {
            get { return matchName.Text; }
            set { matchName.Text = value; }
        }

        public string sMatchId
        {
            get { return matchId; }
            set { matchId = value; }
        }

        public string sTeam1Logo
        {
            get { return team1Logo.Source.ToString(); }
            set { team1Logo.Source = new BitmapImage(new Uri(value, UriKind.Absolute)); }
        }

        public string sTeam2Logo
        {
            get { return team2Logo.Source.ToString(); }
            set { team2Logo.Source = new BitmapImage(new Uri(value, UriKind.Absolute)); }
        }

        public string sTeam1Name
        {
            get { return team1Name.Text; }
            set { team1Name.Text = value; }
        }

        public string sTeam2Name
        {
            get { return team2Name.Text; }
            set { team2Name.Text = value; }
        }

        public string sMatchScore
        {
            get { return matchScores.Text; }
            set { matchScores.Text = value; }
        }

        private void loadLiveMatchURL(string userId, string matchId, string sourceType)
        {
            string wsUrl = WillowUtils.wsBase + "FetchVideoXML.asp?uid=" + userId + "&mid=" + matchId + "&type=live&pr=" + sourceType;
            WillowUtils.startWebRequest(wsUrl, playLiveMatch);
        }

        private void playLiveMatch(string xmlResp)
        {
            XDocument xdoc = XDocument.Load(new StringReader(xmlResp));
            IEnumerator<XElement> iEnum = xdoc.Descendants("item").GetEnumerator();
            string liveURL = string.Empty;
            while (iEnum.MoveNext())
            {
                XElement elem = iEnum.Current;
                liveURL = elem.Descendants("url").First().Value;
            }
            WillowVideo.matchInfo = new MatchInfo(matchId, true, WillowLinkType.liveVideo, liveURL);
            Dispatcher.BeginInvoke(() =>
            {
                MainPage.mainPageInstance.NavigationService.Navigate(new Uri("/WillowVideo.xaml", UriKind.Relative));
            });
        }

        private void btnLiveVideo1_Click(object sender, RoutedEventArgs e)
        {
            if (WillowUtils.IsUserLoggedIn() == true)
                loadLiveMatchURL(WillowUtils.getUserId(), matchId,"1");
            else
                MainPage.mainPageInstance.NavigationService.Navigate(new Uri("/Login.xaml", UriKind.Relative));
        }

        private void btnLiveVideo2_Click(object sender, RoutedEventArgs e)
        {
            if (WillowUtils.IsUserLoggedIn() == true)
                loadLiveMatchURL(WillowUtils.getUserId(), matchId, "2");
            else
                MainPage.mainPageInstance.NavigationService.Navigate(new Uri("/Login.xaml", UriKind.Relative));
        }

        private void btnLiveVideo_Click(object sender, RoutedEventArgs e)
        {
            WillowUtils.checkAndRedirectToLogin();
        }

        private void btnReplay_Click(object sender, RoutedEventArgs e)
        {
            if (WillowUtils.IsUserLoggedIn() == true)
            {
                ReplayList.matchId = matchId;
                ReplayList.userId = WillowUtils.getUserId();
                MainPage.mainPageInstance.NavigationService.Navigate(new Uri("/ReplayList.xaml", UriKind.Relative));
            }
            else
            {
                MainPage.mainPageInstance.NavigationService.Navigate(new Uri("/Login.xaml", UriKind.Relative)); 
            }
        }

        private void btnHighlights_Click(object sender, RoutedEventArgs e)
        {
            WillowUtils.checkAndRedirectToLogin();
        }

        private void btnVideoScorecard_Click(object sender, RoutedEventArgs e)
        {
            WillowVideo.matchInfo = new MatchInfo(matchId, true, WillowLinkType.videoScorecard, "");
            WillowUtils.LoadPageFromTiles(WillowVideo.matchInfo);
        }

        private void btnVideoStatistics_Click(object sender, RoutedEventArgs e)
        {
            WillowVideo.matchInfo = new MatchInfo(matchId, true, WillowLinkType.videoStatistics, "");
            WillowUtils.LoadPageFromTiles(WillowVideo.matchInfo);
        }
    }
}
