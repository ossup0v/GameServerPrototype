public static class DebugExtensions
{
    public static string ToDebugString<T>(T[] array, string seporator = " ")
    {
        return string.Join(seporator, array);
    }
}