using System;
using System.IO;

public static class StringExt
{
    private static DateOnly ExtractDateFromFilePath(string path)
    {
        return DateOnly.ParseExact(path.Substring(path.Length - "yyyy_MM_dd.json".Length, "yyyy_MM_dd".Length), "yyyy_MM_dd", null);
    }

    public static string ExtractDateRangeFromPath(this string path, bool compact)
    {
        var fromDate = ExtractDateFromFilePath(path);
        return $"{fromDate:d}{(compact ? "-" : " - ")}{fromDate.AddDays(5):d}";
    }

    public static string ConvertToTimesheetPdfPath(this string path)
    {
        var fromDate = ExtractDateFromFilePath(path);
        return Path.Join(Manager.SaveDir, "stundenzettel", $"Arbeitsbericht {Settings.UserSettings.User} [{fromDate:d}-{fromDate.AddDays(5):d}].pdf");
    }
}