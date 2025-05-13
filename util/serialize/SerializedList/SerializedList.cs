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
    public class SerializedList<T>: IList<T>, ISerializationCallbackReceiver where T: class
    {
        private List<T> _list;
        [SerializeField]
        private List<ItemWrapper> _serializedList;
        
        public SerializedList()
        {
        }

        public SerializedList(int capacity): this()
        {
            _list ??= new List<T>(capacity);
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

        [Serializable]
        public class ItemWrapper
        {
            public string typeAssemblyName;
            [SerializeReference]
            public object listObject;
        }

        
        public void OnBeforeSerialize()
        {
            if (_serializedList != null)
            {
                foreach (var serializedSetting in _serializedList)
                {
                    var type = EnsureGetInheritedType(typeof(T), serializedSetting.typeAssemblyName);
                    if (!type.AssemblyQualifiedName.Equals(serializedSetting.typeAssemblyName))
                    {
                        serializedSetting.typeAssemblyName = type.AssemblyQualifiedName;
                    }

                    if (serializedSetting.listObject == null)
                    {
                        serializedSetting.listObject = Activator.CreateInstance(type);
                        continue;
                    }

                    var listObjectType = serializedSetting.listObject.GetType();
                    if (listObjectType != type)
                    {
                        var newObject = Activator.CreateInstance(type);
                        var fields = listObjectType.GetFields(BindingFlags.Default | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                        foreach (var field in fields)
                        {
                            var value = field.GetValue(serializedSetting.listObject);
                            var newField = type.GetField(field.Name, BindingFlags.Default | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                            if (newField != null)
                            {
                                newField.SetValue(newObject, value);
                            }
                        }
                        serializedSetting.listObject = newObject;
                    }
                }
            }
        }

        public void OnAfterDeserialize()
        {
            if (_serializedList != null)
            {
                _list ??= new List<T>();
                _list.EnsureCapacity(_serializedList.Count);
                for (var i = 0; i < _serializedList.Count; i++)
                {
                    var item = (T)_serializedList[i].listObject;
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
        
        private Type EnsureGetInheritedType([NotNull] Type baseType, string inheritedTypeName)
        {
            if (string.IsNullOrEmpty(inheritedTypeName))
                return baseType;
            
            var type = Type.GetType(inheritedTypeName);
            if (type == null)
                return baseType;

            return baseType.IsAssignableFrom(type) ? type : baseType;
        }
    }
}