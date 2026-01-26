using System;
using HarmonyLib;

namespace Reactor.Patches.Miscellaneous;

[HarmonyPatch]
internal static class CustomServersPatch
{
    private static bool IsCurrentServerOfficial()
    {
        const string Domain = "among.us";

        if (ServerManager.Instance.CurrentRegion?.TryCast<StaticHttpRegionInfo>() is { } regionInfo &&
               regionInfo.PingServer.EndsWith(Domain, StringComparison.Ordinal))
        {
            foreach (var serverInfo in regionInfo.Servers)
            {
                if (!serverInfo.Ip.EndsWith(Domain, StringComparison.Ordinal))
                {
                    return false;
                }
            }
            return true;
        }

        return false;
    }

    [HarmonyPatch(typeof(AuthManager._CoConnect_d__4), nameof(AuthManager._CoConnect_d__4.MoveNext))]
    [HarmonyPatch(typeof(AuthManager._CoWaitForNonce_d__6), nameof(AuthManager._CoWaitForNonce_d__6.MoveNext))]
    [HarmonyPrefix]
    public static bool DisableAuthServer(ref bool __result)
    {
        if (IsCurrentServerOfficial())
        {
            return true;
        }

        __result = false;
        return false;
    }

    [HarmonyPatch(typeof(AmongUsClient._CoJoinOnlinePublicGame_d__49), nameof(AmongUsClient._CoJoinOnlinePublicGame_d__49.MoveNext))]
    [HarmonyPrefix]
    public static void EnableUdpMatchmaking(AmongUsClient._CoJoinOnlinePublicGame_d__49 __instance)
    {
        // Skip to state 1 which just calls CoJoinOnlineGameDirect
        if (__instance.__1__state == 0 && !ServerManager.Instance.IsHttp)
        {
            __instance.__1__state = 1;
            __instance.__8__1 = new AmongUsClient.__c__DisplayClass49_0
            {
                matchmakerToken = string.Empty,
            };
        }
    }
}
