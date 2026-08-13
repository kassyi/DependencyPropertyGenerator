namespace Kassyi.Generators.Extensions.Models;

/// <summary>Represents a generated source file name and its source text content.</summary>
public readonly record struct FileWithName(
    string Name,
    string Text)
{
    /// <summary>Gets an empty file representation.</summary>
    public static FileWithName Empty => new(
        Name: string.Empty,
        Text: string.Empty);

    /// <summary>Gets a value indicating whether the file name or content is empty.</summary>
    public bool IsEmpty => string.IsNullOrEmpty(Name) || string.IsNullOrEmpty(Text);
}
