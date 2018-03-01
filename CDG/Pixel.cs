// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Pixel.cs">
//   Copyright (c) 2014 
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CDG
{
    using System;
    using System.Drawing;

    /// <summary>
    /// Represents a pixel after its location and color.
    /// </summary>
    public class Pixel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Pixel"/> class with two <see cref="int"/> indicating the location.
        /// </summary>
        /// <param name="x">
        /// A value indicating the X-coordinate of the location of the <see cref="Pixel"/>.
        /// </param>
        /// <param name="y">
        /// A value indicating the Y-coordinate of the location of the <see cref="Pixel"/>.
        /// </param>
        /// <param name="color">
        /// A <see cref="Color"/> indicating the color of the <see cref="Pixel"/>.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="x"/> and/or <paramref name="y"/> is/are less than 0.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="color"/> was passed as NULL.
        /// </exception>
        public Pixel(int x, int y, Color color)
        {
            if (x < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(x), x, "x must be greater than or equals 0.");
            }

            if (y < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(y), y, "y must be greater than or equals 0.");
            }

            if (color == null)
            {
                throw new ArgumentNullException(nameof(color));
            }

            this.Location = new Point(x, y);
            this.Color = color;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Pixel"/> class with a <see cref="Point"/> indicating the location.
        /// </summary>
        /// <param name="location">
        /// A <see cref="Point"/> indicating the location of the <see cref="Pixel"/>.
        /// </param>
        /// <param name="color">
        /// A <see cref="Color"/> indicating the color of the <see cref="Pixel"/>.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="location"/> and/or <paramref name="color"/> was/were passed as NULL.
        /// </exception>
        public Pixel(Point location, Color color)
        {
            if (location == null)
            {
                throw new ArgumentNullException(nameof(location));
            }

            if (color == null)
            {
                throw new ArgumentNullException(nameof(color));
            }

            this.Location = location;
            this.Color = color;
        }

        /// <summary>
        /// Gets a <see cref="Point"/> indicating the location of this <see cref="Pixel"/>.
        /// </summary>
        public Point Location { get; private set; }

        /// <summary>
        /// A <see cref="Color"/> indicating the color of the <see cref="Pixel"/>.
        /// </summary>
        public Color Color { get; private set; }
    }
}
