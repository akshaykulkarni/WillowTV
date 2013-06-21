using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Xml;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.IO;
using System.Xml.Linq;

namespace WillowTV
{
    public partial class Login : PhoneApplicationPage
    {
        public static Grid progressIndicator;
        private AppSettings settings = MainPage.mainPageInstance.appSettings;

        public static int wUserId = 0;

        public Login()
        {
            InitializeComponent();

            MainPage.progressIndicator = progressGrid;
            MainPage.progressIndicator.Visibility = Visibility.Collapsed;

            textBoxUsername.Text = settings.UsernameSetting;
            passwordBoxPassword.Password = settings.PasswordSetting;
        }

        private void btnCheck_Click(object sender, EventArgs e)
        {
            MainPage.progressIndicator.Visibility = Visibility.Visible;

            settings.UsernameSetting = textBoxUsername.Text;
            settings.PasswordSetting = passwordBoxPassword.Password;

            string wsUrl = WillowUtils.wsBase + "/Login.asp?email=" + settings.UsernameSetting + "&password=" + settings.PasswordSetting + "&mget=1";
            WillowUtils.startWebRequest(wsUrl, loginCheckCallback);
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            NavigationService.GoBack();
        }

        public void loginCheckCallback(string xmlResp)
        {
            try
            {
                XDocument xdoc = XDocument.Load(new StringReader(xmlResp));

                string status = xdoc.Descendants("status").First().Value;
                if (status.Equals("success"))
                {
                    int userid = Convert.ToInt32(xdoc.Descendants("userId").First().Value);
                    settings.UserIdSetting = userid;
                    Login.wUserId = userid;

                    DispatchInvoke(() =>
                        {
                            NavigationService.GoBack();
                        }
                    );

                }
                else
                {
                    DispatchInvoke(() =>
                        {
                            errorMsg.Text = "Unable to Login, please try again.";
                        }
                    );
                }
            }
            catch (Exception ex)
            {
                DispatchInvoke(() =>
                    {
                        errorMsg.Text = "Exception: Unable to Login, please try again.";
                    }
                );
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

        public void DispatchInvoke(Action a)
        {
            if (Dispatcher == null)
                a();
            else
                Dispatcher.BeginInvoke(a);
        }
    }
}