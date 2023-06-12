namespace PowMaybeErr;

public abstract class MaybeErr<T>
{
	/// <summary>
	/// Allows writing:
	///		from subNode in parent.QueryNode(xPath)
	///		select subNode.InnerText;					// fun = subNode => subNode.InnerText
	///
	/// otherwise you can only 'select subNode', not 'select fun(subNode)'
	///
	/// in FP, this is called map()
	/// </summary>
	public MaybeErr<V> Select<V>(Func<T, V> fun) =>
		this switch
		{
			Some<T> { Value: var val } => MayErr.Some(fun(val)),
			None<T> { Error: var err } => MayErr.None<V>(err),
			_ => throw new ArgumentException()
		};

	/// <summary>
	/// Allows writing:
	///		from subNode in parent.QueryNode(xPath)
	///		from attrValue in subNode.GetAttrFromNode(attrName)
	///		select attrValue;
	///
	/// otherwise only a single 'from' statement is allowed
	/// 
	/// in FP, this is called bind()
	/// </summary>
	public MaybeErr<V> SelectMany<U, V>(Func<T, MaybeErr<U>> mapper, Func<T, U, V> getResult) =>
		this switch
		{
			Some<T> { Value: var val } => mapper(val) switch
			{
				Some<U> { Value: var valFun } => MayErr.Some(getResult(val, valFun)),
				None<U> { Error: var errFun } => MayErr.None<V>(errFun),
				_ => throw new ArgumentException()
			},
			None<T> { Error: var err } => MayErr.None<V>(err),
			_ => throw new ArgumentException()
		};


	public MaybeErr<T> Where(Func<T, bool> predicate) =>
		this switch
		{
			Some<T> { Value: var val } => predicate(val) switch
			{
				true => this,
				false => MayErr.None<T>("FilterError")
			},
			None<T> => this,
			_ => throw new ArgumentException()
		};
}


public sealed class Some<T> : MaybeErr<T>
{
	public readonly T Value;
	internal Some(T value) => Value = value;
	public override string ToString() => $@"Some<{typeof(T).Name}>({Value})";
}

public sealed class None<T> : MaybeErr<T>
{
	public readonly string Error;
	internal None(string error) => Error = error;
	public override string ToString() => $@"None<{typeof(T).Name}>(""{Error}"")";
}