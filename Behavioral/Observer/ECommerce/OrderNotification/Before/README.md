# Observer Pattern: Before (Anti-pattern)

## The Problem: Tight Coupling to Notifications

Without Observer pattern, order processing becomes tightly coupled to all notification systems.

### Problem Analysis

**Scenario:** eCommerce platform processes orders and needs to:
1. Send customer email
2. Send SMS confirmation
3. Send push notification
4. Update inventory system
5. Track analytics

**Anti-Pattern Solution:** All calls in Order class

```csharp
public void ProcessOrder(Order order)
{
    // Process order...
    
    // Problem: Hard-coded notifications
    emailService.SendConfirmation(order);
    smsService.SendConfirmation(order);
    pushService.SendConfirmation(order);
    inventoryService.Update(order);
    analyticsService.Track(order);
}
```

### Real-World Impact: $2.2M/Year

**Impact Scenario 1: Adding New Notification Type**
- Need to add Slack notification
- Must modify Order class
- Risk of breaking existing notifications
- **Cost:** $300K/year per new channel

**Impact Scenario 2: Notification Service Fails**
- SMS service down → entire order process fails
- No graceful degradation
- Cascade failures
- **Cost:** $500K/year in lost orders

**Impact Scenario 3: Scaling Issues**
- Adding more subscribers slows order processing
- Synchronous calls block order completion
- Customer experience degrades
- **Cost:** $600K/year in lost customers

**Impact Scenario 4: Testing Nightmare**
- Must mock all notification services for order tests
- Hard to test notifications independently
- Hard to test order without side effects
- **Cost:** $800K/year in testing overhead

## Code Example: The Problem

```csharp
public class Order
{
    public string OrderId { get; set; }
    public OrderStatus Status { get; set; }
    public decimal Amount { get; set; }
    public string CustomerEmail { get; set; }
}

public enum OrderStatus
{
    Placed,
    Processing,
    Shipped,
    Delivered
}

// Anti-pattern: Order class handles all notifications
public class OrderProcessor
{
    private EmailService _emailService;
    private SMSService _smsService;
    private PushService _pushService;
    private InventoryService _inventoryService;
    private AnalyticsService _analyticsService;

    public void ProcessOrder(Order order)
    {
        // PROBLEM 1: Tight coupling to all notification services
        // PROBLEM 2: Order class knows about all subscribers
        // PROBLEM 3: Hard to add/remove subscribers
        // PROBLEM 4: Changing notification logic requires modifying order
        // PROBLEM 5: Hard to test without all dependencies

        order.Status = OrderStatus.Processing;

        // Send notifications (tightly coupled)
        _emailService.SendOrderConfirmation(order.CustomerEmail, order);
        _smsService.SendOrderConfirmation(order.CustomerPhone, order);
        _pushService.SendOrderNotification(order.CustomerId, "Order placed");
        _inventoryService.ReserveItems(order);
        _analyticsService.TrackOrderPlaced(order);

        // What if SMS fails? Order is already processed.
        // What if we add a new notification? Must modify this method.
        // What if we want to disable SMS? Must remove the line.
    }

    public void ShipOrder(Order order)
    {
        order.Status = OrderStatus.Shipped;

        // Code duplication: Same notifications again!
        _emailService.SendShippingNotification(order.CustomerEmail, order);
        _smsService.SendShippingNotification(order.CustomerPhone, order);
        _pushService.SendOrderNotification(order.CustomerId, "Order shipped");
        _inventoryService.UpdateInventory(order);
        _analyticsService.TrackOrderShipped(order);
    }

    public void CancelOrder(Order order)
    {
        order.Status = OrderStatus.Cancelled;

        // More duplication and coupling!
        _emailService.SendCancellationNotification(order.CustomerEmail, order);
        _smsService.SendCancellationNotification(order.CustomerPhone, order);
        _pushService.SendOrderNotification(order.CustomerId, "Order cancelled");
        _inventoryService.ReleaseItems(order);
        _analyticsService.TrackOrderCancelled(order);
    }
}
```

### Problems This Creates

1. **Tight Coupling**
   - Order knows about all notification systems
   - Adding notification = modifying order
   - Risk of breaking order logic

2. **Hard to Extend**
   - New notification? Edit Order class
   - New status? Edit all notification calls
   - Cascade changes required

3. **Code Duplication**
   - Same notification calls repeated in ProcessOrder, ShipOrder, CancelOrder
   - Easy to miss updates when notification changes
   - Inconsistent notifications

4. **Testing Issues**
   - Must mock all dependencies to test order
   - Hard to test notifications independently
   - Hard to test order without side effects

5. **Scalability Problems**
   - Each new subscriber slows order processing
   - Synchronous calls = blocking
   - No graceful degradation if service fails

6. **Single Responsibility Violation**
   - Order handles business logic + notifications
   - Too many reasons to change
   - Hard to understand code

---

**Problem Type:** Tight Coupling / Hard-Coded Dependencies  
**Cost Impact:** $2.2M/year in maintenance and failures  
**Solution:** Observer Pattern (see After/)
