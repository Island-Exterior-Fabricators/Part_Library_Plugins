using Autodesk.DataManagement.Client.Framework.Vault.Currency.Entities;
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


namespace Part_Library_App_Vault
{
    public partial class LibraryForm : MaterialForm
    {
        public static Queue<string> LibraryToRevitActions = new Queue<string>();
        public static Queue<string> RevitToLibraryActions = new Queue<string>();

        System.Windows.Forms.Timer action_timer = new System.Windows.Forms.Timer() { Interval = 300 };

        List<PartTypes.Part> PARTS = new List<PartTypes.Part>();
        Bitmap defaultDWG = Resource.Autodesk_DWG_icon;
        ImageList imageList = new ImageList();

        List<ListViewItem> Items = new List<ListViewItem>();
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

            List < FileIteration > parts = null;

            InitVault(out parts);

            //BuildPARTS(parts);
            SeedListViewW(parts);

            //CycleBackground();


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

        private void BuildPARTS(Dictionary<string, List<PartTypes.Part>> parts)
        {
            foreach (List<PartTypes.Part> pa in parts.Values)
            {
                PARTS.AddRange(pa.ToArray());
            }
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;

            foreach (PartTypes.Part p in PARTS)
            {
                try
                {
                    List<string> files = Directory.GetFiles(p.Hyperlink).ToList();
                    foreach (string imgfile in files)
                    {
                        String extension = "";
                        if (imgfile.Contains(".jpg"))
                        {
                            extension = ".jpg";
                        }
                        else if (imgfile.Contains(".png"))
                        {
                            extension = ".png";
                        }
                        if (extension != "")
                        {
                            //p.thumb = (Bitmap)Bitmap.FromFile(imgfile);
                            ThumbSet ts = new ThumbSet();
                            string newPathAndName = @"C:\Documents\ExtrusionLibrary\Thumbnails\" + p.ID + extension;

                            if (!System.IO.Directory.Exists(newPathAndName))
                            {
                                System.IO.File.Copy(imgfile, newPathAndName);
                                ts.IMG = Image.FromFile(newPathAndName);
                                ts.ID = p.ID;
                                worker.ReportProgress(100, ts);
                            }
                            else
                            {
                                if (System.IO.File.GetLastWriteTime(imgfile) != System.IO.File.GetLastWriteTime(newPathAndName))
                                {
                                    System.IO.File.Copy(imgfile, newPathAndName);
                                    ts.IMG = Image.FromFile(newPathAndName);
                                    ts.ID = p.ID;
                                    worker.ReportProgress(100, ts);
                                }
                            }
                            break;
                        }
                    }
                }
                catch
                {
                }
                finally
                {
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

        private void SeedListViewW(List<FileIteration> parts)
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
                foreach (FileIteration f in parts)
                {
                    string[] version;
                    string idval = null;
                    bool partFound = false;
                    try
                    {
                        version = new String[] { };
                        try
                        {
                            idval = GetProperty(v_Conn, f, pnDef.SystemName);
                            version.Append(idval);
                            if (idval != "")
                            {
                                partFound = true;
                                foreach (PropertyDefinition s in propDefs.Values)
                                {
                                    if (s.DisplayName != "Part Number")
                                    {
                                        string val = GetProperty(v_Conn, f, s.SystemName);
                                        //MessageBox.Show(val);
                                        version.Append(val);
                                    }
                                }
                            }
                            else
                            {
                                idval = "default";
                                version.Append("");
                            }
                            //MessageBox.Show(version.Length.ToString());

                        } catch (Exception ex) { MessageBox.Show(ex.Message); }

                        if (partFound)
                        {
                            found = false;
                            try
                            {
                                List<string> files = Directory.GetFiles(@"C:\Documents\ExtrusionLibrary\Thumbnails").ToList();
                                foreach (string imgfile in files)
                                {
                                    if (imgfile.Contains(".jpg"))
                                    {
                                        if (Path.GetFileName(imgfile) == (idval + ".jpg"))
                                        {
                                            imageList.Images.Add(idval, Image.FromFile(imgfile));
                                            found = true;
                                            break;
                                        }
                                    }
                                }
                            }
                            catch
                            {
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        //MessageBox.Show(ex.Message);
                        version = new[] { idval, "" };
                    }
                    if (partFound)
                    {
                        string key = "default";
                        if (found) { key = idval; }
                        var item = new ListViewItem(version, key);
                        item.UseItemStyleForSubItems = true;
                        item.ImageKey = key;
                        item.Name = idval;
                        Items.Add(item);
                        index++;
                    }
                }
            }
            listView1.Items.AddRange(Items.ToArray());
            FilteredItems.AddRange(Items);

            this.listView1.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
            foreach (ColumnHeader ch in cc)
            {
                ch.Width += 30;
            }

            listView1.EndUpdate();
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

        private Vault.Currency.Connections.Connection m_conn = null;
        private Vault.Forms.Models.BrowseVaultNavigationModel m_model = null;
        private WebServiceManager m_svcMgr = null;

        public static Vault.Currency.Connections.Connection v_Conn = null;

        // We will collect Property Definitions here
        public static Vault.Currency.Properties.PropertyDefinitionDictionary propDefs;
        public static long[] propDefIDs = new long[] { };
        public static Dictionary<long, PropertyDefinition> propDict = new Dictionary<long, PropertyDefinition>();

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

        public static List<FileIteration> InitVault(out List<FileIteration> parts)
        {
            parts = new List<FileIteration>();
            List<Autodesk.Connectivity.WebServices.File> files = null;
            string bookmark = string.Empty;
            SrchStatus status = null;
            List<Autodesk.Connectivity.WebServices.File> totalResults = new List<Autodesk.Connectivity.WebServices.File>();

            Vault.Results.LogInResult result = VaultLogin();
            v_Conn = result.Connection;

            while (status == null || totalResults.Count < status.TotalHits)
            {
                Autodesk.Connectivity.WebServices.File[] results = result.Connection.WebServiceManager.DocumentService.FindFilesBySearchConditions(null, null, null, false, true, ref bookmark, out status);

                if (results != null)
                {
                    totalResults.AddRange(results);
                }
                else
                {
                    break;
                }
            }

            ReadProperties(result.Connection);

            foreach (Autodesk.Connectivity.WebServices.File f in totalResults)
            {
                try
                {
                    
                    FileIteration fi = new Vault.Currency.Entities.FileIteration(result.Connection, f);
                    PropInst[] props = v_Conn.WebServiceManager.PropertyService.GetPropertiesByEntityIds("FILE", new long[] { fi.EntityIterationId });
                    //PropInst[] props = v_Conn.WebServiceManager.PropertyService.GetProperties("FILE", new long[] { f.Id }, propDefIDs);
                    try
                    {
                        foreach (PropInst pd in props)
                        {
                            MessageBox.Show(pd + " | " + pd.Val.ToString());
                        }
                    }
                    catch { }
                    //PrintProperties(result.Connection, fi);
                    parts.Add(fi);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }

            return parts;
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
        static void ReadProperties(Vault.Currency.Connections.Connection connection)
        {
            propDefs =
                connection.PropertyManager.GetPropertyDefinitions(
                Vault.Currency.Entities.EntityClassIds.Files,
                null,
                Vault.Currency.Properties.PropertyDefinitionFilter.IncludeUserDefined
                );

            foreach (PropertyDefinition pd in propDefs.Values)
            {
                propDefIDs.Append(pd.Id);
                propDict[pd.Id] = pd;
            }
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