/*
 * Copyright (C) 2011 Keijiro Takahashi
 * Copyright (C) 2012 GREE, Inc.
 * 
 * This software is provided 'as-is', without any express or implied
 * warranty.  In no event will the authors be held liable for any damages
 * arising from the use of this software.
 * 
 * Permission is granted to anyone to use this software for any purpose,
 * including commercial applications, and to alter it and redistribute it
 * freely, subject to the following restrictions:
 * 
 * 1. The origin of this software must not be misrepresented; you must not
 *    claim that you wrote the original software. If you use this software
 *    in a product, an acknowledgment in the product documentation would be
 *    appreciated but is not required.
 * 2. Altered source versions must be plainly marked as such, and must not be
 *    misrepresented as being the original software.
 * 3. This notice may not be removed or altered from any source distribution.
 */

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
#if UNITY_2018_4_OR_NEWER
using UnityEngine.Networking;
#endif

using Callback = System.Action<string>;

public class WebViewObject : MonoBehaviour
{
    Callback onJS;
    Callback onError;
    Callback onHttpError;
    Callback onStarted;
    Callback onLoaded;
    Callback onHooked;
    bool visibility;
    bool alertDialogEnabled = true;
    bool scrollBounceEnabled = true;
    int mMarginLeft;
    int mMarginTop;
    int mMarginRight;
    int mMarginBottom;
#if UNITY_ANDROID
    AndroidJavaObject webView;
    
    bool mVisibility;
    bool mIsKeyboardVisible0;
    bool mIsKeyboardVisible;
    float mResumedTimestamp;
    
    void OnApplicationPause(bool paused)
    {
        if (webView == null)
            return;
        if (!paused)
        {
            webView.Call("SetVisibility", false);
            mResumedTimestamp = Time.realtimeSinceStartup;
        }
        webView.Call("OnApplicationPause", paused);
    }

    void Update()
    {
        if (webView == null)
            return;
        if (mResumedTimestamp != 0.0f && Time.realtimeSinceStartup - mResumedTimestamp > 0.5f)
        {
            mResumedTimestamp = 0.0f;
            webView.Call("SetVisibility", mVisibility);
        }
    }

    /// Called from Java native plugin to set when the keyboard is opened
    public void SetKeyboardVisible(string pIsVisible)
    {
        mIsKeyboardVisible = (pIsVisible == "true");
        if (mIsKeyboardVisible != mIsKeyboardVisible0)
        {
            mIsKeyboardVisible0 = mIsKeyboardVisible;
            SetMargins(mMarginLeft, mMarginTop, mMarginRight, mMarginBottom);
        } else if (mIsKeyboardVisible) {
            SetMargins(mMarginLeft, mMarginTop, mMarginRight, mMarginBottom);
        }
    }
    
    public int AdjustBottomMargin(int bottom)
    {
        if (!mIsKeyboardVisible)
        {
            return bottom;
        }
        else
        {
            int keyboardHeight = 0;
            using(AndroidJavaClass UnityClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
            {
                AndroidJavaObject View = UnityClass.GetStatic<AndroidJavaObject>("currentActivity").Get<AndroidJavaObject>("mUnityPlayer").Call<AndroidJavaObject>("getView");
                using(AndroidJavaObject Rct = new AndroidJavaObject("android.graphics.Rect"))
                {
                    View.Call("getWindowVisibleDisplayFrame", Rct);
                    keyboardHeight = View.Call<int>("getHeight") - Rct.Call<int>("height");
                }
            }
            return (bottom > keyboardHeight) ? bottom : keyboardHeight;
        }
    }
#else
    IntPtr webView;
#endif

    public bool IsKeyboardVisible
    {
        get
        {
#if !UNITY_EDITOR && UNITY_ANDROID
            return mIsKeyboardVisible;
#else
            return false;
#endif
        }
    }

    public static bool IsWebViewAvailable()
    {
#if !UNITY_EDITOR && UNITY_ANDROID
        return (new AndroidJavaObject("net.gree.unitywebview.CWebViewPlugin")).CallStatic<bool>("IsWebViewAvailable");
#else
        return true;
#endif
    }

    public void Init(
        Callback cb = null,
        bool transparent = false,
        string ua = "",
        Callback err = null,
        Callback httpErr = null,
        Callback ld = null,
        bool enableWKWebView = false,
        Callback started = null,
        Callback hooked = null)
    {
        onJS = cb;
        onError = err;
        onHttpError = httpErr;
        onStarted = started;
        onLoaded = ld;
        onHooked = hooked;
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
        //TODO: UNSUPPORTED
        Debug.LogError("Webview is not supported on this platform.");
#elif UNITY_ANDROID
        webView = new AndroidJavaObject("net.gree.unitywebview.CWebViewPlugin");
        webView.Call("Init", name, transparent, ua);
#else
        Debug.LogError("Webview is not supported on this platform.");
#endif
    }

    protected virtual void OnDestroy()
    {
#if UNITY_ANDROID
        if (webView == null)
            return;
        webView.Call("Destroy");
        webView = null;
#endif
    }

    // Use this function instead of SetMargins to easily set up a centered window
    // NOTE: for historical reasons, `center` means the lower left corner and positive y values extend up.
    public void SetCenterPositionWithScale(Vector2 center, Vector2 scale)
    {
#if UNITY_WEBPLAYER || UNITY_WEBGL
        //TODO: UNSUPPORTED
#elif UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN || UNITY_EDITOR_LINUX
        //TODO: UNSUPPORTED
#else
        float left = (Screen.width - scale.x) / 2.0f + center.x;
        float right = Screen.width - (left + scale.x);
        float bottom = (Screen.height - scale.y) / 2.0f + center.y;
        float top = Screen.height - (bottom + scale.y);
        SetMargins((int)left, (int)top, (int)right, (int)bottom);
#endif
    }

    public void SetMargins(int left, int top, int right, int bottom, bool relative = false)
    {
#if UNITY_WEBPLAYER || UNITY_WEBGL
#elif UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN || UNITY_EDITOR_LINUX
        //TODO: UNSUPPORTED
#elif UNITY_ANDROID
        if (webView == null)
            return;
#endif
        mMarginLeft = left;
        mMarginTop = top;
        mMarginRight = right;
        mMarginBottom = bottom;
#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN || UNITY_EDITOR_LINUX
        //TODO: UNSUPPORTED
#elif UNITY_ANDROID
        if (relative) {
            float w = (float)Screen.width;
            float h = (float)Screen.height;
            int iw = Screen.currentResolution.width;
            int ih = Screen.currentResolution.height;
            webView.Call("SetMargins", (int)(left / w * iw), (int)(top / h * ih), (int)(right / w * iw), AdjustBottomMargin((int)(bottom / h * ih)));
        } else {
            webView.Call("SetMargins", left, top, right, AdjustBottomMargin(bottom));
        }
#endif
    }

    public void SetVisibility(bool v)
    {
#if UNITY_EDITOR
        //TODO: UNSUPPORTED
#elif UNITY_ANDROID
        if (webView == null)
            return;
        mVisibility = v;
        webView.Call("SetVisibility", v);
#endif
        visibility = v;
    }

    public bool GetVisibility()
    {
        return visibility;
    }

    public void SetAlertDialogEnabled(bool e)
    {
#if UNITY_ANDROID
        if (webView == null)
            return;
        webView.Call("SetAlertDialogEnabled", e);
#else
        // TODO: UNSUPPORTED
#endif
        alertDialogEnabled = e;
    }

    public bool GetAlertDialogEnabled()
    {
        return alertDialogEnabled;
    }

    public void SetScrollBounceEnabled(bool e)
    {
#if UNITY_ANDROID
        // TODO: UNSUPPORTED
#else
        // TODO: UNSUPPORTED
#endif
        scrollBounceEnabled = e;
    }

    public bool GetScrollBounceEnabled()
    {
        return scrollBounceEnabled;
    }

    public bool SetURLPattern(string allowPattern, string denyPattern, string hookPattern)
    {
#if UNITY_EDITOR
        //TODO: UNSUPPORTED
        return false;
#elif UNITY_ANDROID
        if (webView == null)
            return false;
        return webView.Call<bool>("SetURLPattern", allowPattern, denyPattern, hookPattern);
#endif
    }

    public void LoadURL(string url)
    {
        if (string.IsNullOrEmpty(url))
            return;
#if UNITY_EDITOR
        //TODO: UNSUPPORTED
#elif UNITY_ANDROID
        if (webView == null)
            return;
        webView.Call("LoadURL", url);
#endif
    }

    public void LoadHTML(string html, string baseUrl)
    {
        if (string.IsNullOrEmpty(html))
            return;
        if (string.IsNullOrEmpty(baseUrl))
            baseUrl = "";
#if UNITY_EDITOR_WIN
        //TODO: UNSUPPORTED
#elif UNITY_ANDROID
        if (webView == null)
            return;
        webView.Call("LoadHTML", html, baseUrl);
#endif
    }

    public void EvaluateJS(string js)
    {
#if UNITY_EDITOR
        //TODO: UNSUPPORTED
#elif UNITY_ANDROID
        if (webView == null)
            return;
        webView.Call("EvaluateJS", js);
#endif
    }

    public int Progress()
    {
#if UNITY_EDITOR
        //TODO: UNSUPPORTED
        return 0;
#elif UNITY_ANDROID
        if (webView == null)
            return 0;
        return webView.Get<int>("progress");
#endif
    }

    public bool CanGoBack()
    {
#if UNITY_EDITOR
        //TODO: UNSUPPORTED
        return false;
#elif UNITY_ANDROID
        if (webView == null)
            return false;
        return webView.Get<bool>("canGoBack");
#endif
    }

    public bool CanGoForward()
    {
#if UNITY_EDITOR
        //TODO: UNSUPPORTED
        return false;
#elif UNITY_ANDROID
        if (webView == null)
            return false;
        return webView.Get<bool>("canGoForward");
#endif
    }

    public void GoBack()
    {
#if UNITY_EDITOR
        //TODO: UNSUPPORTED
#elif UNITY_ANDROID
        if (webView == null)
            return;
        webView.Call("GoBack");
#endif
    }

    public void GoForward()
    {
#if UNITY_EDITOR
        //TODO: UNSUPPORTED
#elif UNITY_ANDROID
        if (webView == null)
            return;
        webView.Call("GoForward");
#endif
    }

    public void CallOnError(string error)
    {
        if (onError != null)
        {
            onError(error);
        }
    }

    public void CallOnHttpError(string error)
    {
        if (onHttpError != null)
        {
            onHttpError(error);
        }
    }

    public void CallOnStarted(string url)
    {
        if (onStarted != null)
        {
            onStarted(url);
        }
    }

    public void CallOnLoaded(string url)
    {
        if (onLoaded != null)
        {
            onLoaded(url);
        }
    }

    public void CallFromJS(string message)
    {
        if (onJS != null)
        {
#if !UNITY_ANDROID
#if UNITY_2018_4_OR_NEWER
            message = UnityWebRequest.UnEscapeURL(message);
#else // UNITY_2018_4_OR_NEWER
            message = WWW.UnEscapeURL(message);
#endif // UNITY_2018_4_OR_NEWER
#endif // !UNITY_ANDROID
            onJS(message);
        }
    }

    public void CallOnHooked(string message)
    {
        if (onHooked != null)
        {
#if !UNITY_ANDROID
#if UNITY_2018_4_OR_NEWER
            message = UnityWebRequest.UnEscapeURL(message);
#else // UNITY_2018_4_OR_NEWER
            message = WWW.UnEscapeURL(message);
#endif // UNITY_2018_4_OR_NEWER
#endif // !UNITY_ANDROID
            onHooked(message);
        }
    }


    public void AddCustomHeader(string headerKey, string headerValue)
    {
#if UNITY_EDITOR
        //TODO: UNSUPPORTED
#elif UNITY_ANDROID
        if (webView == null)
            return;
        webView.Call("AddCustomHeader", headerKey, headerValue);
#endif
    }

    public string GetCustomHeaderValue(string headerKey)
    {
#if UNITY_EDITOR
        //TODO: UNSUPPORTED
        return null;
#elif UNITY_ANDROID
        if (webView == null)
            return null;
        return webView.Call<string>("GetCustomHeaderValue", headerKey);
#endif
    }

    public void RemoveCustomHeader(string headerKey)
    {
#if UNITY_ANDROID
        if (webView == null)
            return;
        webView.Call("RemoveCustomHeader", headerKey);
#endif
    }

    public void ClearCustomHeader()
    {
#if UNITY_ANDROID
        if (webView == null)
            return;
        webView.Call("ClearCustomHeader");
#endif
    }

    public void ClearCookies()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        if (webView == null)
            return;
        webView.Call("ClearCookies");
#endif
    }


    public void SaveCookies()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        if (webView == null)
            return;
        webView.Call("SaveCookies");
#endif
    }


    public string GetCookies(string url)
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        if (webView == null)
            return "";
        return webView.Call<string>("GetCookies", url);
#else
        //TODO: UNSUPPORTED
        return "";
#endif
    }

    public void SetBasicAuthInfo(string userName, string password)
    {
#if UNITY_ANDROID
        if (webView == null)
            return;
        webView.Call("SetBasicAuthInfo", userName, password);
#endif
    }
}
