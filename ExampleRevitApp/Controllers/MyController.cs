using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using ExampleRevitAddin.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace ExampleRevitAddin.Controllers
{
    internal static class MyController
    {
        public static Dictionary<string, int> categories = new Dictionary<string, int>();

        internal static void DoSomething(UIApplication uiApp)
        {
            UIDocument uiDoc = uiApp.ActiveUIDocument;
            var selection = uiDoc.Selection;
#if RVT_21
            MessageBox.Show($"Revit 2021: {selection.GetElementIds().Count} Elements Selected");
#elif RVT_22
            MessageBox.Show($"Revit 2022: {selection.GetElementIds().Count} Elements Selected");
#elif RVT_23
            MessageBox.Show($"Revit 2023: {selection.GetElementIds().Count} Elements Selected");
#elif RVT_24
            MessageBox.Show($"Revit 2024: {selection.GetElementIds().Count} Elements Selected");
#elif RVT_25
            MessageBox.Show($"Revit 2025: {selection.GetElementIds().Count} Elements Selected");
#endif
        }

        internal static void ShowProgress(UIApplication uiApp)
        {
            //Clear output
            MyController.categories = new Dictionary<string, int>();

            //Show Progress Bar
            using (ProgressBar varForm = new ProgressBar(uiApp, "Selection"))
            {
                System.Windows.Interop.WindowInteropHelper helper = new System.Windows.Interop.WindowInteropHelper(varForm);
                if (!String.IsNullOrEmpty(System.Diagnostics.Process.GetCurrentProcess().MainWindowTitle))
                    helper.Owner = System.Diagnostics.Process.GetCurrentProcess().MainWindowHandle;
                varForm.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                varForm.ShowDialog();

                if (varForm.DialogResult == false)
                {
                    return;
                }
            }

            //Compile message
            var message = "Your selection contains:" + Environment.NewLine;
            foreach (var category in categories)
            {
                message += $"{category.Key}: {category.Value}{Environment.NewLine}";
            }

            MessageBox.Show(message);
        }

        internal static void ProcessElement(Document doc, ElementId eleId)
        {
            Element ele = doc.GetElement(eleId);
            var cat = ele.Category.Name;

            if (categories.ContainsKey(cat))
            {
                int currentCount;
                categories.TryGetValue(cat, out currentCount);
                categories[cat] = currentCount + 1;
            } else
            {
                categories.Add(cat, 1);
            }
            Thread.Sleep(100);
        }
    }
}
