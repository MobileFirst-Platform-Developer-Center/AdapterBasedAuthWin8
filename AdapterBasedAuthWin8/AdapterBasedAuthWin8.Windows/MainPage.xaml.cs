/**
* Copyright 2015 IBM Corp.
*
* Licensed under the Apache License, Version 2.0 (the "License");
* you may not use this file except in compliance with the License.
* You may obtain a copy of the License at
*
* http://www.apache.org/licenses/LICENSE-2.0
*
* Unless required by applicable law or agreed to in writing, software
* distributed under the License is distributed on an "AS IS" BASIS,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the License for the specific language governing permissions and
* limitations under the License.
*/
 
﻿using System;
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
using Windows.UI.Popups;
using Windows.ApplicationModel.Core;
using System.Diagnostics;
using Newtonsoft.Json.Linq;
using Windows.UI.Core;
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

                ch = new CustomAdapterChallengeHandler(_this);
                wlClient.registerChallengeHandler((BaseChallengeHandler<JObject>)ch);

                MyResponseListener mylistener = new MyResponseListener(this);
                wlClient.connect(mylistener);
            }
            catch (Exception ex)
            { Debug.WriteLine(ex.StackTrace); }
        }


        private void invokeProcedure_Click(object sender, RoutedEventArgs e)
        {
            WLResourceRequest adapter = new WLResourceRequest("/adapters/AuthAdapter/getSecretData", "GET");

            Object[] parameters = { 0 };
            MyInvokeListener listener = new MyInvokeListener(this);
            adapter.send(listener);
            
        }

        public void AddTextToConsole(String consoleText)
        {
            CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                 async () =>
                 {
                     MainPage._this.Console.Text = consoleText;

                 });
        }

        private void ClearConsole(object sender, DoubleTappedRoutedEventArgs e)
        {
            Console.Text = "";
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            ((CustomAdapterChallengeHandler)MainPage._this.getChallengeHandler()).sendResponse(username.Text, password.Text);
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
            ((CustomAdapterChallengeHandler)MainPage._this.getChallengeHandler()).sendResponse("", "");
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

        public void showChallenge()
        {
            CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                 async () =>
                 {
                     MainPage._this.LoginGrid.Visibility = Visibility.Visible;
                     MainPage._this.ConnectServer.IsEnabled = false;
                     MainPage._this.InvokeProcedure.IsEnabled = false;
                 });
        }

        public void hideChallenge()
        {
            CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                async () =>
                {
                    MainPage._this.LoginGrid.Visibility = Visibility.Collapsed;
                    MainPage._this.ConnectServer.IsEnabled = true;
                    MainPage._this.InvokeProcedure.IsEnabled = true;
                });
        }
    }
}
