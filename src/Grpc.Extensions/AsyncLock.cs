// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Threading;
using System.Threading.Tasks;

namespace Grpc.Extensions
{
    public sealed class AsyncLock
    {
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1);
        private readonly Releaser _releaser;
        private readonly Task<Releaser> _releaserTask;

        public AsyncLock()
        {
            _releaser = new Releaser(this);
            _releaserTask = Task.FromResult(_releaser);
        }

        public Task<Releaser> LockAsync(CancellationToken cancellationToken = default)
        {
            var wait = _semaphore.WaitAsync(cancellationToken);

            return wait.IsCompleted
                ? _releaserTask
                : wait.ContinueWith(
                    (_, state) => ((AsyncLock)state)._releaser,
                    this, CancellationToken.None,
                    TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default);
        }

        public Releaser Lock()
        {
            _semaphore.Wait();

            return _releaser;
        }

        public readonly struct Releaser : IDisposable
        {
            private readonly AsyncLock _toRelease;

            internal Releaser(AsyncLock toRelease)
            {
                _toRelease = toRelease;
            }

            public void Dispose()
                => _toRelease._semaphore.Release();
        }
    }
}