using AmongUs.GameOptions;
using HarmonyLib;
using Il2CppInterop.Runtime;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using Il2CppSystem.Reflection;
using UnityEngine;

namespace Reactor.Debugger.Patches;

[HarmonyPatch]
internal static class GameOptionsPatches
{
    public static void Initialize()
    {
        var maxImpostors = new Il2CppStructArray<int>(byte.MaxValue);
        for (int i = 0; i < maxImpostors.Length; i++) maxImpostors[i] = byte.MaxValue;
        NormalGameOptionsV09.MaxImpostors = maxImpostors;

        var minPlayers = new Il2CppStructArray<int>(byte.MaxValue);
        for (int i = 0; i < minPlayers.Length; i++) minPlayers[i] = 1;
        NormalGameOptionsV09.MinPlayers = minPlayers;
    }

    [HarmonyPatch(typeof(GameSettingMenu), nameof(GameSettingMenu.Start))]
    [HarmonyPrefix]
    public static void UnlockAllOptions(GameSettingMenu __instance)
    {
        __instance.GameSettingsTab.HideForOnline = new Il2CppReferenceArray<Transform>(0);
    }

    [HarmonyPatch(typeof(NumberOption), nameof(NumberOption.SetUpFromData))]
    [HarmonyPostfix]
    public static void UnlockOptionRange(NumberOption __instance)
    {
        __instance.ValidRange.min = float.MinValue;
        __instance.ValidRange.max = float.MaxValue;
    }

    [HarmonyPatch(typeof(CreateOptionsPicker), nameof(CreateOptionsPicker.SetImpostorButtons))]
    public static class DisableImpostorCountReset
    {
        private static readonly MethodInfo _refreshMethod = Il2CppType.Of<CreateOptionsPicker>().GetMethod("Refresh", BindingFlags.Public | BindingFlags.Instance);

        public static bool Prefix()
        {
            foreach (var stackFrame in new Il2CppSystem.Diagnostics.StackTrace().GetFrames())
            {
                if (_refreshMethod.Equals(stackFrame.GetMethod()))
                {
                    return false;
                }
            }

            return true;
        }
    }

    [HarmonyPatch(typeof(CrewVisualizer), nameof(CrewVisualizer.SetCrewSize))]
    [HarmonyPrefix]
    public static void SetCrewSizePatch(int numPlayers, ref int numImpostors)
    {
        if (numImpostors >= numPlayers)
        {
            numImpostors = 0;
        }
    }
}
