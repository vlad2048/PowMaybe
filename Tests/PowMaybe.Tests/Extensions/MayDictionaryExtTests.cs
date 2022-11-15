using PowMaybe.Tests.TestSupport;

namespace PowMaybe.Tests.Extensions;

class MayDictionaryExtTests
{
	[Test]
	public void _00_FoundAndNotFound()
	{
		var map = new Dictionary<int, string>
		{
			{ 2, "great" }
		};
		map.GetOrMaybe(2).ShouldBeSome("great");
		map.GetOrMaybe(5).ShouldBeNone();
	}
}