using System;
using System.Linq;

#if BUILD_PEANUTBUTTER_INTERNAL
namespace Imported.PeanutButter.RandomGenerators;
#else
namespace PeanutButter.RandomGenerators;
#endif

internal static class StringArrayExtensionsForMatchingNamespaces
{
    public static int MatchIndexFor(this string[] src, string[] other)
    {
        var shortest = Math.Min(src.Length, other.Length);
        var score = 0;
        for (var i = 0; i < shortest; i++)
        {
            score += StringCompare(src[i], other[i]);
        }

        score += src.Length - other.Length;
        return score;
    }

    private static int StringCompare(string first, string second)
    {
        if (first == second)
        {
            return 0;
        }

        var ordered = new[]
        {
            first,
            second
        }.OrderBy(s => s);
        return ordered.First() == first
            ? -1
            : 1;
    }
}