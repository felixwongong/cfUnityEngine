using System.Threading.Tasks;
using UnityEngine;

namespace cfUnityEngine.Util
{
    public static class TaskExtension
    {
        public static Task ToTask(this AsyncOperation asyncOp)
        {
            TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();
            asyncOp.completed += aop =>
            {
                if (aop.progress >= 0.9f)
                    tcs.SetResult(true);
                else
                    tcs.SetException(new System.Exception("AsyncOperation failed to complete successfully."));
            };
            return tcs.Task;
        }
    }
}