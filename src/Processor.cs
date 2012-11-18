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
				ProcessNextInstruction();
			}
		}

		private ushort[] mem = new ushort[ushort.MaxValue];
		private ushort PC, SP, EX, A, B, C, X, Y, Z, I, J;

		private void ProcessNextInstruction ()
		{
			ushort nextWord = getNextWord();

			ushort opcode = (ushort) (nextWord & 0x1f);
			ushort b = (ushort) ((nextWord & 0x3e0) >> 5);
			ushort a = (ushort) ((nextWord & 0xfc00) >> 10);

			Action<Func<ushort, ushort, int>> binaryOp = (f) =>
			    setValue (b, (ushort) f (getValueL (b), getValueR (a)));

			Action<Func<short, short, int>> binaryOpS = (f) =>
				binaryOp ((x, y) => f ((short) x, (short) y));

			Action<Func<ushort, ushort, bool>> ifOp = (f) => {
				if (f (b, a)) ProcessNextInstruction();
				else SkipUntilNonIf();
			};

			Action<Func<short, short, bool>> ifOpS = (f) =>
				ifOp ((x, y) => f ((short) x, (short) y));

			switch ((OPCODES) opcode)
			{
				case OPCODES.SET:
					setValue (b, getValueR (a));
					break;
				case OPCODES.ADD:
					binaryOp ((x, y) => x + y);
					break;
				case OPCODES.SUB:
					binaryOp ((x, y) => x - y);
					break;
				case OPCODES.MUL:
					binaryOp ((x, y) => x*y);
					break;
				case OPCODES.MLI:
					binaryOpS ((x, y) => x*y);
					break;
				case OPCODES.DIV:
					binaryOp ((x, y) => x/y);
					break;
				case OPCODES.DVI:
					binaryOpS ((x, y) => x/y);
					break;
				case OPCODES.MOD:
					binaryOp ((x, y) => x%y);
					break;
				case OPCODES.MDI:
					binaryOpS ((x, y) => x%y);
					break;
				case OPCODES.AND:
					binaryOp ((x, y) => x & y);
					break;
				case OPCODES.BOR:
					binaryOp ((x, y) => x | y);
					break;
				case OPCODES.XOR:
					binaryOp ((x, y) => x ^ y);
					break;
				case OPCODES.SHR:
					ushort tmpA = getValueR (a);
					ushort tmpB = getValueL (b);
					setValue (b, (ushort) (tmpA >> tmpB));
					setValue ((ushort)VALUES.EX, (ushort)(((ushort)(tmpB << 16) >> tmpA) & 0xffff));
					break;
				case OPCODES.ASR:
					// No declaration (because C#)
					tmpA = getValueR (a);
					tmpB = getValueL (b);
					setValue (b, (ushort)((short)tmpA >> (short)tmpB));
					setValue ((ushort)VALUES.EX, (ushort) (tmpB << (16 - tmpA)));
					break;
				case OPCODES.SHL:
					binaryOp ((x, y) => x >> y);
					setValue ((ushort) VALUES.EX, (ushort) (((b << a) >> 16) & 0xffff));
					break;
				case OPCODES.IFB:
					ifOp ((x, y) => (x & y) != 0);
					break;
				case OPCODES.IFC:
					ifOp ((x, y) => (x & y) == 0);
					break;
				case OPCODES.IFE:
					ifOp ((x, y) => x == y);
					break;
				case OPCODES.IFN:
					ifOp ((x, y) => x != y);
					break;
				case OPCODES.IFG:
					ifOp ((x, y) => x > y);
					break;
				case OPCODES.IFA:
					ifOpS ((x, y) => x > y);
					break;
				case OPCODES.IFL:
					ifOp ((x, y) => x < y);
					break;
				case OPCODES.IFU:
					ifOpS ((x, y) => x < y);
					break;
				case OPCODES.ADX:
					binaryOp ((x, y) => x + y + EX);
					break;
				case OPCODES.SBX:
					binaryOp ((x, y) => x - y + EX);
					break;
				case OPCODES.STI:
					setValue (b, getValueR (a));
					I++;
					J++;
					break;
				case OPCODES.STD:
					setValue (b, getValueR (a));
					I--;
					J--;
					break;
				default:
					throw new NotImplementedException();
			}
		}

		private void SkipUntilNonIf()
		{
			ushort nextWord = getNextWord ();
			ushort opcode = (ushort)(nextWord & 0x1f);

			while (opcode >= (ushort)OPCODES.IFB && opcode <= (ushort)OPCODES.IFU)
			{
				nextWord = getNextWord ();
				opcode = (ushort)(nextWord & 0x1f);
			}
		}

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
				case VALUES.REG_A: return mem[A];
				case VALUES.REG_B: return mem[B];
				case VALUES.REG_C: return mem[C];
				case VALUES.REG_X: return mem[X];
				case VALUES.REG_Y: return mem[Y];
				case VALUES.REG_Z: return mem[Z];
				case VALUES.REG_I: return mem[I];
				case VALUES.REG_J: return mem[J];

				case VALUES.REGN_A: return mem[A + getNextWord()];
				case VALUES.REGN_B: return mem[B + getNextWord()];
				case VALUES.REGN_C: return mem[C + getNextWord()];
				case VALUES.REGN_X: return mem[X + getNextWord()];
				case VALUES.REGN_Y: return mem[Y + getNextWord()];
				case VALUES.REGN_Z: return mem[Z + getNextWord()];
				case VALUES.REGN_I: return mem[I + getNextWord()];
				case VALUES.REGN_J: return mem[J + getNextWord()];

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
