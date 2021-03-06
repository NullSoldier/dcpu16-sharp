﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using dcpu16sharp;

namespace Tests.Tests
{
	[TestFixture]
	public class BaseLoadedTest
	{
		public BaseLoadedTest (string testBinaryPath)
		{
			this.path = Path.Combine (binaryRoot, testBinaryPath);
		}

		[SetUp]
		public void Setup()
		{
			ushort[] instructions = InstructionLoader.Load (path);
			cpu = new Processor (instructions, new IHardware[0]);
		}

		protected void TickCPU (int howManyTimes)
		{
			for (int i = 0; i < howManyTimes; i++)
				cpu.ProcessNextInstruction();
		}

		protected Processor cpu;
		private readonly string path;
		private const string binaryRoot = "Testbinaries";
	}
}
