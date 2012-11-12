using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DCPU16Emulator.Core;

namespace DCPU16Emulator
{
	public class Program
	{
		public static void Main (string[] args)
		{
			string samplePath = "Samples/ParseTest.bin";
			ushort[] instructions;

			using (FileStream stream  = File.Open (samplePath, FileMode.Open))
			{
				byte[] buffer = new byte [stream.Length];
				stream.Read (buffer, 0, buffer.Length);

				instructions = new ushort [buffer.Length / sizeof(ushort)];
				Buffer.BlockCopy (buffer, 0, instructions, 0, buffer.Length);
			}

			var cpu = new Processor (instructions);
			cpu.Run();
		}
	}
}