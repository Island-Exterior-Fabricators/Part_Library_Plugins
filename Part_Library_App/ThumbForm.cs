using MaterialSkin;
using MaterialSkin.Controls;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace Part_Library_App
{
    public partial class ThumbForm : MaterialForm
    {
        public ThumbForm(Bitmap displayImage)
        {
            InitializeComponent();

            var materialSkinManager = MaterialSkinManager.Instance;
            materialSkinManager.AddFormToManage(this);
            materialSkinManager.Theme = MaterialSkinManager.Themes.LIGHT;
            materialSkinManager.ColorScheme = new ColorScheme(Primary.BlueGrey800, Primary.BlueGrey900, Primary.BlueGrey500, Accent.LightBlue200, TextShade.WHITE);

            this.pictureBox1.Image = displayImage;
        }

        private int TextWidth(Control c, string s)
        {
            double factor = 1.15;
            Graphics g = c.CreateGraphics();
            Font font = c.Font;

            int newWidth;
            newWidth = (int)Math.Round(g.MeasureString(s.ToUpper(), font).Width * factor) + 20;
            return newWidth;
        }
    }
}