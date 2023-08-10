using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

#if BUILD_PEANUTBUTTER_INTERNAL
namespace Imported.PeanutButter.Utils.Dictionaries
#else
namespace PeanutButter.Utils.Dictionaries
#endif
{
    /// <summary>
    /// Provides a mechanism to merge multiple dictionaries into one
    /// Source dictionaries
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
#if BUILD_PEANUTBUTTER_INTERNAL
    internal
#else
    public
#endif
        class MergeDictionary<TKey, TValue>
        : IDictionary<TKey, TValue>
    {
        // ReSharper disable once StaticMemberInGenericType
        private static readonly InvalidOperationException ReadonlyException
            = new($"{typeof(MergeDictionary<,>)} is ALWAYS read-only");

        private readonly List<IDictionary<TKey, TValue>> _layers;

        /// <summary>
        /// Expose the first (or least-restrictive, for strings) key comparer
        /// </summary>
        // ReSharper disable once UnusedMember.Global
        public IEqualityComparer<TKey> Comparer => GetComparer();

        private IEqualityComparer<TKey> GetComparer()
        {
            if (typeof(TKey) == typeof(string))
            {
                return FindLeastRestrictiveStringComparer() as IEqualityComparer<TKey>;
            }
            return _layers
                .Select(l => GetPropertyValue(l, "Comparer") as IEqualityComparer<TKey>)
                .FirstOrDefault();
        }

        private IEqualityComparer<string> FindLeastRestrictiveStringComparer()
        {
            return _layers
                .Select(l => GetPropertyValue(l, "Comparer") as IEqualityComparer<string>)
                .Where(c => c != null)
                .Select(c => new
                {
                    Comparer = c,
                    Rank = StringComparerRankings.TryGetValue(c, out var rank)
                        ? rank
                        : 99
                })
                .OrderBy(o => o.Rank)
                .FirstOrDefault()
                ?.Comparer;
        }

        private static object GetPropertyValue(object src, string propertyPath)
        {
            var parts = propertyPath.Split('.');
            return parts.Aggregate(src, GetImmediatePropertyValue);
        }

        private static object GetImmediatePropertyValue(object src, string propertyName)
        {
            var type = src.GetType();
            var propInfo = type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                .FirstOrDefault(pi => pi.Name == propertyName);
            if (propInfo == null)
            {
                throw new Exception($"Member \"${propertyName}\" not found on type \"${type}\"");
            }

            return propInfo.GetValue(src, null);
        }

        // ReSharper disable once StaticMemberInGenericType
        private static readonly Dictionary<IEqualityComparer<string>, int> StringComparerRankings =
            new Dictionary<IEqualityComparer<string>, int>()
            {
                [StringComparer.OrdinalIgnoreCase] = 1,
#if NETSTANDARD
#else
                [StringComparer.InvariantCultureIgnoreCase] = 2,
#endif
                [StringComparer.CurrentCultureIgnoreCase] = 3,
#if NETSTANDARD
#else
                [StringComparer.InvariantCulture] = 4,
#endif
                [StringComparer.CurrentCulture] = 5,
                [StringComparer.Ordinal] = 6
            };

        /// <summary>
        /// Construct MergeDictionary over other dictionaries
        /// </summary>
        /// <param name="layers"></param>
        public MergeDictionary(params IDictionary<TKey, TValue>[] layers)
        {
            _layers = layers.Where(l => l != null).ToList();
            if (_layers.IsEmpty())
            {
                throw new InvalidOperationException("No non-null layers provided");
            }
        }

        /// <summary>
        /// Gets an Enumerator for the KeyValuePairs in this merged
        /// dictionary, prioritised by the order of provided layers
        /// </summary>
        /// <returns></returns>
        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return new GenericDictionaryEnumerator<TKey, TValue>(_layers.ToArray());
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Will throw - MergeDictionary is read-only
        /// </summary>
        /// <param name="item"></param>
        /// <exception cref="InvalidOperationException"></exception>
        public void Add(KeyValuePair<TKey, TValue> item)
        {
            throw ReadonlyException;
        }

        /// <summary>
        /// Will throw - MergeDictionary is read-only
        /// </summary>
        /// <exception cref="InvalidOperationException"></exception>
        public void Clear()
        {
            throw ReadonlyException;
        }

        /// <summary>
        /// Returns true if any layer contains the provided item
        /// </summary>
        /// <param name="item">Item to search for</param>
        /// <returns>True if found, False if not</returns>
        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            return _layers.Aggregate(false,
                (acc, cur) =>
                    acc || cur.Contains(item)
            );
        }

        /// <summary>
        /// Copies the prioritised items to the provided array from the given arrayIndex
        /// </summary>
        /// <param name="array">Target array to copy to</param>
        /// <param name="arrayIndex">Index to start copying at</param>
        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            using (var enumerator = GetEnumerator())
            {
                while (enumerator.MoveNext() &&
                       arrayIndex < array.Length)
                {
                    array[arrayIndex++] = enumerator.Current;
                }
            }
        }

        /// <summary>
        /// Will throw an exception - MergeDictionary is read-only
        /// </summary>
        /// <param name="item">Item to remove</param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            throw ReadonlyException;
        }

        /// <summary>
        /// Returns the count of distinct keys
        /// </summary>
        public int Count => _layers.SelectMany(kvp => kvp.Keys).Distinct().Count();

        /// <summary>
        /// Will return true: MergeDictionaries are read-only
        /// </summary>
        public bool IsReadOnly => true;

        /// <summary>
        /// Searches for the given key across all layers
        /// </summary>
        /// <param name="key">Key to search for</param>
        /// <returns>True if found, False otherwise</returns>
        public bool ContainsKey(TKey key)
        {
            return _layers.Aggregate(false, (acc, cur) => acc || cur.ContainsKey(key));
        }

        /// <summary>
        /// Will throw - MergeDictionaries are read-only
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <exception cref="InvalidOperationException"></exception>
        public void Add(TKey key, TValue value)
        {
            throw ReadonlyException;
        }

        /// <summary>
        /// Will throw - MergeDictionaries are read-only
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public bool Remove(TKey key)
        {
            throw ReadonlyException;
        }

        /// <summary>
        /// Tries to get a value by key from the underlying layers
        /// </summary>
        /// <param name="key">Key to search for</param>
        /// <param name="value">Value to search for</param>
        /// <returns>True if found, False otherwise</returns>
        public bool TryGetValue(TKey key, out TValue value)
        {
            foreach (var layer in _layers)
            {
                if (layer.TryGetValue(key, out value))
                {
                    return true;
                }
            }
            value = default(TValue);
            return false;
        }

        /// <summary>
        /// Index into the layers to find the highest-priority match for the
        /// provided key
        /// </summary>
        /// <param name="key">Key to look up</param>
        public TValue this[TKey key]
        {
            get => TryGetValue(key, out var value)
                ? value
                : throw new KeyNotFoundException(key.ToString());
            set => throw ReadonlyException;
        }

        /// <summary>
        /// Returns a collection of the distinct keys in all layers
        /// </summary>
        public ICollection<TKey> Keys => _layers.SelectMany(l => l).Select(i => i.Key).Distinct().ToArray();

        /// <summary>
        /// Returns a collection of ALL values in all layers
        /// </summary>
        public ICollection<TValue> Values => Keys.Select(k => this[k]).ToArray();
        
        // Specialist methods
        /// <summary>
        /// Append a layer to the collection
        /// - being the lowest layer, only values which are not
        ///   overridden by a higher layer will reflect
        /// </summary>
        /// <param name="layer"></param>
        public void AppendLayer(
            IDictionary<TKey, TValue> layer
        )
        {
            if (layer is null)
            {
                return;
            }
            _layers.Add(layer);
        }

        /// <summary>
        /// Insert a layer at the top of the stack
        /// </summary>
        /// <param name="layer"></param>
        public void InsertLayer(
            IDictionary<TKey, TValue> layer
        )
        {
            InsertLayer(0, layer);
        }

        /// <summary>
        /// Insert a layer at the provided index -
        /// see LayerCount for the valid range.
        /// </summary>
        /// <param name="idx"></param>
        /// <param name="layer"></param>
        public void InsertLayer(
            int idx,
            IDictionary<TKey, TValue> layer
        )
        {
            _layers.Insert(idx, layer);
        }

        /// <summary>
        /// Reflects the number of layers in this merged dictionary
        /// </summary>
        public int LayerCount => _layers.Count;
    }
}