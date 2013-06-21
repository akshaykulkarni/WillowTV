using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.SilverlightMediaFramework.Core.Media;
using System.Collections.ObjectModel;

namespace WillowTV
{
    public partial class WillowVideo : PhoneApplicationPage
    {
        public static MatchInfo matchInfo;
        public static List<string> playlist;

        public WillowVideo()
        {
            InitializeComponent();
            LoadVideo();
        }

        private void LoadVideo()
        {
                if (matchInfo.linkType == WillowLinkType.onDemandVideo)
                {
                    videoPlayer.Playlist[0].MediaSource = new Uri(WillowUtils.getModifiedURL(playlist.First()));
                    for (int i = 1; i < playlist.Count; i++)
                    {
                        PlaylistItem pitem = new PlaylistItem();
                        pitem.DeliveryMethod = Microsoft.SilverlightMediaFramework.Plugins.Primitives.DeliveryMethods.AdaptiveStreaming;
                        pitem.MediaSource = new Uri(WillowUtils.getModifiedURL(playlist[i]));
                        videoPlayer.Playlist.Add(pitem);
                    }
                    //TODO::make playlist
                }
                else if (matchInfo.matchId.CompareTo("999999") == 0)
                {
                    videoPlayer.Playlist[0].MediaSource = new Uri(WillowUtils.getModifiedURL(matchInfo.videoUrl));
                }
                else
                {
                    videoPlayer.Playlist[0].MediaSource = new Uri(matchInfo.videoUrl);
                }
        }
    }
}