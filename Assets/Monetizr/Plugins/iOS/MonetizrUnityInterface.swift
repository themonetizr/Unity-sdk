//
//  MonetizrUnityInterface.swift
//  Unity-iPhone
//
//  Created by Monetizr Macbook Pro on 28/05/2020.
//

import Foundation
import Monetizr

@objc public class MonetizrInterface : NSObject {
    @objc public static func initMonetizr(token: NSString) {
        Monetizr.shared.token = token as String;
    }
    
    @objc public static func initMonetizrApplePay(id: NSString, companyName: NSString) {
        Monetizr.shared.setApplePayMerchantID(id: id as String);
        Monetizr.shared.setCompanyName(companyName: companyName as String);
    }
    
    @objc public static func setMonetizrTestMode(on: Bool) {
        Monetizr.shared.testMode(enabled: on);
    }

    @objc public static func showProductMonetizr(product_tag: NSString, view: UIViewController) {
        Monetizr.shared.showProduct(tag: product_tag as String, presenter: view, completionHandler: {(success:Bool, error:Error?, product:Product?) -> Void in
            if(!success) {
                let cerr = error != nil ? error!.localizedDescription : "Unexpected error in Monetizr iOS SDK occurred.";
                sendUnityMessage("iOSPluginError", cerr);
                print(cerr);
            }
        })
    }
}
