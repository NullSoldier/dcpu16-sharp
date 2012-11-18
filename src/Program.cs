using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dcpu16sharp
{
	public class Program
	{
		public static void Main (string[] args)
		{
			string samplePath = "Samples/ParseTest.bin";
			byte[] buffer = File.ReadAllBytes (samplePath);

			ushort[] instructions = new ushort [buffer.Length / sizeof(ushort)];
			Buffer.BlockCopy (buffer, 0, instructions, 0, buffer.Length);

			var cpu = new Processor (instructions);
			cpu.Run();
		}
	}
}