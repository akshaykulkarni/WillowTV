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
    public partial class SeriesTile : UserControl
    {
        private Brush tileBackgroundDefaultColor;
        private string seriesId;

        public SeriesTile()
        {
            InitializeComponent();
        }

        public SeriesTile(string sid, string title)
        {
            InitializeComponent();
            sSeriesId = sid;
            sSeriesName = title;
        }

        public string sSeriesId
        {
            get { return seriesId; }
            set { seriesId = value; }
        }

        public string sSeriesName
        {
            get { return seriesTitle.Text; }
            set { seriesTitle.Text = value; }
        }

        private void ucSeriesTile_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            SeriesMatchList.seriesId = seriesId;
            MainPage.mainPageInstance.NavigationService.Navigate(new Uri("/SeriesMatchList.xaml", UriKind.Relative));
        }
    }
}
