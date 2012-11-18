using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests
{
	public class InstructionLoader
	{
		/// <summary>
		/// Loads a binary as DCPU-16 instructions
		/// </summary>
		/// <param name="pathToBinary">The relative or absolute path to the file
		/// which stores binary DCPU-16 instructions. If relative, the path is
		/// relative to the application's current directory</param>
		/// <returns>An array of 16 bit words which represent binary instructions</returns>
		public static ushort[] Load (string pathToBinary)
		{
			byte[] buffer = File.ReadAllBytes (pathToBinary);

			ushort[] instructions = new ushort[buffer.Length / sizeof (ushort)];
			Buffer.BlockCopy (buffer, 0, instructions, 0, buffer.Length);

			return instructions;
		}
	}
}
