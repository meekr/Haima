//
//  PreferencesBinding.m
//  Unity-iPhone
//
//  Created by Gregg Patton on 7/22/10.
//  Copyright 2010 Gregg Patton. All rights reserved.
//

typedef void* MonoArray; 

#import "PreferencesBinding.h"

#define kAngularVelocity_X @"av_X"
#define kAngularVelocity_Y @"av_Y"
#define kAngularVelocity_Z @"av_Z"

#define kRotation_X @"rot_X"
#define kRotation_Y @"rot_Y"
#define kRotation_Z @"rot_Z"
#define kRotation_W @"rot_W"

#define kHighestSpinSpeed @"maxSpinSpeed"

#define kAngularDrag @"angularDrag"

#define kAutoRotate @"autoRotate"

#define kiAdsOn @"iAdsOn"


void _GetRotation (const char * levelName, float * rot) {
    
    NSUserDefaults *defaults = [NSUserDefaults standardUserDefaults];
    
    rot[0] = [defaults floatForKey:[NSString stringWithFormat:@"%s_%@", levelName, kRotation_X]];
    rot[1] = [defaults floatForKey:[NSString stringWithFormat:@"%s_%@", levelName, kRotation_Y]];
    rot[2] = [defaults floatForKey:[NSString stringWithFormat:@"%s_%@", levelName, kRotation_Z]];
    rot[3] = [defaults floatForKey:[NSString stringWithFormat:@"%s_%@", levelName, kRotation_W]];
}

void _SetRotation (const char * levelName, float * rot) {
    
    NSUserDefaults *defaults = [NSUserDefaults standardUserDefaults];
    
    [defaults setFloat:rot[0] forKey:[NSString stringWithFormat:@"%s_%@", levelName, kRotation_X]];
    [defaults setFloat:rot[1] forKey:[NSString stringWithFormat:@"%s_%@", levelName, kRotation_Y]];
    [defaults setFloat:rot[2] forKey:[NSString stringWithFormat:@"%s_%@", levelName, kRotation_Z]];
    [defaults setFloat:rot[3] forKey:[NSString stringWithFormat:@"%s_%@", levelName, kRotation_W]];
}

