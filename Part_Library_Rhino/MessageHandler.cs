using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Part_Library_Rhino
{
    public class MessageHandler : Form
    {

        // a lot of static here, but I guess it should be safe in this case
        public static IntPtr library_hwnd = IntPtr.Zero;
        public static int library_pid = 0;
        public static Queue<string> LibraryToAppActions = new Queue<string>();
        public static Queue<string> AppToLibraryActions = new Queue<string>();

        Timer actionHandler = new Timer() { Interval = 300 };

        public MessageHandler() : base()
        {
            // poll for actions to be sent to Library window
            actionHandler.Tick += ActionHandler_Tick;
            actionHandler.Start();
        }

        private void ActionHandler_Tick(object sender, EventArgs e)
        {
            if (AppToLibraryActions.Count > 0)
            {
                // send the message to the Library window
                sendMessage(AppToLibraryActions.Dequeue());
            }
        }

        #region code from https://code.msdn.microsoft.com/windowsapps/CSReceiveWMCOPYDATA-dbbc7ed7/sourcecode?fileId=21692&pathId=224762670

        private void sendMessage(string message)
        {

            // Prepare the COPYDATASTRUCT struct with the data to be sent. 
            MyStruct myStruct;

            myStruct.Message = message;

            // Marshal the managed struct to a native block of memory. 
            int myStructSize = Marshal.SizeOf(myStruct);
            IntPtr pMyStruct = Marshal.AllocHGlobal(myStructSize);
            try
            {
                Marshal.StructureToPtr(myStruct, pMyStruct, true);

                COPYDATASTRUCT cds = new COPYDATASTRUCT();
                cds.cbData = myStructSize;
                cds.lpData = pMyStruct;

                // Send the COPYDATASTRUCT struct through the WM_COPYDATA message to  
                // the receiving window. (The application must use SendMessage,  
                // instead of PostMessage to send WM_COPYDATA because the receiving  
                // application must accept while it is guaranteed to be valid.) 
                NativeMethod.SendMessage(library_hwnd, WM_COPYDATA, this.Handle, ref cds);

                int result = Marshal.GetLastWin32Error();
                if (result != 0)
                {
                    // sometimes there's a false alert here... remove the msgbox
                    //MessageBox.Show(String.Format(
                    //    "SendMessage(WM_COPYDATA) (App->Library) failed w/err 0x{0:X}", result));
                }
            }
            finally
            {
                Marshal.FreeHGlobal(pMyStruct);
            }
        }

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll", EntryPoint = "SetWindowLongPtr")]
        private static extern IntPtr SetWindowLongPtr64(IntPtr hWnd, int nIndex, IntPtr dwNewLong);


        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WM_COPYDATA)
            {
                // Get the COPYDATASTRUCT struct from lParam. 
                COPYDATASTRUCT cds = (COPYDATASTRUCT)m.GetLParam(typeof(COPYDATASTRUCT));

                // If the size matches 
                if (cds.cbData == Marshal.SizeOf(typeof(MyStruct)))
                {
                    // Marshal the data from the unmanaged memory block to a  
                    // MyStruct managed struct. 
                    MyStruct myStruct = (MyStruct)Marshal.PtrToStructure(cds.lpData, typeof(MyStruct));

                    if (myStruct.Message.StartsWith("hwnd:")) // this is how to get the Library window handle (to App) when the window in initialized
                    {
                        // store the window handle of the library window
                        library_hwnd = new IntPtr(int.Parse(myStruct.Message.Replace("hwnd:", "")));
                        // get the App window handle
                        IntPtr Appwindowhandle = System.Diagnostics.Process.GetCurrentProcess().MainWindowHandle;
                        // assign App as the owner of the library window
                        SetWindowLongPtr64(library_hwnd, -8, Appwindowhandle);

                    }
                    else if (myStruct.Message.StartsWith("close:"))
                    {
                        // clear the pointers to the library window (actually, this is not required, but hygenic)
                        library_hwnd = IntPtr.Zero;
                        library_pid = 0;
                    }
                    else
                    {
                        // add the message to the queue (to be processed in the App plugin)
                        LibraryToAppActions.Enqueue(myStruct.Message);

                        // activate App to make the application.Idling-event trigger
                        IntPtr Appwindowhandle = System.Diagnostics.Process.GetCurrentProcess().MainWindowHandle;
                        SetForegroundWindow(Appwindowhandle);
                    }

                }
            }

            base.WndProc(ref m);
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        internal struct MyStruct
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 4096)]
            public string Message;
        }

        /// <summary> 
        /// An application sends the WM_COPYDATA message to pass data to another  
        /// application. 
        /// </summary> 
        internal const int WM_COPYDATA = 0x004A;


        /// <summary> 
        /// The COPYDATASTRUCT structure contains data to be passed to another  
        /// application by the WM_COPYDATA message.  
        /// </summary> 
        [StructLayout(LayoutKind.Sequential)]
        internal struct COPYDATASTRUCT
        {
            public IntPtr dwData;       // Specifies data to be passed 
            public int cbData;          // Specifies the data size in bytes 
            public IntPtr lpData;       // Pointer to data to be passed 
        }

        /// <summary> 
        /// The class exposes Windows APIs to be used in this code sample. 
        /// </summary> 
        [SuppressUnmanagedCodeSecurity]
        internal class NativeMethod
        {
            /// <summary> 
            /// Sends the specified message to a window or windows. The SendMessage  
            /// function calls the window procedure for the specified window and does  
            /// not return until the window procedure has processed the message.  
            /// </summary> 
            /// <param name="hWnd"> 
            /// Handle to the window whose window procedure will receive the message. 
            /// </param> 
            /// <param name="Msg">Specifies the message to be sent.</param> 
            /// <param name="wParam"> 
            /// Specifies additional message-specific information. 
            /// </param> 
            /// <param name="lParam"> 
            /// Specifies additional message-specific information. 
            /// </param> 
            /// <returns></returns> 
            [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
            public static extern IntPtr SendMessage(IntPtr hWnd, int Msg,
                IntPtr wParam, ref COPYDATASTRUCT lParam);

        }

        #endregion


    }
}
