using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using Autofac;
using Caliburn.Micro;

namespace TestDynamicData
{
    /// <summary>
    /// 程序第二入口，主要用于代码配置
    /// </summary>
    public class AppBootstrapper : BootstrapperBase
    {
        public AppBootstrapper()
        {
            Initialize();
        }

        /// <summary>
        /// 注入容器
        /// </summary>
        public IContainer Container { get; private set; }

        protected override void Configure()
        {
            var builder = new ContainerBuilder();

            builder.RegisterType<WindowManager>().As<IWindowManager>().SingleInstance();
            builder.RegisterType<EventAggregator>().As<IEventAggregator>().SingleInstance();
            builder.RegisterType<DataAccessor>().SingleInstance();
            builder.RegisterType<ShellViewModel>().As<IShell>();

            Container = builder.Build();
        }

        protected override object GetInstance(Type service, string key)
        {
            return String.IsNullOrEmpty(key)
                ? Container.Resolve(service)
                : Container.ResolveKeyed(key, service)
                ?? new NullReferenceException($"无法由容器生成实例，可能未注册模块：{service}");
        }

        protected override IEnumerable<object> GetAllInstances(Type service)
        {
            return Container.Resolve(typeof(IEnumerable<>).MakeGenericType(service)) as IEnumerable<object>;
        }

        protected override void BuildUp(object instance)
        {
            Container.InjectProperties(instance);
        }

        protected override async void OnStartup(object sender, System.Windows.StartupEventArgs e)
        {
            await DisplayRootViewFor<IShell>();
        }

        protected override void OnExit(object sender, EventArgs e)
        {
            base.OnExit(sender, e);
        }
    }
}