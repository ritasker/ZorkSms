using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZMachine.Common;
using ZMachine.v3;

namespace ZorkSms.Core
{
    class ClockworkZProcessor : ZProcessor
    {
        public ClockworkZProcessor()
        {
        }

        public override void Save(BinaryWriter writer)
        {
            this.z_memory.Save(writer);

            var frames = GetFrames();

            writer.Write(frames.Count());

            foreach (var frame in frames)
            {
                frame.Save(writer);
            }
        }

        public static ClockworkZProcessor Load(BinaryReader reader)
        {
            var processor = new ClockworkZProcessor();
            processor.z_memory = ZMemory.Load(reader);

            int numFrames = reader.ReadInt32();

            processor.z_frame = null;
            for (int i = 0; i < numFrames; i++)
            {
                ZCallFrame frame = ZCallFrame.Load(reader);

                frame.PrevCallFrame = processor.z_frame;
                processor.z_frame = frame;
            }
            processor.Initialize();

            return processor;
        }

        private IEnumerable<ZCallFrame> GetFrames()
        {
            List<ZCallFrame> frames = new List<ZCallFrame>();

            var currentFrame = this.z_frame;

            while (currentFrame != null)
            {
                frames.Add(currentFrame);
                currentFrame = currentFrame.PrevCallFrame;
            }

            frames.Reverse();

            return frames;
        }
    }
}
