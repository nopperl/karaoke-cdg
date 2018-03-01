// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CdgFile.cs">
//   Copyright (c) 2014 
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CDG
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.IO;
    using System.Linq;

    using Splicer.Renderer;
    using Splicer.Timeline;
    using Splicer.WindowsMedia;

    /// <summary>
    /// Represents a CD+G-File.
    /// </summary>
    public class CdgFile : IDisposable
    {
        private const byte Mask = 0x3F;

        private const byte Command = 0x09;

        private const int FramesPerSecond = 20;

        private const int PacketsPerFrame = 300 / FramesPerSecond;

        ////private const int FrameInterval = 1000;
        private const int PacketSize = 24;

        private const int FullWidth = 300;

        private const int FullHeight = 216;

        private const int InnerWidth = 294;

        private const int InnerHeight = 204;

        ////private const int ColorTableSize = 16;

        private Bitmap image;

        private FileStream stream;

        ////private long position;
        private List<Color> colorTable;

        private List<Pixel> newPixels;

        private readonly Rectangle fullFrame;

        private readonly Rectangle innerFrame;

        /// <summary>
        /// Initializes a new instance of the <see cref="CdgFile"/> class with a <see cref="string"/> containing the path to the file.
        /// </summary>
        /// <param name="fileName">
        /// A <see cref="string"/> representing the full path to the CD+G file.
        /// </param>
        /// <exception cref="FileNotFoundException">
        /// The file in the passed path does not exist.
        /// </exception>
        public CdgFile(string fileName)
        {
            if (!File.Exists(fileName))
            {
                throw new FileNotFoundException("The file in the passed path does not exist.", fileName);
            }

            this.image = new Bitmap(FullWidth, FullHeight);
            this.stream = new FileStream(fileName, FileMode.Open);
            this.colorTable = new List<Color>();
            this.newPixels = new List<Pixel>(FullWidth * FullHeight);
            this.fullFrame = new Rectangle(0, 0, FullWidth, FullHeight);
            this.innerFrame = new Rectangle(6, 12, InnerWidth - 6, InnerHeight - 12);
        }

        /// <summary>
        /// Gets the current <see cref="Bitmap"/> of the <see cref="CdgFile"/>. Gets updated in <see cref="Next()"/>.
        /// </summary>
        public Bitmap CurrentFrame => this.image;

        /// <inheritdoc />
        public void Dispose()
        {
            this.stream.Dispose();
            this.image.Dispose();
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Reads the next frame of <see cref="SubCode"/> and processes them.
        /// </summary>
        /// <returns>
        /// The processed <see cref="Bitmap"/> or NULL, if the <see cref="CurrentFrame"/> is the last of this <see cref="CdgFile"/>.
        /// </returns>
        public Bitmap Next()
        {
            SubCode subCode = null;
            try
            {
                if (this.ReadSubCode(ref subCode))
                {
                    this.InterpreteSubCode(subCode);
                    this.UpdateImage();
                }
                else
                {
                    return null;
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
            }

            return this.CurrentFrame;
        }

        /// <summary>
        /// Reads the  <see cref="SubCode"/> of the given position and processes them.
        /// </summary>
        /// <param name="position">
        /// The position where the <see cref="SubCode"/> should be loaded.
        /// </param>
        /// <returns>
        /// A value indicating whether the operation was successful.
        /// </returns>
        public bool Read(long position)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Saves the content of this <see cref="CdgFile"/> as video.
        /// </summary>
        /// <param name="path">
        /// A <see cref="string"/> indicating the file to which the video should be saved.
        /// </param>
        /// <returns>
        /// The Task.
        /// </returns>
        public void SaveAsVideo(string path)
        {
            Console.WriteLine("Saving...");
            using (ITimeline timeline = new DefaultTimeline())
            {
                IGroup group = timeline.AddVideoGroup(32, 300, 300);
                ITrack videoTrack = group.AddTrack();
                Bitmap frame = this.Next();
                int frames = 0;
                while (frame != null && frames < 100)
                {
                    Console.WriteLine("<!--Getting frame-->");
                    videoTrack.AddImage(frame, 0, 0.2D);
                    frame = this.Next();
                    frames++;
                }

                try
                {
                    Console.WriteLine(videoTrack.Duration);
                    Console.WriteLine(this.stream.Name.Replace("cdg", "mp3"));
                    ITrack audioTrack = timeline.AddAudioGroup().AddTrack();
                    ////IClip audio = audioTrack.AddAudio(this.stream.Name.Replace("cdg", "mp3"), 0, videoTrack.Duration);
                    IRenderer renderer = new WindowsMediaRenderer(timeline, path, WindowsMediaProfiles.HighQualityVideo);
                    renderer.Render();
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception);
                }
            }

            Console.WriteLine("Saving finished.");
        }

        private void BorderPreset(SubCode subCode)
        {
            if (this.colorTable.Count > 0)
            {
                MemoryPreset memoryPreset = new MemoryPreset(subCode.Data.ToArray(), this.colorTable);
                Console.WriteLine("Clearing border to: {0}", memoryPreset.Color);
                int count = 0;
                for (int x = 0; x < FullWidth; x++)
                {
                    for (int y = 0; y < FullHeight; y++)
                    {
                        Pixel pixel = new Pixel(new Point(x, y), memoryPreset.Color);
                        if (this.innerFrame.Contains(pixel.Location))
                        {
                            continue;
                        }

                        this.newPixels.Add(pixel);
                        count++;
                    }
                }
            }
        }

        /// <summary>
        /// Interprets the given <see cref="SubCode"/> and starts the operations associated with the interpretation.
        /// </summary>
        /// <param name="subCode">
        /// A <see cref="SubCode"/> to be interpreted.
        /// </param>
        private void InterpreteSubCode(SubCode subCode)
        {
            if ((subCode.Command & Mask) == Command)
            {
                try
                {
                    switch (subCode.Instruction)
                    {
                        case InstructionType.MemoryPreset:
                            this.MemoryPreset(subCode);
                            break;
                        case InstructionType.BorderPreset:
                            this.BorderPreset(subCode);
                            break;
                        case InstructionType.TitleBlock:
                            this.TileBlock(subCode, false);
                            break;
                        case InstructionType.ScrollPreset:
                            this.Scroll(subCode, true);
                            break;
                        case InstructionType.ScrollCopy:
                            this.Scroll(subCode, false);
                            break;
                        case InstructionType.DefineTransparentColor:
                            break;
                        case InstructionType.LoadColorTableLow:
                            this.LoadColorTable(subCode, ColorTableType.Low);
                            break;
                        case InstructionType.LoadColorTableHigh:
                            this.LoadColorTable(subCode, ColorTableType.High);
                            break;
                        case InstructionType.TitleBlockXor:
                            this.TileBlock(subCode, true);
                            break;
                    }
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception);
                }
            }
        }

        private void LoadColorTable(SubCode subCode, ColorTableType tableType)
        {
            Console.WriteLine("Loading {0} colors", tableType);
            List<byte> colors = new List<byte>();
            switch (tableType)
            {
                case ColorTableType.High:
                    foreach (byte color in subCode.Data)
                    {
                        colors.Add((byte)(color & 0x3F3F));
                        if (colors.Count == 8)
                        {
                            break;
                        }
                    }

                    for (int i = 0; i < colors.Count; i++)
                    {
                        byte color = colors[i];
                        int red = (color & 0x3F3F) >> 2;
                        int halfGreen = (color & 0x3) << 2;
                        if (this.colorTable.Count < i)
                        {
                            this.colorTable.Add(Color.FromArgb(red, halfGreen, 0));
                        }
                        else
                        {
                            int preRed = this.colorTable[i].R;
                            int preGreen = this.colorTable[i].G;
                            this.colorTable[i] = Color.FromArgb(
                                preRed += (byte)red,
                                preGreen += (byte)halfGreen,
                                this.colorTable[i].B);
                        }
                    }
                    break;
                case ColorTableType.Low:
                    subCode.Data.Reverse();
                    foreach (byte color in subCode.Data)
                    {
                        colors.Add(color);
                        if (colors.Count == 8)
                        {
                            break;
                        }
                    }

                    subCode.Data.Reverse();
                    for (int i = 0; i < colors.Count; i++)
                    {
                        byte color = colors[i];
                        int halfGreen = (color & 0x3) >> 4;
                        int blue = (color & 0x3F3F) << 2;
                        if (this.colorTable.Count <= i)
                        {
                            this.colorTable.Add(Color.FromArgb(0, halfGreen, blue));
                        }
                        else
                        {
                            int preGreen = this.colorTable[i].G;
                            int preBlue = this.colorTable[i].B;
                            if (preBlue + blue > 255)
                            {
                                int temp = (blue + preBlue) - 255;
                                preGreen += temp;
                                preBlue -= temp;
                            }
                            this.colorTable[i] = Color.FromArgb(
                                this.colorTable[i].R,
                                preGreen += (byte)halfGreen,
                                preBlue += (byte)blue);
                        }
                    }
                    break;
            }
        }

        private void MemoryPreset(SubCode subCode)
        {
            if (this.colorTable.Count <= 0)
            {
                return;
            }

            MemoryPreset memoryPreset = new MemoryPreset(subCode.Data.ToArray(), this.colorTable);

            if (memoryPreset.Repeat != 0)
            {
                return;
            }

            Console.WriteLine("Clearing screen to: {0}", memoryPreset.Color);
            int count = 0;
            for (int x = 0; x < FullWidth; x++)
            {
                for (int y = 0; y < FullHeight; y++)
                {
                    if (this.newPixels.Count <= count)
                    {
                        this.newPixels.Add(new Pixel(new Point(x, y), memoryPreset.Color));
                    }
                    else
                    {
                        this.newPixels[count] = new Pixel(new Point(x, y), memoryPreset.Color);
                    }

                    count++;
                }
            }
        }

        /// <summary>
        /// Reads the next <see cref="SubCode"/> in the <see cref="FileStream"/>.
        /// </summary>
        /// <param name="subCode">
        /// A <see cref="SubCode"/> used to store the read information.
        /// </param>
        /// <returns>
        /// A value indicating if the reading operation was successful.
        /// </returns>
        private bool ReadSubCode(ref SubCode subCode)
        {
            if (this.stream == null || this.stream.Position >= this.stream.Length)
            {
                return false;
            }

            byte[] data = new byte[24];
            if (this.stream.Read(data, 0, PacketSize) != PacketSize)
            {
                return false;
            }

            subCode = new SubCode(data);
            return true;
        }

        private void Scroll(SubCode subCode, bool preset)
        {
            Scroll scroll = new Scroll(subCode.Data.ToArray(), this.colorTable, preset);
            Console.WriteLine(
                "Scrolling color {0} to right {1} to bottom {2}",
                scroll.Color,
                scroll.PixelsToRight,
                scroll.PixelsToBottom);
        }

        private void TileBlock(SubCode subCode, bool xor)
        {
            Tile tile = new Tile(subCode.Data.ToArray(), this.colorTable);
            if (xor)
            {
                for (int i = 0; i < tile.TilePixels.Count; i++)
                {
                    //TODO: Read pixels from this.image
                    foreach (var pixel in this.newPixels.Where(p => p.Equals(tile.TilePixels[i])))
                    {
                        int index = this.colorTable.IndexOf(pixel.Color);
                        int newIndex = i ^ index;
                        this.newPixels[this.newPixels.IndexOf(pixel)] = new Pixel(
                            pixel.Location,
                            this.colorTable[newIndex]);
                    }
                }
            }
            else
            {
                this.newPixels.AddRange(tile.TilePixels);
            }
        }

        private void UpdateImage()
        {
            foreach (var pixel in this.newPixels)
            {
                if (this.image.GetPixel(pixel.Location.X, pixel.Location.Y) != pixel.Color)
                {
                    this.image.SetPixel(pixel.Location.X, pixel.Location.Y, pixel.Color);
                }
            }
        }
    }

    internal enum ColorTableType
    {
        Low,

        High
    }
}
