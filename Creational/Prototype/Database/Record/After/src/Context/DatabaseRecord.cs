using System;
using System.Collections.Generic;
using System.Linq;

namespace Prototype.Database.Record.Context
{
    /// <summary>
    /// Product: Database record with shallow/deep copy clone capability.
    /// Demonstrates: Prototype pattern for cloning cached database records safely.
    /// </summary>
    public class DatabaseRecord
    {
        public int Id { get; set; }
        public string TableName { get; set; }
        public Dictionary<string, object> Columns { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public bool IsModified { get; set; }
        public IList<string> ModifiedFields { get; set; }
        public RecordMetadata Metadata { get; set; }

        public DatabaseRecord()
        {
            Columns = new Dictionary<string, object>();
            ModifiedFields = new List<string>();
            Metadata = new RecordMetadata();
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Shallow copy - shares references to column values.
        /// </summary>
        public DatabaseRecord ShallowClone()
        {
            var clone = new DatabaseRecord
            {
                Id = this.Id,
                TableName = this.TableName,
                CreatedAt = this.CreatedAt,
                UpdatedAt = this.UpdatedAt,
                IsModified = false, // Reset modified flag
                Metadata = this.Metadata?.ShallowClone()
            };

            // Share references - faster but risky for mutable objects
            foreach (var kvp in this.Columns)
            {
                clone.Columns[kvp.Key] = kvp.Value;
            }

            return clone;
        }

        /// <summary>
        /// Deep copy - duplicates all nested objects.
        /// </summary>
        public DatabaseRecord DeepClone()
        {
            var clone = new DatabaseRecord
            {
                Id = this.Id,
                TableName = this.TableName,
                CreatedAt = this.CreatedAt,
                UpdatedAt = this.UpdatedAt,
                IsModified = false,
                Metadata = this.Metadata?.DeepClone()
            };

            // Deep copy all column values
            foreach (var kvp in this.Columns)
            {
                clone.Columns[kvp.Key] = DeepCopyValue(kvp.Value);
            }

            return clone;
        }

        private static object DeepCopyValue(object value)
        {
            if (value == null || value is string || value is int || value is bool || 
                value is DateTime || value is decimal || value is float || value is double)
            {
                return value; // Immutable types
            }

            if (value is Dictionary<string, object> dict)
            {
                var newDict = new Dictionary<string, object>();
                foreach (var kvp in dict)
                {
                    newDict[kvp.Key] = DeepCopyValue(kvp.Value);
                }
                return newDict;
            }

            if (value is List<object> list)
            {
                return new List<object>(list.Select(DeepCopyValue));
            }

            // For other reference types, try to clone if possible
            return value;
        }

        public void SetColumn(string name, object value)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Column name cannot be null or empty", nameof(name));

            Columns[name] = value;
            if (!ModifiedFields.Contains(name))
                ModifiedFields.Add(name);
            IsModified = true;
            UpdatedAt = DateTime.UtcNow;
        }

        public object GetColumn(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Column name cannot be null or empty", nameof(name));

            return Columns.ContainsKey(name) ? Columns[name] : null;
        }

        public override string ToString()
        {
            return $"DatabaseRecord(Table={TableName}, Id={Id}, Columns={Columns.Count}, " +
                   $"Modified={IsModified}, UpdatedAt={UpdatedAt:O})";
        }
    }

    /// <summary>
    /// Metadata for database records.
    /// </summary>
    public class RecordMetadata
    {
        public string Version { get; set; }
        public string Status { get; set; } // Active, Archived, Deleted
        public Dictionary<string, string> Tags { get; set; }
        public IList<string> AuditLog { get; set; }

        public RecordMetadata()
        {
            Tags = new Dictionary<string, string>();
            AuditLog = new List<string>();
        }

        public RecordMetadata ShallowClone()
        {
            return new RecordMetadata
            {
                Version = this.Version,
                Status = this.Status,
                Tags = new Dictionary<string, string>(this.Tags),
                AuditLog = new List<string>(this.AuditLog)
            };
        }

        public RecordMetadata DeepClone()
        {
            var clone = new RecordMetadata
            {
                Version = this.Version,
                Status = this.Status
            };

            foreach (var kvp in this.Tags)
            {
                clone.Tags[kvp.Key] = new string(kvp.Value.ToCharArray());
            }

            foreach (var entry in this.AuditLog)
            {
                clone.AuditLog.Add(new string(entry.ToCharArray()));
            }

            return clone;
        }

        public override string ToString() =>
            $"Metadata(Version={Version}, Status={Status}, Tags={Tags.Count}, Audit={AuditLog.Count})";
    }

    /// <summary>
    /// Cache for storing prototype records.
    /// </summary>
    public class RecordCache
    {
        private readonly Dictionary<int, DatabaseRecord> _cache =
            new Dictionary<int, DatabaseRecord>();
        private readonly Dictionary<int, DateTime> _cacheTime =
            new Dictionary<int, DateTime>();

        public void AddRecord(DatabaseRecord record)
        {
            if (record == null)
                throw new ArgumentNullException(nameof(record));

            _cache[record.Id] = record;
            _cacheTime[record.Id] = DateTime.UtcNow;
        }

        public DatabaseRecord GetRecordShallow(int id)
        {
            if (!_cache.ContainsKey(id))
                throw new KeyNotFoundException($"Record with id {id} not found in cache");

            return _cache[id].ShallowClone();
        }

        public DatabaseRecord GetRecordDeep(int id)
        {
            if (!_cache.ContainsKey(id))
                throw new KeyNotFoundException($"Record with id {id} not found in cache");

            return _cache[id].DeepClone();
        }

        public DatabaseRecord GetRecordOriginal(int id)
        {
            if (!_cache.ContainsKey(id))
                throw new KeyNotFoundException($"Record with id {id} not found in cache");

            return _cache[id];
        }

        public bool HasRecord(int id) => _cache.ContainsKey(id);

        public int CacheSize => _cache.Count;

        public TimeSpan GetCacheAge(int id)
        {
            if (!_cacheTime.ContainsKey(id))
                throw new KeyNotFoundException($"Record with id {id} not found in cache");

            return DateTime.UtcNow - _cacheTime[id];
        }

        public void InvalidateRecord(int id)
        {
            _cache.Remove(id);
            _cacheTime.Remove(id);
        }

        public void ClearCache()
        {
            _cache.Clear();
            _cacheTime.Clear();
        }
    }

    /// <summary>
    /// Repository using prototype pattern for safe record access.
    /// </summary>
    public class PrototypeRepository
    {
        private readonly RecordCache _cache = new RecordCache();

        public void CacheRecord(DatabaseRecord record)
        {
            if (record == null)
                throw new ArgumentNullException(nameof(record));

            // Store the original for future cloning
            _cache.AddRecord(record);
        }

        public DatabaseRecord GetRecord(int id)
        {
            // Return deep clone to prevent mutation of cached original
            return _cache.GetRecordDeep(id);
        }

        public DatabaseRecord GetRecordForRead(int id)
        {
            // Shallow clone is sufficient for read-only operations
            return _cache.GetRecordShallow(id);
        }

        public bool HasRecord(int id) => _cache.HasRecord(id);

        public void InvalidateRecord(int id) => _cache.InvalidateRecord(id);

        public int CacheSize => _cache.CacheSize;
    }
}
