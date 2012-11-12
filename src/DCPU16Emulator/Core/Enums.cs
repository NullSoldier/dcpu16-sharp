using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DCPU16Emulator.Core
{
	public enum OPCODES
	{
		//- = 0x00,
		SET = 0x01,
		ADD = 0x02,
		SUB = 0x03,
		MUL = 0x04,
		MLI = 0x05,
		DIV = 0x06,
		DVI = 0x07,
		MOD = 0x08,
		MDI = 0x09,
		AND = 0x0a,
		BOR = 0x0b,
		XOR = 0x0c,
		SHR = 0x0d,
		ASR = 0x0e,
		SHL = 0x0f,
		IFB = 0x10,
		IFC = 0x11,
		IFE = 0x12,
		IFN = 0x13,
		IFG = 0x14,
		IFA = 0x15,
		IFL = 0x16,
		IFU = 0x17,
		//- = 0x18,
		//- = 0x19,
		ADX = 0x1a,
		SBX = 0x1b,
		//- = 0x1c,
		//- = 0x1d,
		STI = 0x1e,
		STD = 0x1f

	}

	public enum VALUES : ushort
	{
		// register
		A=0x00,
		B=0x01,
		C=0x02,
		X=0x03,
		Y=0x04,
		Z=0x05,
		I=0x06,
		J=0x07,

		// [register]
		REG_A = 0x08,
		REG_B = 0x09,
		REG_C = 0x0a,
		REG_X = 0x0b,
		REG_Y = 0x0c,
		REG_Z = 0x0d,
		REG_I = 0x0e,
		REG_J = 0x0f,

		// [register + next word]
		REGN_A = 0x10,
		REGN_B = 0x11,
		REGN_C = 0x12,
		REGN_X = 0x13,
		REGN_Y = 0x14,
		REGN_Z = 0x15,
		REGN_I = 0x16,
		REGN_J = 0x17,

		PUSHPOP = 0x18, //If a, POP. If b, push
		PEEK	= 0x19, // [SP]
		PICK	= 0x1a, // [SP + next word]
		SP		= 0x1b, // SP
		PC		= 0x1c, // PC
		EX		= 0x1d, // EX
		NEXT	= 0x1e, // [next word]
		NEXTLIT	= 0x1f  // next word (literal)
	};
}
