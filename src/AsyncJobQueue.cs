using System.Collections.Concurrent;

namespace MathScraper;

public class AsyncJobQueue
{
    private readonly ConcurrentDictionary<Guid, (Func<Guid, CancellationToken, Task> job, Task task, CancellationTokenSource cts)> _jobs = new();
    private SemaphoreSlim _semaphore = null!;
    private bool _concurrencyLimitEnabled;
    private int _maxConcurrentJobs;

    public AsyncJobQueue(bool concurrencyLimitEnabled = false, int maxConcurrentJobs = 10)
    {
        _concurrencyLimitEnabled = concurrencyLimitEnabled;
        _maxConcurrentJobs = maxConcurrentJobs;
        if (_concurrencyLimitEnabled)
        {
            _semaphore = new SemaphoreSlim(_maxConcurrentJobs, _maxConcurrentJobs);
        }
    }

    public Guid AddJob(Func<Guid, CancellationToken, Task> job)
    {
        Guid jobId = Guid.NewGuid();
        CancellationTokenSource cts = new();
        
        var task = Task.Run(async () =>
        {
            if (_concurrencyLimitEnabled) await _semaphore.WaitAsync(cts.Token);

            try
            {
                await ProcessJob(jobId, job, cts.Token);
            }
            finally
            {
                if (_concurrencyLimitEnabled) _semaphore.Release();
            }
        }, cts.Token);

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

    private void RemoveJob(Guid jobId)
    {
        if (!_jobs.TryRemove(jobId, out var jobEntry)) return;
        jobEntry.cts.Cancel();
        jobEntry.cts.Dispose();
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

    public int RunningJobsCount => _jobs.Count;

    public bool ConcurrencyLimitEnabled
    {
        get => _concurrencyLimitEnabled;
        set
        {
            _concurrencyLimitEnabled = value;
            if (_concurrencyLimitEnabled)
            {
                _semaphore = new SemaphoreSlim(_maxConcurrentJobs, _maxConcurrentJobs);
            }
            else
            {
                _semaphore.Dispose();
            }
        }
    }

    public int MaxConcurrentJobs
    {
        get => _maxConcurrentJobs;
        set
        {
            _maxConcurrentJobs = value;
            if (!_concurrencyLimitEnabled) return;
            _semaphore.Release();
            _semaphore = new SemaphoreSlim(_maxConcurrentJobs, _maxConcurrentJobs);
        }
    }
}