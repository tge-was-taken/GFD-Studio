using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UsefulThings.WinForms.UsefulForms
{
    public partial class KFreonMessageBox : Form
    {
        /// <summary>
        /// Provides a more customisable MessageBox with 3 buttons available. 
        /// DialogResults: Button1 = OK, button2 = Abort, button3 = Cancel.
        /// </summary>
        /// <param name="Title">String to display as title.</param>
        /// <param name="Message">String to display in the window.</param>
        /// <param name="Button1Text">First button text.</param>
        /// <param name="Button2Text">Second button text.</param>
        /// <param name="Button3Text">Third button text.</param>
        /// <param name="icon">Icon to display.</param>
        public KFreonMessageBox(string Title, string Message, string Button1Text, MessageBoxIcon icon, string Button2Text = null, string Button3Text = null)
        {
            InitializeComponent();
            this.Text = Title;
            label1.Text = Message;
            button1.Text = Button1Text;

            if (Button2Text != null)
                button2.Text = Button2Text;
            button2.Visible = Button2Text != null;

            if (Button3Text != null)
                button3.Text = Button3Text;
            button3.Visible = Button3Text != null;


            // KFreon: Deal with icon/picture
            if (pictureBox1.Image != null)
                pictureBox1.Image.Dispose();

            if (icon != 0)
                pictureBox1.Image = GetSystemImageForMessageBox(icon);
            else
                pictureBox1.Image = new Bitmap(1, 1);
        }


        /// <summary>
        /// Converts a MessageBoxIcon to a Bitmap cos that's the only thing the ImageBox can display.
        /// </summary>
        /// <param name="icon">MessageBox icon to convert.</param>
        /// <returns>Bitmap version of Icon.</returns>
        private Bitmap GetSystemImageForMessageBox(MessageBoxIcon icon)
        {
            string test = Enum.GetName(typeof(MessageBoxIcon), (object)icon);
            Bitmap bmp = null;
            try
            {
                switch (test)
                {
                    case "Asterisk":
                        bmp = SystemIcons.Asterisk.ToBitmap();
                        break;
                    case "Error":
                        bmp = SystemIcons.Error.ToBitmap();
                        break;
                    case "Exclamation":
                        bmp = SystemIcons.Exclamation.ToBitmap();
                        break;
                    case "Hand":
                        bmp = SystemIcons.Hand.ToBitmap();
                        break;
                    case "Information":
                        bmp = SystemIcons.Information.ToBitmap();
                        break;
                    case "None":
                        break;
                    case "Question":
                        bmp = SystemIcons.Question.ToBitmap();
                        break;
                    case "Stop":
                        bmp = SystemIcons.Shield.ToBitmap();
                        break;
                    case "Warning":
                        bmp = SystemIcons.Warning.ToBitmap();
                        break;
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("Failed to get image: " + e.Message);
            }

            return bmp;
        }
    }
}
