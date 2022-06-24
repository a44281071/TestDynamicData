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
            ScreenItems = new()
            {
                new ShareConnectStreamViewModel(),
                new SelectFirstItemViewModel(dataAccessor)
            };
        }

        private readonly IWindowManager windowManager;

        public BindableCollection<IScreen> ScreenItems { get; }

        // Binding command.
        public async Task ShowScreen(IScreen screen)
        {
            await windowManager.ShowDialogAsync(screen);
        }
    }
}