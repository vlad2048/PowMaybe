namespace PowMaybe.Tests.TestSupport;

static class ShouldlyExtensions
{
	public static void ShouldBeSome<T>(this Maybe<T> mayVal, T expVal)
	{
		mayVal.IsSome(out var actVal).ShouldBeTrue();
		actVal.ShouldBe(expVal);
	}

	public static void ShouldBeNone<T>(this Maybe<T> mayVal) =>
		mayVal.IsNone().ShouldBeTrue();

	public static void ShouldBeArray<T>(this IEnumerable<T> actArr, params T[] expArr) =>
		Assert.That(actArr, Is.EqualTo(expArr));
}