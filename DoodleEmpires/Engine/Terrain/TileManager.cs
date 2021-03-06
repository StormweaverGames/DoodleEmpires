﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DoodleEmpires.Engine.Utilities;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace DoodleEmpires.Engine.Terrain
{
    /// <summary>
    /// Represents a tile manager 
    /// </summary>
    public class TileManager
    {
        List<Tile> _tileTypes = new List<Tile>();
        bool[,] _connections = new bool[256,256];
        Dictionary<string, byte> _tiles = new Dictionary<string, byte>();

        /// <summary>
        /// Gets the list of tiles
        /// </summary>
        public List<Tile> Tiles
        {
            get { return _tileTypes; }
        }
        /// <summary>
        /// Gets the tile with the given ID
        /// </summary>
        /// <param name="ID">The ID of the tile</param>
        /// <returns>The Tile with the given ID</returns>
        public Tile this[int ID]
        {
            get { return _tileTypes[ID]; }
        }
        /// <summary>
        /// Gets the ID of the tile with the name
        /// </summary>
        /// <param name="name">The name of the tile to get</param>
        /// <returns>The ID of the tile with the given name</returns>
        public byte this[string name]
        {
            get { return _tiles[name.ToLower()]; }
        }

        /// <summary>
        /// Creates a new tile manager
        /// </summary>
        public TileManager()
        {
            RegisterTile("Air",0, Color.Transparent, RenderType.None, false); //adds the air tile type
        }

        /// <summary>
        /// Adds a new unnamed tile type to this tile manager
        /// </summary>
        /// <param name="texID">The texture ID for the tile</param>
        /// <param name="renderType">The render mode for this tile</param>
        /// <param name="solid">True if this tile should be treated as solid during collision detection</param>
        /// <returns>The TileID for the new tile</returns>
        public byte RegisterTile(short texID, RenderType renderType = RenderType.Land, bool solid = false)
        {
            string name = GenName();
            Tile tile = new Tile((byte)_tileTypes.Count, texID, renderType, solid);
            tile.Type = (byte)_tileTypes.Count;
            _tileTypes.Add(tile);
            RegisterConnect(tile.Type, tile.Type);
            _tiles.Add((name).ToLower(), tile.Type);
            return tile.Type;
        }

        /// <summary>
        /// Adds a new tile type to this tile manager
        /// </summary>
        /// <param name="name">The name of the tile to add</param>
        /// <param name="texID">The texture ID for the tile</param>
        /// <param name="renderType">The render mode for this tile</param>
        /// <param name="solid">True if this tile should be treated as solid during collision detection</param>
        /// <returns>The TileID for the new tile</returns>
        public byte RegisterTile(string name, short texID, RenderType renderType = RenderType.Land, bool solid = false)
        {
            if (!_tiles.ContainsKey(name))
            {
                Tile tile = new Tile((byte)_tileTypes.Count, texID, renderType, solid);
                tile.Type = (byte)_tileTypes.Count;
                _tileTypes.Add(tile);
                RegisterConnect(tile.Type, tile.Type);
                _tiles.Add((name != null ? name : GenName()).ToLower(), tile.Type);
                return tile.Type;
            }
            else
                throw new ArgumentException(string.Format("There is already a tile named \"{0}\"", name));
        }

        /// <summary>
        /// Adds a new tile type to this tile manager
        /// </summary>
        /// <param name="name">The name of the tile to add</param>
        /// <param name="texID">The texture ID for the tile</param>
        /// <param name="Color">The color for this tile</param>
        /// <param name="renderType">The render mode for this tile</param>
        /// <param name="solid">True if this tile should be treated as solid during collision detection</param>
        /// <returns>The TileID for the new tile</returns>
        public byte RegisterTile(string name, short texID,  Color Color, RenderType renderType = RenderType.Land, bool solid = false)
        {
            if (!_tiles.ContainsKey(name))
            {
                Tile tile = new Tile((byte)_tileTypes.Count, texID, renderType, solid) { Color = Color };
                tile.Type = (byte)_tileTypes.Count;
                _tileTypes.Add(tile);
                RegisterConnect(tile.Type, tile.Type);
                _tiles.Add((name != null ? name : GenName()).ToLower(), tile.Type);
                return tile.Type;
            }
            else
                throw new ArgumentException(string.Format("There is already a tile named \"{0}\"", name));
        }

        /// <summary>
        /// Adds a new tile type to this tile manager
        /// </summary>
        /// <param name="name">The name of the tile to register</param>
        /// <param name="tile">The tile to add</param>
        /// <returns>The TileID for the new tile</returns>
        public byte RegisterTile(string name, Tile tile)
        {
            if (!_tiles.ContainsKey(name))
            {
                tile.Type = (byte)_tileTypes.Count;
                _tileTypes.Add(tile);
                RegisterConnect(tile.Type, tile.Type);
                _tiles.Add((name != null ? name : GenName()).ToLower(), tile.Type);
                return tile.Type;
            }
            else
                throw new ArgumentException(string.Format("There is already a tile named \"{0}\"", name));
        }

        /// <summary>
        /// Gets the name of the tile with the given ID
        /// </summary>
        /// <param name="ID">The ID of the tile</param>
        /// <returns>The name of tile with the given ID</returns>
        public string NameOf(byte ID)
        {
            return _tiles.First( x => x.Value == ID).Key;
        }

        /// <summary>
        /// Gets the index of the tile with the given name
        /// </summary>
        /// <param name="name">The name of the tile</param>
        /// <returns>The index of tile with the given name</returns>
        public byte IndexOf(string name)
        {
            return _tiles.First(x => x.Key == name).Value;
        }

        /// <summary>
        /// Renders a tile to the screen
        /// </summary>
        /// <param name="spriteBatch">The spritebatch to use for rendering</param>
        /// <param name="bounds">The bounds to render the tile to</param>
        /// <param name="atlas">The texture atlas to look up textures from</param>
        /// <param name="mooreState">The neighbour states for this block</param>
        /// <param name="tileID">The ID of the tile to render</param>
        /// <param name="metaData">The meta data for the tile to render</param>
        /// <param name="diffuseColor">The color to transform everything by</param>
        public void RenderTile(SpriteBatch spriteBatch, Rectangle bounds, TextureAtlas atlas, byte mooreState, 
            byte tileID, byte metaData, Color diffuseColor)
        {
            _tileTypes[tileID].Draw(spriteBatch, atlas, bounds, mooreState, metaData);
        }

        /// <summary>
        /// Checks if a tile will collide with the given bounds and check rectangles
        /// </summary>
        /// <param name="tileID">The ID of the tile</param>
        /// <param name="rect">The rectangle to check</param>
        /// <param name="bounds">The bounds of the tile</param>
        /// <returns>True if <i>rect</i> intersects the tile</returns>
        public bool CheckCollision(byte tileID, Rectangle rect, Rectangle bounds)
        {
            return _tileTypes[tileID].Intersects(rect, bounds);
        }

        /// <summary>
        /// Checks if a given tile ID is solid
        /// </summary>
        /// <param name="tileID">The tile ID to check</param>
        /// <returns>True if the tile type is solid</returns>
        public bool IsSolid(byte tileID)
        {
            return _tileTypes[tileID].Solid;
        }

        /// <summary>
        /// Checks if a given tile ID is climable
        /// </summary>
        /// <param name="tileID">The tile ID to check</param>
        /// <returns>True if the tile type is climable</returns>
        public bool IsClimable(byte tileID)
        {
            return _tileTypes[tileID].Climable;
        }

        /// <summary>
        /// Checks if two tiles can connect
        /// </summary>
        /// <param name="sourceID">The source tile ID to check from</param>
        /// <param name="destinationID">The destination tile ID to check to</param>
        /// <returns>True if the tiles can connect</returns>
        public bool CanConnect(byte sourceID, byte destinationID)
        {
            return _connections[sourceID, destinationID];
        }

        /// <summary>
        /// Registers a connection between 2 tiles, will register the connection for both source and destination of each
        /// is setting to true
        /// </summary>
        /// <param name="sourceID">The ID of the first tile</param>
        /// <param name="destinationID">The ID of the second tile</param>
        /// <param name="canConnect">Whether or not these tiles can connect</param>
        public void RegisterConnect(byte sourceID, byte destinationID, bool canConnect = true)
        {
            _connections[sourceID, destinationID] = canConnect;
            if (canConnect)
                _connections[destinationID, sourceID] = canConnect;
        }

        /// <summary>
        /// Registers a connection between 2 tiles, will register the connection for both source and destination of each
        /// is setting to true
        /// </summary>
        /// <param name="tile1">The name of the first tile</param>
        /// <param name="tile2">The name of the second tile</param>
        /// <param name="canConnect">Whether or not these tiles can connect</param>
        public void RegisterConnect(string tile1, string tile2, bool canConnect = true)
        {
            tile1 = tile1.ToLower();
            tile2 = tile2.ToLower();

            if (_tiles.ContainsKey(tile1) && _tiles.ContainsKey(tile2))
            {
                _connections[_tiles[tile1], _tiles[tile2]] = canConnect;

                if (canConnect)
                    _connections[_tiles[tile2], _tiles[tile1]] = canConnect;
            }
        }

        /// <summary>
        /// Registers a connection between 1 tile and another. The second tile will not connect to the first
        /// </summary>
        /// <param name="sourceID">The ID of the first tile</param>
        /// <param name="destinationID">The ID of the second tile</param>
        /// <param name="canConnect">Whether or not these tiles can connect</param>
        public void RegisterOneWayConnect(byte sourceID, byte destinationID, bool canConnect = true)
        {
            _connections[sourceID, destinationID] = canConnect;
        }

        /// <summary>
        /// Registers a connection between 1 tile and another. The second tile will not connect to the first
        /// </summary>
        /// <param name="tile1">The name of the first tile</param>
        /// <param name="tile2">The name of the second tile</param>
        /// <param name="canConnect">Whether or not these tiles can connect</param>
        public void RegisterOneWayConnect(string tile1, string tile2, bool canConnect = true)
        {
            tile1 = tile1.ToLower();
            tile2 = tile2.ToLower();

            if (_tiles.ContainsKey(tile1) && _tiles.ContainsKey(tile2))
            {
                _connections[_tiles[tile1], _tiles[tile2]] = canConnect;
            }
        }

        /// <summary>
        /// Generates a random tile name
        /// </summary>
        /// <returns>A random tile name</returns>
        private string GenName()
        {
            int i = 0;

            while (_tiles.Keys.Contains("tile_" + i))
                i++;

            return "tile_" + i;
        }
    }
}
