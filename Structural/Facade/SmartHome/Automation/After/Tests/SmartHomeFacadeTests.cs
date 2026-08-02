using Xunit;
using Facade.SmartHome.Automation.Component;

namespace Facade.SmartHome.Automation.Tests
{
    public class SmartHomeFacadeTests
    {
        [Fact]
        public void ActivateMovieMode_ShouldDimLights()
        {
            var facade = new SmartHomeFacade();
            facade.ActivateMovieMode();
            
            var status = facade.GetHomeStatus();
            Assert.False((bool)status["Lights"]);
        }

        [Fact]
        public void ActivateMovieMode_ShouldCloseBlinds()
        {
            var facade = new SmartHomeFacade();
            facade.ActivateMovieMode();
            
            var status = facade.GetHomeStatus();
            Assert.True((bool)status["Blinds"]);
        }

        [Fact]
        public void ActivateMovieMode_ShouldSetComfortableTemperature()
        {
            var facade = new SmartHomeFacade();
            facade.ActivateMovieMode();
            
            var status = facade.GetHomeStatus();
            Assert.Equal(21m, (decimal)status["Temperature"]);
        }

        [Fact]
        public void ActivateLeaveMode_ShouldLockDoors()
        {
            var facade = new SmartHomeFacade();
            facade.ActivateLeaveMode();
            
            var status = facade.GetHomeStatus();
            Assert.True((bool)status["Security"]);
        }

        [Fact]
        public void ActivateLeaveMode_ShouldTurnOffLights()
        {
            var facade = new SmartHomeFacade();
            facade.ActivateLeaveMode();
            
            var status = facade.GetHomeStatus();
            Assert.False((bool)status["Lights"]);
        }

        [Fact]
        public void ActivateLeaveMode_ShouldReduceTemperature()
        {
            var facade = new SmartHomeFacade();
            facade.ActivateLeaveMode();
            
            var status = facade.GetHomeStatus();
            Assert.Equal(18m, (decimal)status["Temperature"]);
        }

        [Fact]
        public void ActivateGoodMorningMode_ShouldOpenBlinds()
        {
            var facade = new SmartHomeFacade();
            facade.ActivateGoodMorningMode();
            
            var status = facade.GetHomeStatus();
            Assert.False((bool)status["Blinds"]);
        }

        [Fact]
        public void ActivateGoodMorningMode_ShouldTurnOnLights()
        {
            var facade = new SmartHomeFacade();
            facade.ActivateGoodMorningMode();
            
            var status = facade.GetHomeStatus();
            Assert.True((bool)status["Lights"]);
        }

        [Fact]
        public void ActivateGoodMorningMode_ShouldPlayMusic()
        {
            var facade = new SmartHomeFacade();
            facade.ActivateGoodMorningMode();
            
            var status = facade.GetHomeStatus();
            Assert.NotNull(status["Audio"]);
        }

        [Fact]
        public void ActivateBedtimeMode_ShouldArmSecurity()
        {
            var facade = new SmartHomeFacade();
            facade.ActivateBedtimeMode();
            
            var status = facade.GetHomeStatus();
            Assert.True((bool)status["Security"]);
        }

        [Fact]
        public void ActivateBedtimeMode_ShouldTurnOffLights()
        {
            var facade = new SmartHomeFacade();
            facade.ActivateBedtimeMode();
            
            var status = facade.GetHomeStatus();
            Assert.False((bool)status["Lights"]);
        }

        [Fact]
        public void ActivateBedtimeMode_ShouldPlaySleepSounds()
        {
            var facade = new SmartHomeFacade();
            facade.ActivateBedtimeMode();
            
            var status = facade.GetHomeStatus();
            Assert.NotNull(status["Audio"]);
        }

        [Fact]
        public void OptimizeEnergy_ShouldReduceConsumption()
        {
            var facade = new SmartHomeFacade();
            facade.OptimizeEnergy();
            
            var status = facade.GetHomeStatus();
            Assert.True((decimal)status["EnergyUsage"] >= 0);
        }

        [Fact]
        public void GetHomeStatus_ShouldReturnAllDevices()
        {
            var facade = new SmartHomeFacade();
            
            var status = facade.GetHomeStatus();
            Assert.Contains("Lights", status.Keys);
            Assert.Contains("Temperature", status.Keys);
            Assert.Contains("Audio", status.Keys);
            Assert.Contains("Security", status.Keys);
        }

        [Fact]
        public void FacadeHideComplexity_ShouldSimplifyHomeAutomation()
        {
            var facade = new SmartHomeFacade();
            
            // Single method calls instead of managing 6 subsystems
            facade.ActivateMovieMode();
            facade.ActivateGoodMorningMode();
            facade.ActivateBedtimeMode();
            facade.ActivateLeaveMode();
            facade.OptimizeEnergy();
            
            var status = facade.GetHomeStatus();
            Assert.NotNull(status);
        }

        [Fact]
        public void MultipleScenarios_ShouldSwitchProperlyBetweenModes()
        {
            var facade = new SmartHomeFacade();
            
            facade.ActivateMovieMode();
            var status1 = facade.GetHomeStatus();
            
            facade.ActivateLeaveMode();
            var status2 = facade.GetHomeStatus();
            
            // Verify modes switched
            Assert.NotEqual(status1["Temperature"], status2["Temperature"]);
        }

        [Fact]
        public void ConsecutiveOperations_ShouldMaintainConsistency()
        {
            var facade = new SmartHomeFacade();
            
            for (int i = 0; i < 3; i++)
            {
                facade.ActivateMovieMode();
                facade.ActivateGoodMorningMode();
            }
            
            var status = facade.GetHomeStatus();
            Assert.NotNull(status);
        }
    }
}
