using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Autodesk.Revit.UI;

namespace Part_Library_Revit
{
    public class App : Autodesk.Revit.UI.IExternalApplication
    {

        // create an object taking care of the messaging (an invisible Forms.Form)
        public static MessageHandler messagehandler = new MessageHandler();

        public const int WM_SYSCOMMAND = 0x0112;
        public const int SC_CLOSE = 0xF060;

        [DllImport("user32.dll")]
        public static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, int wParam, int lParam);

        // define a method that will create our tab and button
        static void AddRibbonPanel(UIControlledApplication application)
        {
            String tabName = "Island";
            String panelName = "Tools";
            try
            {
                // Create a custom ribbon tab
                application.CreateRibbonTab(tabName);
            }
            catch
            {

            }
            try
            {
                // Add a new ribbon panel
                Autodesk.Revit.UI.RibbonPanel ribbonPanel = application.CreateRibbonPanel(tabName, panelName);
            }
            catch
            {

            }

            Autodesk.Revit.UI.RibbonPanel panel = application.GetRibbonPanels(tabName).FirstOrDefault(n => n.Name.Equals(panelName, StringComparison.InvariantCulture));

            // Get dll assembly path
            string thisAssemblyPath = Assembly.GetExecutingAssembly().Location;

            // create push button for CurveTotalLength
            PushButtonData b1Data = new PushButtonData(
                "cmdPart_Library",
                "Part Library",
                thisAssemblyPath,
                "Part_Library.Command");

            PushButton pb1 = panel.AddItem(b1Data) as PushButton;
            pb1.ToolTip = "Part Library";
            Image img = Resource.pLibraryico;
            ImageSource imgSrc = GetImageSource(img);
            pb1.LargeImage = imgSrc;
        }

        public Result OnShutdown(UIControlledApplication application)
        {
            // make sure the Library process is closed
            if (MessageHandler.library_pid != 0)
            {
                // this could be enough, but rather try to close the process 
                //SendMessage(MessageHandler.Library_hwnd, WM_SYSCOMMAND, SC_CLOSE, 0);

                Process[] processes = Process.GetProcesses();

                foreach (Process p in processes)
                {
                    if (p.Id == MessageHandler.library_pid)
                    {
                        IntPtr hwnd = p.MainWindowHandle;
                        SendMessage(hwnd, WM_SYSCOMMAND, SC_CLOSE, 0);
                        break;
                    }
                }
            }

            return Result.Succeeded;
        }

        public Result OnStartup(UIControlledApplication application)
        {
            // call our method that will load up our toolbar
            AddRibbonPanel(application);

            return Result.Succeeded;
        }

        private static BitmapSource GetImageSource(Image img)
        {
            BitmapImage bmp = new BitmapImage();

            using (MemoryStream ms = new MemoryStream())
            {
                img.Save(ms, ImageFormat.Png);
                ms.Position = 0;

                bmp.BeginInit();

                bmp.CacheOption = BitmapCacheOption.OnLoad;
                bmp.UriSource = null;
                bmp.StreamSource = ms;

                bmp.EndInit();
            }
            return bmp;
        }
    }
}
