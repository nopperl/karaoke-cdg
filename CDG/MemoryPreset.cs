// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MemoryPreset.cs">
//   Copyright (c) 2014 
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CDG
{
    using System.Collections.Generic;
    using System.Drawing;

    public class MemoryPreset
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MemoryPreset"/> class.
        /// </summary>
        /// <param name="data">
        /// A <see>
        ///         <cref>byte[]</cref>
        ///     </see>
        ///     holding the color data.
        /// </param>
        /// <param name="colorTable">
        /// A <see cref="List{Color}"/> used as color-table for the <see cref="CdgFile"/>.
        /// </param>
        public MemoryPreset(byte[] data, List<Color> colorTable)
        {
            int colorIndex = data[4] & 0x0F;
            this.Color = colorTable[colorIndex];
            this.Repeat = data[5] & 0x0F;
        }

        /// <summary>
        /// Gets the <see cref="Color"/> of the <see cref="MemoryPreset"/>.
        /// </summary>
        public Color Color { get; private set; }

        /// <summary>
        /// Gets the count of repeat of this <see cref="MemoryPreset"/>.
        /// </summary>
        public int Repeat { get; private set; }
    }
}
