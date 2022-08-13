using System.Diagnostics.CodeAnalysis;

namespace PowMaybe;

public static class May
{
	// ************
	// * Creation *
	// ************
	public static Maybe<T> Some<T>(T v) => new Maybe<T>.Some(v);
	public static Maybe<T> None<T>() => new Maybe<T>.None();

	public static Maybe<T> ToMaybe<T>(this T? v) where T : class => v switch
	{
		null => None<T>(),
		not null => Some(v)
	};

	public static T? ToNullable<T>(this Maybe<T> v) where T : class => v.IsSome(out var val) switch
	{
		true => val,
		false => null
	};


	// **************
	// * Unwrapping *
	// **************
	public static bool IsSome<T>(this Maybe<T> may) => may.IsSome(out _);

	public static bool IsSome<T>(this Maybe<T> may, [NotNullWhen(true)] out T? val)
	{
		switch (may)
		{
			case Maybe<T>.Some { V: var valV }:
				val = valV!;
				return true;

			case Maybe<T>.None:
				val = default;
				return false;

			default:
				throw new ArgumentException();
		}
	}

	public static bool IsNone<T>(this Maybe<T> may) => may.IsNone(out _);

	public static bool IsNone<T>(this Maybe<T> may, [NotNullWhen(false)] out T? val)
	{
		switch (may)
		{
			case Maybe<T>.Some { V: var valV }:
				val = valV!;
				return false;

			case Maybe<T>.None:
				val = default;
				return true;

			default:
				throw new ArgumentException();
		}
	}

	public static T Ensure<T>(this Maybe<T> may) => may.IsSome(out var val) switch
	{
		true => val!,
		false => throw new ArgumentException()
	};

	public static T FailWith<T>(this Maybe<T> may, T def) => may.IsSome(out var val) ? val : def;


	// ****************
	// * Enumerations *
	// ****************
	public static IEnumerable<T> WhereSome<T>(this IEnumerable<Maybe<T>> source) =>
		source.Where(e => e.IsSome()).Select(e => e.Ensure());

	public static Maybe<T> FirstOrMaybe<T>(this IEnumerable<T> source, Func<T, bool>? predicate = null)
	{
		foreach (var elt in source)
			if (predicate == null || predicate(elt))
				return Some(elt);
		return None<T>();
	}

	public static Maybe<T> LastOrMaybe<T>(this IEnumerable<T> source, Func<T, bool>? predicate = null)
	{
		foreach (var elt in source.Reverse())
			if (predicate == null || predicate(elt))
				return Some(elt);
		return None<T>();
	}
}