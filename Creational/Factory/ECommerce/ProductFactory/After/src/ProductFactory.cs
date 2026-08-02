using System;
using System.Collections.Generic;

namespace ProductFactory
{
    /// <summary>
    /// Factory for creating product objects
    /// Centralizes all product type creation logic
    /// </summary>
    public static class ProductFactoryClass
    {
        /// <summary>
        /// Create a product by type name
        /// ✅ Centralized creation logic
        /// ✅ Clients don't know concrete types
        /// </summary>
        public static IProduct CreateByType(string productType, Dictionary<string, object> properties)
        {
            if (string.IsNullOrEmpty(productType))
                throw new ArgumentNullException(nameof(productType));

            if (properties == null)
                throw new ArgumentNullException(nameof(properties));

            return productType.ToLower() switch
            {
                "physical" => CreatePhysicalProduct(properties),
                "digital" => CreateDigitalProduct(properties),
                "service" => CreateServiceProduct(properties),
                "subscription" => CreateSubscriptionProduct(properties),
                "bundle" => CreateBundleProduct(properties),
                _ => throw new ArgumentException($"Unknown product type: {productType}")
            };
        }

        /// <summary>Create product by enum type</summary>
        public static IProduct CreateByType(ProductType type, Dictionary<string, object> properties)
        {
            return type switch
            {
                ProductType.Physical => CreatePhysicalProduct(properties),
                ProductType.Digital => CreateDigitalProduct(properties),
                ProductType.Service => CreateServiceProduct(properties),
                ProductType.Subscription => CreateSubscriptionProduct(properties),
                ProductType.Bundle => CreateBundleProduct(properties),
                _ => throw new ArgumentException($"Unknown product type: {type}")
            };
        }

        /// <summary>Create physical product</summary>
        private static IProduct CreatePhysicalProduct(Dictionary<string, object> props)
        {
            return new PhysicalProduct
            {
                SKU = GetProperty<string>(props, "SKU", ""),
                Name = GetProperty<string>(props, "Name", ""),
                Price = GetProperty<decimal>(props, "Price", 0m),
                Weight = GetProperty<decimal>(props, "Weight", 1m),
                Dimensions = GetProperty<string>(props, "Dimensions", ""),
                InStock = GetProperty<bool>(props, "InStock", true)
            };
        }

        /// <summary>Create digital product</summary>
        private static IProduct CreateDigitalProduct(Dictionary<string, object> props)
        {
            return new DigitalProduct
            {
                SKU = GetProperty<string>(props, "SKU", ""),
                Name = GetProperty<string>(props, "Name", ""),
                Price = GetProperty<decimal>(props, "Price", 0m),
                FileFormat = GetProperty<string>(props, "FileFormat", ""),
                FileSize = GetProperty<decimal>(props, "FileSize", 0m)
            };
        }

        /// <summary>Create service product</summary>
        private static IProduct CreateServiceProduct(Dictionary<string, object> props)
        {
            return new ServiceProduct
            {
                SKU = GetProperty<string>(props, "SKU", ""),
                Name = GetProperty<string>(props, "Name", ""),
                Price = GetProperty<decimal>(props, "Price", 0m),
                DurationHours = GetProperty<decimal>(props, "DurationHours", 1m),
                ServiceType = GetProperty<string>(props, "ServiceType", ""),
                Available = GetProperty<bool>(props, "Available", true)
            };
        }

        /// <summary>Create subscription product</summary>
        private static IProduct CreateSubscriptionProduct(Dictionary<string, object> props)
        {
            return new SubscriptionProduct
            {
                SKU = GetProperty<string>(props, "SKU", ""),
                Name = GetProperty<string>(props, "Name", ""),
                Price = GetProperty<decimal>(props, "Price", 0m),
                BillingInterval = GetProperty<string>(props, "BillingInterval", "Monthly"),
                AutoRenewal = GetProperty<bool>(props, "AutoRenewal", true),
                TrialDays = GetProperty<int>(props, "TrialDays", 0)
            };
        }

        /// <summary>Create bundle product</summary>
        private static IProduct CreateBundleProduct(Dictionary<string, object> props)
        {
            var items = new List<string>();
            if (props.ContainsKey("BundleItems") && props["BundleItems"] is List<string> bundleItems)
            {
                items = bundleItems;
            }

            return new BundleProduct
            {
                SKU = GetProperty<string>(props, "SKU", ""),
                Name = GetProperty<string>(props, "Name", ""),
                Price = GetProperty<decimal>(props, "Price", 0m),
                BundleItems = items,
                BundleDiscount = GetProperty<decimal>(props, "BundleDiscount", 0m)
            };
        }

        /// <summary>Helper to safely get property from dictionary</summary>
        private static T GetProperty<T>(Dictionary<string, object> props, string key, T defaultValue)
        {
            if (props.ContainsKey(key) && props[key] is T value)
                return value;
            return defaultValue;
        }

        /// <summary>Get all available product types</summary>
        public static ProductType[] GetAllProductTypes()
        {
            return new[]
            {
                ProductType.Physical,
                ProductType.Digital,
                ProductType.Service,
                ProductType.Subscription,
                ProductType.Bundle
            };
        }

        /// <summary>Get product type description</summary>
        public static string GetProductTypeDescription(ProductType type)
        {
            return type switch
            {
                ProductType.Physical => "Physical product with shipping",
                ProductType.Digital => "Digital product with instant delivery",
                ProductType.Service => "Professional service",
                ProductType.Subscription => "Subscription with recurring billing",
                ProductType.Bundle => "Bundle of multiple products",
                _ => "Unknown type"
            };
        }
    }

    /// <summary>Enum for type-safe product type selection</summary>
    public enum ProductType
    {
        Physical = 1,
        Digital = 2,
        Service = 3,
        Subscription = 4,
        Bundle = 5
    }
}
