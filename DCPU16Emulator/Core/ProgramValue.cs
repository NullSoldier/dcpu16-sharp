using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DCPU16Emulator.Core
{
	public struct Value
	{
		public Value (Values type)
		{
			TypeOfValue = type;
			Register = Registers.A;
			ActualValue = 0;
		}

		public Value (Values type, ushort value)
		{
			TypeOfValue = type;
			Register = Registers.A;
			ActualValue = value;
		}

		public Value (Values type, Registers register)
		{
			TypeOfValue = type;
			Register = register;
			ActualValue = 0;
		}

		public Values TypeOfValue;
		public Registers Register;
		public ushort ActualValue;
	}
}
