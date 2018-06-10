using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using GFDStudio.FormatModules;

namespace GFDStudio.GUI.ViewModels
{

    /// <summary>
    /// Exports the resource held by an view model to the specified file.
    /// </summary>
    /// <param name="filepath">Path to the file to export to.</param>
    public delegate void TreeNodeAdapterExportHandler( string filepath );

    /// <summary>
    /// Replaces the resource in the view model with a resource loaded from a file.
    /// </summary>
    /// <param name="filepath">Path to the resource file to load.</param>
    public delegate object TreeNodeAdapterReplaceHandler( string filepath );

    /// <summary>
    /// Loads a resource and adds the corresponding view model to the tree node.
    /// </summary>
    /// <param name="filepath">Path to the resource file to load.</param>
    public delegate void TreeNodeAdapterAddHandler( string filepath );

    public struct TreeNodeActionHandler<T>
    {
        public readonly Type Type;
        public readonly T Handler;

        public TreeNodeActionHandler( Type type, T handler )
        {
            Type = type;
            Handler = handler;
        }
    }

    /// <summary>
    /// Represents an view model for a node of a TreeView with extra additions.
    /// </summary>
    public abstract partial class TreeNodeViewModel : TreeNode, INotifyPropertyChanged
    {
        private readonly Dictionary<Type, TreeNodeAdapterExportHandler> mExportHandlers;
        private readonly Dictionary<Type, TreeNodeAdapterReplaceHandler> mReplaceHandlers;
        private readonly Dictionary<Type, TreeNodeAdapterAddHandler> mAddHandlers;

        /// <summary>
        /// Gets or sets the resource held by the view model.
        /// </summary>
        [Browsable( false )]
        public object Model { get; protected set; }

        /// <summary>
        /// Gets the resource type.
        /// </summary>
        [Browsable( false )]
        public abstract Type ModelType { get; }

        /// <summary>
        /// Gets the context menu flags.
        /// </summary>
        [Browsable( false )]
        public abstract TreeNodeViewModelMenuFlags ContextMenuFlags { get; }

        /// <summary>
        /// Gets the node flags.
        /// </summary>
        [Browsable( false )]
        public abstract TreeNodeViewModelFlags NodeFlags { get; }

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
        public new TreeNodeViewModel Parent
        {
            get => ( TreeNodeViewModel )base.Parent;
        }

        /// <summary>
        /// Gets the parent tree view of the tree node.
        /// </summary>
        [Browsable( false )]
        public new TreeNodeViewModelView TreeView
        {
            get => ( TreeNodeViewModelView )base.TreeView;
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

        private bool mHasPendingChanges;
        private bool mIsInitializingView;
        private List<ToolStripMenuItem> mCustomHandlers;

        /// <summary>
        /// Gets or sets if the tree node is dirty and its resource needs to be rebuilt.
        /// </summary>
        internal bool HasPendingChanges
        {
            get => mHasPendingChanges;
            set
            {
                Trace.TraceInformation( $"{nameof( TreeNodeViewModel )} [{Text}]: {nameof( HasPendingChanges )} => {value}" );

                mHasPendingChanges = value;

                if ( Parent != null )
                {
                    Parent.HasPendingChanges = true;
                }
            }
        }

        protected TreeNodeViewModel( string text ) : base( text )
        {
            mExportHandlers = new Dictionary<Type, TreeNodeAdapterExportHandler>();
            mReplaceHandlers = new Dictionary<Type, TreeNodeAdapterReplaceHandler>();
            mAddHandlers = new Dictionary<Type, TreeNodeAdapterAddHandler>();
            mCustomHandlers = new List< ToolStripMenuItem >();
        }

        //
        // Export methods
        //
        public string Export()
        {
            if ( mExportHandlers.Count == 0 )
                return null;

            using ( var dialog = new SaveFileDialog() )
            {
                dialog.AutoUpgradeEnabled = true;
                dialog.CheckPathExists = true;
                dialog.FileName = Text;
                dialog.Filter = ModuleFilterGenerator.GenerateFilter( FormatModuleUsageFlags.Export, mExportHandlers.Keys.ToArray() );
                dialog.OverwritePrompt = true;
                dialog.Title = "Select a file to export to.";
                dialog.ValidateNames = true;
                dialog.AddExtension = true;

                if ( dialog.ShowDialog() != DialogResult.OK )
                {
                    return null;
                }

                Export( dialog.FileName );
                return dialog.FileName;
            }
        }

        public void Export( string filepath )
        {
            if ( mExportHandlers.Count == 0 )
                return;

            var type = GetTypeFromPath( filepath, mExportHandlers.Keys );
            var exportAction = mExportHandlers[type];

            Trace.TraceInformation( $"{nameof( TreeNodeViewModel )} [{Text}]: {nameof( Export )} {type} to {filepath}" );

            exportAction( filepath );
        }

        //
        // Replace methods
        //
        public void Replace()
        {
            if ( mReplaceHandlers.Count == 0 )
                return;

            using ( var dialog = new OpenFileDialog() )
            {
                dialog.AutoUpgradeEnabled = true;
                dialog.CheckPathExists = true;
                dialog.CheckFileExists = true;
                dialog.FileName = Text;
                dialog.Filter = ModuleFilterGenerator.GenerateFilter(
                    new[] { FormatModuleUsageFlags.Import, FormatModuleUsageFlags.ImportForEditing },
                    mReplaceHandlers.Keys.ToArray() );
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
            if ( mReplaceHandlers.Count == 0 )
                return;

            var type = GetTypeFromPath( filepath, mReplaceHandlers.Keys );
            var replaceAction = mReplaceHandlers[type];

            Trace.TraceInformation( $"{nameof( TreeNodeViewModel )} [{Text}]: {nameof( Replace )} {type} from {filepath}" );

            Replace( replaceAction( filepath ) );
        }

        public void Replace( object model )
        {
            Model = model;
            NotifyModelPropertyChanged();
            InitializeView( true );
            TreeView.RefreshSelection();
        }

        //
        // Add methods
        //
        public void Add()
        {
            if ( mAddHandlers.Count == 0 )
                return;

            using ( OpenFileDialog openFileDlg = new OpenFileDialog() )
            {
                openFileDlg.AutoUpgradeEnabled = true;
                openFileDlg.CheckPathExists = true;
                openFileDlg.CheckFileExists = true;
                openFileDlg.Filter = ModuleFilterGenerator.GenerateFilter( new[] { FormatModuleUsageFlags.Import, FormatModuleUsageFlags.ImportForEditing }, mAddHandlers.Keys.ToArray() );
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

                InitializeView( true );
            }

        }

        public void Add( string filepath )
        {
            if ( mAddHandlers.Count == 0 )
                return;

            var type = GetTypeFromPath( filepath, mAddHandlers.Keys );
            var addAction = mAddHandlers[type];

            Trace.TraceInformation( $"{nameof( TreeNodeViewModel )} [{Text}]: {nameof( Add )} {type} from {filepath}" );

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
        public void Delete()
        {
            if ( Parent != null )
                Parent.HasPendingChanges = true;
            base.Remove();
        }

        public new void Remove() => Delete();

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

            // ReSharper disable once ExplicitCallerInfoArgument
            NotifyPropertyChanged( propertyName );
        }

        protected void SetModelProperty<T>( T value, [CallerMemberName] string propertyName = null )
        {
            // ReSharper disable once ExplicitCallerInfoArgument
            SetProperty( Model, value, propertyName );
        }

        protected void SetModelProperty<T>( ref T property )
        {

        }

        protected void SetProperty<T>( T value, [CallerMemberName] string propertyName = null )
        {
            SetProperty( this, value, propertyName );
        }

        protected T GetModelProperty<T>( [CallerMemberName] string propertyName = null )
        {
            if ( propertyName == null )
                throw new ArgumentNullException( nameof( propertyName ) );

            var prop = ModelType
                                .GetProperties(  )
                                .Where( x => x.Name == propertyName )
                                .FirstOrDefault(x => x.PropertyType == typeof( T ));

            if ( prop == null )
            {
                var field = ModelType
                                    .GetFields()
                                    .Where( x => x.Name == propertyName )
                                    .FirstOrDefault( x => x.FieldType == typeof( T ) );

                if ( field == null )
                    throw new Exception($"No field or property with name '{propertyName}' found in Model type '{ModelType}'" );

                return ( T )field.GetValue( Model );
            }

            return ( T )prop.GetValue( Model );
        }

        protected void NotifyPropertyChanged( [CallerMemberName]string propertyName = null )
        {
            // dont bother if instance is not initialized
            if ( !IsInitialized )
                return;

            Trace.TraceInformation( $"{nameof( TreeNodeViewModel )} [{Text}]: {nameof( NotifyPropertyChanged )} {propertyName}" );

            // set dirty flag as a property was changed
            HasPendingChanges = true;

            // invoke property changed event
            PropertyChanged?.Invoke( this, new PropertyChangedEventArgs( propertyName ) );
            TreeView?.InvokeUserPropertyChanged( this, propertyName );
        }

        protected void NotifyModelPropertyChanged()
        {
            // dont bother if instance is not initialized
            if ( !IsInitialized )
                return;

            Trace.TraceInformation( $"{nameof( TreeNodeViewModel )} [{Text}]: {nameof( NotifyModelPropertyChanged )}" );

            // set dirty flag to false because the resource is already in sync with the view model
            HasPendingChanges = false;

            // invoke property changed event
            PropertyChanged?.Invoke( this, new PropertyChangedEventArgs( nameof( Model ) ) );
            TreeView?.InvokeUserPropertyChanged( this, nameof(Model) );
        }

        // 
        // Action register actions
        //
        protected void RegisterExportHandler<T>( TreeNodeAdapterExportHandler handler )
        {
            mExportHandlers[typeof( T )] = handler;
        }

        protected void RegisterReplaceHandler<T>( TreeNodeAdapterReplaceHandler handler )
        {
            mReplaceHandlers[typeof( T )] = handler;
        }

        protected void RegisterAddHandler<T>( TreeNodeAdapterAddHandler handler )
        {
            mAddHandlers[typeof( T )] = handler;
        }

        protected void RegisterCustomHandler( string text, Action action, Keys shortcutKeys = Keys.None )
        {
            mCustomHandlers.Add( new ToolStripMenuItem( text, null, CreateEventHandler( action ), shortcutKeys ) );
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

            bool textWasEdited = !e.CancelEdit && e.Label != null && e.Label != Text;
            if ( textWasEdited )
                Text = e.Label;

            // unsubscribe from the event
            if ( TreeView != null )
                TreeView.AfterLabelEdit -= EndRename;

            if ( TextChanged != null && textWasEdited )
                TextChanged.Invoke( this, e );
        }

        //
        // Private initialize methods
        //

        /// <summary>
        /// Called by <see cref="TreeNodeViewModelFactory"/>.
        /// </summary>
        protected internal void Initialize()
        {
            // check for double initialization
            if ( IsInitialized )
                throw new Exception( $"{nameof( Initialize )} was called twice" );

            // initialize the derived view model
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

        protected abstract void UpdateModel();

        /// <summary>
        /// Populates the view -- which is the current node's child nodes and/or any other properties
        /// </summary>
        protected internal void InitializeView( bool force = false )
        {
            if ( mIsInitializingView || ( !force && !HasPendingChanges && IsViewInitialized ) )
                return;

            mIsInitializingView = true;

            Trace.TraceInformation( $"{nameof( TreeNodeViewModel )} [{Text}]: {nameof( InitializeView )}" );

            // rebuild for initializing view, if necessary
            if ( HasPendingChanges )
                UpdateModel();

            if ( NodeFlags.HasFlag( TreeNodeViewModelFlags.Branch ) )
            {
                // clear nodes, will get rid of the dummy node added by the hack required for late view initialization
                Nodes.Clear();

                // let the derived view model populate the view
                InitializeViewCore();

                if ( TreeView != null )
                {
                    // expand node to refresh view of child nodes, required for late view initialization
                    // if this is missing the expand button won't show up until the child node is selected
                    TreeView.ExpandNode( this );
                }
            }

            if ( !IsViewInitialized )
            {
                // initialize the context menu strip if the view model isn't initialized yet
                InitializeContextMenuStrip();
            }

            mIsInitializingView = false;
            IsViewInitialized = true;
        }

        /// <summary>
        /// Populates the context menu strip, should be called after the context menu options have been set.
        /// </summary>
        private void InitializeContextMenuStrip()
        {
            ContextMenuStrip = new ContextMenuStrip();

            if ( mCustomHandlers.Count > 0 )
            {
                foreach ( var menuItem in mCustomHandlers )
                    ContextMenuStrip.Items.Add( menuItem );

                ContextMenuStrip.Items.Add( new ToolStripSeparator() );
            }

            if ( ContextMenuFlags.HasFlag( TreeNodeViewModelMenuFlags.Export ) )
            {
                ContextMenuStrip.Items.Add( new ToolStripMenuItem( "&Export", null, CreateEventHandler( () => Export() ), Keys.Control | Keys.E ) );
            }

            if ( ContextMenuFlags.HasFlag( TreeNodeViewModelMenuFlags.Replace ) )
            {
                ContextMenuStrip.Items.Add( new ToolStripMenuItem( "&Replace", null, CreateEventHandler( Replace ), Keys.Control | Keys.R ) );
                if ( !ContextMenuFlags.HasFlag( TreeNodeViewModelMenuFlags.Add ) )
                    ContextMenuStrip.Items.Add( new ToolStripSeparator() );
            }

            if ( ContextMenuFlags.HasFlag( TreeNodeViewModelMenuFlags.Add ) )
            {
                ContextMenuStrip.Items.Add( new ToolStripMenuItem( "&Add", null, CreateEventHandler( Add ), Keys.Control | Keys.A ) );
                if ( ContextMenuFlags.HasFlag( TreeNodeViewModelMenuFlags.Move ) )
                    ContextMenuStrip.Items.Add( new ToolStripSeparator() );
            }

            if ( ContextMenuFlags.HasFlag( TreeNodeViewModelMenuFlags.Move ) )
            {
                ContextMenuStrip.Items.Add( new ToolStripMenuItem( "Move &Up", null, CreateEventHandler( MoveUp ), Keys.Control | Keys.Up ) );
                ContextMenuStrip.Items.Add( new ToolStripMenuItem( "Move &Down", null, CreateEventHandler( MoveDown ), Keys.Control | Keys.Down ) );
            }

            if ( ContextMenuFlags.HasFlag( TreeNodeViewModelMenuFlags.Rename ) )
            {
                ContextMenuStrip.Items.Add( new ToolStripMenuItem( "Re&name", null, CreateEventHandler( BeginRename ), Keys.Control | Keys.N ) );
            }

            if ( ContextMenuFlags.HasFlag( TreeNodeViewModelMenuFlags.Delete ) )
            {
                ContextMenuStrip.Items.Add( new ToolStripMenuItem( "&Delete", null, CreateEventHandler( Delete ), Keys.Control | Keys.Delete ) );
            }
        }

        //
        // Helpers
        //
        private static Type GetTypeFromPath( string filePath, IEnumerable<Type> types )
        {
            var extension = Path.GetExtension( filePath );
            if ( string.IsNullOrEmpty( extension ) )
                extension = string.Empty;
            else
                extension = extension.Substring( 1 );

            bool isBaseType = false;

            var modulesWithType = FormatModuleRegistry.Modules.Where( x => types.Contains( x.ModelType ) );
            if ( !modulesWithType.Any() )
            {
                modulesWithType = FormatModuleRegistry.Modules.Where( x => types.Any( y => x.ModelType.IsSubclassOf( y ) ) );
                isBaseType = true;
            }

            List<IFormatModule> modules;
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
            {
                throw new Exception( "No suitable modules for format found." );
            }

            if ( modules.Count != 1 )
                throw new Exception( "Ambigious module match. Multiple suitable modules format found." );

            if ( !isBaseType )
                return modules[0].ModelType;
            else
                return modules[0].ModelType.BaseType;
        }

        private EventHandler CreateEventHandler( Action action )
        {
            return ( s, e ) => action();
        }
    }

    [Flags]
    public enum TreeNodeViewModelFlags
    {
        Leaf = 0b0001,
        Branch = 0b0010,
    }

    [Flags]
    public enum TreeNodeViewModelMenuFlags
    {
        Export = 0b00000001,
        Replace = 0b00000010,
        Add = 0b00000100,
        Move = 0b00001000,
        Rename = 0b00010000,
        Delete = 0b00100000,
    }
}
