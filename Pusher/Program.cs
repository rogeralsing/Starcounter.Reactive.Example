using System;
using System.Reactive.Linq;
using Pusher.ViewModels;
using Starcounter;
using static Proto.Actor;

namespace Pusher
{
    class Program
    {
        static void Main()
        {
            Application.Current.Use(new HtmlFromJsonProvider());
            Application.Current.Use(new PartialToStandaloneHtmlProvider());
            Handle.GET("/actors", () =>
            {
                Session.Current = new Session(SessionOptions.PatchVersioning);
                var pushable = Pushable.Create(new MySource());
                var page = new MainPage
                {
                    Data = pushable.Value,
                };

                Spawn(FromProducer(() => new MyActor(pushable)));

                return page;
            });


            Handle.GET("/rx", () =>
            {
                Session.Current = new Session(SessionOptions.PatchVersioning);
                var pushable = Pushable.Create(new MySource());
                var page = new MainPage
                {
                    Data = pushable.Value,
                };

                Observable
                    .Interval(TimeSpan.FromSeconds(1))
                    .Select(_ => DateTime.Now + " from RX")
                    .Subscribe(time =>
                    {
                        pushable.Apply(m => { m.Time = time; });
                    });

                return page;
            });
        }
    }
}