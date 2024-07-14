using System.Collections.Concurrent;

namespace MathScraper;

public class AsyncJobQueue
{
    private ConcurrentDictionary<Guid, (Func<Guid, CancellationToken, Task> job, Task task, CancellationTokenSource cts)> _jobs = new();

    public Guid AddJob(Func<Guid, CancellationToken, Task> job)
    {
        Guid jobId = Guid.NewGuid();
        CancellationTokenSource cts = new();
        Task task = ProcessJob(jobId, job, cts.Token);
        _jobs.TryAdd(jobId, (job, task, cts));
        return jobId;
    }

    private async Task ProcessJob(Guid jobId, Func<Guid, CancellationToken, Task> job, CancellationToken token)
    {
        try
        {
            await job(jobId, token);
        }
        catch (OperationCanceledException)
        {
            Console.WriteLine($"Job {jobId} was canceled.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Job {jobId} failed: {ex.Message}");
        }
        finally
        {
            RemoveJob(jobId);
        }
    }

    public void RemoveJob(Guid jobId)
    {
        if (_jobs.TryRemove(jobId, out var jobEntry))
        {
            jobEntry.cts.Cancel();
            jobEntry.cts.Dispose();
        }
    }

    public void CancelJob(Guid jobId)
    {
        if (_jobs.TryGetValue(jobId, out var jobEntry))
        {
            jobEntry.cts.Cancel();
        }
    }

    public void CancelAllJobs()
    {
        foreach (var jobId in _jobs.Keys)
        {
            CancelJob(jobId);
        }
    }

    public async Task WaitForAllJobsAsync()
    {
        await Task.WhenAll(_jobs.Values.Select(j => j.task));
    }
}