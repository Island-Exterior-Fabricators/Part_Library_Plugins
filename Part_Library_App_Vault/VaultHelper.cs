using System;
using System.Collections.Generic;

using ACW = Autodesk.Connectivity.WebServices;
using Framework = Autodesk.DataManagement.Client.Framework;
using Vault = Autodesk.DataManagement.Client.Framework.Vault;
using Forms = Autodesk.DataManagement.Client.Framework.Vault.Forms;
using Autodesk.Connectivity.WebServices;
using Autodesk.DataManagement.Client.Framework.Services;
using Autodesk.Connectivity.WebServicesTools;
using System.Windows.Forms;
using System.Linq;
using System.Configuration;
using System.Reflection;
using Autodesk.DataManagement.Client.Framework.Vault.Currency.Entities;
using Autodesk.DataManagement.Client.Framework.Internal.ExtensionMethods;
using Autodesk.DataManagement.Client.Framework.Currency;
using Autodesk.DataManagement.Client.Framework.Vault.Currency.Properties;
using Autodesk.DataManagement.Client.Framework.Vault.Forms.Controls;
using Autodesk.DataManagement.Client.Framework.Vault.Forms.Models;

namespace Part_Library_App_Vault
{
    public class VaultHelper { 

        private Vault.Currency.Connections.Connection m_conn = null;
        private Vault.Forms.Models.BrowseVaultNavigationModel m_model = null;
        private WebServiceManager m_svcMgr = null;

        public static Vault.Currency.Connections.Connection v_Conn = null;

        // We will collect Property Definitions here
        public static Vault.Currency.Properties.PropertyDefinitionDictionary propDefs;

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

            // For Login Sequence to work AdskLicensingSDK_#.dll must be present and referenced in
            // build events
            // xcopy "$(ProjectDir)..\..\..\..\bin\$(PlatformName)\AdskLicensingSDK_*.dll" "$(TargetDir)" /y
            // m_conn = Vault.Forms.Library.Login(null);
            // m_model = new Forms.Models.BrowseVaultNavigationModel(m_conn, true, true);

            List<File> files = null;
            string bookmark = string.Empty;
            SrchStatus status = null;
            List<File> totalResults = new List<File>();

            Vault.Results.LogInResult result = VaultLogin();
            v_Conn = result.Connection;

            while (status == null || totalResults.Count < status.TotalHits)
            {
                File[] results = result.Connection.WebServiceManager.DocumentService.FindFilesBySearchConditions(null, null, null, false, true, ref bookmark, out status);

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

            foreach (File f in totalResults)
            {
                try
                {
                    FileIteration fi = new Vault.Currency.Entities.FileIteration(result.Connection, f);
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
                return (propValue == null ? "" : propValue.ToString());
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message);
                return null;
            }
        }
    }
}