# PowMaybe

## Table of content

- [Introduction](#introduction)
- [Usage](#usage)
- [Example](#example)
- [License](#license)



## Introduction

Lightweight Maybe monad library to simplify code that can fail.

It will help you transform code like this:

### Before
```c#
Person? ParsePage(string url)
{
    var html = client.Query(url);
    if (html == null)
        return null;
    var root = Html.GetRoot(html);
    var node = root.SelectSingleNode("xpath query");
    if (node == null)
        return null;
    var personInfo = Utils.ParseNode(node);
    if (personInfo == null)
        return null;
    return new Person(personInfo);
}
```

Into this:

### After
```c#
Maybe<Person> ParsePage(string url) =>
    from html in Utils.Query(url)
    let root = Utils.GetRoot(html)
    from node in root.MaySelectSingleNode("xpath query")
    from personInfo in Utils.MayParseNode(node)
    select new Person(personInfo);
```

The result is:
- shorter code
- less nested
- no if conditions
- no nullable references

It will not apply for all the code everywhere, but when it does apply it will drastically reduce the potential for bugs.


## Usage

### Creation
```c#
var a = May.Some(47);
var b = May.None<string>();
// nullable reference -> Maybe<>
var mayPerson = person.ToMaybe();
// Maybe<> -> nullable reference
var person = mayPerson.ToNullable();
```

### Combining
```c#
Maybe<string> QueryHtml(string url);
Maybe<Person> ParsePerson(string html);

// use any number of from/in statements with a select at the end
Maybe<Person> QueryAndParse(string url) =>
    from html in QueryHtml(url)
    from person in ParsePerson(html)
    where person.Name != "John" // you can also use where statements
    select parson;

// Unwrapping
Maybe<Person> mayPerson = ...

if (mayPerson.IsSome(out var person))
{
    // Success, you can access person here
}
else
{
    // Failure
}

Person person = mayPerson.Ensure(); // throws an Exception if mayPerson is None

Person person = mayPerson.FailWith(peter); // returns peter if mayPerson is None
```

### Enumerations
```c#
IEnumerable<T> WhereSome<T>(this IEnumerable<Maybe<T>> source);
Maybe<T> FirstOrMaybe<T>(this IEnumerable<T> source, Func<T, bool>? predicate = null)
// and similar LastOrMaybe

// Examples
// ========
new [] { May.Some(4), May.None<int>() May.Some(12) }.WhereSome();
// int[] { 4, 12 }

new [] { 2, 6, 5 }.FirstOrMaybe(e => e % 3 == 0)
// Some(6)

new [] { 2, 6, 5 }.FirstOrMaybe(e => e % 3 == 1)
// None<int>()
```


## Example
Let's say you want to read your configuration from multiple sources:
- environment variables
- command line arguments
- json file

And once a source returns a configuration, you do not want to read the other sources.

You could write it this way:
```c#
Maybe<Config> ReadFromEnvVars();
Maybe<Config> ReadFromArgs(string[] args);
Maybe<Config> ReadFromFile(string file);

Maybe<Config> ReadConfig(string[] args, string file) =>
	new[]
	{
		() => ReadFromEnvVars(),
		() => ReadFromArgs(args),
		() => ReadFromFile(file),
	}
	.Select(readFun => readFun())
	.WhereSome()
	.FirstOrMaybe();
```


## License

MIT
