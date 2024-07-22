using Microsoft.Extensions.Logging;
using System;
using System.IO;

public class CustomFileLogger 
{
    private const string FilePath = @"Log.txt"; 
    private const string LogDirectory = "\\Log Error";
    public static void LogError(string message, Exception ex)
    {
        var directory = Directory.GetCurrentDirectory();
        var currentDirectory = AppDomain.CurrentDomain.BaseDirectory;
        var DirectoryFullPath = Path.Combine(directory+LogDirectory,FilePath);

        if (!Path.IsPathFullyQualified(DirectoryFullPath))
        {
            Directory.CreateDirectory(DirectoryFullPath);
        }

        try
        {
            var logMessage = $"{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} [ERROR] {message} - Exception occured - {ex}";
            File.AppendAllText(DirectoryFullPath, logMessage + Environment.NewLine);
        }
        catch (Exception exception)
        {
            Console.WriteLine(exception.ToString());
        }
    }
}
