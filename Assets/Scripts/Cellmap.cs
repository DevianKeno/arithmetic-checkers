using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Damath
{
    /// <summary>
    /// Represents cellmap data where T is the cell's content.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Cellmap<T> : IEnumerable<T>
    {
        public Dictionary<Vector2, T> Map { get; set; }

        public T this[Vector2 coords]
        {
            get { return Map[coords]; }
            set { Map[coords] = value; }
        }

        /// <summary>
        /// Retrieve cell content.
        /// </summary>
        public T Get(Vector2 coordinate)
        {
            return Map[coordinate];
        }

        /// <summary>
        /// Replace cell content.
        /// </summary>
        public void Replace(Vector2 coordinate, T obj)
        {
            Map[coordinate] = obj;
        }

        /// <summary>
        /// Set map content.
        /// </summary>
        public void Add(Vector2 coordinate, T obj)
        {
            Map[coordinate] = obj;
        }

        public bool Contains(Vector2 coordinate)
        {
            return Map.ContainsKey(coordinate);
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            throw new System.NotImplementedException();
        }
        
        public IEnumerator GetEnumerator()
        {
            throw new System.NotImplementedException();
        }

        public static Cellmap<T> ToCellmap(Dictionary<Vector2, T> dict)
        {
            Cellmap<T> cellmap = new()
            {
                Map = dict
            };
            return cellmap;
        }

    }
}