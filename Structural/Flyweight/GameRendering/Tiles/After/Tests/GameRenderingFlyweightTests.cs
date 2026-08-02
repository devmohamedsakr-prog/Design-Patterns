using Xunit;
using Flyweight.GameRendering.Tiles.Component;

namespace Flyweight.GameRendering.Tiles.Tests
{
    public class GameRenderingFlyweightTests
    {
        [Fact]
        public void TerrainTypeFactory_ShouldCreateBasicTerrains()
        {
            var factory = new TerrainTypeFactory();
            Assert.Equal(4, factory.GetPoolSize());
        }

        [Fact]
        public void TerrainTypeFactory_ShouldReuseTerrain()
        {
            var factory = new TerrainTypeFactory();
            var grass1 = factory.GetTerrain("Grass");
            var grass2 = factory.GetTerrain("Grass");
            
            Assert.Same(grass1, grass2);
        }

        [Fact]
        public void TerrainType_ShouldHaveCorrectProperties()
        {
            var factory = new TerrainTypeFactory();
            var grass = factory.GetTerrain("Grass");
            
            Assert.Equal("Grass", grass.Name);
            Assert.True(grass.IsWalkable);
            Assert.Equal(1, grass.MovementCost);
        }

        [Fact]
        public void GameMap_ShouldCreateTiles()
        {
            var map = new GameMap();
            map.CreateTile(0, 0, "Grass");
            
            Assert.Equal(1, map.GetTileCount());
        }

        [Fact]
        public void GameMap_ShouldReuseTerrain()
        {
            var map = new GameMap();
            map.CreateTile(0, 0, "Grass");
            map.CreateTile(1, 0, "Grass");
            map.CreateTile(2, 0, "Grass");
            
            Assert.Equal(3, map.GetTileCount());
            Assert.Equal(1, map.GetUniqueTerrainCount()); // All same terrain
        }

        [Fact]
        public void GameMap_ShouldCreateCompleteMap()
        {
            var map = new GameMap();
            map.CreateMap(100, 100);
            
            Assert.Equal(10000, map.GetTileCount());
            Assert.True(map.GetUniqueTerrainCount() <= 4);
        }

        [Fact]
        public void GameMap_ShouldHighlightTile()
        {
            var map = new GameMap();
            map.CreateTile(5, 5, "Grass");
            map.HighlightTile(5, 5);
            
            var tile = map.GetTile(5, 5);
            Assert.True(tile.IsHighlighted);
        }

        [Fact]
        public void GameMap_ShouldCalculateMemorySavings()
        {
            var map = new GameMap();
            map.CreateMap(500, 500);
            
            var savings = map.EstimateMemorySavings();
            Assert.True(savings > 0);
        }

        [Fact]
        public void TerrainType_ShouldHaveDifferentMovementCosts()
        {
            var factory = new TerrainTypeFactory();
            
            var grass = factory.GetTerrain("Grass");
            var water = factory.GetTerrain("Water");
            var mountain = factory.GetTerrain("Mountain");
            
            Assert.Equal(1, grass.MovementCost);
            Assert.Equal(5, water.MovementCost);
            Assert.Equal(3, mountain.MovementCost);
        }

        [Fact]
        public void GameMap_ShouldRetrieveTile()
        {
            var map = new GameMap();
            map.CreateTile(10, 20, "Mountain");
            
            var tile = map.GetTile(10, 20);
            Assert.NotNull(tile);
            Assert.Equal(10, tile.X);
            Assert.Equal(20, tile.Y);
        }

        [Fact]
        public void GameMap_ShouldReturnNullForNonexistentTile()
        {
            var map = new GameMap();
            var tile = map.GetTile(999, 999);
            
            Assert.Null(tile);
        }

        [Fact]
        public void LargeMappedGame_ShouldHandleManyTiles()
        {
            var map = new GameMap();
            map.CreateMap(1000, 1000);
            
            Assert.Equal(1000000, map.GetTileCount());
            Assert.True(map.EstimateMemorySavings() > 100000000); // Huge savings
        }
    }
}
