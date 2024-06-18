using Autodesk.Revit.UI;
using ExampleRevitAddin;
using ExampleRevitAddin.Properties;
using System;
using System.Collections.Generic;
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
using static ExampleRevitAddin.RequestHandler;

namespace ExampleRevitApp.Views
{
    /// <summary>
    /// Interaction logic for DockableWindow.xaml
    /// </summary>
    public partial class DockableWindow : Page, Autodesk.Revit.UI.IDockablePaneProvider
    {
        public DockableWindow(ExternalEvent exEvent, RequestHandler handler)
        {
            InitializeComponent();

            m_Handler = handler;
            m_ExEvent = exEvent;
            this.DataContext = this;

            this.cmbScale.SelectedIndex = 2;
            this.cmbTheme.SelectedIndex = 0;
        }

        #region Window Size Code
        public static readonly DependencyProperty ScaleValueProperty = DependencyProperty.Register("ScaleValue", typeof(double), typeof(DockableWindow), new UIPropertyMetadata(1.0, new PropertyChangedCallback(OnScaleValueChanged), new CoerceValueCallback(OnCoerceScaleValue)));

        private static object OnCoerceScaleValue(DependencyObject o, object value)
        {
            DockableWindow mainWindow = o as DockableWindow;
            if (mainWindow != null)
                return mainWindow.OnCoerceScaleValue((double)value);
            else
                return value;
        }

        private static void OnScaleValueChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            DockableWindow mainWindow = o as DockableWindow;
            if (mainWindow != null)
                mainWindow.OnScaleValueChanged((double)e.OldValue, (double)e.NewValue);
        }

        protected virtual double OnCoerceScaleValue(double value)
        {
            if (double.IsNaN(value))
                return 1.0d;

            value = Math.Max(0.1, value);
            return value;
        }

        protected virtual void OnScaleValueChanged(double oldValue, double newValue)
        {

        }

        public double ScaleValue
        {
            get
            {
                return (double)GetValue(ScaleValueProperty);
            }
            set
            {
                SetValue(ScaleValueProperty, value);
            }
        }
        #endregion

        //Dock Location
        private Guid m_targetGuid;
        private DockPosition m_position = DockPosition.Right;
        private int m_left = 1;
        private int m_right = 1;
        private int m_top = 1;
        private int m_bottom = 1;

        //Event Handling
        private RequestHandler m_Handler;
        private ExternalEvent m_ExEvent;

        /// <summary>
        /// Called by Revit to initialize dockable pane settings set in DockingSetupDialog.
        /// </summary>
        /// <param name="data"></param>
        public void SetupDockablePane(Autodesk.Revit.UI.DockablePaneProviderData data)
        {
            data.FrameworkElement = this as FrameworkElement;
            DockablePaneProviderData d = new DockablePaneProviderData();


            data.InitialState = new Autodesk.Revit.UI.DockablePaneState();
            data.InitialState.DockPosition = m_position;
            DockablePaneId targetPane;
            if (m_targetGuid == Guid.Empty)
                targetPane = null;
            else targetPane = new DockablePaneId(m_targetGuid);
            if (m_position == DockPosition.Tabbed)
                data.InitialState.TabBehind = targetPane;


            if (m_position == DockPosition.Floating)
            {
                data.InitialState.SetFloatingRectangle(new Autodesk.Revit.DB.Rectangle(m_left, m_top, m_right, m_bottom));
            }

        }

        private void DockableDialogs_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void DockableDialogs_SizeChanged(object sender, SizeChangedEventArgs e)
        {

        }

        private void cmbScale_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBoxItem typeItem = (sender as System.Windows.Controls.ComboBox).SelectedItem as ComboBoxItem;
            ScaleValue = Convert.ToDouble(typeItem.Content.ToString().Replace("%", "")) / 100;
        }

        private void cmbTheme_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBoxItem typeItem = (sender as System.Windows.Controls.ComboBox).SelectedItem as ComboBoxItem;
            Settings.Default.ColorMode = typeItem.Content.ToString();
        }

        private void MakeRequest(RequestId request)
        {
            m_Handler.request.Make(request);
            m_ExEvent.Raise();
        }

        private void btnDoSomethingCool_Click(object sender, RoutedEventArgs e)
        {
            Task.Run(() =>
            {
                MakeRequest(RequestHandler.RequestId.DoSomething);
            });
        }

        private void btnShowProgress_Click(object sender, RoutedEventArgs e)
        {
            Task.Run(() =>
            {
                MakeRequest(RequestHandler.RequestId.ShowProgress);
            });
        }

        private void btnAction1_Click(object sender, RoutedEventArgs e)
        {
        }

        private void btnAction2_Click(object sender, RoutedEventArgs e)
        {
        }

        private void btnAction3_Click(object sender, RoutedEventArgs e)
        {
        }

        private void btnAction4_Click(object sender, RoutedEventArgs e)
        {
        }

        private void menuControls_Click(object sender, RoutedEventArgs e)
        {
            var addButton = sender as FrameworkElement;
            if (addButton != null)
            {
                addButton.ContextMenu.IsOpen = true;
            }
        }
    }

    public class DpiDecorator : System.Windows.Controls.Decorator
    {
        public DpiDecorator()
        {
            this.Loaded += (s, e) =>
            {
                Matrix m = System.Windows.PresentationSource.FromVisual(this).CompositionTarget.TransformToDevice;
                ScaleTransform dpiTransform = new ScaleTransform(1 / m.M11, 1 / m.M22);
                if (dpiTransform.CanFreeze)
                    dpiTransform.Freeze();
                this.LayoutTransform = dpiTransform;
            };
        }
    }
}
