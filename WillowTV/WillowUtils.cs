using System;
using System.Net;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Controls;
using System.Windows;
using System.Xml.Linq;
using System.Windows.Threading;

namespace WillowTV
{
    public class MatchInfo
    {
        public string matchId;
        public Boolean isLive;
        public WillowLinkType linkType;
        public string videoUrl;

        public MatchInfo(string sMatchId, Boolean bIsLive, WillowLinkType wltLinkType, string vUrl)
        {
            matchId = sMatchId;
            isLive = bIsLive;
            linkType = wltLinkType;
            videoUrl = vUrl;
        }
    }

    public enum WillowLinkType
    {
        liveVideo,
        onDemandVideo,
        videoScorecard,
        videoStatistics,
        videoCommentary
    }

    class WillowUtils
    {
        public static ManualResetEvent allDone = new ManualResetEvent(false);
        const int BUFFER_SIZE = 1024;

        public static Action<string> webReqCallback;
        public static string wsBase = "http://www.willow.tv/EventMgmt/webservices/wp/";

        public static void startWebRequest(string url, Action<string> myCallback)
        {
            try
            {
                webReqCallback = myCallback;

                HttpWebRequest webReq = (HttpWebRequest)WebRequest.Create(url);
                webReq.Method = "GET";
                // Create an instance of the RequestState and assign the previous myHttpWebRequest1
                // object to it's request field.  
                RequestState myRequestState = new RequestState();
                myRequestState.request = webReq;

                IAsyncResult result = (IAsyncResult)webReq.BeginGetResponse(new AsyncCallback(RespCallback), myRequestState);
                Console.WriteLine("in startWebRequest()");
            }
            catch (WebException e)
            {
            }
            catch (Exception e)
            {
            }
        }

        private static void RespCallback(IAsyncResult asyncResult)
        {
            try
            {
                Console.WriteLine("in RespCallback()");
                // State of request is asynchronous.
                RequestState myRequestState = (RequestState)asyncResult.AsyncState;
                HttpWebRequest webReq = myRequestState.request;
                myRequestState.response = (HttpWebResponse)webReq.EndGetResponse(asyncResult);

                // Read the response into a Stream object.
                Stream responseStream = myRequestState.response.GetResponseStream();
                myRequestState.streamResponse = responseStream;

                // Begin the Reading of the contents of the HTML page and print it to the console.
                IAsyncResult asynchronousInputRead = responseStream.BeginRead(myRequestState.BufferRead, 0, BUFFER_SIZE, new AsyncCallback(ReadCallBack), myRequestState);

            }
            catch (WebException e)
            {
            }
        }

        private static void ReadCallBack(IAsyncResult asyncResult)
        {
            try
            {
                string stringContent = "";

                RequestState myRequestState = (RequestState)asyncResult.AsyncState;
                Stream responseStream = myRequestState.streamResponse;
                int read = responseStream.EndRead(asyncResult);
                // Read the HTML page and then do something with it
                if (read > 0)
                {
                    myRequestState.requestData.Append(Encoding.UTF8.GetString(myRequestState.BufferRead, 0, read));
                    IAsyncResult asynchronousResult = responseStream.BeginRead(myRequestState.BufferRead, 0, BUFFER_SIZE, new AsyncCallback(ReadCallBack), myRequestState);
                }
                else
                {
                    if (myRequestState.requestData.Length > 1)
                    {
                        stringContent = myRequestState.requestData.ToString();
                        // do something with the response stream here
                    }

                    responseStream.Close();
                    allDone.Set();

                    webReqCallback(stringContent);
                }

            }
            catch (WebException e)
            {
                // Need to handle the exception
            }

        }

        public static Boolean IsUserLoggedIn()
        {
            return (0 != MainPage.mainPageInstance.appSettings.UserIdSetting);
        }

        public static void checkAndRedirectToLogin()
        {
            if (!IsUserLoggedIn())
            {
                MainPage.mainPageInstance.NavigationService.Navigate(new Uri("/Login.xaml", UriKind.Relative));
                /*if (Application.Current.RootVisual is Page)
                {
                    ((Page)(Application.Current.RootVisual)).NavigationService.Navigate(new Uri("/Login.xaml", UriKind.Relative));
                }*/
            }
        }

        public static void LoadPageFromTiles(MatchInfo mInfo)
        {
            if (WillowLinkType.liveVideo == mInfo.linkType)
            {
                if (WillowUtils.IsUserLoggedIn())
                {
                    WillowVideo.matchInfo = mInfo;
                    MainPage.mainPageInstance.NavigationService.Navigate(new Uri("/WillowVideo.xaml", UriKind.Relative));
                }
                else
                    MainPage.mainPageInstance.NavigationService.Navigate(new Uri("/Login.xaml", UriKind.Relative));
            }
            else if (WillowLinkType.videoScorecard == mInfo.linkType)
            {
                MainPage.mainPageInstance.NavigationService.Navigate(new Uri("/Scorecard.xaml", UriKind.Relative));
            }
            else if (WillowLinkType.videoStatistics == mInfo.linkType)
            {
                MainPage.mainPageInstance.NavigationService.Navigate(new Uri("/Statistics.xaml", UriKind.Relative));
            }
        }

        public static string getModifiedURL(string url)
        {
            string res = string.Empty;
            int l = url.Length;
            for (int i = 0; i < l; i++)
            {
                if (url[i] == '&')
                {
                    res = res + "%26";
                }
                else
                {
                    res = res + url[i];
                }
            }
            return res;
        }


        public static string getUserId()
        {
            if (IsUserLoggedIn() == true) return MainPage.mainPageInstance.appSettings.UserIdSetting.ToString();
            else return string.Empty;
        }
    }

    public class RequestState
    {
        // This class stores the State of the request.
        const int BUFFER_SIZE = 1024;
        public StringBuilder requestData;
        public byte[] BufferRead;
        public HttpWebRequest request;
        public HttpWebResponse response;
        public Stream streamResponse;

        public RequestState()
        {
            BufferRead = new byte[BUFFER_SIZE];
            requestData = new StringBuilder("");
            request = null;
            streamResponse = null;
        }
    }
}
