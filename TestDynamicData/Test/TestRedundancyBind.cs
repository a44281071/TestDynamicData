using System;
using System.Diagnostics;
using DynamicData;
using DynamicData.Binding;

namespace TestDynamicData.Test
{
    /// <summary>
    /// Hard to debug bind() Redundancy. Multiple subcribe will cause error.
    /// </summary>
    internal class TestRedundancyBind
    {
        public TestRedundancyBind()
        {
            var source = new SourceCache<Person, int>(x => x.Age);
            source.Connect()
                .Bind(Items)
                .Subscribe();

            var person = new Person("A", 1);
            var person2 = new Person("B", 2);
            source.AddOrUpdate(person);
            source.AddOrUpdate(person2);

            var source2 = new SourceCache<Person, int>(x => x.Age);
            source2.Connect()
                .Bind(Items)
                .Subscribe();
            var person3 = new Person("C", 3);
            var person4 = new Person("D", 4);
            source2.Edit(ul =>
            {
                source.AddOrUpdate(person3);
                source.AddOrUpdate(person4);
            });

            Trace.TraceInformation("d1 Item name = {0}", Items[0].Name);
        }

        public ObservableCollectionExtended<Person> Items { get; } = new();
    }
}