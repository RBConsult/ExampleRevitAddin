using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using CommunityToolkit.Mvvm.DependencyInjection;
using ExampleRevitAddin.Controllers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace ExampleRevitAddin.Views
{
    /// <summary>
    /// Interaction logic for ProgressBar.xaml
    /// </summary>
    public partial class ProgressBar : Window, IDisposable
    {
        private bool disposedValue;
        private string _mode;
        private UIApplication _uiApp;
        private Document _doc;
        private bool isCanceled = false;

        public ProgressBar(UIApplication uiApp, string mode)
        {
            InitializeComponent();
            _uiApp = uiApp;
            UIDocument uiDoc = uiApp.ActiveUIDocument;
            _doc = uiDoc.Document;
            _mode = mode;

            BackgroundWorker backgroundWorker = new BackgroundWorker();
            backgroundWorker.DoWork += BackgroundWorker_DoWork;
            backgroundWorker.ProgressChanged += BackgroundWorker_ProgressChanged;
            backgroundWorker.WorkerReportsProgress = true;
            if (!backgroundWorker.IsBusy)
            {
                backgroundWorker.RunWorkerAsync();
            }
        }

        private void BackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;

            switch (_mode)
            {
                case "Selection":
                    {
                        var selection = _uiApp.ActiveUIDocument.Selection.GetElementIds().ToList();
                        int max = selection.Count;

                        for (int i = 0; i < max; i++)
                        {
                            if (isCanceled)
                            {
                                this.Dispatcher.Invoke(() =>
                                {
                                    this.txtMessage.Text = $"Canceled by user.";
                                });
                                System.Threading.Thread.Sleep(2000);
                                this.Dispatcher.Invoke(() =>
                                {
                                    this.DialogResult = false;
                                });
                                return;
                            }
                            worker.ReportProgress((int)(((decimal)(i + 1) / (decimal)max) * 100));
                            var task = Task.Run(() =>
                            {
                                MyController.ProcessElement(_doc, selection[i]);
                            });
                            task.Wait(5000);
                            this.Dispatcher.Invoke(() =>
                            {
                                this.txtMessage.Text = $"Processing {i + 1} of {max}";
                            });
                        }
                        break;
                    }
                case "Something Else":
                    {
                        break;
                    }
            }

            this.Dispatcher.Invoke(() =>
            {
                this.DialogResult = true;
                this.Close();
            });

        }

        private void BackgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {

            this.progressBar.Value = e.ProgressPercentage;
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            isCanceled = true;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    //dispose managed objects if any
                }

                //dispose unmanaged objects if any

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
