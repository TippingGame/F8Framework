
#if defined(__cplusplus)
extern "C"{
#endif

void sendU3dMessage(NSString * messageName,NSString *dict)
{
    if (dict != nil)
    {
        UnitySendMessage("SDKManager", [messageName UTF8String], [dict UTF8String]);
    }
    else{
        UnitySendMessage("SDKManager", [messageName UTF8String], "");
    }
}

void SDKInit(){
	::printf("-> SDKInit()\n");
	sendU3dMessage(@"onInitSuccessed",@"");
}

void SDKLogin()
{
    ::printf("-> SDKLogin()\n");
}

void SDKLogout()
{
	::printf("-> SDKLogout()\n");
}

void SDKSwitchAccount()
{
	::printf("-> SDKSwitchAccount()\n");
}

void SDKLoadVideoAd()
{
	::printf("-> SDKLoadVideoAd()\n");
}

void SDKShowVideoAd()
{
	::printf("-> SDKShowVideoAd()\n");
}

void SDKPay(const char *serverNum, const char *serverName, const char *playerId, const char *playerName, const char *amount, const char *extra, const char *orderId, const char *productName, const char *productContent, const char *playerLevel, const char *sign, const char *guid)
{
	::printf("-> SDKPay()\n");
    NSString *NSSservernum = [[NSString alloc] initWithUTF8String:serverNum];
    NSString *NSSservername = [[NSString alloc] initWithUTF8String:serverName];
    NSString *NSSplayerid = [[NSString alloc] initWithUTF8String:playerId];
    NSString *NSSplayername = [[NSString alloc] initWithUTF8String:playerName];
    NSString *NSSamount = [[NSString alloc] initWithUTF8String:amount];
    NSString *NSSextra = [[NSString alloc] initWithUTF8String:extra];
    NSString *NSSorderid = [[NSString alloc] initWithUTF8String:orderId];
    NSString *NSSproductname = [[NSString alloc] initWithUTF8String:productName];
    NSString *NSSproductcontent = [[NSString alloc] initWithUTF8String:productContent];
    NSString *NSSplayerlevel = [[NSString alloc] initWithUTF8String:playerLevel];
    NSString *NSSsign = [[NSString alloc] initWithUTF8String:sign];
    NSString *NSSguid = [[NSString alloc] initWithUTF8String:guid];
}

void SDKUpdateRole(const char scenes,const char *serverId, const char *serverName, const char *roleId, const char *roleName, const char *roleLeve, const char *roleCTime,const char *rolePower,const char *guid){
    ::printf("-> SDKUpdateRole()\n");
    NSString *NSSscenes = [NSString stringWithFormat:@"%c", scenes];
    NSString *NSSserverId = [[NSString alloc] initWithUTF8String:serverId];
    NSString *NSSserverName = [[NSString alloc] initWithUTF8String:serverName];
    NSString *NSSroleId = [[NSString alloc] initWithUTF8String:roleId];
    NSString *NSSroleName = [[NSString alloc] initWithUTF8String:roleName];
    NSString *NSSroleLeve = [[NSString alloc] initWithUTF8String:roleLeve];
    NSString *NSSroleCTime = [[NSString alloc] initWithUTF8String:roleCTime];
    NSString *NSSrolePower = [[NSString alloc] initWithUTF8String:rolePower];
    NSString *NSSguid = [[NSString alloc] initWithUTF8String:guid];
}

void SDKExitGame(){
    ::printf("-> SDKExitGame()\n");
}

void SDKToast(const char *msg){
    ::printf("-> SDKToast()\n");
	NSString *message = [NSString stringWithUTF8String:msg];
    
#if __IPHONE_OS_VERSION_MIN_REQUIRED < __IPHONE_9_0
    // iOS 8 及以下版本使用 UIAlertView
    UIAlertView *alert = [[UIAlertView alloc] initWithTitle:nil
                                                    message:message
                                                   delegate:nil
                                          cancelButtonTitle:nil
                                          otherButtonTitles:nil];
    [alert show];
    dispatch_after(dispatch_time(DISPATCH_TIME_NOW, (int64_t)(2.0 * NSEC_PER_SEC)), dispatch_get_main_queue(), ^{
        [alert dismissWithClickedButtonIndex:0 animated:YES];
    });
#else
    // iOS 9 及以上版本使用 UIAlertController
    UIAlertController *alertController = [UIAlertController alertControllerWithTitle:nil
                                                                             message:message
                                                                      preferredStyle:UIAlertControllerStyleAlert];
    [UIApplication.sharedApplication.delegate.window.rootViewController presentViewController:alertController animated:YES completion:nil];
    dispatch_after(dispatch_time(DISPATCH_TIME_NOW, (int64_t)(2.0 * NSEC_PER_SEC)), dispatch_get_main_queue(), ^{
        [alertController dismissViewControllerAnimated:YES completion:nil];
    });
#endif
}

#if defined(__cplusplus)
}
#endif
