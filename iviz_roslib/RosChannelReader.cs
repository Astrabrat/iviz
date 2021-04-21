using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Iviz.Msgs;
using Iviz.XmlRpc;
using Nito.AsyncEx;

namespace Iviz.Roslib
{
    public abstract class BaseRosChannelReader<T> : IEnumerable<T>, IRosChannelReader
#if !NETSTANDARD2_0
        , IAsyncEnumerable<T>
#endif
        where T : IMessage
    {
        readonly ConcurrentQueue<T> backQueue = new();
        protected readonly AsyncCollection<T> messageQueue;
        protected bool disposed;
        protected string? subscriberId;
        protected IRosSubscriber<T>? subscriber;
        protected CancellationTokenRegistration subscriberToken;

        /// <summary>
        /// Tentative number of elements. This number may become outdated right after calling this property.
        /// Use this only as an estimate for the number of elements.
        /// </summary>
        public int Count => backQueue.Count;

        public IRosSubscriber<T> Subscriber =>
            subscriber ?? throw new InvalidOperationException("Channel has not been started!");

        public string Topic => Subscriber.Topic;

        public bool IsAlive => subscriber != null && !disposed;

        public BaseRosChannelReader()
        {
            messageQueue = new AsyncCollection<T>(backQueue);
        }

        /// <summary>
        /// Starts the channel. Must be called after the constructor.
        /// </summary>
        /// <param name="client">A connected RosClient</param>
        /// <param name="topic">The topic to listen to</param>
        /// <param name="token">An optional cancellation token.</param>
        /// <exception cref="ArgumentNullException">Thrown if the client or the topic are null</exception>
        public abstract Task StartAsync(IRosClient client, string topic, CancellationToken token = default);

        /// <summary>
        /// Starts the channel. Must be called after the constructor.
        /// </summary>
        /// <param name="client">A connected RosClient</param>
        /// <param name="topic">The topic to listen to</param>
        /// <exception cref="ArgumentNullException">Thrown if the client or the topic are null</exception>        
        public abstract void Start(IRosClient client, string topic);

        public async Task DisposeAsync()
        {
            if (disposed)
            {
                return;
            }

            disposed = true;
            messageQueue.CompleteAdding();

            if (subscriber == null)
            {
                return; // not started
            }


#if !NETSTANDARD2_0
            await subscriberToken.DisposeAsync();
#else
            subscriberToken.Dispose();
#endif
            await subscriber.UnsubscribeAsync(subscriberId!).AsTask().AwaitNoThrow(this);
        }

#if !NETSTANDARD2_0
        async ValueTask IAsyncDisposable.DisposeAsync()
        {
            await DisposeAsync();
        }
#endif

        public void Dispose()
        {
            if (disposed)
            {
                return;
            }

            disposed = true;
            messageQueue.CompleteAdding();

            if (subscriber == null)
            {
                return; // not started
            }

            try
            {
                subscriberToken.Dispose();
                subscriber.Unsubscribe(subscriberId!);
            }
            catch (Exception e)
            {
                Logger.LogErrorFormat("{0}: Error in Dispose: {1}", this, e);
            }
        }

        /// <summary>
        /// Enumerates through the available messages, without blocking.
        /// </summary>
        /// <returns>An enumerator that can be used in a foreach</returns>
        public IEnumerator<T> GetEnumerator()
        {
            return ReadAll().GetEnumerator();
        }

        /// <summary>
        /// Enumerates through the available messages, without blocking.
        /// </summary>
        /// <returns>An enumerator that can be used in a foreach</returns>        
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        protected void OnSubscriberDisposed()
        {
            messageQueue.CompleteAdding();
        }

        /// <summary>
        /// Waits until a message arrives.
        /// </summary>
        /// <returns>False if the channel has been disposed</returns>
        public bool WaitToRead(int timeoutInMs)
        {
            using CancellationTokenSource ts = new(timeoutInMs);
            try
            {
                return WaitToRead(ts.Token);
            }
            catch (OperationCanceledException)
            {
                throw new TimeoutException("Wait to read timed out");
            }
        }

        /// <summary>
        /// Waits until a message arrives.
        /// </summary>
        /// <param name="token">A cancellation token that makes the function stop blocking when cancelled. If not provided, waits indefinitely.</param>
        /// <returns>False if the channel has been disposed</returns>
        public bool WaitToRead(CancellationToken token = default)
        {
            return messageQueue.OutputAvailable(token);
        }

        /// <summary>
        /// Waits until a message arrives.
        /// </summary>
        /// <returns>False if the channel has been disposed</returns>
        public async ValueTask<bool> WaitToReadAsync(int timeoutInMs)
        {
            using CancellationTokenSource ts = new(timeoutInMs);
            try
            {
                return await WaitToReadAsync(ts.Token);
            }
            catch (OperationCanceledException)
            {
                throw new TimeoutException("Wait to read timed out");
            }
        }

        /// <summary>
        /// Waits until a message arrives.
        /// </summary>
        /// <param name="token">A cancellation token that makes the function stop blocking when cancelled. If not provided, waits indefinitely.</param>
        /// <returns>False if the channel has been disposed</returns>
        public ValueTask<bool> WaitToReadAsync(CancellationToken token = default)
        {
            return new(messageQueue.OutputAvailableAsync(token));
        }


        /// <summary>
        /// Waits a given time until a message arrives, and pulls it from the queue.
        /// </summary>
        /// <param name="timeoutInMs">The maximal time to wait</param>
        /// <returns>The message that arrived</returns>
        /// <exception cref="OperationCanceledException">Thrown if the waiting times out</exception>
        /// <exception cref="InvalidOperationException">Thrown if the queue has been disposed</exception>
        public T Read(int timeoutInMs)
        {
            using CancellationTokenSource ts = new(timeoutInMs);
            try
            {
                return Read(ts.Token);
            }
            catch (OperationCanceledException)
            {
                throw new TimeoutException("Wait for read timed out");
            }
        }

        void ThrowIfNotStarted()
        {
            if (subscriber == null)
            {
                throw new InvalidOperationException("Channel has not been started");
            }
        }

        /// <summary>
        /// Waits until a message arrives, and pulls it from the queue.
        /// </summary>
        /// <param name="token">A cancellation token that makes the function stop blocking when cancelled. If not provided, waits indefinitely.</param>
        /// <returns>The message that arrived.</returns>
        /// <exception cref="OperationCanceledException">Thrown if the token is canceled</exception>
        /// <exception cref="InvalidOperationException">Thrown if the queue has been disposed</exception>
        public T Read(CancellationToken token = default)
        {
            ThrowIfNotStarted();
            return messageQueue.Take(token);
        }

        /// <summary>
        /// Awaits a given time until a message arrives, and pulls it from the queue.
        /// </summary>
        /// <param name="timeoutInMs">The maximal time to wait</param>
        /// <returns>The message that arrived</returns>
        /// <exception cref="OperationCanceledException">Thrown if the waiting times out</exception>
        /// <exception cref="InvalidOperationException">Thrown if the queue has been disposed</exception>
        public async ValueTask<T> ReadAsync(int timeoutInMs)
        {
            using CancellationTokenSource ts = new(timeoutInMs);
            try
            {
                return await ReadAsync(ts.Token);
            }
            catch (OperationCanceledException)
            {
                throw new TimeoutException("Wait for read timed out");
            }
        }

        /// <summary>
        /// Awaits until a message arrives, and pulls it from the queue.
        /// </summary>
        /// <param name="token">A cancellation token that makes the function stop blocking when cancelled. If not provided, waits indefinitely.</param>
        /// <returns>The message that arrived.</returns>
        /// <exception cref="OperationCanceledException">Thrown if the token is canceled</exception>
        /// <exception cref="InvalidOperationException">Thrown if the queue has been disposed</exception>
        public ValueTask<T> ReadAsync(CancellationToken token = default)
        {
            ThrowIfNotStarted();
            return new ValueTask<T>(messageQueue.TakeAsync(token));
        }


        /// <summary>
        /// Checks if there is a message in the queue, and returns it without blocking.
        /// </summary>
        /// <param name="t">The received message, or default if no message was available.</param>
        /// <returns>True if there was a message available.</returns>
        /// <exception cref="InvalidOperationException">Thrown if the queue has been disposed</exception>
        public bool TryRead(out T t)
        {
            ThrowIfNotStarted();

            if (backQueue.Count == 0)
            {
                t = default!;
                return false;
            }

            try
            {
                t = messageQueue.Take(new CancellationToken(true));
                return true;
            }
            catch (OperationCanceledException)
            {
                // this shouldn't happen unless multiple reads get called at the same time
                t = default!;
                return false;
            }
        }


        /// <summary>
        /// Enumerates through the available messages, and blocks while waiting for the next.
        /// It will only return either when the token has been canceled, or the channel has been disposed.
        /// </summary>
        /// <param name="token">An optional cancellation token.</param>
        /// <returns>An enumerator that can be used in a foreach</returns>
        /// <exception cref="InvalidOperationException">Thrown if the queue has been disposed</exception>
        public IEnumerable<T> ReadAll(CancellationToken token = default)
        {
            while (true)
            {
                yield return Read(token);
            }
        }

        public IEnumerable<T> TryReadAll()
        {
            while (true)
            {
                if (backQueue.Count == 0)
                {
                    yield break;
                }

                T element;
                try
                {
                    element = messageQueue.Take(new CancellationToken(true));
                }
                catch (OperationCanceledException)
                {
                    // this shouldn't happen unless multiple reads get called at the same time
                    yield break;
                }

                yield return element!;
            }
        }

#if !NETSTANDARD2_0
        /// <summary>
        /// Enumerates through the available messages, and blocks while waiting for the next.
        /// It will only return either when the token has been canceled, or the channel has been disposed.
        /// </summary>
        /// <param name="token">A cancellation token that makes the function stop blocking when cancelled.</param>
        /// <returns>An enumerator that can be used in a foreach</returns>
        /// <exception cref="InvalidOperationException">Thrown if the queue has been disposed</exception>
        public async IAsyncEnumerable<T> ReadAllAsync(
            [EnumeratorCancellation] CancellationToken token = default)
        {
            ThrowIfNotStarted();
            while (true)
            {
                yield return await messageQueue.TakeAsync(token);
            }
        }

        public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken token = default)
        {
            return ReadAllAsync(token).GetAsyncEnumerator(token);
        }
#endif

        IMessage IRosChannelReader.Read(CancellationToken token)
        {
            return Read(token);
        }

        async ValueTask<IMessage> IRosChannelReader.ReadAsync(CancellationToken token)
        {
            return await ReadAsync(token);
        }

        IEnumerable<IMessage> IRosChannelReader.TryReadAll()
        {
            return TryReadAll().Cast<IMessage>();
        }

        IEnumerable<IMessage> IRosChannelReader.ReadAll(CancellationToken token)
        {
            return ReadAll(token).Cast<IMessage>();
        }

#if !NETSTANDARD2_0
        async IAsyncEnumerable<IMessage> IRosChannelReader.ReadAllAsync(
            [EnumeratorCancellation] CancellationToken token)
        {
            ThrowIfNotStarted();
            while (true)
            {
                yield return await messageQueue.TakeAsync(token);
            }
        }
#endif

        public override string ToString()
        {
            if (subscriber == null)
            {
                return "[RosChannelReader (uninitialized)]";
            }

            return disposed
                ? "[RosChannelReader (disposed)]"
                : $"[RosChannelReader {subscriber.Topic} [{subscriber.TopicType}]]";
        }
    }

    /// <summary>
    /// A helper class that wraps a subscriber. It employs a queue that stores messages
    /// in the background, and can be accessed without having to use a separate callback.
    /// </summary>
    /// <typeparam name="T">The message type</typeparam>
    public sealed class RosChannelReader<T> : BaseRosChannelReader<T>
        where T : IMessage, IDeserializable<T>, new()
    {
        public RosChannelReader()
        {
        }

        public RosChannelReader(IRosClient client, string topic)
        {
            Start(client, topic);
        }

        public override async Task StartAsync(IRosClient client, string topic, CancellationToken token = default)
        {
            if (client == null)
            {
                throw new ArgumentNullException(nameof(client));
            }

            if (subscriber != null)
            {
                throw new InvalidOperationException("Channel has already been started!");
            }

            var (newId, newSubscriber) = await client.SubscribeAsync<T>(topic, Callback, token: token);

            subscriberId = newId;
            subscriber = newSubscriber;
            subscriberToken = subscriber.CancellationToken.Register(OnSubscriberDisposed);
        }

        public override void Start(IRosClient client, string topic)
        {
            if (client == null)
            {
                throw new ArgumentNullException(nameof(client));
            }

            subscriberId = client.Subscribe(topic, Callback, out subscriber);
            subscriberToken = subscriber.CancellationToken.Register(OnSubscriberDisposed);
        }

        void Callback(T t)
        {
            if (disposed)
            {
                return;
            }

            messageQueue.Add(t);
        }
    }

    /// <summary>
    /// A helper class that wraps a subscriber. It employs a queue that stores messages
    /// in the background, and can be accessed without having to use a separate callback.
    /// Similar to <see cref="RosChannelReader{T}"/>, but it uses whatever message type the
    /// publisher offers in the connection. Note that if a publisher accidentally offers a wrong
    /// message type in the same topic, you will receive those messages too. 
    /// </summary>
    public sealed class RosChannelReader : BaseRosChannelReader<IMessage>
    {
        public RosChannelReader()
        {
        }

        public RosChannelReader(IRosClient client, string topic)
        {
            Start(client, topic);
        }

        public override async Task StartAsync(IRosClient client, string topic, CancellationToken token = default)
        {
            if (client == null)
            {
                throw new ArgumentNullException(nameof(client));
            }

            if (subscriber != null)
            {
                throw new InvalidOperationException("Channel has already been started!");
            }

            var (newId, newSubscriber) = await client.SubscribeAsync(topic, Callback, token: token);

            subscriberId = newId;
            subscriber = (RosSubscriber<IMessage>) newSubscriber;
            subscriberToken = subscriber.CancellationToken.Register(OnSubscriberDisposed);
        }

        public override void Start(IRosClient client, string topic)
        {
            if (client == null)
            {
                throw new ArgumentNullException(nameof(client));
            }

            subscriberId = client.Subscribe(topic, Callback, out var newSubscriber);
            subscriber = (RosSubscriber<IMessage>) newSubscriber;
            subscriberToken = subscriber.CancellationToken.Register(OnSubscriberDisposed);
        }

        void Callback(IMessage t)
        {
            if (disposed)
            {
                return;
            }

            messageQueue.Add(t);
        }
    }

    public static class RosChannelReaderUtils
    {
        public static RosChannelReader CreateReader(this IRosClient client, string topic)
        {
            return new RosChannelReader(client, topic);
        }

        public static async ValueTask<RosChannelReader<T>> CreateReaderAsync<T>(this IRosClient client, string topic,
            CancellationToken token = default)
            where T : IMessage, IDeserializable<T>, new()
        {
            var writer = new RosChannelReader<T>();
            await writer.StartAsync(client, topic, token);
            return writer;
        }

        public static async ValueTask<RosChannelReader> CreateReaderAsync(this IRosClient client, string topic,
            CancellationToken token = default)
        {
            var writer = new RosChannelReader();
            await writer.StartAsync(client, topic, token);
            return writer;
        }

        public static IRosChannelReader CreateInstance(Type msgType)
        {
            if (typeof(IMessage) == msgType || !typeof(IMessage).IsAssignableFrom(msgType))
            {
                throw new ArgumentException("msgType is not a message type", nameof(msgType));
            }

            Type readerType = typeof(RosChannelReader<>).MakeGenericType(msgType);
            return (IRosChannelReader) Activator.CreateInstance(readerType)!;
        }
    }
}