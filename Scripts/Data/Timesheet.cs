using System;
using System.Text.Json.Serialization;

public record Timesheet
{
    [JsonInclude] public DateOnly FomDate { get; init; }
    [JsonInclude] public TimesheetEntry[] Entries { get; init; }
}

public record TimesheetEntry(string Hours, string Location, string Manager, string Km);
