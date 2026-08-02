using Xunit;
using Prototype.Game.Entity.Context;
using System;

namespace Prototype.Game.Entity.Tests
{
    public class GameEntityTests
    {
        private GameEntity CreateSampleEnemy()
        {
            var enemy = new GameEntity
            {
                Name = "Goblin",
                Type = "Enemy",
                IsActive = true
            };

            enemy.Position.X = 10;
            enemy.Position.Y = 5;
            enemy.Position.Z = 0;

            enemy.Scale.X = 1;
            enemy.Scale.Y = 1;
            enemy.Scale.Z = 1;

            enemy.Health.MaxHealth = 50;
            enemy.Health.CurrentHealth = 50;
            enemy.Health.Armor = 5;

            enemy.Movement.Speed = 5.5f;
            enemy.Movement.MaxSpeed = 10f;
            enemy.Movement.MovementType = "Walk";

            enemy.Combat.AttackPower = 15;
            enemy.Combat.AttackSpeed = 2;
            enemy.Combat.AttackRange = 1.5f;
            enemy.Combat.WeaponType = "Club";
            enemy.Combat.Level = 3;

            enemy.Tags.Add("Hostile");
            enemy.Tags.Add("Melee");

            return enemy;
        }

        [Fact]
        public void Clone_CreatesIndependentCopy()
        {
            var original = CreateSampleEnemy();
            var clone = original.Clone();

            Assert.NotSame(original, clone);
            Assert.Equal(original.Name, clone.Name);
            Assert.NotSame(original.Position, clone.Position);
            Assert.NotSame(original.Health, clone.Health);
        }

        [Fact]
        public void Clone_ChangeToCloneDoesNotAffectOriginal()
        {
            var original = CreateSampleEnemy();
            var clone = original.Clone();

            clone.Name = "Modified Goblin";
            clone.Position.X = 20;
            clone.Health.CurrentHealth = 25;

            Assert.Equal("Goblin", original.Name);
            Assert.Equal(10, original.Position.X);
            Assert.Equal(50, original.Health.CurrentHealth);
        }

        [Fact]
        public void Clone_DeepCopiesPosition()
        {
            var original = CreateSampleEnemy();
            var clone = original.Clone();

            clone.Position.X = 50;
            clone.Position.Y = 100;
            clone.Position.Z = 200;

            Assert.Equal(10, original.Position.X);
            Assert.Equal(5, original.Position.Y);
            Assert.Equal(0, original.Position.Z);
        }

        [Fact]
        public void Clone_DeepCopiesHealth()
        {
            var original = CreateSampleEnemy();
            var clone = original.Clone();

            clone.Health.CurrentHealth = 10;
            clone.Health.Armor = 10;

            Assert.Equal(50, original.Health.CurrentHealth);
            Assert.Equal(5, original.Health.Armor);
        }

        [Fact]
        public void Clone_DeepCopiesMovement()
        {
            var original = CreateSampleEnemy();
            var clone = original.Clone();

            clone.Movement.Speed = 15f;
            clone.Movement.MovementType = "Run";

            Assert.Equal(5.5f, original.Movement.Speed);
            Assert.Equal("Walk", original.Movement.MovementType);
        }

        [Fact]
        public void Clone_DeepCopiesCombat()
        {
            var original = CreateSampleEnemy();
            var clone = original.Clone();

            clone.Combat.AttackPower = 50;
            clone.Combat.Level = 10;

            Assert.Equal(15, original.Combat.AttackPower);
            Assert.Equal(3, original.Combat.Level);
        }

        [Fact]
        public void Clone_DeepCopiesTags()
        {
            var original = CreateSampleEnemy();
            var clone = original.Clone();

            clone.Tags.Add("Flying");

            Assert.Equal(2, original.Tags.Count);
            Assert.Equal(3, clone.Tags.Count);
        }

        [Fact]
        public void Factory_RegisterAndCreate_Success()
        {
            var factory = new EntityFactory();
            var goblinPrototype = CreateSampleEnemy();

            factory.RegisterPrototype("Goblin", goblinPrototype);
            var createdEntity = factory.CreateEntity("Goblin");

            Assert.NotSame(goblinPrototype, createdEntity);
            Assert.Equal("Goblin", createdEntity.Name);
            Assert.Equal(50, createdEntity.Health.CurrentHealth);
        }

        [Fact]
        public void Factory_CreateMultipleInstances_AllIndependent()
        {
            var factory = new EntityFactory();
            factory.RegisterPrototype("Goblin", CreateSampleEnemy());

            var entity1 = factory.CreateEntity("Goblin", "Goblin1");
            var entity2 = factory.CreateEntity("Goblin", "Goblin2");
            var entity3 = factory.CreateEntity("Goblin", "Goblin3");

            Assert.Equal("Goblin1", entity1.Name);
            Assert.Equal("Goblin2", entity2.Name);
            Assert.Equal("Goblin3", entity3.Name);

            entity1.Health.CurrentHealth = 10;
            Assert.Equal(50, entity2.Health.CurrentHealth);
        }

        [Fact]
        public void Factory_RegisterNullPrototype_ThrowsException()
        {
            var factory = new EntityFactory();

            var exception = Assert.Throws<ArgumentNullException>(() =>
                factory.RegisterPrototype("Goblin", null)
            );

            Assert.Contains("entity", exception.Message);
        }

        [Fact]
        public void Factory_CreateNonExistentPrototype_ThrowsException()
        {
            var factory = new EntityFactory();

            var exception = Assert.Throws<KeyNotFoundException>(() =>
                factory.CreateEntity("NonExistent")
            );

            Assert.Contains("not registered", exception.Message);
        }

        [Fact]
        public void Factory_MultiplePrototypes_Success()
        {
            var factory = new EntityFactory();

            var goblin = CreateSampleEnemy();
            goblin.Name = "Goblin";

            var orc = CreateSampleEnemy();
            orc.Name = "Orc";
            orc.Combat.AttackPower = 25;

            factory.RegisterPrototype("Goblin", goblin);
            factory.RegisterPrototype("Orc", orc);

            Assert.Equal(2, factory.PrototypeCount);
            
            var goblinInstance = factory.CreateEntity("Goblin");
            var orcInstance = factory.CreateEntity("Orc");

            Assert.Equal(15, goblinInstance.Combat.AttackPower);
            Assert.Equal(25, orcInstance.Combat.AttackPower);
        }

        [Fact]
        public void Vector3_Clone_Independent()
        {
            var original = new Vector3 { X = 10, Y = 20, Z = 30 };
            var clone = original.Clone();

            clone.X = 100;
            clone.Y = 200;

            Assert.Equal(10, original.X);
            Assert.Equal(20, original.Y);
        }

        [Fact]
        public void Transform_Clone_Independent()
        {
            var original = new Transform();
            original.Position.X = 5;
            original.Rotation.Y = 90;
            original.ParentName = "World";

            var clone = original.Clone();
            clone.Position.X = 15;
            clone.ParentName = "Container";

            Assert.Equal(5, original.Position.X);
            Assert.Equal("World", original.ParentName);
        }

        [Fact]
        public void HealthComponent_Clone_Independent()
        {
            var original = new HealthComponent
            {
                MaxHealth = 100,
                CurrentHealth = 50
            };
            original.StatusEffects.Add("Poisoned");

            var clone = original.Clone();
            clone.CurrentHealth = 25;
            clone.StatusEffects.Add("Burned");

            Assert.Equal(50, original.CurrentHealth);
            Assert.Single(original.StatusEffects);
        }

        [Fact]
        public void MovementComponent_Clone_Independent()
        {
            var original = new MovementComponent
            {
                Speed = 5.5f,
                MaxSpeed = 10f,
                MovementType = "Walk"
            };

            var clone = original.Clone();
            clone.Speed = 15f;
            clone.MovementType = "Run";

            Assert.Equal(5.5f, original.Speed);
            Assert.Equal("Walk", original.MovementType);
        }

        [Fact]
        public void CombatComponent_Clone_Independent()
        {
            var original = new CombatComponent
            {
                AttackPower = 20,
                Level = 5
            };
            original.Abilities.Add("Fireball");

            var clone = original.Clone();
            clone.AttackPower = 50;
            clone.Abilities.Add("Lightning");

            Assert.Equal(20, original.AttackPower);
            Assert.Single(original.Abilities);
        }

        [Fact]
        public void Clone_PreservesScale()
        {
            var original = CreateSampleEnemy();
            var clone = original.Clone();

            Assert.Equal(1, clone.Scale.X);
            Assert.Equal(1, clone.Scale.Y);
            Assert.Equal(1, clone.Scale.Z);
        }

        [Fact]
        public void GameEntity_ToString_ContainsInfo()
        {
            var entity = CreateSampleEnemy();
            var str = entity.ToString();

            Assert.Contains("Goblin", str);
            Assert.Contains("Enemy", str);
        }

        [Fact]
        public void Factory_HasPrototype_ChecksCorrectly()
        {
            var factory = new EntityFactory();
            factory.RegisterPrototype("Goblin", CreateSampleEnemy());

            Assert.True(factory.HasPrototype("Goblin"));
            Assert.False(factory.HasPrototype("Orc"));
        }

        [Fact]
        public void Clone_ChainedClones_AllIndependent()
        {
            var original = CreateSampleEnemy();
            var clone1 = original.Clone();
            var clone2 = clone1.Clone();

            clone2.Health.CurrentHealth = 5;
            clone2.Position.X = 100;

            Assert.Equal(50, original.Health.CurrentHealth);
            Assert.Equal(50, clone1.Health.CurrentHealth);
            Assert.Equal(10, original.Position.X);
            Assert.Equal(10, clone1.Position.X);
        }

        [Fact]
        public void Clone_DeepCopiesTransform()
        {
            var original = CreateSampleEnemy();
            original.Transform.ParentName = "Arena";

            var clone = original.Clone();
            clone.Transform.ParentName = "Room1";

            Assert.Equal("Arena", original.Transform.ParentName);
            Assert.Equal("Room1", clone.Transform.ParentName);
        }

        [Fact]
        public void Factory_CreateWithCustomName_Success()
        {
            var factory = new EntityFactory();
            factory.RegisterPrototype("Goblin", CreateSampleEnemy());

            var entity = factory.CreateEntity("Goblin", "Boss Goblin");

            Assert.Equal("Boss Goblin", entity.Name);
            Assert.Equal(50, entity.Health.CurrentHealth);
        }
    }
}
