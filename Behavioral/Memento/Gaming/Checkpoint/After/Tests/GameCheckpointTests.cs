using NUnit.Framework;
using GameCheckpoint.After.Context;

namespace GameCheckpoint.After.Tests
{
    [TestFixture]
    public class GameCheckpointMementoTests
    {
        private Character _character;
        private GameCheckpointManager _manager;

        [SetUp]
        public void Setup()
        {
            _character = new Character("Hero");
            _manager = new GameCheckpointManager();
        }

        [Test]
        public void SaveCheckpoint_Success()
        {
            _character.GainExperience(50);
            _manager.SaveCheckpoint(_character, "AfterFirstQuest");
            
            Assert.That(_manager.GetCheckpointCount(), Is.EqualTo(1));
        }

        [Test]
        public void SaveAndLoadCheckpoint()
        {
            _character.TakeDamage(30);
            _character.GainExperience(100);
            var originalHealth = _character.Health;
            var originalExp = _character.Experience;
            
            _manager.SaveCheckpoint(_character, "Checkpoint1");

            _character.TakeDamage(50);
            _character.GainExperience(50);
            
            _manager.LoadCheckpoint(_character, "Checkpoint1");
            
            Assert.That(_character.Health, Is.EqualTo(originalHealth));
            Assert.That(_character.Experience, Is.EqualTo(originalExp));
        }

        [Test]
        public void LevelUp_Checkpoint()
        {
            _character.GainExperience(100); // Causes level up
            int levelAfterGain = _character.Level;
            int maxHealthAfterGain = _character.MaxHealth;
            
            _manager.SaveCheckpoint(_character, "LevelUpSave");

            _character.TakeDamage(200);
            
            _manager.LoadCheckpoint(_character, "LevelUpSave");
            
            Assert.That(_character.Level, Is.EqualTo(levelAfterGain));
            Assert.That(_character.MaxHealth, Is.EqualTo(maxHealthAfterGain));
            Assert.That(_character.Health, Is.EqualTo(maxHealthAfterGain));
        }

        [Test]
        public void MultipleCheckpoints()
        {
            _character.GainExperience(50);
            _manager.SaveCheckpoint(_character, "Save1");

            _character.MoveTo("ForestDungeon");
            _character.TakeDamage(20);
            _manager.SaveCheckpoint(_character, "Save2");

            _character.MoveTo("CastleKeep");
            _character.GainExperience(200);
            _manager.SaveCheckpoint(_character, "Save3");

            Assert.That(_manager.GetCheckpointCount(), Is.EqualTo(3));
        }

        [Test]
        public void RestoreOldCheckpoint()
        {
            _character.MoveTo("VillageStart");
            _manager.SaveCheckpoint(_character, "Beginning");

            // Progress significantly
            _character.MoveTo("LateGameZone");
            _character.GainExperience(500);
            
            // Restore to beginning
            _manager.LoadCheckpoint(_character, "Beginning");
            
            Assert.That(_character.CurrentLocation, Is.EqualTo("VillageStart"));
            Assert.That(_character.Level, Is.EqualTo(1));
        }

        [Test]
        public void DeleteCheckpoint()
        {
            _manager.SaveCheckpoint(_character, "ToDelete");
            _manager.DeleteCheckpoint("ToDelete");
            
            Assert.That(_manager.GetCheckpointCount(), Is.EqualTo(0));
        }

        [Test]
        public void GetAvailableCheckpoints()
        {
            _manager.SaveCheckpoint(_character, "Save1");
            _manager.SaveCheckpoint(_character, "Save2");
            _manager.SaveCheckpoint(_character, "Save3");

            var checkpoints = _manager.GetAvailableCheckpoints();
            Assert.That(checkpoints.Count, Is.EqualTo(3));
            Assert.That(checkpoints, Does.Contain("Save1"));
        }

        [Test]
        public void FullCombatSequence()
        {
            _character.MoveTo("BossDungeon");
            _character.Heal(100);
            _manager.SaveCheckpoint(_character, "BeforeBoss");

            _character.TakeDamage(80);
            Assert.That(_character.Health, Is.EqualTo(20));

            _manager.LoadCheckpoint(_character, "BeforeBoss");
            Assert.That(_character.Health, Is.EqualTo(100));
        }

        [Test]
        public void CheckpointTimestamp()
        {
            _manager.SaveCheckpoint(_character, "TimedSave");
            var checkpoint = _manager.GetCheckpoint("TimedSave");
            
            Assert.That(checkpoint?.SaveTime, Is.LessThanOrEqualTo(DateTime.Now.AddSeconds(1)));
            Assert.That(checkpoint?.SaveTime, Is.GreaterThanOrEqualTo(DateTime.Now.AddSeconds(-5)));
        }

        [Test]
        public void SaveHistory()
        {
            _manager.SaveCheckpoint(_character, "Save1");
            _character.GainExperience(100);
            _manager.SaveCheckpoint(_character, "Save2");

            var history = _manager.GetSaveHistory();
            Assert.That(history.Count, Is.EqualTo(2));
            Assert.That(history[0].CheckpointName, Is.EqualTo("Save1"));
            Assert.That(history[1].CheckpointName, Is.EqualTo("Save2"));
        }

        [Test]
        public void ComplexCharacterProgression()
        {
            // Early game
            _character.GainExperience(50);
            _manager.SaveCheckpoint(_character, "EarlyGame");

            // Mid game
            _character.MoveTo("SecondZone");
            _character.GainExperience(150);
            _manager.SaveCheckpoint(_character, "MidGame");

            // Late game
            _character.MoveTo("EndgameZone");
            _character.GainExperience(300);
            _manager.SaveCheckpoint(_character, "LateGame");

            // Restore mid game
            _manager.LoadCheckpoint(_character, "MidGame");
            Assert.That(_character.CurrentLocation, Is.EqualTo("SecondZone"));
        }
    }
}
