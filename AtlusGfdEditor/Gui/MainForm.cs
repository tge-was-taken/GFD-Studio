using System;
using System.Windows.Forms;
using System.IO;
using AtlusGfdEditor.Framework;
using AtlusGfdFramework;
using AtlusGfdEditor.Gui.WrapperTreeNodes;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace AtlusGfdEditor.Gui
{
    public partial class MainForm : Form
    {
        private MainFormSettingsForm m_SettingsForm;

        // Singleton implementation
        private static MainForm m_Instance;
        public static MainForm Instance
        {
            get { return m_Instance; }
            set
            {
                if (m_Instance != null)
                    throw new SingletonException(nameof(MainForm));

                m_Instance = value;
            }
        }

        public ApplicationSettings Settings { get; private set; }

        public MainForm()
        {
            InitializeComponent();
            InitializeState();
            LoadSettings();
            InitializeEvents();
        }

        private void InitializeState()
        {
            Instance = this;
            m_TreeView.LabelEdit = true;
        }

        private void LoadSettings()
        {
            Settings = new ApplicationSettings();
            
            // load settings from file maybe?
        }

        private void InitializeEvents()
        {
            Settings.PropertyChanged += SettingsPropertyChangedHandler;
            m_TreeView.AfterSelect += TreeViewAfterSelectEventHandler;
        }

        private void LoadDefaultTheme()
        {

        }

        private void LoadDarkTheme()
        {

        }

        private void LoadResourceBundle(GfdResourceBundle resourceBundle, string name)
        {
            var resourceBundleNode = WrapperTreeNodeFactory.CreateWrapper(resourceBundle);
            resourceBundleNode.Text = name;

            m_TreeView.Nodes.Add(resourceBundleNode);
        }

        private void LoadTexture(GfdTexture texture)
        {
            if (texture.Format == GfdTextureFormat.DDS)
            {
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            Application.Exit();
        }

        // Event handlers
        private void SettingsPropertyChangedHandler(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Settings.Theme))
            {
                switch (Settings.Theme)
                {
                    case FormTheme.Default:
                        LoadDefaultTheme();
                        break;
                    case FormTheme.Dark:
                        LoadDarkTheme();
                        break;
                }
            }

        }

        private void TreeViewAfterSelectEventHandler(object sender, TreeViewEventArgs e)
        {
            // Set property grid to display properties of the currently selected node
            m_PropertyGrid.SelectedObject = e.Node;

            // Attempt to load the node as a texture
            GfdTexture texture = e.Node.Tag as GfdTexture;
            if (texture == null)
                return;

            LoadTexture(texture);
        }

        private void OpenToolStripMenuItemClickEventHandler(object sender, EventArgs e)
        {
            using (var openFileDlg = new OpenFileDialog())
            {
                if (openFileDlg.ShowDialog() != DialogResult.OK)
                    return;

                var resource = GfdResource.Load(openFileDlg.FileName);

                // Cancel if resource couldn't be loaded
                if (resource == null)
                {
                    MessageBox.Show("File could not be loaded.", "Error", MessageBoxButtons.OK);
                    return;
                }

                // Clear nodes before loading anything
                m_TreeView.Nodes.Clear();

                switch (resource.Type)
                {
                    case GfdResourceType.ResourceBundle:
                        LoadResourceBundle(resource as GfdResourceBundle, Path.GetFileName(openFileDlg.FileName));
                        break;

                    case GfdResourceType.ShaderCache:
                        //LoadShaderCache(resource as GfdShaderCache, Path.GetFileName(openFileDlg.FileName));
                        break;

                    default:
                        return;
                }
            }
        }

        private void OptionsToolStripMenuItemClickEventHandler(object sender, EventArgs e)
        {
            if (m_SettingsForm == null)
                m_SettingsForm = new MainFormSettingsForm();

            m_SettingsForm.Show();
        }
    }
}
