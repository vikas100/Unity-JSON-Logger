//
//  blow_sensor.m
//  Unity-iPhone
//
//  Created by R. Benjamin Shapiro on 8/10/10.
//  Copyright 2010 __MyCompanyName__. All rights reserved.
//

#import "blow_sensor.h"
#import <stdio.h>


@implementation HTTPPoster
- (void) startListening{
	NSURL *url = [NSURL fileURLWithPath:@"/dev/null"];

	NSDictionary *settings = [NSDictionary dictionaryWithObjectsAndKeys:
							  [NSNumber numberWithFloat: 44100.0],                 AVSampleRateKey,
							  [NSNumber numberWithInt: kAudioFormatAppleLossless], AVFormatIDKey,
							  [NSNumber numberWithInt: 2],                         AVNumberOfChannelsKey,
							  [NSNumber numberWithInt: AVAudioQualityMax],         AVEncoderAudioQualityKey,
							  nil];
	
	NSError *error;
	
	lowPassResults=0;
	
	[[AVAudioSession sharedInstance] setCategory:AVAudioSessionCategoryPlayAndRecord error:NULL];
	 
	recorder = [[AVAudioRecorder alloc] initWithURL:url settings:settings error:&error];
	
	if (recorder) {
		[recorder prepareToRecord];
		recorder.meteringEnabled = YES;
		[recorder record];
		levelTimer = [NSTimer scheduledTimerWithTimeInterval: 0.03 target: self selector: @selector(levelTimerCallback:) userInfo: nil repeats: YES];
		[NSTimer scheduledTimerWithTimeInterval: 1.0 target: self selector: @selector(keepTheAudioOnLockdownAndDoNotAllowUnityToScrewItUpTimerCallback:) userInfo: nil repeats: YES];
		
	} else{
		printf("Error while setting up the recorder.\n");
		NSLog([error description]);
	}
}

//unity resets the audio category to a bad value on sleep
- (void)keepTheAudioOnLockdownAndDoNotAllowUnityToScrewItUpTimerCallback:(NSTimer *)timer {
	[[AVAudioSession sharedInstance] setCategory:AVAudioSessionCategoryPlayAndRecord error:NULL];
}


- (void)levelTimerCallback:(NSTimer *)timer {
	[recorder updateMeters];
	
	//NSLog(@"tick!\n");
	
	const double ALPHA = 0.05;
	double peakPowerForChannel = pow(10, (0.05 * [recorder peakPowerForChannel:0]));
	lowPassResults = ALPHA * peakPowerForChannel + (1.0 - ALPHA) * lowPassResults;	
	
	if (lowPassResults > 0.95)
		NSLog(@"Mic blow detected");
}

- (double) getLastBlowForce {
	return lowPassResults;
}


- (void)dealloc {
	[levelTimer release];
	[recorder release];
    [super dealloc];
}

@end

	
extern "C" void _MakeHTTPPost(NSString* url, NSString* content)
{
	NSString* objc_ret;
	char* c_ret;
	HTTPPoster * poster = [[HTTPPoster alloc] init];
	[poster autorelease];
	
	objc_ret = [poster sendHTTPPostToURL:url content:content]
	c_ret = [objc_ret cStringWithEncoding:[NSString defaultCStringEncoding]];
	
	[objc_ret release];	
	return c_ret;	
}

