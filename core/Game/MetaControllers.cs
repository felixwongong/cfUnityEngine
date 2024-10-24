using System;
using cfEngine.Core;
using cfEngine.Meta.Statistic;

public class MetaControllers : IDisposable
{
    public readonly StatisticController Statistic;

    public MetaControllers()
    {
        Statistic = new StatisticController();
    }

    public void Dispose()
    {
        Statistic?.Dispose();
    }
}