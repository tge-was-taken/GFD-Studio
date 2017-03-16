namespace AtlusGfdEditor.Gui
{
    partial class MainFormSettingsForm
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
            this.m_ThemeComboBox = new System.Windows.Forms.ComboBox();
            this.m_ThemeLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // m_ThemeComboBox
            // 
            this.m_ThemeComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.m_ThemeComboBox.FormattingEnabled = true;
            this.m_ThemeComboBox.Items.AddRange(new object[] {
            "Default",
            "Dark"});
            this.m_ThemeComboBox.Location = new System.Drawing.Point(98, 12);
            this.m_ThemeComboBox.Name = "m_ThemeComboBox";
            this.m_ThemeComboBox.Size = new System.Drawing.Size(121, 21);
            this.m_ThemeComboBox.TabIndex = 0;
            // 
            // m_ThemeLabel
            // 
            this.m_ThemeLabel.AutoSize = true;
            this.m_ThemeLabel.Location = new System.Drawing.Point(49, 15);
            this.m_ThemeLabel.Name = "m_ThemeLabel";
            this.m_ThemeLabel.Size = new System.Drawing.Size(43, 13);
            this.m_ThemeLabel.TabIndex = 1;
            this.m_ThemeLabel.Text = "Theme:";
            // 
            // MainFormSettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 46);
            this.Controls.Add(this.m_ThemeLabel);
            this.Controls.Add(this.m_ThemeComboBox);
            this.Name = "MainFormSettingsForm";
            this.Text = "MainFormSettingsForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox m_ThemeComboBox;
        private System.Windows.Forms.Label m_ThemeLabel;
    }
}