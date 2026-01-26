using System.Collections.Generic;
using System.Linq;
using BepInEx.Configuration;
using Reactor.Utilities;
using UnityEngine;

namespace Reactor.Debugger.Window.Tabs;

internal sealed class ConfigTab : BaseTab
{
    public override string Name => "Config";

    private List<IGrouping<string, KeyValuePair<ConfigDefinition, ConfigEntryBase>>>? _cachedConfigs;

    public override void OnGUI()
    {
        _cachedConfigs ??= PluginSingleton<ReactorPlugin>.Instance.Config
            .Concat(PluginSingleton<DebuggerPlugin>.Instance.Config)
            .GroupBy(e => e.Key.Section)
            .ToList();

        foreach (var section in _cachedConfigs)
        {
            GUILayout.Label(section.Key);

            foreach (var (definition, entry) in section)
            {
                if (entry is ConfigEntry<bool> booleanEntry)
                {
                    booleanEntry.Value = GUILayout.Toggle(booleanEntry.Value, definition.Key);
                }
            }
        }
    }
}
