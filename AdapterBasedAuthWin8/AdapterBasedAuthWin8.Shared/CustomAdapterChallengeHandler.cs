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
