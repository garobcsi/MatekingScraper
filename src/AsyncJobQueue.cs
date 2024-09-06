using System.Collections.Concurrent;

namespace MathScraper;

public class AsyncJobQueue
{
    private readonly ConcurrentDictionary<int, (Func<int, CancellationToken, Task> job, Task task, CancellationTokenSource cts)> _jobs = new();
    private SemaphoreSlim _semaphore = null!;
    private bool _concurrencyLimitEnabled;
    private int _maxConcurrentJobs;
    private int _currentJobId;
    private int _failedJobsCount;

    public AsyncJobQueue(bool concurrencyLimitEnabled = false, int maxConcurrentJobs = 10)
    {
        _concurrencyLimitEnabled = concurrencyLimitEnabled;
        _maxConcurrentJobs = maxConcurrentJobs;
        _currentJobId = 0;
        _failedJobsCount = 0;

        if (_concurrencyLimitEnabled)
        {
            _semaphore = new SemaphoreSlim(_maxConcurrentJobs, _maxConcurrentJobs);
        }
    }

    public int AddJob(Func<int, CancellationToken, Task> job)
    {
        int jobId = Interlocked.Increment(ref _currentJobId);
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

    private async Task ProcessJob(int jobId, Func<int, CancellationToken, Task> job, CancellationToken token)
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
            Interlocked.Increment(ref _failedJobsCount);  // Increment the failed jobs counter
        }
        finally
        {
            RemoveJob(jobId);
        }
    }

    private void RemoveJob(int jobId)
    {
        if (!_jobs.TryRemove(jobId, out var jobEntry)) return;
        jobEntry.cts.Cancel();
        jobEntry.cts.Dispose();
    }

    public void CancelJob(int jobId)
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

    public int AddedJobsCount => _jobs.Count;

    public int FailedJobsCount => _failedJobsCount;

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