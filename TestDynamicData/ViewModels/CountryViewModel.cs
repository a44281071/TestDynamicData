using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;

namespace TestDynamicData
{
    public class CountryViewModel : PropertyChangedBase
    {
        public CountryViewModel(CountryInfo info)
        {
            Info = info;
        }

        public CountryInfo Info { get; }

        public Guid Id => Info.Id;
    }
}