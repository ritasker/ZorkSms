// ==================================================================
// ZMachine.NET / Jason Follas  (http://zmachine.codeplex.com) 
// ==================================================================

using System;
using System.Collections.Generic;

namespace ZMachine
{
    public interface IZProcessor
    {
        IZMemory Memory { get; }
        void Stop();
        void Start();
        bool Running { get; }
        ZMachine.Common.ZIO LoadStory(byte[] storyBytes);
        //string GetOutput(string input);
        ZMachine.Common.ZStatus GetStatus();

        void Save(System.IO.BinaryWriter writer);

        Common.ZIO IO { get; }
    }
}
