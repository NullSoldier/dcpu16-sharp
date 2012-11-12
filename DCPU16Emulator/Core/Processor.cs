using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DCPU16Emulator.Core
{
	// Processor
	// - Load instructions into the processor at offset 0
	// - Tell it to run (set PC counter to offset 0) and start

	//start at instruction 0
	//For each instruction
	//	[PC++]
	//	Parse a (upper 6 bits)
	//	Parse b (next 5 bits)
	//	Parse o (lower 5 bits)

	public class Processor
	{
		public Processor()
		{
			clearMemory ();
		}

		public void Run(ushort[] instructions)
		{
			instructions.CopyTo (mem, 0);

			while (true)
			{
				ushort nextWord = getNextWord ();

				ushort opcode = (ushort)(nextWord & 0x1f);
				Value a = getValue (true, (ushort) ((nextWord & 0xfc00) >> 10));
				Value b = getValue (false, (ushort) (nextWord & 0x3e0 >> 5));

				switch (opcode)
				{
					case 0x01: // SET
						setValue (b, a);
						break;
					default:
						throw new NotImplementedException ();
				}
			}
		}

		private Dictionary<Registers, ushort> registers;
		private ushort[] mem;
		private ushort sp;
		private ushort pc;
		private ushort ex;

		private ushort getNextWord()
		{
			return mem[pc++];
		}

		private Value getValue(bool isA, ushort value)
		{
			if (value >= 0x00 && value <= 0x07) // register
			{
				var register = (Registers) value;
				return new Value (Values.Register, register);
			}
			else if (value >= 0x08 && value <= 0x0f) // [register]
			{
				var register = (Registers) value;
				return new Value (Values.PointerValue, registers[register]);
			}
			else if (value >= 0x10 && value <= 0x17) // [register + next word]
			{
				var register = (Registers) value;
				var addedValue = (ushort) (registers[register] + getNextWord());

				return new Value (Values.PointerValue, addedValue);
			}
			else if (value >= 0x20 && value <= 0x3f) // literals
			{
				ushort literal = (ushort) (value & 31 - 1);
				return new Value (Values.Literal, literal);
			}

			switch (value)
			{
				case 0x18: // if A, PUSH. if B, POP
					return isA ? new Value (Values.PointerValue, pop ())
						: new Value (Values.PointerValue, push());

				case 0x19: // PEEK
					return new Value (Values.PointerValue, peek());

				case 0x1a: // [SP + next word]
					ushort stackValue = mem[sp];
					Value nextValue = getValue (isA, getNextWord());
					ushort combinedValue = (ushort) (stackValue + nextValue.ActualValue);
					return new Value (Values.PointerValue, combinedValue);
					
				case 0x1b: // SP
					return new Value (Values.SP);

				case 0x1c: // PC
					return new Value (Values.PC);

				case 0x1d: // EX
					return new Value (Values.EX);

				case 0x1e: // [next word]
					return getValue (isA, getNextWord());

				case 0x1f: // next word (literal)
					return new Value (Values.PointerValue, getNextWord ());

				default:
					throw new NotSupportedException ("Instruction is not supported");
			}
		}

		private void setValue (Value dest, Value src)
		{
			switch (dest.TypeOfValue)
			{
				case Values.SP:
					sp = src.ActualValue;
					break;
				case Values.PC:
					pc = src.ActualValue;
					break;
				case Values.Register:
					registers[dest.Register] = src.ActualValue;
					break;
				case Values.PointerValue:
				case Values.Literal:
					// Setting literal fails silently
					break;
				default:
					throw new NotSupportedException ("Unsupported type to set");
			}
		}

		private void clearMemory()
		{
			mem = new ushort[ushort.MaxValue];
			sp = 0;
			pc = 0;

			registers = new Dictionary<Registers, ushort> {
				{Registers.A, 0},
				{Registers.B, 0},
				{Registers.C, 0},
				{Registers.I, 0},
				{Registers.J, 0},
				{Registers.X, 0},
				{Registers.Y, 0},
				{Registers.Z, 0}
			};
		}

		private ushort peek()
		{
			return mem[sp];
		}

		private ushort pop()
		{
			return mem [sp++];
		}

		private ushort push()
		{
			return mem [--sp];
		}
	}
}
