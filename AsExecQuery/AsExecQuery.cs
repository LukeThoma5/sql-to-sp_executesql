// See https://aka.ms/new-console-template for more information

using System.Text;
using System.Text.RegularExpressions;

var source = await File.ReadAllTextAsync("in.sql");

var descriptionRegex = MyRegex();
var description = descriptionRegex.Match(source).Groups[1].Value;

var param = description.Split(",").Select(a =>
{
	var split = a.Split(" ");
	return new
	{
		Name = split[0],
		Type = split[1],
	};
}).ToArray();

var rest = source[(description.Length + 2)..];

var builder = new StringBuilder();

builder.Append("EXEC sp_executesql\nN'");
builder.Append(rest.Replace("'", "''"));
builder.Append("',\nN'");
builder.Append(description);
builder.Append("'\n");
foreach (var a in param)
{
	builder.Append(", " + a.Name + " = null\n");
}

Console.WriteLine(builder.ToString());

await File.WriteAllTextAsync("out.sql", builder.ToString());

partial class Program
{
    [GeneratedRegex("\\((.+)\\)", RegexOptions.Compiled)]
    private static partial Regex MyRegex();
}