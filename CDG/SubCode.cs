// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SubCode.cs">
//   Copyright (c) 2014 
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CDG
{
    using System.Collections.Generic;

    /// <summary>
    /// Represents a packet read from the CD+G.
    /// </summary>
    public class SubCode
    {
        private const byte Mask = 0x3F;

        /// <summary>
        /// Initializes a new instance of the <see cref="SubCode"/> class via a <see>
        ///         <cref>byte[]</cref>
        ///     </see>
        ///     holding all data.
        /// </summary>
        public SubCode(IReadOnlyList<byte> data)
        {
            this.Command = data[0];
            this.Instruction = (InstructionType)(data[1] & SubCode.Mask);
            this.ParityQ = new List<byte>(2) { data[2], data[3] };
            this.Data = new List<byte>(16);
            for (int i = 4; i < 20; i++)
            {
                this.Data.Add(data[i]);
            }

            this.ParityP = new List<byte>(4);
            for (int i = 20; i < data.Count; i++)
            {
                this.ParityP.Add(data[i]);
            }
        }

        /// <summary>
        /// Gets the command in the <see cref="SubCode"/>.
        /// </summary>
        public byte Command { get; private set; }

        /// <summary>
        /// Gets the instruction in the <see cref="SubCode"/>.
        /// </summary>
        public InstructionType Instruction { get; private set; }

        public List<byte> ParityQ { get; private set; }

        /// <summary>
        /// Gets the whole data found in the <see cref="SubCode"/>.
        /// </summary>
        public List<byte> Data { get; private set; }

        public List<byte> ParityP { get; private set; }
    }
}
