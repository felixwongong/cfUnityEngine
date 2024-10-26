using System;

namespace cfEngine.Meta
{
    public class MetaControllers : IDisposable
    {
#if CF_STATISTIC
        public readonly StatisticController Statistic;
#endif

        public MetaControllers()
        {
#if CF_STATISTIC
            Statistic = new StatisticController();
#endif
        }

        public void Dispose()
        {
#if CF_STATISTIC
            Statistic?.Dispose();
#endif
        }
    }
}