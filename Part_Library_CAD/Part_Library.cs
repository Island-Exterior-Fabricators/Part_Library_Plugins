﻿using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Windows;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MgdAcApplication = Autodesk.AutoCAD.ApplicationServices.Application;
using MgdAcDocument = Autodesk.AutoCAD.ApplicationServices.Document;
using AcWindowsNS = Autodesk.AutoCAD.Windows;
using Autodesk.Windows;
using System.Reflection;
using System.Windows.Media.Imaging;
using System.Drawing;
using System.Windows;
using System.Threading;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.IO;

namespace Part_Library_CAD
{
    public class Part_Library
    {
        public class Initialization : IExtensionApplication
        {
            // create an object taking care of the messaging (an invisible Forms.Form)
            public static MessageHandler messagehandler = new MessageHandler();

            public const int WM_SYSCOMMAND = 0x0112;
            public const int SC_CLOSE = 0xF060;

            [DllImport("user32.dll")]
            public static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, int wParam, int lParam);

            private DocumentCollection docMgr = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager;

            public void Initialize()
            {
                try
                {
                    /// When the ribbon is available, do
                    /// initialization for it:
                    Autodesk.AutoCAD.Ribbon.RibbonHelper.OnRibbonFound(SetupRibbon);

                    /// TODO: Initialize your app
                }
                catch (System.Exception ex)
                {
                    Document doc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
                    if (doc != null)
                        doc.Editor.WriteMessage(ex.ToString());
                }
            }

            void SetupRibbon(RibbonControl ribbon)
            {
                /// TODO: Place ribbon initialization code here
                ContextMenu.Create();
                /// Example only: Verify our method was called
                //Autodesk.AutoCAD.ApplicationServices.Application.ShowAlertDialog("SetupRibbon() called");
            }

            public void Terminate()
            {

            }
        }

        [CommandMethod("Part_Library", CommandFlags.Transparent)]
        public void Execute()
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
                process.StartInfo.Arguments = Initialization.messagehandler.Handle.ToString(); // pass the MessageHandler's window handle the the process as a command line argument
                process.Start();

                MessageHandler.library_pid = process.Id; // grab the PID so we can kill the process if required;

                Thread _thread = new Thread(CheckForChanges);

                _thread.Start();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message);
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
        public static async void CheckForChanges()
        {
            var dm = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager;
            var doc = dm.MdiActiveDocument;
            if (doc == null)
            {
                return;
            }
            var ed = doc.Editor;
            try
            {
                while (true)
                {
                    if (MessageHandler.LibraryToAppActions.Count > 0)
                    {
                        libraryInContext(dm, ed);
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


        private static bool _drawing = false;

        private static async void libraryInContext(DocumentCollection dc, Editor ed)
        {
            // Protect the command-calling function with a flag to avoid
            // eInvalidInput failures
            if (!_drawing)
            {
                _drawing = true;
                // Call our square creation function asynchronously
                await dc.ExecuteInCommandContextAsync(async (o) => LibraryAction(ed), null);
                _drawing = false;
            }
        }


        private static void LibraryAction(Editor ed)
        {
            if (MessageHandler.LibraryToAppActions.Count > 0)
            {
                string action = MessageHandler.LibraryToAppActions.Dequeue();
                //MessageBox.Show(action);
                Dictionary<string, string> actionD = JsonConvert.DeserializeObject<Dictionary<string, string>>(action);

                if (actionD["option"] == "OPEN")
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
                        DocumentCollection docMgr = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager;
                        DocumentCollectionExtension.Open(docMgr, dwg, false);
                    }
                }
                else if (actionD["option"] == "LOAD")
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
                        Database sourceDb = new Database(false, true);
                        Document doc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
                        Database cDB = doc.Database;
                        sourceDb.ReadDwgFile(dwg, FileOpenMode.OpenForReadAndAllShare, true, ""); //Read the DWG into a side database
                        String fileName = Path.GetFileNameWithoutExtension(dwg);
                        cDB.Insert(fileName, sourceDb, false);

                        Autodesk.AutoCAD.DatabaseServices.TransactionManager tm = cDB.TransactionManager;

                        try
                        {
                            ObjectIdCollection blockIds = new ObjectIdCollection();
                            using (Transaction myT = tm.StartTransaction())
                            {
                                // Open the block table
                                BlockTable bt =
                                    (BlockTable)tm.GetObject(cDB.BlockTableId,
                                                            OpenMode.ForRead,
                                                            false);
                                
                                BlockTableRecord btr = myT.GetObject(bt[fileName], OpenMode.ForRead) as BlockTableRecord;

                                BlockReference br = new BlockReference(Point3d.Origin, btr.ObjectId);

                                if (BlockMovingRotating.Jig(br))
                                {
                                    BlockTableRecord modelspace = myT.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;
                                    modelspace.AppendEntity(br);
                                    myT.AddNewlyCreatedDBObject(br, true);
                                    myT.Commit();
                                }
                                else
                                {
                                    myT.Abort();
                                }
                                btr.Dispose();
                            }
                        }
                        catch (System.Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                    }
                }
                else if (actionD["option"] == "VIEW")
                {
                    MessageBox.Show("VIEW");
                }

                // do your AutoCAD tasks here
                //MessageBox.Show("Text received from the Library window: " + actionD);

                // reply something back
                //MessageHandler.AppToLibraryActions.Enqueue("A message from the AutoCAD plugin - " + DateTime.Now.ToString());

            }
        }

        public class BlockMovingRotating : EntityJig
        {
            #region Fields

            public int mCurJigFactorNumber = 1;

            private Point3d mPosition;    // Factor #1
            private double mRotation;    // Factor #2

            #endregion

            #region Constructors

            public BlockMovingRotating(Entity ent)
                : base(ent)
            {
            }

            #endregion

            #region Overrides

            protected override bool Update()
            {
                switch (mCurJigFactorNumber)
                {
                    case 1:
                        (Entity as BlockReference).Position = mPosition;
                        break;
                    case 2:
                        (Entity as BlockReference).Rotation = mRotation;
                        break;
                    default:
                        return false;
                }

                return true;
            }

            protected override SamplerStatus Sampler(JigPrompts prompts)
            {
                switch (mCurJigFactorNumber)
                {
                    case 1:
                        JigPromptPointOptions prOptions1 = new JigPromptPointOptions("\nBlock position:");
                        PromptPointResult prResult1 = prompts.AcquirePoint(prOptions1);
                        if (prResult1.Status == PromptStatus.Cancel) return SamplerStatus.Cancel;

                        if (prResult1.Value.Equals(mPosition))
                        {
                            return SamplerStatus.NoChange;
                        }
                        else
                        {
                            mPosition = prResult1.Value;
                            return SamplerStatus.OK;
                        }
                    case 2:
                        JigPromptAngleOptions prOptions2 = new JigPromptAngleOptions("\nBlock rotation angle:");
                        prOptions2.BasePoint = (Entity as BlockReference).Position;
                        prOptions2.UseBasePoint = true;
                        PromptDoubleResult prResult2 = prompts.AcquireAngle(prOptions2);
                        if (prResult2.Status == PromptStatus.Cancel) return SamplerStatus.Cancel;

                        if (prResult2.Value.Equals(mRotation))
                        {
                            return SamplerStatus.NoChange;
                        }
                        else
                        {
                            mRotation = prResult2.Value;
                            return SamplerStatus.OK;
                        }
                    default:
                        break;
                }

                return SamplerStatus.OK;
            }

            #endregion

            #region Method to Call

            public static bool Jig(BlockReference ent)
            {
                try
                {
                    Editor ed = MgdAcApplication.DocumentManager.MdiActiveDocument.Editor;
                    BlockMovingRotating jigger = new BlockMovingRotating(ent);
                    PromptResult pr;
                    do
                    {
                        pr = ed.Drag(jigger);
                    } while (pr.Status != PromptStatus.Cancel &&
                                pr.Status != PromptStatus.Error &&
                                pr.Status != PromptStatus.Keyword &&
                                jigger.mCurJigFactorNumber++ <= 2);

                    return pr.Status == PromptStatus.OK;
                }
                catch
                {
                    return false;
                }
            }

            #endregion
        }
    }
}
