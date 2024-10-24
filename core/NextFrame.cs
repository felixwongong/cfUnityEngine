using System;
using System.Collections.Generic;

public class NextFrame: MonoInstance<NextFrame>
{
    public override bool persistent => true;

    private int nextFrameExecutionActionCount = -1;
    private Queue<Action> _frameAction = new();

    private void Update()
    {
        if (nextFrameExecutionActionCount > 0)
        {
            var i = 0;
            while (i < nextFrameExecutionActionCount && _frameAction.TryDequeue(out var action))
            {
               action();
               i++;
            }
        }

        if (_frameAction.Count > 0)
        {
            nextFrameExecutionActionCount = _frameAction.Count;
        }
        else
        {
            nextFrameExecutionActionCount = -1;
        }
    }

    public void Execute(Action action)
    {
        _frameAction.Enqueue(action);
    }
}