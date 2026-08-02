using System;
using System.Collections.Generic;
using System.Linq;

namespace Composite.Menu.Navigation.Component
{
    /// <summary>
    /// Component interface: Menu items and submenus.
    /// Demonstrates: Composite pattern for treating menu item same as submenu hierarchy.
    /// </summary>
    public abstract class MenuItem
    {
        public string Id { get; set; }
        public string Label { get; set; }
        public int Priority { get; set; }

        protected MenuItem(string id, string label)
        {
            Id = id;
            Label = label;
            Priority = 100;
        }

        public abstract void Display(int level = 0);
        public abstract MenuItem Find(string id);
        public abstract int GetItemCount();
        public abstract void Enable();
        public abstract void Disable();
        public abstract List<MenuItem> GetAllItems();
    }

    /// <summary>
    /// Leaf: Simple menu item that executes action.
    /// </summary>
    public class ActionMenuItem : MenuItem
    {
        public string Action { get; set; }
        public bool IsEnabled { get; set; }
        public string KeyboardShortcut { get; set; }

        public ActionMenuItem(string id, string label, string action) : base(id, label)
        {
            Action = action;
            IsEnabled = true;
        }

        public override void Display(int level = 0)
        {
            Console.WriteLine($"{new string(' ', level * 2)}> {Label} ({KeyboardShortcut ?? "N/A"})");
        }

        public override MenuItem Find(string id)
        {
            return Id == id ? this : null;
        }

        public override int GetItemCount() => 1;

        public override void Enable() => IsEnabled = true;

        public override void Disable() => IsEnabled = false;

        public override List<MenuItem> GetAllItems() => new List<MenuItem> { this };

        public void Execute()
        {
            if (IsEnabled)
                Console.WriteLine($"Executing: {Action}");
        }

        public override string ToString() => $"MenuItem({Label}, {Action})";
    }

    /// <summary>
    /// Composite: Submenu containing other menu items.
    /// </summary>
    public class SubMenu : MenuItem
    {
        private readonly List<MenuItem> _items = new List<MenuItem>();
        public bool IsExpanded { get; set; }
        public bool IsEnabled { get; set; }

        public SubMenu(string id, string label) : base(id, label)
        {
            IsExpanded = false;
            IsEnabled = true;
        }

        public void Add(MenuItem item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));
            _items.Add(item);
            _items.Sort((a, b) => b.Priority.CompareTo(a.Priority));
        }

        public void Remove(MenuItem item)
        {
            _items.Remove(item);
        }

        public IReadOnlyList<MenuItem> GetItems() => _items.AsReadOnly();

        public override void Display(int level = 0)
        {
            Console.WriteLine($"{new string(' ', level * 2)}+ {Label} ({_items.Count} items)");
            if (IsExpanded)
            {
                foreach (var item in _items)
                {
                    item.Display(level + 1);
                }
            }
        }

        public override MenuItem Find(string id)
        {
            if (Id == id)
                return this;

            foreach (var item in _items)
            {
                var found = item.Find(id);
                if (found != null)
                    return found;
            }

            return null;
        }

        public override int GetItemCount()
        {
            int count = 1;
            foreach (var item in _items)
            {
                count += item.GetItemCount();
            }
            return count;
        }

        public override void Enable()
        {
            IsEnabled = true;
            foreach (var item in _items)
            {
                item.Enable();
            }
        }

        public override void Disable()
        {
            IsEnabled = false;
            foreach (var item in _items)
            {
                item.Disable();
            }
        }

        public override List<MenuItem> GetAllItems()
        {
            var items = new List<MenuItem> { this };
            foreach (var item in _items)
            {
                items.AddRange(item.GetAllItems());
            }
            return items;
        }

        public override string ToString() => $"SubMenu({Label}, {_items.Count} items)";
    }

    /// <summary>
    /// Composite: Menu bar with menus.
    /// </summary>
    public class MenuBar : MenuItem
    {
        private readonly List<MenuItem> _menus = new List<MenuItem>();

        public MenuBar() : base("menubar", "MenuBar")
        {
        }

        public void AddMenu(MenuItem menu)
        {
            if (menu == null)
                throw new ArgumentNullException(nameof(menu));
            _menus.Add(menu);
        }

        public override void Display(int level = 0)
        {
            Console.WriteLine("=== Menu Bar ===");
            foreach (var menu in _menus)
            {
                menu.Display(level);
            }
            Console.WriteLine("===============");
        }

        public override MenuItem Find(string id)
        {
            if (Id == id)
                return this;

            foreach (var menu in _menus)
            {
                var found = menu.Find(id);
                if (found != null)
                    return found;
            }

            return null;
        }

        public override int GetItemCount()
        {
            return _menus.Sum(m => m.GetItemCount());
        }

        public override void Enable()
        {
            foreach (var menu in _menus)
            {
                menu.Enable();
            }
        }

        public override void Disable()
        {
            foreach (var menu in _menus)
            {
                menu.Disable();
            }
        }

        public override List<MenuItem> GetAllItems()
        {
            var items = new List<MenuItem>();
            foreach (var menu in _menus)
            {
                items.AddRange(menu.GetAllItems());
            }
            return items;
        }

        public override string ToString() => $"MenuBar({_menus.Count} menus)";
    }

    /// <summary>
    /// Leaf: Separator in menu.
    /// </summary>
    public class MenuSeparator : MenuItem
    {
        public MenuSeparator() : base("separator", "---")
        {
        }

        public override void Display(int level = 0)
        {
            Console.WriteLine($"{new string(' ', level * 2)}─────────");
        }

        public override MenuItem Find(string id) => null;

        public override int GetItemCount() => 0;

        public override void Enable() { }

        public override void Disable() { }

        public override List<MenuItem> GetAllItems() => new List<MenuItem>();

        public override string ToString() => "Separator";
    }
}
