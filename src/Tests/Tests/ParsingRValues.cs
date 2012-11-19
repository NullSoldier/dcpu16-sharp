using System;
using NUnit.Framework;
using dcpu16sharp;

namespace Tests.Tests
{
	[TestFixture]
	public class ParsingRValueTests
	{
		[SetUp]
		public void Setup()
		{
			cpu = new Processor (new ushort[0], new IHardware[0]);
		}

		[Test]
		public void Literals()
		{
			for (int i = -1; i < 31; i++)
			{
				ushort given = (ushort) (i + 0x21);
				ushort actual = cpu.GetValueR (given);
				Assert.AreEqual ((ushort)i, actual);
			}
		}

		[Test]
		public void Push()
		{
			cpu.mem[0x0] = getInstruction (OPCODES.SET, VALUES.PUSHPOP, (VALUES)0x20);
			cpu.ProcessNextInstruction ();

			unchecked
			{
				Assert.AreEqual (0xffff, cpu.SP);
				Assert.AreEqual ((ushort)-1, cpu.mem[0xffff]);
			}
		}

		[Test]
		public void Pop()
		{
			cpu.SP = 0xffff;
			cpu.mem[cpu.SP] = 0x20;

			cpu.mem[0x0] = getInstruction (OPCODES.SET, VALUES.A, VALUES.PUSHPOP);
			cpu.ProcessNextInstruction ();

			Assert.AreEqual (cpu.SP, 0x0);
			Assert.AreEqual (cpu.A, 0x20);
		}

		[Test]
		public void Peek()
		{
			cpu.SP = 0xffff;
			cpu.mem[cpu.SP] = 0x20;

			cpu.mem[0x0] = getInstruction (OPCODES.SET, VALUES.A, VALUES.PEEK);
			cpu.ProcessNextInstruction ();

			Assert.AreEqual (cpu.A, 0x20);
		}

		[Test]
		public void Pick()
		{
			cpu.SP = 0xfffd;
			cpu.mem[0xffff] = 0x20;

			cpu.mem[0x0] = getInstruction ( OPCODES.SET, VALUES.A, VALUES.PICK);
			cpu.mem[0x1] = 2;
			cpu.ProcessNextInstruction();

			Assert.AreEqual (cpu.A, 0x20);
			Assert.AreEqual (cpu.SP, 0xfffd);
		}

		[Test]
		public void NextWord()
		{
			cpu.mem[0x0] = getInstruction (OPCODES.SET, VALUES.A, VALUES.NEXT);
			cpu.mem[0x1] = 0x42;
			cpu.mem[0x42] = 2;
			cpu.ProcessNextInstruction ();

			Assert.AreEqual (cpu.A, 0x2);
		}

		[Test]
		public void NextWordLiteral()
		{
			cpu.mem[0x0] = getInstruction (OPCODES.SET, VALUES.A, VALUES.NEXTLIT);
			cpu.mem[0x1] = 0x4242;
			cpu.ProcessNextInstruction ();

			Assert.AreEqual (cpu.A, 0x4242);
		}

		private Processor cpu;

		private ushort getInstruction (OPCODES opcode, VALUES b, VALUES a)
		{
			int word = (ushort)opcode;
			word += (ushort)b << 5;
			word += (ushort)a << 10;

			return (ushort) word;
		}
	}
}
