namespace Part_Library_App
{
    partial class LibraryForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LibraryForm));
            this.filterTextBox = new MaterialSkin.Controls.MaterialMultiLineTextBox();
            this.filterFieldCbo = new MaterialSkin.Controls.MaterialComboBox();
            this.addFilterBtn = new MaterialSkin.Controls.MaterialFloatingActionButton();
            this.materialContextMenuStrip1 = new MaterialSkin.Controls.MaterialContextMenuStrip();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.clearButton = new MaterialSkin.Controls.MaterialButton();
            this.filtersButton = new MaterialSkin.Controls.MaterialButton();
            this.listView1 = new System.Windows.Forms.ListView();
            this.materialContextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // filterTextBox
            // 
            this.filterTextBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.filterTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.filterTextBox.Depth = 0;
            this.filterTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.filterTextBox.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(222)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.filterTextBox.Hint = "";
            this.filterTextBox.Location = new System.Drawing.Point(139, 78);
            this.filterTextBox.MouseState = MaterialSkin.MouseState.HOVER;
            this.filterTextBox.Multiline = false;
            this.filterTextBox.Name = "filterTextBox";
            this.filterTextBox.Size = new System.Drawing.Size(1000, 20);
            this.filterTextBox.TabIndex = 1;
            this.filterTextBox.Text = "";
            // 
            // filterFieldCbo
            // 
            this.filterFieldCbo.AutoResize = true;
            this.filterFieldCbo.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.filterFieldCbo.Depth = 0;
            this.filterFieldCbo.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
            this.filterFieldCbo.DropDownHeight = 118;
            this.filterFieldCbo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.filterFieldCbo.DropDownWidth = 121;
            this.filterFieldCbo.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
            this.filterFieldCbo.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(222)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.filterFieldCbo.FormattingEnabled = true;
            this.filterFieldCbo.Hint = "Filter";
            this.filterFieldCbo.IntegralHeight = false;
            this.filterFieldCbo.ItemHeight = 29;
            this.filterFieldCbo.Location = new System.Drawing.Point(12, 69);
            this.filterFieldCbo.MaxDropDownItems = 4;
            this.filterFieldCbo.MouseState = MaterialSkin.MouseState.OUT;
            this.filterFieldCbo.Name = "filterFieldCbo";
            this.filterFieldCbo.Size = new System.Drawing.Size(121, 35);
            this.filterFieldCbo.TabIndex = 0;
            this.filterFieldCbo.UseTallSize = false;
            // 
            // addFilterBtn
            // 
            this.addFilterBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.addFilterBtn.AnimateShowHideButton = false;
            this.addFilterBtn.BackColor = System.Drawing.Color.White;
            this.addFilterBtn.Depth = 0;
            this.addFilterBtn.DrawShadows = true;
            this.addFilterBtn.Icon = global::Part_Library_App.Resource.plus;
            this.addFilterBtn.Location = new System.Drawing.Point(1137, 35);
            this.addFilterBtn.Mini = false;
            this.addFilterBtn.MouseState = MaterialSkin.MouseState.HOVER;
            this.addFilterBtn.Name = "addFilterBtn";
            this.addFilterBtn.Size = new System.Drawing.Size(61, 63);
            this.addFilterBtn.TabIndex = 2;
            this.addFilterBtn.Text = "materialFloatingActionButton1";
            this.addFilterBtn.UseVisualStyleBackColor = false;
            this.addFilterBtn.Click += new System.EventHandler(this.materialFloatingActionButton1_Click);
            // 
            // materialContextMenuStrip1
            // 
            this.materialContextMenuStrip1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(242)))), ((int)(((byte)(242)))));
            this.materialContextMenuStrip1.Depth = 0;
            this.materialContextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openToolStripMenuItem,
            this.loadToolStripMenuItem,
            this.viewToolStripMenuItem});
            this.materialContextMenuStrip1.MouseState = MaterialSkin.MouseState.HOVER;
            this.materialContextMenuStrip1.Name = "materialContextMenuStrip1";
            this.materialContextMenuStrip1.Size = new System.Drawing.Size(104, 70);
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.Size = new System.Drawing.Size(103, 22);
            this.openToolStripMenuItem.Text = "Open";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
            // 
            // loadToolStripMenuItem
            // 
            this.loadToolStripMenuItem.Name = "loadToolStripMenuItem";
            this.loadToolStripMenuItem.Size = new System.Drawing.Size(103, 22);
            this.loadToolStripMenuItem.Text = "Load";
            this.loadToolStripMenuItem.Click += new System.EventHandler(this.loadToolStripMenuItem_Click);
            // 
            // viewToolStripMenuItem
            // 
            this.viewToolStripMenuItem.Name = "viewToolStripMenuItem";
            this.viewToolStripMenuItem.Size = new System.Drawing.Size(103, 22);
            this.viewToolStripMenuItem.Text = "View";
            this.viewToolStripMenuItem.Click += new System.EventHandler(this.viewToolStripMenuItem_Click);
            // 
            // clearButton
            // 
            this.clearButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.clearButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.clearButton.Depth = 0;
            this.clearButton.DrawShadows = true;
            this.clearButton.HighEmphasis = true;
            this.clearButton.Icon = null;
            this.clearButton.Location = new System.Drawing.Point(1122, 119);
            this.clearButton.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.clearButton.MouseState = MaterialSkin.MouseState.HOVER;
            this.clearButton.Name = "clearButton";
            this.clearButton.Size = new System.Drawing.Size(66, 36);
            this.clearButton.TabIndex = 3;
            this.clearButton.Text = "Clear";
            this.clearButton.Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Contained;
            this.clearButton.UseAccentColor = false;
            this.clearButton.UseVisualStyleBackColor = true;
            this.clearButton.Click += new System.EventHandler(this.clearButton_Click);
            // 
            // filtersButton
            // 
            this.filtersButton.AutoSize = false;
            this.filtersButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.filtersButton.Depth = 0;
            this.filtersButton.DrawShadows = true;
            this.filtersButton.HighEmphasis = true;
            this.filtersButton.Icon = null;
            this.filtersButton.Location = new System.Drawing.Point(13, 119);
            this.filtersButton.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.filtersButton.MouseState = MaterialSkin.MouseState.HOVER;
            this.filtersButton.Name = "filtersButton";
            this.filtersButton.Size = new System.Drawing.Size(1101, 36);
            this.filtersButton.TabIndex = 7;
            this.filtersButton.TabStop = false;
            this.filtersButton.Text = "-- Filters --";
            this.filtersButton.Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Contained;
            this.filtersButton.UseAccentColor = true;
            this.filtersButton.UseVisualStyleBackColor = true;
            // 
            // listView1
            // 
            this.listView1.AllowColumnReorder = true;
            this.listView1.BackColor = System.Drawing.Color.White;
            this.listView1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.listView1.ContextMenuStrip = this.materialContextMenuStrip1;
            this.listView1.FullRowSelect = true;
            this.listView1.HideSelection = false;
            this.listView1.Location = new System.Drawing.Point(13, 166);
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(1175, 497);
            this.listView1.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.listView1.TabIndex = 8;
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.View = System.Windows.Forms.View.Details;
            this.listView1.ItemMouseHover += new System.Windows.Forms.ListViewItemMouseHoverEventHandler(this.listView1_ItemMouseHover);
            this.listView1.MouseClick += new System.Windows.Forms.MouseEventHandler(this.listView1_MouseClick);
            this.listView1.MouseEnter += new System.EventHandler(this.listView1_MouseEnter);
            this.listView1.MouseLeave += new System.EventHandler(this.listView1_MouseLeave);
            // 
            // LibraryForm
            // 
            this.AcceptButton = this.addFilterBtn;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1200, 675);
            this.Controls.Add(this.listView1);
            this.Controls.Add(this.filtersButton);
            this.Controls.Add(this.clearButton);
            this.Controls.Add(this.filterFieldCbo);
            this.Controls.Add(this.addFilterBtn);
            this.Controls.Add(this.filterTextBox);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(1000, 600);
            this.Name = "LibraryForm";
            this.Text = "Parts Library";
            this.Resize += new System.EventHandler(this.resize_form);
            this.materialContextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private MaterialSkin.Controls.MaterialMultiLineTextBox filterTextBox;
        private MaterialSkin.Controls.MaterialFloatingActionButton addFilterBtn;
        private MaterialSkin.Controls.MaterialComboBox filterFieldCbo;
        private MaterialSkin.Controls.MaterialContextMenuStrip materialContextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loadToolStripMenuItem;
        private MaterialSkin.Controls.MaterialButton clearButton;
        private MaterialSkin.Controls.MaterialButton filtersButton;
        private System.Windows.Forms.ToolStripMenuItem viewToolStripMenuItem;
        private System.Windows.Forms.ListView listView1;
    }
}