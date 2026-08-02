<div align="center">

<br>

# 🎯 Design Patterns

### *Master the Art of Software Architecture*

<br>

[![License: MIT](https://img.shields.io/badge/License-MIT-yellow?style=for-the-badge&logo=open-source-initiative&logoColor=white)](https://opensource.org/licenses/MIT)
[![Stars](https://img.shields.io/github/stars/devmohamedsakr-prog/Design-Patterns?style=for-the-badge&logo=github&logoColor=white&color=gold)](https://github.com/devmohamedsakr-prog/Design-Patterns)
[![Forks](https://img.shields.io/github/forks/devmohamedsakr-prog/Design-Patterns?style=for-the-badge&logo=github&logoColor=white&color=blue)](https://github.com/devmohamedsakr-prog/Design-Patterns/fork)
[![Patterns](https://img.shields.io/badge/GoF%20Patterns-23-7B2FBE?style=for-the-badge&logo=dotnet&logoColor=white)](https://github.com/devmohamedsakr-prog/Design-Patterns)

<br>

A **comprehensive, well-organized repository** of software design patterns  
with clean implementation examples, before/after comparisons, and detailed documentation.

<br>

[🔧 Creational](#-creational-patterns) &nbsp;·&nbsp;
[🏗️ Structural](#️-structural-patterns) &nbsp;·&nbsp;
[📡 Behavioral](#-behavioral-patterns) &nbsp;·&nbsp;
[🚀 Quick Start](#-quick-start) &nbsp;·&nbsp;
[🤝 Contributing](#-contributing)

<br>

</div>

---

## 📖 About

> **Design Patterns** are reusable solutions to commonly occurring problems in software design. They represent battle-tested development paradigms that speed up development and improve code quality.

This repository is a **structured learning resource and reference guide** covering all **23 Gang of Four (GoF) design patterns**, organized into three categories — each with clear explanations, real-world use cases, and practical C# implementations.

---

## 🗺️ Pattern Categories at a Glance

| Category | Focus | Patterns |
|:---:|---|:---:|
| 🔧 **Creational** | *How objects are created* | 5 |
| 🏗️ **Structural** | *How objects are composed* | 7 |
| 📡 **Behavioral** | *How objects communicate* | 11 |
| | **Total GoF Patterns** | **23** |

---

## 🔧 Creational Patterns

> *Control the creation process — decouple the system from how its objects are created, composed, and represented.*

| Pattern | Intent | Common Use Cases |
|---|---|---|
| 🎲 **Singleton** | One instance, global access point | DB connections, config managers |
| 🏭 **Factory Method** | Delegate creation to subclasses | UI components, plugin loaders |
| 🏢 **Abstract Factory** | Create families of related objects | Cross-platform UI kits |
| 🔨 **Builder** | Construct complex objects step-by-step | Query builders, document generators |
| 👥 **Prototype** | Clone existing objects | Game entities, undo snapshots |

**✅ Reach for Creational patterns when you need to:**
- Hide the complexity of object creation
- Ensure only one instance of a class exists
- Create families of related objects together
- Build objects with many optional parameters

---

## 🏗️ Structural Patterns

> *Simplify structure by identifying simple ways to realize relationships between entities.*

| Pattern | Intent | Common Use Cases |
|---|---|---|
| 🔌 **Adapter** | Make incompatible interfaces work together | Third-party library wrappers |
| 🌉 **Bridge** | Decouple abstraction from implementation | Cross-platform rendering |
| 🌳 **Composite** | Treat single objects and groups uniformly | File systems, UI trees |
| 🎨 **Decorator** | Add behavior to objects dynamically | Logging, caching, auth layers |
| 🎭 **Facade** | Simplified interface to complex subsystems | API clients, SDK wrappers |
| 💫 **Flyweight** | Share common state to reduce memory | Game tiles, text rendering |
| 🛡️ **Proxy** | Controlled access to another object | Lazy loading, access control |

**✅ Reach for Structural patterns when you need to:**
- Compose objects into larger, flexible structures
- Wrap incompatible interfaces without changing them
- Add new responsibilities dynamically at runtime
- Minimize memory footprint through shared state

---

## 📡 Behavioral Patterns

> *Define how objects interact and distribute responsibility — making systems more flexible and maintainable.*

| Pattern | Intent | Common Use Cases |
|---|---|---|
| ⛓️ **Chain of Responsibility** | Pass requests along a handler chain | Middleware, validation pipelines |
| 📋 **Command** | Encapsulate requests as objects | Undo/redo, task queues |
| 🔄 **Iterator** | Sequential access to collection elements | Custom collections, data streams |
| 🤝 **Mediator** | Centralize complex communications | Chat systems, air traffic control |
| 📸 **Memento** | Capture and restore object state | Undo history, snapshots |
| 👁️ **Observer** | Notify subscribers of state changes | Event systems, reactive UIs |
| 🔀 **State** | Change behavior based on internal state | Order workflows, game states |
| ⚡ **Strategy** | Define a family of interchangeable algorithms | Sorting, payment methods |
| 📖 **Template Method** | Define algorithm skeleton in base class | Data parsers, report generators |
| 🚶 **Visitor** | Perform operations on object structures | Compilers, document export |
| 🗣️ **Interpreter** | Interpret sentences in a language | DSLs, expression evaluators |

**✅ Reach for Behavioral patterns when you need to:**
- Decouple senders from receivers of requests
- Define algorithms that can vary independently
- Allow objects to notify others without tight coupling
- Implement complex conditional logic cleanly

---

## 🚀 Quick Start

### Step 1 — Clone the Repository

```bash
git clone https://github.com/devmohamedsakr-prog/Design-Patterns.git
cd Design-Patterns
```

### Step 2 — Navigate to a Pattern

```bash
# Example: explore the Singleton implementation
cd Creational/Singleton/CurrencyConverter/After
```

### Step 3 — Build & Run

```bash
dotnet restore     # Restore NuGet packages
dotnet build       # Build the project
dotnet test        # Run the test suite
dotnet run         # Run the application
```

### Step 4 — Study the Structure

Each pattern folder follows a consistent layout:

```
PatternName/
├── Before/          ← The problem (without the pattern)
│   ├── app.cs
│   └── README.md
│
└── After/           ← The solution (with the pattern applied)
    ├── src/         ← Implementation files
    ├── Tests/       ← Unit & integration tests
    └── docs/        ← Pattern documentation
```

---

## 📁 Full Repository Structure

```
Design-Patterns/
│
├── 📂 Creational/
│   ├── Singleton/
│   ├── Factory/
│   ├── AbstractFactory/
│   ├── Builder/
│   └── Prototype/
│
├── 📂 Structural/
│   ├── Adapter/
│   ├── Bridge/
│   ├── Composite/
│   ├── Decorator/
│   ├── Facade/
│   ├── Flyweight/
│   └── Proxy/
│
├── 📂 Behavioral/
│   ├── ChainOfResponsibility/
│   ├── Command/
│   ├── Iterator/
│   ├── Mediator/
│   ├── Memento/
│   ├── Observer/
│   ├── State/
│   ├── Strategy/
│   ├── TemplateMethod/
│   ├── Visitor/
│   └── Interpreter/
│
├── 📄 README.md
└── 📄 RELEASE_NOTES.md
```

---

## 💡 Choosing the Right Pattern

Not sure which pattern to use? Use these decision points:

### 🔧 Use a Creational Pattern if…
- You need to control **how** objects are instantiated
- Object creation logic is becoming too complex or scattered
- You want to enforce a **single instance** across the system
- You're building **families of related objects**

### 🏗️ Use a Structural Pattern if…
- You need to **combine objects** into larger, flexible structures
- You're working with **legacy or third-party interfaces** you can't change
- You want to **extend behavior** without subclassing
- You need to **reduce memory** by sharing common state

### 📡 Use a Behavioral Pattern if…
- You need to **decouple** who sends a request from who handles it
- You want behavior to **change at runtime** based on state
- You're building an **event-driven** or **reactive** system
- You need to make algorithms **interchangeable**

---

## 🤝 Contributing

Contributions are welcome — new implementations, improvements, and documentation all count.

### How to Contribute

```bash
# 1. Fork the repo and clone your fork
git clone https://github.com/YOUR-USERNAME/Design-Patterns.git

# 2. Create a feature branch
git checkout -b feature/add-observer-pattern

# 3. Make your changes, then commit
git commit -m "feat: add Observer pattern with event system example"

# 4. Push and open a Pull Request
git push origin feature/add-observer-pattern
```

### Guidelines

**Do ✅**
- Write clear, commented code with real-world context
- Include a `Before/` and `After/` for each pattern
- Add unit tests where applicable
- Update or create documentation in `docs/`

**Don't ❌**
- Submit untested or uncommented code
- Add content unrelated to design patterns
- Break the existing folder structure

---

## 📚 Resources

### Essential Reading
- 📘 [Refactoring Guru — Design Patterns](https://refactoring.guru/design-patterns) — Visual, easy-to-follow explanations
- 📗 [Gang of Four Book](https://en.wikipedia.org/wiki/Design_Patterns) — The original reference (Gamma et al.)
- 📙 [Martin Fowler's Patterns](https://martinfowler.com/articles/designElements.html) — Enterprise-level pattern thinking

### Related Concepts
- **SOLID Principles** — The foundation every pattern builds on
- **Clean Architecture** — How patterns fit into larger system design
- **Microservices Patterns** — Cloud-native extensions of GoF thinking

---

## 📄 License

This project is licensed under the **MIT License** — see [LICENSE](LICENSE) for details.  
Free to use, modify, and distribute with attribution.

---

## 👨‍💻 Author

**Mohamed Sakr** — [@devmohamedsakr-prog](https://github.com/devmohamedsakr-prog)

- 🐛 Found a bug? [Open an issue](https://github.com/devmohamedsakr-prog/Design-Patterns/issues)
- 💬 Have a question? [Start a discussion](https://github.com/devmohamedsakr-prog/Design-Patterns/discussions)

---

<div align="center">

⭐ **If this repository helped you, please give it a star — it helps others find it!**

<br>

Made with ❤️ for developers who love clean code and thoughtful design.

**Happy Learning! 🚀**

</div>
