using NUnit.Framework;
using StockMarket.After.Context;

namespace StockMarket.After.Tests
{
    [TestFixture]
    public class StockMarketObserverTests
    {
        private Stock _stock;
        private Investor _investor1, _investor2;
        private TradingBot _bot;

        [SetUp]
        public void Setup()
        {
            _stock = new Stock("ACME", 100m);
            _investor1 = new Investor("Alice");
            _investor2 = new Investor("Bob");
            _bot = new TradingBot("AutoTrader", -5m, 5m);
        }

        [Test]
        public void Subscribe_Observer()
        {
            _stock.Subscribe(_investor1);
            Assert.That(_stock.GetObserverCount(), Is.EqualTo(1));
        }

        [Test]
        public void MultipleSubscribers()
        {
            _stock.Subscribe(_investor1);
            _stock.Subscribe(_investor2);
            _stock.Subscribe(_bot);
            Assert.That(_stock.GetObserverCount(), Is.EqualTo(3));
        }

        [Test]
        public void PriceChange_NotifiesAll()
        {
            _stock.Subscribe(_investor1);
            _stock.Subscribe(_investor2);
            
            _stock.Price = 105m;
            
            Assert.That(_investor1.PriceHistory.Count, Is.EqualTo(1));
            Assert.That(_investor2.PriceHistory.Count, Is.EqualTo(1));
        }

        [Test]
        public void Unsubscribe_Observer()
        {
            _stock.Subscribe(_investor1);
            _stock.Unsubscribe(_investor1);
            Assert.That(_stock.GetObserverCount(), Is.EqualTo(0));
        }

        [Test]
        public void PriceHistory_Tracking()
        {
            _stock.Subscribe(_investor1);
            _stock.Price = 102m;
            _stock.Price = 105m;
            _stock.Price = 103m;
            
            Assert.That(_investor1.PriceHistory.Count, Is.EqualTo(3));
            Assert.That(_investor1.PriceHistory[0].NewPrice, Is.EqualTo(102m));
            Assert.That(_investor1.PriceHistory[1].NewPrice, Is.EqualTo(105m));
            Assert.That(_investor1.PriceHistory[2].NewPrice, Is.EqualTo(103m));
        }

        [Test]
        public void TradingBot_BuySignal()
        {
            _stock.Subscribe(_bot);
            _stock.Price = 95m; // 5% drop
            
            Assert.That(_bot.TradesExecuted, Is.EqualTo(1));
        }

        [Test]
        public void TradingBot_SellSignal()
        {
            _stock.Subscribe(_bot);
            _stock.Price = 105m; // 5% rise
            
            Assert.That(_bot.TradesExecuted, Is.EqualTo(1));
        }

        [Test]
        public void NoNotification_SamePrice()
        {
            _stock.Subscribe(_investor1);
            _stock.Price = 100m; // Same price
            
            Assert.That(_investor1.PriceHistory.Count, Is.EqualTo(0));
        }

        [Test]
        public void AfterUnsubscribe_NoNotifications()
        {
            _stock.Subscribe(_investor1);
            _stock.Unsubscribe(_investor1);
            _stock.Price = 110m;
            
            Assert.That(_investor1.PriceHistory.Count, Is.EqualTo(0));
        }

        [Test]
        public void MultiplePriceChanges()
        {
            _stock.Subscribe(_investor1);
            _stock.Subscribe(_investor2);
            
            for (decimal price = 100m; price <= 110m; price += 1m)
            {
                _stock.Price = price;
            }
            
            Assert.That(_investor1.PriceHistory.Count, Is.EqualTo(10));
            Assert.That(_investor2.PriceHistory.Count, Is.EqualTo(10));
        }

        [Test]
        public void ResubscribeAfterUnsubscribe()
        {
            _stock.Subscribe(_investor1);
            _stock.Unsubscribe(_investor1);
            _stock.Subscribe(_investor1);
            
            _stock.Price = 105m;
            Assert.That(_investor1.PriceHistory.Count, Is.EqualTo(1));
        }

        [Test]
        public void LargePercentageChange()
        {
            _stock.Subscribe(_bot);
            _stock.Price = 50m; // 50% drop
            
            Assert.That(_bot.TradesExecuted, Is.GreaterThan(0));
        }

        [Test]
        public void SmallPriceChange_NoBotSignal()
        {
            _bot.BuyThreshold = -10m;
            _bot.SellThreshold = 10m;
            _stock.Subscribe(_bot);
            
            _stock.Price = 102m; // 2% change
            Assert.That(_bot.TradesExecuted, Is.EqualTo(0));
        }
    }
}
