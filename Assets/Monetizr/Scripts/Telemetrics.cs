using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Monetizr.Telemetry
{
    public static class Telemetrics
    {
        private static bool _sessionRegistered;
        private static bool _firstImpressionRegistered;
        private static bool _firstClickRegistered;
        private static DateTime? _sessionStartTime;
        private static DateTime? _firstImpression;
        private static DateTime? _firstClickTime;

        private static MonetizrMonoBehaviour _mmb
        {
            get
            {
                return MonetizrClient.Instance;
            }
        }

        public static void ResetTelemetricsFlags()
        {
            _firstImpressionRegistered = false;
            _sessionRegistered = false;
            _firstClickRegistered = false;
        }

        public static void RegisterProductPageDismissed(string tag)
        {
            if (Application.internetReachability == NetworkReachability.NotReachable)
                return;

            var value = new { trigger_tag = tag };
            var jsonData = JsonUtility.ToJson(value);
            _mmb.StartCoroutine(_mmb.PostData("telemetric/dismiss", jsonData));
        }

        public static void RegisterFirstImpressionProduct()
        {
            if (Application.internetReachability == NetworkReachability.NotReachable)
                return;

            if (_sessionStartTime.HasValue && !_firstImpressionRegistered)
            {
                _firstImpression = _firstImpression ?? DateTime.UtcNow;
                var timespan = new { first_impression_shown = (int)(_firstImpression.Value - _sessionStartTime.Value).TotalSeconds };
                var jsonData = JsonUtility.ToJson(timespan);
                _mmb.StartCoroutine(_mmb.PostData("telemetric/firstimpression", jsonData));
                _firstImpressionRegistered = true;
            }
        }

        public static void RegisterFirstImpressionCheckout()
        {
            if (Application.internetReachability == NetworkReachability.NotReachable)
                return;

            if (_firstClickRegistered || !_firstImpression.HasValue)
                return;

            _firstClickTime = _firstClickTime ?? DateTime.UtcNow;
            var timespan = new { first_impression_checkout = (int)(_firstClickTime.Value - _firstImpression.Value).TotalSeconds };
            var jsonData = JsonUtility.ToJson(timespan);
            _mmb.StartCoroutine(_mmb.PostData("telemetric/firstimpressioncheckout", jsonData));
            _firstClickRegistered = true;
        }

        public static void RegisterEncounter
       (string trigger_type = null, 
            int? completion_status = null, 
            string trigger_tag = null, 
            string level_name = null, 
            string difficulty_level_name = null, 
            int? difficulty_estimation = null)
        {
            if (Application.internetReachability == NetworkReachability.NotReachable)
                return;

            if (string.IsNullOrEmpty(level_name))
                level_name = SceneManager.GetActiveScene().name;

            var encounter = new { trigger_type, completion_status, trigger_tag, level_name, difficulty_level_name, difficulty_estimation };
            var jsonData = JsonUtility.ToJson(encounter);
            _mmb.StartCoroutine(_mmb.PostData("telemetric/encounter", jsonData));
        }

        public static void RegisterSessionStart()
        {
            if (Application.internetReachability == NetworkReachability.NotReachable)
                return;

            if (_sessionRegistered)
                return;

            var session = new Dto.SessionDto()
            {
                device_identifier = SystemInfo.deviceUniqueIdentifier,
                session_start = DateTime.UtcNow
            };

            _sessionStartTime = session.session_start;

            var jsonString = JsonUtility.ToJson(session);
            _mmb.StartCoroutine(_mmb.PostData("telemetric/session", jsonString));
            _sessionRegistered = true;
        }

        public static void RegisterSessionEnd()
        {
            if (Application.internetReachability == NetworkReachability.NotReachable)
                return;

            var session = new Dto.SessionDto()
            {
                device_identifier = SystemInfo.deviceUniqueIdentifier,
                session_start = _sessionStartTime ?? DateTime.UtcNow,
                session_end = DateTime.UtcNow
            };

            var jsonString = JsonUtility.ToJson(session);
            _mmb.StartCoroutine(_mmb.PostData("telemetric/session_end", jsonString));

            ResetTelemetricsFlags();
        }

        public static void SendDeviceInfo()
        {
            if (Application.internetReachability == NetworkReachability.NotReachable)
                return;

            var deviceData = new Dto.DeviceData()
            {
                language = Application.systemLanguage.ToString(),
                device_name = SystemInfo.deviceModel,
                device_identifier = SystemInfo.deviceUniqueIdentifier,
                os_version = SystemInfo.operatingSystem,
                region = GetUserCountryByIp().region
            };

            var jsonString = JsonUtility.ToJson(deviceData);
            _mmb.StartCoroutine(_mmb.PostData("telemetric/devicedata", jsonString));
        }

        public static Dto.IpInfo GetUserCountryByIp()
        {
            if (Application.internetReachability == NetworkReachability.NotReachable)
                return null;

            string IP = new WebClient().DownloadString("http://icanhazip.com");
            Dto.IpInfo ipInfo = new Dto.IpInfo();
            try
            {
                string info = new WebClient().DownloadString("http://ipinfo.io/" + IP);
                ipInfo = JsonUtility.FromJson<Dto.IpInfo>(info);
                RegionInfo myRI1 = new RegionInfo(ipInfo.country);
                var ci = CultureInfo.CreateSpecificCulture(myRI1.TwoLetterISORegionName);
                ipInfo.region = ci.TwoLetterISOLanguageName + "-" + myRI1.TwoLetterISORegionName;
            }
            catch (Exception)
            {
                ipInfo.country = null;
            }
            return ipInfo;
        }
    }
}