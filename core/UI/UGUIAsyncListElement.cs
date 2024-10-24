using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using cfEngine.Logging;
using Unity.Mathematics;
using UnityEngine;

public abstract class UGUIAsyncListElementBase<TItem, TData> : UGUIAsyncDataElement<IReadOnlyList<TData>> where TItem: UGUIAsyncDataElement<TData>
{
    [SerializeField] protected TItem _itemPrefab;
    [SerializeField] private bool releaseOnDisable = false;

    protected readonly List<TItem> _items = new();

    private void Awake()
    {
        if (_itemPrefab == null)
        {
            enabled = false;
            Log.LogException(new ArgumentNullException(nameof(_itemPrefab), "Item Prefab unset"));
            return;
        }
        
        var childCount = transform.childCount;
        for (int i = 0; i < childCount; i++)
        {
            var child = transform.GetChild(i);
            if (child.TryGetComponent<TItem>(out var item))
            {
                _items.Add(item);
            }
        }
    }
    
    private void OnDisable()
    {
        if (releaseOnDisable && _items.Count > 0)
        {
            var pool = Game.Pool.GetOrCreatPool(_itemPrefab.name, () => new PrefabPool<TItem>(_itemPrefab, true));
            foreach (var item in _items)
            {
                pool.Release(item);
            }
            
            _items.Clear();
        }
    }
}
public abstract class UGUIAsyncListElement<TItem, TData> : UGUIAsyncListElementBase<TItem, TData> where TItem : UGUIAsyncDataElement<TData>
{
    public override Task SetDataAsync(IReadOnlyList<TData> data)
    {
        for (var i = data.Count; i < _items.Count; i++)
        {
            _items[i].gameObject.SetActive(false);
        }

        var tasks = new Task[data.Count];
        for (var i = 0; i < data.Count; i++)
        {
            TItem item;
            if (i < _items.Count)
            {
                item = _items[i];
            }
            else
            {
                var pool = Game.Pool.GetOrCreatPool(_itemPrefab.name, () => new PrefabPool<TItem>(_itemPrefab, true));
                item = pool.Get();
                var itemTransform = item.transform;
                itemTransform.SetParent(transform);
                itemTransform.localPosition = Vector3.zero;
                itemTransform.localRotation = quaternion.identity;
                itemTransform.localScale = Vector3.one;
                _items.Add(item);
            }
            
            item.gameObject.SetActive(true); 
            tasks[i] = item.SetDataAsync(data[i]);
        }

        return Task.WhenAll(tasks);
    }
}

