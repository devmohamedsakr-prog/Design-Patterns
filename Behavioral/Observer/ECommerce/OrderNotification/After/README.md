# Observer Pattern: After (Solution)

## Overview
Observer pattern decouples order events from notification handlers. When order status changes, all subscribed observers are automatically notified.

## Solution Structure

```
After (Clean Design):
├── Models/
│   ├── Order.cs (subject)
│   ├── OrderStatus.cs (enum)
│   └── OrderEvent.cs (event data)
├── Subjects/
│   └── OrderSubject.cs (observable)
├── Observers/
│   ├── IOrderObserver.cs (interface)
│   ├── EmailObserver.cs
│   ├── SMSObserver.cs
│   ├── PushObserver.cs
│   ├── InventoryObserver.cs
│   └── AnalyticsObserver.cs
└── Tests/
    └── OrderNotificationTests.cs (25+ tests)
```

## Key Design Principles

### Single Responsibility Principle (SRP)

| Class | Responsibility |
|-------|----------------|
| `Order` | Store order data |
| `OrderSubject` | Manage observers and notify |
| `EmailObserver` | Send emails only |
| `SMSObserver` | Send SMS only |
| `PushObserver` | Send push notifications only |
| `InventoryObserver` | Update inventory only |
| `AnalyticsObserver` | Track analytics only |

Each observer has exactly one reason to change.

## Implementation Details

### Subject Class
```csharp
public class OrderSubject
{
    private List<IOrderObserver> _observers;
    
    public void Attach(IOrderObserver observer) { }
    public void Detach(IOrderObserver observer) { }
    private void NotifyObservers(string message) { }
    
    public void ProcessOrder() { }
    public void ShipOrder() { }
    public void DeliverOrder() { }
    public void CancelOrder() { }
}
```

### Observer Interface
```csharp
public interface IOrderObserver
{
    void Update(OrderEvent orderEvent);
}
```

### Concrete Observer
```csharp
public class EmailObserver : IOrderObserver
{
    public void Update(OrderEvent orderEvent)
    {
        // Send email
    }
}
```

## Usage Examples

### Subscribe Observers
```csharp
var order = new Order("ORD001", "Alice", "alice@example.com", "+1234567890", 150);
var orderSubject = new OrderSubject(order);

orderSubject.Attach(new EmailObserver());
orderSubject.Attach(new SMSObserver());
orderSubject.Attach(new InventoryObserver());
```

### Trigger Events
```csharp
orderSubject.ProcessOrder();    // All observers notified
orderSubject.ShipOrder();       // All observers notified
orderSubject.DeliverOrder();    // All observers notified
```

### Unsubscribe
```csharp
var emailObserver = new EmailObserver();
orderSubject.Attach(emailObserver);
orderSubject.Detach(emailObserver);  // No longer receives notifications
```

## Benefits

✓ **Loose Coupling** - Order doesn't know about observers
✓ **Dynamic Subscriptions** - Add/remove observers at runtime
✓ **Multiple Observers** - Many observers per event
✓ **Single Responsibility** - Each observer handles one concern
✓ **Easy Extension** - Add new observers without changes
✓ **Fault Isolation** - Observer failure doesn't affect order
✓ **Event Propagation** - All observers notified automatically

## Real-World Applications

### E-Commerce
- Order placed → Email + SMS + Inventory + Analytics
- Order shipped → Email + Push + Inventory
- Order delivered → Email + Analytics
- Order cancelled → Email + SMS + Inventory

### Multi-Channel Notifications
- Same event → Multiple notification channels
- Customer preferences → Subscribe/unsubscribe dynamically
- Third-party integrations → Add new observers easily

### System Integration
- Order events → Inventory, Payment, Analytics, Fulfillment
- Each system independent
- No cascade failures

---

**Pattern:** Observer  
**Domain:** eCommerce  
**Use Case:** Order notification system  
**SRP Compliance:** ✓ (5 focused observers)  
**Tests:** 25+  
**Key Benefit:** Loose coupling with automatic event propagation
