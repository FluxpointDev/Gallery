using RethinkDb.Driver.Model;
using RethinkDb.Driver.Net;
using System;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace Gallery.Database
{
    public class RethinkUpdaterService
    {
        public void Start()
        {
            Task.Run(() =>
            {
                Cursor<Change<RethinkUser>> changes = DB.R.Table("Users")
                           .Changes()
                           .RunChanges<RethinkUser>(DB.Con);

                IObservable<Change<RethinkUser>> observable = changes.ToObservable();
                observable.SubscribeOn(NewThreadScheduler.Default)
                     .Subscribe(
                         x => OnNext(x)
                     );
            });
        }
        public void OnNext(Change<RethinkUser> m)
        {
            if (m.OldValue == null || m.NewValue == null || !DB.Users.ContainsKey(m.OldValue.ID))
                return;
            DB.Users[m.OldValue.ID] = m.NewValue;
        }
    }
}
