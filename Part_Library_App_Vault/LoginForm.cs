using MaterialSkin;
using MaterialSkin.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Configuration;
using System.Reflection;
using System.Security;

namespace Part_Library_App_Vault
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
                configuration.AppSettings.Settings.Add("server", "");
                configuration.AppSettings.Settings.Add("vault", "");

                configuration.Save();
                // Reload app config file
                ConfigurationManager.RefreshSection("appSettings");
                return false;
            }
            else
            {
                try
                {
                    bool check = VaultHelper.LoginCheck();
                    return check;
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
                configuration.AppSettings.Settings["server"].Value = crypt.EncryptString(serverTextBox.Text);
                configuration.AppSettings.Settings["vault"].Value = crypt.EncryptString(vaultTextBox.Text);

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


        private void loginMaterialButton_Click(object sender, EventArgs e)
        {
            if (serverTextBox.Text == "")
            {
                MessageBox.Show("You must enter a valid sharepoint user email for authentification!");
            }
            else if (vaultTextBox.Text == "")
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
            vaultTextBox.Text = string.Empty;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}