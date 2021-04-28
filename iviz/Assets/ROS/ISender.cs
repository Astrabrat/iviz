using System;
using Iviz.Core;
using Iviz.Displays;
using Iviz.Msgs;
using JetBrains.Annotations;
using Logger = Iviz.Core.Logger;

namespace Iviz.Ros
{
    public interface ISender
    {
        [NotNull] string Topic { get; }
        [NotNull] string Type { get; }
        int Id { get; }
        RosSenderStats Stats { get; }
        int NumSubscribers { get; }
        void Stop();
        void Publish([NotNull] IMessage msg);
    }

    public sealed class Sender<T> : ISender where T : IMessage
    {
        [NotNull] static RoslibConnection Connection => ConnectionManager.Connection;

        int lastMsgBytes;
        int lastMsgCounter;
        int totalMsgCounter;

        public Sender([NotNull] string topic)
        {
            if (string.IsNullOrWhiteSpace(topic))
            {
                throw new ArgumentException("Invalid topic!", nameof(topic));
            }

            Topic = topic;
            Type = BuiltIns.GetMessageType(typeof(T));

            Logger.Info($"Advertising <b>{topic}</b> <i>[{Type}]</i>.");
            GameThread.EverySecond += UpdateStats;
            Connection.Advertise(this);
        }

        public string Topic { get; }
        public string Type { get; }
        public int Id { get; private set; }
        public RosSenderStats Stats { get; private set; }
        public int NumSubscribers => Connection.GetNumSubscribers(Topic);

        void ISender.Publish(IMessage msg)
        {
            Publish((T) msg);
        }

        public void Stop()
        {
            GameThread.EverySecond -= UpdateStats;
            Logger.Info($"Unadvertising {Topic}.");
            Connection.Unadvertise(this);
        }

        public void SetId(int id)
        {
            Id = id;
        }

        public void Publish([NotNull] in T msg)
        {
            if (msg == null)
            {
                throw new ArgumentNullException(nameof(msg));
            }

            totalMsgCounter++;
            lastMsgCounter++;
            lastMsgBytes += msg.RosMessageLength;

            Connection.Publish(this, msg);
        }

        public void Reset()
        {
            Connection.Unadvertise(this);
            Connection.Advertise(this);
        }

        void UpdateStats()
        {
            if (lastMsgCounter == 0)
            {
                Stats = new RosSenderStats();
                return;
            }

            Stats = new RosSenderStats(totalMsgCounter, lastMsgCounter, lastMsgBytes);

            ConnectionManager.ReportBandwidthUp(lastMsgBytes);

            lastMsgBytes = 0;
            lastMsgCounter = 0;
        }

        public bool TryGetResolvedTopicName(out string topicName)
        {
            return ConnectionManager.Connection.TryGetResolvedTopicName(this, out topicName);
        }
    }
}