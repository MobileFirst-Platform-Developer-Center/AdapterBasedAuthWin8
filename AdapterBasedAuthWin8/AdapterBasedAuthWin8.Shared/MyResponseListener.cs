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
using Windows.UI.Popups;
using Windows.ApplicationModel.Core;
using System.Diagnostics;
using Newtonsoft.Json.Linq;
using Windows.UI.Core;
using Windows.UI;

namespace AdapterBasedAuthWin8
{
    class MyResponseListener : WLResponseListener
    {
        MainPage page;
        public MyResponseListener(MainPage mainPage)
        {
            Debug.WriteLine("\n In responseListener constructor");
            page = mainPage;
        }

        public void onSuccess(WLResponse resp)
        {
            Debug.WriteLine("Success connecting to server " + resp.getResponseText());
            page.AddTextToConsole("\n\nSuccessfully connected to server");

        }

        public void onFailure(WLFailResponse resp)
        {
            Debug.WriteLine("Failure " + resp.getErrorMsg() + "\n" + resp.getErrorCode());            
            page.AddTextToConsole("\n" + resp.getErrorMsg());
        }
    }
}
