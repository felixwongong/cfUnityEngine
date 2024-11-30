using System;
using UnityEngine;
using ILogger = cfEngine.Logging.ILogger;

public class UnityLogger : ILogger
{
    public void LogDebug(string message, object context = null)
    {
        if (context is UnityEngine.Object unityObject)
        {
            Debug.Log(message, unityObject);
        }
        else
        {
            Debug.Log(message);
        }
    }

    public void LogInfo(string message, object context = null)
    {
        if (context is UnityEngine.Object unityObject)
        {
            Debug.Log(message, unityObject);
        }
        else
        {
            Debug.Log(message);
        }
    }

    public void Asset(bool condition, object context = null)
    {
        if (context != null)
        {
            Debug.Assert(condition);
        }
        else
        {
            Debug.Assert(condition, context);
        }
    }

    public void LogWarning(string message, object context = null)
    {
        if (context is UnityEngine.Object unityObject)
        {
            Debug.LogWarning(message, unityObject);
        }
        else
        {
            Debug.LogWarning(message);
        }
    }

    public void LogException(Exception ex, object context = null)
    {
        if (context is UnityEngine.Object unityObject)
        {
            Debug.LogException(ex, unityObject);
        }
        else
        {
            Debug.LogException(ex);
        }
    }

    public void LogError(string message, object context = null)
    {
        if (context is UnityEngine.Object unityObject)
        {
            Debug.LogError(message, unityObject);
        }
        else
        {
            Debug.LogError(message);
        }
    }
}