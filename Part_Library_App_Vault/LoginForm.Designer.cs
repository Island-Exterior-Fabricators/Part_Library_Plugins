namespace Part_Library_App_Vault
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
            this.serverLabel = new MaterialSkin.Controls.MaterialLabel();
            this.vaultLabel = new MaterialSkin.Controls.MaterialLabel();
            this.serverTextBox = new MaterialSkin.Controls.MaterialTextBox();
            this.vaultTextBox = new MaterialSkin.Controls.MaterialTextBox();
            this.loginMaterialButton = new MaterialSkin.Controls.MaterialButton();
            this.passwordTextBox = new MaterialSkin.Controls.MaterialTextBox();
            this.usernameTextBox = new MaterialSkin.Controls.MaterialTextBox();
            this.pwLabel = new MaterialSkin.Controls.MaterialLabel();
            this.usernameLabel = new MaterialSkin.Controls.MaterialLabel();
            this.SuspendLayout();
            // 
            // serverLabel
            // 
            this.serverLabel.AutoSize = true;
            this.serverLabel.Depth = 0;
            this.serverLabel.Font = new System.Drawing.Font("Roboto", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.serverLabel.Location = new System.Drawing.Point(26, 93);
            this.serverLabel.MouseState = MaterialSkin.MouseState.HOVER;
            this.serverLabel.Name = "serverLabel";
            this.serverLabel.Size = new System.Drawing.Size(49, 19);
            this.serverLabel.TabIndex = 2;
            this.serverLabel.Text = "Server:";
            // 
            // vaultLabel
            // 
            this.vaultLabel.AutoSize = true;
            this.vaultLabel.Depth = 0;
            this.vaultLabel.Font = new System.Drawing.Font("Roboto", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.vaultLabel.Location = new System.Drawing.Point(26, 151);
            this.vaultLabel.MouseState = MaterialSkin.MouseState.HOVER;
            this.vaultLabel.Name = "vaultLabel";
            this.vaultLabel.Size = new System.Drawing.Size(42, 19);
            this.vaultLabel.TabIndex = 4;
            this.vaultLabel.Text = "Vault:";
            // 
            // serverTextBox
            // 
            this.serverTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.serverTextBox.Depth = 0;
            this.serverTextBox.Font = new System.Drawing.Font("Roboto", 12F);
            this.serverTextBox.Hint = "Server";
            this.serverTextBox.Location = new System.Drawing.Point(120, 86);
            this.serverTextBox.MaxLength = 50;
            this.serverTextBox.MouseState = MaterialSkin.MouseState.OUT;
            this.serverTextBox.Multiline = false;
            this.serverTextBox.Name = "serverTextBox";
            this.serverTextBox.Size = new System.Drawing.Size(358, 36);
            this.serverTextBox.TabIndex = 5;
            this.serverTextBox.Text = "";
            this.serverTextBox.UseTallSize = false;
            // 
            // vaultTextBox
            // 
            this.vaultTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.vaultTextBox.Depth = 0;
            this.vaultTextBox.Font = new System.Drawing.Font("Roboto", 12F);
            this.vaultTextBox.Hint = "Vault Name";
            this.vaultTextBox.Location = new System.Drawing.Point(120, 143);
            this.vaultTextBox.MaxLength = 50;
            this.vaultTextBox.MouseState = MaterialSkin.MouseState.OUT;
            this.vaultTextBox.Multiline = false;
            this.vaultTextBox.Name = "vaultTextBox";
            this.vaultTextBox.Size = new System.Drawing.Size(358, 36);
            this.vaultTextBox.TabIndex = 6;
            this.vaultTextBox.Text = "";
            this.vaultTextBox.UseTallSize = false;
            // 
            // loginMaterialButton
            // 
            this.loginMaterialButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.loginMaterialButton.Depth = 0;
            this.loginMaterialButton.DrawShadows = true;
            this.loginMaterialButton.HighEmphasis = true;
            this.loginMaterialButton.Icon = null;
            this.loginMaterialButton.Location = new System.Drawing.Point(414, 299);
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
            // passwordTextBox
            // 
            this.passwordTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.passwordTextBox.Depth = 0;
            this.passwordTextBox.Font = new System.Drawing.Font("Roboto", 12F);
            this.passwordTextBox.Hint = "Password";
            this.passwordTextBox.Location = new System.Drawing.Point(120, 254);
            this.passwordTextBox.MaxLength = 50;
            this.passwordTextBox.MouseState = MaterialSkin.MouseState.OUT;
            this.passwordTextBox.Multiline = false;
            this.passwordTextBox.Name = "passwordTextBox";
            this.passwordTextBox.Password = true;
            this.passwordTextBox.Size = new System.Drawing.Size(358, 36);
            this.passwordTextBox.TabIndex = 11;
            this.passwordTextBox.Text = "";
            this.passwordTextBox.UseTallSize = false;
            // 
            // usernameTextBox
            // 
            this.usernameTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.usernameTextBox.Depth = 0;
            this.usernameTextBox.Font = new System.Drawing.Font("Roboto", 12F);
            this.usernameTextBox.Hint = "Username";
            this.usernameTextBox.Location = new System.Drawing.Point(120, 197);
            this.usernameTextBox.MaxLength = 50;
            this.usernameTextBox.MouseState = MaterialSkin.MouseState.OUT;
            this.usernameTextBox.Multiline = false;
            this.usernameTextBox.Name = "usernameTextBox";
            this.usernameTextBox.Size = new System.Drawing.Size(358, 36);
            this.usernameTextBox.TabIndex = 10;
            this.usernameTextBox.Text = "";
            this.usernameTextBox.UseTallSize = false;
            // 
            // pwLabel
            // 
            this.pwLabel.AutoSize = true;
            this.pwLabel.Depth = 0;
            this.pwLabel.Font = new System.Drawing.Font("Roboto", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.pwLabel.Location = new System.Drawing.Point(26, 262);
            this.pwLabel.MouseState = MaterialSkin.MouseState.HOVER;
            this.pwLabel.Name = "pwLabel";
            this.pwLabel.Size = new System.Drawing.Size(75, 19);
            this.pwLabel.TabIndex = 9;
            this.pwLabel.Text = "Password:";
            // 
            // usernameLabel
            // 
            this.usernameLabel.AutoSize = true;
            this.usernameLabel.Depth = 0;
            this.usernameLabel.Font = new System.Drawing.Font("Roboto", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.usernameLabel.Location = new System.Drawing.Point(26, 204);
            this.usernameLabel.MouseState = MaterialSkin.MouseState.HOVER;
            this.usernameLabel.Name = "usernameLabel";
            this.usernameLabel.Size = new System.Drawing.Size(76, 19);
            this.usernameLabel.TabIndex = 8;
            this.usernameLabel.Text = "Username:";
            // 
            // LoginForm
            // 
            this.AcceptButton = this.loginMaterialButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(500, 350);
            this.Controls.Add(this.passwordTextBox);
            this.Controls.Add(this.usernameTextBox);
            this.Controls.Add(this.pwLabel);
            this.Controls.Add(this.usernameLabel);
            this.Controls.Add(this.loginMaterialButton);
            this.Controls.Add(this.vaultTextBox);
            this.Controls.Add(this.serverTextBox);
            this.Controls.Add(this.vaultLabel);
            this.Controls.Add(this.serverLabel);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximumSize = new System.Drawing.Size(500, 350);
            this.MinimumSize = new System.Drawing.Size(500, 350);
            this.Name = "LoginForm";
            this.Text = "Parts Library | Vault Login";
            this.Load += new System.EventHandler(this.LoginForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private MaterialSkin.Controls.MaterialButton loginMaterialButton;
        private MaterialSkin.Controls.MaterialTextBox vaultTextBox;
        private MaterialSkin.Controls.MaterialLabel serverLabel;
        private MaterialSkin.Controls.MaterialLabel vaultLabel;
        private MaterialSkin.Controls.MaterialTextBox serverTextBox;
        private MaterialSkin.Controls.MaterialTextBox passwordTextBox;
        private MaterialSkin.Controls.MaterialTextBox usernameTextBox;
        private MaterialSkin.Controls.MaterialLabel pwLabel;
        private MaterialSkin.Controls.MaterialLabel usernameLabel;
    }
}