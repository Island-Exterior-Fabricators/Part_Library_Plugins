namespace Part_Library_App
{
    partial class LoginForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LoginForm));
            this.usernameLabel = new MaterialSkin.Controls.MaterialLabel();
            this.pwLabel = new MaterialSkin.Controls.MaterialLabel();
            this.usernameTextBox = new MaterialSkin.Controls.MaterialTextBox();
            this.passwordTextBox = new MaterialSkin.Controls.MaterialTextBox();
            this.loginMaterialButton = new MaterialSkin.Controls.MaterialButton();
            this.SuspendLayout();
            // 
            // usernameLabel
            // 
            this.usernameLabel.AutoSize = true;
            this.usernameLabel.Depth = 0;
            this.usernameLabel.Font = new System.Drawing.Font("Roboto", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.usernameLabel.Location = new System.Drawing.Point(26, 93);
            this.usernameLabel.MouseState = MaterialSkin.MouseState.HOVER;
            this.usernameLabel.Name = "usernameLabel";
            this.usernameLabel.Size = new System.Drawing.Size(76, 19);
            this.usernameLabel.TabIndex = 2;
            this.usernameLabel.Text = "Username:";
            // 
            // pwLabel
            // 
            this.pwLabel.AutoSize = true;
            this.pwLabel.Depth = 0;
            this.pwLabel.Font = new System.Drawing.Font("Roboto", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.pwLabel.Location = new System.Drawing.Point(26, 153);
            this.pwLabel.MouseState = MaterialSkin.MouseState.HOVER;
            this.pwLabel.Name = "pwLabel";
            this.pwLabel.Size = new System.Drawing.Size(75, 19);
            this.pwLabel.TabIndex = 4;
            this.pwLabel.Text = "Password:";
            // 
            // usernameTextBox
            // 
            this.usernameTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.usernameTextBox.Depth = 0;
            this.usernameTextBox.Font = new System.Drawing.Font("Roboto", 12F);
            this.usernameTextBox.Hint = "Username";
            this.usernameTextBox.Location = new System.Drawing.Point(120, 86);
            this.usernameTextBox.MaxLength = 50;
            this.usernameTextBox.MouseState = MaterialSkin.MouseState.OUT;
            this.usernameTextBox.Multiline = false;
            this.usernameTextBox.Name = "usernameTextBox";
            this.usernameTextBox.Size = new System.Drawing.Size(358, 36);
            this.usernameTextBox.TabIndex = 5;
            this.usernameTextBox.Text = "";
            this.usernameTextBox.UseTallSize = false;
            // 
            // passwordTextBox
            // 
            this.passwordTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.passwordTextBox.Depth = 0;
            this.passwordTextBox.Font = new System.Drawing.Font("Roboto", 12F);
            this.passwordTextBox.Hint = "Password";
            this.passwordTextBox.Location = new System.Drawing.Point(120, 145);
            this.passwordTextBox.MaxLength = 50;
            this.passwordTextBox.MouseState = MaterialSkin.MouseState.OUT;
            this.passwordTextBox.Multiline = false;
            this.passwordTextBox.Name = "passwordTextBox";
            this.passwordTextBox.Password = true;
            this.passwordTextBox.Size = new System.Drawing.Size(358, 36);
            this.passwordTextBox.TabIndex = 6;
            this.passwordTextBox.Text = "";
            this.passwordTextBox.UseTallSize = false;
            // 
            // loginMaterialButton
            // 
            this.loginMaterialButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.loginMaterialButton.Depth = 0;
            this.loginMaterialButton.DrawShadows = true;
            this.loginMaterialButton.HighEmphasis = true;
            this.loginMaterialButton.Icon = null;
            this.loginMaterialButton.Location = new System.Drawing.Point(414, 199);
            this.loginMaterialButton.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.loginMaterialButton.MouseState = MaterialSkin.MouseState.HOVER;
            this.loginMaterialButton.Name = "loginMaterialButton";
            this.loginMaterialButton.Size = new System.Drawing.Size(64, 36);
            this.loginMaterialButton.TabIndex = 7;
            this.loginMaterialButton.Text = "Login";
            this.loginMaterialButton.Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Contained;
            this.loginMaterialButton.UseAccentColor = false;
            this.loginMaterialButton.UseVisualStyleBackColor = true;
            this.loginMaterialButton.Click += new System.EventHandler(this.loginMaterialButton_Click);
            // 
            // LoginForm
            // 
            this.AcceptButton = this.loginMaterialButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(500, 250);
            this.Controls.Add(this.loginMaterialButton);
            this.Controls.Add(this.passwordTextBox);
            this.Controls.Add(this.usernameTextBox);
            this.Controls.Add(this.pwLabel);
            this.Controls.Add(this.usernameLabel);
            //this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximumSize = new System.Drawing.Size(500, 250);
            this.MinimumSize = new System.Drawing.Size(500, 250);
            this.Name = "LoginForm";
            this.Text = "Parts Library | Login";
            this.Load += new System.EventHandler(this.LoginForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private MaterialSkin.Controls.MaterialButton loginMaterialButton;
        private MaterialSkin.Controls.MaterialTextBox passwordTextBox;
        private MaterialSkin.Controls.MaterialLabel usernameLabel;
        private MaterialSkin.Controls.MaterialLabel pwLabel;
        private MaterialSkin.Controls.MaterialTextBox usernameTextBox;
    }
}