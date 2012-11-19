using NUnit.Framework;

namespace Tests.Tests
{
	[TestFixture]
	public class InteruptTest : BaseLoadedTest
	{
		public InteruptTest() : base ("SoftwareInterruptTest.bin") { }

		[Test]
		public void Interrupt()
		{
			cpu.SoftwareInterruptFired += (sender, args) =>
				Assert.Pass ("Interrupt was encountered");

			base.TickCPU (5);
			Assert.Fail ("Interrupt was not encountered");
		}
	}
}
