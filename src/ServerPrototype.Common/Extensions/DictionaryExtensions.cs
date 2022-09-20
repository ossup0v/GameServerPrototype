using System.Diagnostics.CodeAnalysis;

public static class DictionaryExtensions
{
    [return: MaybeNull]
    public static TValue? TryGetValue<TKey, TValue>(this Dictionary<TKey, TValue> source, TKey key)
    {
        source.TryGetValue(key, out TValue value);
        return value;
    }
}
