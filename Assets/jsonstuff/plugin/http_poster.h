//
//  blow_sensor.h
//  Unity-iPhone
//
//  Created by R. Benjamin Shapiro on 8/10/10.
//  Copyright 2010 __MyCompanyName__. All rights reserved.
//

#import <Foundation/Foundation.h>


@interface HTTPPoster : NSObject {
}

- (char *) sendHTTPPostToURL:(NSString *)url content:(NSString *)content;

@end
