
#if defined(__cplusplus)
extern "C"{
#endif

	extern void SDKInit();
    extern void SDKLogin();
    extern void SDKLogout();
    extern void SDKSwitchAccount();
    extern void SDKLoadVideoAd(const char *id, const char *userId);
    extern void SDKShowVideoAd(const char *id, const char *userId);
    extern void SDKPay(const char *serverNum, const char *serverName, const char *playerId, const char *playerName, const char *amount, const char *extra, const char *orderId, const char *productName, const char *productContent, const char *playerLevel, const char *sign, const char *guid);
    extern void SDKUpdateRole(const char scenes,const char *serverId, const char *serverName, const char *roleId, const char *roleName, const char *roleLeve, const char *roleCTime,const char *rolePower,const char *guid);
	extern void SDKExitGame();
	extern void SDKToast(const char *msg);

#if defined(__cplusplus)
}
#endif
