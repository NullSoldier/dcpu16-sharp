using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dcpu16sharp
{
	public class Processor
	{
		public Processor (ushort[] instructions, IHardware[] connectedHardware)
		{
			instructions.CopyTo (mem, 0);
		}

		public event EventHandler SoftwareInterruptFired;

		public bool QueueInterrupts;
		public IHardware[] Hardware;
		public ushort[] mem = new ushort[ushort.MaxValue+1];
		public ushort PC, SP, EX, IA;
		public ushort A, B, C, X, Y, Z, I, J;

		public ushort GetNextWord()
		{
			return mem[PC++];
		}

		public ushort GetValueL (ushort bits)
		{
			return GetValue (bits, false);
		}

		public ushort GetValueR (ushort bits)
		{
			return GetValue (bits, true);
		}

		public ushort GetValue (ushort bits, bool bitsAreA)
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

				// [register]
				case VALUES.REG_A: return mem[A];
				case VALUES.REG_B: return mem[B];
				case VALUES.REG_C: return mem[C];
				case VALUES.REG_X: return mem[X];
				case VALUES.REG_Y: return mem[Y];
				case VALUES.REG_Z: return mem[Z];
				case VALUES.REG_I: return mem[I];
				case VALUES.REG_J: return mem[J];
				
				// [register + nextWord]
				case VALUES.REGN_A: return mem[A + GetNextWord ()];
				case VALUES.REGN_B: return mem[B + GetNextWord ()];
				case VALUES.REGN_C: return mem[C + GetNextWord ()];
				case VALUES.REGN_X: return mem[X + GetNextWord ()];
				case VALUES.REGN_Y: return mem[Y + GetNextWord ()];
				case VALUES.REGN_Z: return mem[Z + GetNextWord ()];
				case VALUES.REGN_I: return mem[I + GetNextWord ()];
				case VALUES.REGN_J: return mem[J + GetNextWord ()];

				case VALUES.SP: return SP;
				case VALUES.PC: return PC;
				case VALUES.EX: return EX;
				case VALUES.PUSHPOP: return bitsAreA ? mem[SP++] : mem[--SP];
				case VALUES.PEEK: return mem[SP];
				case VALUES.PICK: return mem[SP + GetNextWord()];
				case VALUES.NEXT: return mem[GetNextWord()];
				case VALUES.NEXTLIT: return GetNextWord ();

				default: throw new NotSupportedException ("Value " + bits + " is not supported");
			}
		}

		public void SetValue(ushort dest, ushort x)
		{
			switch ((VALUES)dest)
			{
				case VALUES.A: A = x; return;
				case VALUES.B: B = x; return;
				case VALUES.C: C = x; return;
				case VALUES.X: X = x; return;
				case VALUES.Y: Y = x; return;
				case VALUES.Z: Z = x; return;
				case VALUES.I: I = x; return;
				case VALUES.J: J = x; return;

				// [register]
				case VALUES.REG_A: mem[A] = x; return;
				case VALUES.REG_B: mem[B] = x; return;
				case VALUES.REG_C: mem[C] = x; return;
				case VALUES.REG_X: mem[X] = x; return;
				case VALUES.REG_Y: mem[Y] = x; return;
				case VALUES.REG_Z: mem[Z] = x; return;
				case VALUES.REG_I: mem[I] = x; return;
				case VALUES.REG_J: mem[J] = x; return;

				// [register + next word]
				case VALUES.REGN_A: mem[A + GetNextWord ()] = x; return;
				case VALUES.REGN_B: mem[B + GetNextWord ()] = x; return;
				case VALUES.REGN_C: mem[C + GetNextWord ()] = x; return;
				case VALUES.REGN_X: mem[X + GetNextWord ()] = x; return;
				case VALUES.REGN_Y: mem[Y + GetNextWord ()] = x; return;
				case VALUES.REGN_Z: mem[Z + GetNextWord ()] = x; return;
				case VALUES.REGN_I: mem[I + GetNextWord ()] = x; return;
				case VALUES.REGN_J: mem[J + GetNextWord ()] = x; return;

				case VALUES.PUSHPOP: mem[--SP] = x; return;
				case VALUES.PEEK: mem[SP] = x; return;
				case VALUES.PICK: mem[SP + GetNextWord ()] = x; return;
				case VALUES.SP: SP = x; return;
				case VALUES.PC: PC = x; return;
				case VALUES.EX: EX = x; return;
				case VALUES.NEXT: mem[GetNextWord ()] = x; return;
			}

			// Literals fails silently
			if (dest == 0x1f || (dest >= 0x20 && dest <= 0x3f)) // literals
				return;

			throw new NotSupportedException ("Unsupported type to set to");
		}

		public void ProcessNextInstruction()
		{
			ushort nextWord = GetNextWord();

			ushort opcode = (ushort) (nextWord & 0x1f);
			ushort b = (ushort) ((nextWord & 0x3e0) >> 5);
			ushort a = (ushort) ((nextWord & 0xfc00) >> 10);

			Action<Func<ushort, ushort, int>> binaryOp = (f) =>
			    SetValue (b, (ushort) f (GetValueL (b), GetValueR (a)));

			Action<Func<short, short, int>> binaryOpS = (f) =>
				binaryOp ((x, y) => f ((short) x, (short) y));

			Action<Func<ushort, ushort, bool>> ifOp = (f) => {
				if (f (GetValueL(b), GetValueR(a))) ProcessNextInstruction();
				else SkipUntilNonIf();
			};

			Action<Func<short, short, bool>> ifOpS = (f) =>
				ifOp ((x, y) => f ((short) x, (short) y));

			switch ((OPCODES) opcode)
			{
				case OPCODES.SPECIAL:
					ProcessSpecialInstruction (b, GetValueR (a));
					break;
				case OPCODES.SET:
					SetValue (b, GetValueR (a));
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
					ushort tmpA = GetValueR (a);
					ushort tmpB = GetValueL (b);
					SetValue (b, (ushort) (tmpA >> tmpB));
					SetValue ((ushort)VALUES.EX, (ushort)(((ushort)(tmpB << 16) >> tmpA) & 0xffff));
					break;
				case OPCODES.ASR:
					// No declaration (because C#)
					tmpA = GetValueR (a);
					tmpB = GetValueL (b);
					SetValue (b, (ushort)((short)tmpA >> (short)tmpB));
					SetValue ((ushort)VALUES.EX, (ushort) (tmpB << (16 - tmpA)));
					break;
				case OPCODES.SHL:
					tmpA = GetValueR (a);
					tmpB = GetValueL (b);
					SetValue (b, (ushort) (tmpB << tmpA));
					SetValue ((ushort) VALUES.EX, (ushort) (((tmpB << tmpA) >> 16) & 0xffff));
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
					SetValue (b, GetValueR (a));
					I++;
					J++;
					break;
				case OPCODES.STD:
					SetValue (b, GetValueR (a));
					I--;
					J--;
					break;
				default:
					throw new NotImplementedException();
			}
		}

		private void ProcessSpecialInstruction (ushort opcode, ushort rValue)
		{
			switch ((SPECIAL_OPCODES)opcode)
			{
				case SPECIAL_OPCODES.JSR:
					mem[--SP] = PC;
					PC = GetValueR (rValue);
					break;
				case SPECIAL_OPCODES.INT:
					FireSoftwareInterrupt (GetValueR(rValue));
					break;
				case SPECIAL_OPCODES.IAG:
					SetValue (rValue, IA);
					break;
				case SPECIAL_OPCODES.IAS:
					IA = GetValueR (rValue);
					break;
				case SPECIAL_OPCODES.RFI:
					QueueInterrupts = false;
					A = mem[SP++];
					PC = mem[SP++];
					break;
				case SPECIAL_OPCODES.IAQ:
					QueueInterrupts = GetValueR (rValue) != 0;
					break;
				case SPECIAL_OPCODES.HWN:
					SetValue (rValue, (ushort)Hardware.Length);
					break;
				case SPECIAL_OPCODES.HWQ:
				{
					var hardware = Hardware[GetValueR (rValue)];
					A = (ushort) (hardware.ID & 0xffff0000);
					B = (ushort) (hardware.ID & 0x0000ffff);
					C = hardware.Version;
					X = (ushort) (hardware.Manufactorer & 0xffff0000);
					Y = (ushort) (hardware.Manufactorer & 0x0000ffff);
					break;
				}
				case SPECIAL_OPCODES.HWI:
				{
					var hardware = Hardware[GetValueR (rValue)];
					hardware.Interrupt();
					break;
				}
				default:
					throw new NotSupportedException();
			}
		}

		private void SkipUntilNonIf()
		{
			ushort tmpSP = SP;

			ushort currentWord = mem[PC];
			ushort opcode = (ushort)(currentWord & 0x1f);

			while (opcode >= (ushort)OPCODES.IFB && opcode <= (ushort)OPCODES.IFU)
			{
				currentWord = GetNextWord();
				opcode = (ushort)(currentWord & 0x1f);

				GetValueR ((ushort)((currentWord & 0x3e0) >> 5));
				GetValueL ((ushort)((currentWord & 0xfc00) >> 10));
			}

			currentWord = GetNextWord ();
			GetValueR ((ushort)((currentWord & 0x3e0) >> 5));
			GetValueL ((ushort)((currentWord & 0xfc00) >> 10));

			SP = tmpSP;
		}

		private void FireSoftwareInterrupt (ushort message)
		{
			var handler = SoftwareInterruptFired;
			if (handler != null)
				handler (this, EventArgs.Empty);
		}
	}
}
