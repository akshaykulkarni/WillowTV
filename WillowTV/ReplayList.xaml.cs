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
using System.Windows.Media;

namespace WillowTV
{
    public partial class ReplayList : PhoneApplicationPage
    {
        public static Grid progressIndicator;
        public static string matchId;
        public static string userId;
        private static Dictionary<int, List<string> > dict;

        private List<string> urls;

        public ReplayList()
        {
            InitializeComponent();
            ReplayList.progressIndicator = progressGrid;
            dict = new Dictionary<int, List<string>>();
            loadReplyList();
        }

        private void loadReplyList()
        {
            urls = new List<string>();
            string wsUrl = WillowUtils.wsBase + "FetchVideoXML.asp?uid=" + userId + "&mid=" + matchId + "&type=replay";
            WillowUtils.startWebRequest(wsUrl,playReplay);
        }

        private void playReplay(string xmlResp)
        {
            try
            {
                XDocument xdoc = XDocument.Load(new StringReader(xmlResp));
                bool isHighLightsPresent = (xdoc.Descendants("items").Count() != 0);
                if (isHighLightsPresent)
                {
                    bool isTestMatch = !(xdoc.Descendants("videoGroup").Count() == 1);
                    if (isTestMatch)
                    {
                        parseTestMatch(xdoc);
                    }
                    else
                    {
                        parseOneDays(xdoc);
                    }
                }
                else
                {
                }
            }
            catch (Exception)
            {
            }
            finally
            {
            }
        }

        private void parseTestMatch(XDocument xdoc)
        {
            IEnumerator<XElement> iEnum = xdoc.Descendants("videoGroup").GetEnumerator();
            int numofTiles=0;
            dict.Clear();
            while (iEnum.MoveNext())
            {
                XElement elem = iEnum.Current;
                string day = getDayofTestMatch(elem.FirstAttribute.ToString());
                IEnumerator<XElement> iEnumGroup = elem.Descendants("item").GetEnumerator();

                //replays for a particular day
                int i = 0;
                urls.Clear();
                while (iEnumGroup.MoveNext())
                {
                    string url = iEnumGroup.Current.Descendants("url").First().Value;
                    string priority = iEnumGroup.Current.Descendants("priority").First().Value;
                    urls.Add(url);
                }
                dict.Add(Convert.ToInt32(day), urls);

                DispatchInvoke(() =>
                {
                    ReplayTile dayNumberReplayTile = new ReplayTile("", "Day " + day);
                    dayNumberReplayTile.SetValue(Canvas.NameProperty, "dayNumberReplayTile" + numofTiles++);
                    dayNumberReplayTile.replayTitle.FontSize = (double)50.0;
                    Brush brush = new SolidColorBrush(Colors.Red);
                    brush.Opacity = (double)0.6;
                    dayNumberReplayTile.LayoutRoot.Background = brush;
                    if (dayNumberReplayTile.replayTilePanel.Children.Count >= 2)
                    {
                        dayNumberReplayTile.replayTilePanel.Children[1].Visibility = System.Windows.Visibility.Collapsed;
                        (dayNumberReplayTile.replayTilePanel.Children[0] as TextBlock).TextAlignment = TextAlignment.Center;
                    }
                    replayList.Children.Add(dayNumberReplayTile);

                    ReplayTile playAllReplayTile = new ReplayTile("","Play All");
                    playAllReplayTile.SetValue(Canvas.NameProperty, "playAllReplayTile" + day);
                    brush = new SolidColorBrush(Colors.Green); brush.Opacity = (double)0.6;
                    playAllReplayTile.LayoutRoot.Background = brush;
                    playAllReplayTile.MatchId = matchId;
                    replayList.Children.Add(playAllReplayTile);

                    for(i=0;i<urls.Count;i++)
                    {
                        ReplayTile replayTile = new ReplayTile(urls[i], "Replay " + (i+1) + " of " + urls.Count);
                        replayTile.SetValue(Canvas.NameProperty, "replayTile" + numofTiles++);
                        replayTile.MatchId = matchId;
                        replayList.Children.Add(replayTile);
                    }
                    ReplayList.progressIndicator.Visibility = System.Windows.Visibility.Collapsed;
                    
                    //Add a dummy Tile
                    /*
                    ReplayTile dummyTile = new ReplayTile("", "");
                    dummyTile.SetValue(Canvas.NameProperty, "dummyTile" + numofTiles++);
                    dummyTile.LayoutRoot.Height = (double)50.0;
                    dummyTile.LayoutRoot.Background = new SolidColorBrush(Colors.White);
                    replayList.Children.Add(dummyTile);
                     */
                }
                );
            }
        }

        private string getDayofTestMatch(string p)
        {
            return p.Split(' ').Last().Trim('"').Last().ToString();
        }

        private void parseOneDays(XDocument xdoc)
        {
            IEnumerator<XElement> iEnum = xdoc.Descendants("item").GetEnumerator();
            int numofTiles = 0,i=0;
            urls.Clear();
            dict.Clear();
            while (iEnum.MoveNext())
            {
                XElement elem = iEnum.Current;
                string url = elem.Descendants("url").First().Value;
                string priority = elem.Descendants("priority").First().Value;
                urls.Add(url);                
            }

            dict.Add(0, urls);
            DispatchInvoke(() =>
            {
                ReplayTile playAllReplayTile = new ReplayTile("", "Play All");
                playAllReplayTile.SetValue(Canvas.NameProperty, "playAllReplayTile0");
                Brush brush = new SolidColorBrush(Colors.Red);
                brush.Opacity = (double)0.6;
                playAllReplayTile.LayoutRoot.Background = brush;
                playAllReplayTile.MatchId = matchId;
                replayList.Children.Add(playAllReplayTile);

                for (i = 0; i < urls.Count; i++)
                {
                    ReplayTile replayTile = new ReplayTile(urls[i], "Replay " + (i + 1) + " of " + urls.Count);
                    replayTile.SetValue(Canvas.NameProperty, "replayTile" + numofTiles++);
                    replayTile.MatchId = matchId;
                    replayList.Children.Add(replayTile);
                }
                ReplayList.progressIndicator.Visibility = System.Windows.Visibility.Collapsed;
            }
            );
        }

        public void DispatchInvoke(Action a)
        {
            if (Dispatcher == null)
                a();
            else
                Dispatcher.BeginInvoke(a);
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {

        }

        private void btnSettings_Click(object sender, EventArgs e)
        {
            OpenSettings();
        }

        private void OpenSettings()
        {
            NavigationService.Navigate(new Uri("/Login.xaml", UriKind.Relative));
        }

        private void ReplayAll_Click(object sender, RoutedEventArgs e)
        {
            WillowVideo.matchInfo = null;
            WillowVideo.playlist = urls;
            MainPage.mainPageInstance.NavigationService.Navigate(new Uri("/WillowVideo.xaml", UriKind.Relative));
        }

        public static List<string> getUrlsForParticularDay(int day)
        {
            if (dict.ContainsKey(day) == true) return dict[day];
            else return null;
        }
    }
}