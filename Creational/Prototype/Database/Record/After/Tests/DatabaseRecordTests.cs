using Xunit;
using Prototype.Database.Record.Context;
using System;

namespace Prototype.Database.Record.Tests
{
    public class DatabaseRecordTests
    {
        private DatabaseRecord CreateSampleRecord()
        {
            var record = new DatabaseRecord
            {
                Id = 1,
                TableName = "Users"
            };

            record.SetColumn("FirstName", "John");
            record.SetColumn("LastName", "Doe");
            record.SetColumn("Email", "john@example.com");
            record.SetColumn("Age", 30);
            record.SetColumn("IsActive", true);

            record.Metadata.Version = "1.0";
            record.Metadata.Status = "Active";
            record.Metadata.Tags["Department"] = "Engineering";
            record.Metadata.AuditLog.Add("Created by admin");

            return record;
        }

        [Fact]
        public void ShallowClone_CreatesNewInstance()
        {
            var original = CreateSampleRecord();
            var clone = original.ShallowClone();

            Assert.NotSame(original, clone);
            Assert.Equal(original.Id, clone.Id);
            Assert.Equal("john@example.com", clone.GetColumn("Email"));
        }

        [Fact]
        public void ShallowClone_SharesColumnReferences()
        {
            var original = CreateSampleRecord();
            var clone = original.ShallowClone();

            // Shallow copy shares references for reference types
            Assert.Same(original.Columns["Email"], clone.Columns["Email"]);
        }

        [Fact]
        public void ShallowClone_ResetsModifiedFlag()
        {
            var original = CreateSampleRecord();
            original.IsModified = true;

            var clone = original.ShallowClone();

            Assert.True(original.IsModified);
            Assert.False(clone.IsModified);
        }

        [Fact]
        public void DeepClone_CreatesIndependentCopy()
        {
            var original = CreateSampleRecord();
            var clone = original.DeepClone();

            Assert.NotSame(original, clone);
            Assert.Equal(original.Id, clone.Id);
            Assert.Equal("john@example.com", clone.GetColumn("Email"));
        }

        [Fact]
        public void DeepClone_ColumnsAreIndependent()
        {
            var original = CreateSampleRecord();
            var clone = original.DeepClone();

            clone.SetColumn("Email", "modified@example.com");

            Assert.Equal("john@example.com", original.GetColumn("Email"));
            Assert.Equal("modified@example.com", clone.GetColumn("Email"));
        }

        [Fact]
        public void DeepClone_ModifiedFieldsAreIndependent()
        {
            var original = CreateSampleRecord();
            original.SetColumn("Age", 31);

            var clone = original.DeepClone();
            clone.SetColumn("Age", 40);

            Assert.Equal(1, original.ModifiedFields.Count);
            Assert.Equal(1, clone.ModifiedFields.Count);
        }

        [Fact]
        public void DeepClone_MetadataIsIndependent()
        {
            var original = CreateSampleRecord();
            var clone = original.DeepClone();

            clone.Metadata.Status = "Inactive";
            clone.Metadata.Tags["Department"] = "Sales";

            Assert.Equal("Active", original.Metadata.Status);
            Assert.Equal("Engineering", original.Metadata.Tags["Department"]);
        }

        [Fact]
        public void SetColumn_UpdatesModifiedFlag()
        {
            var record = CreateSampleRecord();
            record.ModifiedFields.Clear();
            record.IsModified = false;

            record.SetColumn("FirstName", "Jane");

            Assert.True(record.IsModified);
            Assert.Contains("FirstName", record.ModifiedFields);
        }

        [Fact]
        public void GetColumn_ReturnsCorrectValue()
        {
            var record = CreateSampleRecord();

            Assert.Equal("John", record.GetColumn("FirstName"));
            Assert.Equal("Doe", record.GetColumn("LastName"));
            Assert.Equal(30, record.GetColumn("Age"));
            Assert.True((bool)record.GetColumn("IsActive"));
        }

        [Fact]
        public void GetColumn_ReturnsNullForMissingColumn()
        {
            var record = CreateSampleRecord();

            Assert.Null(record.GetColumn("NonExistent"));
        }

        [Fact]
        public void SetColumn_NullName_ThrowsException()
        {
            var record = CreateSampleRecord();

            var exception = Assert.Throws<ArgumentException>(() =>
                record.SetColumn(null, "value")
            );

            Assert.Contains("Column name cannot be null or empty", exception.Message);
        }

        [Fact]
        public void GetColumn_NullName_ThrowsException()
        {
            var record = CreateSampleRecord();

            var exception = Assert.Throws<ArgumentException>(() =>
                record.GetColumn(null)
            );

            Assert.Contains("Column name cannot be null or empty", exception.Message);
        }

        [Fact]
        public void DeepClone_AuditLogIsIndependent()
        {
            var original = CreateSampleRecord();
            var clone = original.DeepClone();

            clone.Metadata.AuditLog.Add("Modified by user");

            Assert.Single(original.Metadata.AuditLog);
            Assert.Equal(2, clone.Metadata.AuditLog.Count);
        }

        [Fact]
        public void Cache_AddRecord_Success()
        {
            var cache = new RecordCache();
            var record = CreateSampleRecord();

            cache.AddRecord(record);

            Assert.Equal(1, cache.CacheSize);
        }

        [Fact]
        public void Cache_GetRecordShallow_ReturnsClone()
        {
            var cache = new RecordCache();
            var original = CreateSampleRecord();
            cache.AddRecord(original);

            var retrieved = cache.GetRecordShallow(1);

            Assert.NotSame(original, retrieved);
            Assert.Equal(1, retrieved.Id);
        }

        [Fact]
        public void Cache_GetRecordDeep_ReturnsIndependentClone()
        {
            var cache = new RecordCache();
            var original = CreateSampleRecord();
            cache.AddRecord(original);

            var retrieved = cache.GetRecordDeep(1);
            retrieved.SetColumn("Email", "new@example.com");

            Assert.Equal("john@example.com", original.GetColumn("Email"));
            Assert.Equal("new@example.com", retrieved.GetColumn("Email"));
        }

        [Fact]
        public void Cache_GetRecordNotFound_ThrowsException()
        {
            var cache = new RecordCache();

            var exception = Assert.Throws<KeyNotFoundException>(() =>
                cache.GetRecordShallow(99)
            );

            Assert.Contains("not found", exception.Message);
        }

        [Fact]
        public void Cache_HasRecord_ChecksCorrectly()
        {
            var cache = new RecordCache();
            var record = CreateSampleRecord();

            cache.AddRecord(record);

            Assert.True(cache.HasRecord(1));
            Assert.False(cache.HasRecord(99));
        }

        [Fact]
        public void Cache_InvalidateRecord_RemovesFromCache()
        {
            var cache = new RecordCache();
            var record = CreateSampleRecord();
            cache.AddRecord(record);

            cache.InvalidateRecord(1);

            Assert.False(cache.HasRecord(1));
        }

        [Fact]
        public void Cache_ClearCache_RemovesAllRecords()
        {
            var cache = new RecordCache();
            cache.AddRecord(CreateSampleRecord());

            var record2 = new DatabaseRecord { Id = 2, TableName = "Products" };
            cache.AddRecord(record2);

            Assert.Equal(2, cache.CacheSize);

            cache.ClearCache();

            Assert.Equal(0, cache.CacheSize);
        }

        [Fact]
        public void Cache_GetCacheAge_ReturnsTimespan()
        {
            var cache = new RecordCache();
            var record = CreateSampleRecord();
            cache.AddRecord(record);

            System.Threading.Thread.Sleep(100);

            var age = cache.GetCacheAge(1);

            Assert.True(age.TotalMilliseconds >= 100);
        }

        [Fact]
        public void Repository_CacheRecord_Success()
        {
            var repo = new PrototypeRepository();
            var record = CreateSampleRecord();

            repo.CacheRecord(record);

            Assert.Equal(1, repo.CacheSize);
        }

        [Fact]
        public void Repository_GetRecord_ReturnsDeepClone()
        {
            var repo = new PrototypeRepository();
            var original = CreateSampleRecord();
            repo.CacheRecord(original);

            var retrieved = repo.GetRecord(1);
            retrieved.SetColumn("Email", "new@example.com");

            Assert.Equal("john@example.com", original.GetColumn("Email"));
        }

        [Fact]
        public void Repository_GetRecordForRead_ReturnsSafe()
        {
            var repo = new PrototypeRepository();
            var record = CreateSampleRecord();
            repo.CacheRecord(record);

            var retrieved = repo.GetRecordForRead(1);

            Assert.Equal(1, retrieved.Id);
            Assert.Equal("john@example.com", retrieved.GetColumn("Email"));
        }

        [Fact]
        public void Repository_HasRecord_ChecksCorrectly()
        {
            var repo = new PrototypeRepository();
            var record = CreateSampleRecord();
            repo.CacheRecord(record);

            Assert.True(repo.HasRecord(1));
            Assert.False(repo.HasRecord(99));
        }

        [Fact]
        public void Repository_InvalidateRecord_Success()
        {
            var repo = new PrototypeRepository();
            var record = CreateSampleRecord();
            repo.CacheRecord(record);

            repo.InvalidateRecord(1);

            Assert.False(repo.HasRecord(1));
        }

        [Fact]
        public void DeepClone_DeepCopiesNestedDictionary()
        {
            var record = new DatabaseRecord { Id = 1, TableName = "Test" };
            var tags = new Dictionary<string, object>
            {
                { "Type", "Admin" },
                { "Permissions", new Dictionary<string, object> { { "Read", true } } }
            };
            record.SetColumn("Tags", tags);

            var clone = record.DeepClone();

            Assert.NotSame(record.GetColumn("Tags"), clone.GetColumn("Tags"));
        }

        [Fact]
        public void DatabaseRecord_ToString_ContainsInfo()
        {
            var record = CreateSampleRecord();
            var str = record.ToString();

            Assert.Contains("Users", str);
            Assert.Contains("1", str);
        }

        [Fact]
        public void Metadata_ShallowClone_Independent()
        {
            var original = new RecordMetadata
            {
                Version = "2.0",
                Status = "Active"
            };
            original.Tags["Role"] = "Admin";
            original.AuditLog.Add("Entry 1");

            var clone = original.ShallowClone();
            clone.Status = "Inactive";

            Assert.Equal("Active", original.Status);
            Assert.Equal("Inactive", clone.Status);
        }

        [Fact]
        public void Metadata_DeepClone_Independent()
        {
            var original = new RecordMetadata();
            original.Tags["Key"] = "Value";
            original.AuditLog.Add("Log Entry");

            var clone = original.DeepClone();
            clone.Tags["Key"] = "NewValue";
            clone.AuditLog.Add("New Log Entry");

            Assert.Equal("Value", original.Tags["Key"]);
            Assert.Single(original.AuditLog);
        }
    }
}
