using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dcpu16sharp
{
	public interface IHardware
	{
		UInt32 ID { get; }
		UInt32 Manufactorer { get; }
		ushort Version { get; }
		
		void Interrupt();
	}
}
