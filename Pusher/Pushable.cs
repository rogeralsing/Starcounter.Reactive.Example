using System;
using System.Threading.Tasks;
using Starcounter;

namespace Pusher
{
    public static class Pushable
    {
        public static Pushable<T> Create<T>(T data)
        {
            return new Pushable<T>(data);
        }
    }

    public class Pushable<T>
    {
        public string SessionId { get; }
        public T Value { get; }

        public Pushable(T data) : this(Session.Current.SessionId, data)
        {
        }

        public Pushable(string sessionId, T value)
        {
            SessionId = sessionId;
            Value = value;
        }

        public Task ApplyAsync(Action<T> action)
        {
            var ts = new TaskCompletionSource<bool>();
            Session.ScheduleTask(SessionId, (s, i) =>
            {
                action(Value);
                s.CalculatePatchAndPushOnWebSocket();
                ts.SetResult(true);
            });
            return ts.Task;
        }

        public void Apply(Action<T> action)
        {
            Session.ScheduleTask(SessionId, (s, i) =>
            {
                action(Value);
                s.CalculatePatchAndPushOnWebSocket();
            }, true);
        }
    }
}