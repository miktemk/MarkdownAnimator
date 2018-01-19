using CefSharp;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MarkdownAnimatorCef.Controls
{
    /// <summary>
    /// Interaction logic for WebViewJasarUIControl.xaml
    /// </summary>
    public partial class WebViewJasarUI : UserControl //, IWebWordRectanglesExinvoker
    {
        public WebViewJasarUI()
        {
            InitializeComponent();
            //Browser.RegisterAsyncJsObject("exinvoker", this as IWebWordRectanglesExinvoker, BindingOptions.DefaultBinder);
        }

        private void Browser_IsBrowserInitializedChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (Browser.IsBrowserInitialized)
            {
                Browser.Load("local://jasar/index.html");
            }
        }

        public void exinkoveAddSentence()
        {

        }


        //#region -------------------------------- DP: VisualizationVM -----------------------------------

        //public VisualizationVM VisualizationModel
        //{
        //    get { return (VisualizationVM)GetValue(VisualizationModelProperty); }
        //    set { SetValue(VisualizationModelProperty, value); }
        //}

        //// Using a DependencyProperty as the backing store for VisualizationModel.  This enables animation, styling, binding, etc...
        //public static readonly DependencyProperty VisualizationModelProperty =
        //    DependencyProperty.Register("VisualizationModel", typeof(VisualizationVM), typeof(WebViewJasarUI),
        //                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender, OnVisualizationModelPropertyChanged));

        //private static void OnVisualizationModelPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        //{
        //    var control = sender as WebViewJasarUI;
        //    if (control != null)
        //        control.OnVisualizationModelChanged((VisualizationVM)e.OldValue, (VisualizationVM)e.NewValue);
        //}

        //private void OnVisualizationModelChanged(VisualizationVM oldValue, VisualizationVM newValue)
        //{
        //    if (!Browser.IsBrowserInitialized)
        //        return;
        //    if (newValue == null)
        //        newValue = new VisualizationVM { ViewType = VisualizationType.None };

        //    var vizJson = AlexaIdeUtils.JsonSerialize(newValue);
        //    Browser.GetMainFrame().ExecuteJavaScriptAsync($"exinvoke_setVisualizationVM({vizJson})");
        //}

        //#endregion

        //#region ------------------------------ exop ---------------------------------

        //public void regionClicked(VmTextRegion region)
        //{
        //    this.Dispatcher.Invoke(() =>
        //    {
        //        if (OnRegionClicked != null)
        //            OnRegionClicked.Execute(region);
        //    });
        //}

        //public ICommand OnRegionClicked
        //{
        //    get { return (ICommand)GetValue(OnRegionClickedProperty); }
        //    set { SetValue(OnRegionClickedProperty, value); }
        //}

        //// Using a DependencyProperty as the backing store for OnRegionClicked.  This enables animation, styling, binding, etc...
        //public static readonly DependencyProperty OnRegionClickedProperty =
        //    DependencyProperty.Register("OnRegionClicked", typeof(ICommand), typeof(WebViewJasarUI),
        //                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender, OnOnRegionClickedPropertyChanged));

        //private static void OnOnRegionClickedPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        //{
        //    var control = sender as WebViewJasarUI;
        //    if (control != null)
        //        control.OnOnRegionClickedChanged((ICommand)e.OldValue, (ICommand)e.NewValue);
        //}

        //private void OnOnRegionClickedChanged(ICommand oldValue, ICommand newValue) { }

        //#endregion

    }

    //public interface IWebWordRectanglesInvoker
    //{
    //    void setVisualizationVM(VisualizationVM vm);
    //}
    //public interface IWebWordRectanglesExinvoker
    //{
    //    void regionClicked(VmTextRegion region);
    //}

    //public class WebWordRectanglesExinvoker 
    //{
    //    //public void Error()
    //    //{
    //    //    throw new Exception("This is an exception coming from C#");
    //    //}
    //    //public int Div(int divident, int divisor)
    //    //{
    //    //    return divident / divisor;
    //    //}
    //}
}
