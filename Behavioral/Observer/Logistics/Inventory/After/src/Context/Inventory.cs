using System;
using System.Collections.Generic;
using System.Linq;

namespace Inventory.After.Context
{
    public interface IInventoryObserver
    {
        void OnStockLevelChanged(Product product, int oldLevel, int newLevel);
        string GetName();
    }

    public class Product
    {
        private int _stockLevel;
        private List<IInventoryObserver> _observers = new();

        public string ProductId { get; set; } = "";
        public string Name { get; set; } = "";
        public int ReorderLevel { get; set; }

        public int StockLevel 
        { 
            get => _stockLevel;
            set
            {
                if (value != _stockLevel)
                {
                    int oldLevel = _stockLevel;
                    _stockLevel = value;
                    NotifyObservers(oldLevel, value);
                }
            }
        }

        public Product(string productId, string name, int initialStock, int reorderLevel)
        {
            ProductId = productId;
            Name = name;
            _stockLevel = initialStock;
            ReorderLevel = reorderLevel;
        }

        public void Subscribe(IInventoryObserver observer)
        {
            if (!_observers.Contains(observer))
            {
                _observers.Add(observer);
                Console.WriteLine($"  ✓ {observer.GetName()} observing {Name}");
            }
        }

        public void Unsubscribe(IInventoryObserver observer)
        {
            _observers.Remove(observer);
        }

        private void NotifyObservers(int oldLevel, int newLevel)
        {
            Console.WriteLine($"📦 {Name} stock: {oldLevel} → {newLevel}");
            foreach (var observer in _observers.ToList())
            {
                observer.OnStockLevelChanged(this, oldLevel, newLevel);
            }
        }

        public int GetObserverCount() => _observers.Count;
    }

    public class WarehouseManager : IInventoryObserver
    {
        public string ManagerName { get; set; }
        public List<string> Alerts { get; set; } = new();

        public WarehouseManager(string name)
        {
            ManagerName = name;
        }

        public void OnStockLevelChanged(Product product, int oldLevel, int newLevel)
        {
            if (newLevel <= product.ReorderLevel)
            {
                string alert = $"⚠️ {ManagerName}: {product.Name} low stock ({newLevel})";
                Alerts.Add(alert);
                Console.WriteLine($"    {alert}");
            }
        }

        public string GetName() => ManagerName;
    }

    public class SupplierNotifier : IInventoryObserver
    {
        public string SupplierName { get; set; }
        public int ReorderCount { get; set; } = 0;

        public SupplierNotifier(string name)
        {
            SupplierName = name;
        }

        public void OnStockLevelChanged(Product product, int oldLevel, int newLevel)
        {
            if (newLevel <= product.ReorderLevel)
            {
                ReorderCount++;
                Console.WriteLine($"    📞 {SupplierName} notified to reorder {product.Name}");
            }
        }

        public string GetName() => SupplierName;
    }

    public class SalesAnalyzer : IInventoryObserver
    {
        public string AnalyzerName { get; set; }
        public Dictionary<string, int> SalesData { get; set; } = new();

        public SalesAnalyzer(string name)
        {
            AnalyzerName = name;
        }

        public void OnStockLevelChanged(Product product, int oldLevel, int newLevel)
        {
            int decrease = oldLevel - newLevel;
            if (decrease > 0)
            {
                if (!SalesData.ContainsKey(product.ProductId))
                    SalesData[product.ProductId] = 0;
                SalesData[product.ProductId] += decrease;
                Console.WriteLine($"    📊 {AnalyzerName}: {product.Name} sold {decrease} units");
            }
        }

        public string GetName() => AnalyzerName;
    }
}
