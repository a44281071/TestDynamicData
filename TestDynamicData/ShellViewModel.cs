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
            ScreenItems = new[]
            {
                nameof(ShareConnectStreamViewModel),
                nameof(SelectFirstItemViewModel),
            };
        }

        private readonly IWindowManager windowManager;
        private readonly DataAccessor dataAccessor;

        public string[] ScreenItems { get; }

        // Binding command.
        public async Task ShowScreen(string screenName)
        {
            if (screenName == nameof(ShareConnectStreamViewModel))
            {
                await windowManager.ShowDialogAsync(new ShareConnectStreamViewModel());
            }
            else if (screenName == nameof(SelectFirstItemViewModel))
            {
                await windowManager.ShowDialogAsync(new SelectFirstItemViewModel(dataAccessor));
            }
        }
    }
}