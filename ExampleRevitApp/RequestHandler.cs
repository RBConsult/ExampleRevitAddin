using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static ExampleRevitAddin.RequestHandler;
using System.Windows;
using ExampleRevitAddin.Controllers;

namespace ExampleRevitAddin
{
    public class RequestHandler : IExternalEventHandler
    {
        //Set up documents
        public static UIApplication uiApp;

        public class Request
        {
            // Storing the value as a plain Int makes using the interlocking mechanism simpler
            private int m_request = (int)RequestId.None;

            public RequestId Take()
            {
                return (RequestId)Interlocked.Exchange(ref m_request, (int)RequestId.None);
            }

            public void Make(RequestId request)
            {
                Interlocked.Exchange(ref m_request, (int)request);
            }
        }

        public enum RequestId : int
        {
            None = 0,
            DoSomething = 1,
            ShowProgress = 2
        }

        // The value of the latest request made by the modeless form 
        private Request m_request = new Request();
        public Request request
        {
            get { return m_request; }
        }

        public String GetName()
        {
            return "Revit App Request Handler";
        }

        public void Execute(UIApplication uiapp)
        {
            //Get local copy of UI Application Object
            uiApp = uiapp;

            switch (request.Take())
            {
                case RequestId.None:
                    {
                        return;  // no request at this time -> we can leave immediately
                    }
                case RequestId.DoSomething:
                    {
                        try
                        {
                            MyController.DoSomething(uiapp);
                        }
                        catch (Exception ex) { MessageBox.Show(ex.Message, "Example Revit Addin"); }
                        break;
                    }
                case RequestId.ShowProgress:
                    {
                        try
                        {
                            MyController.ShowProgress(uiapp);
                        }
                        catch (Exception ex) { MessageBox.Show(ex.Message, "Example Revit Addin"); }
                        break;
                    }
                default:
                    {
                        // some kind of a warning here should
                        // notify us about an unexpected request 
                        break;
                    }
            }
        }
    }
}
