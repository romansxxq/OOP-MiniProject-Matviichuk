namespace MedCore.ConsoleUI;

public static class ConsoleInput
{
    public static int ReadInt(string prompt, string errorMessage)
    {
        while (true)
        {
            System.Console.Write(prompt);
            var input = System.Console.ReadLine();
            if (int.TryParse(input, out var value))
                return value;

            System.Console.WriteLine(errorMessage);
        }
    }

    public static DateTime ReadDateTime(string prompt, string errorMessage)
    {
        while (true)
        {
            System.Console.Write(prompt);
            var input = System.Console.ReadLine();
            if (DateTime.TryParse(input, out var value))
                return value;

            System.Console.WriteLine(errorMessage);
        }
    }

    public static string ReadText(string prompt, string errorMessage)
    {
        while (true)
        {
            System.Console.Write(prompt);
            var input = System.Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(input))
                return input.Trim();

            System.Console.WriteLine(errorMessage);
        }
    }

    public static string? ReadOptionalText(string prompt)
    {
        System.Console.Write(prompt);
        var input = System.Console.ReadLine();
        return string.IsNullOrWhiteSpace(input) ? null : input.Trim();
    }
}
