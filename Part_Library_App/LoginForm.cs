using MaterialSkin;
using MaterialSkin.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Configuration;
using System.Reflection;
using Microsoft.SharePoint.Client;
using System.Security;

namespace Part_Library_App
{
    public partial class LoginForm : MaterialForm
    {
        public LoginForm()
        {
            InitializeComponent();

            var materialSkinManager = MaterialSkinManager.Instance;
            materialSkinManager.AddFormToManage(this);
            materialSkinManager.Theme = MaterialSkinManager.Themes.LIGHT;
            materialSkinManager.ColorScheme = new ColorScheme(Primary.BlueGrey800, Primary.BlueGrey900, Primary.BlueGrey500, Accent.LightBlue200, TextShade.WHITE);
        }

        static bool CheckExistingCredentials()
        {
            Configuration configuration = ConfigurationManager.OpenExeConfiguration(Assembly.GetExecutingAssembly().Location);

            // Update configuration file, store some arbitrary values, "new username" & "new password"
            if (configuration.AppSettings.Settings["username"] == null)
            {
                configuration.AppSettings.Settings.Add("username", "");
                configuration.AppSettings.Settings.Add("password", "");

                configuration.Save();
                // Reload app config file
                ConfigurationManager.RefreshSection("appSettings");
                return false;
            }
            else
            {
                try
                {
                    ClientContext ctx = new ClientContext("https://islandcompanies.sharepoint.com/sites/PartLibrary/");
                    SecureString pw = new SecureString();

                    foreach (char a in crypt.DecryptString(configuration.AppSettings.Settings["password"].Value)) pw.AppendChar(a);
                    ctx.Credentials = new SharePointOnlineCredentials(crypt.DecryptString(configuration.AppSettings.Settings["username"].Value), pw);

                    Web web = ctx.Web;

                    try
                    {
                        string xlFileLink = "https://islandcompanies.sharepoint.com/:x:/r/sites/PartLibrary/Shared%20Documents/181128%20-%20Extrusion%20Library%20(Working%20File).xlsm?d=wd6dbf2af9eac4627863d096af93f1a13&csf=1&web=1&e=GWCPKi";

                        getSharepointTree(xlFileLink, out List<String> sharepointTree);

                        Folder sourceFolder = web.GetFolderByServerRelativeUrl(sharepointTree.Aggregate((i, j) => i + "/" + j));
                        ctx.Load(ctx.Web);
                        ctx.Load(sourceFolder);
                        ctx.ExecuteQuery();
                        return true;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    return false;
                }
            }
        }

        private bool UpdateCredentials()
        {
            try
            {
                Configuration configuration = ConfigurationManager.OpenExeConfiguration(Assembly.GetExecutingAssembly().Location);
                configuration.AppSettings.Settings["username"].Value = crypt.EncryptString(usernameTextBox.Text);
                configuration.AppSettings.Settings["password"].Value = crypt.EncryptString(passwordTextBox.Text);

                configuration.Save();
                // Reload app config file
                ConfigurationManager.RefreshSection("appSettings");

                return CheckExistingCredentials();
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public static List<string> getSharepointTree(string xlFileLink, out List<string> sharepointTree)
        {
            sharepointTree = new List<string>();
            string[] splitSharepointLink = xlFileLink.Split('/');

            bool start = false;
            foreach (string s in splitSharepointLink)
            {
                if (s.Contains("Documents"))
                {
                    start = true;
                }
                if (start)
                {
                    sharepointTree.Add(s);
                }
            }

            sharepointTree.RemoveAt(sharepointTree.Count - 1);

            return sharepointTree;
        }

        private void loginMaterialButton_Click(object sender, EventArgs e)
        {
            if (usernameTextBox.Text == "")
            {
                MessageBox.Show("You must enter a valid sharepoint user email for authentification!");
            }
            else if (passwordTextBox.Text == "")
            {
                MessageBox.Show("You must enter a valid sharepoint password for authentification!");
            }
            else
            {
                if (UpdateCredentials())
                {
                    try
                    {
                        CloseOut();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
            }
        }

        private void LoginForm_Load(object sender, EventArgs e)
        {
            if (CheckExistingCredentials())
            {
                CloseOut();
            }
        }

        private void CloseOut()
        {
            passwordTextBox.Text = string.Empty;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}