using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Threading;
using Newtonsoft.Json;
using Rhino;
using Rhino.Commands;
using Rhino.Geometry;
using Rhino.Input;
using Rhino.Input.Custom;

namespace Part_Library_Rhino
{
    public class PartLibraryRhinoCommand : Command
    {

        // create an object taking care of the messaging (an invisible Forms.Form)
        public static MessageHandler messagehandler = new MessageHandler();

        public const int WM_SYSCOMMAND = 0x0112;
        public const int SC_CLOSE = 0xF060;

        [DllImport("user32.dll")]
        public static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, int wParam, int lParam);

        public static Dispatcher RhinoDispatcher;

        public PartLibraryRhinoCommand()
        {
            // Rhino only creates one instance of each command class defined in a
            // plug-in, so it is safe to store a refence in a static property.
            Instance = this;
        }

        ///<summary>The only instance of this command.</summary>
        public static PartLibraryRhinoCommand Instance
        {
            get; private set;
        }

        ///<returns>The command name as it appears on the Rhino command line.</returns>
        public override string EnglishName
        {
            get { return "Part_Library"; }
        }

        protected override Result RunCommand(RhinoDoc doc, RunMode mode)
        {

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
                        }
                    }
                }

                // execute the library window process
                Process process = new Process();
                process.StartInfo.FileName = AssemblyDirectory + "\\Part_Library_App.exe";
                process.StartInfo.Arguments = messagehandler.Handle.ToString(); // pass the MessageHandler's window handle the the process as a command line argument
                process.Start();

                MessageHandler.library_pid = process.Id; // grab the PID so we can kill the process if required;

                Thread _thread = new Thread(CheckForChanges);

                _thread.Start();

                RhinoDispatcher = Dispatcher.CurrentDispatcher;
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            return Result.Success;
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
                        RhinoDispatcher.BeginInvoke(new Action(() =>
                        {
                            LibraryAction();
                        }));
                    }

                    // Wait a moment and relinquish control before
                    // next check for pending database updates.
                    Thread.Sleep(_timeout);
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public static void LibraryAction()
        {
            if (MessageHandler.LibraryToAppActions.Count > 0)
            {
                string action = MessageHandler.LibraryToAppActions.Dequeue();
                //MessageBox.Show(action);
                Dictionary<string, string> actionD = JsonConvert.DeserializeObject<Dictionary<string, string>>(action);

                if (actionD["option"] == "OPEN")
                {
                    try
                    {
                        List<string> files = Directory.GetFiles(actionD["hyperlink"]).ToList();
                        string dwg = "";
                        string rhinofile = "";
                        foreach (string file in files)
                        {
                            if (file.Contains(".dwg"))
                            {
                                dwg = file;
                            }
                            if (file.Contains(".3dm"))
                            {
                                rhinofile = file;
                            }
                        }
                        if (rhinofile != "")
                        {
                            bool alreadyOpen = false;
                            RhinoApp.WriteLine("Opening Rhino");
                            //RhinoDoc.Open(rhinofile, out alreadyOpen);
                            string command = "_Open " + '"' + dwg + '"' + " _Enter";
                            Rhino.RhinoApp.RunScript(command, false);
                        }
                        else if (dwg != "")
                        {
                            bool alreadyOpen = false;
                            RhinoApp.WriteLine("Opening DWG");
                            //RhinoDoc.Open(dwg, out alreadyOpen);
                            string command = "_Open " + '"' + dwg + '"' + " _Enter";
                            Rhino.RhinoApp.RunScript(command, false);
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
                        List<string> files = Directory.GetFiles(actionD["hyperlink"]).ToList();
                        string dwg = "";
                        foreach (string file in files)
                        {
                            if (file.Contains(".dwg"))
                            {
                                dwg = file;
                            }
                        }
                        if (dwg != "")
                        {
                            RhinoApp.WriteLine("Loading DWG");
                            string command = "_Import " + '"' + dwg + '"' + " _Enter";
                            Rhino.RhinoApp.RunScript(command, false);
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
                        List<string> files = Directory.GetFiles(actionD["hyperlink"]).ToList();
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

                // do your AutoCAD tasks here
                //MessageBox.Show("Text received from the Library window: " + actionD);

                // reply something back
                //MessageHandler.AppToLibraryActions.Enqueue("A message from the AutoCAD plugin - " + DateTime.Now.ToString());

            }
        }
    }
}
