namespace ServerPrototype.Common
{
    public static class ThrowHelper
    {
        public static void ThrowIfTrue(Func<bool> predicate, string message, Func<Exception> exceptionFactory = null)
        {
            if (predicate())
            {
                exceptionFactory ??= () => new Exception(message);
                throw exceptionFactory();
            }
        }

        public static void ThrowIfFalse(Func<bool> predicate, string message, Func<Exception> exceptionFactory = null)
        {
            if (!predicate())
            {
                exceptionFactory ??= () => new Exception(message);
                throw exceptionFactory();
            }
        }
    }
}
