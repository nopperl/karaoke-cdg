// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InstructionType.cs" company="">
//   Copyright (c) 2014 
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CDG
{
    /// <summary>
    /// Indicates the type of a <see cref="SubCode.Instruction"/>.
    /// </summary>
    public enum InstructionType
    {
        /// <summary>
        /// The instruction to set the screen to a particular color.
        /// </summary>
        MemoryPreset = 1,

        /// <summary>
        /// The instruction to set the border of the screen to a particular color.
        /// </summary>
        BorderPreset = 2,

        /// <summary>
        /// The instruction to load a 12 x 6, 2 color tile and display it normally.
        /// </summary>
        TitleBlock = 6,

        /// <summary>
        /// The instruction to scroll the image, filling in the new area with a color.
        /// </summary>
        ScrollPreset = 20,

        /// <summary>
        /// The instruction to scroll the image, rotating the bits back around.
        /// </summary>
        ScrollCopy = 24,

        /// <summary>
        /// The instruction to define a specific color as being transparent.
        /// </summary>
        DefineTransparentColor = 28,

        /// <summary>
        /// The instruction to load in the lower 8 entries of the color table.
        /// </summary>
        LoadColorTableLow = 30,

        /// <summary>
        /// The instruction to load in the upper 8 entries of the color table.
        /// </summary>
        LoadColorTableHigh = 31,

        /// <summary>
        /// The instruction to load a 12 x 6, 2 color tile and display it using the XOR method.
        /// </summary>
        TitleBlockXor = 38
    }
}
