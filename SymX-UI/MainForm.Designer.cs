namespace SymX_UI
{
    partial class MainForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.MainMenu = new System.Windows.Forms.MenuStrip();
            this.MainMenuFile = new System.Windows.Forms.ToolStripMenuItem();
            this.MainMenuFileGenerateCSV = new System.Windows.Forms.ToolStripMenuItem();
            this.MainMenuFileLoadCSV = new System.Windows.Forms.ToolStripMenuItem();
            this.MainMenuFileExit = new System.Windows.Forms.ToolStripMenuItem();
            this.MainMenuSettings = new System.Windows.Forms.ToolStripMenuItem();
            this.MainMenuSettingsSearchRecursively = new System.Windows.Forms.ToolStripMenuItem();
            this.MainMenuSettingsHexTime = new System.Windows.Forms.ToolStripMenuItem();
            this.MainMenuSettingsPreferences = new System.Windows.Forms.ToolStripMenuItem();
            this.MainMenuHelp = new System.Windows.Forms.ToolStripMenuItem();
            this.MainMenuHelpReleaseNotes = new System.Windows.Forms.ToolStripMenuItem();
            this.MainMenuHelpAbout = new System.Windows.Forms.ToolStripMenuItem();
            this.FileList = new System.Windows.Forms.ListView();
            this.FileListFileName = new System.Windows.Forms.ColumnHeader();
            this.FileListImageSizeMin = new System.Windows.Forms.ColumnHeader();
            this.TimeDateStamp = new System.Windows.Forms.ColumnHeader();
            this.FileListHeader = new System.Windows.Forms.Label();
            this.DownloadButton = new System.Windows.Forms.Button();
            this.AddFileButton = new System.Windows.Forms.Button();
            this.RemoveFileButton = new System.Windows.Forms.Button();
            this.FileListImageSizeMax = new System.Windows.Forms.ColumnHeader();
            this.MainMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // MainMenu
            // 
            this.MainMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.MainMenuFile,
            this.MainMenuSettings,
            this.MainMenuHelp});
            this.MainMenu.Location = new System.Drawing.Point(0, 0);
            this.MainMenu.Name = "MainMenu";
            this.MainMenu.Size = new System.Drawing.Size(800, 24);
            this.MainMenu.TabIndex = 0;
            this.MainMenu.Text = "menuStrip1";
            // 
            // MainMenuFile
            // 
            this.MainMenuFile.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.MainMenuFileGenerateCSV,
            this.MainMenuFileLoadCSV,
            this.MainMenuFileExit});
            this.MainMenuFile.Name = "MainMenuFile";
            this.MainMenuFile.Size = new System.Drawing.Size(37, 20);
            this.MainMenuFile.Text = "File";
            // 
            // MainMenuFileGenerateCSV
            // 
            this.MainMenuFileGenerateCSV.Name = "MainMenuFileGenerateCSV";
            this.MainMenuFileGenerateCSV.Size = new System.Drawing.Size(180, 22);
            this.MainMenuFileGenerateCSV.Text = "Generate CSV";
            // 
            // MainMenuFileLoadCSV
            // 
            this.MainMenuFileLoadCSV.Name = "MainMenuFileLoadCSV";
            this.MainMenuFileLoadCSV.Size = new System.Drawing.Size(180, 22);
            this.MainMenuFileLoadCSV.Text = "Load CSV";
            // 
            // MainMenuFileExit
            // 
            this.MainMenuFileExit.Name = "MainMenuFileExit";
            this.MainMenuFileExit.Size = new System.Drawing.Size(180, 22);
            this.MainMenuFileExit.Text = "Exit";
            // 
            // MainMenuSettings
            // 
            this.MainMenuSettings.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.MainMenuSettingsSearchRecursively,
            this.MainMenuSettingsHexTime,
            this.MainMenuSettingsPreferences});
            this.MainMenuSettings.Name = "MainMenuSettings";
            this.MainMenuSettings.Size = new System.Drawing.Size(61, 20);
            this.MainMenuSettings.Text = "Settings";
            // 
            // MainMenuSettingsSearchRecursively
            // 
            this.MainMenuSettingsSearchRecursively.Name = "MainMenuSettingsSearchRecursively";
            this.MainMenuSettingsSearchRecursively.Size = new System.Drawing.Size(180, 22);
            this.MainMenuSettingsSearchRecursively.Text = "Search Recursively";
            // 
            // MainMenuSettingsHexTime
            // 
            this.MainMenuSettingsHexTime.Name = "MainMenuSettingsHexTime";
            this.MainMenuSettingsHexTime.Size = new System.Drawing.Size(180, 22);
            this.MainMenuSettingsHexTime.Text = "Hex Time Format";
            // 
            // MainMenuSettingsPreferences
            // 
            this.MainMenuSettingsPreferences.Name = "MainMenuSettingsPreferences";
            this.MainMenuSettingsPreferences.Size = new System.Drawing.Size(180, 22);
            this.MainMenuSettingsPreferences.Text = "Preferences";
            // 
            // MainMenuHelp
            // 
            this.MainMenuHelp.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.MainMenuHelpReleaseNotes,
            this.MainMenuHelpAbout});
            this.MainMenuHelp.Name = "MainMenuHelp";
            this.MainMenuHelp.Size = new System.Drawing.Size(44, 20);
            this.MainMenuHelp.Text = "Help";
            // 
            // MainMenuHelpReleaseNotes
            // 
            this.MainMenuHelpReleaseNotes.Name = "MainMenuHelpReleaseNotes";
            this.MainMenuHelpReleaseNotes.Size = new System.Drawing.Size(180, 22);
            this.MainMenuHelpReleaseNotes.Text = "Release Notes";
            // 
            // MainMenuHelpAbout
            // 
            this.MainMenuHelpAbout.Name = "MainMenuHelpAbout";
            this.MainMenuHelpAbout.Size = new System.Drawing.Size(180, 22);
            this.MainMenuHelpAbout.Text = "About SymX";
            // 
            // FileList
            // 
            this.FileList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.FileListFileName,
            this.FileListImageSizeMin,
            this.FileListImageSizeMax,
            this.TimeDateStamp});
            this.FileList.Location = new System.Drawing.Point(70, 49);
            this.FileList.Name = "FileList";
            this.FileList.Size = new System.Drawing.Size(662, 335);
            this.FileList.TabIndex = 1;
            this.FileList.UseCompatibleStateImageBehavior = false;
            // 
            // FileListFileName
            // 
            this.FileListFileName.Text = "Filename";
            this.FileListFileName.Width = 250;
            // 
            // FileListImageSizeMin
            // 
            this.FileListImageSizeMin.Text = "Minimum image size";
            this.FileListImageSizeMin.Width = 100;
            // 
            // TimeDateStamp
            // 
            this.TimeDateStamp.DisplayIndex = 2;
            this.TimeDateStamp.Text = "TimeDateStamp";
            // 
            // FileListHeader
            // 
            this.FileListHeader.AutoSize = true;
            this.FileListHeader.Location = new System.Drawing.Point(70, 31);
            this.FileListHeader.Name = "FileListHeader";
            this.FileListHeader.Size = new System.Drawing.Size(33, 15);
            this.FileListHeader.TabIndex = 2;
            this.FileListHeader.Text = "Files:";
            // 
            // DownloadButton
            // 
            this.DownloadButton.Location = new System.Drawing.Point(612, 390);
            this.DownloadButton.Name = "DownloadButton";
            this.DownloadButton.Size = new System.Drawing.Size(120, 32);
            this.DownloadButton.TabIndex = 3;
            this.DownloadButton.Text = "Download";
            this.DownloadButton.UseVisualStyleBackColor = true;
            // 
            // AddFileButton
            // 
            this.AddFileButton.Location = new System.Drawing.Point(70, 390);
            this.AddFileButton.Name = "AddFileButton";
            this.AddFileButton.Size = new System.Drawing.Size(120, 32);
            this.AddFileButton.TabIndex = 4;
            this.AddFileButton.Text = "Add File";
            this.AddFileButton.UseVisualStyleBackColor = true;
            this.AddFileButton.Click += new System.EventHandler(this.AddFileButton_Click);
            // 
            // RemoveFileButton
            // 
            this.RemoveFileButton.Location = new System.Drawing.Point(196, 390);
            this.RemoveFileButton.Name = "RemoveFileButton";
            this.RemoveFileButton.Size = new System.Drawing.Size(120, 32);
            this.RemoveFileButton.TabIndex = 5;
            this.RemoveFileButton.Text = "Remove File";
            this.RemoveFileButton.UseVisualStyleBackColor = true;
            // 
            // FileListImageSizeMax
            // 
            this.FileListImageSizeMax.DisplayIndex = 3;
            this.FileListImageSizeMax.Text = "Maximum image size";
            this.FileListImageSizeMax.Width = 100;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.RemoveFileButton);
            this.Controls.Add(this.AddFileButton);
            this.Controls.Add(this.DownloadButton);
            this.Controls.Add(this.FileListHeader);
            this.Controls.Add(this.FileList);
            this.Controls.Add(this.MainMenu);
            this.MainMenuStrip = this.MainMenu;
            this.Name = "MainForm";
            this.Text = "SymX";
            this.MainMenu.ResumeLayout(false);
            this.MainMenu.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private MenuStrip MainMenu;
        private ToolStripMenuItem MainMenuFile;
        private ToolStripMenuItem MainMenuSettings;
        private ToolStripMenuItem MainMenuHelp;
        private ListView FileList;
        private Label FileListHeader;
        private Button DownloadButton;
        private ToolStripMenuItem MainMenuFileGenerateCSV;
        private ToolStripMenuItem MainMenuFileLoadCSV;
        private ToolStripMenuItem MainMenuFileExit;
        private ToolStripMenuItem MainMenuSettingsSearchRecursively;
        private ToolStripMenuItem MainMenuSettingsHexTime;
        private ToolStripMenuItem MainMenuSettingsPreferences;
        private ToolStripMenuItem MainMenuHelpReleaseNotes;
        private ToolStripMenuItem MainMenuHelpAbout;
        private ColumnHeader FileListFileName;
        private ColumnHeader FileListImageSizeMin;
        private ColumnHeader TimeDateStamp;
        private Button AddFileButton;
        private Button RemoveFileButton;
        private ColumnHeader FileListImageSizeMax;
    }
}