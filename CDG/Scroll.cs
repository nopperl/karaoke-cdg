// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Scroll.cs">
//   Copyright (c) 2014 
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CDG
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;

    public class Scroll
    {
        private const int MaxHorizontal = 6;

        private const int MaxVertical = 12;

        public Scroll(IReadOnlyList<byte> data, IReadOnlyList<Color> colorTable, bool preset)
        {
            Console.WriteLine("Scrolling");
            if (preset)
            {
                int color = data[0] & 0x0F;
                this.Color = colorTable[color];
            }
            else
            {
                this.Color = Color.Empty;
            }

            int horizontalScroll = data[1] & 0x3F;
            int verticalScroll = data[2] & 0x3F;

            int specialHorizontalCommand = ((byte)horizontalScroll & 0x30) >> 4;
            int horizontalOffset = (byte)horizontalScroll & 0x07;
            switch (specialHorizontalCommand)
            {
                case 0:
                    this.PixelsToRight = horizontalOffset;
                    break;
                case 1:
                    this.PixelsToRight = Scroll.MaxHorizontal;
                    break;
                case 2:
                    this.PixelsToRight = -Scroll.MaxHorizontal;
                    break;
            }

            int specialVerticalCommand = ((byte)verticalScroll & 0x30) >> 4;
            int verticalOffset = (byte)verticalScroll & 0x0F;
            switch (specialVerticalCommand)
            {
                case 0:
                    this.PixelsToBottom = verticalOffset;
                    break;
                case 1:
                    this.PixelsToBottom = Scroll.MaxVertical;
                    break;
                case 2:
                    this.PixelsToBottom = -Scroll.MaxVertical;
                    break;
            }
        }

        /// <summary>
        /// Gets the <see cref="Color"/> of this <see cref="Scroll"/> or <see cref="System.Drawing.Color.Empty"/>, if this <see cref="Scroll"/> represents a <see cref="InstructionType.ScrollCopy"/> operation.
        /// </summary>
        public Color Color { get; private set; }

        public int PixelsToRight { get; private set; }

        public int PixelsToBottom { get; private set; }
    }
}
