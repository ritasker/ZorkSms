// ==================================================================
// ZMachine.NET / Jason Follas  (http://zmachine.codeplex.com) 
// ==================================================================

using System;
using System.Collections.Generic;
using System.IO;

namespace ZMachine.Common 
{
    /// <summary>
    /// ZCallFrame contains the current state information
    /// </summary>
    [Serializable]
    public class ZCallFrame
    {
        // Provide a link to the previous call frame, which is restored after the current routine returns
        private ZCallFrame m_prevCallFrame = null;

        /// <summary>
        /// Constructor.  Initializes the program counter to the value provided.
        /// </summary>
        /// <param name="programCounter">Program Counter</param>
        public ZCallFrame(int programCounter)
        {
            this.ProgramCounter = programCounter;
        }

        /// <summary>
        /// Local variables array.  Note that this is used during CALL persistence only, and that the active list
        /// of local variables is found in ZMemory.
        /// </summary>
        public short[] Locals { get; set; }

        /// <summary>
        /// Program Counter
        /// </summary>
        public int ProgramCounter { get; set; }

        /// <summary>
        /// Keeps track of the variable where the result is stored after a routine returns (and this call frame is restored).  
        /// 00H = Stack; 01H-0FH = Locals; 10H-FFH = Globals
        /// </summary>
        public byte? Result { get; set; }

        /// <summary>
        /// Reference to the previous call frame that was in place when this call frame was created.
        /// </summary>
        public ZCallFrame PrevCallFrame
        {
            get { return m_prevCallFrame; }
            set { m_prevCallFrame = value; }
        }

        public int ArgCount { get; set; }
        public int FrameNumber { get; set; }

        public ZCallFrame(int programCounter, int argCount, int frameNum, byte? result, short[] locals){
            ProgramCounter = programCounter;
            ArgCount = argCount;
            FrameNumber = frameNum;
            Result = result;
            Locals = locals;
        }

        public void Save(System.IO.BinaryWriter writer)
        {
            writer.Write(ProgramCounter);
            writer.Write(ArgCount);
            writer.Write(FrameNumber);
            writer.Write(Result.HasValue);
            if (Result.HasValue) writer.Write(Result.Value);

            writer.Write(Locals.Length);

            for (int i = 0; i < Locals.Length; i++)
            {
                writer.Write(Locals[i]);
            }
        }

        public static ZCallFrame Load(BinaryReader reader)
        {
            int programCounter = reader.ReadInt32();
            int argCount = reader.ReadInt32();
            int frameNum = reader.ReadInt32();
            bool hasResult = reader.ReadBoolean();

            byte? result = null;

            if (hasResult) result = reader.ReadByte();

            int localSize = reader.ReadInt32();

            short[] locals = new short[localSize];
            for (int i = 0; i < localSize; i++)
            {
                locals[i] = reader.ReadInt16();
            }

            return new ZCallFrame(programCounter, argCount, frameNum, result, locals);
        }
    }
}
