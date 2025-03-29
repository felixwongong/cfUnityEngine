using System;
using NUnit.Framework;
using UnityEngine;
using Object = UnityEngine.Object;

namespace cfUnityEngine.test
{
    public class PropertyBinder_Test
    {
        private class TestPropertySource : PropertySource
        {
            private int _testValue = -1;

            public int testValue
            {
                get => _testValue;
                set
                {
                    _testValue = value;
                    OnPropertyChanged(nameof(testValue), value);
                }
            }
        }
        
        private class TestPropertyResolver : MonoBehaviour, IPropertyIntResolver 
        {
            public int resolvedValue = -1;
            public void Resolve(string resolveProperty, int value)
            {
                Debug.Log($"PropertyResolver: {resolveProperty} = {value}");
                if (resolveProperty == "testValue")
                {
                    resolvedValue = value;
                }
            }
        }
        
        [Test]
        public void PropertyBinder_TestSimplePasses()
        {
            var go = new GameObject("TestObject");

            var source = new TestPropertySource();
            var resolver = go.AddComponent<TestPropertyResolver>();
            var binder = go.AddComponent<PropertyObjectBinder>();
            binder.BindSource(source);
            
            Assert.AreEqual(-1, resolver.resolvedValue);
            source.testValue = 42;
            Assert.AreEqual(42, resolver.resolvedValue);
            
            Object.DestroyImmediate(go);
        }
    }
}