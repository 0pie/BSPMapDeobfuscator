using System;
using System.Collections.Generic;
using System.IO;

namespace IID_BSP
{
    internal class RestoreOriginalBSP
    {
        private static void Main(string[] args)
        {
            if (args == null || args.Length == 0)
            {
                Console.WriteLine("No arguments passed!");
                Console.ReadKey(true);
                return;
            }

            foreach (string path in args)
            {
                if (path[0] == '/')
                {
                    Console.WriteLine("Switches not supported in restore mode.");
                    continue;
                }

                if (!path.EndsWith("_proc.bsp"))
                {
                    Console.WriteLine($"Skipping non-processed file: {path}");
                    continue;
                }

                Console.WriteLine($"Restoring: {path}");

                BinaryReader reader = new BinaryReader(new FileStream(path, FileMode.Open));
                reader.BaseStream.Position = 104L;
                int surfOffset = reader.ReadInt32();
                int surfLength = reader.ReadInt32();

                // Read the flags to detect SURF_NOPORTAL
                List<bool> surfNopFlags = new List<bool>();
                reader.BaseStream.Position = surfOffset;
                for (int i = 0; i < surfLength / 72; i++)
                {
                    reader.BaseStream.Position += 64L;
                    int flags = reader.ReadInt32();
                    surfNopFlags.Add((flags & (int)SurfaceFlags.SURF_NOPORTAL) != 0);
                    reader.BaseStream.Position += 4L;
                }

                // Reading the lump of faces
                reader.BaseStream.Position = 316L;
                int faceLength = reader.ReadInt32();
                reader.BaseStream.Position -= 8L;
                long faceOffset = reader.ReadInt32();

                Dictionary<long, bool> patchPositions = new Dictionary<long, bool>();

                reader.BaseStream.Position = faceOffset;
                for (int i = 0; i < faceLength / 8; i++)
                {
                    reader.BaseStream.Position += 2L;
                    long position = reader.BaseStream.Position;
                    short index = reader.ReadInt16();
                    bool isMarked = index < 0;
                    patchPositions[position - 2] = isMarked;
                    reader.ReadInt32();
                }
                reader.Close();

                // Restore indexes to 0 for marked surfaces
                string restoredPath = path.Replace("_proc.bsp", "_restored.bsp");
                File.Copy(path, restoredPath, true);

                BinaryWriter writer = new BinaryWriter(new FileStream(restoredPath, FileMode.Open));
                foreach (var entry in patchPositions)
                {
                    if (entry.Value)
                    {
                        writer.BaseStream.Position = entry.Key;
                        writer.Write((short)0);
                    }
                }
                writer.Close();

                Console.WriteLine($"Restored file saved as: {restoredPath}");
            }
        }

        [Flags]
        private enum SurfaceFlags
        {
            SURF_NOPORTAL = 0x00002000
        }
    }
}
