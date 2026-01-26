using System;
using System.Reflection;
using BepInEx.Unity.IL2CPP;

namespace Reactor.Utilities;

/// <summary>
/// Provides singleton access to plugins instance.
/// </summary>
/// <typeparam name="T">The type of the plugin.</typeparam>
public static class PluginSingleton<T> where T : BasePlugin
{
    private static T? _instance;

    /// <summary>
    /// Gets or sets an instance of <typeparamref name="T"/> plugin.
    /// </summary>
    public static T Instance
    {
        get
        {
            if (_instance != null) return _instance;

            foreach (var plugin in IL2CPPChainloader.Instance.Plugins.Values)
            {
                if (plugin.Instance is T instance)
                {
                    _instance = instance;
                    return _instance;
                }
            }
            throw new InvalidOperationException($"Could not find instance for {typeof(T)}");
        }
        set
        {
            if (_instance == value) return;
            if (_instance != null) throw new InvalidOperationException($"Instance for {typeof(T)} is already set");
            _instance = value;
        }
    }

    internal static void Initialize()
    {
        IL2CPPChainloader.Instance.PluginLoad += (_, _, plugin) =>
        {
            typeof(PluginSingleton<>).MakeGenericType(plugin.GetType())
                .GetField(nameof(_instance), BindingFlags.Static | BindingFlags.NonPublic)!
                .SetValue(null, plugin);
        };
    }
}
