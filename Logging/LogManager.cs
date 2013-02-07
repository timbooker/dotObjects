namespace dotObjects.Logging
{
    public static class LogManager
    {
        private static readonly object Moniker = new object();
        private static ILogger logger;

        public static ILogger Logger
        {
            get
            {
                lock(Moniker)
                {
                    if (logger == null)
                        logger = new DefaultLogger();
                }
                return logger;
            }
            private set { logger = value; }
        }

        public static void Define(ILogger logger)
        {
            Logger = logger;
        }
    }
}
