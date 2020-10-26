//------------------------------------------------------------------------------
// <copyright file="ServiceInstallerDialog.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>                                                                
//------------------------------------------------------------------------------

namespace System.ServiceProcess.Design
{

    using System.Diagnostics;
    using System;
    using System.Drawing;
    using System.Collections;
    using System.ComponentModel;
    using System.Windows.Forms;

    /// <summary>Specifies the return value of a <see cref="T:System.ServiceProcess.Design.ServiceInstallerDialog" /> form.</summary>
    public enum ServiceInstallerDialogResult
    {
        /// <summary>The dialog return value is <see langword="OK" />. This value typically indicates that the user confirmed the account properties and pressed the <see langword="OK" /> button to close the dialog.</summary>
        OK,
        /// <summary>Install the service with a system account rather than a user account. This value typically indicates that the dialog was not displayed to the user. For example, the <see cref="P:System.ServiceProcess.ServiceProcessInstaller.Account" /> property is set to something other than <see langword="User" />.</summary>
        UseSystem,
        /// <summary>The dialog return value is <see langword="Canceled" />. This value typically indicates that the user canceled out of the dialog without setting the account fields.</summary>
        Canceled
    }

    /// <summary>Provides a dialog box, which prompts for account information of a Windows Service application.</summary>
    public class ServiceInstallerDialog : Form
    {

        private System.Windows.Forms.Button okButton;

        private System.Windows.Forms.TextBox passwordEdit;

        private System.Windows.Forms.Button cancelButton;

        private System.Windows.Forms.TextBox confirmPassword;

        private System.Windows.Forms.TextBox usernameEdit;

        private System.Windows.Forms.Label label1;

        private System.Windows.Forms.Label label2;

        private System.Windows.Forms.Label label3;
        private TableLayoutPanel okCancelTableLayoutPanel;
        private TableLayoutPanel overarchingTableLayoutPanel;

        private ServiceInstallerDialogResult result = ServiceInstallerDialogResult.OK;

        /// <summary>Initializes a new instance of the service account form.</summary>
        public ServiceInstallerDialog()
        {
            this.InitializeComponent();
        }

        /// <summary>Gets or sets the password for the service account form.</summary>
		/// <returns>A string representing the password in the service account form. The default is an empty string ("").</returns>
        public string Password
        {
            get
            {
                return passwordEdit.Text;
            }
            set
            {
                passwordEdit.Text = value;
            }
        }

        /// <summary>Gets the dialog result for the service account form.</summary>
        /// <returns>A <see cref="T:System.ServiceProcess.Design.ServiceInstallerDialogResult" /> indicating the user response to the dialog box. The default is <see langword="OK" />.</returns>
        public ServiceInstallerDialogResult Result
        {
            get
            {
                return result;
            }
        }

        /// <summary>Gets or sets the user name for the service account form.</summary>
		/// <returns>A string representing the user name in the service account form. The default is an empty string ("").</returns>
        public string Username
        {
            get
            {
                return usernameEdit.Text;
            }
            set
            {
                usernameEdit.Text = value;
            }
        }

        /// <summary>Begins running a standard application message loop and displays the service account form.</summary>
        public static void Main()
        {
            System.Windows.Forms.Application.Run(new ServiceInstallerDialog());
        }

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ServiceInstallerDialog));
            this.okButton = new System.Windows.Forms.Button();
            this.passwordEdit = new System.Windows.Forms.TextBox();
            this.cancelButton = new System.Windows.Forms.Button();
            this.confirmPassword = new System.Windows.Forms.TextBox();
            this.usernameEdit = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.okCancelTableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.overarchingTableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.okCancelTableLayoutPanel.SuspendLayout();
            this.overarchingTableLayoutPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // okButton
            // 
            resources.ApplyResources(this.okButton, "okButton");
            this.okButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton.Margin = new System.Windows.Forms.Padding(0, 0, 3, 0);
            this.okButton.MinimumSize = new System.Drawing.Size(75, 23);
            this.okButton.Name = "okButton";
            this.okButton.Padding = new System.Windows.Forms.Padding(10, 0, 10, 0);
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // passwordEdit
            // 
            resources.ApplyResources(this.passwordEdit, "passwordEdit");
            this.passwordEdit.Margin = new System.Windows.Forms.Padding(3, 3, 0, 3);
            this.passwordEdit.Name = "passwordEdit";
            // 
            // cancelButton
            // 
            resources.ApplyResources(this.cancelButton, "cancelButton");
            this.cancelButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Margin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.cancelButton.MinimumSize = new System.Drawing.Size(75, 23);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Padding = new System.Windows.Forms.Padding(10, 0, 10, 0);
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // confirmPassword
            // 
            resources.ApplyResources(this.confirmPassword, "confirmPassword");
            this.confirmPassword.Margin = new System.Windows.Forms.Padding(3, 3, 0, 3);
            this.confirmPassword.Name = "confirmPassword";
            // 
            // usernameEdit
            // 
            resources.ApplyResources(this.usernameEdit, "usernameEdit");
            this.usernameEdit.Margin = new System.Windows.Forms.Padding(3, 0, 0, 3);
            this.usernameEdit.Name = "usernameEdit";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Margin = new System.Windows.Forms.Padding(0, 0, 3, 3);
            this.label1.Name = "label1";
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Margin = new System.Windows.Forms.Padding(0, 3, 3, 3);
            this.label2.Name = "label2";
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Margin = new System.Windows.Forms.Padding(0, 3, 3, 3);
            this.label3.Name = "label3";
            // 
            // okCancelTableLayoutPanel
            // 
            resources.ApplyResources(this.okCancelTableLayoutPanel, "okCancelTableLayoutPanel");
            this.okCancelTableLayoutPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.overarchingTableLayoutPanel.SetColumnSpan(this.okCancelTableLayoutPanel, 2);
            this.okCancelTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.okCancelTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.okCancelTableLayoutPanel.Controls.Add(this.okButton, 0, 0);
            this.okCancelTableLayoutPanel.Controls.Add(this.cancelButton, 1, 0);
            this.okCancelTableLayoutPanel.Margin = new System.Windows.Forms.Padding(0, 6, 0, 0);
            this.okCancelTableLayoutPanel.Name = "okCancelTableLayoutPanel";
            this.okCancelTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            // 
            // overarchingTableLayoutPanel
            // 
            resources.ApplyResources(this.overarchingTableLayoutPanel, "overarchingTableLayoutPanel");
            this.overarchingTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.overarchingTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.overarchingTableLayoutPanel.Controls.Add(this.label1, 0, 0);
            this.overarchingTableLayoutPanel.Controls.Add(this.okCancelTableLayoutPanel, 0, 3);
            this.overarchingTableLayoutPanel.Controls.Add(this.label2, 0, 1);
            this.overarchingTableLayoutPanel.Controls.Add(this.confirmPassword, 1, 2);
            this.overarchingTableLayoutPanel.Controls.Add(this.label3, 0, 2);
            this.overarchingTableLayoutPanel.Controls.Add(this.passwordEdit, 1, 1);
            this.overarchingTableLayoutPanel.Controls.Add(this.usernameEdit, 1, 0);
            this.overarchingTableLayoutPanel.Name = "overarchingTableLayoutPanel";
            this.overarchingTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.overarchingTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.overarchingTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.overarchingTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            // 
            // ServiceInstallerDialog
            // 
            this.AcceptButton = this.okButton;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScaleDimensions = new SizeF(6, 13);
            this.CancelButton = this.cancelButton;
            this.Controls.Add(this.overarchingTableLayoutPanel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.HelpButton = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ServiceInstallerDialog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.HelpButtonClicked += new System.ComponentModel.CancelEventHandler(this.ServiceInstallerDialog_HelpButtonClicked);
            this.okCancelTableLayoutPanel.ResumeLayout(false);
            this.okCancelTableLayoutPanel.PerformLayout();
            this.overarchingTableLayoutPanel.ResumeLayout(false);
            this.overarchingTableLayoutPanel.PerformLayout();
            this.ResumeLayout(false);

        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            result = ServiceInstallerDialogResult.Canceled;
            DialogResult = DialogResult.Cancel;
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            result = ServiceInstallerDialogResult.OK;
            if (passwordEdit.Text == confirmPassword.Text)
                DialogResult = DialogResult.OK;
            else
            {
                MessageBoxOptions options = (MessageBoxOptions)0;
                Control current = this;
                while (current.RightToLeft == RightToLeft.Inherit)
                    current = current.Parent;
                if (current.RightToLeft == RightToLeft.Yes)
                    options = MessageBoxOptions.RtlReading | MessageBoxOptions.RightAlign;

                DialogResult = DialogResult.None;
                MessageBox.Show(Res.GetString(Res.Label_MissmatchedPasswords), Res.GetString(Res.Label_SetServiceLogin), MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1, options);
                passwordEdit.Text = string.Empty;
                confirmPassword.Text = string.Empty;
                passwordEdit.Focus();
            }
            // Consider, V2, jruiz: check to make sure the password is correct for the given account.                
        }

        private void ServiceInstallerDialog_HelpButtonClicked(object sender, CancelEventArgs e)
        {
            Debug.Fail("Undone: Needs a help topic. VSWhidbey 326855");
            e.Cancel = true;
        }
    }
}
