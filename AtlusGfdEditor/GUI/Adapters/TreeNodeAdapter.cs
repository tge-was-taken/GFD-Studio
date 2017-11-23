using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using AtlusGfdEditor.Modules;

namespace AtlusGfdEditor.GUI.Adapters
{

    /// <summary>
    /// Exports the resource held by an adapter to the specified file.
    /// </summary>
    /// <param name="filepath">Path to the file to export to.</param>
    public delegate void TreeNodeAdapterExportAction( string filepath );

    /// <summary>
    /// Replaces the resource in the adapter with a resource loaded from a file.
    /// </summary>
    /// <param name="filepath">Path to the resource file to load.</param>
    public delegate object TreeNodeAdapterReplaceAction( string filepath );

    /// <summary>
    /// Loads a resource and adds the corresponding adapter to the tree node.
    /// </summary>
    /// <param name="filepath">Path to the resource file to load.</param>
    public delegate void TreeNodeAdapterAddAction( string filepath );

    /// <summary>
    /// Represents an adapter for a node of a TreeView with extra additions.
    /// </summary>
    public abstract partial class TreeNodeAdapter : TreeNode, INotifyPropertyChanged
    {
        [Flags]
        public enum MenuFlags
        {
            Export = 0b00000001,
            Replace = 0b00000010,
            Add = 0b00000100,
            Move = 0b00001000,
            Rename = 0b00010000,
            Delete = 0b00100000,
        }

        [Flags]
        public enum Flags
        {
            Leaf = 0b0001,
            Branch = 0b0010,
        }

        private Dictionary<Type, TreeNodeAdapterExportAction> mExportActions;
        private Dictionary<Type, TreeNodeAdapterReplaceAction> mReplaceActions;
        private Dictionary<Type, TreeNodeAdapterAddAction> mAddActions;

        /// <summary>
        /// Gets or sets the resource held by the adapter.
        /// </summary>
        [Browsable( false )]
        public object Resource { get; protected set; }

        /// <summary>
        /// Gets the resource type.
        /// </summary>
        [Browsable( false )]
        public abstract Type ResourceType { get; }

        /// <summary>
        /// Gets the context menu flags.
        /// </summary>
        [Browsable( false )]
        public abstract MenuFlags ContextMenuFlags { get; }

        /// <summary>
        /// Gets the node flags.
        /// </summary>
        [Browsable( false )]
        public abstract Flags NodeFlags { get; }

        /// <summary>
        /// Gets or sets the text displayed in the label of the tree node.
        /// </summary>
        [Browsable( false )]
        public new string Text
        {
            get => base.Text;
            set
            {
                if ( value != base.Text )
                {
                    base.Text = value;
                    NotifyPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets the parent of the tree node.
        /// </summary>
        [Browsable( false )]
        public new TreeNodeAdapter Parent
        {
            get => ( TreeNodeAdapter )base.Parent;
        }

        /// <summary>
        /// Gets the parent tree view of the tree node.
        /// </summary>
        [Browsable( false )]
        public new TreeNodeAdapterView TreeView
        {
            get => ( TreeNodeAdapterView )base.TreeView;
        }

        /// <summary>
        /// This event is fired whenever the tree node label text is changed.
        /// </summary>
        [Browsable( false )]
        public event EventHandler<NodeLabelEditEventArgs> TextChanged;

        /// <summary>
        /// This event is fired whenever a property of the tree node is changed.
        /// </summary>
        [Browsable( false )]
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Gets if the node is initialized.
        /// </summary>
        protected bool IsInitialized { get; private set; }

        /// <summary>
        /// Gets if the node's view is initialized.
        /// </summary>
        protected bool IsViewInitialized { get; private set; }

        private bool mIsDirty;

        /// <summary>
        /// Gets or sets if the tree node is dirty and its resource needs to be rebuilt.
        /// </summary>
        protected bool IsDirty
        {
            get => mIsDirty;
            set
            {
                Trace.TraceInformation( $"{nameof( TreeNodeAdapter )} [{Text}]: {nameof( IsDirty )} => {value}" );

                mIsDirty = value;

                if ( Parent != null )
                {
                    Parent.IsDirty = true;
                }
            }
        }

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

            using ( var dialog = new SaveFileDialog() )
            {
                dialog.AutoUpgradeEnabled = true;
                dialog.CheckPathExists = true;
                dialog.FileName = Text;
                dialog.Filter = ModuleFilterGenerator.GenerateFilter( FormatModuleUsageFlags.Export, mExportActions.Keys.ToArray() );
                dialog.OverwritePrompt = true;
                dialog.Title = "Select a file to export to.";
                dialog.ValidateNames = true;
                dialog.AddExtension = true;

                if ( dialog.ShowDialog() != DialogResult.OK )
                {
                    return;
                }

                Export( dialog.FileName );
            }
        }

        public void Export( string filepath )
        {
            if ( mExportActions.Count == 0 )
                return;

            var type = GetTypeFromPath( filepath, mExportActions.Keys );
            var exportAction = mExportActions[type];

            Trace.TraceInformation( $"{nameof( TreeNodeAdapter )} [{Text}]: {nameof( Export )} {type} to {filepath}" );

            exportAction( filepath );
        }

        //
        // Replace methods
        //
        public void Replace()
        {
            if ( mReplaceActions.Count == 0 )
                return;

            using ( var dialog = new OpenFileDialog() )
            {
                dialog.AutoUpgradeEnabled = true;
                dialog.CheckPathExists = true;
                dialog.CheckFileExists = true;
                dialog.FileName = Text;
                dialog.Filter = ModuleFilterGenerator.GenerateFilter(
                    new[] { FormatModuleUsageFlags.Import, FormatModuleUsageFlags.ImportForEditing },
                    mReplaceActions.Keys.ToArray() );
                dialog.Multiselect = false;
                dialog.SupportMultiDottedExtensions = true;
                dialog.Title = "Select a replacement file.";
                dialog.ValidateNames = true;

                if ( dialog.ShowDialog() != DialogResult.OK )
                {
                    return;
                }

                Replace( dialog.FileName );
            }
        }

        public void Replace( string filepath )
        {
            if ( mReplaceActions.Count == 0 )
                return;

            var type = GetTypeFromPath( filepath, mReplaceActions.Keys );
            var replaceAction = mReplaceActions[type];

            Trace.TraceInformation( $"{nameof( TreeNodeAdapter )} [{Text}]: {nameof( Replace )} {type} from {filepath}" );

            Resource = replaceAction( filepath );
            NotifyResourcePropertyChanged();
            InitializeView( true );
            TreeView.RefreshSelection();
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

            Trace.TraceInformation( $"{nameof( TreeNodeAdapter )} [{Text}]: {nameof( Add )} {type} from {filepath}" );

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

        // INotifyPropertyChanged
        protected void SetProperty<T>( object instance, T value, [CallerMemberName] string propertyName = null )
        {
            if ( propertyName == null )
                throw new ArgumentNullException( nameof( propertyName ) );

            var instanceType = instance.GetType();
            var propertyType = typeof( T );

            var property = instanceType.GetProperty( propertyName, propertyType );
            if ( property == null )
                throw new ArgumentNullException( nameof( property ) );

            if ( propertyType.IsValueType )
            {
                if ( Equals( property.GetValue( instance, null ), value ) )
                    return;
            }

            property.SetValue( instance, value );

            NotifyPropertyChanged( propertyName );
        }

        protected void SetResourceProperty<T>( T value, [CallerMemberName] string propertyName = null )
        {
            // ReSharper disable once ExplicitCallerInfoArgument
            SetProperty( Resource, value, propertyName );
        }

        protected void SetResourceProperty<T>( ref T property )
        {

        }

        protected void SetProperty<T>( T value, [CallerMemberName] string propertyName = null )
        {
            SetProperty( this, value, propertyName );
        }

        protected T GetResourceProperty<T>( [CallerMemberName] string propertyName = null )
        {
            if ( propertyName == null )
                throw new ArgumentNullException( nameof( propertyName ) );

            var prop = ResourceType.GetProperty( propertyName );
            if ( prop == null )
                throw new ArgumentNullException( nameof( prop ) );

            return ( T )prop.GetValue( Resource );
        }

        protected void NotifyPropertyChanged( [CallerMemberName]string propertyName = null )
        {
            // dont bother if instance is not initialized
            if ( !IsInitialized )
                return;

            Trace.TraceInformation( $"{nameof( TreeNodeAdapter )} [{Text}]: {nameof( NotifyPropertyChanged )} {propertyName}" );

            // set dirty flag as a property was changed
            IsDirty = true;

            // invoke property changed event
            PropertyChanged?.Invoke( this, new PropertyChangedEventArgs( propertyName ) );
        }

        protected void NotifyResourcePropertyChanged()
        {
            // dont bother if instance is not initialized
            if ( !IsInitialized )
                return;

            Trace.TraceInformation( $"{nameof( TreeNodeAdapter )} [{Text}]: {nameof( NotifyResourcePropertyChanged )}" );

            // set dirty flag to false because the resource is already in sync with the view model
            IsDirty = false;

            // invoke property changed event
            PropertyChanged?.Invoke( this, new PropertyChangedEventArgs( nameof( Resource ) ) );
        }

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

        protected virtual void InitializeViewCore() { }

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

            Text = e.Label;

            // unsubscribe from the event
            if ( TreeView != null )
                TreeView.AfterLabelEdit -= EndRename;

            TextChanged.Invoke( this, e );
        }

        //
        // Private initialize methods
        //

        /// <summary>
        /// Called by <see cref="TreeNodeAdapterFactory"/>.
        /// </summary>
        protected internal void Initialize()
        {
            // check for double initialization
            if ( IsInitialized )
                throw new Exception( $"{nameof( Initialize )} was called twice" );

            // initialize the derived adapter
            InitializeCore();

            // set initialization flag
            IsInitialized = true;

            // subscribe to the PropertyChanged event /after/ init
            PropertyChanged += OnPropertyChanged;
        }

        protected virtual void OnPropertyChanged( object sender, PropertyChangedEventArgs e )
        {
            IsViewInitialized = false;
        }

        protected abstract void Rebuild();

        /// <summary>
        /// Populates the view -- which is the current node's child nodes and/or any other properties
        /// </summary>
        protected internal void InitializeView( bool force = false )
        {
            if ( !force && !IsDirty && IsViewInitialized )
                return;

            Trace.TraceInformation( $"{nameof( TreeNodeAdapter )} [{Text}]: {nameof( InitializeView )}" );

            // rebuild for initializing view, if necessary
            if ( IsDirty )
                Rebuild();

            if ( NodeFlags.HasFlag( Flags.Branch ) )
            {
                // clear nodes, will get rid of the dummy node added by the hack required for late view initialization
                Nodes.Clear();

                // let the derived adapter populate the view
                InitializeViewCore();
            }

            if ( !IsViewInitialized )
            {
                // initialize the context menu strip if the adapter isn't initialized yet
                InitializeContextMenuStrip();
            }

            IsViewInitialized = true;
        }

        /// <summary>
        /// Populates the context menu strip, should be called after the context menu options have been set.
        /// </summary>
        private void InitializeContextMenuStrip()
        {
            ContextMenuStrip = new ContextMenuStrip();

            if ( ContextMenuFlags.HasFlag( MenuFlags.Export ) )
            {
                ContextMenuStrip.Items.Add( new ToolStripMenuItem( "&Export", null, CreateEventHandler( Export ), Keys.Control | Keys.E ) );
            }

            if ( ContextMenuFlags.HasFlag( MenuFlags.Replace ) )
            {
                ContextMenuStrip.Items.Add( new ToolStripMenuItem( "&Replace", null, CreateEventHandler( Replace ), Keys.Control | Keys.R ) );
                if ( !ContextMenuFlags.HasFlag( MenuFlags.Add ) )
                    ContextMenuStrip.Items.Add( new ToolStripSeparator() );
            }

            if ( ContextMenuFlags.HasFlag( MenuFlags.Add ) )
            {
                ContextMenuStrip.Items.Add( new ToolStripMenuItem( "&Add", null, CreateEventHandler( Add ), Keys.Control | Keys.A ) );
                if ( ContextMenuFlags.HasFlag( MenuFlags.Move ) )
                    ContextMenuStrip.Items.Add( new ToolStripSeparator() );
            }

            if ( ContextMenuFlags.HasFlag( MenuFlags.Move ) )
            {
                ContextMenuStrip.Items.Add( new ToolStripMenuItem( "Move &Up", null, CreateEventHandler( MoveUp ), Keys.Control | Keys.Up ) );
                ContextMenuStrip.Items.Add( new ToolStripMenuItem( "Move &Down", null, CreateEventHandler( MoveDown ), Keys.Control | Keys.Down ) );
            }

            if ( ContextMenuFlags.HasFlag( MenuFlags.Rename ) )
            {
                ContextMenuStrip.Items.Add( new ToolStripMenuItem( "Re&name", null, CreateEventHandler( BeginRename ), Keys.Control | Keys.N ) );
            }

            if ( ContextMenuFlags.HasFlag( MenuFlags.Delete ) )
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
            if ( string.IsNullOrEmpty( extension ) )
                extension = string.Empty;
            else
                extension = extension.Substring( 1 );

            var modulesWithType = ModuleRegistry.Modules.Where( x => types.Contains( x.ObjectType ) );
            List<IModule> modules;
            if ( extension.Length > 0 )
            {
                modules = modulesWithType.Where( x =>
                                                     ( x.Extensions.Any( ext => ext == "*" ) ||
                                                       x.Extensions.Contains( extension, StringComparer.InvariantCultureIgnoreCase ) ) )
                                         .ToList();
            }
            else
            {
                modules = modulesWithType.Where( x => x.Extensions.Any( ext => ext == "*" ) )
                                         .ToList();
            }

            // remove wild card modules if we have more than 1 module
            if ( modules.Count > 1 )
            {
                modules.RemoveAll( x => x.Extensions.Contains( "*" ) );

                if ( modules.Count == 0 )
                    throw new Exception( "Only suitable modules are multiple modules with wild cards?" );
            }

            if ( modules.Count == 0 )
                throw new Exception( "No suitable modules found" );

            if ( modules.Count != 1 )
                throw new Exception( "Ambigious module match. Multiple suitable modules format found." );

            return modules[0].ObjectType;
        }

        private EventHandler CreateEventHandler( Action action )
        {
            return ( s, e ) => action();
        }
    }
}
