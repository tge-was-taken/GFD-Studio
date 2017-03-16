using System;
using System.Windows.Forms;

namespace AtlusGfdEditor.Gui
{
    public partial class MainFormSettingsForm : Form
    {
        public MainFormSettingsForm()
        {
            InitializeComponent();
            InitializeState();
            InitializeEvents();
        }

        private void InitializeState()
        {
            m_ThemeComboBox.SelectedIndex = 0;
        }

        private void InitializeEvents()
        {
            m_ThemeComboBox.SelectedIndexChanged += ThemeComboBoxSelectedIndexChangedEventHandler;
        }

        private void ThemeComboBoxSelectedIndexChangedEventHandler(object sender, EventArgs e)
        {
            MainForm.Instance.Settings.Theme = (FormTheme)m_ThemeComboBox.SelectedIndex;
        }
    }
}
