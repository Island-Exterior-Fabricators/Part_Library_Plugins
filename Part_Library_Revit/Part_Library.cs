using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Events;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace Part_Library_Revit
{
    class ExternalEventPL : IExternalEventHandler
    {
        /// <summary>
        /// Revit UI application.
        /// </summary>
        UIApplication _uiapp = null;
        public static void RunExternal()
        {

            // A new handler to handle request posting by the dialog
            m_ExHandler = new ExternalEventPL();

            // External Event for the dialog to use (to post requests)
            m_ExEvent = ExternalEvent.Create(m_ExHandler);

            Thread _thread = new Thread(CheckForChanges);

            _thread.Start();

            return;
        }
        public void Execute(UIApplication uiapp)
        {
            LibraryAction(uiapp);
        }

        /// <summary>
        /// Required IExternalEventHandler interface 
        /// method returning a descriptive name.
        /// </summary>
        public string GetName()
        {
            return "Part_Library";
        }

        /// <summary>
        /// Store the external event.
        /// </summary>
        static ExternalEvent m_ExEvent = null;
        static ExternalEventPL m_ExHandler = null;

        public static ExternalEvent LibraryAction(UIApplication revit)
        {
            // get the UIApplication object, which opens the Revit API for you...
            UIDocument uidoc = revit.ActiveUIDocument;
            Document doc = revit.ActiveUIDocument.Document;
            Transaction trans = new Transaction(revit.ActiveUIDocument.Document, "Part Library");

            if (MessageHandler.LibraryToAppActions.Count > 0)
            {
                string action = MessageHandler.LibraryToAppActions.Dequeue();
                //MessageBox.Show(action);
                Dictionary<string, string> actionD = JsonConvert.DeserializeObject<Dictionary<string, string>>(action);

                //####################################################
                //#"Start" the transaction
                //trans.Start();
                if (actionD["option"] == "OPEN")
                {
                    try
                    {
                        ElementId pElementId;
                        List<string> files = Directory.GetFiles(actionD["hyperlink"]).ToList();
                        string rfa = "";
                        string dwg = "";
                        foreach (string file in files)
                        {
                            if (file.Contains(".rfa"))
                            {
                                rfa = file;
                            }
                            else if (file.Contains(".dwg"))
                            {
                                dwg = file;
                            }
                            else
                            {

                            }
                        }
                        if (rfa != "")
                        {
                            try
                            {
                                revit.OpenAndActivateDocument(rfa);
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message);
                            }
                        }
                        else if (dwg != "")
                        {
                            Document famDoc = revit.Application.NewFamilyDocument(doc.Application.FamilyTemplatePath + "\\Detail Item.rft");
                            DWGImportOptions dwgImportOption = new DWGImportOptions();
                            FilteredElementCollector col = new FilteredElementCollector(famDoc).OfCategory(BuiltInCategory.OST_Views);
                            Autodesk.Revit.DB.View view = col.First() as Autodesk.Revit.DB.View;
                            Transaction famTrans = new Transaction(famDoc, "Import CAD");
                            famTrans.Start();
                            famDoc.Import(dwg, dwgImportOption, view, out pElementId);
                            famTrans.Commit();

                            SaveAsOptions saveOpt = new SaveAsOptions();
                            string familyfilename = dwg.Remove(dwg.Length - 4, 4) + ".rfa";
                            famDoc.SaveAs(familyfilename, saveOpt);
                            famDoc.Close();
                            revit.Application.OpenDocumentFile(familyfilename);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
                else if (actionD["option"] == "LOAD")
                {
                    try
                    {
                        ElementId pElementId;
                        List<string> files = Directory.GetFiles(actionD["hyperlink"]).ToList();
                        string rfa = "";
                        string dwg = "";
                        foreach (string file in files)
                        {
                            if (file.Contains(".rfa"))
                            {
                                rfa = file;
                            }
                            else if (file.Contains(".dwg"))
                            {
                                dwg = file;
                            }
                            else
                            {

                            }
                        }
                        if (rfa != "")
                        {
                            Transaction famLoad = new Transaction(doc, "Load Family");
                            famLoad.Start();
                            doc.LoadFamily(rfa);
                            famLoad.Commit();
                        }
                        else if (dwg != "")
                        {
                            Document famDoc = revit.Application.NewFamilyDocument(doc.Application.FamilyTemplatePath + "\\Detail Item.rft");
                            DWGImportOptions dwgImportOption = new DWGImportOptions();
                            FilteredElementCollector col = new FilteredElementCollector(famDoc).OfCategory(BuiltInCategory.OST_Views);
                            Autodesk.Revit.DB.View view = col.First() as Autodesk.Revit.DB.View;
                            Transaction famTrans = new Transaction(famDoc, "Import CAD");
                            famTrans.Start();
                            famDoc.Import(dwg, dwgImportOption, view, out pElementId);
                            famTrans.Commit();

                            SaveAsOptions saveOpt = new SaveAsOptions();
                            string familyfilename = dwg.Remove(dwg.Length - 4, 4) + ".rfa";
                            famDoc.SaveAs(familyfilename, saveOpt);
                            famDoc.Close();

                            Transaction famLoad = new Transaction(doc, "Load Family");
                            famLoad.Start();
                            doc.LoadFamily(familyfilename);
                            famLoad.Commit();
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
                else if (actionD["option"] == "VIEW")
                {
                    try
                    {
                        ElementId pElementId;
                        List<string> files = Directory.GetFiles(actionD["hyperlink"]).ToList();
                        string rfa = "";
                        string dwg = "";
                        foreach (string file in files)
                        {
                            if (file.Contains(".dwg"))
                            {
                                dwg = file;
                            }
                            else
                            {

                            }
                        }
                        if (dwg != "")
                        {
                            string progId = "AutoCAD.Application";
                            dynamic cadApp = null;


                            try
                            {
                                cadApp = Marshal.GetActiveObject(progId);
                            }
                            catch
                            {
                                try
                                {
                                    Type t = Type.GetTypeFromProgID(progId);
                                    cadApp = Activator.CreateInstance(t);
                                }
                                catch
                                {
                                }
                            }

                            if (cadApp != null)
                            {
                                dynamic cadFile = cadApp.Documents.Open(dwg, false);
                                cadApp.Visible = true;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }

                //# "End" the transaction
                //trans.Commit();
                //####################################################

                // do your Revit tasks here
                //MessageBox.Show("Text received from the Library window: " + actionD);

                // reply something back
                MessageHandler.AppToLibraryActions.Enqueue("A message from the Revit plugin - " + DateTime.Now.ToString());

            }

            return m_ExEvent;
        }

        /// <summary>
        /// Number of milliseconds to wait and relinquish
        /// CPU control before next check for pending
        /// database updates.
        /// </summary>
        static int _timeout = 500;

        /// <summary>
        /// Repeatedly check database status and raise 
        /// external event when updates are pending.
        /// Relinquish control and wait for timeout
        /// period between each attempt. Run in a 
        /// separate thread.
        /// </summary>
        public static void CheckForChanges()
        {
            try
            {
                while (true)
                {
                    if (MessageHandler.LibraryToAppActions.Count > 0)
                    {
                        m_ExEvent.Raise();
                    }

                    // Wait a moment and relinquish control before
                    // next check for pending database updates.
                    Thread.Sleep(_timeout);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
    /// <summary>
    /// Part Library
    /// </summary>
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    [Autodesk.Revit.Attributes.Regeneration(Autodesk.Revit.Attributes.RegenerationOption.Manual)]
    [Autodesk.Revit.Attributes.Journaling(Autodesk.Revit.Attributes.JournalingMode.NoCommandData)]
    public class Command : IExternalCommand
    {
        public Autodesk.Revit.UI.Result Execute(
        Autodesk.Revit.UI.ExternalCommandData commandData,
        ref string message, Autodesk.Revit.DB.ElementSet elementSet)
        {
            UIApplication uiApp = new UIApplication(commandData.Application.Application);

            bool error = false;
            try
            {
                // if the browser window/process is already open, activate it instead of opening a new process 
                if (MessageHandler.library_pid != 0)
                {
                    // the following line could be enough, but rather activate the window thru the process
                    //SetForegroundWindow(MessageHandler.browser_hwnd);

                    // try to activate the existing instance
                    Process[] processes = Process.GetProcesses();

                    foreach (Process p in processes)
                    {
                        if (p.Id == MessageHandler.library_pid)
                        {
                            IntPtr windowHandle = p.MainWindowHandle;
                            SetForegroundWindow(windowHandle);
                            return Result.Succeeded;
                        }
                    }
                }

                // execute the library window process
                Process process = new Process();
                process.StartInfo.FileName = AssemblyDirectory + "\\Part_Library_App.exe ";
                process.StartInfo.Arguments = App.messagehandler.Handle.ToString(); // pass the MessageHandler's window handle the the process as a command line argument
                process.Start();

                MessageHandler.library_pid = process.Id; // grab the PID so we can kill the process if required;

                ExternalEventPL.RunExternal();

                return Result.Succeeded;

            }
            catch (Exception e)
            {
                // if revit threw an exception, try to catch it
                TaskDialog.Show("Error", e.Message);
                error = true;
                return Autodesk.Revit.UI.Result.Failed;
            }
            finally
            {
                // if revit threw an exception, display error and return failed
                if (error)
                {
                    TaskDialog.Show("Error", "Part Library failed.");
                }
            }
        }


        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool SetForegroundWindow(IntPtr hWnd);

        /// <summary>
        /// Gets the path to the folder where this files compiled code lies 
        /// </summary>
        public static string AssemblyDirectory
        {
            get { return System.IO.Path.GetDirectoryName(new System.Uri(System.Reflection.Assembly.GetExecutingAssembly().CodeBase).LocalPath); }
        }
    }
}
