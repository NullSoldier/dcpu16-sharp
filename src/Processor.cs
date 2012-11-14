using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DCPU16Emulator
{
	public class Processor
	{
		public Processor(ushort[] instructions)
		{
			instructions.CopyTo (mem, 0);
		}

		public void Run()
		{
			while (true)
			{
				ushort nextWord = getNextWord();

				ushort opcode = (ushort)(nextWord & 0x1f);
				ushort b = (ushort) ((nextWord & 0x3e0) >> 5);
				ushort a = (ushort) ((nextWord & 0xfc00) >> 10);

				switch ((OPCODES)opcode)
				{
					case OPCODES.SET:
						setValue (b, getValueR (a));
						break;
					case OPCODES.ADD:
						setValue (b, (ushort) (getValueL (b) + getValueR (a)));
						break;
					case OPCODES.SUB:
						setValue (b, (ushort) (getValueL (b) - getValueR (a)));
						break;
					default:
						throw new NotImplementedException ();
				}
			}
		}

		private ushort[] mem = new ushort[ushort.MaxValue];
		private ushort PC, SP, EX, A, B, C, X, Y, Z, I, J;

		private ushort getNextWord()
		{
			return mem[PC++];
		}

		private ushort getValueL (ushort bits)
		{
			return getValue (bits, false);
		}

		private ushort getValueR (ushort bits)
		{
			return getValue (bits, true);
		}

		private ushort getValue (ushort bits, bool bitsAreA)
		{
			// Literals (-1..30)
			if (bits >= 0x20 && bits <= 0x3f)
				return (ushort)(bits - 0x21);

			// Turn next word registers
			ushort nextWord = 0;
			if (bits >= 0x10 && bits <= 0x17)
			{
				nextWord = getNextWord();
				bits /= 2;
			}

			switch ((VALUES)bits)
			{
				//register
				case VALUES.A: return A;
				case VALUES.B: return B;
				case VALUES.C: return C;
				case VALUES.X: return X;
				case VALUES.Y: return Y;
				case VALUES.Z: return Z;
				case VALUES.I: return I;
				case VALUES.J: return J;

				// [register] and [register + nextWord]
				case VALUES.REG_A: return mem[A + nextWord];
				case VALUES.REG_B: return mem[B + nextWord];
				case VALUES.REG_C: return mem[C + nextWord];
				case VALUES.REG_X: return mem[X + nextWord];
				case VALUES.REG_Y: return mem[Y + nextWord];
				case VALUES.REG_Z: return mem[Z + nextWord];
				case VALUES.REG_I: return mem[I + nextWord];
				case VALUES.REG_J: return mem[J + nextWord];

				case VALUES.SP: return SP;
				case VALUES.PC: return PC;
				case VALUES.EX: return EX;
				case VALUES.PUSHPOP: return bitsAreA ? mem[SP++] : mem[--SP];
				case VALUES.PEEK: return mem[SP];
				case VALUES.PICK: return mem[SP + getNextWord ()];
				case VALUES.NEXT: return mem[getNextWord ()];
				case VALUES.NEXTLIT: return getNextWord ();

				default: throw new NotSupportedException ("Value " + bits + " is not supported");
			}
		}

		private void setValue (ushort dest, ushort x)
		{
			switch (dest)
			{
				case 0x00: A = x; return;
				case 0x01: B = x; return;
				case 0x02: C = x; return;
				case 0x03: X = x; return;
				case 0x04: Y = x; return;
				case 0x05: Z = x; return;
				case 0x06: I = x; return;
				case 0x07: J = x; return;

				// [register]
				case 0x08: mem[A] = x; return;
				case 0x09: mem[B] = x; return;
				case 0x0a: mem[C] = x; return;
				case 0x0b: mem[X] = x; return;
				case 0x0c: mem[Y] = x; return;
				case 0x0d: mem[Z] = x; return;
				case 0x0e: mem[I] = x; return;
				case 0x0f: mem[J] = x; return;

				// [register + next word]
				case 0x10: mem[A + getNextWord ()] = x; return;
				case 0x11: mem[B + getNextWord ()] = x; return;
				case 0x12: mem[C + getNextWord ()] = x; return;
				case 0x13: mem[X + getNextWord ()] = x; return;
				case 0x14: mem[Y + getNextWord ()] = x; return;
				case 0x15: mem[Z + getNextWord ()] = x; return;
				case 0x16: mem[I + getNextWord ()] = x; return;
				case 0x17: mem[J + getNextWord ()] = x; return;

				case 0x18: throw new NotImplementedException(); // PUSH/POP
				case 0x19: mem[SP] = x; return;					// PEEK
				case 0x1a: mem[SP + getNextWord ()] = x; return; // [SP + next word]
				case 0x1b: SP = x; return;						// SP
				case 0x1c: PC = x; return;						// PC
				case 0x1d: EX = x; return;						// EX
				case 0x1e: mem[getNextWord ()] = x; return;		// [next word]
			}

			// Literals fails silently
			if (dest == 0x1f || (dest >= 0x20 && dest <= 0x3f)) // literals
				return;

			throw new NotSupportedException ("Unsupported type to set to");
		}
	}
}
