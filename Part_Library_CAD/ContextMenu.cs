using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using sio = System.IO;
using sw = System.Windows;


using Autodesk.AutoCAD;
using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using AcAp = Autodesk.AutoCAD.ApplicationServices.Application;
using acw = Autodesk.AutoCAD.Windows;
using System.IO;
using System.Windows;
using Autodesk.Windows;
using System.Windows.Controls;
using Application = Autodesk.AutoCAD.ApplicationServices.Application;

/// RibbonHelper class
namespace Autodesk.AutoCAD.Ribbon
{
    public class RibbonHelper
    {
        Action<RibbonControl> action = null;
        bool idleHandled = false;
        bool created = false;

        RibbonHelper(Action<RibbonControl> action)
        {
            if (action == null)
                throw new ArgumentNullException("initializer");
            this.action = action;
            SetIdle(true);
        }

        /// <summary>
        /// 
        /// Pass a delegate that takes the RibbonControl
        /// as its only argument, and it will be invoked
        /// when the RibbonControl is available. 
        /// 
        /// If the RibbonControl exists when the constructor
        /// is called, the delegate will be invoked on the
        /// next idle event. Otherwise, the delegate will be
        /// invoked on the next idle event following the 
        /// creation of the RibbonControl.
        /// 
        /// </summary>

        public static void OnRibbonFound(Action<RibbonControl> action)
        {
            new RibbonHelper(action);
        }

        void SetIdle(bool value)
        {
            if (value ^ idleHandled)
            {
                if (value)
                    Application.Idle += idle;
                else
                    Application.Idle -= idle;
                idleHandled = value;
            }
        }

        void idle(object sender, EventArgs e)
        {
            SetIdle(false);
            if (action != null)
            {
                var ps = RibbonServices.RibbonPaletteSet;
                if (ps != null)
                {
                    var ribbon = ps.RibbonControl;
                    if (ribbon != null)
                    {
                        action(ribbon);
                        action = null;
                    }
                }
                else if (!created)
                {
                    created = true;
                    RibbonServices.RibbonPaletteSetCreated +=
                       ribbonPaletteSetCreated;
                }
            }
        }

        void ribbonPaletteSetCreated(object sender, EventArgs e)
        {
            RibbonServices.RibbonPaletteSetCreated
               -= ribbonPaletteSetCreated;
            SetIdle(true);
        }
    }
}

namespace Part_Library_CAD
{
    class ContextMenu
    {
        public static void Create()
        {
            RibbonTab tab1 = CreateRibbon("Island");
            if (tab1 == null)
            {
                return;
            }

            RibbonPanelSource rpannel = CreateButtonPannel("Tools", tab1);
            if (rpannel == null)
            {
                return;
            }


            RibbonButton b1 = CreateButton("Part Library", "Part_Library", rpannel);

        }

        private static RibbonTab CreateRibbon(string str)
        {
            //1. Create ribbon control
            RibbonControl rc = ComponentManager.Ribbon;
            //2. Create ribbon tab
            RibbonTab rt = new RibbonTab();
            //3. ribbon tab information assignment
            rt.Title = str;
            rt.Id = str;

            //4. add ribbon tab to the ribbon control's tab
            rc.Tabs.Add(rt);
            //5. optional: set ribbon tab active
            rt.IsActive = true;
            return rt;
        }

        private static RibbonPanelSource CreateButtonPannel(string str, RibbonTab rtab)
        {
            //1. Create  panel source
            RibbonPanelSource bp = new RibbonPanelSource();

            //3. ribbon panel source information assignment
            bp.Title = str;
            //4. create ribbon pannel
            RibbonPanel rp = new RibbonPanel();
            //4. add ribbon pannel to the ribbon pannel source
            rp.Source = bp;
            //5. add ribbon pannel to the tobs
            rtab.Panels.Add(rp);
            return bp;
        }

        private static RibbonButton CreateButton(string str, string CommandParameter, RibbonPanelSource rps)
        {
            //1. Create  Button
            RibbonButton button = new RibbonButton();


            //3. ribbon panel source information assignment
            button.Text = str;
            button.Id = str;
            button.CommandParameter = CommandParameter; // name of the command
            button.ShowImage = true;
            button.ShowText = true;
            button.Size = RibbonItemSize.Large;

            button.Orientation = Orientation.Vertical; // from system.windows.controls

            button.LargeImage = BitmapHelper.ConvertBitmapToBitmapImage.Convert(Resource.pLibraryico);

            // 4. to add command to the button write the line below
            button.CommandHandler = new AdskCommandHandler();

            //5. add ribbon button pannel to the ribbon pannel source
            rps.Items.Add(button);
            return button;

        }

        public class AdskCommandHandler : System.Windows.Input.ICommand
        {
            public bool CanExecute(object parameter)
            {
                return true;
            }

            public event EventHandler CanExecuteChanged;

            public void Execute(object parameter)
            {
                //is from a Ribbon Button?
                RibbonButton ribBtn = parameter as RibbonButton;
                if (ribBtn != null)
                {
                    string cmdName = (String)ribBtn.CommandParameter;

                    // remove all empty spaces and add
                    // a new one at the end.
                    cmdName = "_." + cmdName.TrimEnd() + " ";

                    // execute the command using command prompt
                    Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.
                      SendStringToExecute(cmdName,
                      true, false, false);
                }
            }
        }
    }
}
