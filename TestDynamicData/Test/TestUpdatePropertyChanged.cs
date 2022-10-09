using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using DynamicData;
using DynamicData.Binding;

namespace TestDynamicData.Test
{
    internal class TestUpdatePropertyChanged
    {
        public TestUpdatePropertyChanged()
        {
            var chosen_cache = scache.Connect()
                 .Publish();

            IDisposable chosen_clean = chosen_cache.Connect();
            IDisposable debug_clean = chosen_cache.WhenAnyPropertyChanged(nameof(Person.Name))
                .Subscribe(dd =>
                {
                    Trace.TraceInformation($"Person.Name changed to: {dd?.Name}");
                });
            IDisposable bind_clean = chosen_cache
                .Bind(Items)
                .Subscribe();

            var persona = new Person("A", 1);
            scache.AddOrUpdate(persona);
            var personb = new Person("B", 2);
            scache.AddOrUpdate(personb);

            // update items name.
            IDisposable tick_clean = Observable.Interval(TimeSpan.FromSeconds(1))
                .Subscribe(_ =>
                {
                    foreach (var ji in scache.Items)
                    {
                        ji.Name = System.IO.Path.GetRandomFileName();
                    }
                });

            _ = UpdatePerson2After2sAsync();
        }

        private readonly SourceCache<Person, int> scache = new(x => x.Age);

        private async Task UpdatePerson2After2sAsync()
        {
            await Task.Delay(2222);
            // update
            var persona2 = new Person("A", 1);
            scache.AddOrUpdate(persona2);
            Trace.TraceInformation($"update person 1.");
        }

        public ObservableCollectionExtended<Person> Items { get; } = new();
    }
}