//
//  CalculatorViewController.m
//  Unity-iPhone
//
//  Created by Perry on 11-9-26.
//  Copyright 2011年 __MyCompanyName__. All rights reserved.
//

#import "CalculatorViewController.h"

static CalculatorViewController *sharedCalculatorViewController = nil;

@implementation CalculatorViewController

+ (id) sharedCalculator {
    return sharedCalculatorViewController;
}

+ (id) initSharedCalculator {
    if (!sharedCalculatorViewController){
        sharedCalculatorViewController = [[self alloc] init];
    }
    return sharedCalculatorViewController;
}

+ (void) releaseSharedCalculator {
    if (sharedCalculatorViewController != nil){
        [sharedCalculatorViewController.view removeFromSuperview];
        [sharedCalculatorViewController release];
        sharedCalculatorViewController = nil;
    }
}

- (void)show{
    [UIView transitionWithView:self.view duration:0.4
                       options:UIViewAnimationOptionCurveEaseInOut
                    animations:^ {
                        self.view.alpha = 1;
                    }
                    completion:^(BOOL finished) {
                    }];
}

- (void)hide{
    [UIView transitionWithView:self.view duration:0.4
                       options:UIViewAnimationOptionCurveEaseInOut
                    animations:^ {
                        self.view.alpha = 0;
                    }
                    completion:^(BOOL finished) {
                    }];    
}

- (void)dealloc
{
    [super dealloc];
}

#pragma mark - View lifecycle

// Implement loadView to create a view hierarchy programmatically, without using a nib.
- (void)loadView
{
    UIView *view = [[UIView alloc] initWithFrame:CGRectMake(0, 0, 685, 1024)];
    view.alpha = 0;
    self.view = view;
    [view release];
    
    UIView *subView = [[UIView alloc] initWithFrame:CGRectMake(0, 0, 1024, 1024)];
    [self.view addSubview:subView];
    
    UIWebView *webView = [[UIWebView alloc] initWithFrame:CGRectMake(0, 0, 1024, 685)];
	NSString *path = [[NSBundle mainBundle] pathForResource:@"calculator" ofType:@"html"];
	[webView loadRequest:[NSURLRequest requestWithURL:[NSURL fileURLWithPath:path]]];
	webView.delegate = self;
    [subView addSubview:webView];
    
    // turn off bounce
    for (id sv in webView.subviews){
        if ([[sv class] isSubclassOfClass: [UIScrollView class]])
            ((UIScrollView *)sv).bounces = NO;
    }
    
    [webView release];
    
    
    [subView release];
}

// Implement viewDidLoad to do additional setup after loading the view, typically from a nib.
- (void)viewDidLoad
{
    [super viewDidLoad];
    
    UIView *subView = (UIView *)[[self.view subviews] objectAtIndex:0];
    // If we are landscape, manually rotate our view
    if( [UIApplication sharedApplication].statusBarOrientation == UIInterfaceOrientationLandscapeLeft ){
        subView.transform = CGAffineTransformMake(0, -1, 1, 0, 0, 0);//CGAffineTransformMakeRotation( -1.5708 );
    }
    else if( [UIApplication sharedApplication].statusBarOrientation == UIInterfaceOrientationLandscapeRight ){
        subView.transform = CGAffineTransformMake(0, 1, -1, 0, 0, 0);//CGAffineTransformMakeRotation( 1.5708 );
    }
}

- (void)viewDidUnload
{
    [super viewDidUnload];
    // Release any retained subviews of the main view.
    // e.g. self.myOutlet = nil;
}

- (BOOL)shouldAutorotateToInterfaceOrientation:(UIInterfaceOrientation)interfaceOrientation
{
    // Return YES for supported orientations
    return (interfaceOrientation != UIInterfaceOrientationPortrait);
}

@end
