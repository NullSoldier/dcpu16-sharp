using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using dcpu16sharp;

namespace Tests.Tests
{
	[TestFixture]
	public class BaseTest
	{
		public BaseTest (string testBinaryPath)
		{
			this.path = testBinaryPath;
		}

		[SetUp]
		public void Setup()
		{
			ushort[] instructions = InstructionLoader.Load (path);
			cpu = new Processor (instructions);
		}

		protected void TickCPU (int howManyTimes)
		{
			for (int i = 0; i < howManyTimes; i++)
				cpu.ProcessNextInstruction();
		}

		protected Processor cpu;
		private readonly string path;
	}
}
