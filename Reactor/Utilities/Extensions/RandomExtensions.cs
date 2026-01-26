using System;
using System.Collections.Generic;

namespace Reactor.Utilities.Extensions;

/// <summary>
/// Provides Random related extension methods.
/// </summary>
public static class RandomExtensions
{
    /// <summary>
    /// Returns a random double that is within a specified range.
    /// </summary>
    /// <param name="random">The <see cref="System.Random"/> to get the random number from.</param>
    /// <param name="minValue">The minimum value of the range.</param>
    /// <param name="maxValue">The maximum value of the range.</param>
    /// <returns>A random double that is within a specified range.</returns>
    public static double NextDouble(this Random random, double minValue, double maxValue)
    {
        return random.NextDouble() * (maxValue - minValue) + minValue;
    }

    /// <summary>
    /// Returns a random <typeparamref name="T"/> from the <paramref name="input"/>.
    /// </summary>
    /// <param name="input">The input enumerable to take the item from.</param>
    /// <typeparam name="T">The type of <paramref name="input"/> items.</typeparam>
    /// <returns>A random item from the <paramref name="input"/> enumerable.</returns>
    public static T? Random<T>(this IEnumerable<T> input)
    {
        if (input is IList<T> list)
        {
            return list.Count == 0 ? default : list[UnityEngine.Random.Range(0, list.Count)];
        }

        var newList = new List<T>();
        foreach (var item in input)
        {
            newList.Add(item);
        }
        return newList.Count == 0 ? default : newList[UnityEngine.Random.Range(0, newList.Count)];
    }

    /// <summary>
    /// Returns a random <typeparamref name="T"/> from the <paramref name="input"/> using <paramref name="random"/>.
    /// </summary>
    /// <param name="input">The input enumerable to take the item from.</param>
    /// <param name="random">The <see cref="System.Random"/> to get the random number from.</param>
    /// <typeparam name="T">The type of <paramref name="input"/> items.</typeparam>
    /// <returns>A random item from the <paramref name="input"/> enumerable.</returns>
    public static T? Random<T>(this IEnumerable<T> input, Random random)
    {
        if (input is IList<T> list)
        {
            return list.Count == 0 ? default : list[random.Next(0, list.Count)];
        }

        var newList = new List<T>();
        foreach (var item in input)
        {
            newList.Add(item);
        }
        return newList.Count == 0 ? default : newList[random.Next(0, newList.Count)];
    }
}
