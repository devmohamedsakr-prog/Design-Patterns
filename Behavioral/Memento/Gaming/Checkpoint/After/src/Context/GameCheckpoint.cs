using System;
using System.Collections.Generic;

namespace GameCheckpoint.After.Context
{
    /// <summary>
    /// GameState: Character/Game memento - captures game progress
    /// </summary>
    public class GameStateMemento
    {
        public string CharacterName { get; set; } = "";
        public int Level { get; set; }
        public int Experience { get; set; }
        public int Health { get; set; }
        public int MaxHealth { get; set; }
        public int Mana { get; set; }
        public int Inventory { get; set; }
        public string CurrentLocation { get; set; } = "";
        public DateTime SaveTime { get; set; }
        public string CheckpointName { get; set; } = "";

        public GameStateMemento(string characterName, int level, int experience, int health, int maxHealth, 
            int mana, int inventory, string location, string checkpointName)
        {
            CharacterName = characterName;
            Level = level;
            Experience = experience;
            Health = health;
            MaxHealth = maxHealth;
            Mana = mana;
            Inventory = inventory;
            CurrentLocation = location;
            CheckpointName = checkpointName;
            SaveTime = DateTime.Now;
        }

        public override string ToString() => $"{CheckpointName} - Lvl {Level} {CharacterName} @ {CurrentLocation} ({SaveTime:HH:mm:ss})";
    }

    /// <summary>
    /// Character: Originator - manages character state
    /// </summary>
    public class Character
    {
        public string Name { get; set; } = "";
        public int Level { get; set; } = 1;
        public int Experience { get; set; } = 0;
        public int Health { get; set; } = 100;
        public int MaxHealth { get; set; } = 100;
        public int Mana { get; set; } = 50;
        public int InventorySlots { get; set; } = 20;
        public string CurrentLocation { get; set; } = "Village";

        public Character(string name)
        {
            Name = name;
        }

        public void TakeDamage(int damage)
        {
            Health = Math.Max(0, Health - damage);
            Console.WriteLine($"  💢 {Name} takes {damage} damage. HP: {Health}/{MaxHealth}");
        }

        public void Heal(int amount)
        {
            Health = Math.Min(MaxHealth, Health + amount);
            Console.WriteLine($"  💚 {Name} heals {amount}. HP: {Health}/{MaxHealth}");
        }

        public void GainExperience(int exp)
        {
            Experience += exp;
            if (Experience >= Level * 100)
            {
                LevelUp();
            }
            Console.WriteLine($"  ⭐ {Name} gains {exp} XP (Total: {Experience})");
        }

        public void LevelUp()
        {
            Level++;
            MaxHealth += 20;
            Health = MaxHealth;
            Mana += 10;
            Console.WriteLine($"  🎉 {Name} levels up to {Level}! MaxHP: {MaxHealth}");
        }

        public void MoveTo(string location)
        {
            CurrentLocation = location;
            Console.WriteLine($"  🗺️ {Name} moves to {location}");
        }

        public GameStateMemento SaveCheckpoint(string checkpointName)
        {
            var memento = new GameStateMemento(Name, Level, Experience, Health, MaxHealth, 
                Mana, InventorySlots, CurrentLocation, checkpointName);
            Console.WriteLine($"💾 Checkpoint saved: {memento}");
            return memento;
        }

        public void LoadCheckpoint(GameStateMemento memento)
        {
            Name = memento.CharacterName;
            Level = memento.Level;
            Experience = memento.Experience;
            Health = memento.Health;
            MaxHealth = memento.MaxHealth;
            Mana = memento.Mana;
            InventorySlots = memento.Inventory;
            CurrentLocation = memento.CurrentLocation;
            Console.WriteLine($"↶ Checkpoint loaded: {memento}");
        }

        public override string ToString() => $"{Name} (Lvl {Level}, HP: {Health}/{MaxHealth}) @ {CurrentLocation}";
    }

    /// <summary>
    /// GameCheckpointManager: Caretaker - manages game save files
    /// </summary>
    public class GameCheckpointManager
    {
        private Dictionary<string, GameStateMemento> _checkpoints = new();
        private List<GameStateMemento> _saveHistory = new();

        public void SaveCheckpoint(Character character, string checkpointName)
        {
            var memento = character.SaveCheckpoint(checkpointName);
            _checkpoints[checkpointName] = memento;
            _saveHistory.Add(memento);
        }

        public void LoadCheckpoint(Character character, string checkpointName)
        {
            if (_checkpoints.TryGetValue(checkpointName, out var memento))
            {
                character.LoadCheckpoint(memento);
            }
            else
            {
                Console.WriteLine($"✗ Checkpoint '{checkpointName}' not found");
            }
        }

        public List<string> GetAvailableCheckpoints() => new(_checkpoints.Keys);

        public int GetCheckpointCount() => _checkpoints.Count;

        public GameStateMemento? GetCheckpoint(string name) => 
            _checkpoints.TryGetValue(name, out var m) ? m : null;

        public void DeleteCheckpoint(string checkpointName)
        {
            if (_checkpoints.Remove(checkpointName))
            {
                Console.WriteLine($"🗑️ Checkpoint '{checkpointName}' deleted");
            }
        }

        public List<GameStateMemento> GetSaveHistory() => new(_saveHistory);
    }
}
