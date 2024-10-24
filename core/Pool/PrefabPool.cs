using System;
using System.Collections;
using cfEngine.Pooling;
using JetBrains.Annotations;
using UnityEngine;
using Object = UnityEngine.Object;

public class PrefabPool<T> : ObjectPool<T> where T: Component
{
    [CanBeNull] 
    private readonly Transform _poolRoot;

    public PrefabPool(T prefab, bool createPoolRoot)
        : this(prefab, createPoolRoot ? CreatePoolRoot(prefab.name): null)
    {
    }
    
    public PrefabPool(T prefab, [CanBeNull] Transform poolRoot) 
        : base(() => CreateInstance(prefab), disposed => ReleaseInstance(disposed, poolRoot))
    {
        _poolRoot = poolRoot;
    }

    public override void Dispose()
    {
        while (Queue.TryDequeue(out var component))
        {
            Object.Destroy(component.gameObject);
        }
        Queue.Clear();
        if (_poolRoot != null)
        {
            Object.Destroy(_poolRoot);
        }
    }

    private static Transform CreatePoolRoot(string poolName)
    {
        GameObject poolRoot = new GameObject($"{poolName}_Pool");
        Object.DontDestroyOnLoad(poolRoot);
        return poolRoot.transform;
    }
    
    private static T CreateInstance(T prefab)
    {
        
        var instance = Object.Instantiate(prefab);
        return instance;
    }

    private static void ReleaseInstance(T instance, Transform poolRoot)
    {
        instance.gameObject.SetActive(false);
        
        NextFrame.Instance.Execute(() => instance.transform.SetParent(poolRoot));
    }
}