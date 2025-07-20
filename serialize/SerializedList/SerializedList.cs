using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using cfEngine.Extension;
using UnityEngine;
using Object = System.Object;

namespace cfUnityEngine.Util
{
    [Serializable]
    public partial class SerializedList<T>: IList<T>, ISerializationCallbackReceiver where T: class
    {
        private List<T> _list;
        [SerializeField]
        private List<Serialized<T>> _serializedList;
        
        public SerializedList()
        {
            _list = new();
        }

        public SerializedList(int capacity)
        {
            _list = new List<T>(capacity);
        }
        
        public IEnumerator<T> GetEnumerator() => _list.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        public void Add(T item) { _list.Add(item); }
        public void Clear() { _list.Clear(); }
        public bool Contains(T item) => _list.Contains(item);
        public void CopyTo(T[] array, int arrayIndex) { _list.CopyTo(array, arrayIndex); }
        public bool Remove(T item) => _list.Remove(item);
        public int Count => _list.Count;
        public bool IsReadOnly => false;
        public int IndexOf(T item) => _list.IndexOf(item);
        public void Insert(int index, T item) { _list.Insert(index, item); }
        public void RemoveAt(int index) { _list.RemoveAt(index); }
        public T this[int index] { get => _list[index]; set => _list[index] = value; }
        public void EnsureCapacity(int capacity) { _list.EnsureCapacity(capacity); }

        public void OnBeforeSerialize()
        {
        }

        public void OnAfterDeserialize()
        {
            var listObjectField = typeof(Serialized<T>).GetField("_listObject", BindingFlags.Instance | BindingFlags.NonPublic);
            if (listObjectField == null)
            {
                Debug.LogError("SerializedList: Cannot find field '_listObject' in Serialized<T> class. Make sure the Serialized<T> class is properly defined.");
                return;
            }
            
            if (_serializedList != null)
            {
                _list ??= new List<T>();
                _list.EnsureCapacity(_serializedList.Count);
                for (var i = 0; i < _serializedList.Count; i++)
                {
                    
                    var item = (T)listObjectField.GetValue(_serializedList[i]);
                    if (_list.Count <= i)
                    {
                        _list.Add(item);
                    }
                    else
                    {
                        _list[i] = item;
                    }
                }
            }
        }
    }
}