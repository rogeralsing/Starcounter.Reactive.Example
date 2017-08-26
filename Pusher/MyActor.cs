using System;
using System.Threading.Tasks;
using Proto;

namespace Pusher
{
    public class MyActor : IActor
    {
        private readonly Pushable<MySource> _pushable;

        public MyActor(Pushable<MySource> pushable)
        {
            _pushable = pushable;
        }

        public async Task ReceiveAsync(IContext context)
        {
            switch (context.Message)
            {
                case Started _:
                {
                    context.SetReceiveTimeout(TimeSpan.FromSeconds(1));
                    break;
                }
                case ReceiveTimeout _:
                {
                    await _pushable.ApplyAsync(ts =>
                    {
                        ts.Time = DateTime.Now + " from Proto.Actor";
                    });
                    break;
                }
            }
        }
    }
}