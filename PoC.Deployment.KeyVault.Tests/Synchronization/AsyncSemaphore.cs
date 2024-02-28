namespace PoC.Deployment.KeyVault.Tests.Synchronization;

/// <summary>
/// AsyncSemaphore synchronization primitive as found on:
/// https://blogs.msdn.microsoft.com/pfxteam/2012/02/12/building-async-coordination-primitives-part-5-asyncsemaphore/
/// </summary>
internal class AsyncSemaphore
{
  private static readonly Task _completed = Task.FromResult(true);
  private readonly Queue<TaskCompletionSource<bool>> _waiters = new Queue<TaskCompletionSource<bool>>();
  private int _currentCount;

  public AsyncSemaphore(int initialCount)
  {
    if (initialCount < 0) throw new ArgumentOutOfRangeException(nameof(initialCount));
    _currentCount = initialCount;
  }

  public void Release()
  {
    TaskCompletionSource<bool> toRelease = null!;
    lock (_waiters)
    {
      if (_waiters.Count > 0)
        toRelease = _waiters.Dequeue();
      else
        ++_currentCount;
    }

    toRelease?.SetResult(true);
  }

  public Task WaitAsync()
  {
    lock (_waiters)
    {
      if (_currentCount > 0)
      {
        --_currentCount;
        return _completed;
      }

      var waiter = new TaskCompletionSource<bool>();
      _waiters.Enqueue(waiter);
      return waiter.Task;
    }
  }
}
