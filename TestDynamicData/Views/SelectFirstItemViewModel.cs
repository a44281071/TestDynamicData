using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Caliburn.Micro;
using DynamicData;
using DynamicData.Binding;

namespace TestDynamicData.Views
{
    /// <summary>
    /// How to auto select first item?
    /// </summary>
    /// <see href=""="https://github.com/reactivemarbles/DynamicData/discussions/611"/>
    public class SelectFirstItemViewModel : Screen
    {
        public SelectFirstItemViewModel(DataAccessor dataAccessor)
        {
            this.dataAccessor = dataAccessor;
        }

        private readonly DataAccessor dataAccessor;

        private CompositeDisposable? close_clean_handler;

        /// <summary>
        /// Bindable country collection.
        /// </summary>
        public ObservableCollectionExtended<CountryViewModel> CountryItems { get; } = new();

        /// <summary>
        /// Selected item.
        /// </summary>
        public CountryViewModel CurrentCountry { get; set; }

        private IDisposable CreateCountryBind()
        {
            var handle = dataAccessor.CountryCache.Connect()
                .Transform(dd => new CountryViewModel(dd))
                .Sort(SortExpressionComparer<CountryViewModel>.Ascending(p => p.Info.Name))
                .ObserveOnDispatcher()
                .Bind(CountryItems)
                .Subscribe();

            return handle;
        }

        private void SelectDefaultItem()
        {
            // get saved data, and set display.
            CurrentCountry = CountryItems.FirstOrDefault(dd => dd.Id == dataAccessor.SavedCountryId);
        }

        protected override async Task OnInitializeAsync(CancellationToken cancellationToken)
        {
            var handlers = close_clean_handler ??= new();
            handlers.Add(CreateCountryBind());

            // MUST wait obser filling countries.
            // await Task.Delay(1000);
            // MUST wait obser filling countries.

            SelectDefaultItem();

            await base.OnInitializeAsync(cancellationToken);
        }

        protected override Task OnDeactivateAsync(bool close, CancellationToken cancellationToken)
        {
            if (close)
            {
                dataAccessor.SavedCountryId = CurrentCountry?.Id;

                close_clean_handler?.Dispose();
                close_clean_handler = null;
            }
            return base.OnDeactivateAsync(close, cancellationToken);
        }
    }
}