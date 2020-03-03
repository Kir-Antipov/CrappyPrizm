using System.Collections.Concurrent;

namespace CrappyPrizm.JS
{
    internal static class EnginePool
    {
        #region Var
        private const int Max = 128;
        private static readonly ConcurrentBag<Engine> Engines;
        #endregion

        #region Init
        static EnginePool()
        {
            Engines = new ConcurrentBag<Engine>();
            for (int i = 0; i < 8; ++i)
                Engines.Add(new Engine());
        }
        #endregion

        #region Functions
        public static void Release(Engine engine)
        {
            if (Engines.Count < Max)
                Engines.Add(engine);
        }

        public static Engine Get()
        {
            if (Engines.TryTake(out Engine? engine))
                return engine;

            engine = new Engine();
            if (Engines.Count < Max)
                Engines.Add(engine);
            return engine;
        }
        #endregion
    }
}
