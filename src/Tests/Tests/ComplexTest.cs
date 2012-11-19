using NUnit.Framework;

namespace Tests.Tests
{
	[TestFixture]
	public class ComplexTest : BaseLoadedTest
	{
		public ComplexTest() : base ("ComplexTest.bin") { }

		[Test]
		public void Complex()
		{
			base.TickCPU (50);

			Assert.AreEqual (0x40, cpu.X);
		}
	}
}
