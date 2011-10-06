//
//  CalculatorViewController.h
//  Unity-iPhone
//
//  Created by Perry on 11-9-26.
//  Copyright 2011年 __MyCompanyName__. All rights reserved.
//

#import <UIKit/UIKit.h>

@interface CalculatorViewController : UIViewController<UIWebViewDelegate> {
    
}

+ (id)sharedCalculator;

+ (id)initSharedCalculator;

+ (void) releaseSharedCalculator;

- (void)show;
- (void)hide;

@end
