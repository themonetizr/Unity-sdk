#import {POST-PROCESS-OVERWRITE}

NSString* CreateNSString(const char* string) {
    if (string) {
        return [NSString stringWithUTF8String:string];
    }
    else {
        return [NSString stringWithUTF8String:""];
    }
}

void objCinitMonetizr(const char* token) {
    [MonetizrInterface initMonetizrWithToken:CreateNSString(token)];
}

void objCinitMonetizrApplePay(const char* merchantId, const char* companyName) {
    [MonetizrInterface initMonetizrApplePay:CreateNSString(merchantId) companyName:CreateNSString(companyName)];
}

void objCsetMonetizrTestMode(bool on) {
    [MonetizrInterface setMonetizrTestMode:on];
}

void objCshowProductForTag(const char* tag) {
    [MonetizrInterface showProductMonetizrWithProduct_tag:CreateNSString(tag) view:UnityGetGLViewController()];
}

void sendUnityMessage(NSString* method, NSString* msg) {
    UnitySendMessage("_MonetizrInstance", [method UTF8String], [msg UTF8String]);
}
