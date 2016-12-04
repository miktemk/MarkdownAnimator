using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using Microsoft.Practices.ServiceLocation;
using Miktemk.TextToSpeech.Services;
using Miktemk.Wpf.Services;

namespace MarkdownAnimator.ViewModel
{
    public class ViewModelLocator
    {
        static ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            if (!ViewModelBase.IsInDesignModeStatic)
            {
                SimpleIoc.Default.Register<MvvmDialogs.IDialogService>(() => new MvvmDialogs.DialogService());
                SimpleIoc.Default.Register<ITtsService, TtsService>();
                SimpleIoc.Default.Register<IRegistryService, RegistryService>();
                SimpleIoc.Default.Register<MainViewModel>();
            }
        }

        public MainViewModel Main => ServiceLocator.Current.GetInstance<MainViewModel>();

        public static void Cleanup()
        {
        }
    }
}