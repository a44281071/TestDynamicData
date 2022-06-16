using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using DynamicData;
using DynamicData.Binding;
using TheRx;

namespace TestDynamicData
{
    public class ShellViewModel : Screen, IShell
    {
        public ShellViewModel()
        {
            //pager = new BehaviorSubject<PageRequest>(new PageRequest(FIRST_PAGE, PAGE_SIZE));
            var pager = PageParameters.WhenChanged(vm => vm.PageSize, vm => vm.CurrentPage, (_, size, pge) => new PageRequest(pge, size))
                .StartWith(new PageRequest(FIRST_PAGE, PAGE_SIZE))
                .DistinctUntilChanged()
                .Sample(TimeSpan.FromMilliseconds(100));

            var chosen = itemsCache.Connect()

                // WhenAnyPropertyChanged() did'nt work, because Transform()
                .Transform(dd => new UserViewModel(dd))
                // WhenAnyPropertyChanged() did'nt work, because Transform()

                .AutoRefresh(p => p.IsCamOn)
                .Sort(SortExpressionComparer<UserViewModel>.Descending(e => e.IsCamOn))
                .Page(pager)
                .Do(change => PageParameters.Update(change.Response))
                .ObserveOnDispatcher();

            chosen.OnItemAdded(OnItemAdded)
                .OnItemRemoved(OnItemRemoved)
                .OnItemRefreshed(OnItemRefreshed)
                .OnItemUpdated(OnItemUpdated)
                .Subscribe();

            // warn.
            // WhenAnyPropertyChanged() did'nt work, because Transform()
            // warn.
            chosen.WhenAnyPropertyChanged(nameof(UserViewModel.IsCamOn))
                .Subscribe(dd => Trace.TraceInformation("WhenAnyPropertyChanged() name = {0}", dd!.Info.Name));

            // items collection
            chosen.Bind(Items)
                .Subscribe();

            // all items collection
            itemsCache.Connect()
                .Transform(dd => new UserViewModel(dd))
                .ObserveOnDispatcher()
                .Bind(AllItems)
                .Subscribe();
        }

        private const int PAGE_SIZE = 5;
        private const int FIRST_PAGE = 1;

        private readonly SourceCache<UserInfo, string> itemsCache = new(dd => dd.Id);

        public ObservableCollectionExtended<UserViewModel> Items { get; } = new();
        public ObservableCollectionExtended<UserViewModel> AllItems { get; } = new();

        public PageParameterData PageParameters { get; } = new PageParameterData(FIRST_PAGE, PAGE_SIZE);

        private void OnItemUpdated(UserViewModel arg1, UserViewModel arg2)
        {
            Trace.TraceInformation("OnItemUpdated {0}, {1}", arg1.Id, arg2.Id);
        }

        private void OnItemRefreshed(UserViewModel obj)
        {
            Trace.TraceInformation("OnItemRefreshed {0}", obj.Id);
        }

        private void OnItemRemoved(UserViewModel obj)
        {
            Trace.TraceInformation("OnItemRemoved {0}", obj.Id);
            // TODO: close camera display.
        }

        private void OnItemAdded(UserViewModel obj)
        {
            Trace.TraceInformation("OnItemAdded {0}", obj.Id);
            // TODO: open camera display.
        }

        public void ChangeAnyProperty()
        {
            if (Items.FirstOrDefault() is UserViewModel uvm)
            {
                uvm.IsMicOn = !uvm.IsMicOn;
                uvm.IsCamOn = !uvm.IsCamOn;
            }
        }

        public void Add()
        {
            var data = new UserInfo
            {
                Id = Path.GetRandomFileName(),
                Name = Path.GetRandomFileName()
            };
            itemsCache.AddOrUpdate(data);
        }

        public void Remove()
        {
            if (Items.FirstOrDefault() is UserViewModel user)
            {
                itemsCache.Remove(user.Id);
            }
        }

        public void Update()
        {
            if (Items.FirstOrDefault() is UserViewModel user)
            {
                var data = new UserInfo { Id = user.Id, Name = Path.GetRandomFileName() };
                itemsCache.AddOrUpdate(data);
            }
        }
    }
}