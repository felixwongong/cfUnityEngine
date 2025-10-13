using System;
using System.Collections;
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

        public static Task ToTask(this AsyncOperation asyncOp, MonoBehaviour coroutineHost, IProgress<float> progress)
        {
            var tcs = new TaskCompletionSource<bool>();
            coroutineHost.StartCoroutine(ReportProgress(asyncOp, progress, tcs));
            return tcs.Task;
            
            static IEnumerator ReportProgress(AsyncOperation asyncOp, IProgress<float> progress, TaskCompletionSource<bool> tcs)
            {
                while (!asyncOp.isDone)
                {
                    progress?.Report(asyncOp.progress);
                    yield return null;
                }

                if (asyncOp.progress >= 0.9f)
                    tcs.SetResult(true);
                else
                    tcs.SetException(new Exception("AsyncOperation failed to complete successfully."));
            }
        }
    }
}