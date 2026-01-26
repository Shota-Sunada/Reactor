using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using TMPro;
using UnityEngine;

namespace Reactor.Patches.Fixes;

/// <summary>
/// "Fixes" an issue where empty TextBoxes have wrong cursor positions.
/// </summary>
[HarmonyPatch]
internal static class CursorPosPatch
{
    public static IEnumerable<MethodBase> TargetMethods()
    {
        var methods = new List<MethodBase>();
        foreach (var method in AccessTools.GetDeclaredMethods(typeof(TextMeshProExtensions)))
        {
            if (method.Name == nameof(TextMeshProExtensions.CursorPos))
            {
                methods.Add(method);
            }
        }
        return methods;
    }

    public static bool Prefix(TextMeshPro self, ref Vector2 __result)
    {
        if (self.textInfo == null || self.textInfo.lineCount == 0 || self.textInfo.lineInfo[0].characterCount <= 0)
        {
            __result = self.GetTextInfo(" ").lineInfo[0].lineExtents.max;
            self.text = string.Empty;
            return false;
        }

        return true;
    }
}
