using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DCPU16Emulator.Core
{
	public enum Registers
	{
		A,
		B,
		C,
		X,
		Y,
		Z,
		I,
		J
	};

	public enum Values
	{
		PC,
		SP,
		EX,
		Register,
		Command,
		Literal,
		PointerValue
	}
}
