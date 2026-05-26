using Spectre.Console;

namespace MedCore.ConsoleUI;

public static class ConsoleInput
{
    public static int ReadInt(string prompt, string errorMessage)
    {
        var input = AnsiConsole.Prompt(
            new TextPrompt<string>(prompt)
                .ValidationErrorMessage(errorMessage)
                .Validate(value => int.TryParse(value, out _)
                    ? ValidationResult.Success()
                    : ValidationResult.Error(errorMessage)));

        return int.Parse(input);
    }

    public static DateTime ReadDateTime(string prompt, string errorMessage)
    {
        return AnsiConsole.Prompt(
            new TextPrompt<DateTime>(prompt)
                .ValidationErrorMessage(errorMessage));
    }

    public static string ReadText(string prompt, string errorMessage)
    {
        var value = AnsiConsole.Prompt(
            new TextPrompt<string>(prompt)
                .ValidationErrorMessage(errorMessage)
                .Validate(input => !string.IsNullOrWhiteSpace(input)
                    ? ValidationResult.Success()
                    : ValidationResult.Error(errorMessage)));

        return value.Trim();
    }

    public static string? ReadOptionalText(string prompt)
    {
        var value = AnsiConsole.Prompt(new TextPrompt<string>(prompt).AllowEmpty());
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }

    public static string ReadMenu(string title, IEnumerable<string> options)
    {
        return AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title(title)
                .PageSize(10)
                .AddChoices(options));
    }
}
