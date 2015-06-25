/*
 * COPYRIGHT LICENSE: This information contains sample code provided in source code form. You may copy, modify, and distribute
 * these sample programs in any form without payment to IBM® for the purposes of developing, using, marketing or distributing
 * application programs conforming to the application programming interface for the operating platform for which the sample code is written.
 * Notwithstanding anything to the contrary, IBM PROVIDES THE SAMPLE SOURCE CODE ON AN "AS IS" BASIS AND IBM DISCLAIMS ALL WARRANTIES,
 * EXPRESS OR IMPLIED, INCLUDING, BUT NOT LIMITED TO, ANY IMPLIED WARRANTIES OR CONDITIONS OF MERCHANTABILITY, SATISFACTORY QUALITY,
 * FITNESS FOR A PARTICULAR PURPOSE, TITLE, AND ANY WARRANTY OR CONDITION OF NON-INFRINGEMENT. IBM SHALL NOT BE LIABLE FOR ANY DIRECT,
 * INDIRECT, INCIDENTAL, SPECIAL OR CONSEQUENTIAL DAMAGES ARISING OUT OF THE USE OR OPERATION OF THE SAMPLE SOURCE CODE.
 * IBM HAS NO OBLIGATION TO PROVIDE MAINTENANCE, SUPPORT, UPDATES, ENHANCEMENTS OR MODIFICATIONS TO THE SAMPLE SOURCE CODE.
 */
 
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using IBM.Worklight;
using System.Diagnostics;
using Newtonsoft.Json.Linq;
using Windows.UI.Xaml.Media.Imaging;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using Windows.UI.Popups;
using Microsoft.VisualBasic;
using Windows.UI;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace AdapterBasedAuthWin8
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {

        WLClient wlClient = null;

        public static MainPage _this;
        CustomAdapterChallengeHandler ch;


        public MainPage()
        {
            this.InitializeComponent();
            _this = this;
        }

        public ChallengeHandler getChallengeHandler()
        {
            return ch;
        }

        private void ConnectServer_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                async () =>
                {
                    MainPage._this.Console.Text = "\n\nConnecting to server";
                });
                wlClient = WLClient.getInstance();

                ch = new CustomAdapterChallengeHandler();
                wlClient.registerChallengeHandler((BaseChallengeHandler<JObject>)ch);
                MyResponseListener mylistener = new MyResponseListener(null);
                wlClient.connect(mylistener);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.StackTrace);
            }

        }

        private void invokeProcedure_Click(object sender, RoutedEventArgs e)
        {
            WLProcedureInvocationData invocationData = new WLProcedureInvocationData("AuthAdapter", "getSecretData");
            Object[] parameters = { 0 };
            invocationData.setParameters(parameters);
            MyInvokeListener listener = new MyInvokeListener(this);
            WLClient.getInstance().invokeProcedure(invocationData, listener, new WLRequestOptions());

        }

        private void ClearConsole(object sender, DoubleTappedRoutedEventArgs e)
        {
            Console.Text = "";
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            ((MainPage.CustomAdapterChallengeHandler)MainPage._this.getChallengeHandler()).sendResponse(username.Text, password.Text);
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
            async () =>
            {
                MainPage._this.LoginGrid.Visibility = Visibility.Collapsed;
                MainPage._this.ConnectServer.IsEnabled = true;
                MainPage._this.InvokeProcedure.IsEnabled = true;

            });
            ((MainPage.CustomAdapterChallengeHandler)MainPage._this.getChallengeHandler()).sendResponse("", "");

        }

        public class MyResponseListener : WLResponseListener
        {
            MainPage page;
            public MyResponseListener(MainPage mainPage)
            {
                page = mainPage;
            }

            public void onSuccess(WLResponse resp)
            {
                Debug.WriteLine("Successfully connected to server " + resp.getResponseText());
                CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                async () =>
                {
                    MainPage._this.Console.Text = "\n\nSuccessfully connected to server";
                });
            }

            public void onFailure(WLFailResponse resp)
            {
                Debug.WriteLine("Failure " + resp.getErrorMsg());
                CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                async () =>
                {
                    MainPage._this.Console.Text = resp.getErrorMsg();
                });
            }
        }

        public class MyInvokeListener : WLResponseListener
        {
            MainPage page;
            public MyInvokeListener(MainPage mainPage)
            {
                page = mainPage;
            }

            public void onSuccess(WLResponse resp)
            {
                Debug.WriteLine("Success " + resp.getResponseText());
                JObject respObj = resp.getResponseJSON();
                CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
               async () =>
               {
                   MainPage._this.Console.Text = respObj.ToString();
               });
            }

            public void onFailure(WLFailResponse resp)
            {
                MainPage._this.Console.Text = "\n\nFailure " + resp.getErrorMsg() + ":::" + resp.getErrorCode() + "::" + resp.getResponseText();
                Debug.WriteLine("Failure " + resp.getErrorMsg() + ":::" + resp.getErrorCode() + "::" + resp.getResponseText());
            }
        }

        public class CustomAdapterChallengeHandler : ChallengeHandler
        {
            public CustomAdapterChallengeHandler()
                : base("AuthRealm")
            { }

            public override void handleChallenge(JObject response)
            {
                Debug.WriteLine("In CustomChallengeHandler handleChallenge");

                CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                 async () =>
                 {
                     MainPage._this.LoginGrid.Visibility = Visibility.Visible;
                     MainPage._this.ConnectServer.IsEnabled = false;
                     MainPage._this.InvokeProcedure.IsEnabled = false;
                 });
            }

            public void sendResponse(String username, String password)
            {
                Debug.WriteLine("In CustomChallengeHandler sendResponse");

                WLProcedureInvocationData invData = new WLProcedureInvocationData("AuthAdapter", "submitAuthentication");
                invData.setParameters(new Object[] { username, password });
                submitAdapterAuthentication(invData, new WLRequestOptions());

            }

            public override bool isCustomResponse(WLResponse response)
            {
                Debug.WriteLine("ChallengeHandler isCustomResponse");

                JObject responseJSON = response.getResponseJSON();

                if (response == null || response.getResponseText() == null ||
                    responseJSON["authRequired"] == null || String.Compare(responseJSON["authRequired"].ToString(), "false", StringComparison.OrdinalIgnoreCase) == 0)
                {
                    return false;
                }
                else
                {
                    return true;
                }

            }

            public override void onSuccess(WLResponse resp)
            {
                Debug.WriteLine("Challenge handler success");

                CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                async () =>
                {
                    MainPage._this.LoginGrid.Visibility = Visibility.Collapsed;
                    MainPage._this.ConnectServer.IsEnabled = true;
                    MainPage._this.InvokeProcedure.IsEnabled = true;
                });

                submitSuccess(resp);
            }
            public override void onFailure(WLFailResponse failResp)
            {
                Debug.WriteLine("Challenge handler failure ");
            }

        }

        private void ShowConsole(object sender, TappedRoutedEventArgs e)
        {
            MainPage._this.ConsolePanel.Visibility = Visibility.Visible;
            MainPage._this.InfoPanel.Visibility = Visibility.Collapsed;
            MainPage._this.ConsoleTab.Foreground = new SolidColorBrush(Colors.DodgerBlue);
            MainPage._this.InfoTab.Foreground = new SolidColorBrush(Colors.Gray);
        }

        private void ShowInfo(object sender, TappedRoutedEventArgs e)
        {
            MainPage._this.ConsolePanel.Visibility = Visibility.Collapsed;
            MainPage._this.InfoPanel.Visibility = Visibility.Visible;
            MainPage._this.InfoTab.Foreground = new SolidColorBrush(Colors.DodgerBlue);
            MainPage._this.ConsoleTab.Foreground = new SolidColorBrush(Colors.Gray);
        }

    }
}
