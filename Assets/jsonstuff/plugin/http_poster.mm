//
//  blow_sensor.m
//  Unity-iPhone
//
//  Created by R. Benjamin Shapiro on 8/10/10.
//  Copyright 2010 __MyCompanyName__. All rights reserved.
//

#import "http_poster.h"
#import <stdio.h>


@implementation HTTPPoster

- (NSString *)sendHTTPPostToURL:(NSString *)url withContent:(NSString *)content{
	NSData* requestContent = [NSData dataWithBytes: [content UTF8String] length: [content length]];
	NSMutableURLRequest * urlRequest = [NSMutableURLRequest requestWithURL:[NSURL URLWithString:url]];
	[urlRequest setHTTPMethod: @"POST"];
	[urlRequest setValue:@"application/json; charset=utr-8" forHTTPHeaderField:@"Content-Type"];
	[urlRequest setHTTPBody: requestContent];
	NSData * returnData = [NSURLConnection sendSynchronousRequest:urlRequest returningResponse:nil error:nil];
	NSString * returnString = [[NSString alloc] initWithData:returnData encoding:NSUTF8StringEncoding]; 
	return returnString;
}

- (void)dealloc {

    [super dealloc];
}

@end

	
extern "C" char* _MakeHTTPPost(char* c_url, char* c_content)
{
	NSString* objc_url = [[NSString stringWithUTF8String: c_url] retain];
	NSString* objc_content = [[NSString stringWithUTF8String: c_content] retain];
	NSString* objc_ret;
	char* c_ret;
	
	//NSLog(@"In _MakeHTTPPost...");
	//NSLog(objc_content);
	//NSLog(@"^^^^^^^ content ^^^^^^^^");
	HTTPPoster * poster = [[HTTPPoster alloc] init];
	
	//NSLog(@"About to send data to the poster");
	objc_ret = [poster sendHTTPPostToURL:objc_url withContent:objc_content];
	//NSLog(@"Converting poster return data to a char*");
	const char* c_ret_const = [objc_ret UTF8String];
	c_ret = (char*)malloc(strlen((char*)c_ret_const));
	strcpy(c_ret, c_ret_const);
	
	//NSLog(@"Releaseing poster");
	[poster release];
	//NSLog(@"Releaseing objc_url");
	[objc_url release];
	//NSLog(@"Releaseing obcj_content");
	[objc_content release];
	//NSLog(@"Releaseing objc_ret");
	[objc_ret release];	
	//NSLog(@"Returning...");
	return c_ret;	
}

