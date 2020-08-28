using System.Drawing;

namespace Part_Library_App_Vault
{
    public class PartTypes
    {
        public class Part
        {
            public Bitmap thumb { get; set; }
            public Bitmap thumbS { get; set; }
            public Bitmap thumbL { get; set; }
            public string FolderLink { get; set; }
            public string PartDescription { get; set; }
            public string ProjectFirstUsedOn { get; set; }
            public string Function { get; set; }
            public string Type { get; set; }
            public string Keymark { get; set; }
            public string Sapa { get; set; }
            public string InternationalExtrusions { get; set; }
            public string Alloy { get; set; }
            public string Temper { get; set; }
            public string Finish { get; set; }
            public string ID { get; set; }
            public string Hyperlink { get; set; }

            public string ExteriorProfile { get; set; }
            public string ThermalBreakSize { get; set; }
            public string InteriorProfile { get; set; }
            public string Extruder { get; set; }
            public string ExtruderNumber { get; set; }

            public string Material { get; set; }
            public string Durometer { get; set; }
            public string Color { get; set; }
            public string Trelleborg { get; set; }
            public string Tremco { get; set; }
            public string McThermo { get; set; }

            public string Ensinger { get; set; }
            public string Quicksilver { get; set; }
            public string JiffRam { get; set; }

            public string ExtruderDieNumber { get; set; }

            public string ThermalBreakHeight { get; set; }
            public string CornerKey { get; set; }

            public string DXF { get; set; }
            public string DWG { get; set; }
            public string PDF { get; set; }
            public string RVT { get; set; }
            public string RDM { get; set; }
            public string IAM { get; set; }
            public string _DXF { get; set; }
            public string _DWG { get; set; }
            public string _PDF { get; set; }
            public string _RVT { get; set; }
            public string _RDM { get; set; }
            public string _IAM { get; set; }

            // Remaining implementation of Part class.
        }

        public class Project
        {
            public string name;

            public Project(string _name = "")
            {
                name = _name;
            }

            // Remaining implementation of Project class.
        }
    }
}
