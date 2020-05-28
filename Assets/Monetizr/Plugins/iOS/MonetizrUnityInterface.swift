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

    @objc public static func showProductMonetizr(product_tag: NSString) {
        Monetizr.shared.showProduct(tag: product_tag as String, completionHandler: {(success:Bool, error:Error?, product:Product?) -> Void in
        })
    }
}
