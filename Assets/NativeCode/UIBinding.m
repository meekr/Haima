//
//  UIBinding.m
//  Unity-iPhone
//
//  Created by Gregg Patton on 7/6/10.
//  Copyright 2010 Gregg Patton. All rights reserved.
//

#import "UIBinding.h"
#import "CalculatorViewController.h"
#import "ConfigViewController.h"

//Initialize the native UI on the first call to this function and hide the splash screen after a delay
//All subsequent calls will call the slideUp method to show the native UI.
void _ActivateUICalculator() {
	//Get the applications UIWindow
	UIWindow *window = [[UIApplication sharedApplication] keyWindow];

	//Only add CalculatorViewController to the main window once
	if ([CalculatorViewController sharedCalculator] == nil) {
        CalculatorViewController *vc = [CalculatorViewController initSharedCalculator];

		//Add the CalculatorViewController view to the main window.
		[window addSubview: vc.view];
	}
    [[CalculatorViewController sharedCalculator] show];
}

void _DeactivateUICalculator() {
	if ([CalculatorViewController sharedCalculator] != nil) {
        [[CalculatorViewController sharedCalculator] hide];
	}
}

void _ActivateUIConfig() {
	//Get the applications UIWindow
	UIWindow *window = [[UIApplication sharedApplication] keyWindow];
    
	//Only add CalculatorViewController to the main window once
	if ([ConfigViewController sharedConfig] == nil) {
        ConfigViewController *vc = [ConfigViewController initSharedConfig];
        
		[window addSubview: vc.view];
	}
    [[ConfigViewController sharedConfig] show];
}

void _DeactivateUIConfig() {
	if ([ConfigViewController sharedConfig] != nil) {
        [[ConfigViewController sharedConfig] hide];
	}
}