namespace TaskExtensions;

public static class TaskExtensions
{
    extension<T>(Task<T> task)
    {
        public async Task<T> WithTimeout(TimeSpan timeout)
        {
            using var cts = new CancellationTokenSource(timeout);
            var completedTask = await Task.WhenAny(task, Task.Delay(timeout, cts.Token));

            if (completedTask == task)
            {
                cts.Cancel(); // Cancel the delay task
                return await task;
            }

            throw new TimeoutException($"Task timed out after {timeout}");
        }

        public async Task<T?> WithDefaultOnTimeout(TimeSpan timeout, T? defaultValue = default)
        {
            try
            {
                return await task.WithTimeout(timeout);
            }
            catch (TimeoutException)
            {
                return defaultValue;
            }
        }
    }

    extension<T>(IEnumerable<Task<T>> tasks)
    {
        public async Task<IEnumerable<T>> WhenAllSuccessful()
        {
            // Similar to Task.WhenAll, but returns only successful results

            var todo = tasks.ToList();
            var results = new List<T>();

            while (todo.Count != 0)
            {
                var completed = await Task.WhenAny(todo);
                todo.Remove(completed);

                try
                {
                    results.Add(await completed);
                }
                catch
                {
                    // Log error if needed, but continue with other tasks
                    Console.Error.WriteLine("Something bad has happened...");
                }
            }

            return results;
        }

        public async Task<IEnumerable<TResult>> WhenAllConverted<TResult>(Func<T, TResult> converter)
        {
            var results = await Task.WhenAll(tasks);
            return results.Select(converter);
        }
    }
}