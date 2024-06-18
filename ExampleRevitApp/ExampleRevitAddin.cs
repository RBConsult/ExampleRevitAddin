using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Events;
using ExampleRevitApp.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ExampleRevitAddin
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    [Autodesk.Revit.Attributes.Regeneration(Autodesk.Revit.Attributes.RegenerationOption.Manual)]
    [Autodesk.Revit.Attributes.Journaling(Autodesk.Revit.Attributes.JournalingMode.NoCommandData)]
    public class ExampleRevitAddin : IExternalApplication
    {
        //Dockable Window Setup
        internal static DockableWindow m_MyDockableWindow = null;
        internal static Boolean DocShown = false;

        public Result OnShutdown(UIControlledApplication application)
        {
            return Autodesk.Revit.UI.Result.Succeeded;
        }

        public Autodesk.Revit.UI.Result OnStartup(UIControlledApplication application)
        {

            // Add a new ribbon panel
            CreateRibbonPanel(application);

            //Register Dockable Window
            if (RegisterDockableWindow(application))
                return Autodesk.Revit.UI.Result.Succeeded;

            return Autodesk.Revit.UI.Result.Failed;
        }

        /// <summary>
        /// Creates Ribbon and Buttons for Example Addin.
        /// </summary>
        /// <param name="application"></param>
        private void CreateRibbonPanel(UIControlledApplication application)
        {
            RibbonPanel ribbonPanel = application.CreateRibbonPanel("ExampleRevitAddin");

            ContextualHelp help = new ContextualHelp(ContextualHelpType.Url, "https://www.revitapidocs.com/");

            // Create a push button to trigger a command add it to the ribbon panel.
            string thisAssemblyPath = Assembly.GetExecutingAssembly().Location;

            PushButtonData buttonData = new PushButtonData("cmdOpenDock",
               "Open" + Environment.NewLine + "Dock", thisAssemblyPath, typeof(ShowDockableWindow).FullName);
            PushButton pushButton = ribbonPanel.AddItem(buttonData) as PushButton;
            pushButton.ToolTip = "Open a dockable window in Revit.";
            pushButton.LargeImage = ImageUtil.GetEmbeddedImage(Assembly.GetExecutingAssembly(), "ExampleRevitAddin.Resources.Logo_Icon_32.png");
            pushButton.SetContextualHelp(help);
        }


        [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
        public class ShowDockableWindow : IExternalCommand
        {
            public static UIApplication uiApp;
            public static UIDocument uiDoc;
            public static Document doc;

            public int callCounter = 0;
            public System.Drawing.Point lastCursor
                = new System.Drawing.Point(-5, -5);

            public Result Execute(
              ExternalCommandData commandData,
              ref string message,
              ElementSet elements)
            {
                uiApp = commandData.Application;
                uiDoc = uiApp.ActiveUIDocument;
                doc = uiDoc.Document;

                try
                {
                    DockablePaneId dpid = new DockablePaneId(
                      new Guid("{595392D7-1DA2-4F6B-82B9-1DE7DDAA03B9}"));

                    DockablePane dp = commandData.Application
                      .GetDockablePane(dpid);

                    dp.Show();
                }
                catch (Exception ex) { System.Windows.MessageBox.Show(ex.Message, "Load Dockable Window"); }

                return Result.Succeeded;
            }
        }

        /// <summary>
        /// Configures and Registers Dockable Window
        /// </summary>
        /// <param name="application"></param>
        /// <returns></returns>
        private bool RegisterDockableWindow(UIControlledApplication application)
        {
            try
            {
                DockablePaneProviderData data
                 = new DockablePaneProviderData();

                // A new handler to handle request posting by the dialog
                RequestHandler handler = new RequestHandler();

                // External Event for the dialog to use (to post requests)
                ExternalEvent exEvent = ExternalEvent.Create(handler);

                DockableWindow MainDockableWindow = new DockableWindow(exEvent, handler);

                m_MyDockableWindow = MainDockableWindow;

                data.FrameworkElement = MainDockableWindow
                  as System.Windows.FrameworkElement;

                data.InitialState = new DockablePaneState();

                data.InitialState.DockPosition
                  = DockPosition.Tabbed;

                data.InitialState.TabBehind = DockablePanes
                  .BuiltInDockablePanes.ProjectBrowser;

                //data.VisibleByDefault = false;

                DockablePaneId dpid = new DockablePaneId(
                  new Guid("{595392D7-1DA2-4F6B-82B9-1DE7DDAA03B9}"));

                application.RegisterDockablePane(
                  dpid, "Example Addin for Autodesk® Revit®", MainDockableWindow
                  as IDockablePaneProvider);

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        private void CtrlApp_DockableFrameVisibilityChanged(object sender, DockableFrameVisibilityChangedEventArgs e)
        {
            if (e.PaneId.Guid == new Guid("{595392D7-1DA2-4F6B-82B9-1DE7DDAA03B9}") && e.DockableFrameShown == true)
            {
                DocShown = true;
            }
            else
            {
                DocShown = false;
            }
        }
    }
}
