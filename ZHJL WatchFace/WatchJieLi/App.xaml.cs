using log4net.Config;
using Prism.DryIoc;
using Prism.Ioc;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using WatchBasic.Tool;
using WatchUI.ViewModels;
using WatchUI.Views;

namespace WatchJieLi
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : PrismApplication
    {
        public App()
        {
           
            XmlConfigurator.Configure();
            
        }
        protected override void OnInitialized() // 启动过程中
        {
            var dialog = Container.Resolve<IDialogService>();

            dialog.ShowDialog("Setting", null, callback =>
            {
                if (callback.Result != ButtonResult.OK)
                {
                    Application.Current.Shutdown();
                    return;
                }
                base.OnInitialized();

            });
        }
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
            DispatcherUnhandledException += new System.Windows.Threading.DispatcherUnhandledExceptionEventHandler(App_DispatcherUnhandledException);
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            //处理未处理的异常
            LogHelper.WriteLog(e.ExceptionObject.ToString());
            MessageBox.Show( "An unhandled exception occurred: " + e.ExceptionObject.ToString());

        }

        private void App_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            //处理UI线程中未处理的异常
            LogHelper.WriteLog(e.Exception.Message + "\n" + e.Exception.StackTrace);
            MessageBox.Show("An unhandled(Dispatcher) exception occurred: " + e.Exception.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            e.Handled = true;
        }

        protected override Window CreateShell()
        {
            return Container.Resolve<Views.MainView>();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<Kwh, KwhWatch>("Kwh");
            containerRegistry.RegisterForNavigation<GeneralDate, GeneralDateWatch>("GeneralDate");
            containerRegistry.RegisterForNavigation<HeartRate, HeartRateWatch>("HeartRate");
            containerRegistry.RegisterForNavigation<Step, BaseWatch>("Step");
            containerRegistry.RegisterForNavigation<Calorie, BaseWatch>("Calorie");
            containerRegistry.RegisterForNavigation<EditLayerView, EditLayerViewModel>("EditLayer");
            containerRegistry.RegisterForNavigation<Time, TimeWatch>("Time");
            containerRegistry.RegisterDialog<Catalogue, CatalogueViewModel>();
            containerRegistry.RegisterDialog<Information, InformationViewModel>();
            containerRegistry.RegisterDialog<Thumbnail, ThumbnailViewModel>();
            containerRegistry.RegisterDialog<WatchSetting, WatchSettingModel>("WatchSetting");
            containerRegistry.RegisterForNavigation<SettingsView, SettingsViewModel>("Setting");
            containerRegistry.RegisterDialog<AddSetting, AddSettingModel>("AddSetting");
        }
    }
}
