using NUnit.Framework;
using System.Collections.Generic;
using ProductFactory;

namespace ProductFactory.Tests
{
    [TestFixture]
    public class ProductFactoryTests
    {
        #region Factory Creation Tests

        [TestFixture]
        public class FactoryCreationTests
        {
            [Test]
            public void CreateByType_Physical_ReturnsPhysicalProduct()
            {
                var props = new Dictionary<string, object>
                {
                    { "SKU", "PHYS-001" },
                    { "Name", "Laptop" },
                    { "Price", 999m },
                    { "Weight", 3.5m }
                };

                var product = ProductFactoryClass.CreateByType("physical", props);
                Assert.That(product, Is.InstanceOf<PhysicalProduct>());
                Assert.That(product.Name, Is.EqualTo("Laptop"));
            }

            [Test]
            public void CreateByType_Digital_ReturnsDigitalProduct()
            {
                var props = new Dictionary<string, object>
                {
                    { "SKU", "DIGI-001" },
                    { "Name", "eBook" },
                    { "Price", 9.99m },
                    { "FileFormat", "PDF" }
                };

                var product = ProductFactoryClass.CreateByType("digital", props);
                Assert.That(product, Is.InstanceOf<DigitalProduct>());
            }

            [Test]
            public void CreateByType_Service_ReturnsServiceProduct()
            {
                var props = new Dictionary<string, object>
                {
                    { "SKU", "SERV-001" },
                    { "Name", "Consulting" },
                    { "Price", 150m },
                    { "DurationHours", 2m }
                };

                var product = ProductFactoryClass.CreateByType("service", props);
                Assert.That(product, Is.InstanceOf<ServiceProduct>());
            }

            [Test]
            public void CreateByType_Subscription_ReturnsSubscriptionProduct()
            {
                var props = new Dictionary<string, object>
                {
                    { "SKU", "SUB-001" },
                    { "Name", "Premium Plan" },
                    { "Price", 9.99m },
                    { "BillingInterval", "Monthly" }
                };

                var product = ProductFactoryClass.CreateByType("subscription", props);
                Assert.That(product, Is.InstanceOf<SubscriptionProduct>());
            }

            [Test]
            public void CreateByType_Bundle_ReturnsBundleProduct()
            {
                var props = new Dictionary<string, object>
                {
                    { "SKU", "BUND-001" },
                    { "Name", "Starter Bundle" },
                    { "Price", 49.99m },
                    { "BundleDiscount", 10m }
                };

                var product = ProductFactoryClass.CreateByType("bundle", props);
                Assert.That(product, Is.InstanceOf<BundleProduct>());
            }

            [Test]
            public void CreateByType_InvalidType_ThrowsException()
            {
                var props = new Dictionary<string, object>();
                Assert.Throws<ArgumentException>(() =>
                    ProductFactoryClass.CreateByType("invalid", props));
            }

            [Test]
            public void CreateByType_NullType_ThrowsException()
            {
                var props = new Dictionary<string, object>();
                Assert.Throws<ArgumentNullException>(() =>
                    ProductFactoryClass.CreateByType(null, props));
            }

            [Test]
            public void CreateByType_CaseInsensitive_ReturnsCorrectType()
            {
                var props = new Dictionary<string, object> { { "SKU", "TEST" } };
                var product1 = ProductFactoryClass.CreateByType("PHYSICAL", props);
                var product2 = ProductFactoryClass.CreateByType("PhYsIcAl", props);

                Assert.That(product1, Is.InstanceOf<PhysicalProduct>());
                Assert.That(product2, Is.InstanceOf<PhysicalProduct>());
            }
        }

        #endregion

        #region Physical Product Tests

        [TestFixture]
        public class PhysicalProductTests
        {
            [Test]
            public void PhysicalProduct_Shipping_CalculatedByWeight()
            {
                var product = new PhysicalProduct { Weight = 2m };
                var shipping = product.CalculateShippingCost();
                Assert.That(shipping, Is.EqualTo(10m)); // 2 * $5
            }

            [Test]
            public void PhysicalProduct_Tax_VariesByLocation()
            {
                var product = new PhysicalProduct { Price = 100m };
                var taxCA = product.CalculateTax("CA");
                var taxNY = product.CalculateTax("NY");
                
                Assert.That(taxCA, Is.EqualTo(7.25m));
                Assert.That(taxNY, Is.EqualTo(8m));
            }

            [Test]
            public void PhysicalProduct_Fulfillment_IsWarehouse()
            {
                var product = new PhysicalProduct();
                Assert.That(product.GetFulfillmentMethod(), Is.EqualTo("Warehouse Fulfillment"));
            }

            [Test]
            public void PhysicalProduct_Delivery_Is3to5Days()
            {
                var product = new PhysicalProduct();
                Assert.That(product.GetDeliveryEstimate(), Contains.Substring("3-5"));
            }

            [Test]
            public void PhysicalProduct_InStock_ReturnsStockStatus()
            {
                var inStock = new PhysicalProduct { InStock = true };
                var outOfStock = new PhysicalProduct { InStock = false };

                Assert.That(inStock.IsInStock(), Is.True);
                Assert.That(outOfStock.IsInStock(), Is.False);
            }
        }

        #endregion

        #region Digital Product Tests

        [TestFixture]
        public class DigitalProductTests
        {
            [Test]
            public void DigitalProduct_Shipping_IsFree()
            {
                var product = new DigitalProduct();
                var shipping = product.CalculateShippingCost();
                Assert.That(shipping, Is.EqualTo(0m));
            }

            [Test]
            public void DigitalProduct_Tax_IsZero()
            {
                var product = new DigitalProduct { Price = 100m };
                var tax = product.CalculateTax("CA");
                Assert.That(tax, Is.EqualTo(0m));
            }

            [Test]
            public void DigitalProduct_Fulfillment_IsInstantDownload()
            {
                var product = new DigitalProduct();
                Assert.That(product.GetFulfillmentMethod(), Is.EqualTo("Instant Download"));
            }

            [Test]
            public void DigitalProduct_Delivery_IsInstant()
            {
                var product = new DigitalProduct();
                Assert.That(product.GetDeliveryEstimate(), Contains.Substring("Instant"));
            }

            [Test]
            public void DigitalProduct_InStock_AlwaysTrue()
            {
                var product = new DigitalProduct();
                Assert.That(product.IsInStock(), Is.True);
            }
        }

        #endregion

        #region Service Product Tests

        [TestFixture]
        public class ServiceProductTests
        {
            [Test]
            public void ServiceProduct_Shipping_IsFree()
            {
                var product = new ServiceProduct();
                var shipping = product.CalculateShippingCost();
                Assert.That(shipping, Is.EqualTo(0m));
            }

            [Test]
            public void ServiceProduct_Tax_VariesByLocation()
            {
                var product = new ServiceProduct { Price = 100m };
                var taxCA = product.CalculateTax("CA");
                var taxNY = product.CalculateTax("NY");

                Assert.That(taxCA, Is.EqualTo(8.25m));
                Assert.That(taxNY, Is.EqualTo(8.5m));
            }

            [Test]
            public void ServiceProduct_Fulfillment_IsServiceDelivery()
            {
                var product = new ServiceProduct();
                Assert.That(product.GetFulfillmentMethod(), Is.EqualTo("Service Delivery"));
            }

            [Test]
            public void ServiceProduct_Delivery_IncludesDuration()
            {
                var product = new ServiceProduct { DurationHours = 2m };
                var delivery = product.GetDeliveryEstimate();
                Assert.That(delivery, Contains.Substring("2"));
            }

            [Test]
            public void ServiceProduct_Available_ReturnsAvailabilityStatus()
            {
                var available = new ServiceProduct { Available = true };
                var unavailable = new ServiceProduct { Available = false };

                Assert.That(available.IsInStock(), Is.True);
                Assert.That(unavailable.IsInStock(), Is.False);
            }
        }

        #endregion

        #region Subscription Product Tests

        [TestFixture]
        public class SubscriptionProductTests
        {
            [Test]
            public void SubscriptionProduct_Shipping_IsFree()
            {
                var product = new SubscriptionProduct();
                var shipping = product.CalculateShippingCost();
                Assert.That(shipping, Is.EqualTo(0m));
            }

            [Test]
            public void SubscriptionProduct_Fulfillment_IsSubscription()
            {
                var product = new SubscriptionProduct();
                Assert.That(product.GetFulfillmentMethod(), Contains.Substring("Subscription"));
            }

            [Test]
            public void SubscriptionProduct_Delivery_IncludesBillingInterval()
            {
                var product = new SubscriptionProduct { BillingInterval = "Monthly" };
                var delivery = product.GetDeliveryEstimate();
                Assert.That(delivery, Contains.Substring("Monthly"));
            }

            [Test]
            public void SubscriptionProduct_Delivery_WithTrial_ShowsTrialDays()
            {
                var product = new SubscriptionProduct 
                { 
                    BillingInterval = "Monthly",
                    TrialDays = 7
                };
                var delivery = product.GetDeliveryEstimate();
                Assert.That(delivery, Contains.Substring("7"));
            }

            [Test]
            public void SubscriptionProduct_InStock_AlwaysTrue()
            {
                var product = new SubscriptionProduct();
                Assert.That(product.IsInStock(), Is.True);
            }
        }

        #endregion

        #region Bundle Product Tests

        [TestFixture]
        public class BundleProductTests
        {
            [Test]
            public void BundleProduct_Shipping_CalculatedByWeight()
            {
                var product = new BundleProduct();
                var shipping = product.CalculateShippingCost(3m);
                Assert.That(shipping, Is.EqualTo(15m)); // 3 * $5
            }

            [Test]
            public void BundleProduct_Tax_VariesByLocation()
            {
                var product = new BundleProduct { Price = 100m };
                var taxCA = product.CalculateTax("CA");
                var taxNY = product.CalculateTax("NY");

                Assert.That(taxCA, Is.EqualTo(7.25m));
                Assert.That(taxNY, Is.EqualTo(8m));
            }

            [Test]
            public void BundleProduct_Delivery_Is3to5Days()
            {
                var product = new BundleProduct();
                Assert.That(product.GetDeliveryEstimate(), Contains.Substring("3-5"));
            }

            [Test]
            public void BundleProduct_Description_IncludesItemCount()
            {
                var product = new BundleProduct 
                { 
                    BundleItems = new List<string> { "Item1", "Item2", "Item3" },
                    BundleDiscount = 15m
                };
                var description = product.GetProductTypeDescription();
                Assert.That(description, Contains.Substring("3"));
                Assert.That(description, Contains.Substring("15"));
            }
        }

        #endregion

        #region Cart Processing Tests

        [TestFixture]
        public class CartProcessingTests
        {
            private CartProcessor processor;

            [SetUp]
            public void Setup()
            {
                processor = new CartProcessor();
            }

            [Test]
            public void ProcessCart_MixedProducts_CalculatesCorrectly()
            {
                var items = new List<CartItem>
                {
                    new CartItem
                    {
                        ProductType = "physical",
                        Quantity = 1,
                        Weight = 2m,
                        Properties = new Dictionary<string, object>
                        {
                            { "SKU", "PHYS-001" },
                            { "Name", "Book" },
                            { "Price", 20m },
                            { "Weight", 2m }
                        }
                    },
                    new CartItem
                    {
                        ProductType = "digital",
                        Quantity = 1,
                        Properties = new Dictionary<string, object>
                        {
                            { "SKU", "DIGI-001" },
                            { "Name", "eBook" },
                            { "Price", 10m }
                        }
                    }
                };

                var summary = processor.ProcessCart(items, "CA");
                Assert.That(summary.Subtotal, Is.EqualTo(30m));
                Assert.That(summary.Shipping, Is.GreaterThan(0m)); // Physical has shipping
            }

            [Test]
            public void ProcessCart_DigitalProduct_NoShipping()
            {
                var items = new List<CartItem>
                {
                    new CartItem
                    {
                        ProductType = "digital",
                        Quantity = 2,
                        Properties = new Dictionary<string, object>
                        {
                            { "SKU", "DIGI-001" },
                            { "Name", "eBook" },
                            { "Price", 10m }
                        }
                    }
                };

                var summary = processor.ProcessCart(items, "CA");
                Assert.That(summary.Shipping, Is.EqualTo(0m));
                Assert.That(summary.Tax, Is.EqualTo(0m)); // No tax on digital
            }

            [Test]
            public void ProcessCart_ServiceProduct_NoShipping()
            {
                var items = new List<CartItem>
                {
                    new CartItem
                    {
                        ProductType = "service",
                        Quantity = 1,
                        Properties = new Dictionary<string, object>
                        {
                            { "SKU", "SERV-001" },
                            { "Name", "Consulting" },
                            { "Price", 100m },
                            { "DurationHours", 2m }
                        }
                    }
                };

                var summary = processor.ProcessCart(items, "NY");
                Assert.That(summary.Shipping, Is.EqualTo(0m));
                Assert.That(summary.Tax, Is.EqualTo(8.5m)); // 8.5% NY tax
            }

            [Test]
            public void ProcessCart_EmptyCart_ThrowsException()
            {
                Assert.Throws<ArgumentException>(() =>
                    processor.ProcessCart(new List<CartItem>(), "CA"));
            }
        }

        #endregion

        #region Factory Methods Tests

        [TestFixture]
        public class FactoryMethodsTests
        {
            [Test]
            public void GetAllProductTypes_Returns5Types()
            {
                var types = ProductFactoryClass.GetAllProductTypes();
                Assert.That(types.Length, Is.EqualTo(5));
            }

            [Test]
            public void GetProductTypeDescription_ReturnsDescription()
            {
                var description = ProductFactoryClass.GetProductTypeDescription(ProductType.Digital);
                Assert.That(description, Does.Contain("Digital"));
            }

            [Test]
            public void RequiresSpecialHandling_Service_ReturnsTrue()
            {
                var processor = new CartProcessor();
                Assert.That(processor.RequiresSpecialHandling("service"), Is.True);
            }

            [Test]
            public void RequiresSpecialHandling_Digital_ReturnsFalse()
            {
                var processor = new CartProcessor();
                Assert.That(processor.RequiresSpecialHandling("digital"), Is.False);
            }
        }

        #endregion

        #region Edge Cases Tests

        [TestFixture]
        public class EdgeCasesTests
        {
            [Test]
            public void PhysicalProduct_ZeroWeight_DefaultsTo1()
            {
                var product = new PhysicalProduct();
                var shipping = product.CalculateShippingCost();
                Assert.That(shipping, Is.GreaterThan(0m));
            }

            [Test]
            public void Product_DefaultTax_Applied()
            {
                var product = new PhysicalProduct { Price = 100m };
                var tax = product.CalculateTax("Unknown");
                Assert.That(tax, Is.EqualTo(7m)); // 7% default
            }

            [Test]
            public void CartSummary_ToString_FormatsCorrectly()
            {
                var summary = new CartSummary
                {
                    Subtotal = 100m,
                    Shipping = 10m,
                    Tax = 7.25m,
                    Total = 117.25m
                };

                var str = summary.ToString();
                Assert.That(str, Contains.Substring("100.00"));
                Assert.That(str, Contains.Substring("117.25"));
            }
        }

        #region Null Object Pattern Tests

        [TestFixture]
        public class NullObjectPatternTests
        {
            [Test]
            public void CreateByType_NullProductType_ReturnsNullProduct()
            {
                var props = new Dictionary<string, object>();
                var product = ProductFactoryClass.CreateByType((string)null, props);
                Assert.That(product, Is.InstanceOf<NullProduct>());
            }

            [Test]
            public void CreateByType_EmptyProductType_ReturnsNullProduct()
            {
                var props = new Dictionary<string, object>();
                var product = ProductFactoryClass.CreateByType("", props);
                Assert.That(product, Is.InstanceOf<NullProduct>());
            }

            [Test]
            public void CreateByType_InvalidProductType_ReturnsNullProduct()
            {
                var props = new Dictionary<string, object>();
                var product = ProductFactoryClass.CreateByType("unknown", props);
                Assert.That(product, Is.InstanceOf<NullProduct>());
            }

            [Test]
            public void CreateByType_NullProperties_SafelyHandles()
            {
                var product = ProductFactoryClass.CreateByType("physical", null);
                Assert.That(product, Is.Not.Null);
            }

            [Test]
            public void NullProduct_SKU_ReturnsSafeDefault()
            {
                var product = new NullProduct();
                Assert.That(product.SKU, Is.EqualTo("NULL-PRODUCT"));
            }

            [Test]
            public void NullProduct_Shipping_ReturnsFree()
            {
                var product = new NullProduct();
                Assert.That(product.CalculateShippingCost(), Is.EqualTo(0m));
            }

            [Test]
            public void NullProduct_Tax_ReturnsZero()
            {
                var product = new NullProduct();
                Assert.That(product.CalculateTax("CA"), Is.EqualTo(0m));
            }

            [Test]
            public void NullProduct_InStock_ReturnsFalse()
            {
                var product = new NullProduct();
                Assert.That(product.IsInStock(), Is.False);
            }

            [Test]
            public void NullProduct_Fulfillment_IsNoFulfillment()
            {
                var product = new NullProduct();
                Assert.That(product.GetFulfillmentMethod(), Does.Contain("No"));
            }

            [Test]
            public void NullProduct_Description_IsDescriptive()
            {
                var product = new NullProduct();
                var description = product.GetProductTypeDescription();
                Assert.That(description, Does.Contain("Null"));
            }
        }

        #endregion
