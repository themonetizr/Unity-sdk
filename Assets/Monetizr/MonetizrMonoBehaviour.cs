using Assets.Monetizr.Dto;
using Newtonsoft.Json;
using System;
using System.Globalization;
using System.Net;
using UnityEngine;

public class MonetizrMonoBehaviour : MonoBehaviour
{
    public string AccessToken;
    public string MerchandiseId;
    private string _baseUrl = "https://api3.themonetizr.com/api/";
    private bool _sessionRegistered;
    private DateTime? _sessionStartTime;

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
    public async void Init(string accessToken, string merchandiseId)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
    {
        AccessToken = accessToken;
        MerchandiseId = merchandiseId;
        _sessionRegistered = false;
        RegisterSessionStart();
        SendDeviceInfo();
    }

    private async void RegisterSessionStart()
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
        if (PostData("telemetric/session_end", jsonString))
        {
            _sessionRegistered = false;
        }
    }

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
    private async void SendDeviceInfo()
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
    {
        if (Application.internetReachability == NetworkReachability.NotReachable)
            return;

        var deviceData = new DeviceData()
        {
            language = SystemInfo.deviceType.ToString(),
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

    protected bool PostData(string actinUrl, string jsonData)
    {
        if (Application.internetReachability == NetworkReachability.NotReachable)
            return false;
        try
        {
            WebClient client = GetWebClient();
            var response = client.UploadString(new Uri($"{_baseUrl}{actinUrl}"), "POST", jsonData);
            Debug.Log(response);
        }
        catch (Exception e)
        {
            Debug.LogWarning(e.Message);
            return false;
        }

        return true;
    }

    private WebClient GetWebClient()
    {
        var client = new WebClient();
        client.Headers.Add(HttpRequestHeader.ContentType, "application/json");
        client.Headers.Add(HttpRequestHeader.Authorization, $"Bearer {AccessToken}");
        return client;
    }
}
