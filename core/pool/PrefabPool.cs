using cfEngine.Pooling;
using UnityEngine;
using Object = UnityEngine.Object;

public interface IPoolableObject
{
    public void Clear();
}

public class PrefabPool<T> : ObjectPool<T> where T: Component, IPoolableObject
{
    private readonly Transform _poolRoot;

    public PrefabPool(T prefab, bool createPoolRoot, int warmupSize = 0)
        : this(prefab, createPoolRoot ? CreatePoolRoot(prefab.name): null, warmupSize)
    {
    }
    
    public PrefabPool(T prefab, Transform poolRoot, int warmupSize = 0) 
        : base(() => CreateInstance(prefab), disposed => ReleaseInstance(disposed, poolRoot), warmupSize)
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
        instance.transform.SetParent(poolRoot);
        instance.gameObject.SetActive(false);
    }
}