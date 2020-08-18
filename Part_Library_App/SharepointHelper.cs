using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Security;

namespace Part_Library_App
{
    public class SharepointHelper
    {
        public static Dictionary<string, List<PartTypes.Part>> SharepointPull(string xlFileLink, out Dictionary<string, List<PartTypes.Part>> parts)
        {
            parts = new Dictionary<string, List<PartTypes.Part>>();

            ClientContext ctx = new ClientContext("https://islandcompanies.sharepoint.com/sites/PartLibrary/");
            SecureString pw = new SecureString();

            Configuration configuration = ConfigurationManager.OpenExeConfiguration(Assembly.GetExecutingAssembly().Location);
            foreach (char a in crypt.DecryptString(configuration.AppSettings.Settings["password"].Value)) pw.AppendChar(a);
            ctx.Credentials = new SharePointOnlineCredentials(crypt.DecryptString(configuration.AppSettings.Settings["username"].Value), pw);

            Web web = ctx.Web;
            ctx.Load(web, w => w.Title);

            ExcelHelper.getXLFileName(xlFileLink, out string xlFileName);
            getSharepointTree(xlFileLink, out List<String> sharepointTree);

            try
            {
                Folder sourceFolder = web.GetFolderByServerRelativeUrl(sharepointTree.Aggregate((i, j) => i + "/" + j));
                ctx.Load(ctx.Web);
                ctx.Load(sourceFolder);
                ctx.ExecuteQuery();

                CamlQuery camlQuery = new CamlQuery();
                camlQuery.ViewXml = @"<View Scope='Recursive'>
                                    <Query>
                                        <Where>
                                            <Eq>
                                                <FieldRef Name='FileLeafRef'></FieldRef>
                                                <Value Type='Text'>" + xlFileName + @"</Value>
                                            </Eq>
                                        </Where>
                                    </Query>
                                </View>";
                //string srvRel = "sites/PartLibrary/Shared%20Documents/";
                ListItemCollection listItems = ctx.Web.Lists.GetByTitle("Documents").GetItems(camlQuery);
                ctx.Load(listItems);
                ctx.ExecuteQuery();

                Microsoft.SharePoint.Client.File file = listItems[0].File;
                ctx.Load(file);
                ctx.ExecuteQuery();

                FileInformation fileInfo = Microsoft.SharePoint.Client.File.OpenBinaryDirect(ctx, file.ServerRelativeUrl);
                ctx.ExecuteQuery();

                System.IO.Directory.CreateDirectory(@"C:\Documents\Part Library\");
                string filePath = @"C:\Documents\Part Library\" + file.Name;
                using (var fileStream = new System.IO.FileStream(filePath, System.IO.FileMode.Create))
                {
                    fileInfo.Stream.CopyTo(fileStream);
                }

                using (SpreadsheetDocument spreadsheetDocument = SpreadsheetDocument.Open(filePath, false))
                {
                    WorkbookPart workbookPart = spreadsheetDocument.WorkbookPart;
                    WorksheetPart worksheetPart = workbookPart.WorksheetParts.First();

                    SharedStringTablePart sharedStringTablePart = spreadsheetDocument.WorkbookPart.SharedStringTablePart;

                    int sheetIndex = 0;
                    Workbook workbook = spreadsheetDocument.WorkbookPart.Workbook;

                    Dictionary<int, string> dict = ExcelHelper.LoadDictionary(workbookPart.SharedStringTablePart.SharedStringTable);

                    foreach (WorksheetPart worksheetpart in workbook.WorkbookPart.WorksheetParts)
                    {
                        Worksheet worksheet = worksheetpart.Worksheet;

                        // Grab the sheet name each time through your loop
                        string sheetName = workbookPart.Workbook.Descendants<Sheet>().ElementAt(sheetIndex).Name;
                        List<PartTypes.Part> partList = new List<PartTypes.Part>();
                        foreach (SheetData sheetData in worksheet.Elements<SheetData>())
                        {
                            bool init = false;
                            bool run = false;
                            Dictionary<int, string> properties = new Dictionary<int, string>();
                            foreach (Row r in sheetData.Elements<Row>())
                            {
                                PartTypes.Part part = new PartTypes.Part();
                                foreach (Cell cell in r.Elements<Cell>())
                                {
                                    string val = "";
                                    try
                                    {
                                        if (cell.DataType != null && cell.DataType.Value == CellValues.SharedString)
                                        {
                                            val = dict[int.Parse(cell.CellValue.Text)];
                                        }
                                        else if (cell.CellValue != null)
                                        {
                                            val = ExcelHelper.ReadExcelCell(cell, workbookPart).Value;
                                        }
                                        else
                                            val = null;

                                        if (val == "Folder Link" && run == false)
                                        {
                                            init = true;
                                        }
                                    }
                                    catch (Exception ex)
                                    {

                                    }

                                    if (init && run == false)
                                    {

                                        try
                                        {
                                            int i = ExcelHelper.ColumnIndex(cell.CellReference);
                                            properties.Add(i, val.Replace(" ", String.Empty));
                                        }
                                        catch (Exception ex)
                                        {
                                            //MessageBox.Show(ex.Message);
                                        }
                                    }
                                    else if (run)
                                    {
                                        try
                                        {
                                            int i = ExcelHelper.ColumnIndex(cell.CellReference);
                                            string value;

                                            if (properties.TryGetValue(i, out value))
                                            {
                                                typeof(PartTypes.Part).GetProperty(value).SetValue(part, val);
                                                // IF VALUES MISSING IN RESULT CHECK TO MAKE SURE SPELLING OF COLUMN MATCHES PROPERTY
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            //MessageBox.Show(ex.Message);
                                        }
                                    }
                                }
                                if (run != true)
                                {
                                    if (properties.Count() > 0)
                                    {
                                        run = true;
                                        init = false;
                                    }
                                }
                                else
                                {
                                    try
                                    {
                                        if (part.ID != null)
                                        {
                                            partList.Add(part);
                                        }
                                    }
                                    catch
                                    {

                                    }
                                }
                            }
                        }
                        parts.Add(sheetName, partList);
                        sheetIndex++;
                    }
                }
            }
            catch
            {

            }
            return parts;
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
    }
}
