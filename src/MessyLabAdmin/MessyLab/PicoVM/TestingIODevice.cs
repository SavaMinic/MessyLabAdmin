using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MessyLab.PicoComputer
{
    class TestingIODevice : IIODevice
    {

        private ushort[] givenInput;
        private List<ushort> input;
        private List<ushort> expectedOutput;
        private List<ushort> output;

        public TestingIODevice(ushort[] given, ushort[] expected)
        {
            givenInput = given;
            input = new List<ushort>(givenInput);
            expectedOutput = new List<ushort>(expected);
            output = new List<ushort>();
        }

        /// <summary>
		/// Reads integer values from the given input
		/// </summary>
		/// <param name="address">The destination address</param>
		/// <param name="count">The number of values to read</param>
		/// <param name="data">The destination Data object</param>
        public void Read(ushort address, int count, Data data)
        {
            for (int i = 0; i < count; i++)
            {
                data[address++] = ReadFromInput();
            }
        }

        /// <summary>
		/// Writes integer values to the output
		/// </summary>
		/// <param name="address">The source address</param>
		/// <param name="count">The number of values to write</param>
		/// <param name="data">The source Data object</param>
        public void Write(ushort address, int count, Data data)
        {
            for (int i = 0; i < count; i++)
            {
                WriteToOutput(data[address++]);
            }
        }

        public void Abort()
        {
            throw new NotSupportedException("Calling Abort for a TestingIODevice is not supported.");
        }

        public void Clear()
        {
            input = new List<ushort>(givenInput);
            output = new List<ushort>();
        }

        public bool IsOutputOk()
        {
            if (expectedOutput.Count != output.Count) return false;
            for(int i = 0; i < expectedOutput.Count; i++)
            {
                if (expectedOutput[i] != output[i]) return false;
            }
            return true;
        }

        private ushort ReadFromInput()
        {
            if (input.Count == 0) throw new EmptyInputRuntimeException();
            var first = input.ElementAt(0);
            input.RemoveAt(0);
            return first;
        }

        private void WriteToOutput(ushort val)
        {
            output.Add(val);
        }
    }
}
