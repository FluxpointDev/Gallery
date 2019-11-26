using RethinkDb.Driver.Model;
using RethinkDb.Driver.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace Gallery.Database
{
    public static class UserFeeds
    {
        public static void Setup()
        {
            Task.Run(() =>
            {
                Cursor<Change<ApiUser>> changes = DB.R.Db("API").Table("Users")
                                .Changes()
                                .RunChanges<ApiUser>(DB.Con);

                IObservable<Change<ApiUser>> observable = changes.ToObservable();
                observable.SubscribeOn(NewThreadScheduler.Default)
                     .Subscribe(
                         x => OnNext(x)
                     );
            });
        }
        private static async Task OnNext(Change<ApiUser> m)
        {
            if (m.OldValue == null && m.NewValue != null)
                DB.Keys.Add(m.NewValue.Token, m.NewValue);
            else if (m.OldValue != null && m.NewValue != null)
            {
                if (m.OldValue.Token == m.NewValue.Token)
                    DB.Keys[m.NewValue.Token] = m.NewValue;
                else
                {
                    DB.Keys.Add(m.NewValue.Token, m.NewValue);
                    try
                    {
                        DB.Keys.Remove(m.OldValue.Token);
                    }
                    catch { }
                }
            }
        }
    }
}
