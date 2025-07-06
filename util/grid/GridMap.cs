using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace cfUnityEngine.Util
{
    [Serializable]
    public class GridMap<T>: IEnumerable<T>
    {
        [SerializeField]
        private Vector3Int _dimensions;
        [SerializeField]
        private List<T> _list;
        
        public Vector3Int dimensions => _dimensions;

        public GridMap(Vector3Int dimensions, Func<T> createFn)
        {
            var dimension = dimensions.x * dimensions.y * dimensions.z;
            _list = new List<T>(dimension);
            for (int i = 0; i < dimension; i++)
            {
                _list.Add(createFn());
            }
        }
        
        public T this[Vector3Int position]
        {
            get => _list[GetIndex(position)];
            set => _list[GetIndex(position)] = value;
        }
        
        public bool IsOutOfBounds(Vector3Int position)
        {
            return position.x < 0 || position.y < 0 || position.z < 0 ||
                   position.x >= _dimensions.x || position.y >= _dimensions.y || position.z >= _dimensions.z;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetIndexUnsafe(Vector3Int position)
        {
            return position.x + position.y * _dimensions.x + position.z * _dimensions.x * _dimensions.y;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetIndex(Vector3Int position)
        {
            if (IsOutOfBounds(position))
            {
                throw new ArgumentOutOfRangeException(nameof(position), $"Position ({position.ToString()}) is out of bounds of the grid map dimension ({_dimensions.ToString()}).");
            }

            return GetIndexUnsafe(position);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Vector3Int GetPositionUnsafe(int index)
        {
            int z = index / (_dimensions.x * _dimensions.y);
            int y = (index - z * _dimensions.x * _dimensions.y) / _dimensions.x;
            int x = index - z * _dimensions.x * _dimensions.y - y * _dimensions.x;
            return new Vector3Int(x, y, z);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Vector3Int GetPosition(int index)
        {
            if (index < 0 || index >= _list.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(index), $"Index {index} is out of bounds of the grid map list size {_list.Count}.");
            }
            
            return GetPositionUnsafe(index);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}