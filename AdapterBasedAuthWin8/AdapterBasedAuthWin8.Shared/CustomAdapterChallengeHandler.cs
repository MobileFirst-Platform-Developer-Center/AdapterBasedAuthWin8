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

﻿using System;
using System.Collections.Generic;
using System.Text;
using IBM.Worklight;
using System.Diagnostics;
using Newtonsoft.Json.Linq;
using Windows.UI.Xaml.Media.Imaging;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using Windows.UI.Popups;
using Microsoft.VisualBasic;
using Windows.UI;

namespace AdapterBasedAuthWin8
{
    class CustomAdapterChallengeHandler : ChallengeHandler
    {
        MainPage page;
        public CustomAdapterChallengeHandler(MainPage mainPage)
            : base("AuthRealm")
        { page = mainPage; }

        public override void handleChallenge(JObject response)
        {
            Debug.WriteLine("In CustomChallengeHandler handleChallenge");
            MainPage._this.showChallenge();

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

            MainPage._this.hideChallenge();

            submitSuccess(resp);
        }
        public override void onFailure(WLFailResponse failResp)
        {
            Debug.WriteLine("Challenge handler failure ");
        }
    }
}
