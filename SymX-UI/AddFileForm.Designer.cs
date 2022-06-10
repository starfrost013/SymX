namespace SymX_UI
{
    partial class AddFileForm
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
            this.AddFileHeader = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // AddFileHeader
            // 
            this.AddFileHeader.AutoSize = true;
            this.AddFileHeader.Font = new System.Drawing.Font("Segoe UI", 36F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.AddFileHeader.Location = new System.Drawing.Point(12, 9);
            this.AddFileHeader.Name = "AddFileHeader";
            this.AddFileHeader.Size = new System.Drawing.Size(200, 65);
            this.AddFileHeader.TabIndex = 0;
            this.AddFileHeader.Text = "Add File";
            // 
            // AddFileForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.AddFileHeader);
            this.Name = "AddFileForm";
            this.Text = "Add File";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Label AddFileHeader;
    }
}