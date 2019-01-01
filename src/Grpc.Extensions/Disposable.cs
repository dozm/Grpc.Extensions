using System;

namespace Grpc.Extensions
{
    public static class Disposable
    {
        public static readonly IDisposable Empty = EmptyDisposable.Singleton;

        public static IDisposable Create(Action disposeAction)
        {
            return new AnonymousDisposable(disposeAction);
        }

        public static IDisposable CreateWithState<TState>(TState state, Action<TState> disposeAction)
        {
            return new AnonymousDisposable<TState>(state, disposeAction);
        }

        private class EmptyDisposable : IDisposable
        {
            public static EmptyDisposable Singleton = new EmptyDisposable();

            private EmptyDisposable()
            { }

            public void Dispose()
            { }
        }

        private class AnonymousDisposable : IDisposable
        {
            private bool isDisposed = false;
            private readonly Action dispose;

            public AnonymousDisposable(Action dispose)
            {
                this.dispose = dispose;
            }

            public void Dispose()
            {
                if (!isDisposed)
                {
                    isDisposed = true;
                    dispose();
                }
            }
        }

        private class AnonymousDisposable<T> : IDisposable
        {
            private bool isDisposed = false;
            private readonly T state;
            private readonly Action<T> dispose;

            public AnonymousDisposable(T state, Action<T> dispose)
            {
                this.state = state;
                this.dispose = dispose;
            }

            public void Dispose()
            {
                if (!isDisposed)
                {
                    isDisposed = true;
                    dispose(state);
                }
            }
        }
    }
}