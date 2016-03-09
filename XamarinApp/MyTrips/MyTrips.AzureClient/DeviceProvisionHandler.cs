﻿using Microsoft.WindowsAzure.MobileServices;
using MyTrips.Utils;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MyTrips.AzureClient
{
    public class DeviceProvisionHandler
    {
        private static DeviceProvisionHandler handler;

        public string DeviceId
        {
            get; private set;
        }
        
        public string UserId
        {
            get; private set;
        }

        public string HostName
        {
            get; private set;
        }

        public string AccessKey
        {
            get; private set;
        }

        public string DeviceConnectionString
        {
            get
            {
                //Hack for bug #320; hard-coding the AccessKey since we have a provisioned device already that we can re-use
                //When bug #319 is fixed, we should remove this
                this.AccessKey = "PZ4daiMJXIFvuHEOiXqperWQUHcq73fAiskFcjqYX84=";

                string connectionStr = String.Empty;
                if (!String.IsNullOrEmpty(this.AccessKey))
                {
                    connectionStr = String.Format("HostName={0};DeviceId={1};SharedAccessKey={2}", this.HostName, this.DeviceId, this.AccessKey);
                }

                return connectionStr;
            }
        }

        public static DeviceProvisionHandler GetHandler()
        {
            if (handler == null)
            {
                handler = new DeviceProvisionHandler();
                //TODO: Need to get these values from Settings.Current
                handler.UserId = "TestDeviceUserNLHNew";
                handler.DeviceId = "TestDeviceNLHNew";
                handler.HostName = "smarttrips-dev.azure-devices.net";
            }

            return handler;
        }

        public async Task<string> ProvisionDevice()
        {
            //TODO: Need to get these values from Settings.Current
            Dictionary<string, string> myParms = new Dictionary<string, string>();
            myParms.Add("userId", this.UserId);
            myParms.Add("deviceName", this.DeviceId);

            var client = ServiceLocator.Instance.Resolve<IAzureClient>()?.Client as MobileServiceClient;

            try
            {
                var response = await client.InvokeApiAsync("provision", null, HttpMethod.Post, myParms);
                this.AccessKey = response.Value<string>();
            }
            catch(Exception e)
            {
                Logger.Instance.WriteLine("Unable to provision device with IOT Hub: " + e);
            }

            return this.DeviceConnectionString;
        }
    }
}
