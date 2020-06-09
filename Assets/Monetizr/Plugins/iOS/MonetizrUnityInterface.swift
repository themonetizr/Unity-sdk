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

    @objc public static func showProductMonetizr(product_tag: NSString, view: UIViewController) {
        Monetizr.shared.showProduct(tag: product_tag as String, presenter: view, completionHandler: {(success:Bool, error:Error?, product:Product?) -> Void in
            if(success) {
                // print("This was a triumph.");
            }
            else {
                let cerr = error != nil ? error!.localizedDescription : "Unexpected error in Monetizr iOS SDK occurred.";
                sendUnityMessage("iOSPluginError", cerr);
                print(cerr);
            }
        })
    }
}
