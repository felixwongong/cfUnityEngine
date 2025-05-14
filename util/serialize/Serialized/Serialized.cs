using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using UnityEngine;

namespace cfUnityEngine.Util
{
    [Serializable]
    public class Serialized<T>: ISerializationCallbackReceiver
    {
        public string typeAssemblyName;
        [SerializeReference]
        public object listObject;

        public void OnBeforeSerialize()
        {
            var type = EnsureGetInheritedType(typeof(T), typeAssemblyName);
            if (type.IsInterface)
            {
                typeAssemblyName = string.Empty;
                return;
            } 
            
            if (!type.AssemblyQualifiedName.Equals(typeAssemblyName))
            {
                typeAssemblyName = type.AssemblyQualifiedName;
            }

            if (listObject == null)
            {
                listObject = Activator.CreateInstance(type);
                return;
            }

            var listObjectType = listObject.GetType();
            if (listObjectType != type)
            {
                var newObject = Activator.CreateInstance(type);
                var fields = listObjectType.GetFields(BindingFlags.Default | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                foreach (var field in fields)
                {
                    var value = field.GetValue(listObject);
                    var newField = type.GetField(field.Name, BindingFlags.Default | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                    if (newField != null)
                    {
                        newField.SetValue(newObject, value);
                    }
                }
                listObject = newObject;
            }
        }

        public void OnAfterDeserialize()
        {
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