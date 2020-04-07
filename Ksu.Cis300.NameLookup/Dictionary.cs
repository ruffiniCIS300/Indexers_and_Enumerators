/* Dictionary.cs
 * Author: Rod Howell
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KansasStateUniversity.TreeViewer2;

namespace Ksu.Cis300.NameLookup
{
    /// <summary>
    /// A generic dictionary in which keys must implement IComparable.
    /// </summary>
    /// <typeparam name="TKey">The key type.</typeparam>
    /// <typeparam name="TValue">The value type.</typeparam>
    public class Dictionary<TKey, TValue> 
    {
        /// <summary>
        /// The initial size of the table.
        /// </summary>
        private const int _initialSize = 5;

        /// <summary>
        /// The allowed table sizes.
        /// </summary>
        private int[] _tableSizes =
        {
            _initialSize, 11, 23, 47, 97, 197, 397, 797, 1597, 3203, 6421, 12853, 25717,
            51437, 102877, 205759, 411527, 823117, 1646237, 3292489, 6584983,
            13169977, 26339969, 52679969, 105359939, 210719881, 421439783,
            842879579, 1685759167
        };

        /// <summary>
        /// The index into _tableSizes of the current table size.
        /// </summary>
        private int _sizeIndex = 0;

        /// <summary>
        /// The keys and values in the dictionary.
        /// </summary>
        private LinkedListCell<KeyValuePair<TKey, TValue>>[] _elements = new LinkedListCell<KeyValuePair<TKey, TValue>>[_initialSize];

        /// <summary>
        /// Gets the number of keys in the dictionary.
        /// </summary>
        public int Count { get; private set; }

        /// <summary>
        /// Checks to see if the given key is null, and if so, throws an
        /// ArgumentNullException.
        /// </summary>
        /// <param name="key">The key to check.</param>
        private static void CheckKey(TKey key)
        {
            if (key == null)
            {
                throw new ArgumentNullException();
            }
        }

        /// <summary>
        /// Tries to get the value associated with the given key.
        /// </summary>
        /// <param name="k">The key.</param>
        /// <param name="v">The value associated with k, or the default value if
        /// k is not in the dictionary.</param>
        /// <returns>Whether k was found as a key in the dictionary.</returns>
        public bool TryGetValue(TKey k, out TValue v)
        {
            CheckKey(k);
            LinkedListCell<KeyValuePair<TKey, TValue>> p = GetCell(k, _elements[GetLocation(k)]);
            if (p == null)
            {
                v = default(TValue);
                return false;
            }
            else
            {
                v = p.Data.Value;
                return true;
            }
        }

        /// <summary>
        /// Adds the given key with the given associated value.
        /// If the given key is already in the dictionary, throws an
        /// InvalidOperationException.
        /// </summary>
        /// <param name="k">The key.</param>
        /// <param name="v">The value.</param>
        public void Add(TKey k, TValue v)
        {
            CheckKey(k);
            int loc = GetLocation(k);
            if (GetCell(k, _elements[loc]) == null)
            {
                Insert(k, v, loc);
            }
            else
            {
                throw new ArgumentException();
            }
        }

        /// <summary>
        /// Gets the hash table location where the given key belongs.
        /// </summary>
        /// <param name="k">The key.</param>
        /// <returns>The hash table location where key k belongs.</returns>
        private int GetLocation(TKey k)
        {
            return (k.GetHashCode() & 0x7fffffff) % _elements.Length;
        }

        /// <summary>
        /// Gets the cell containing the given key in the given linked list.
        /// If the linked list doesn't contain such a cell, returns null.
        /// </summary>
        /// <param name="k">The key to look for.</param>
        /// <param name="list">The linked list to look in.</param>
        /// <returns>The cell containing k, or null if no such cell exists.</returns>
        private LinkedListCell<KeyValuePair<TKey, TValue>> GetCell(TKey k, LinkedListCell<KeyValuePair<TKey, TValue>> list)
        {
            while (list != null && !k.Equals(list.Data.Key))
            {
                list = list.Next;
            }
            return list;
        }

        /// <summary>
        /// Inserts the given cell at the beginning of the linked list at the given
        /// table location.
        /// </summary>
        /// <param name="cell">The cell to insert.</param>
        /// <param name="loc">The location of the list in which to insert the cell.</param>
        private void Insert(LinkedListCell<KeyValuePair<TKey, TValue>> cell, int loc)
        {
            cell.Next = _elements[loc];
            _elements[loc] = cell;
        }

        /// <summary>
        /// Inserts the given key and value into the beginning of the linked list at
        /// the given table location.
        /// </summary>
        /// <param name="k">The key.</param>
        /// <param name="v">The value associated with k.</param>
        /// <param name="loc">The table location at which k and v are to be inserted.</param>
        private void Insert(TKey k, TValue v, int loc)
        {
            LinkedListCell<KeyValuePair<TKey, TValue>> cell = new LinkedListCell<KeyValuePair<TKey, TValue>>();
            cell.Data = new KeyValuePair<TKey, TValue>(k, v);
            Insert(cell, loc);
            Count++;
            if (Count > _elements.Length && _sizeIndex < _tableSizes.Length - 1)
            {
                _sizeIndex++;
                LinkedListCell<KeyValuePair<TKey, TValue>>[] el = _elements;
                _elements = new LinkedListCell<KeyValuePair<TKey, TValue>>[_tableSizes[_sizeIndex]];
                for (int i = 0; i < el.Length; i++)
                {
                    while(el[i] != null)
                    {
                        cell = el[i];
                        el[i] = el[i].Next;
                        Insert(cell, GetLocation(cell.Data.Key));
                    }
                }
            }
        }
    }
}
