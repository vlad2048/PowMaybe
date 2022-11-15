using PowMaybe.Tests.TestSupport;

namespace PowMaybe.Tests.Extensions;

class MayParseExtTests
{
	[Test]
	public void _01_Byte()
	{
		"47".TryParseByteMaybe().ShouldBeSome<byte>(47);
		"386".TryParseByteMaybe().ShouldBeNone();
	}

	[Test]
	public void _02_Int()
	{
		"47".TryParseIntMaybe().ShouldBeSome(47);
		"err".TryParseIntMaybe().ShouldBeNone();
	}

	[Test]
	public void _03_Double()
	{
		"12.34".TryParseDoubleMaybe().ShouldBeSome(12.34);
		"not a number".TryParseDoubleMaybe().ShouldBeNone();
	}
}