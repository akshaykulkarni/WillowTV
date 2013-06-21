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
    public partial class SeriesMatchList : PhoneApplicationPage
    {
        public static Grid progressIndicator;
        public static string seriesId;

        public SeriesMatchList()
        {
            InitializeComponent();
            SeriesMatchList.progressIndicator = progressGrid;
            SeriesMatchList.progressIndicator.Visibility = Visibility.Collapsed;

            loadSeriesMatchList();
        }

        private void loadSeriesMatchList()
        {
            SeriesMatchList.progressIndicator.Visibility = Visibility.Visible;
            string wsUrl = WillowUtils.wsBase + "seriesMatchList.asp?sid=" + seriesId;

            WillowUtils.startWebRequest(wsUrl, renderSeriesMatchList);
        }

        public void renderSeriesMatchList(string xmlResp)
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
                    string team1Name = elem.Descendants("team1").First().Value;
                    string team2Name = elem.Descendants("team2").First().Value;
                    string team1Image = elem.Descendants("team1").First().Attribute("src720").Value;
                    string team2Image = elem.Descendants("team2").First().Attribute("src720").Value;
                    string score = elem.Descendants("score").First().Value;
                    Boolean replaysPresent = elem.Descendants("replays").First().Value.Equals("1");
                    Boolean highlightsPresent = elem.Descendants("highlights").First().Value.Equals("1");
                    Boolean scorecardPresent = elem.Descendants("scorecards").First().Value.Equals("1");

                    score = score.Replace("||||", "\n");
                    score = score.Replace("||", " || ");
                    DispatchInvoke(() =>
                    {
                        MatchTile matchTile = new MatchTile(matchId, mName, team1Image, team2Image, team1Name, team2Name, score, false, replaysPresent, highlightsPresent, scorecardPresent);
                        stackPanelMatchList.Children.Add(matchTile);
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
                    SeriesMatchList.progressIndicator.Visibility = Visibility.Collapsed;
                }
                );
            }
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
    }
}