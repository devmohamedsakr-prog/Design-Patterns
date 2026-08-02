using System;
using System.Collections.Generic;

namespace Prototype.Game.Entity.Context
{
    /// <summary>
    /// Product: Game entity with deep copy clone for efficient spawning.
    /// Demonstrates: Prototype pattern for game object creation without allocation overhead.
    /// </summary>
    public class GameEntity
    {
        public string Name { get; set; }
        public string Type { get; set; } // Player, Enemy, NPC, Item
        public Vector3 Position { get; set; }
        public Vector3 Velocity { get; set; }
        public Vector3 Scale { get; set; }
        public Transform Transform { get; set; }
        public HealthComponent Health { get; set; }
        public MovementComponent Movement { get; set; }
        public CombatComponent Combat { get; set; }
        public IList<string> Tags { get; set; }
        public bool IsActive { get; set; }

        public GameEntity()
        {
            Tags = new List<string>();
            Position = new Vector3();
            Velocity = new Vector3();
            Scale = new Vector3 { X = 1, Y = 1, Z = 1 };
            Transform = new Transform();
            Health = new HealthComponent();
            Movement = new MovementComponent();
            Combat = new CombatComponent();
            IsActive = true;
        }

        /// <summary>
        /// Deep copy clone of this entity.
        /// </summary>
        public GameEntity Clone()
        {
            var clone = new GameEntity
            {
                Name = this.Name,
                Type = this.Type,
                Position = this.Position?.Clone(),
                Velocity = this.Velocity?.Clone(),
                Scale = this.Scale?.Clone(),
                Transform = this.Transform?.Clone(),
                Health = this.Health?.Clone(),
                Movement = this.Movement?.Clone(),
                Combat = this.Combat?.Clone(),
                IsActive = this.IsActive
            };

            foreach (var tag in this.Tags)
            {
                clone.Tags.Add(tag);
            }

            return clone;
        }

        public override string ToString()
        {
            return $"GameEntity(Name={Name}, Type={Type}, Pos={Position}, HP={Health?.CurrentHealth})";
        }
    }

    /// <summary>
    /// 3D vector representation.
    /// </summary>
    public class Vector3
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }

        public Vector3 Clone() => new Vector3 { X = this.X, Y = this.Y, Z = this.Z };

        public override string ToString() => $"({X:F2}, {Y:F2}, {Z:F2})";
    }

    /// <summary>
    /// Transform component (position, rotation, scale).
    /// </summary>
    public class Transform
    {
        public Vector3 Position { get; set; }
        public Vector3 Rotation { get; set; }
        public Vector3 Scale { get; set; }
        public string ParentName { get; set; }

        public Transform()
        {
            Position = new Vector3();
            Rotation = new Vector3();
            Scale = new Vector3 { X = 1, Y = 1, Z = 1 };
        }

        public Transform Clone()
        {
            return new Transform
            {
                Position = this.Position?.Clone(),
                Rotation = this.Rotation?.Clone(),
                Scale = this.Scale?.Clone(),
                ParentName = this.ParentName
            };
        }

        public override string ToString() =>
            $"Transform(Pos={Position}, Rot={Rotation}, Scale={Scale})";
    }

    /// <summary>
    /// Health component for entities with HP.
    /// </summary>
    public class HealthComponent
    {
        public int MaxHealth { get; set; }
        public int CurrentHealth { get; set; }
        public int Armor { get; set; }
        public bool IsAlive { get; set; }
        public IList<string> StatusEffects { get; set; }

        public HealthComponent()
        {
            StatusEffects = new List<string>();
            IsAlive = true;
        }

        public HealthComponent Clone()
        {
            var clone = new HealthComponent
            {
                MaxHealth = this.MaxHealth,
                CurrentHealth = this.CurrentHealth,
                Armor = this.Armor,
                IsAlive = this.IsAlive
            };

            foreach (var effect in this.StatusEffects)
            {
                clone.StatusEffects.Add(effect);
            }

            return clone;
        }

        public override string ToString() =>
            $"Health(Current={CurrentHealth}/{MaxHealth}, Armor={Armor}, Alive={IsAlive})";
    }

    /// <summary>
    /// Movement component for mobile entities.
    /// </summary>
    public class MovementComponent
    {
        public float Speed { get; set; }
        public float Acceleration { get; set; }
        public float MaxSpeed { get; set; }
        public bool IsMoving { get; set; }
        public string MovementType { get; set; } // Walk, Run, Fly, Swim

        public MovementComponent Clone()
        {
            return new MovementComponent
            {
                Speed = this.Speed,
                Acceleration = this.Acceleration,
                MaxSpeed = this.MaxSpeed,
                IsMoving = this.IsMoving,
                MovementType = this.MovementType
            };
        }

        public override string ToString() =>
            $"Movement(Speed={Speed:F2}, MaxSpeed={MaxSpeed:F2}, Type={MovementType})";
    }

    /// <summary>
    /// Combat component for combat-capable entities.
    /// </summary>
    public class CombatComponent
    {
        public int AttackPower { get; set; }
        public int AttackSpeed { get; set; }
        public float AttackRange { get; set; }
        public string WeaponType { get; set; }
        public int Experience { get; set; }
        public int Level { get; set; }
        public IList<string> Abilities { get; set; }

        public CombatComponent()
        {
            Abilities = new List<string>();
        }

        public CombatComponent Clone()
        {
            var clone = new CombatComponent
            {
                AttackPower = this.AttackPower,
                AttackSpeed = this.AttackSpeed,
                AttackRange = this.AttackRange,
                WeaponType = this.WeaponType,
                Experience = this.Experience,
                Level = this.Level
            };

            foreach (var ability in this.Abilities)
            {
                clone.Abilities.Add(ability);
            }

            return clone;
        }

        public override string ToString() =>
            $"Combat(Power={AttackPower}, Speed={AttackSpeed}, Range={AttackRange}, Level={Level})";
    }

    /// <summary>
    /// Entity factory using prototype pattern.
    /// </summary>
    public class EntityFactory
    {
        private readonly Dictionary<string, GameEntity> _prototypes =
            new Dictionary<string, GameEntity>();

        public void RegisterPrototype(string name, GameEntity entity)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Name cannot be null or empty", nameof(name));
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            _prototypes[name] = entity;
        }

        public GameEntity CreateEntity(string prototypeName)
        {
            if (!_prototypes.ContainsKey(prototypeName))
                throw new KeyNotFoundException($"Prototype '{prototypeName}' not registered");

            return _prototypes[prototypeName].Clone();
        }

        public GameEntity CreateEntity(string prototypeName, string name)
        {
            var entity = CreateEntity(prototypeName);
            entity.Name = name;
            return entity;
        }

        public bool HasPrototype(string name) => _prototypes.ContainsKey(name);

        public int PrototypeCount => _prototypes.Count;
    }
}
