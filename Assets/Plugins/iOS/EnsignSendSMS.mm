//
//  EnsignSendSMS.m
//  EnsignSendSMS
//
//  Created by vietdungvn88@gmail.com on 02/4/15.
//  Copyright (c) 2015 Ensign. All rights reserved.
//

#import <Foundation/Foundation.h>
#import <SystemConfiguration/SystemConfiguration.h>
#import <AVFoundation/AVFoundation.h>
#ifdef __APPLE__
#include "TargetConditionals.h"
#endif
#import <MessageUI/MessageUI.h>

#ifdef __cplusplus
extern "C" {
#endif
    void UnitySendMessage(const char* obj, const char* method, const char* msg);
#ifdef __cplusplus
}
#endif

#define MakeStringCopy( _x_ ) ( _x_ != NULL && [_x_ isKindOfClass:[NSString class]] ) ? strdup( [_x_ UTF8String] ) : NULL


@interface EnsignSendSMS : NSObject <MFMessageComposeViewControllerDelegate, MFMailComposeViewControllerDelegate, UINavigationControllerDelegate>
{
}
@end


@implementation EnsignSendSMS

- (id)init
{
    self = [super init];
    return self;
}

+ (instancetype)sharedManager
{
    static id sharedInstance = nil;
    
    static dispatch_once_t onceToken;
    dispatch_once(&onceToken, ^{
        sharedInstance = [[self alloc] init];
    });
    
    return sharedInstance;
}


- (void)sendMessageToUnity:(NSString *)message object:(NSString *)object method:(NSString *)method
{
    UnitySendMessage([object UTF8String], [method UTF8String], [message UTF8String]);
}

- (BOOL)isRunningInSimulator
{
#if TARGET_IPHONE_SIMULATOR
    return YES;
#elif TARGET_OS_IPHONE
    return NO;
#else
    return NO;
#endif
}

-(void)dealloc
{
#if !(__has_feature(objc_arc))
    [super dealloc];
#endif   
}

#pragma mark - Reachbility

- (BOOL)showMessageComposerWithMessage:(NSString *)message phoneNumbers:(NSString *)phoneNumbers
{
    if ([MFMessageComposeViewController canSendText]) {
        MFMessageComposeViewController *picker = [[MFMessageComposeViewController alloc] init];
        picker.messageComposeDelegate = self;
        if (!phoneNumbers) {
            phoneNumbers = @"";
        }
        NSArray *arr = [phoneNumbers componentsSeparatedByString:@","];
        if (!arr || arr.count == 0) {
            return NO;
        }
        picker.recipients = arr;
        picker.body = message;
        
        id appController = [[UIApplication sharedApplication] delegate];
        UIViewController *vc = [appController valueForKey:@"rootViewController"];
        [vc presentViewController:picker animated:YES completion:nil];
        return YES;
    }
    return NO;
}

- (BOOL)showMailComposerWithSubject:(NSString *)subject message:(NSString *)message emails:(NSString *)emails
{
    if ([MFMailComposeViewController canSendMail]) {
        MFMailComposeViewController *composer = [[MFMailComposeViewController alloc] init];
        composer.mailComposeDelegate = self;
        if (!emails) {
            emails = @"";
        }
        NSArray *arr = [emails componentsSeparatedByString:@","];
        if (!arr || arr.count == 0) {
            return NO;
        }
        [composer setSubject:subject];
        [composer setMessageBody:message isHTML:YES];
        [composer setToRecipients:arr];

        id appController = [[UIApplication sharedApplication] delegate];
        UIViewController *vc = [appController valueForKey:@"rootViewController"];
        [vc presentViewController:composer animated:YES completion:nil];
        return YES;
    }
    return NO;
}

#pragma mark - Mail/Message Composer Delegate
- (void)messageComposeViewController:(MFMessageComposeViewController *)controller didFinishWithResult:(MessageComposeResult)result
{
    switch (result) {
        case MessageComposeResultCancelled:
            NSLog(@"Results: SMS sending canceled");
            break;
        case MessageComposeResultSent:
            NSLog(@"Results: SMS sent");
            break;
        case MessageComposeResultFailed:
            NSLog(@"Results: SMS sending failed");
            break;
        default:
            NSLog(@"Results: SMS not sent");
            break;
    }
    id appController = [[UIApplication sharedApplication] delegate];
    UIViewController *vc = [appController valueForKey:@"rootViewController"];
    [vc dismissViewControllerAnimated:YES completion:nil];
}

- (void)mailComposeController:(MFMailComposeViewController *)controller didFinishWithResult:(MFMailComposeResult)result error:(NSError *)error
{
    switch (result) {
        case MFMailComposeResultCancelled:
            NSLog(@"Results: Mail sending cancelled");
            break;
        case MFMailComposeResultSent:
            NSLog(@"Results: Mail sent");
            break;
        case MFMailComposeResultFailed:
            NSLog(@"Results: Mail sending failed");
            break;
        case MFMailComposeResultSaved:
            NSLog(@"Results: Mail saved");
            break;
            
        default:
            break;
    }
    id appController = [[UIApplication sharedApplication] delegate];
    UIViewController *vc = [appController valueForKey:@"rootViewController"];
    [vc dismissViewControllerAnimated:YES completion:nil];    
}

@end

extern "C" {
    
	const bool dungnv_IsRunningInSimulator()
    {
        return [[EnsignSendSMS sharedManager] isRunningInSimulator];
    }
    
    bool dungnv_ShowMessageComposer(char *phone_numbers, char *message)
    {
        NSString *phones = [[NSString alloc] initWithCString:phone_numbers encoding:NSUTF8StringEncoding];
        NSString *msg = [[NSString alloc] initWithCString:message encoding:NSUTF8StringEncoding];
        return [[EnsignSendSMS sharedManager] showMessageComposerWithMessage:msg phoneNumbers:phones];
    }
    
    bool dungnv_ShowMailComposer(char *email_addresses, char *subject, char *message)
    {
        NSString *emails = [[NSString alloc] initWithCString:email_addresses encoding:NSUTF8StringEncoding];
        NSString *sbj = [[NSString alloc] initWithCString:subject encoding:NSUTF8StringEncoding];
        NSString *msg = [[NSString alloc] initWithCString:message encoding:NSUTF8StringEncoding];
        return [[EnsignSendSMS sharedManager] showMailComposerWithSubject:sbj message:msg emails:emails];
    }
}
