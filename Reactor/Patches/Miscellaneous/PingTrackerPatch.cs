using System;
using System.Text;
using HarmonyLib;
using Reactor.Utilities;

namespace Reactor.Patches.Miscellaneous;

[HarmonyPatch(typeof(PingTracker), nameof(PingTracker.Update))]
internal static class PingTrackerPatch
{
    private static string? _lastExtraText;
    private static string? _lastFullText;

    [HarmonyPostfix]
    [HarmonyPriority(Priority.Last)]
    public static void Postfix(PingTracker __instance)
    {
        var extraText = ReactorCredits.GetText(ReactorCredits.Location.PingTracker);
        if (extraText == null) return;

        var currentText = __instance.text.text;
        if (currentText == _lastFullText && extraText == _lastExtraText) return;

        if (currentText.EndsWith(extraText, StringComparison.InvariantCulture))
        {
            _lastFullText = currentText;
            _lastExtraText = extraText;
            return;
        }

        var sb = new StringBuilder(currentText);
        if (!currentText.EndsWith("\n", StringComparison.InvariantCulture))
        {
            sb.Append('\n');
        }
        sb.Append(extraText);

        var newText = sb.ToString();
        __instance.text.text = newText;

        _lastFullText = newText;
        _lastExtraText = extraText;
    }
}
