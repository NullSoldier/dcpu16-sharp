using NUnit.Framework;

namespace Tests.Tests
{
	[TestFixture]
	public class IncrementTest : BaseTest
	{
		public IncrementTest() : base ("Increment.bin") { }

		[Test]
		public void Increment()
		{
			base.TickCPU (15);

			Assert.AreEqual (101, cpu.A);
			Assert.AreEqual (56, cpu.B);
			Assert.AreEqual (157, cpu.C);
		}
	}
}
