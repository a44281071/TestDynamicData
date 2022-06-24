using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DynamicData;

namespace TestDynamicData
{
    public class DataAccessor
    {
        public DataAccessor()
        {
            CountryCache = new SourceCache<CountryInfo, Guid>(dd => dd.Id);
            InsertCountry();
            InsertCountry();
        }

        public Guid? SavedCountryId { get; set; }
        public ISourceCache<CountryInfo, Guid> CountryCache { get; }

        public void InsertCountry()
        {
            var cc = new CountryInfo
            {
                Id = Guid.NewGuid(),
                Name = Path.GetRandomFileName()
            };
            CountryCache.AddOrUpdate(cc);
        }
    }
}