using PowMaybe.Tests.TestSupport;

namespace PowMaybe.Tests;

sealed class Other<T> : Maybe<T>
{
}

class Tests
{
	[Test]
	public void _00_Some() => May.Some(4).ShouldBeSome(4);

	[Test]
	public void _01_None() => May.None<int>().ShouldBeNone();

	[Test]
	public void _02_ToString()
	{
		May.Some(4).ToString().ShouldBe("Some<Int32>(4)");
		May.None<int>().ToString().ShouldBe("None<Int32>()");
	}

	[Test]
	public void _03_Equality()
	{
		(May.Some(4) == May.Some(3 + 1)).ShouldBeTrue();
		(May.Some(4) != May.Some(5)).ShouldBeTrue();

		var set = new HashSet<Maybe<int>> { May.Some(5) };
		set.ShouldContain(May.Some(5));
		set.ShouldNotContain(May.Some(8));
		set.ShouldNotContain(May.None<int>());

		Maybe<int>? other = null;
		May.Some(2).Equals(other).ShouldBeFalse();

		object? otherObj = null;
		May.Some(2).Equals(otherObj).ShouldBeFalse();

		May.None<int>().Equals(May.None<int>()).ShouldBeTrue();
		May.Some(3).Equals(May.Some(4)).ShouldBeFalse();
		May.Some(3).Equals(May.None<int>()).ShouldBeFalse();

		var v = May.Some(4);

		v.Equals((object?)v).ShouldBeTrue();
		v.Equals("john").ShouldBeFalse();

		//(((Maybe<int>?)null) == May.Some(3)).ShouldBeFalse();
		//(May.Some(3) == ((Maybe<int>?)null)).ShouldBeFalse();
		//(May.Some(3).Equals((Maybe<int>?)null)).ShouldBeFalse();
	}

	[Test]
	public void _04_Select()
	{
		(
			from a in May.Some(4)
			select a
		).ShouldBeSome(4);

		(
			from a in May.None<int>()
			select a
		).ShouldBeNone();

		Should.Throw<ArgumentException>(() => new Other<int>().Select(e => e));
	}

	[Test]
	public void _05_SelectMany()
	{
		static Maybe<double> Sqrt(double v) => v switch
		{
			>= 0 => May.Some(Math.Sqrt(v)),
			_ => May.None<double>()
		};
		static Maybe<double> Div(double a, double b) => b switch
		{
			0 => May.None<double>(),
			_ => May.Some(a / b)
		};
		
		(
			from a in Sqrt(16)
			from b in Div(a, 2)
			select b
		).ShouldBeSome(2);

		(
			from a in Sqrt(16)
			from b in Div(a, 0)
			select b
		).ShouldBeNone();

		(
			from a in Sqrt(-16)
			from b in Div(a, 2)
			select b
		).ShouldBeNone();

		Should.Throw<ArgumentException>(() => 
			from a in new Other<int>()
			from b in Sqrt(16)
			select b
		);

		Should.Throw<ArgumentException>(() => 
			from a in Sqrt(16)
			from b in new Other<int>()
			select b
		);
	}

	[Test]
	public void _06_Where()
	{
		May.Some(4).Where(e => e == 4).ShouldBeSome(4);
		May.Some(4).Where(e => e == 5).ShouldBeNone();
		May.None<int>().Where(e => e == 4).ShouldBeNone();
		Should.Throw<ArgumentException>(() => new Other<int>().Where(e => e == 4));
	}

	[Test]
	public void _10_ToMaybe()
	{
		"john".ToMaybe().ShouldBeSome("john");
		((string?)null).ToMaybe().ShouldBeNone();
	}

	[Test]
	public void _11_ToNullable()
	{
		May.Some("john").ToNullable().ShouldBe("john");
		May.None<string>().ToNullable().ShouldBe(null);
	}

	[Test]
	public void _12_IsSome()
	{
		May.Some(23).IsSome().ShouldBeTrue();
		May.None<int>().IsSome().ShouldBeFalse();

		May.Some(23).IsSome(out var valSome).ShouldBeTrue();
		valSome.ShouldBe(23);

		May.None<int>().IsSome(out var valNone).ShouldBeFalse();
		valNone.ShouldBe(default);

		Should.Throw<ArgumentException>(() => new Other<int>().IsSome());
	}

	[Test]
	public void _13_IsNone()
	{
		May.Some(23).IsNone().ShouldBeFalse();
		May.None<int>().IsNone().ShouldBeTrue();

		May.Some(23).IsNone(out var valSome).ShouldBeFalse();
		valSome.ShouldBe(23);

		May.None<int>().IsNone(out var valNone).ShouldBeTrue();
		valNone.ShouldBe(default);

		Should.Throw<ArgumentException>(() => new Other<int>().IsNone());
	}

	[Test]
	public void _14_Ensure()
	{
		May.Some(6).Ensure().ShouldBe(6);
		Should.Throw<ArgumentException>(() => May.None<int>().Ensure());
	}

	[Test]
	public void _15_FailWith()
	{
		May.Some(9).FailWith(2).ShouldBe(9);
		May.None<int>().FailWith(2).ShouldBe(2);
	}
}