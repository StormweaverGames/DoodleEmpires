﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace DoodleAnims
{
    /// <summary>
    /// A panel that uses double buffering
    /// </summary>
    class DoubleBufferedPanel : Panel
    {
        /// <summary>
        /// Creates a new double-buffered panel
        /// </summary>
        public DoubleBufferedPanel()
            : base()
        {
            this.DoubleBuffered = true;
        }
    }
}
