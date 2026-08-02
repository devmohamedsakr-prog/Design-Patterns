using System;
using System.Collections.Generic;

namespace Proxy.ResourceAccess.Enterprise.Component
{
    // Subject: Heavy resource interface
    public interface IDataset
    {
        string GetData();
        int GetSize();
    }

    // Real Subject: Expensive dataset
    public class HeavyDataset : IDataset
    {
        private byte[] _data;
        private string _name;

        public HeavyDataset(string name)
        {
            _name = name;
            LoadData();
        }

        private void LoadData()
        {
            // Simulate expensive initialization
            System.Threading.Thread.Sleep(500);
            _data = new byte[10_000_000]; // 10 MB
            Console.WriteLine($"  [HeavyDataset] Loaded: {_name} ({GetSize()} bytes)");
        }

        public string GetData() => $"Data from {_name}";
        public int GetSize() => _data?.Length ?? 0;
    }

    // Proxy: Controls access + manages lifecycle
    public class ResourceAccessProxy : IDataset
    {
        private IDataset _realResource;
        private string _resourceName;
        private string _userRole;
        private List<string> _accessLog;
        private bool _isLoaded;

        public ResourceAccessProxy(string resourceName, string userRole)
        {
            _resourceName = resourceName;
            _userRole = userRole;
            _accessLog = new List<string>();
            _isLoaded = false;
            Console.WriteLine($"✓ [ResourceProxy] Created for: {resourceName} (role: {userRole})");
        }

        private bool CheckAccess()
        {
            // Authorization logic
            if (_userRole == "Admin") return true;
            if (_userRole == "User" && _resourceName.StartsWith("public")) return true;
            if (_userRole == "Guest") return false;
            return false;
        }

        private void LazyLoad()
        {
            if (!_isLoaded)
            {
                if (!CheckAccess())
                {
                    _accessLog.Add($"[{DateTime.UtcNow:O}] ACCESS DENIED");
                    throw new UnauthorizedAccessException($"Access denied for {_userRole}");
                }

                Console.WriteLine($"✓ [ResourceProxy] Lazy loading resource");
                _realResource = new HeavyDataset(_resourceName);
                _isLoaded = true;
                _accessLog.Add($"[{DateTime.UtcNow:O}] LOADED");
            }
        }

        public string GetData()
        {
            LazyLoad();
            _accessLog.Add($"[{DateTime.UtcNow:O}] GET_DATA");
            return _realResource.GetData();
        }

        public int GetSize()
        {
            if (!_isLoaded) return 0;
            _accessLog.Add($"[{DateTime.UtcNow:O}] GET_SIZE");
            return _realResource.GetSize();
        }

        public void Unload()
        {
            _isLoaded = false;
            _realResource = null;
            _accessLog.Add($"[{DateTime.UtcNow:O}] UNLOADED");
            Console.WriteLine($"✓ [ResourceProxy] Unloaded resource");
        }

        public bool IsLoaded => _isLoaded;
        public IReadOnlyList<string> GetAccessLog() => _accessLog;
    }
}
