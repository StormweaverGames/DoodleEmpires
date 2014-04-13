﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace DoodleEmpires.Engine.Utilities
{
    public class TextureAtlas
    {
        protected Texture2D _texture;
        protected Rectangle[] _sources;
        protected int _xTexs;
        protected int _yTexs;
        private int _maxID;

        /// <summary>
        /// Gets the texture that this atlas is bound to
        /// </summary>
        public Texture2D Texture
        {
            get { return _texture; }
        }

        /// <summary>
        /// Creates a new texture atlas
        /// </summary>
        /// <param name="texture">The texture to bind to</param>
        /// <param name="xTexs">The number of textures along the horizontal axis</param>
        /// <param name="yTexs">The number of textures along the vertical axis</param>
        public TextureAtlas(Texture2D texture, int xTexs, int yTexs)
        {
            if (texture.Width % xTexs != 0 | texture.Height % yTexs != 0)
                throw new ArgumentException("The texture size must divide evenly with xTexs and yTexs!");

            _texture = texture;
            _xTexs = xTexs;
            _yTexs = yTexs;
            _maxID = _xTexs * _yTexs;

            BuildSources();
        }

        /// <summary>
        /// Builds the source rectangles
        /// </summary>
        protected virtual void BuildSources()
        {
            _sources = new Rectangle[_maxID];
            int texSizeX = _texture.Width / _xTexs;
            int texSizeY = _texture.Height / _yTexs;

            for (int y = 0; y < _yTexs; y++)
            {
                for (int x = 0; x < _xTexs; x++)
                {
                    _sources[y * _xTexs + x] = new Rectangle(x * texSizeX, y * texSizeY, texSizeX, texSizeY);
                }
            }
        }

        /// <summary>
        /// Gets a source rectangle for the given texture ID
        /// </summary>
        /// <param name="ID">The texture ID to search for</param>
        /// <returns>The source rectangle for the given ID</returns>
        public Rectangle GetSource(int ID)
        {
            if (ID >= 0 & ID < _maxID)
                return _sources[ID];
            else
                throw new IndexOutOfRangeException("The ID must be between 0 and " + _maxID);
        }

        /// <summary>
        /// Gets a source rectangle for the given texture ID
        /// </summary>
        /// <param name="ID">The texture ID to search for</param>
        /// <returns>The source rectangle for the given ID</returns>
        public Rectangle this[int ID]
        {
            get { return GetSource(ID); }
        }
    }
}