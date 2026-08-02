<div align="center">

# 🎯 Design Patterns

### *Master the Art of Software Architecture*

[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)
[![GitHub stars](https://img.shields.io/github/stars/devmohamedsakr-prog/Design-Patterns?style=flat-square)](https://github.com/devmohamedsakr-prog/Design-Patterns)
[![GitHub forks](https://img.shields.io/github/forks/devmohamedsakr-prog/Design-Patterns?style=flat-square)](https://github.com/devmohamedsakr-prog/Design-Patterns/fork)

A **comprehensive, well-organized repository** of software design patterns with clean implementation examples and detailed documentation.

[Explore Patterns](#-creational-patterns) • [Contributing](#-contributing) • [License](#-license)

</div>

---

## 📚 Table of Contents

- [About](#about)
- [Pattern Categories](#-pattern-categories)
- [Quick Start](#-quick-start)
- [Folder Structure](#-folder-structure)
- [Pattern Overview](#-pattern-overview)
- [Contributing](#-contributing)
- [Resources](#-resources)
- [License](#-license)

---

## About

This repository serves as a **learning resource and reference guide** for all 23 Gang of Four (GoF) design patterns. Each pattern is organized by category with clear explanations and practical implementations.

> **Design Patterns** are reusable solutions to common problems in software design. They represent best practices and can speed up development by providing proven, tested development paradigms.

---

## 🎨 Pattern Categories

<table align="center">
<tr>
<td align="center" width="33%">

### 🔧 Creational

**Object Creation**

Patterns that provide mechanisms for object creation while hiding the creation logic.

</td>
<td align="center" width="33%">

### 🏗️ Structural

**Object Composition**

Patterns that deal with object composition and relationships between entities.

</td>
<td align="center" width="33%">

### 📡 Behavioral

**Object Interaction**

Patterns that focus on communication between objects and responsibility distribution.

</td>
</tr>
</table>

---

## 🔧 Creational Patterns
> *Deal with object creation mechanisms*

| Pattern | Purpose |
|---------|---------|
| 🎲 **Singleton** | Ensures a class has only one instance with global access point |
| 🏭 **Factory** | Creates objects without specifying exact classes |
| 🏢 **AbstractFactory** | Creates families of related or dependent objects |
| 🔨 **Builder** | Constructs complex objects step by step |
| 👥 **Prototype** | Creates new objects by cloning existing ones |

**Use Cases:** Database connections, configuration managers, UI components, complex objects

---

## 🏗️ Structural Patterns
> *Deal with object composition and relationships*

| Pattern | Purpose |
|---------|---------|
| 🔌 **Adapter** | Makes incompatible interfaces work together |
| 🌉 **Bridge** | Decouples abstraction from its implementation |
| 🌳 **Composite** | Treats individual objects and compositions uniformly |
| 🎨 **Decorator** | Adds new behaviors to objects dynamically |
| 🎭 **Facade** | Provides simplified interface to complex subsystems |
| 💫 **Flyweight** | Shares common data to reduce memory usage |
| 🛡️ **Proxy** | Provides placeholder for another object |

**Use Cases:** UI layers, system integrations, permission systems, caching

---

## 📡 Behavioral Patterns
> *Deal with object collaboration and communication*

| Pattern | Purpose |
|---------|---------|
| ⛓️ **ChainOfResponsibility** | Passes requests along a chain of handlers |
| 📋 **Command** | Encapsulates requests as objects |
| 🔄 **Iterator** | Accesses collection elements sequentially |
| 🤝 **Mediator** | Reduces coupling between communicating objects |
| 📸 **Memento** | Captures and externalizes object state |
| 👁️ **Observer** | Notifies multiple objects about state changes |
| 🔀 **State** | Changes behavior based on internal state |
| ⚡ **Strategy** | Defines interchangeable algorithms |
| 📖 **TemplateMethod** | Defines algorithm skeleton in base class |
| 🚶 **Visitor** | Performs operations on object structures |
| 🗣️ **Interpreter** | Defines language grammar and interpretation |

**Use Cases:** Event systems, workflows, data processing, game logic

---

## 🚀 Quick Start

### 1️⃣ Clone the Repository
```bash
git clone https://github.com/devmohamedsakr-prog/Design-Patterns.git
cd Design-Patterns
```

### 2️⃣ Explore Patterns
Navigate to any pattern folder to find implementations and examples:
```
Design-Patterns/
├── Creational/
├── Structural/
└── Behavioral/
```

### 3️⃣ Study & Learn
- Read pattern descriptions
- Review implementation examples
- Understand use cases and benefits
- Apply patterns to your projects

---

## 📁 Folder Structure

```
Design-Patterns/
│
├── Creational/
│   ├── Singleton/
│   ├── Factory/
│   ├── AbstractFactory/
│   ├── Builder/
│   └── Prototype/
│
├── Structural/
│   ├── Adapter/
│   ├── Bridge/
│   ├── Composite/
│   ├── Decorator/
│   ├── Facade/
│   ├── Flyweight/
│   └── Proxy/
│
└── Behavioral/
    ├── ChainOfResponsibility/
    ├── Command/
    ├── Iterator/
    ├── Mediator/
    ├── Memento/
    ├── Observer/
    ├── State/
    ├── Strategy/
    ├── TemplateMethod/
    ├── Visitor/
    └── Interpreter/
```

---

## 📊 Pattern Overview

### Total Patterns: **23** Gang of Four Patterns

| Category | Count | Patterns |
|----------|-------|----------|
| 🔧 Creational | 5 | Singleton, Factory, AbstractFactory, Builder, Prototype |
| 🏗️ Structural | 7 | Adapter, Bridge, Composite, Decorator, Facade, Flyweight, Proxy |
| 📡 Behavioral | 11 | ChainOfResponsibility, Command, Iterator, Mediator, Memento, Observer, State, Strategy, TemplateMethod, Visitor, Interpreter |

---

## 💡 When to Use Each Pattern

### � Creational Patterns
- Need to create objects in a flexible way
- Want to hide creation complexity
- Need to ensure single instances
- Working with object families

### 🏗️ Structural Patterns
- Need to compose objects into larger structures
- Want to add new functionality dynamically
- Dealing with incompatible interfaces
- Reducing memory usage

### 📡 Behavioral Patterns
- Need to define interactions between objects
- Want to decouple command senders from receivers
- Implementing event systems
- Defining algorithms that vary

---

## 🤝 Contributing

We welcome contributions! Whether it's new implementations, improvements, or documentation:

### How to Contribute

1. **Fork** the repository
2. **Create** a feature branch (`git checkout -b feature/pattern-implementation`)
3. **Add** your implementation to the appropriate pattern folder
4. **Commit** with clear messages
5. **Push** and create a **Pull Request**

### Contribution Guidelines

✅ **Do's:**
- Add clear, commented code examples
- Follow language conventions
- Include usage examples
- Update documentation as needed
- Test your code

❌ **Don'ts:**
- Don't submit untested code
- Don't ignore existing code style
- Don't add non-pattern related content

See [CONTRIBUTING.md](CONTRIBUTING.md) for detailed guidelines.

---

## � Resources

### Learn More
- [Refactoring Guru - Design Patterns](https://refactoring.guru/design-patterns)
- [Gang of Four Design Patterns Book](https://en.wikipedia.org/wiki/Design_Patterns)
- [Martin Fowler's Patterns](https://martinfowler.com/articles/designElements.html)

### Related Topics
- **SOLID Principles** - Fundamental design principles
- **Architecture Patterns** - Large-scale design patterns
- **Microservices Patterns** - Cloud-native patterns

---

## 📄 License

This project is licensed under the **MIT License** - see the [LICENSE](LICENSE) file for details.

```
MIT License - Feel free to use, modify, and distribute
```

---

## 👨‍💻 Author

**devmohamedsakr-prog**

- 🔗 GitHub: [@devmohamedsakr-prog](https://github.com/devmohamedsakr-prog)
- 📧 Questions? Open an issue!

---

<div align="center">

### ⭐ If you find this helpful, please star the repository!

Made with ❤️ for developers who love clean code and smart design.

**Happy Learning! 🚀**

</div>
