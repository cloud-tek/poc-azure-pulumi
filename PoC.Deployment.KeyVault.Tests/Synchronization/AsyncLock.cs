namespace PoC.Deployment.KeyVault.Tests.Synchronization;

public class AsyncLock
{
  private readonly AsyncSemaphore _semaphore;
  private readonly Task<AsyncLockReleaser> _releaser;

  public AsyncLock()
  {
    _semaphore = new AsyncSemaphore(1);
    _releaser = Task.FromResult(new AsyncLockReleaser(this));
  }

  public Task<AsyncLockReleaser> LockAsync()
  {
    var wait = _semaphore.WaitAsync();
    return wait.IsCompleted
      ? _releaser
      : wait.ContinueWith((_, state) => new AsyncLockReleaser((AsyncLock)state),
        this, CancellationToken.None,
        TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default);
  }

  public readonly struct AsyncLockReleaser : IDisposable
  {
    private readonly AsyncLock _toRelease;

    internal AsyncLockReleaser(AsyncLock toRelease) { _toRelease = toRelease; }

    public void Dispose()
    {
      if (_toRelease != null)
        _toRelease._semaphore.Release();
    }
  }
}
