using System;
using System.Collections.Generic;
using System.Linq;

namespace StockMarket.After.Context
{
    /// <summary>
    /// IStockPriceObserver: Observer interface - receives price updates
    /// </summary>
    public interface IStockPriceObserver
    {
        void OnPriceChanged(Stock stock, decimal oldPrice, decimal newPrice);
        string GetName();
    }

    /// <summary>
    /// Stock: Subject - represents a stock with observable price changes
    /// </summary>
    public class Stock
    {
        private decimal _price;
        private List<IStockPriceObserver> _observers = new();

        public string Symbol { get; set; } = "";
        public decimal Price 
        { 
            get => _price;
            set
            {
                if (value != _price)
                {
                    decimal oldPrice = _price;
                    _price = value;
                    NotifyObservers(oldPrice, value);
                }
            }
        }

        public Stock(string symbol, decimal initialPrice)
        {
            Symbol = symbol;
            _price = initialPrice;
        }

        public void Subscribe(IStockPriceObserver observer)
        {
            if (!_observers.Contains(observer))
            {
                _observers.Add(observer);
                Console.WriteLine($"  ✓ {observer.GetName()} subscribed to {Symbol}");
            }
        }

        public void Unsubscribe(IStockPriceObserver observer)
        {
            if (_observers.Remove(observer))
            {
                Console.WriteLine($"  ✗ {observer.GetName()} unsubscribed from {Symbol}");
            }
        }

        private void NotifyObservers(decimal oldPrice, decimal newPrice)
        {
            Console.WriteLine($"📊 {Symbol} price changed: ${oldPrice} → ${newPrice}");
            foreach (var observer in _observers.ToList())
            {
                observer.OnPriceChanged(this, oldPrice, newPrice);
            }
        }

        public int GetObserverCount() => _observers.Count;
        public override string ToString() => $"{Symbol}: ${_price}";
    }

    /// <summary>
    /// Investor: Concrete observer - tracks stock prices
    /// </summary>
    public class Investor : IStockPriceObserver
    {
        public string Name { get; set; }
        public List<PriceChange> PriceHistory { get; set; } = new();

        public Investor(string name)
        {
            Name = name;
        }

        public void OnPriceChanged(Stock stock, decimal oldPrice, decimal newPrice)
        {
            PriceHistory.Add(new PriceChange { Symbol = stock.Symbol, OldPrice = oldPrice, NewPrice = newPrice, Time = DateTime.Now });
            decimal change = newPrice - oldPrice;
            string direction = change > 0 ? "📈" : "📉";
            Console.WriteLine($"    {direction} {Name} notified: {stock.Symbol} ${oldPrice} → ${newPrice}");
        }

        public string GetName() => Name;
    }

    /// <summary>
    /// TradingBot: Concrete observer - automated trading
    /// </summary>
    public class TradingBot : IStockPriceObserver
    {
        public string BotName { get; set; }
        public decimal BuyThreshold { get; set; }
        public decimal SellThreshold { get; set; }
        public int TradesExecuted { get; set; } = 0;

        public TradingBot(string name, decimal buyThreshold, decimal sellThreshold)
        {
            BotName = name;
            BuyThreshold = buyThreshold;
            SellThreshold = sellThreshold;
        }

        public void OnPriceChanged(Stock stock, decimal oldPrice, decimal newPrice)
        {
            decimal percentChange = ((newPrice - oldPrice) / oldPrice) * 100;
            if (percentChange <= BuyThreshold)
            {
                Console.WriteLine($"    🤖 {BotName} BUY signal: {stock.Symbol} dropped {percentChange:F2}%");
                TradesExecuted++;
            }
            else if (percentChange >= SellThreshold)
            {
                Console.WriteLine($"    🤖 {BotName} SELL signal: {stock.Symbol} rose {percentChange:F2}%");
                TradesExecuted++;
            }
        }

        public string GetName() => BotName;
    }

    public class PriceChange
    {
        public string Symbol { get; set; } = "";
        public decimal OldPrice { get; set; }
        public decimal NewPrice { get; set; }
        public DateTime Time { get; set; }
    }
}
