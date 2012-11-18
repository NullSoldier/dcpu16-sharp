using NUnit.Framework;

namespace Tests.Tests
{
	[TestFixture]
	public class LoopTest : BaseTest
	{
		public LoopTest() : base ("Loop.bin") { }

		[Test]
		public void Loop()
		{
			base.TickCPU (50);

			Assert.AreEqual (5, cpu.A);
		}
	}
}
