using Spectre.Console;
using System.Diagnostics;
using System.Globalization;
using static CodingTracker.Enums;

namespace CodingTracker;

class UserInterface
{
    DatabaseConnector db = new DatabaseConnector();

    internal void MainMenu()
    {
        bool tracking = true;
        while (tracking)
        {
            AnsiConsole.MarkupLine("Welcome to the [bold red]Coding Tracker[/]\n");
            var option = AnsiConsole.Prompt(
                new SelectionPrompt<MenuOptions>()
                .Title("[green]What do you want to do?[/]")
                .AddChoices(Enum.GetValues<MenuOptions>())
                );

            switch (option)
            {
                case MenuOptions.ViewRecords:
                    ViewRecords();
                    break;

                case MenuOptions.Stopwatch:
                    Stopwatch();
                    break;

                case MenuOptions.AddRecord:
                    AddRecord();
                    break;

                case MenuOptions.UpdateRecord:
                    UpdateRecord();
                    break;

                case MenuOptions.DeleteRecord: DeleteRecord();
                    DeleteRecord();
                    break;

                case MenuOptions.CloseApplication:
                    Environment.Exit(0);
                    break;

            }
        }
        
    }
    
    internal void ViewRecords()
    {
        string filterInput = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("What period would you like to see [yellow]results[/] from?")
                .AddChoices("All time","Last week","Last month","Last year","Custom"));

        string filterStartDate = "01/01/0001";
        string filterEndDate = DateTime.Now.ToShortDateString();
        switch(filterInput)
        {
            case "Last week":
                filterStartDate = DateTime.Now.AddDays(-7).ToShortDateString();
                break;

            case "Last month":
                filterStartDate = DateTime.Now.AddMonths(-1).ToShortDateString();
                break;

            case "Last year":
                filterStartDate = DateTime.Now.AddYears(-1).ToShortDateString();
                break;

            case "Custom":
                filterStartDate = AnsiConsole.Ask<string>("Enter what date would you like to start filtering from (e.g. 02/06/2025): ");
                while (!DateTime.TryParseExact(filterStartDate, "dd/MM/yyyy", new CultureInfo("en-GB"), DateTimeStyles.None, out _))
                {
                    filterStartDate = AnsiConsole.Ask<string>("Invalid time format. Enter what date would you like to start filtering from (e.g. 02/06/2025): ");
                }

                filterEndDate = AnsiConsole.Ask<string>("Enter what date you would like to end filtering from (e.g. 02/06/2025): ", filterEndDate);
                while (!DateTime.TryParseExact(filterEndDate, "dd/MM/yyyy", new CultureInfo("en-GB"), DateTimeStyles.None, out _))
                {
                    filterEndDate = AnsiConsole.Ask<string>("Invalid time format. Enter what date you would like to end filtering from (e.g. 02/06/2025): ", filterEndDate);
                }
                break;
        }

        var records = db.GetRecords();

        var table = new Table();
        table.AddColumn("ID");
        table.AddColumn("Start time");
        table.AddColumn("End time");
        table.AddColumn("Duration");
        table.AddColumn("Date");
        TimeSpan totalDurationTime = new TimeSpan();
        foreach (Records record in records)
        {
            if (Convert.ToDateTime(record.date) >= Convert.ToDateTime(filterStartDate) && Convert.ToDateTime(record.date) <= Convert.ToDateTime(filterEndDate))
            {
                table.AddRow(record.trackerID.ToString(), record.startTime, record.endTime, record.duration, record.date);
                totalDurationTime += TimeSpan.Parse(record.duration);
            }
        }

        AnsiConsole.Write(table);
        AnsiConsole.MarkupLine("The total duration of coding time is [DarkSeaGreen]{0} days {1} hours {2} minutes[/]",totalDurationTime.Days ,totalDurationTime.Hours,totalDurationTime.Minutes);


        AnsiConsole.MarkupLine("\nPress [green]ENTER[/] return to main menu");
        Console.ReadLine();
        AnsiConsole.Clear();
    }

    internal void AddRecord()
    {
        DateTime currentDateTime = DateTime.Now;
        string currentTime = currentDateTime.ToShortTimeString();
        string currentDate = currentDateTime.ToShortDateString();

        DateTime inputedStartTime;
        DateTime inputedEndTime;

        string inputedStartTimeString = AnsiConsole.Ask<string>("Enter the [blue]start[/] time (e.g. 09:10): ",currentTime);
        while (!DateTime.TryParseExact(inputedStartTimeString, "HH:mm", new CultureInfo("en-GB"), DateTimeStyles.None, out inputedStartTime))
        {
            inputedStartTimeString = AnsiConsole.Ask<string>("Invalid time format. Please enter the [blue]start[/] time (e.g. 09:10): ", currentTime);
        }
        
        string inputedEndTimeString = AnsiConsole.Ask<string>("\nEnter the [green]end[/] time (e.g. 09:10): ", currentTime);
        while (!(DateTime.TryParseExact(inputedEndTimeString, "HH:mm", new CultureInfo("en-GB"),DateTimeStyles.None, out inputedEndTime)) || inputedStartTime > inputedEndTime)
        {
            inputedEndTimeString = AnsiConsole.Ask<string>("Invalid time format. Please enter the [green]end[/] time (e.g. 09:10): ", currentTime);
        }

        string inputedDate = AnsiConsole.Ask<string>("\nEnter the [red]date[/]: ", currentDate);
        while (!DateTime.TryParseExact(inputedDate, "dd/MM/yyyy", new CultureInfo("en-GB"), DateTimeStyles.None, out _)) 
        {
            inputedDate = AnsiConsole.Ask<string>("Invalid date format. Please enter the [red]date[/] (e.g. 02/06/2025): ", currentDate);
        }

        var timeDifference = inputedEndTime - inputedStartTime;

        var duration = string.Format("{0:00}:{1:00}", timeDifference.Hours, timeDifference.Minutes);

        var confimRecord =  AnsiConsole.Confirm(@$"Do you want to create a record where:
            [blue]Start time[/]: {inputedStartTimeString}
            [green]End time[/]: {inputedEndTimeString}
            [teal]Duration[/]: {duration}
            [red]Date[/]: {inputedDate}",
            true);

        if (confimRecord)
        {
            db.AddRecord(inputedStartTimeString, inputedEndTimeString, duration, inputedDate);
        }
        else
        {
            AnsiConsole.Clear();
            return;
        }

        AnsiConsole.MarkupLine("New record [green]created[/]. Press [green]ENTER[/] to return to main menu");
        Console.ReadKey();
        AnsiConsole.Clear();
    }

    internal void UpdateRecord()
    {
        var fieldToUpdate = AnsiConsole.Prompt(
            new SelectionPrompt<tableFields>()
            .Title("What would you like to update?")
            .AddChoices(Enum.GetValues<tableFields>().Where(f => f != tableFields.trackerID && f != tableFields.duration)));

        var records = db.GetRecords();
        var recordToUpdate = AnsiConsole.Prompt(
            new SelectionPrompt<Records>()
            .Title("Which [blue]record[/] would you like to [red]update[/]?")
            .PageSize(5)
            .UseConverter(r => $"ID: {r.trackerID} | Start Time: {r.startTime} | EndTime: {r.endTime} | Duration: {r.duration} | Date: {r.date}")
            .AddChoices(records)).trackerID ?? 0;

        switch (fieldToUpdate)
        {
            case tableFields.startTime:
                DateTime endTime = Convert.ToDateTime(db.GetRecord(recordToUpdate).endTime);
        
                DateTime updatedStartTime;
                string updatedStartTimeString = AnsiConsole.Ask<string>("Please enter the updated [blue]start[/] time (e.g. 09:10): ");
                while (!DateTime.TryParseExact(updatedStartTimeString, "HH:mm", new CultureInfo("en-GB"), DateTimeStyles.None, out updatedStartTime) || updatedStartTime > endTime)
                {
                    updatedStartTimeString = AnsiConsole.Ask<string>("Invalid time format. Please enter the updated [blue]start[/] time(e.g. 09:10): ");
                }

                var timeDifference = endTime - updatedStartTime;
                var duration = string.Format("{0:00}:{1:00}", timeDifference.Hours, timeDifference.Minutes);

                var confirmupdate = AnsiConsole.Confirm(@$"Would you like to update record {recordToUpdate} to:
                    [blue]Start Time[/]: {updatedStartTimeString}
                    [green]End Time[/]: {endTime.ToString("HH:mm")}
                    [teal]Duration[/]: {duration}");

                if(confirmupdate)
                {
                    db.UpdateRecord(Enums.tableFields.startTime, recordToUpdate, updatedStartTimeString);
                    db.UpdateRecord(Enums.tableFields.duration, recordToUpdate, duration);
                }
                else
                {
                    AnsiConsole.Clear();
                    return;
                }
                break;

            case tableFields.endTime:
                DateTime startTime = Convert.ToDateTime(db.GetRecord(recordToUpdate).startTime);

                DateTime updatedEndTime;
                string updatedEndTimeString = AnsiConsole.Ask<string>("Please enter the updated [green]end[/] time (e.g. 09:10): ");
                while (!DateTime.TryParseExact(updatedEndTimeString, "HH:mm", new CultureInfo("en-GB"), DateTimeStyles.None, out updatedEndTime) || startTime > updatedEndTime)
                {
                    updatedEndTimeString = AnsiConsole.Ask<string>("Invalid time format. Please enter the updated [green]end[/] time(e.g. 09:10): ");
                }

                timeDifference = updatedEndTime - startTime;
                duration = string.Format("{0:00}:{1:00}", timeDifference.Hours, timeDifference.Minutes);

                confirmupdate = AnsiConsole.Confirm(@$"Would you like to update record {recordToUpdate} to:
                    [blue]Start Time[/]: {startTime.ToString("HH:mm")}
                    [green]End Time[/]: {updatedEndTimeString}
                    [teal]Duration[/]: {duration}");

                if (confirmupdate)
                {
                    db.UpdateRecord(Enums.tableFields.endTime, recordToUpdate, updatedEndTimeString);
                    db.UpdateRecord(Enums.tableFields.duration, recordToUpdate, duration);
                }
                else
                {
                    AnsiConsole.Clear();
                    return;
                }
                break;

            case tableFields.date:
                var updatedDateString = AnsiConsole.Ask<string>("Please enter the updated [red]date[/] (e.g. 02/06/2025): ");
                while (!DateTime.TryParseExact(updatedDateString, "dd/MM/yyyy", new CultureInfo("en-GB"), DateTimeStyles.None, out _))
                {
                    updatedDateString = AnsiConsole.Ask<string>("Invalid date format. Please enter the updated [red]date[/] (e.g. 02/06/2025): ");
                }

                confirmupdate = AnsiConsole.Confirm(@$"Would you like to update [yellow]record[/] {recordToUpdate} to:
                    [red]Date[/]: {updatedDateString}");

                if (confirmupdate)
                {
                    db.UpdateRecord(Enums.tableFields.date, recordToUpdate, updatedDateString);
                }
                else
                {
                    AnsiConsole.Clear();
                    return;
                }

                    break;
        }
        AnsiConsole.Clear();
    }

    internal void DeleteRecord()
    {
        var records = db.GetRecords();
        var recordToDelete = AnsiConsole.Prompt(
            new SelectionPrompt<Records>()
            .Title("Which record would you like to [bold red]delete[/]?")
            .PageSize(5)
            .UseConverter(r => $"ID: {r.trackerID} | Start Time: {r.startTime} | EndTime: {r.endTime} | Duration: {r.duration} | Date: {r.date}")
            .AddChoices(records)).trackerID ?? 0;

        var confimDelete = AnsiConsole.Confirm($"Are you sure you would like to [bold red]delete[/] record {recordToDelete}");
        if (confimDelete)
        {
            db.DeleteRecord(recordToDelete);
        }
        else
        {
            AnsiConsole.Clear();
            return;
        }
        AnsiConsole.Clear() ;
    }

    internal void Stopwatch()
    {
        DateTime startDateTime = DateTime.Now;
        string date = startDateTime.ToShortDateString();
        string startTime = startDateTime.ToShortTimeString();

        string duration = StopwatchUI();

        string endTime = DateTime.Now.ToShortTimeString();

        AnsiConsole.MarkupLine(@$"Creating record:
            [blue]Start time[/]: {startTime}
            [green]End time[/]: {endTime}
            [teal]Duration[/]: {duration}
            [red]Date[/]: {date}");

        db.AddRecord(startTime, endTime, duration, date);

        AnsiConsole.MarkupLine("Press [green]ENTER[/] to return to main menu");
        Console.ReadKey();
        AnsiConsole.Clear();
    }

    internal string StopwatchUI()
    {
        Stopwatch stopWatch = new Stopwatch();
        stopWatch.Start();

        TimeSpan ts = stopWatch.Elapsed;
        var table = new Table().BorderColor(Color.DarkGreen).Expand().Border(TableBorder.HeavyEdge);
        table.AddColumn("[DeepPink2]Stopwatch[/]", r => r.Centered());
        table.AddRow($"{ts.Hours:00} hours {ts.Minutes:00} minutes {ts.Seconds:00} seconds");

        Console.CursorVisible = false;
        var top = Console.CursorTop;

        bool view = true;
        while (view)
        {
            ts = stopWatch.Elapsed;
            table.UpdateCell(0, 0, $"{ts.Hours:00} hours {ts.Minutes:00} minutes {ts.Seconds:00} seconds");
            Console.SetCursorPosition(0, top);
            AnsiConsole.Write(table);
            AnsiConsole.MarkupLine("\nPress [green]ENTER[/] to stop the stopwatch");
            if (Console.KeyAvailable)
            {
                var key = Console.ReadKey(true);
                if (key.Key == ConsoleKey.Enter)
                {
                    stopWatch.Stop();
                    string paused = AnsiConsole.Prompt(
                        new SelectionPrompt<string>()
                        .Title("[DarkOrange3_1]Stopwatch paused[/]")
                        .AddChoices("Unpause", "End Stopwatch"));
                    if (paused == "End Stopwatch")
                    {
                        return $"{ts.Hours:00}:{ts.Minutes:00}";
                    }
                    else
                    {
                        stopWatch.Start();
                        Console.CursorVisible = false;
                    }
                }
            }
        }
        Console.CursorVisible = true;
        return $"{ts.Hours:00}:{ts.Minutes:00}";
    }
}