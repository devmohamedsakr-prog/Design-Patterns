using System;
using System.Collections.Generic;

namespace Flyweight.GameRendering.Tiles.Component
{
    // Intrinsic State: Shared terrain type
    public class TerrainType
    {
        public string Name { get; set; }
        public string SpriteTexture { get; set; }
        public int MovementCost { get; set; }
        public string Color { get; set; }
        public bool IsWalkable { get; set; }

        public override string ToString() => $"{Name} (Move:{MovementCost}, Walkable:{IsWalkable})";
    }

    // Flyweight Factory for terrain types
    public class TerrainTypeFactory
    {
        private Dictionary<string, TerrainType> _terrainPool = new();

        public TerrainTypeFactory()
        {
            _terrainPool["Grass"] = new TerrainType { Name = "Grass", SpriteTexture = "grass.png", MovementCost = 1, Color = "#00AA00", IsWalkable = true };
            _terrainPool["Water"] = new TerrainType { Name = "Water", SpriteTexture = "water.png", MovementCost = 5, Color = "#0000FF", IsWalkable = false };
            _terrainPool["Mountain"] = new TerrainType { Name = "Mountain", SpriteTexture = "mountain.png", MovementCost = 3, Color = "#808080", IsWalkable = false };
            _terrainPool["Sand"] = new TerrainType { Name = "Sand", SpriteTexture = "sand.png", MovementCost = 2, Color = "#FFFF00", IsWalkable = true };
        }

        public TerrainType GetTerrain(string terrainName)
        {
            return _terrainPool.ContainsKey(terrainName) ? _terrainPool[terrainName] : null;
        }

        public int GetPoolSize() => _terrainPool.Count;
        public IReadOnlyDictionary<string, TerrainType> GetPool() => _terrainPool;
    }

    // Extrinsic State: Per-tile unique data
    public class Tile
    {
        public int X { get; set; }
        public int Y { get; set; }
        public TerrainType Terrain { get; set; }
        public bool IsOccupied { get; set; }
        public bool IsHighlighted { get; set; }

        public override string ToString() => $"Tile({X},{Y}) {Terrain.Name}";
    }

    // Game Map using Flyweight pattern
    public class GameMap
    {
        private Dictionary<(int, int), Tile> _tiles = new();
        private TerrainTypeFactory _terrainFactory = new();

        public void CreateTile(int x, int y, string terrainName)
        {
            var terrain = _terrainFactory.GetTerrain(terrainName);
            if (terrain != null)
            {
                _tiles[(x, y)] = new Tile { X = x, Y = y, Terrain = terrain, IsOccupied = false };
            }
        }

        public void CreateMap(int width, int height)
        {
            var terrains = new[] { "Grass", "Water", "Mountain", "Sand" };
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    var terrainName = terrains[(x + y) % terrains.Length];
                    CreateTile(x, y, terrainName);
                }
            }
        }

        public Tile GetTile(int x, int y) => _tiles.ContainsKey((x, y)) ? _tiles[(x, y)] : null;

        public void HighlightTile(int x, int y)
        {
            if (_tiles.ContainsKey((x, y)))
                _tiles[(x, y)].IsHighlighted = true;
        }

        public int GetTileCount() => _tiles.Count;
        public int GetUniqueTerrainCount() => _terrainFactory.GetPoolSize();
        public long EstimateMemorySavings() => (long)_tiles.Count * 500 - _terrainFactory.GetPoolSize() * 500 - _tiles.Count * 64;
    }
}
