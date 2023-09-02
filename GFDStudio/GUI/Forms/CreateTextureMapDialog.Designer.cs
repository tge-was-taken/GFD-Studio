using MetroSet_UI.Controls;
using MetroSet_UI.Forms;

namespace GFDStudio.GUI.Forms
{
    partial class CreateTextureMapDialog : MetroSetForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose( bool disposing )
        {
            if ( disposing && ( components != null ) )
            {
                components.Dispose();
            }
            base.Dispose( disposing );
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            label2 = new MetroSetLabel();
            label1 = new MetroSetLabel();
            comboBox1 = new System.Windows.Forms.ComboBox();
            textBox1 = new MetroSetTextBox();
            CancelButton = new System.Windows.Forms.Button();
            OKButton = new System.Windows.Forms.Button();
            tableLayoutPanel1.SuspendLayout();
            SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.Anchor =    System.Windows.Forms.AnchorStyles.Top  |  System.Windows.Forms.AnchorStyles.Left   |  System.Windows.Forms.AnchorStyles.Right  ;
            tableLayoutPanel1.ColumnCount = 2;
            tableLayoutPanel1.ColumnStyles.Add( new System.Windows.Forms.ColumnStyle( System.Windows.Forms.SizeType.Percent, 18.88412F ) );
            tableLayoutPanel1.ColumnStyles.Add( new System.Windows.Forms.ColumnStyle( System.Windows.Forms.SizeType.Percent, 81.11588F ) );
            tableLayoutPanel1.Controls.Add( label2, 0, 1 );
            tableLayoutPanel1.Controls.Add( label1, 0, 0 );
            tableLayoutPanel1.Controls.Add( comboBox1, 1, 0 );
            tableLayoutPanel1.Controls.Add( textBox1, 1, 1 );
            tableLayoutPanel1.Location = new System.Drawing.Point( 12, 12 );
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 2;
            tableLayoutPanel1.RowStyles.Add( new System.Windows.Forms.RowStyle( System.Windows.Forms.SizeType.Percent, 50F ) );
            tableLayoutPanel1.RowStyles.Add( new System.Windows.Forms.RowStyle( System.Windows.Forms.SizeType.Percent, 50F ) );
            tableLayoutPanel1.Size = new System.Drawing.Size( 371, 71 );
            tableLayoutPanel1.TabIndex = 0;
            // 
            // label2
            // 
            label2.Anchor = System.Windows.Forms.AnchorStyles.Right;
            label2.AutoSize = true;
            label2.Font = new System.Drawing.Font( "Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point );
            label2.IsDerivedStyle = true;
            label2.Location = new System.Drawing.Point( 14, 43 );
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size( 53, 20 );
            label2.Style = MetroSet_UI.Enums.Style.Dark;
            label2.StyleManager = null;
            label2.TabIndex = 3;
            label2.Text = "Name";
            label2.ThemeAuthor = "Narwin";
            label2.ThemeName = "MetroDark";
            // 
            // label1
            // 
            label1.Anchor = System.Windows.Forms.AnchorStyles.Right;
            label1.AutoSize = true;
            label1.Font = new System.Drawing.Font( "Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point );
            label1.IsDerivedStyle = true;
            label1.Location = new System.Drawing.Point( 22, 7 );
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size( 45, 20 );
            label1.Style = MetroSet_UI.Enums.Style.Dark;
            label1.StyleManager = null;
            label1.TabIndex = 3;
            label1.Text = "Type";
            label1.ThemeAuthor = "Narwin";
            label1.ThemeName = "MetroDark";
            // 
            // comboBox1
            // 
            comboBox1.Anchor = System.Windows.Forms.AnchorStyles.Left;
            comboBox1.BackColor = System.Drawing.Color.FromArgb(   20  ,   20  ,   20   );
            comboBox1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            comboBox1.ForeColor = System.Drawing.Color.Silver;
            comboBox1.FormattingEnabled = true;
            comboBox1.Items.AddRange( new object[] { "Diffuse Map", "Normal Map", "Specular Map", "Reflection Map", "Highlight Map", "Glow Map", "Night Map", "Detail Map", "Shadow Map" } );
            comboBox1.Location = new System.Drawing.Point( 73, 3 );
            comboBox1.Name = "comboBox1";
            comboBox1.Size = new System.Drawing.Size( 295, 34 );
            comboBox1.TabIndex = 4;
            // 
            // textBox1
            // 
            textBox1.Anchor =   System.Windows.Forms.AnchorStyles.Left  |  System.Windows.Forms.AnchorStyles.Right  ;
            textBox1.AutoCompleteCustomSource = null;
            textBox1.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.None;
            textBox1.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.None;
            textBox1.BorderColor = System.Drawing.Color.FromArgb(   110  ,   110  ,   110   );
            textBox1.DisabledBackColor = System.Drawing.Color.FromArgb(   80  ,   80  ,   80   );
            textBox1.DisabledBorderColor = System.Drawing.Color.FromArgb(   109  ,   109  ,   109   );
            textBox1.DisabledForeColor = System.Drawing.Color.FromArgb(   109  ,   109  ,   109   );
            textBox1.Font = new System.Drawing.Font( "Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point );
            textBox1.HoverColor = System.Drawing.Color.FromArgb(   65  ,   177  ,   225   );
            textBox1.Image = null;
            textBox1.IsDerivedStyle = true;
            textBox1.Lines = null;
            textBox1.Location = new System.Drawing.Point( 73, 39 );
            textBox1.MaxLength = 32767;
            textBox1.Multiline = false;
            textBox1.Name = "textBox1";
            textBox1.ReadOnly = false;
            textBox1.Size = new System.Drawing.Size( 295, 28 );
            textBox1.Style = MetroSet_UI.Enums.Style.Dark;
            textBox1.StyleManager = null;
            textBox1.TabIndex = 5;
            textBox1.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            textBox1.ThemeAuthor = "Narwin";
            textBox1.ThemeName = "MetroDark";
            textBox1.UseSystemPasswordChar = false;
            textBox1.WatermarkText = "";
            // 
            // CancelButton
            // 
            CancelButton.Anchor = System.Windows.Forms.AnchorStyles.None;
            CancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            CancelButton.ForeColor = System.Drawing.Color.FromArgb(   30  ,   30  ,   30   );
            CancelButton.Location = new System.Drawing.Point( 153, 104 );
            CancelButton.Margin = new System.Windows.Forms.Padding( 10 );
            CancelButton.Name = "CancelButton";
            CancelButton.Size = new System.Drawing.Size( 97, 35 );
            CancelButton.TabIndex = 1;
            CancelButton.Text = "Cancel";
            CancelButton.UseVisualStyleBackColor = true;
            // 
            // OKButton
            // 
            OKButton.Anchor = System.Windows.Forms.AnchorStyles.None;
            OKButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            OKButton.ForeColor = System.Drawing.Color.FromArgb(   30  ,   30  ,   30   );
            OKButton.Location = new System.Drawing.Point( 270, 104 );
            OKButton.Margin = new System.Windows.Forms.Padding( 10 );
            OKButton.Name = "OKButton";
            OKButton.Size = new System.Drawing.Size( 110, 35 );
            OKButton.TabIndex = 2;
            OKButton.Text = "OK";
            OKButton.UseVisualStyleBackColor = true;
            OKButton.Click += OKButton_Click;
            // 
            // CreateTextureMapDialog
            // 
            AcceptButton = OKButton;
            AutoScaleDimensions = new System.Drawing.SizeF( 120F, 120F );
            
            BackColor = System.Drawing.Color.FromArgb(   30  ,   30  ,   30   );
            BackgroundColor = System.Drawing.Color.FromArgb(   30  ,   30  ,   30   );
            ClientSize = new System.Drawing.Size( 395, 151 );
            Controls.Add( OKButton );
            Controls.Add( CancelButton );
            Controls.Add( tableLayoutPanel1 );
            DropShadowEffect = false;
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable;
            HeaderHeight = -40;
            Name = "CreateTextureMapDialog";
            Opacity = 0.99D;
            Padding = new System.Windows.Forms.Padding( 2, 0, 2, 2 );
            ShowHeader = true;
            ShowLeftRect = false;
            SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Style = MetroSet_UI.Enums.Style.Dark;
            Text = "CREATETEXTUREMAPDIALOG";
            ThemeName = "MetroDark";
            tableLayoutPanel1.ResumeLayout( false );
            tableLayoutPanel1.PerformLayout();
            ResumeLayout( false );
        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private new System.Windows.Forms.Button CancelButton;
        private System.Windows.Forms.Button OKButton;
        private MetroSetLabel label1;
        private System.Windows.Forms.ComboBox comboBox1;
        private MetroSetLabel label2;
        private MetroSetTextBox textBox1;
    }
}