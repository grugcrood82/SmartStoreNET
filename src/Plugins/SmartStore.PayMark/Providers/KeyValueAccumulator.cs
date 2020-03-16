using System;
using System.Collections.Generic;

namespace SmartStore.PayMark
{
    public struct KeyValueAccumulator
    {
        private Dictionary<string, string[]> _accumulator;
        private Dictionary<string, List<string>> _expandingAccumulator;

        public void Append(string key, string value)
        {
            if (_accumulator == null)
            {
                _accumulator = new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase);
            }

            if (_accumulator.TryGetValue(key, out var values))
            {
                switch (values.Length)
                {
                    case 0:
                        // Marker entry for this key to indicate entry already in expanding list dictionary
                        _expandingAccumulator[key].Add(value);
                        break;
                    case 1:
                        // Second value for this key
                        _accumulator[key] = new[] { values[0], value };
                        break;
                    default:
                    {
                        // Third value for this key
                        // Add zero count entry and move to data to expanding list dictionary
                        _accumulator[key] = default;

                        if (_expandingAccumulator == null)
                        {
                            _expandingAccumulator = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);
                        }

                        // Already 3 entries so use starting allocated as 8; then use List's expansion mechanism for more
                        var list = new List<string>(8);
                        var array = values;

                        list.Add(array[0]);
                        list.Add(array[1]);
                        list.Add(value);

                        _expandingAccumulator[key] = list;
                        break;
                    }
                }
            }
            else
            {
                // First value for this key
                _accumulator[key] = new[]{value};
            }

            ValueCount++;
        }

        public bool HasValues => ValueCount > 0;

        public int KeyCount => _accumulator?.Count ?? 0;

        public int ValueCount { get; private set; }

        public Dictionary<string, string[]> GetResults()
        {
            if (_expandingAccumulator != null)
            {
                // Coalesce count 3+ multi-value entries into _accumulator dictionary
                foreach (var entry in _expandingAccumulator)
                {
                    _accumulator[entry.Key] = entry.Value.ToArray();
                }
            }

            return _accumulator ?? new Dictionary<string, string[]>(0, StringComparer.OrdinalIgnoreCase);
        }
    }
}