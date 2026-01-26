using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using HarmonyLib;
using InnerNet;

namespace Reactor.Patches.Fixes;

[HarmonyPatch]
internal static class ServerInfoFixesPatch
{
    [HarmonyPatch(typeof(InnerNetClient), nameof(InnerNetClient.Connect))]
    [HarmonyPrefix]
    public static void LogConnect(InnerNetClient __instance)
    {
        Info($"Joining {__instance.networkAddress}:{__instance.networkPort}");
    }

    // Fixes hardcoded port and filters out IPv6 in DnsRegionInfo.
    [HarmonyPatch(typeof(DnsRegionInfo), nameof(DnsRegionInfo.PopulateServers))]
    [HarmonyPrefix]
    public static bool FixPopulateServers(DnsRegionInfo __instance)
    {
        try
        {
            var i = 0;
            var hostAddresses = Dns.GetHostAddresses(__instance.Fqdn);
            var serverList = new List<ServerInfo>();
            var addedIps = new List<IPAddress>();
            foreach (var ipAddress in hostAddresses)
            {
                if (ipAddress.AddressFamily == AddressFamily.InterNetwork)
                {
                    bool exists = false;
                    foreach (var added in addedIps)
                    {
                        if (added.Equals(ipAddress))
                        {
                            exists = true;
                            break;
                        }
                    }
                    if (!exists)
                    {
                        addedIps.Add(ipAddress);
                        serverList.Add(new ServerInfo($"{__instance.Name}-{i++}", ipAddress.ToString(), __instance.Port, __instance.UseDtls));
                    }
                }
            }
            var servers = serverList.ToArray();

            __instance.cachedServers = servers;
            var serverStrings = new StringBuilder();
            for (var j = 0; j < servers.Length; j++)
            {
                if (j > 0) serverStrings.Append(", ");
                serverStrings.Append(servers[j].ToString());
            }
            Info($"Populated {__instance.Name} ({__instance.Fqdn}:{__instance.Port}) with {servers.Length} server(s) {{{serverStrings}}}");
        }
        catch (Exception e)
        {
            Info($"Failed to populate {__instance.Name}: {e}");
            __instance.cachedServers = new[]
            {
                new ServerInfo(__instance.Name ?? string.Empty, __instance.DefaultIp, __instance.Port, __instance.UseDtls),
            };
        }

        return false;
    }
}
