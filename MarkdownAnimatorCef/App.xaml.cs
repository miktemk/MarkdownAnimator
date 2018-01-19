using System.Windows;
using GalaSoft.MvvmLight.Threading;
using MarkdownAnimatorCef.ConfigCef;

namespace MarkdownAnimatorCef
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        static App()
        {
            DispatcherHelper.Initialize();

            MyCefSharpConfiguration.Configure();
        }
    }
}
