﻿using Autodesk.DataManagement.Client.Framework.Vault.Currency.Entities;
using Autodesk.DataManagement.Client.Framework.Vault.Currency.Properties;
using Autodesk.DataManagement.Client.Framework.Vault.Forms.Controls;
using MaterialSkin;
using MaterialSkin.Controls;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Windows.Forms;
using Control = System.Windows.Forms.Control;

using Vault = Autodesk.DataManagement.Client.Framework.Vault;
using Forms = Autodesk.DataManagement.Client.Framework.Vault.Forms;
using Autodesk.Connectivity.WebServices;
using Autodesk.Connectivity.WebServicesTools;
using System.Configuration;
using System.Reflection;
using Autodesk.DataManagement.Client.Framework.Vault.Forms.Models;
using System.Diagnostics;
using System.Drawing.Imaging;

namespace Part_Library_App_Vault
{
    public partial class LibraryForm : MaterialForm
    {
        public static Queue<string> LibraryToRevitActions = new Queue<string>();
        public static Queue<string> RevitToLibraryActions = new Queue<string>();

        System.Windows.Forms.Timer action_timer = new System.Windows.Forms.Timer() { Interval = 300 };

        List<Autodesk.Connectivity.WebServices.Item> PARTS = new List<Autodesk.Connectivity.WebServices.Item>();
        Bitmap defaultDWG = Resource.Autodesk_DWG_icon;
        ImageList imageList = new ImageList();

        List<ListViewItem> Items = new List<ListViewItem>();
        List<ListViewItem> _Items = new List<ListViewItem>();
        List<ListViewItem> ItemsIMG = new List<ListViewItem>();
        List<ListViewItem> FilteredItems = new List<ListViewItem>();
        List<Filter> filterSet = new List<Filter>();
        Dictionary<string, int> colindex = new Dictionary<string, int>();
        List<Control> filterControls = new List<Control>();


        public List<PartTypes.Part> selected = new List<PartTypes.Part>();
        public string option = null;

        ListViewItem lastHover = null;
        ListViewItem lastFocus = null;

        ThumbForm tf = null;

        int filterbuttonoffset = 12;

        public LibraryForm()
        {
            InitializeComponent();

            var materialSkinManager = MaterialSkinManager.Instance;
            materialSkinManager.AddFormToManage(this);
            materialSkinManager.Theme = MaterialSkinManager.Themes.LIGHT;
            materialSkinManager.ColorScheme = new ColorScheme(Primary.BlueGrey800, Primary.BlueGrey900, Primary.BlueGrey500, Accent.LightBlue200, TextShade.WHITE);

            listView1.BackColor = materialSkinManager.BackgroundColor;

            List<Autodesk.Connectivity.WebServices.Item> parts = InitVault();

            BuildPARTS(parts);
            SeedListViewW(parts);

            CycleBackground();


            // send this window's hwnd back (finish the handshake)
            sendMessage("hwnd:" + this.Handle.ToString());

            action_timer.Tick += Action_timer_Tick;
            action_timer.Start();
        }

        private void Action_timer_Tick(object sender, EventArgs e)
        {

            if (LibraryToRevitActions.Count > 0)
            {
                this.Visible = false;
                string action = LibraryToRevitActions.Dequeue();
                sendMessage(action);
            }

            if (RevitToLibraryActions.Count > 0)
            {
                string action = RevitToLibraryActions.Dequeue();
                // call a javascript-method in the html-file
                //browser.ExecuteScriptAsync("receiveFromRevit", new string[] { action });
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
                NativeMethod.SendMessage(Program.app_hwnd, WM_COPYDATA, this.Handle, ref cds);

                int result = Marshal.GetLastWin32Error();
                if (result != 0)
                {
                    // sometimes there's a false alarm here... remove the msgbox
                    //MessageBox.Show(String.Format(
                    //    "SendMessage(WM_COPYDATA) (Browser->Revit) failed w/err 0x{0:X}", result));
                }
            }
            finally
            {
                Marshal.FreeHGlobal(pMyStruct);
            }

        }

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
                    MyStruct myStruct = (MyStruct)Marshal.PtrToStructure(cds.lpData,
                        typeof(MyStruct));

                    RevitToLibraryActions.Enqueue(myStruct.Message);

                    // show the window again
                    this.Visible = true;
                    this.Activate();

                }
            }

            base.WndProc(ref m);
        }


        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        internal struct MyStruct
        {
            // Kim: use 4 MB message size. Using a dynamic size is probably possible, but would require some Googling 
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 4096)]
            public string Message;
        }

        /// <summary> 
        /// An application sends the WM_COPYDATA message to pass data to another  
        /// application 
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


            /// <summary> 
            /// The FindWindow function retrieves a handle to the top-level window  
            /// whose class name and window name match the specified strings. This  
            /// function does not search child windows. This function does not  
            /// perform a case-sensitive search. 
            /// </summary> 
            /// <param name="lpClassName">Class name</param> 
            /// <param name="lpWindowName">Window caption</param> 
            /// <returns></returns> 
            [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
            public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
        }

        #endregion

        public class Filter
        {
            public string Field { get; set; }
            public string Value { get; set; }
            public int Index { get; set; }
        }

        public class ThumbSet
        {
            public string ID { get; set; }
            public Image IMG { get; set; }
            public bool last { get; set; }
        }

        private void BuildPARTS(List<Autodesk.Connectivity.WebServices.Item> parts)
        {
            PARTS = parts;
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;
            foreach (Autodesk.Connectivity.WebServices.Item f in PARTS)
            {
                try
                {
                    ThumbSet ts = new ThumbSet();
                    Byte[] imgbyte = v_Conn.WebServiceManager.PropertyService.GetProperties("ITEM", new long[] { f.Id }, new long[] { thumbID }).FirstOrDefault().Val as Byte[];
                    if (imgbyte == null)
                    {

                    }
                    else
                    {
                        using (var ms = new MemoryStream(imgbyte))
                        {
                            using (System.IO.BinaryReader br = new System.IO.BinaryReader(ms))
                            {
                                // Vault saves thumbnails as jpeg per developer comments for 2013, assume same
                                // https://justonesandzeros.typepad.com/blog/2012/05/thumbnail-optimization.html#:~:text=One%20of%20the%20optimizations%20in,JPEG%20when%20at%20thumbnail%20size.

                                // the bytes do not represent a metafile, try to convert to an Image
                                ms.Seek(0, System.IO.SeekOrigin.Begin);
                                ts.IMG = Image.FromStream(ms, true, false);
                            }
                        }
                        ts.ID = v_Conn.WebServiceManager.PropertyService.GetProperties("ITEM", new long[] { f.Id }, new long[] { nameID }).FirstOrDefault().Val.ToString();
                        string newPathAndName = @"C:\Documents\ExtrusionLibrary\Thumbnails\" + ts.ID + ".jpeg";

                        if (System.IO.File.Exists(newPathAndName))
                        {

                        }
                        else
                        {
                            if (ts.IMG != null)
                            {
                                System.IO.File.WriteAllBytes(newPathAndName, imgbyte);
                                ts.IMG = Image.FromFile(newPathAndName);
                                worker.ReportProgress(100, ts);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {

                    MessageBox.Show("END | " + ex.Message);
                }
            }

            ThumbSet tsLast = new ThumbSet();
            tsLast.last = true;
            worker.ReportProgress(100, tsLast);
        }

        private void CycleBackground()
        {
            BackgroundWorker backgroundWorker1 = new BackgroundWorker();
            backgroundWorker1.WorkerReportsProgress = true;
            backgroundWorker1.ProgressChanged += backgroundWorker1_ProgressChanged;
            backgroundWorker1.DoWork += backgroundWorker1_DoWork;
            backgroundWorker1.RunWorkerAsync(PARTS);
        }

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            // This code executes in the UI thread, no problem to 
            // work with Controls like the ListView
            try
            {
                ThumbSet ts = e.UserState as ThumbSet;
                if (ts.last)
                {
                    CycleBackground();
                }
                else
                {
                    if (imageList.Images.ContainsKey(ts.ID))
                    {
                        imageList.Images.RemoveByKey(ts.ID);
                    }
                    imageList.Images.Add(ts.ID, ts.IMG);
                    ListViewItem lvi = listView1.FindItemWithText(ts.ID);
                    lvi.ImageKey = ts.ID;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void SeedListViewW(List<Autodesk.Connectivity.WebServices.Item> parts)
        {
            listView1.BeginUpdate();
            List<string> headers = new List<string>() { };
            PropertyDefinition pnDef = null;
            headers.Add("Part Number");
            foreach (PropertyDefinition s in propDefs.Values)
            {
                if (s.DisplayName == "Part Number")
                {
                    pnDef = s;
                } else
                {
                    headers.Add(s.DisplayName);
                }
            }

            string longest = "";
            List<ColumnHeader> cc = new List<ColumnHeader>();
            int i = 0;
            foreach (string h in headers)
            {
                cc.Add(this.listView1.Columns.Add(h, 200));
                this.filterFieldCbo.Items.Add(h);
                colindex.Add(h, i++);

                if (h.Length > longest.Length)
                {
                    longest = h;
                }
            }

            AdjustWidthComboBox_DropDown(longest);
            imageList = new ImageList();
            int imgDim = 64;
            imageList.ImageSize = new Size(imgDim, imgDim);
            imageList.ColorDepth = ColorDepth.Depth32Bit;
            listView1.SmallImageList = imageList;
            imageList.Images.Add("default", defaultDWG);
            int index = 0;
            bool found = false;

            if (!System.IO.Directory.Exists(@"C:\Documents\ExtrusionLibrary\Thumbnails"))
            {
                System.IO.Directory.CreateDirectory(@"C:\Documents\ExtrusionLibrary\Thumbnails");
            }

            if (parts != null)
            {
                //Items = GetVersions(parts);
                Items = GetAllVersions(parts);
            }
            listView1.Items.AddRange(Items.ToArray());
            FilteredItems.AddRange(Items);

            this.listView1.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
            foreach (ColumnHeader ch in cc)
            {
                ch.Width += 30;
            }

            listView1.EndUpdate();

            listView1.SuspendLayout();

            // hide/delete unused columns
            for (int col = versionCull.Length-1; col > 0; col--)
            {
                if (versionCull[col] != 1)
                {
                    //MessageBox.Show(listView1.Columns[col].Text);
                    listView1.Columns.RemoveAt(col);
                }
            }

            listView1.ResumeLayout();
            lastHover = listView1.Items[0];
        }

        private void listView1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (this.listView1.FocusedItem.Bounds.Contains(e.Location))
                {
                    this.materialContextMenuStrip1.Show(Cursor.Position);
                }
            }
        }

        private void listView1_ItemMouseHover(object sender, ListViewItemMouseHoverEventArgs e)
        {
            if (tf != null)
            {
                tf.Close();
                tf.Dispose();
                tf = null;
            }
            try { lastHover.BackColor = System.Drawing.Color.White; } catch { } finally { lastHover = e.Item as ListViewItem; }
            ListViewItem lvi = e.Item as ListViewItem;
            try
            {
                String filepath = @"C:\Documents\ExtrusionLibrary\Thumbnails";
                if (!System.IO.Directory.Exists(filepath))
                {
                    System.IO.Directory.CreateDirectory(filepath);
                }

                List<string> files = Directory.GetFiles(filepath).ToList();
                string dwg = "";
                foreach (string imgfile in files)
                {
                    // Use local thumb resources
                    if (Path.GetFileNameWithoutExtension(imgfile) == lvi.Name)
                    {
                        Bitmap thumb = (Bitmap)Bitmap.FromFile(imgfile);

                        double ratioL = 0;
                        if (thumb.Width > thumb.Height)
                        {
                            ratioL = 200.0 / thumb.Width;
                        }
                        else
                        {
                            ratioL = 200.0 / thumb.Height;
                        }
                        Bitmap thumbL = new Bitmap(thumb, new Size((int)Math.Round(thumb.Width * ratioL), (int)Math.Round(thumb.Height * ratioL)));
                        tf = new ThumbForm(thumbL);

                        tf.StartPosition = FormStartPosition.Manual;
                        tf.Location = new Point(this.Location.X - (tf.Width + 5), this.Location.Y);
                        tf.Show(this);
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void listView1_MouseLeave(object sender, EventArgs e)
        {
            //this.materialCard1.Visible = false;
            if (tf != null)
            {
                tf.Close();
                tf.Dispose();
                tf = null;
            }
        }

        private void listView1_MouseEnter(object sender, EventArgs e)
        {
        }


        private void resize_form(object sender, EventArgs e)
        {
            this.listView1.Height = this.Height - 180;
            this.listView1.Width = this.Width - 25;

            this.filterTextBox.Width = this.Width - 200;
            this.filtersButton.Width = this.Width - 100;
        }

        private void AdjustWidthComboBox_DropDown(string s)
        {
            ComboBox senderComboBox = this.filterFieldCbo;
            int width = senderComboBox.DropDownWidth;
            Graphics g = senderComboBox.CreateGraphics();
            System.Drawing.Font font = senderComboBox.Font;
            int vertScrollBarWidth =
                (senderComboBox.Items.Count > senderComboBox.MaxDropDownItems)
                ? SystemInformation.VerticalScrollBarWidth : 0;

            int newWidth;
            newWidth = (int)g.MeasureString(s, font).Width + vertScrollBarWidth + 20;
            senderComboBox.DropDownWidth = newWidth;
        }

        private int TextWidth(Control c, string s)
        {
            double factor = 1.15;
            Graphics g = c.CreateGraphics();
            System.Drawing.Font font = c.Font;

            int newWidth;
            newWidth = (int)Math.Round(g.MeasureString(s.ToUpper(), font).Width * factor) + 20;
            return newWidth;
        }

        private void materialFloatingActionButton1_Click(object sender, EventArgs e)
        {
            Filter f = new Filter();
            try
            {
                f.Field = filterFieldCbo.SelectedItem.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show("You must provide a Field to filter by.");
                return;
            }
            try
            {
                if (filterTextBox.Text == "" || filterTextBox.Text == null)
                {
                    MessageBox.Show("You must provide text to filter by.");
                    return;
                }
                f.Value = filterTextBox.Text;
            }
            catch (Exception ex)
            {
                MessageBox.Show("You must provide text to filter by.");
                return;
            }
            try
            {
                f.Index = colindex[f.Field];
            }
            catch (Exception ex)
            {
                MessageBox.Show("AHHHH");
            }
            filterSet.Add(f);
            this.filterFieldCbo.SelectedIndex = -1;
            this.filterFieldCbo.Refresh();
            this.filterTextBox.Clear();

            string tval = f.Field + " | " + f.Value;
            MaterialButton mb = CreateButton(tval);

            this.filtersButton.Visible = false;
            this.Controls.Add(mb);
            filterControls.Add(mb);

            filterListView();
        }

        private MaterialButton CreateButton(string tval)
        {
            MaterialButton materialButton = new MaterialButton();
            materialButton.AutoSize = false;
            materialButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowOnly;
            materialButton.Depth = 1;
            materialButton.DrawShadows = true;
            materialButton.HighEmphasis = true;
            materialButton.Location = new System.Drawing.Point(filterbuttonoffset, 116);
            materialButton.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            materialButton.MouseState = MaterialSkin.MouseState.HOVER;
            materialButton.Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Contained;
            materialButton.UseAccentColor = true;
            materialButton.UseVisualStyleBackColor = true;
            materialButton.Click += new System.EventHandler(this.deleteFilter_Click);

            materialButton.Name = "materialButton" + tval;
            materialButton.Text = tval;
            materialButton.Size = new System.Drawing.Size(TextWidth(materialButton, tval), 36);

            filterbuttonoffset += materialButton.Width + 10;

            return materialButton;
        }

        private void filterListView()
        {
            foreach (Filter f in filterSet)
            {
                List<ListViewItem> temp = new List<ListViewItem>();
                temp.AddRange(FilteredItems.Where(i => i.SubItems[f.Index].Text.ToLower().Contains(f.Value.ToLower())).ToArray());

                this.listView1.Items.Clear();
                FilteredItems.Clear();
                FilteredItems.AddRange(temp);
            }

            try
            {
                this.listView1.Items.AddRange(FilteredItems.ToArray());
            }
            catch
            {
            }

            this.listView1.Refresh();

        }

        private void clearButton_Click(object sender, EventArgs e)
        {
            clearList();

            foreach (Control c in filterControls)
            {
                c.Dispose();
            }

            filterControls.Clear();
            filtersButton.Visible = true;
            filterbuttonoffset = 12;
        }

        private void clearList()
        {
            filterSet.Clear();
            FilteredItems.Clear();
            FilteredItems.AddRange(Items);

            listView1.Items.Clear();
            listView1.Items.AddRange(Items.ToArray());
            listView1.Refresh();
        }

        private void deleteFilter_Click(object sender, EventArgs e)
        {
            MaterialButton b = sender as MaterialButton;

            List<string> split = b.Text.Split(new string[] { " | " }, StringSplitOptions.None).ToList();
            string field = String.Join(" ", split[0]);
            string val = String.Join(" ", split[1]);
            foreach (Filter f in filterSet.ToList())
            {
                if (f.Field == field && f.Value == val)
                {
                    filterSet.Remove(f);
                }
            }

            b.Visible = false;
            foreach (Control c in filterControls.ToList())
            {
                if (c == b)
                {
                    filterControls.Remove(c);
                }
                if (c.Location.X > b.Location.X)
                {
                    c.Location = new Point(c.Location.X - (b.Width + 10), c.Location.Y);
                }
            }

            filterbuttonoffset -= b.Width + 10;

            b.Dispose();
            if (filterControls.Count == 0)
            {
                filtersButton.Visible = true;
                clearList();
            }
            else
            {
                FilteredItems.Clear();
                FilteredItems.AddRange(Items);
                filterListView();
            }
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            option = "OPEN";
            setClose();
        }

        private void loadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            option = "LOAD";
            setClose();
        }

        private void viewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            option = "VIEW";
            setClose();
        }

        private void setClose()
        {
            int linkindex = 0;
            int idindex = 0;
            foreach (Filter f in filterSet)
            {
                if (f.Field == "ID")
                {
                    idindex = f.Index;
                }
                else if (f.Field == "Hyperlink")
                {
                    linkindex = f.Index;
                }
            }

            Dictionary<string, string> _dict = new Dictionary<string, string>();
            _dict["option"] = option;

            sendMessage(JsonConvert.SerializeObject(_dict));

            //this.Close();
        }

        public static Vault.Currency.Connections.Connection v_Conn = null;

        // We will collect Property Definitions here
        public static Vault.Currency.Properties.PropertyDefinitionDictionary propDefs;
        public static long[] propDefIDs = new long[] { };
        public long thumbID;
        public long nameID;
        public static Dictionary<long, PropertyDefinition> propDict = new Dictionary<long, PropertyDefinition>();

        int[] versionCull;

        static bool showMessage = false;

        public static List<FileIteration> collectionLVI = new List<FileIteration>();

        public static bool LoginCheck()
        {
            Configuration configuration = ConfigurationManager.OpenExeConfiguration(Assembly.GetExecutingAssembly().Location);

            Vault.Results.LogInResult result = null;
            result = Vault.Library.ConnectionManager.LogIn(crypt.DecryptString(configuration.AppSettings.Settings["server"].Value),
                                                            crypt.DecryptString(configuration.AppSettings.Settings["vault"].Value),
                                                            crypt.DecryptString(configuration.AppSettings.Settings["username"].Value),
                                                            crypt.DecryptString(configuration.AppSettings.Settings["password"].Value),
                                                            Vault.Currency.Connections.AuthenticationFlags.ReadOnly, null);

            if (result.Success)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static Vault.Results.LogInResult VaultLogin()
        {
            Configuration configuration = ConfigurationManager.OpenExeConfiguration(Assembly.GetExecutingAssembly().Location);

            Vault.Results.LogInResult result = null;
            result = Vault.Library.ConnectionManager.LogIn(crypt.DecryptString(configuration.AppSettings.Settings["server"].Value),
                                                            crypt.DecryptString(configuration.AppSettings.Settings["vault"].Value),
                                                            crypt.DecryptString(configuration.AppSettings.Settings["username"].Value),
                                                            crypt.DecryptString(configuration.AppSettings.Settings["password"].Value),
                                                            Vault.Currency.Connections.AuthenticationFlags.ReadOnly, null);

            return result;
        }

        public List<Autodesk.Connectivity.WebServices.Item> InitVault()
        {
            List<Autodesk.Connectivity.WebServices.Item> parts = new List<Autodesk.Connectivity.WebServices.Item>();
            List<long> partIds = new List<long>();
            string bookmark = string.Empty;
            SrchStatus status = null;

            Vault.Results.LogInResult result = VaultLogin();
            v_Conn = result.Connection;

            while (status == null || parts.Count < status.TotalHits)
            {
                //Autodesk.Connectivity.WebServices.File[] results = result.Connection.WebServiceManager.DocumentService.FindFilesBySearchConditions(null, null, null, false, true, ref bookmark, out status);
                Autodesk.Connectivity.WebServices.Item[] results = result.Connection.WebServiceManager.ItemService.FindItemRevisionsBySearchConditions(null, null, false, ref bookmark, out status);

                if (results != null)
                {
                    parts.AddRange(results);
                }
                else
                {
                    break;
                }
                Console.WriteLine("Total Results: " + parts.Count.ToString() + " TotalHits: " + status.TotalHits.ToString());
            }

            ReadProperties(result.Connection);

            return parts;
        }

        public List<ListViewItem> GetAllVersions(List<Item> parts)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            List<ListViewItem> Items = new List<ListViewItem>();
            Dictionary<long, int> versionKey = new Dictionary<long, int>();
            int index = 1;
            long partNumberId = -1;

            try
            {
                foreach (long i in propDefIDs)
                {
                    if (propDefs.ContainsKey((int)i))
                    {
                        //MessageBox.Show(propDefs[(int)i].DisplayName);
                        if (propDefs[(int)i].DisplayName == "Name")
                        {
                            partNumberId = i;
                            versionKey.Add(i, 0);
                            //MessageBox.Show(partNumberId.ToString());
                        }
                        else
                        {
                            versionKey.Add(i, index);
                            index++;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Index Error | " + ex.Message);
            }

            versionCull = new int[propDefs.Count()];
            for (int vci = 0; vci < versionCull.Length; vci++)
            {
                versionCull[vci] = 0;
            }

            Dictionary<long, string[]> versions = new Dictionary<long, string[]>();

            try
            {
                IEnumerable<long> partIds = parts.Select(x => x.Id).Distinct();
                PropInst[] props = v_Conn.WebServiceManager.PropertyService.GetPropertiesByEntityIds("ITEM", partIds.ToArray());
                try
                {
                    foreach (PropInst pd in props)
                    {
                        try
                        {
                            if (!versions.ContainsKey(pd.EntityId))
                            {
                                versions.Add(pd.EntityId, new string[propDefs.Count()]);
                            }

                            var pdVal = "";
                            if (pd.Val != null)
                            {
                                pdVal = pd.Val.ToString();
                            }

                            if (versionKey.ContainsKey(pd.PropDefId) && versions.ContainsKey(pd.EntityId))
                            {
                                long _key = versionKey[pd.PropDefId];
                                versions[pd.EntityId][_key] = pdVal;
                                versionCull[_key] = 1;
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Version Error | " + ex.Message); 
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("END | " + ex.Message);
                }

                foreach (string[] version in versions.Values)
                {

                    bool found = false;
                    try
                    {
                        if (version[0] != "")
                        {
                            List<string> files = Directory.GetFiles(@"C:\Documents\ExtrusionLibrary\Thumbnails").ToList();
                            foreach (string imgfile in files)
                            {
                                if (imgfile.Contains(".jpeg"))
                                {
                                    if (Path.GetFileName(imgfile) == (version[0] + ".jpeg"))
                                    {
                                        imageList.Images.Add(version[0], Image.FromFile(imgfile));
                                        found = true;
                                        break;
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("PRESUBEND | " + ex.Message);
                    }

                    string key = "default";
                    if (found) { key = version[0]; }
                    var item = new ListViewItem(version, key);
                    item.UseItemStyleForSubItems = true;
                    item.ImageKey = key;
                    item.Name = version[0];
                    Items.Add(item);
                }
            }
            catch (Exception ex)
            {
                //MessageBox.Show("END | " + ex.Message);
            }
            stopwatch.Stop();
            Console.WriteLine("GetAllVersion Time: " + stopwatch.Elapsed);
            Console.WriteLine("Items: " + Items.Count.ToString());
            return Items;
        }

        public List<ListViewItem> GetVersions(List<Autodesk.Connectivity.WebServices.Item> parts)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            List<ListViewItem> Items = new List<ListViewItem>();
            Dictionary<long, int> versionKey = new Dictionary<long, int>();
            int index = 1;
            long partNumberId = -1;

            try
            {
                foreach (long i in propDefIDs)
                {
                    if (propDefs.ContainsKey((int)i))
                    {
                        //MessageBox.Show(propDefs[(int)i].DisplayName);
                        if (propDefs[(int)i].DisplayName == "Name")
                        {
                            partNumberId = i;
                            versionKey.Add(i, 0);
                            //MessageBox.Show(partNumberId.ToString());
                        }
                        else
                        {
                            versionKey.Add(i, index);
                            index++;
                        }
                    }
                }
            }
            catch (Exception ex) 
            {
                MessageBox.Show("Index Error | " + ex.Message);
            }

            versionCull = new int[propDefs.Count()];
            for (int vci = 0; vci < versionCull.Length; vci++)
            {
                versionCull[vci] = 0;
            }

            foreach (Autodesk.Connectivity.WebServices.Item f in parts)
            {
                string[] version = new string[propDefs.Count()];

                string idval = "";
                try
                {
                    PropInst[] props = v_Conn.WebServiceManager.PropertyService.GetPropertiesByEntityIds("ITEM", new long[] { f.Id });
                    try
                    {
                        foreach (PropInst pd in props)
                        {
                            try
                            {
                                if (propDefs.ContainsKey(pd.PropDefId))
                                {
                                    var pdVal = "";
                                    if (pd.Val != null)
                                    {
                                        pdVal = pd.Val.ToString();
                                    }
                                    // Insert Property Values into Array for ListView Item
                                    // Insert Part Number at beginning
                                    if (pd.PropDefId == partNumberId)
                                    {
                                        idval = pdVal;
                                        //MessageBox.Show(pdVal);
                                    }
                                    //MessageBox.Show(propDefs[pd.PropDefId].DisplayName + " | " + pdVal);
                                    long _key = versionKey[pd.PropDefId];
                                    version[_key] = pdVal;
                                    versionCull[_key] = 1;
                                }
                            }
                            catch (Exception ex) { MessageBox.Show("Version Error | " + ex.Message); }
                        }

                        bool found = false;
                        try
                        {
                            if (idval != "")
                            {
                                List<string> files = Directory.GetFiles(@"C:\Documents\ExtrusionLibrary\Thumbnails").ToList();
                                foreach (string imgfile in files)
                                {
                                    if (imgfile.Contains(".jpeg"))
                                    {
                                        if (Path.GetFileName(imgfile) == (idval + ".jpeg"))
                                        {
                                            imageList.Images.Add(idval, Image.FromFile(imgfile));
                                            found = true;
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("PRESUBEND | " + ex.Message);
                        }

                        string key = "default";
                        if (found) { key = idval; }
                        var item = new ListViewItem(version, key);
                        item.UseItemStyleForSubItems = true;
                        item.ImageKey = key;
                        item.Name = idval;
                        Items.Add(item);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("END | " + ex.Message);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("END | " + ex.Message);
                }
            }
            stopwatch.Stop();
            Console.WriteLine("GetVersion Time: " + stopwatch.Elapsed);
            return Items;

        }

        // Output information about each file in a folder along with its properties
        //===========================================================================
        static void PrintChildren(Vault.Currency.Connections.Connection connection,
                Vault.Currency.Entities.Folder parentFolder)
        {
            //MessageBox.Show(parentFolder.FullName);

            IEnumerable<Vault.Currency.Entities.IEntity> entities = connection
                .EntityOperations.GetBrowseChildren(parentFolder);
            if (entities != null && entities.Count<Vault.Currency.Entities.IEntity>() > 0)
            {
                foreach (var ent in entities)
                {
                    if (ent is Vault.Currency.Entities.FileIteration)
                    {
                        Vault.Currency.Entities.FileIteration fileIteration
                            = ent as Vault.Currency.Entities.FileIteration;

                        //Now print the properties of the file
                        //PrintProperties(connection, fileIteration);

                        collectionLVI.Add(fileIteration);

                    }
                    else if (ent is Vault.Currency.Entities.Folder)
                    {
                        // Recursively print info about subfolders and files in them
                        //-------------------------------------------------------------------------

                        Vault.Currency.Entities.Folder folder
                            = ent as Vault.Currency.Entities.Folder;
                        PrintChildren(connection, folder);

                    }
                }
            }
        }

        // Read all the Property Definitions for the "FILE" Entity Class
        //===============================================================================
        public void ReadProperties(Vault.Currency.Connections.Connection connection)
        {
            propDefs =
                connection.PropertyManager.GetPropertyDefinitions(
                Vault.Currency.Entities.EntityClassIds.Files,
                null,
                PropertyDefinitionFilter.IncludeAll
                );

            List<long> tempIDs = new List<long>();
            foreach (PropertyDefinition pd in propDefs.Values)
            {
                if (pd.DisplayName == "Thumbnail")
                {
                    thumbID = pd.Id;
                }
                else if (pd.DisplayName == "Name")
                {
                    nameID = pd.Id;
                }
                try
                {
                    tempIDs.Add(pd.Id);
                    propDict.Add(pd.Id, pd);
                } catch (Exception ex) 
                { 
                    //MessageBox.Show("Property Build: " + pd.DisplayName + " | " + ex.Message);
                }
            }
            propDefIDs = tempIDs.ToArray();
        }

        static void PrintProperties(Vault.Currency.Connections.Connection connection,
                        Vault.Currency.Entities.FileIteration fileInteration)
        {
            foreach (var key in propDefs.Keys)
            {
                // Print the Name from the Definition and the Value from the Property
                object propValue = connection.PropertyManager.GetPropertyValue(
                            fileInteration, propDefs[key], null);
                try
                {
                    MessageBox.Show(propDefs[key].DisplayName + ": " + (propValue == null ? "" : propValue.ToString()));
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        public static String GetProperty(Vault.Currency.Connections.Connection connection,
                        Vault.Currency.Entities.FileIteration fileInteration,
                        string key)
        {
            // Print the Name from the Definition and the Value from the Property
            object propValue = connection.PropertyManager.GetPropertyValue(
                        fileInteration, propDefs[key], null);
            try
            {
                return (propValue == null ? " " : propValue.ToString());
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message);
                return null;
            }
        }
    }
}