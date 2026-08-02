# 💱 Currency Converter - Singleton Pattern Example

## Overview
This example demonstrates the **Singleton pattern** using a real-world currency converter scenario. Compare the **Before** (without pattern) and **After** (with pattern) implementations to understand the benefits.

## 📂 Structure
```
CurrencyConverter/
├── Before/          # Without Singleton Pattern
│   ├── README.md
│   └── app.cs
├── After/           # With Singleton Pattern (SRP Applied)
│   ├── README.md
│   └── app.cs
└── README.md        # This file
```

## 🎯 What is Singleton Pattern?

The **Singleton pattern** is a creational design pattern that ensures a class has only one instance and provides a global point of access to it.

### Key Characteristics:
- ✅ Only one instance exists throughout application lifetime
- ✅ Global access point to that single instance
- ✅ Lazy or eager initialization options
- ✅ Thread-safe implementation

## 💡 Currency Converter Use Case

A currency converter system needs to:
1. Load exchange rates from an external source
2. Cache rates in memory to avoid repeated API calls
3. Provide conversion functionality across the application
4. Ensure consistent data across the application

**Perfect for Singleton Pattern!**

## 🔄 Comparison

| Aspect | Before (No Pattern) | After (Singleton + SRP) |
|--------|-------------------|------------------------|
| **Instances** | Multiple instances created | Single shared instance |
| **Memory** | Inefficient - duplicate data | Efficient - one copy |
| **API Calls** | Repeated calls per instance | Single load for all |
| **Consistency** | Data inconsistency possible | Guaranteed consistency |
| **Global Access** | Manual passing required | Built-in access |
| **Testability** | Difficult | Easier with interfaces |

## 📖 How to Use

### Before Implementation:
```
cd Before
# Review the problems with multiple instances
# See how rates get duplicated and out of sync
```

### After Implementation:
```
cd After
# See the clean, efficient Singleton solution
# Notice SRP applied with separate concerns
# Understand why this approach is better
```

## 🎓 Learning Path

1. **Start with Before** - Understand the problems
2. **Read the Before README** - Learn what goes wrong
3. **Review the Before Code** - See the implementation issues
4. **Study the After** - Understand the solution
5. **Read the After README** - Learn the benefits and SRP
6. **Compare Code** - Identify key differences

## ✨ Topics Covered

- ✅ Problem with multiple instances
- ✅ Global state management
- ✅ Thread-safety considerations
- ✅ Lazy initialization
- ✅ Single Responsibility Principle (SRP)
- ✅ Dependency Inversion
- ✅ Testing considerations

## 🚀 Key Takeaways

### When to Use Singleton:
- Database connections
- Configuration managers
- Logger instances
- Cache managers
- Exchange rate providers
- Session managers

### When NOT to Use:
- Business logic objects that need multiple states
- Services that should be stateless
- Objects that need independent instances
- When you need dependency injection

## 📚 Related Resources
- [Singleton Pattern Explained](https://refactoring.guru/design-patterns/singleton)
- [SRP - Single Responsibility Principle](https://en.wikipedia.org/wiki/Single-responsibility_principle)
- [Thread-Safe Singleton](https://www.dotnetperls.com/singleton)

---

**Explore both implementations to master the Singleton pattern!** 🎯
