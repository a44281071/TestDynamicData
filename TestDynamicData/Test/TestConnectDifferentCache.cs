using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DynamicData;
using DynamicData.Binding;

namespace TestDynamicData.Test
{
    internal class TestConnectDifferentCache
    {
        public TestConnectDifferentCache()
        {
            var source = new SourceCache<Person, int>(x => x.Age);
            IDisposable d1 = source.Connect()
                 .Bind(Items)
                 .Subscribe();

            var person = new Person("A", 1);
            source.AddOrUpdate(person);

            d1.Dispose();
            Trace.TraceInformation("d1 Item name = {0}", Items[0].Name);

            var person2 = new Person("B", 2);
            source.Edit(ul => {
                source.Clear();
                source.AddOrUpdate(person2);
            });
            IDisposable d2 = source.Connect()
               .Bind(Items)
               .Subscribe();

            Trace.TraceInformation("d1 Item name = {0}", Items[0].Name);
        }

        public ObservableCollectionExtended<Person> Items { get; } = new();

    }
}
