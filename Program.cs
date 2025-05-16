using Pomodoro;
using Spectre.Console;

var running = true;

while (running)
{
    var option = AnsiConsole.Prompt(
        new SelectionPrompt<MenuOptions>()
            .Title("Menu")
            .UseConverter((option) => option.ToString())
            .AddChoices(MenuOptions.Start, MenuOptions.Quit));

    switch (option)
    {
        case MenuOptions.Start:
            await BuildSession(25, 5);
            break;
        case MenuOptions.Quit:
            running = false;
            break;
        default:
            throw new ArgumentOutOfRangeException();
    }
}

return;

async Task BuildSession(int workTime, int restTime)
{
    await BuildTimeProgress("Work", workTime, "red");
    await BuildTimeProgress("Rest", restTime, "green");
}

Task BuildTimeProgress(string name, int minutes, string color = "green") =>
    AnsiConsole.Progress()
        .Columns(new TaskDescriptionColumn(), new ProgressBarColumn(), new RemainingTimeColumn())
        .StartAsync(async ctx =>
        {
            // Define tasks
            var task1     = ctx.AddTask($"[{color}]{name}[/]");
            var startTime = task1.StartTime.Value;
            var endTime   = startTime.AddMinutes(minutes);

            while (!ctx.IsFinished)
            {
                // Simulate some work
                await Task.Delay(100);
                task1.Value = TimePercentage(startTime, endTime, task1.ElapsedTime.Value);
            }
        });

double TimePercentage(DateTime startDateTime, DateTime endDateTime, TimeSpan elapsed) =>
    elapsed.TotalMilliseconds / (endDateTime - startDateTime).TotalMilliseconds * 100;