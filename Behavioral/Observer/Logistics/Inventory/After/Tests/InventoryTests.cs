using NUnit.Framework;
using Inventory.After.Context;

namespace Inventory.After.Tests
{
    [TestFixture]
    public class InventoryObserverTests
    {
        private Product _product;
        private WarehouseManager _manager;
        private SupplierNotifier _supplier;
        private SalesAnalyzer _analyzer;

        [SetUp]
        public void Setup()
        {
            _product = new Product("P001", "Widget", 100, 20);
            _manager = new WarehouseManager("Manager");
            _supplier = new SupplierNotifier("Supplier");
            _analyzer = new SalesAnalyzer("Analyzer");
        }

        [Test]
        public void Subscribe_Observer() { _product.Subscribe(_manager); Assert.That(_product.GetObserverCount(), Is.EqualTo(1)); }

        [Test]
        public void StockLevelChange_NotifiesAll() { _product.Subscribe(_manager); _product.Subscribe(_supplier); _product.StockLevel = 50; Assert.That(_manager.Alerts.Count, Is.EqualTo(0)); }

        [Test]
        public void LowStock_Alert() { _product.Subscribe(_manager); _product.StockLevel = 15; Assert.That(_manager.Alerts.Count, Is.EqualTo(1)); }

        [Test]
        public void SupplierReorder() { _product.Subscribe(_supplier); _product.StockLevel = 10; Assert.That(_supplier.ReorderCount, Is.EqualTo(1)); }

        [Test]
        public void SalesTracking() { _product.Subscribe(_analyzer); _product.StockLevel = 80; Assert.That(_analyzer.SalesData["P001"], Is.EqualTo(20)); }

        [Test]
        public void MultiplePriceChanges() { _product.Subscribe(_analyzer); _product.StockLevel = 80; _product.StockLevel = 60; Assert.That(_analyzer.SalesData["P001"], Is.EqualTo(40)); }
    }
}
