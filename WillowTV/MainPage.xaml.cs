using System;
using System.Net;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Xml.Linq;
using System.IO;

namespace WillowTV
{
    public partial class MainPage : PhoneApplicationPage
    {
        public static Grid progressIndicator;
        public static MatchInfo matchInfo;
        public static MainPage mainPageInstance;
        public AppSettings appSettings = new AppSettings();

        // Constructor
        public MainPage()
        {
            InitializeComponent();

            MainPage.progressIndicator = progressGrid;
            MainPage.progressIndicator.Visibility = Visibility.Collapsed;

            MainPage.mainPageInstance = this;
        }

        /*private void LiveTile_Tap(object sender, RoutedEventArgs e)
        {
            try
            {
                NavigationService.Navigate(new Uri("/Scorecard.xaml", UriKind.Relative));
            }
            catch (Exception ex)
            {
            }
        }*/

        public void renderLiveVideoTile(string xmlResp)
        {
            try
            {
                //xmlResp = "<?xml version='1.0' encoding='UTF-8'?><windowsphone><items><item><matchId>2738</matchId><matchName>Qualifier 1 - Chennai Super Kings vs Mumbai Indians. Feroz Shah Kotla, Delhi</matchName><matchDate>2013-05-21 14:20:00</matchDate><team1 src720='http://aimages.willow.tv/teamLogos/chennaisuperkings.png'>CSK</team1><team2 src720='http://aimages.willow.tv/teamLogos/mumbaiindians.png'>MI</team2><score>CSK 192/1(20.0)||MI 144/10(18.4)||||CSK won by 48 runs</score></item><item><matchId>2738</matchId><matchName>Qualifier 1 - Chennai Super Kings vs Mumbai Indians. Feroz Shah Kotla, Delhi</matchName><matchDate>2013-05-21 14:20:00</matchDate><team1 src720='http://aimages.willow.tv/teamLogos/australia.png'>Australia</team1><team2 src720='http://aimages.willow.tv/teamLogos/india.png'>India</team2><score>CSK 192/1(20.0)||MI 144/10(18.4)||||CSK won by 48 runs</score></item></items></windowsphone>";
                XDocument xdoc = XDocument.Load(new StringReader(xmlResp));

                IEnumerator<XElement> iEnum = xdoc.Descendants("item").GetEnumerator();
                while (iEnum.MoveNext())
                {
                    XElement elem = iEnum.Current;
                    string matchId = elem.Descendants("matchId").First().Value;
                    string mName = elem.Descendants("matchName").First().Value;
                    if (matchId.Equals("999999"))
                    {
                        DispatchInvoke(() =>
                            {
                                WillowTile willowTile = new WillowTile();
                                LiveTileStackPanel.Children.Add(willowTile);
                            }
                        );
                        break;
                    }

                    string team1Name = elem.Descendants("team1").First().Value;
                    string team2Name = elem.Descendants("team2").First().Value;
                    string team1Image = elem.Descendants("team1").First().Attribute("src720").Value;
                    string team2Image = elem.Descendants("team2").First().Attribute("src720").Value;
                    string score = elem.Descendants("score").First().Value;
                    //score = "CSK 192/1(20.0)||MI 144/10(18.4)||||CSK won by 48 runs";
                    score = score.Replace("||||", "\n");
                    score = score.Replace("||", " || ");
                    if (score.CompareTo("undefined") == 0) score = string.Empty;

                    DispatchInvoke(() =>
                        {
                            //TODO: write the logic to show correct number of sources on the main page
                            MatchTile matchTile = new MatchTile(matchId, mName, team1Image, team2Image, team1Name, team2Name, score, true, false, false, false);
                            LiveTileStackPanel.Children.Add(matchTile);
                        }
                    );

                }
            }
            catch (Exception ex)
            {
            }
            finally
            {
                DispatchInvoke(() =>
                    {
                        MainPage.progressIndicator.Visibility = Visibility.Collapsed;
                    }
                );
            }
        }

        // Load data for the ViewModel Items
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            try
            {
                if (0 == LiveTileStackPanel.Children.Count)
                {
                    MainPage.progressIndicator.Visibility = Visibility.Visible;
                    string wsUrl = WillowUtils.wsBase + "/LiveMatchList.asp";

                    WillowUtils.startWebRequest(wsUrl, renderLiveVideoTile);
                }
            }
            catch (Exception ex)
            {
            }
        }



        private void Panorama_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                switch (appPanorama.SelectedIndex)
                {
                    case 0:
                        //Live Now page
                        break;
                    case 1:
                        if (0 == ArchiveTileStackPanel.Children.Count)
                        {
                            fillArchivePanorama();
                        }
                        //Archives
                        break;
                    case 2:
                        //fetch trending videos
                        fetchTrendingVideos();
                        break;
                }
            }
            catch (Exception ex)
            {
            }
        }

        private void fetchTrendingVideos()
        {
        }

        /// <summary>
        /// Get Archives from webservice and fill Panorama
        /// </summary>
        private void fillArchivePanorama()
        {
            try
            {
                MainPage.progressIndicator.Visibility = Visibility.Visible;
                string wsUrl = WillowUtils.wsBase + "/seriesList.asp";

                WillowUtils.startWebRequest(wsUrl, renderArchiveSeries);
            }
            catch (Exception ex)
            {
            }
        }

        /// <summary>
        /// Render Archives tab with response from webservice
        /// </summary>
        /// <param name="xmlResp"></param>
        public void renderArchiveSeries(string xmlResp)
        {
            try
            {
                //xmlResp = "<?xml version='1.0' encoding='UTF-8'?><windowsphone><items><item><matchId>2738</matchId><matchName>Qualifier 1 - Chennai Super Kings vs Mumbai Indians. Feroz Shah Kotla, Delhi</matchName><matchDate>2013-05-21 14:20:00</matchDate><team1 src720='http://aimages.willow.tv/teamLogos/chennaisuperkings.png'>CSK</team1><team2 src720='http://aimages.willow.tv/teamLogos/mumbaiindians.png'>MI</team2><score>CSK 192/1(20.0)||MI 144/10(18.4)||||CSK won by 48 runs</score></item><item><matchId>2738</matchId><matchName>Qualifier 1 - Chennai Super Kings vs Mumbai Indians. Feroz Shah Kotla, Delhi</matchName><matchDate>2013-05-21 14:20:00</matchDate><team1 src720='http://aimages.willow.tv/teamLogos/australia.png'>Australia</team1><team2 src720='http://aimages.willow.tv/teamLogos/india.png'>India</team2><score>CSK 192/1(20.0)||MI 144/10(18.4)||||CSK won by 48 runs</score></item></items></windowsphone>";
                XDocument xdoc = XDocument.Load(new StringReader(xmlResp));

                IEnumerator<XElement> iEnum = xdoc.Descendants("item").GetEnumerator();
                int objCount=0;
                while (iEnum.MoveNext())
                {
                    XElement elem = iEnum.Current;
                    string seriesId = elem.Descendants("sid").First().Value;
                    string title = elem.Descendants("title").First().Value;
                    
                    DispatchInvoke(() =>
                    {
                        SeriesTile seriesTile = new SeriesTile(seriesId, title);
                        seriesTile.SetValue(Canvas.NameProperty, "seriesTile" + objCount++);
                        ArchiveTileStackPanel.Children.Add(seriesTile);
                    }
                    );
                }
            }
            catch (Exception ex)
            {
            }
            finally
            {
                DispatchInvoke(() =>
                {
                    MainPage.progressIndicator.Visibility = Visibility.Collapsed;
                }
                );
            }
        }

        private void btnSettings_Click(object sender, EventArgs e)
        {
            OpenSettings();
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {

        }

        private void OpenSettings()
        {
            NavigationService.Navigate(new Uri("/Login.xaml", UriKind.Relative));
        }

        public void DispatchInvoke(Action a)
        {
            if (Dispatcher == null)
                a();
            else
                Dispatcher.BeginInvoke(a);
        }
    }
}