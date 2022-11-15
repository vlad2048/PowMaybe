using PowMaybe.Tests.TestSupport;

namespace PowMaybe.Tests.Extensions;

class MayIEnumerableExtTests
{
	[Test]
	public void _01_WhereSome()
	{
		Array.Empty<Maybe<int>>().WhereSome().ShouldBeArray();
		new[] { May.None<int>() }.WhereSome().ShouldBeArray();
		new[] { May.None<int>(), May.Some(5) }.WhereSome().ShouldBeArray(5);
		new[] { May.Some(7), May.None<int>(), May.Some(2) }.WhereSome().ShouldBeArray(7, 2);
	}

	[Test]
	public void _02_FirstOrMaybe()
	{
		Array.Empty<int>().FirstOrMaybe().ShouldBeNone();
		new[] { 3 }.FirstOrMaybe().ShouldBeSome(3);
		new[] { 3, 4 }.FirstOrMaybe().ShouldBeSome(3);
		new[] { 3, 4, 8 }.FirstOrMaybe(e => e % 2 == 0).ShouldBeSome(4);
		new[] { 3, 4, 8 }.FirstOrMaybe(e => e == 10).ShouldBeNone();
	}

	[Test]
	public void _03_LastOrMaybe()
	{
		Array.Empty<int>().LastOrMaybe().ShouldBeNone();
		new[] { 3 }.LastOrMaybe().ShouldBeSome(3);
		new[] { 3, 4 }.LastOrMaybe().ShouldBeSome(4);
		new[] { 8, 4, 3 }.LastOrMaybe(e => e % 2 == 0).ShouldBeSome(4);
		new[] { 8, 4, 3 }.LastOrMaybe(e => e == 10).ShouldBeNone();
	}

	[Test]
	public void _04_IndexOfMaybe()
	{
		var arr = new[] { 3, 5, 11 };
		arr.IndexOfMaybe(e => e == 5).ShouldBeSome(1);
		arr.IndexOfMaybe(e => e == 42).ShouldBeNone();
	}
}