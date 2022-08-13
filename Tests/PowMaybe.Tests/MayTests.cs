using PowMaybe.Tests.TestSupport;

namespace PowMaybe.Tests;

class Tests
{
	[Test]
	public void _00_Some() => May.Some(4).ShouldBeSome(4);

	[Test]
	public void _01_None() => May.None<int>().ShouldBeNone();

	[Test]
	public void _02_ToMaybe()
	{
		"john".ToMaybe().ShouldBeSome("john");
		((string?)null).ToMaybe().ShouldBeNone();
	}

	[Test]
	public void _03_ToNullable()
	{
		May.Some("john").ToNullable().ShouldBe("john");
		May.None<string>().ToNullable().ShouldBe(null);
	}

	[Test]
	public void _04_IsSome()
	{
		May.Some(23).IsSome().ShouldBeTrue();
		May.None<int>().IsSome().ShouldBeFalse();

		May.Some(23).IsSome(out var valSome).ShouldBeTrue();
		valSome.ShouldBe(23);

		May.None<int>().IsSome(out var valNone).ShouldBeFalse();
		valNone.ShouldBe(default);
	}

	[Test]
	public void _05_IsNone()
	{
		May.Some(23).IsNone().ShouldBeFalse();
		May.None<int>().IsNone().ShouldBeTrue();

		May.Some(23).IsNone(out var valSome).ShouldBeFalse();
		valSome.ShouldBe(23);

		May.None<int>().IsNone(out var valNone).ShouldBeTrue();
		valNone.ShouldBe(default);
	}

	[Test]
	public void _06_Ensure()
	{
		May.Some(6).Ensure().ShouldBe(6);
		Should.Throw<ArgumentException>(() => May.None<int>().Ensure());
	}

	[Test]
	public void _07_FailWith()
	{
		May.Some(9).FailWith(2).ShouldBe(9);
		May.None<int>().FailWith(2).ShouldBe(2);
	}

	[Test]
	public void _08_WhereSome()
	{
		Array.Empty<Maybe<int>>().WhereSome().ShouldBeArray();
		new[] { May.None<int>() }.WhereSome().ShouldBeArray();
		new[] { May.None<int>(), May.Some(5) }.WhereSome().ShouldBeArray(5);
		new[] { May.Some(7), May.None<int>(), May.Some(2) }.WhereSome().ShouldBeArray(7, 2);
	}

	[Test]
	public void _09_FirstOrMaybe()
	{
		Array.Empty<int>().FirstOrMaybe().ShouldBeNone();
		new[] { 3 }.FirstOrMaybe().ShouldBeSome(3);
		new[] { 3, 4 }.FirstOrMaybe().ShouldBeSome(3);
		new[] { 3, 4, 8 }.FirstOrMaybe(e => e % 2 == 0).ShouldBeSome(4);
		new[] { 3, 4, 8 }.FirstOrMaybe(e => e == 10).ShouldBeNone();
	}

	[Test]
	public void _10_LastOrMaybe()
	{
		Array.Empty<int>().LastOrMaybe().ShouldBeNone();
		new[] { 3 }.LastOrMaybe().ShouldBeSome(3);
		new[] { 3, 4 }.LastOrMaybe().ShouldBeSome(4);
		new[] { 8, 4, 3 }.LastOrMaybe(e => e % 2 == 0).ShouldBeSome(4);
		new[] { 8, 4, 3 }.LastOrMaybe(e => e == 10).ShouldBeNone();
	}
}