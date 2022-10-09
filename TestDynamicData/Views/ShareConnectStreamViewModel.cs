using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
using Caliburn.Micro;
using DynamicData;
using DynamicData.Binding;
using TheRx;

namespace TestDynamicData.Views
{
    /// <summary>
    /// WhenAnyPropertyChanged() did'nt work, because Transform()
    /// </summary>
    /// <see href="https://github.com/reactivemarbles/DynamicData/discussions/608#discussioncomment-2995114"/>
    public class ShareConnectStreamViewModel : Screen
    {
        public ShareConnectStreamViewModel()
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
                .Page(pager!)
                .Do(change => PageParameters.Update(change.Response))
                //.ObserveOnDispatcher()
                .Publish();  //THE MAGIC IS HERE - means multiple subscribers share the output from this point onwards

            // rx Connect() turns the sharing on.
            // It is not dd Connect(). Ambiguous I know and it's a historic naming mistake.
            var myConnectedDisposable = chosen.Connect();

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

        private readonly SourceCache<UserInfo, Guid> itemsCache = new(dd => dd.Id);

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
                Id = Guid.NewGuid(),
                Name = Path.GetRandomFileName()
            };
            itemsCache.AddOrUpdate(data);
        }

        private UserInfo[] ListResetData()
        {
            var users = new UserInfo[]
                {
                    new UserInfo
                    {
                        Id = Guid.Parse("{ECBB4905-DE52-4311-8C3F-4D3DF6B3B437}"),
                        Name = Path.GetRandomFileName()
                    },
                    new UserInfo
                    {
                        Id = Guid.Parse("{121D71B5-BD3D-4F05-88C9-E7D9D8DF92F5}"),
                        Name = Path.GetRandomFileName()
                    },
                };
            return users;
        }

        public void Reset()
        {
            ThreadPool.QueueUserWorkItem(state => {
                itemsCache.Edit(cc =>
                {
                    cc.Clear();
                    cc.AddOrUpdate(ListResetData());
                });
            });
         
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