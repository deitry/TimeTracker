using System.Runtime.CompilerServices;

namespace TimeTracker;

public static class AsyncUtil
{
    public readonly struct SynchronizationContextAwaiter : INotifyCompletion
    {
        private static readonly SendOrPostCallback _postCallback = state => (state as Action)?.Invoke();

        private readonly SynchronizationContext _context;

        public SynchronizationContextAwaiter(SynchronizationContext context)
        {
            _context = context;
        }

        public bool IsCompleted => _context == SynchronizationContext.Current;

        public void OnCompleted(Action continuation) => _context.Post(_postCallback, continuation);

        public void GetResult()
        {
        }
    }

    public static SynchronizationContextAwaiter GetAwaiter(this SynchronizationContext context)
    {
        if (context == null)
            throw new ArgumentNullException(nameof(context));

        return new SynchronizationContextAwaiter(context);
    }
}
