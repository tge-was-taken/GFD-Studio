using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AtlusGfdEditor.Modules;

namespace AtlusGfdEditor.GUI.Adapters
{
    public enum ContextMenuOptions
    {
        Export  = 0b00000001,
        Replace = 0b00000010,
        Add     = 0b00000100,
        Move    = 0b00001000,
        Rename  = 0b00010000,
        Delete  = 0b00100000,
    }

    public delegate void TreeNodeAdapterExportAction( string filepath );

    public delegate void TreeNodeAdapterReplaceAction( string filepath );

    public delegate void TreeNodeAdapterAddAction( string filepath );

    public abstract class TreeNodeAdapter : TreeNode
    {
        private Dictionary<Type, TreeNodeAdapterExportAction> mExportActions;
        private Dictionary<Type, TreeNodeAdapterReplaceAction> mReplaceActions;
        private Dictionary<Type, TreeNodeAdapterAddAction> mAddActions;

        public object Resource { get; protected set; }

        public ContextMenuOptions ContextMenuOptions { get; protected set; }

        protected bool IsInitialized { get; private set; }

        protected TreeNodeAdapter( string text ) : base( text )
        {
            mExportActions = new Dictionary<Type, TreeNodeAdapterExportAction>();
            mReplaceActions = new Dictionary<Type, TreeNodeAdapterReplaceAction>();
            mAddActions = new Dictionary<Type, TreeNodeAdapterAddAction>();
        }

        //
        // Export methods
        //
        public void Export()
        {
            if ( mExportActions.Count == 0 )
                return;

            using ( var saveFileDlg = new SaveFileDialog() )
            {
                saveFileDlg.AutoUpgradeEnabled = true;
                saveFileDlg.CheckPathExists = true;
                saveFileDlg.FileName = Text;
                saveFileDlg.Filter = ModuleFilterGenerator.GenerateFilter( FormatModuleUsageFlags.Export, mExportActions.Keys.ToArray() );
                saveFileDlg.OverwritePrompt = true;
                saveFileDlg.Title = "Select a file to export to.";
                saveFileDlg.ValidateNames = true;

                if ( saveFileDlg.ShowDialog() != DialogResult.OK )
                {
                    return;
                }

                Export( saveFileDlg.FileName );
            }
        }

        public void Export( string filepath )
        {
            if ( mExportActions.Count == 0 )
                return;

            var type = GetTypeFromPath( filepath, mExportActions.Keys );
            var exportAction = mExportActions[type];

            exportAction.Invoke( filepath );
        }

        //
        // Replace methods
        //
        public void Replace()
        {
            if ( mReplaceActions.Count == 0 )
                return;

            using ( var openFileDlg = new OpenFileDialog() )
            {
                openFileDlg.AutoUpgradeEnabled = true;
                openFileDlg.CheckPathExists = true;
                openFileDlg.CheckFileExists = true;
                openFileDlg.FileName = Text;
                openFileDlg.Filter = ModuleFilterGenerator.GenerateFilter( FormatModuleUsageFlags.Import | FormatModuleUsageFlags.ImportForEditing, mReplaceActions.Keys.ToArray() );
                openFileDlg.Multiselect = false;
                openFileDlg.SupportMultiDottedExtensions = true;
                openFileDlg.Title = "Select a replacement file.";
                openFileDlg.ValidateNames = true;

                if ( openFileDlg.ShowDialog() != DialogResult.OK )
                {
                    return;
                }

                Replace( openFileDlg.FileName );
            }
        }

        public void Replace( string filepath )
        {
            if ( mReplaceActions.Count == 0 )
                return;

            var type = GetTypeFromPath( filepath, mReplaceActions.Keys );
            var replaceAction = mReplaceActions[type];

            replaceAction.Invoke( filepath );
        }

        //
        // Add methods
        //
        public void Add()
        {
            if ( mAddActions.Count == 0 )
                return;

            using ( OpenFileDialog openFileDlg = new OpenFileDialog() )
            {
                openFileDlg.AutoUpgradeEnabled = true;
                openFileDlg.CheckPathExists = true;
                openFileDlg.CheckFileExists = true;
                openFileDlg.Filter = ModuleFilterGenerator.GenerateFilter( FormatModuleUsageFlags.Import | FormatModuleUsageFlags.ImportForEditing, mAddActions.Keys.ToArray() );
                openFileDlg.Multiselect = true;
                openFileDlg.SupportMultiDottedExtensions = true;
                openFileDlg.Title = "Select file(s) to add.";
                openFileDlg.ValidateNames = true;

                if ( openFileDlg.ShowDialog() != DialogResult.OK )
                {
                    return;
                }

                foreach ( string fileName in openFileDlg.FileNames )
                {
                    Add( fileName );
                }
            }

        }

        public void Add( string filepath )
        {
            if ( mAddActions.Count == 0 )
                return;

            var type = GetTypeFromPath( filepath, mAddActions.Keys );
            var addAction = mAddActions[type];

            addAction.Invoke( filepath );
        }

        // 
        // Move methods
        //
        public void MoveUp()
        {
            TreeNode parent = Parent;

            if ( parent != null )
            {
                int index = parent.Nodes.IndexOf( this );
                if ( index > 0 )
                {
                    parent.Nodes.RemoveAt( index );
                    parent.Nodes.Insert( index - 1, this );
                }
            }
            else if ( TreeView != null && TreeView.Nodes.Contains( this ) ) //root node
            {
                int index = TreeView.Nodes.IndexOf( this );
                if ( index > 0 )
                {
                    TreeView.Nodes.RemoveAt( index );
                    TreeView.Nodes.Insert( index - 1, this );
                }
            }

            if ( TreeView != null )
                TreeView.SelectedNode = this;
        }

        public void MoveDown()
        {
            TreeNode parent = Parent;
            if ( parent != null )
            {
                int index = parent.Nodes.IndexOf( this );
                if ( index < parent.Nodes.Count - 1 )
                {
                    parent.Nodes.RemoveAt( index );
                    parent.Nodes.Insert( index + 1, this );
                }
            }
            else if ( TreeView != null && TreeView.Nodes.Contains( this ) ) //root node
            {
                int index = TreeView.Nodes.IndexOf( this );
                if ( index < TreeView.Nodes.Count - 1 )
                {
                    TreeView.Nodes.RemoveAt( index );
                    TreeView.Nodes.Insert( index + 1, this );
                }
            }

            if ( TreeView != null )
                TreeView.SelectedNode = this;
        }

        //
        // Delete method
        //
        public void Delete() => Remove();

        // 
        // Action register actions
        //
        protected void RegisterExportAction<T>( TreeNodeAdapterExportAction action )
        {
            mExportActions[typeof( T )] = action;
        }

        protected void RegisterReplaceAction<T>( TreeNodeAdapterReplaceAction action )
        {
            mReplaceActions[typeof( T )] = action;
        }

        protected void RegisterAddAction<T>( TreeNodeAdapterAddAction action )
        {
            mAddActions[typeof( T )] = action;
        }

        //
        // Initialization actions
        //
        protected abstract void InitializeCore();

        protected abstract void InitializeViewCore();

        // Rename methods
        private void BeginRename()
        {
            // renaming can only be done through the treeview that owns this node
            if ( TreeView != null )
            {
                // start editing the name
                TreeView.LabelEdit = true;
                BeginEdit();

                // subscribe to the label edit event, so we can finish 
                TreeView.AfterLabelEdit += EndRename;
            }
        }

        private void EndRename( object sender, NodeLabelEditEventArgs e )
        {
            // stop editing the name
            EndEdit( false );

            // unsubscribe from the event
            if ( TreeView != null )
                TreeView.AfterLabelEdit -= EndRename;
        }

        //
        // Private initialize methods
        //

        // will be used by factory to initialize adapters
        protected internal void Initialize()
        {
            if ( IsInitialized )
                throw new Exception( $"{nameof(Initialize)} was called twice" );

            // initialize the derived adapter
            InitializeCore();

            // initialize the adapter view
            InitializeView();

            IsInitialized = true;
        }

        protected void InitializeView()
        {
            // clean up any nodes if this isn't the first initialization
            if ( IsInitialized )
                Nodes.Clear();

            // let the derived adapter populate the view
            InitializeViewCore();

            if ( !IsInitialized )
            {
                // initialize the context menu strip
                InitializeContextMenuStrip();
            }
        }

        /// <summary>
        /// Populates the context menu strip, should be called after the context menu options have been set.
        /// </summary>
        private void InitializeContextMenuStrip()
        {
            ContextMenuStrip = new ContextMenuStrip();

            if ( ContextMenuOptions.HasFlag( ContextMenuOptions.Export ) )
            {
                ContextMenuStrip.Items.Add( new ToolStripMenuItem( "&Export", null, CreateEventHandler( Export ), Keys.Control | Keys.E ) );
            }

            if ( ContextMenuOptions.HasFlag( ContextMenuOptions.Replace ) )
            {
                ContextMenuStrip.Items.Add( new ToolStripMenuItem( "&Replace", null, CreateEventHandler( Replace ), Keys.Control | Keys.R ) );
                if ( !ContextMenuOptions.HasFlag( ContextMenuOptions.Add ) )
                    ContextMenuStrip.Items.Add( new ToolStripSeparator() );
            }

            if ( ContextMenuOptions.HasFlag( ContextMenuOptions.Add ) )
            {
                ContextMenuStrip.Items.Add( new ToolStripMenuItem( "&Add", null, CreateEventHandler( Add ), Keys.Control | Keys.A ) );
                if ( ContextMenuOptions.HasFlag( ContextMenuOptions.Move ) )
                    ContextMenuStrip.Items.Add( new ToolStripSeparator() );
            }

            if ( ContextMenuOptions.HasFlag( ContextMenuOptions.Move ) )
            {
                ContextMenuStrip.Items.Add( new ToolStripMenuItem( "Move &Up", null, CreateEventHandler( MoveUp ), Keys.Control | Keys.Up ) );
                ContextMenuStrip.Items.Add( new ToolStripMenuItem( "Move &Down", null, CreateEventHandler( MoveDown ), Keys.Control | Keys.Down ) );
            }

            if ( ContextMenuOptions.HasFlag( ContextMenuOptions.Rename ) )
            {
                ContextMenuStrip.Items.Add( new ToolStripMenuItem( "Re&name", null, CreateEventHandler( BeginRename ), Keys.Control | Keys.N ) );
            }

            if ( ContextMenuOptions.HasFlag( ContextMenuOptions.Delete ) )
            {
                ContextMenuStrip.Items.Add( new ToolStripMenuItem( "&Delete", null, CreateEventHandler( Delete ), Keys.Control | Keys.Delete ) );
            }
        }

        //
        // Helpers
        //
        private Type GetTypeFromPath( string filepath, IEnumerable<Type> types )
        {
            var extension = Path.GetExtension( filepath );
            var modules = ModuleRegistry.Modules.Where(
                x => types.Contains( x.ObjectType ) &&
                x.Extensions.Contains( extension, StringComparer.InvariantCultureIgnoreCase ) )
                .ToList();

            if ( modules.Count == 0 )
            {
                modules = ModuleRegistry.Modules.Where(x => types.Contains( x.ObjectType ) ).ToList();
                if ( modules.Count == 0 )
                {
                    throw new Exception( "Can't find module for export" );
                }
                else if ( modules.Count != 1 )
                {
                    throw new Exception( "Ambigious format" );
                }
            }
            else if ( modules.Count != 1 )
                throw new Exception( "Ambigious format" );

            return modules[0].ObjectType;
        }

        private EventHandler CreateEventHandler( Action action )
        {
            return ( s, e ) => action();
        }
    }
}
