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
using TestDynamicData.Test;
using TestDynamicData.Views;
using TheRx;

namespace TestDynamicData
{
    public class ShellViewModel : Screen, IShell
    {
        public ShellViewModel(IWindowManager windowManager
            , DataAccessor dataAccessor)
        {
            this.windowManager = windowManager;
            this.dataAccessor = dataAccessor;
        }

        private readonly IWindowManager windowManager;
        private readonly DataAccessor dataAccessor;

        public async Task ShareConnectStream()
        {
            await windowManager.ShowDialogAsync(new ShareConnectStreamViewModel());
        }

        public async Task SelectFirstItem()
        {
            await windowManager.ShowDialogAsync(new SelectFirstItemViewModel(dataAccessor));
        }

        public void TestUpdatePropertyChanged()
        {
            var _ = new TestUpdatePropertyChanged();
            // please watch Trace output message.
        }

        public void TestOnItemRemovedCalled()
        {
            var _ = new TestOnItemRemovedCalled();
            // please watch Trace output message.
        }

        public void TestConnectDifferentCache()
        {
            var _ = new TestConnectDifferentCache();
            // please watch Trace output message.
        }

        public void TestRedundancyBind()
        {
            var _ = new TestRedundancyBind();
            // please watch Trace output message.
        }
    }
}