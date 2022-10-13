using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using GFDLibrary;
using GFDStudio.DataManagement;
using GFDStudio.FormatModules;

namespace GFDStudio.GUI.DataViewNodes
{

    /// <summary>
    /// Exports the resource held by an view model to the specified file.
    /// </summary>
    /// <param name="filepath">Path to the file to export to.</param>
    public delegate void DataViewNodeExportHandler( string filepath );

    /// <summary>
    /// Replaces the resource in the view model with a resource loaded from a file.
    /// </summary>
    /// <param name="filepath">Path to the resource file to load.</param>
    public delegate object DataViewNodeReplaceHandler( string filepath );

    /// <summary>
    /// Loads a resource and adds the corresponding view model to the tree node.
    /// </summary>
    /// <param name="filepath">Path to the resource file to load.</param>
    public delegate void DataViewNodeAddHandler( string filepath );

    /// <summary>
    /// Represents an view model for a node of a DataTreeView with extra additions.
    /// </summary>
    public abstract partial class DataViewNode : TreeNode, INotifyPropertyChanged
    {
        private readonly Dictionary<Type, DataViewNodeExportHandler> mExportHandlers;
        private readonly Dictionary<Type, DataViewNodeReplaceHandler> mReplaceHandlers;
        private readonly Dictionary<Type, DataViewNodeAddHandler> mAddHandlers;

        internal static ImageList ImageList { get; } = new ImageList();

        /// <summary>
        /// Gets or sets the resource held by the view model.
        /// </summary>
        [Browsable( false )]
        public object Data { get; protected set; }

        /// <summary>
        /// Gets the resource type.
        /// </summary>
        [Browsable( false )]
        public abstract Type DataType { get; }

        /// <summary>
        /// Gets the context menu flags.
        /// </summary>
        [Browsable( false )]
        public abstract DataViewNodeMenuFlags ContextMenuFlags { get; }

        /// <summary>
        /// Gets the node flags.
        /// </summary>
        [Browsable( false )]
        public abstract DataViewNodeFlags NodeFlags { get; }

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
        public new DataViewNode Parent => ( DataViewNode )base.Parent;

        /// <summary>
        /// Gets the parent tree view of the tree node.
        /// </summary>
        [Browsable( false )]
        public DataTreeView DataTreeView => ( DataTreeView )base.TreeView;

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
        private readonly List<(string Menu, ToolStripMenuItem Item)> mCustomHandlers;

        /// <summary>
        /// Gets or sets if the tree node is dirty and its resource needs to be rebuilt.
        /// </summary>
        internal bool HasPendingChanges
        {
            get => mHasPendingChanges;
            set
            {
                Trace.TraceInformation( $"{nameof( DataViewNode )} [{Text}]: {nameof( HasPendingChanges )} => {value}" );

                mHasPendingChanges = value;

                if ( Parent != null )
                {
                    Parent.HasPendingChanges = true;
                }
            }
        }

        protected DataViewNode( string text ) : base( text )
        {      
            mExportHandlers = new Dictionary<Type, DataViewNodeExportHandler>();
            mReplaceHandlers = new Dictionary<Type, DataViewNodeReplaceHandler>();
            mAddHandlers = new Dictionary<Type, DataViewNodeAddHandler>();
            mCustomHandlers = new List<(string Menu, ToolStripMenuItem Item)>();
        }

        public void AddChildNode( TreeNode node )
        {
            Nodes.Add( node );

            if ( !mIsInitializingView )
            {
                HasPendingChanges = true;

                if ( node is DataViewNode dataViewNode )
                    dataViewNode.InitializeView();
            }
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
                ( var filter, var filterIndexToTypeMap ) = ModuleFilterGenerator.GenerateFilter( FormatModuleUsageFlags.Export, mExportHandlers.Keys.ToArray() );

                dialog.AutoUpgradeEnabled = true;
                dialog.CheckPathExists = true;
                dialog.FileName = Text;
                dialog.Filter = filter;
                dialog.OverwritePrompt = true;
                dialog.Title = "Select a file to export to.";
                dialog.ValidateNames = true;
                dialog.AddExtension = true;

                if ( dialog.ShowDialog() != DialogResult.OK )
                {
                    return null;
                }

                Export( dialog.FileName, filterIndexToTypeMap[ dialog.FilterIndex ] );
                return dialog.FileName;
            }
        }

        public void Export( string filePath )
        {
            Export( filePath, null );
        }

        public void Export( string filePath, Type selectedType )
        {
            if ( mExportHandlers.Count == 0 )
                return;

            var type         = GetTypeFromPath( filePath, mExportHandlers.Keys, selectedType );
            var exportAction = mExportHandlers[type];

            Trace.TraceInformation( $"{nameof( DataViewNode )} [{Text}]: {nameof( Export )} {type} to {filePath}" );

            exportAction( filePath );
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
                (var filter, var filterIndexToTypeMap) = ModuleFilterGenerator.GenerateFilter( new[] { FormatModuleUsageFlags.Import, FormatModuleUsageFlags.ImportForEditing },
                                                                                              mReplaceHandlers.Keys.ToArray() );

                dialog.AutoUpgradeEnabled = true;
                dialog.CheckPathExists = true;
                dialog.CheckFileExists = true;
                dialog.FileName = Text;
                dialog.Filter = filter;
                dialog.Multiselect = false;
                dialog.SupportMultiDottedExtensions = true;
                dialog.Title = "Select a replacement file.";
                dialog.ValidateNames = true;

                if ( dialog.ShowDialog() != DialogResult.OK )
                {
                    return;
                }

                ReplaceInternal( dialog.FileName, filterIndexToTypeMap[ dialog.FilterIndex ] );
            }
        }

        public void Replace( string filepath )
        {
            ReplaceInternal( filepath, null );
        }

        private void ReplaceInternal( string filepath, Type selectedType )
        {
            if ( mReplaceHandlers.Count == 0 )
                return;

            var type          = GetTypeFromPath( filepath, mReplaceHandlers.Keys, selectedType );
            var replaceAction = mReplaceHandlers[type];

            Trace.TraceInformation( $"{nameof( DataViewNode )} [{Text}]: {nameof( ReplaceInternal )} {type} from {filepath}" );

            ReplaceProcessing( type, replaceAction( filepath ) );
        }

        public void ReplaceProcessing ( Type type, object replacement )
        {
            if ( type == typeof( GFDLibrary.Materials.Material ) )
            {
                ToolStripMenuItem item = Forms.MainForm.Instance.retainTexNameToolStripMenuItem;

                if ( item.Checked )
                {
                    GFDLibrary.Materials.Material OriginalMat = (GFDLibrary.Materials.Material)Data;
                    GFDLibrary.Materials.Material ReplacementMat = (GFDLibrary.Materials.Material)replacement;

                    // Retain original mat's texture names
                    if ( OriginalMat.DiffuseMap != null && ReplacementMat.DiffuseMap != null ) ReplacementMat.DiffuseMap.Name = OriginalMat.DiffuseMap.Name;
                    if ( OriginalMat.NormalMap != null && ReplacementMat.NormalMap != null ) ReplacementMat.NormalMap.Name = OriginalMat.NormalMap.Name;
                    if ( OriginalMat.SpecularMap != null && ReplacementMat.SpecularMap != null ) ReplacementMat.SpecularMap.Name = OriginalMat.SpecularMap.Name;
                    if ( OriginalMat.ReflectionMap != null && ReplacementMat.ReflectionMap != null ) ReplacementMat.ReflectionMap.Name = OriginalMat.ReflectionMap.Name;
                    if ( OriginalMat.HighlightMap != null && ReplacementMat.HighlightMap != null ) ReplacementMat.HighlightMap.Name = OriginalMat.HighlightMap.Name;
                    if ( OriginalMat.GlowMap != null && ReplacementMat.GlowMap != null ) ReplacementMat.GlowMap.Name = OriginalMat.GlowMap.Name;
                    if ( OriginalMat.NightMap != null && ReplacementMat.NightMap != null ) ReplacementMat.NightMap.Name = OriginalMat.NightMap.Name;
                    if ( OriginalMat.DetailMap != null && ReplacementMat.DetailMap != null ) ReplacementMat.DetailMap.Name = OriginalMat.DetailMap.Name;
                    if ( OriginalMat.ShadowMap != null && ReplacementMat.ShadowMap != null ) ReplacementMat.ShadowMap.Name = OriginalMat.ShadowMap.Name;

                    Replace( ReplacementMat );
                }
                else
                {
                    Replace( replacement );
                }
            }
            else
            {
                Replace( replacement );
            }
        }

        public void Replace( object model )
        {
            Data = model;
            NotifyDataPropertyChanged();
            InitializeView( true );
            DataTreeView.RefreshSelection();
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
                (var filter, var filterIndexToTypeMap) = ModuleFilterGenerator.GenerateFilter( new[] { FormatModuleUsageFlags.Import, FormatModuleUsageFlags.ImportForEditing }, mAddHandlers.Keys.ToArray() );
                openFileDlg.AutoUpgradeEnabled = true;
                openFileDlg.CheckPathExists = true;
                openFileDlg.CheckFileExists = true;
                openFileDlg.Filter = filter;
                openFileDlg.Multiselect = true;
                openFileDlg.SupportMultiDottedExtensions = true;
                openFileDlg.Title = "Select file(s) to add.";
                openFileDlg.ValidateNames = true;

                if ( openFileDlg.ShowDialog() != DialogResult.OK )
                {
                    return;
                }

                var selectedType = filterIndexToTypeMap[ openFileDlg.FilterIndex ];
                foreach ( string fileName in openFileDlg.FileNames )
                    AddInternal( fileName, selectedType );

                InitializeView( true );
            }

        }

        public void Add( string filepath )
        {
            AddInternal( filepath, null );
            InitializeView( true );
        }

        private void AddInternal( string filepath, Type selectedType )
        {
            if ( mAddHandlers.Count == 0 )
                return;

            var type      = GetTypeFromPath( filepath, mAddHandlers.Keys, selectedType );
            var addAction = mAddHandlers[type];

            Trace.TraceInformation( $"{nameof( DataViewNode )} [{Text}]: {nameof( Add )} {type} from {filepath}" );

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
            else if ( DataTreeView != null && DataTreeView.Nodes.Contains( this ) ) //root node
            {
                int index = DataTreeView.Nodes.IndexOf( this );
                if ( index > 0 )
                {
                    DataTreeView.Nodes.RemoveAt( index );
                    DataTreeView.Nodes.Insert( index - 1, this );
                }
            }

            if ( DataTreeView != null )
                DataTreeView.SelectedNode = this;
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
            else if ( DataTreeView != null && DataTreeView.Nodes.Contains( this ) ) //root node
            {
                int index = DataTreeView.Nodes.IndexOf( this );
                if ( index < DataTreeView.Nodes.Count - 1 )
                {
                    DataTreeView.Nodes.RemoveAt( index );
                    DataTreeView.Nodes.Insert( index + 1, this );
                }
            }

            if ( DataTreeView != null )
                DataTreeView.SelectedNode = this;
        }

        //
        // Expand all nodes method
        //
        public void ExpandAllNodes()
        {
            DataTreeView.ExpandAll();
            DataTreeView.Nodes[0].EnsureVisible();
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

        protected void SetDataProperty<T>( T value, [CallerMemberName] string propertyName = null )
        {
            // ReSharper disable once ExplicitCallerInfoArgument
            SetProperty( Data, value, propertyName );
        }

        protected T GetDataProperty<T>( [CallerMemberName] string propertyName = null )
        {
            if ( propertyName == null )
                throw new ArgumentNullException( nameof( propertyName ) );

            var prop = DataType
                                .GetProperties(  )
                                .Where( x => x.Name == propertyName )
                                .FirstOrDefault(x => x.PropertyType == typeof( T ));

            if ( prop == null )
            {
                var field = DataType
                                    .GetFields()
                                    .Where( x => x.Name == propertyName )
                                    .FirstOrDefault( x => x.FieldType == typeof( T ) );

                if ( field == null )
                    throw new Exception($"No field or property with name '{propertyName}' found in Data type '{DataType}'" );

                return ( T )field.GetValue( Data );
            }

            return ( T )prop.GetValue( Data );
        }

        protected void NotifyPropertyChanged( [CallerMemberName]string propertyName = null )
        {
            // dont bother if instance is not initialized
            if ( !IsInitialized )
                return;

            Trace.TraceInformation( $"{nameof( DataViewNode )} [{Text}]: {nameof( NotifyPropertyChanged )} {propertyName}" );

            // set dirty flag as a property was changed
            HasPendingChanges = true;

            // invoke property changed event
            PropertyChanged?.Invoke( this, new PropertyChangedEventArgs( propertyName ) );
            DataTreeView?.InvokeUserPropertyChanged( this, propertyName );
        }

        protected void NotifyDataPropertyChanged()
        {
            // dont bother if instance is not initialized
            if ( !IsInitialized )
                return;

            Trace.TraceInformation( $"{nameof( DataViewNode )} [{Text}]: {nameof( NotifyDataPropertyChanged )}" );

            // set dirty flag to false because the resource is already in sync with the view model
            HasPendingChanges = false;

            // invoke property changed event
            PropertyChanged?.Invoke( this, new PropertyChangedEventArgs( nameof( Data ) ) );
            DataTreeView?.InvokeUserPropertyChanged( this, nameof(Data) );
        }

        // 
        // Action register actions
        //
        protected void RegisterExportHandler<T>( DataViewNodeExportHandler handler )
        {
            mExportHandlers[typeof( T )] = handler;
        }

        protected void RegisterReplaceHandler<T>( DataViewNodeReplaceHandler handler )
        {
            mReplaceHandlers[typeof( T )] = handler;
        }

        protected void RegisterAddHandler<T>( DataViewNodeAddHandler handler )
        {
            mAddHandlers[typeof( T )] = handler;
        }

        protected void RegisterCustomHandler( string text, Action action, Keys shortcutKeys = Keys.None )
        {
            RegisterCustomHandler( null, text, action, shortcutKeys );
        }

        protected void RegisterCustomHandler( string menu, string text, Action action, Keys shortcutKeys = Keys.None )
        {
            mCustomHandlers.Add( ( menu, new ToolStripMenuItem( text, null, CreateEventHandler( action ), shortcutKeys ) ) );
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
            if ( DataTreeView != null )
            {
                // start editing the name
                DataTreeView.LabelEdit = true;
                BeginEdit();

                // subscribe to the label edit event, so we can finish 
                DataTreeView.AfterLabelEdit += EndRename;
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
            if ( DataTreeView != null )
                DataTreeView.AfterLabelEdit -= EndRename;

            if ( TextChanged != null && textWasEdited )
                TextChanged.Invoke( this, e );
        }

        //
        // Private initialize methods
        //

        /// <summary>
        /// Called by <see cref="DataViewNodeFactory"/>.
        /// </summary>
        protected internal void Initialize()
        {
            // check for double initialization
            if ( IsInitialized )
                throw new Exception( $"{nameof( Initialize )} was called twice" );

            // Register default handlers
            RegisterCustomHandler( "Export", "YAML", () =>
            {
                using ( var dialog = new SaveFileDialog() )
                {
                    dialog.AutoUpgradeEnabled = true;
                    dialog.CheckPathExists = true;
                    dialog.FileName = Text;
                    dialog.Filter = "YAML files (*.yml)|*.yml";
                    dialog.OverwritePrompt = true;
                    dialog.Title = "Select a file to export to.";
                    dialog.ValidateNames = true;
                    dialog.AddExtension = true;

                    if ( dialog.ShowDialog() != DialogResult.OK )
                    {
                        return;
                    }

                    YamlSerializer.SaveYamlFile( Data, dialog.FileName );
                }
            } );
            RegisterCustomHandler( "Replace", "YAML", () =>
            {
                using ( var dialog = new OpenFileDialog() )
                {
                    dialog.AutoUpgradeEnabled = true;
                    dialog.CheckPathExists = true;
                    dialog.FileName = Text;
                    dialog.Filter = "YAML files (*.yml)|*.yml";
                    dialog.Title = "Select a file to export to.";
                    dialog.ValidateNames = true;
                    dialog.AddExtension = true;

                    if ( dialog.ShowDialog() != DialogResult.OK )
                    {
                        return;
                    }

                    var data = YamlSerializer.LoadYamlFile( dialog.FileName, DataType );
                    ReplaceProcessing( DataType, data );
                }
            } );

            // initialize the derived view model
            InitializeCore();

            // Set the icon
            SetIcon();

            // set initialization flag
            IsInitialized = true;

            // subscribe to the PropertyChanged event /after/ init
            PropertyChanged += OnPropertyChanged;
        }


        private void SetIcon()
        {
            if ( string.IsNullOrWhiteSpace( ImageKey ) )
            {
                if ( NodeFlags.HasFlag( DataViewNodeFlags.Branch ) )
                {
                    ImageKey = DataStore.GetPath( "icons/folder.png" );
                }
                else
                {
                    ImageKey = DataStore.GetPath( "icons/node.png" );
                }
            }

            if ( !ImageList.Images.ContainsKey( ImageKey ) )
                ImageList.Images.Add( ImageKey, new Bitmap( ImageKey ) );

            ImageKey = SelectedImageKey = ImageKey;
        }

        protected virtual void OnPropertyChanged( object sender, PropertyChangedEventArgs e )
        {
            IsViewInitialized = false;
        }

        protected abstract void UpdateData();

        /// <summary>
        /// Populates the view -- which is the current node's child nodes and/or any other properties
        /// </summary>
        protected internal void InitializeView( bool force = false )
        {
            if ( mIsInitializingView || ( !force && !HasPendingChanges && IsViewInitialized ) )
                return;

            mIsInitializingView = true;

            Trace.TraceInformation( $"{nameof( DataViewNode )} [{Text}]: {nameof( InitializeView )}" );

            // rebuild for initializing view, if necessary
            if ( HasPendingChanges )
                UpdateData();

            if ( NodeFlags.HasFlag( DataViewNodeFlags.Branch ) )
            {
                // clear nodes, will get rid of the dummy node added by the hack required for late view initialization
                Nodes.Clear();

                // let the derived view model populate the view
                InitializeViewCore();

                if ( DataTreeView != null )
                {
                    // expand node to refresh view of child nodes, required for late view initialization
                    // if this is missing the expand button won't show up until the child node is selected
                    DataTreeView.ExpandNode( this );
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

            var uncategorizedCustomHandlers = mCustomHandlers.Where( x => x.Menu == null ).ToList();
            if ( uncategorizedCustomHandlers.Count > 0 )
            {
                foreach ( var handler in uncategorizedCustomHandlers )
                    ContextMenuStrip.Items.Add( handler.Item );

                ContextMenuStrip.Items.Add( new ToolStripSeparator() );
            }

            if ( ContextMenuFlags.HasFlag( DataViewNodeMenuFlags.Export ) )
            {
                ContextMenuStrip.Items.Add( new ToolStripMenuItem( "&Export", null, CreateEventHandler( () => Export() ), Keys.Control | Keys.E )
                {
                    Name = "Export"
                });
            }

            if ( ContextMenuFlags.HasFlag( DataViewNodeMenuFlags.Replace ) )
            {
                ContextMenuStrip.Items.Add( new ToolStripMenuItem( "&Replace", null, CreateEventHandler( Replace ), Keys.Control | Keys.R )
                {
                    Name = "Replace"
                });

                if ( !ContextMenuFlags.HasFlag( DataViewNodeMenuFlags.Add ) )
                    ContextMenuStrip.Items.Add( new ToolStripSeparator() );
            }

            if ( ContextMenuFlags.HasFlag( DataViewNodeMenuFlags.Add ) )
            {
                ContextMenuStrip.Items.Add( new ToolStripMenuItem( "&Add", null, CreateEventHandler( Add ),
                                                                   Keys.Control | Keys.A ) { Name = "Add" } );

                if ( ContextMenuFlags.HasFlag( DataViewNodeMenuFlags.Move ) )
                    ContextMenuStrip.Items.Add( new ToolStripSeparator() );
            }

            if ( ContextMenuFlags.HasFlag( DataViewNodeMenuFlags.Move ) )
            {
                ContextMenuStrip.Items.Add( new ToolStripMenuItem( "Move &Up", null, CreateEventHandler( MoveUp ), Keys.Control | Keys.Up )
                {
                    Name = "Move Up"
                });
                ContextMenuStrip.Items.Add( new ToolStripMenuItem( "Move &Down", null, CreateEventHandler( MoveDown ), Keys.Control | Keys.Down )
                {
                    Name = "Move Down"
                });
            }

            if ( ContextMenuFlags.HasFlag( DataViewNodeMenuFlags.Rename ) )
            {
                ContextMenuStrip.Items.Add( new ToolStripMenuItem( "Re&name", null, CreateEventHandler( BeginRename ), Keys.Control | Keys.N )
                {
                    Name = "Rename"
                });
            }

            if (ContextMenuFlags.HasFlag(DataViewNodeMenuFlags.Expand))
            {
                ContextMenuStrip.Items.Add(new ToolStripMenuItem("&Expand All", null, CreateEventHandler(() => ExpandAllNodes()), Keys.Control | Keys.A)
                {
                    Name = "Expand All"
                });
            }

            foreach ( (string menuPath, ToolStripMenuItem item) in mCustomHandlers.Where( x => x.Menu != null ) )
            {
                // Add custom handlers with a menu path
                var menuParts = menuPath.Split( '/' );
                ToolStripMenuItem parentMenuItem = null;

                foreach ( string menu in menuParts )
                {
                    var menuItem = ( ToolStripMenuItem ) ( parentMenuItem == null ? ContextMenuStrip.Items.Find( menu, false ).FirstOrDefault()
                                                                                  : parentMenuItem.DropDownItems.Find( menu, false ).FirstOrDefault() );

                    var isNewMenuItem = false;
                    if ( menuItem == null )
                    {
                        isNewMenuItem = true;
                        menuItem      = new ToolStripMenuItem( menu ) { Name = menu };
                    }

                    if ( isNewMenuItem )
                    {
                        if ( parentMenuItem == null )
                            ContextMenuStrip.Items.Add( menuItem );
                        else
                            parentMenuItem.DropDownItems.Add( menuItem );
                    }

                    parentMenuItem = menuItem;
                }

                if ( parentMenuItem == null )
                    ContextMenuStrip.Items.Add( item );
                else
                    parentMenuItem.DropDownItems.Add( item );
            }

            if ( ContextMenuFlags.HasFlag( DataViewNodeMenuFlags.Delete ) )
            {
                ContextMenuStrip.Items.Add( new ToolStripMenuItem( "&Delete", null, CreateEventHandler( Delete ), Keys.Control | Keys.Delete )
                {
                    Name = "Delete"
                });
            }
        }

        //
        // Helpers
        //
        private static Type GetTypeFromPath( string filePath, IEnumerable<Type> types, Type selectedType )
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
            {
                if ( selectedType == null )
                    throw new Exception( "Ambigious module match. Multiple suitable modules format found." );

                return selectedType;
            }

            if ( !isBaseType )
                return modules[0].ModelType;
            else
                return modules[0].ModelType.BaseType;
        }

        private EventHandler CreateEventHandler( Action action )
        {
            return ( s, e ) =>
            {
                // Annoyingly, the context menu strip stays visible when a dialog is opened while a 
                // drop down menu is visible. That's why we explicitly hide it here.
                ContextMenuStrip.Visible = false;

                action();
            };
        }
    }

    [Flags]
    public enum DataViewNodeFlags
    {
        Leaf = 0b0001,
        Branch = 0b0010,
    }

    [Flags]
    public enum DataViewNodeMenuFlags
    {
        Export = 0b00000001,
        Replace = 0b00000010,
        Add = 0b00000100,
        Move = 0b00001000,
        Rename = 0b00010000,
        Delete = 0b00100000,
        Expand = 0b01000000,
    }
}
