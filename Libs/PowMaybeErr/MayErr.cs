using System.Diagnostics.CodeAnalysis;

namespace PowMaybeErr;

public static class MayErr
{
	public static MaybeErr<T> Some<T>(T val) => new Some<T>(val);
	public static MaybeErr<T> None<T>(string msg) => new None<T>(msg);

	
	public static MaybeErr<T> ToMaybeErr<T>(this T? v, string err) where T : class => v switch
	{
		null => None<T>(err),
		not null => Some(v)
	};

	public static T Ensure<T>(this MaybeErr<T> may) => may.IsSome(out var val) switch
	{
		true => val!,
		false => throw new ArgumentException()
	};

	public static string EnsureNone<T>(this MaybeErr<T> may) => may.IsSome(out _, out var err) switch
	{
		true => throw new ArgumentException(),
		false => err!
	};
	
	public static bool IsSome<T>(this MaybeErr<T> may) => may.IsSome(out _, out _);
	public static bool IsSome<T>(this MaybeErr<T> may, [NotNullWhen(true)] out T? val) => may.IsSome(out val, out _);
	public static bool IsSome<T>(this MaybeErr<T> may, [NotNullWhen(true)] out T? val, [NotNullWhen(false)] out string? err)
	{
		switch (may)
		{
			case Some<T> { Value: var someVal }:
				val = someVal;
				err = null;
				if (val == null) throw new ArgumentException();
				return true;
			case None<T> { Error: var noneErr }:
				val = default;
				err = noneErr;
				return false;
			default:
				throw new ArgumentException();
		}
	}

	public static bool IsNone<T>(this MaybeErr<T> may) => may.IsNone(out _, out _);
	public static bool IsNone<T>(this MaybeErr<T> may, [NotNullWhen(false)] out T? val) => may.IsNone(out val, out _);
	public static bool IsNone<T>(this MaybeErr<T> may, [NotNullWhen(false)] out T? val, [NotNullWhen(true)] out string? err)
	{
		switch (may)
		{
			case Some<T> { Value: var someVal }:
				val = someVal;
				err = null;
				if (val == null) throw new ArgumentException();
				return false;
			case None<T> { Error: var noneErr }:
				val = default;
				err = noneErr;
				return true;
			default:
				throw new ArgumentException();
		}
	}

	public static MaybeErr<T> TryMultiple<T>(params Func<MaybeErr<T>>[] gens)
	{
		var res = None<T>("empty array passed to TryMultiple");
		foreach (var gen in gens)
		{
			res = gen();
			if (res.IsSome())
				return res;
		}
		return res;
	}
	
	public static IEnumerable<T> WhereSome<T>(this IEnumerable<MaybeErr<T>> source) =>
		source.Where(e => e.IsSome()).Select(e => e.Ensure());

	public static IEnumerable<string> WhereNone<T>(this IEnumerable<MaybeErr<T>> source) =>
		source.Where(e => e.IsNone()).Select(e => e.EnsureNone());
	

	public static MaybeErr<T> LastOrMaybeErr<T>(this IEnumerable<T> source, string err)
	{
		var list = source.ToList();
		return (list.Count > 0) switch
		{
			true => Some(list.Last()),
			false => None<T>(err)
		};
	}

	public static MaybeErr<T> LastOrMaybeErr<T>(this IEnumerable<T> source, Func<T, bool> predicate, string err)
	{
		var list = source.Where(predicate).ToList();
		return (list.Count > 0) switch
		{
			true => Some(list.Last()),
			false => None<T>(err)
		};
	}

	public static MaybeErr<T> FirstOrMaybeErr<T>(this IEnumerable<T> source, string err)
	{
		foreach (var elt in source)
			return Some(elt);
		return None<T>(err);
	}

	public static MaybeErr<T> FirstOrMaybeErr<T>(this IEnumerable<T> source, Func<T, bool> predicate, string err)
	{
		foreach (var elt in source)
			if (predicate(elt))
				return Some(elt);
		return None<T>(err);
	}
}