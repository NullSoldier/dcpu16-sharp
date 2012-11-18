using NUnit.Framework;

namespace Tests.Tests
{
	[TestFixture]
	public class InteruptTest : BaseTest
	{
		public InteruptTest() : base ("Samples/SoftwareInterruptTest.bin") { }

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
