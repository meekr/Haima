//
//  ConfigViewController.h
//  Unity-iPhone
//
//  Created by Perry on 11-9-28.
//  Copyright 2011年 __MyCompanyName__. All rights reserved.
//

#import <UIKit/UIKit.h>


@interface ConfigViewController : UIViewController<UIWebViewDelegate> {
    
}

+ (id)sharedConfig;

+ (id)initSharedConfig;

+ (void) releaseSharedConfig;

- (void)show;
- (void)hide;

@end
