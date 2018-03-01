// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Tile.cs">
//   Copyright (c) 2014 
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CDG
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Drawing;

    public class Tile
    {
        private const int TileHeight = 12;

        private const int TileWidth = 6;

        public Tile(IReadOnlyList<byte> data, IReadOnlyList<Color> colorTable)
        {
            int color0 = data[0] & 0x0F;
            int color1 = data[1] & 0x0F;
            int row = data[2] & 0x1F;
            int column = data[3] & 0x3F;
            this.Row = row * 12;
            this.Column = column * 6;
            this.TilePixels = new List<Pixel>();
            List<BitArray> tilePixels = new List<BitArray>();
            for (int i = 4; i < data.Count; i++)
            {
                byte newByte = (byte)(data[i] & 0x3F);
                tilePixels.Add(new BitArray(new[] { newByte }));
            }

            for (int rowIndex = 0; rowIndex < tilePixels.Count; rowIndex++)
            {
                for (int columnIndex = 0; columnIndex < tilePixels[rowIndex].Length; columnIndex++)
                {
                    this.TilePixels.Add(
                        tilePixels[rowIndex][columnIndex]
                            ? new Pixel(new Point(this.Column + columnIndex, this.Row + rowIndex), colorTable[color1])
                            : new Pixel(new Point(this.Column + columnIndex, this.Row + rowIndex), colorTable[color0]));
                }
            }
        }

        public int Row { get; }

        public int Column { get; }

        public List<Pixel> TilePixels { get; }
    }
}
