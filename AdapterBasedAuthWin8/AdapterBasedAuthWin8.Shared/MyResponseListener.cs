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
