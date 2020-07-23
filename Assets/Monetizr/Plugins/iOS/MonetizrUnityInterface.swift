//
//  MonetizrUnityInterface.swift
//  Unity-iPhone
//
//  Created by Monetizr Macbook Pro on 28/05/2020.
//

import Foundation
import Monetizr

@objc public class MonetizrInterface : NSObject {
    static var currentPlayerId : String = "";
    
    @objc public static func initMonetizr(token: NSString) {
        Monetizr.shared.token = token as String;
    }
    
    @objc public static func setMonetizrPlayerId(playerId: NSString) {
        currentPlayerId = playerId as String;
    }
    
    @objc public static func initMonetizrApplePay(id: NSString, companyName: NSString) {
        Monetizr.shared.setApplePayMerchantID(id: id as String);
        Monetizr.shared.setCompanyName(companyName: companyName as String);
    }
    
    @objc public static func setMonetizrTestMode(on: Bool) {
        Monetizr.shared.testMode(enabled: on);
    }

    @objc public static func showProductMonetizr(product_tag: NSString, view: UIViewController) {
        //let viewWithDelegate : MonetizrUnityView = view as! MonetizrUnityView;
        Monetizr.shared.showProduct(tag: product_tag as String, playerID: currentPlayerId, presenter: view, completionHandler: {(success:Bool, error:Error?, product:Product?, unique:String?) -> Void in
            if(!success) {
                let cerr = error != nil ? error!.localizedDescription : "Unexpected error in Monetizr iOS SDK occurred.";
                sendUnityMessage("iOSPluginError", cerr);
                print(cerr);
            }
        })
    }
}

extension UIViewController : MonetizrDelegate {
    public func monetizrPurchase(tag: String?, uniqueID: String?) {
        let ctag = tag != nil ? tag! : "Unknown tag!";
        sendUnityMessage("iOSPluginPurchaseDelegate", ctag);
    }
}
