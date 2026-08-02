using System;
using System.Collections.Generic;

namespace TaxCalculator.After.Context
{
    public interface IProduct
    {
        void Accept(IPriceVisitor visitor);
    }

    public interface IPriceVisitor
    {
        void Visit(PhysicalProduct product);
        void Visit(DigitalProduct product);
        void Visit(ServiceProduct product);
        void Visit(BundleProduct product);
    }

    public class PhysicalProduct : IProduct
    {
        public string Name { get; set; } = "";
        public decimal Price { get; set; }
        public decimal Weight { get; set; }
        public void Accept(IPriceVisitor visitor) => visitor.Visit(this);
    }

    public class DigitalProduct : IProduct
    {
        public string Name { get; set; } = "";
        public decimal Price { get; set; }
        public void Accept(IPriceVisitor visitor) => visitor.Visit(this);
    }

    public class ServiceProduct : IProduct
    {
        public string Name { get; set; } = "";
        public decimal Price { get; set; }
        public int Hours { get; set; }
        public void Accept(IPriceVisitor visitor) => visitor.Visit(this);
    }

    public class BundleProduct : IProduct
    {
        public string Name { get; set; } = "";
        public List<IProduct> Items { get; set; } = new();

        public void Accept(IPriceVisitor visitor)
        {
            visitor.Visit(this);
            foreach (var item in Items)
                item.Accept(visitor);
        }
    }

    public class TaxCalculator : IPriceVisitor
    {
        public decimal TotalTax { get; set; } = 0;

        public void Visit(PhysicalProduct product) => TotalTax += product.Price * 0.10m;
        public void Visit(DigitalProduct product) => TotalTax += product.Price * 0.05m;
        public void Visit(ServiceProduct product) => TotalTax += product.Price * 0.08m;
        public void Visit(BundleProduct product) { }
    }

    public class DiscountCalculator : IPriceVisitor
    {
        public decimal TotalDiscount { get; set; } = 0;

        public void Visit(PhysicalProduct product) => TotalDiscount += product.Price * 0.05m;
        public void Visit(DigitalProduct product) => TotalDiscount += product.Price * 0.10m;
        public void Visit(ServiceProduct product) => TotalDiscount += product.Price * 0.15m;
        public void Visit(BundleProduct product) => TotalDiscount += product.Items.Count * 2;
    }

    public class PriceExtractor : IPriceVisitor
    {
        public decimal TotalPrice { get; set; } = 0;
        public int ItemCount { get; set; } = 0;

        public void Visit(PhysicalProduct product) { TotalPrice += product.Price; ItemCount++; }
        public void Visit(DigitalProduct product) { TotalPrice += product.Price; ItemCount++; }
        public void Visit(ServiceProduct product) { TotalPrice += product.Price; ItemCount++; }
        public void Visit(BundleProduct product) { }
    }
}
