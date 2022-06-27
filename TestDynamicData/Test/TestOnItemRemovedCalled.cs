using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DynamicData;
using DynamicData.Binding;

namespace TestDynamicData
{
    public class TestOnItemRemovedCalled
    {
        public TestOnItemRemovedCalled()
        {
            OnItemRemovedCalled();
        }

        private void OnItemRemovedCalled()
        {
            var source = new SourceCache<Person, int>(x => x.Age);
            source.Connect()
                //.Bind(person_items)
                .OnItemAdded(_ => Trace.TraceInformation("OnItemAdded()"))

                // He must not trace
                .OnItemRemoved(_ => Trace.TraceInformation("OnItemRemoved(). But this is error message."))
                // He must not trace

                .OnItemUpdated((_, _) => Trace.TraceInformation("OnItemUpdated()"))
                .ForEachChange(change =>
                {
                    Trace.TraceInformation("Changed({0}) old = {1}, new = {2}", change.Reason, change.Previous, change.Current);
                })
                .Subscribe();

            var person = new Person("A", 1);
            //var person2 = new Person("A", 1);
            source.AddOrUpdate(person);
            source.AddOrUpdate(person);
        }
    }

    public class Person : AbstractNotifyPropertyChanged, IEquatable<Person>
    {
        private int _age;

        public Person()
            : this("unknown", 0, "none")
        {
        }

        public Person(string firstname, string lastname, int age, string gender = "F", string? parentName = null)
            : this(firstname + " " + lastname, age, gender, parentName)
        {
        }

        public Person(string name, int age, string gender = "F", string? parentName = null)
        {
            Name = name;
            _age = age;
            Gender = gender;
            ParentName = parentName ?? string.Empty;
        }

        public static IEqualityComparer<Person> AgeComparer { get; } = new AgeEqualityComparer();

        public static IEqualityComparer<Person> NameAgeGenderComparer { get; } = new NameAgeGenderEqualityComparer();

        public int Age
        {
            get => _age;
            set => SetAndRaise(ref _age, value);
        }

        public string Gender { get; }

        public string Key => Name;

        public string Name { get; }

        public string ParentName { get; }

        public static bool operator ==(Person left, Person right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Person left, Person right)
        {
            return !Equals(left, right);
        }

        public bool Equals(Person? other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return string.Equals(Name, other.Name);
        }

        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj.GetType() != GetType())
            {
                return false;
            }

            return Equals((Person)obj);
        }

        public override int GetHashCode()
        {
            return (Name is not null ? Name.GetHashCode() : 0);
        }

        public override string ToString()
        {
            return $"{Name}. {Age}";
        }

        private sealed class AgeEqualityComparer : IEqualityComparer<Person>
        {
            public bool Equals(Person? x, Person? y)
            {
                if (ReferenceEquals(x, y))
                {
                    return true;
                }

                if (ReferenceEquals(x, null))
                {
                    return false;
                }

                if (ReferenceEquals(y, null))
                {
                    return false;
                }

                if (x.GetType() != y.GetType())
                {
                    return false;
                }

                return x._age == y._age;
            }

            public int GetHashCode(Person obj)
            {
                return obj._age;
            }
        }

        private sealed class NameAgeGenderEqualityComparer : IEqualityComparer<Person>
        {
            public bool Equals(Person? x, Person? y)
            {
                if (ReferenceEquals(x, y))
                {
                    return true;
                }

                if (ReferenceEquals(x, null))
                {
                    return false;
                }

                if (ReferenceEquals(y, null))
                {
                    return false;
                }

                if (x.GetType() != y.GetType())
                {
                    return false;
                }

                return string.Equals(x.Name, y.Name) && x._age == y._age && string.Equals(x.Gender, y.Gender);
            }

            public int GetHashCode(Person obj)
            {
                unchecked
                {
                    var hashCode = obj.Name.GetHashCode();
                    hashCode = (hashCode * 397) ^ obj._age;
                    hashCode = (hashCode * 397) ^ obj.Gender.GetHashCode();
                    return hashCode;
                }
            }
        }
    }
}