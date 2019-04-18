﻿using Assets.Monetizr.Dto;
using Newtonsoft.Json;
using System;
using System.Globalization;
using System.Net;
using UnityEngine;

public class MonetizrMonoBehaviour : MonoBehaviour
{
    [Header("Monetizr Plugin Settings")]
    public string AccessToken;
    public string MerchandiseId;
    public Canvas RootCanvas;
    public ProductPageScript Prefab;

    private string _baseUrl = "https://api3.themonetizr.com/api/";
    private bool _sessionRegistered;
    private bool _firstImpressionRegistered;
    private bool _firstClickRegistered;
    private DateTime? _sessionStartTime;
    private DateTime? _firstImpression;
    private DateTime? _firstClickTime;
    private string _language;

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
    public async void Init(string accessToken, string merchandiseId)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
    {
        AccessToken = accessToken;
        MerchandiseId = merchandiseId;
        DisableFlags();
        RegisterSessionStart();
        SendDeviceInfo();
    }

    internal void RegisterProductPageDismissed(string tag)
    {
        var value = new { trigger_tag = tag };
        var jsonData = JsonConvert.SerializeObject(value);
        PostData("telemetric/dismiss", jsonData);
    }

    internal void RegisterClick()
    {
        if (_firstClickRegistered || !_firstImpression.HasValue)
            return;

        _firstClickTime = _firstClickTime ?? DateTime.UtcNow;
        var timespan = new { first_impression_checkout = (int)(_firstClickTime.Value - _firstImpression.Value).TotalSeconds };
        var jsonData = JsonConvert.SerializeObject(timespan);
        if (PostData("telemetric/firstimpressioncheckout", jsonData))
        {
            _firstClickRegistered = true;
        }

    }

    public void SetLanguage(string language)
    {
        _language = language;
    }

    public void ShowProductForTag(string tag)
    {
        if (string.IsNullOrEmpty(_language))
            _language = "en_En";

        var productInfo = GetData<ProductInfo>($"products/tag/{tag}?language={_language}");
        var page = Instantiate(Prefab, RootCanvas.transform, false);
        page.Init(productInfo, tag);
        if (_sessionStartTime.HasValue && !_firstImpressionRegistered)
        {
            _firstImpression = _firstImpression ?? DateTime.UtcNow;
            var timespan = new { first_impression_shown = (int)(_firstImpression.Value - _sessionStartTime.Value).TotalSeconds };
            var jsonData = JsonConvert.SerializeObject(timespan);
            if (PostData("telemetric/firstimpression", jsonData))
            {
                _firstImpressionRegistered = true;
            }
        }
    }



#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
    private async void RegisterSessionStart()
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
    {
        if (_sessionRegistered)
            return;

        var session = new SessionDto()
        {
            device_identifier = SystemInfo.deviceUniqueIdentifier,
            session_start = DateTime.UtcNow
        };

        _sessionStartTime = session.session_start;

        var jsonString = JsonConvert.SerializeObject(session);
        if (PostData("telemetric/session", jsonString))
        {
            _sessionRegistered = true;
        }

    }

    private void Start()
    {
        Init(AccessToken, MerchandiseId);
    }

    void OnApplicationQuit()
    {
        var session = new SessionDto()
        {
            device_identifier = SystemInfo.deviceUniqueIdentifier,
            session_start = _sessionStartTime ?? DateTime.UtcNow,
            session_end = DateTime.UtcNow
        };

        var jsonString = JsonConvert.SerializeObject(session);
        PostData("telemetric/session_end", jsonString);

        DisableFlags();

    }

    private void DisableFlags()
    {
        _firstImpressionRegistered = false;
        _sessionRegistered = false;
        _firstClickRegistered = false;
    }

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
    private async void SendDeviceInfo()
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
    {
        if (Application.internetReachability == NetworkReachability.NotReachable)
            return;

        var deviceData = new DeviceData()
        {
            language = Application.systemLanguage.ToString(),
            device_name = SystemInfo.deviceModel,
            device_identifier = SystemInfo.deviceUniqueIdentifier,
            os_version = SystemInfo.operatingSystem,
            region = GetUserCountryByIp().Region
        };

        var jsonString = JsonConvert.SerializeObject(deviceData);
        PostData("telemetric/devicedata", jsonString);
    }


    private static IpInfo GetUserCountryByIp()
    {
        string IP = new WebClient().DownloadString("http://icanhazip.com");
        IpInfo ipInfo = new IpInfo();
        try
        {
            string info = new WebClient().DownloadString("http://ipinfo.io/" + IP);
            ipInfo = JsonConvert.DeserializeObject<IpInfo>(info);
            RegionInfo myRI1 = new RegionInfo(ipInfo.Country);
            var ci = CultureInfo.CreateSpecificCulture(myRI1.TwoLetterISORegionName);
            ipInfo.Region = $"{ci.TwoLetterISOLanguageName}-{myRI1.TwoLetterISORegionName}";
        }
        catch (Exception)
        {
            ipInfo.Country = null;
        }

        return ipInfo;
    }

    protected bool PostData(string actionUrl, string jsonData)
    {
        if (Application.internetReachability == NetworkReachability.NotReachable)
            return false;
        try
        {
            WebClient client = GetWebClient();
            var response = client.UploadString(new Uri($"{_baseUrl}{actionUrl}"), "POST", jsonData);
            Debug.Log(response);
        }
        catch (Exception e)
        {
            Debug.LogWarning(e.Message);
            return false;
        }

        return true;
    }

    protected T GetData<T>(string actionUrl) where T : class, new()
    {
        if (Application.internetReachability == NetworkReachability.NotReachable)
            return null;

        WebClient client = GetWebClient();
        var response = client.DownloadString(new Uri($"{_baseUrl}{actionUrl}"));
        var res = JsonConvert.DeserializeObject<T>(response);
        return res;
    }

    private WebClient GetWebClient()
    {
        var client = new WebClient();
        client.Headers.Add(HttpRequestHeader.ContentType, "application/json");
        client.Headers.Add(HttpRequestHeader.Authorization, $"Bearer {AccessToken}");
        return client;
    }
}
