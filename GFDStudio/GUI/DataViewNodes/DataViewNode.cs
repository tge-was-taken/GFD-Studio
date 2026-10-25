using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using GFDLibrary;
using GFDStudio.DataManagement;
using GFDStudio.FormatModules;
using GFDStudio.GUI.Forms;

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
    public abstract partial class DataViewNode : INotifyPropertyChanged
    {
        private readonly Dictionary<Type, DataViewNodeExportHandler> mExportHandlers;
        private readonly Dictionary<Type, DataViewNodeReplaceHandler> mReplaceHandlers;
        private readonly Dictionary<Type, DataViewNodeAddHandler> mAddHandlers;

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
        public string Text
        {
            get => mText;
            set
            {
                if ( value != mText )
                {
                    mText = value;
                    NotifyPropertyChanged();
                }
            }
        }

        /// <summary>
        /// An identifier for the node, which is not what gets displayed.
        /// </summary>
        [Browsable( false )]
        public string Name { get; set; }

        /// <summary>
        /// Paths to the icons to show, resolved by the view.
        /// </summary>
        [Browsable( false )]
        public string ImageKey { get; set; }

        [Browsable( false )]
        public string SelectedImageKey { get; set; }

        [Browsable( false )]
        public bool IsExpanded { get; internal set; }

        /// <summary>
        /// Gets the parent of the node.
        /// </summary>
        [Browsable( false )]
        public DataViewNode Parent { get; private set; }

        /// <summary>
        /// Gets the child nodes.
        /// </summary>
        [Browsable( false )]
        public DataViewNodeCollection Nodes { get; }

        /// <summary>
        /// The position among the parent's children.
        /// </summary>
        [Browsable( false )]
        public int Index => Parent?.Nodes.IndexOf( this ) ?? 0;

        /// <summary>
        /// The view displaying this node, which the root of the tree holds on everyone's behalf.
        /// </summary>
        [Browsable( false )]
        public IDataViewNodeView View
        {
            get => Parent != null ? Parent.View : mView;
            private set => mView = value;
        }

        /// <summary>
        /// The node's context menu, described without reference to a UI framework.
        /// </summary>
        [Browsable( false )]
        public List<DataViewNodeMenuItem> MenuItems { get; private set; } = new();

        /// <summary>
        /// This event is fired whenever the tree node label text is changed.
        /// </summary>
        [Browsable( false )]
        public event EventHandler TextChanged;

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
        private string mText;
        private IDataViewNodeView mView;
        private readonly List<(string Menu, DataViewNodeMenuItem Item)> mCustomHandlers;

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

        protected DataViewNode( string text )
        {
            mText = text;
            Nodes = new DataViewNodeCollection( this );
            mExportHandlers = new Dictionary<Type, DataViewNodeExportHandler>();
            mReplaceHandlers = new Dictionary<Type, DataViewNodeReplaceHandler>();
            mAddHandlers = new Dictionary<Type, DataViewNodeAddHandler>();
            mCustomHandlers = new List<(string Menu, DataViewNodeMenuItem Item)>();
        }

        internal void SetParent( DataViewNode parent ) => Parent = parent;

        internal void AttachView( IDataViewNodeView view ) => View = view;

        internal void NotifyChildrenChanged() => View?.OnChildrenChanged( this );

        public void Expand()
        {
            IsExpanded = true;
            View?.OnChildrenChanged( this );
        }

        public void AddChildNode( DataViewNode node )
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

            ( var filter, var filterIndexToTypeMap ) = ModuleFilterGenerator.GenerateFilter( FormatModuleUsageFlags.Export, mExportHandlers.Keys.ToArray() );

            var result = DataViewNodeDialogs.Current.SaveFile( new FileDialogRequest
            {
                Title = "Select a file to export to.",
                Filter = filter,
                FileName = Text,
            } );

            if ( result == null )
                return null;

            Export( result.FileName, filterIndexToTypeMap[ result.FilterIndex ] );
            return result.FileName;
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

            (var filter, var filterIndexToTypeMap) = ModuleFilterGenerator.GenerateFilter( new[] { FormatModuleUsageFlags.Import, FormatModuleUsageFlags.ImportForEditing },
                                                                                          mReplaceHandlers.Keys.ToArray() );

            var result = DataViewNodeDialogs.Current.OpenFile( new FileDialogRequest
            {
                Title = "Select a replacement file.",
                Filter = filter,
                FileName = Text,
            } );

            if ( result == null )
                return;

            ReplaceInternal( result.FileName, filterIndexToTypeMap[ result.FilterIndex ] );
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

        public void ReplaceProcessing( Type type, object replacement )
        {
            if ( type == typeof( GFDLibrary.Materials.Material ) )
            {
                GFDLibrary.Materials.Material OriginalMat = (GFDLibrary.Materials.Material)Data;
                GFDLibrary.Materials.Material ReplacementMat = (GFDLibrary.Materials.Material)replacement;

                if ( DataViewNodeOptions.Current.RetainMaterialTextureNames )
                {
                    // Retain original mat's texture names
                    ReplacementMat.Name = OriginalMat.Name;
                    if ( OriginalMat.DiffuseMap != null && ReplacementMat.DiffuseMap != null ) ReplacementMat.DiffuseMap.Name = OriginalMat.DiffuseMap.Name;
                    if ( OriginalMat.NormalMap != null && ReplacementMat.NormalMap != null ) ReplacementMat.NormalMap.Name = OriginalMat.NormalMap.Name;
                    if ( OriginalMat.SpecularMap != null && ReplacementMat.SpecularMap != null ) ReplacementMat.SpecularMap.Name = OriginalMat.SpecularMap.Name;
                    if ( OriginalMat.ReflectionMap != null && ReplacementMat.ReflectionMap != null ) ReplacementMat.ReflectionMap.Name = OriginalMat.ReflectionMap.Name;
                    if ( OriginalMat.HighlightMap != null && ReplacementMat.HighlightMap != null ) ReplacementMat.HighlightMap.Name = OriginalMat.HighlightMap.Name;
                    if ( OriginalMat.GlowMap != null && ReplacementMat.GlowMap != null ) ReplacementMat.GlowMap.Name = OriginalMat.GlowMap.Name;
                    if ( OriginalMat.NightMap != null && ReplacementMat.NightMap != null ) ReplacementMat.NightMap.Name = OriginalMat.NightMap.Name;
                    if ( OriginalMat.DetailMap != null && ReplacementMat.DetailMap != null ) ReplacementMat.DetailMap.Name = OriginalMat.DetailMap.Name;
                    if ( OriginalMat.ShadowMap != null && ReplacementMat.ShadowMap != null ) ReplacementMat.ShadowMap.Name = OriginalMat.ShadowMap.Name;
                }

                if ( DataViewNodeOptions.Current.RetainMaterialColorValues )
                {
                    // TODO: Fix for Metaphor!
                    ReplacementMat.LegacyParameters.AmbientColor = OriginalMat.LegacyParameters.AmbientColor;
                    ReplacementMat.LegacyParameters.DiffuseColor = OriginalMat.LegacyParameters.DiffuseColor;
                    ReplacementMat.LegacyParameters.EmissiveColor = OriginalMat.LegacyParameters.EmissiveColor;
                    ReplacementMat.LegacyParameters.SpecularColor = OriginalMat.LegacyParameters.SpecularColor;
                }

                replacement = ReplacementMat;
            }

            Replace( replacement );
        }

        public void Replace( object model )
        {
            Data = model;
            NotifyDataPropertyChanged();
            InitializeView( true );
            View?.RefreshSelection();
        }

        //
        // Add methods
        //
        public void Add()
        {
            if ( mAddHandlers.Count == 0 )
                return;

            (var filter, var filterIndexToTypeMap) = ModuleFilterGenerator.GenerateFilter( new[] { FormatModuleUsageFlags.Import, FormatModuleUsageFlags.ImportForEditing }, mAddHandlers.Keys.ToArray() );

            var result = DataViewNodeDialogs.Current.OpenFile( new FileDialogRequest
            {
                Title = "Select file(s) to add.",
                Filter = filter,
                Multiselect = true,
            } );

            if ( result == null )
                return;

            var selectedType = filterIndexToTypeMap[ result.FilterIndex ];
            foreach ( string fileName in result.FileNames )
                AddInternal( fileName, selectedType );

            InitializeView( true );
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
            var parent = Parent;

            if ( parent != null )
            {
                int index = parent.Nodes.IndexOf( this );
                if ( index > 0 )
                {
                    parent.Nodes.RemoveAt( index );
                    parent.Nodes.Insert( index - 1, this );
                }
            }

            View?.Select( this );
        }

        public void MoveDown()
        {
            var parent = Parent;
            if ( parent != null )
            {
                int index = parent.Nodes.IndexOf( this );
                if ( index < parent.Nodes.Count - 1 )
                {
                    parent.Nodes.RemoveAt( index );
                    parent.Nodes.Insert( index + 1, this );
                }
            }

            View?.Select( this );
        }

        //
        // Expand all nodes method
        //
        public void ExpandAllNodes()
        {
            View?.ExpandAll();
            View?.EnsureVisible( this );
        }

        //
        // Delete method
        //
        public void Delete()
        {
            if ( Parent != null )
                Parent.HasPendingChanges = true;

            Parent?.Nodes.Remove( this );
        }

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
            View?.NotifyUserPropertyChanged( this, propertyName );
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
            View?.NotifyUserPropertyChanged( this, nameof( Data ) );
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

        protected void RegisterCustomHandler( string text, Action action, DataViewNodeMenuShortcut shortcut = default )
        {
            RegisterCustomHandler( null, text, action, shortcut );
        }

        protected void RegisterCustomHandler( string menu, string text, Action action, DataViewNodeMenuShortcut shortcut = default )
        {
            mCustomHandlers.Add( ( menu, new DataViewNodeMenuItem { Name = text, Text = text, Action = action, Shortcut = shortcut } ) );
        }

        //
        // Initialization actions
        //
        protected abstract void InitializeCore();

        protected virtual void InitializeViewCore() { }

        // Rename methods
        private void BeginRename() => View?.BeginRename( this );

        /// <summary>
        /// Applies a label the user edited. A null label means they cancelled.
        /// </summary>
        public void CommitRename( string newText )
        {
            if ( newText == null || newText == Text )
                return;

            Text = newText;
            TextChanged?.Invoke( this, EventArgs.Empty );
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
                var result = DataViewNodeDialogs.Current.SaveFile( new FileDialogRequest
                {
                    Title = "Select a file to export to.",
                    Filter = "YAML files (*.yml)|*.yml",
                    FileName = Text,
                } );

                if ( result != null )
                    YamlSerializer.SaveYamlFile( Data, result.FileName );
            } );
            RegisterCustomHandler( "Replace", "YAML", () =>
            {
                var result = DataViewNodeDialogs.Current.OpenFile( new FileDialogRequest
                {
                    Title = "Select a file to replace from.",
                    Filter = "YAML files (*.yml)|*.yml",
                    FileName = Text,
                } );

                if ( result == null )
                    return;

                var data = YamlSerializer.LoadYamlFile( result.FileName, DataType );
                ReplaceProcessing( DataType, data );
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

            SelectedImageKey = ImageKey;
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

                // let the view catch up, so the expander shows before a child is selected
                View?.OnChildrenChanged( this );
            }

            if ( !IsViewInitialized )
            {
                // initialize the context menu strip if the view model isn't initialized yet
                MenuItems = BuildContextMenu();
            }

            mIsInitializingView = false;
            IsViewInitialized = true;
        }

        /// <summary>
        /// Populates the context menu strip, should be called after the context menu options have been set.
        /// </summary>
        /// <summary>
        /// Describes the node's context menu without building any UI.
        /// </summary>
        protected internal List<DataViewNodeMenuItem> BuildContextMenu()
        {
            var items = new List<DataViewNodeMenuItem>();

            var uncategorized = mCustomHandlers.Where( x => x.Menu == null ).ToList();
            if ( uncategorized.Count > 0 )
            {
                foreach ( var handler in uncategorized )
                    items.Add( handler.Item );

                items.Add( DataViewNodeMenuItem.Separator );
            }

            if ( ContextMenuFlags.HasFlag( DataViewNodeMenuFlags.Export ) )
                items.Add( Item( "Export", "&Export", () => Export(), DataViewNodeMenuKey.E ) );

            if ( ContextMenuFlags.HasFlag( DataViewNodeMenuFlags.Replace ) )
            {
                items.Add( Item( "Replace", "&Replace", Replace, DataViewNodeMenuKey.R ) );

                if ( !ContextMenuFlags.HasFlag( DataViewNodeMenuFlags.Add ) )
                    items.Add( DataViewNodeMenuItem.Separator );
            }

            if ( ContextMenuFlags.HasFlag( DataViewNodeMenuFlags.Add ) )
            {
                items.Add( Item( "Add", "&Add", Add, DataViewNodeMenuKey.A ) );

                if ( ContextMenuFlags.HasFlag( DataViewNodeMenuFlags.Move ) )
                    items.Add( DataViewNodeMenuItem.Separator );
            }

            if ( ContextMenuFlags.HasFlag( DataViewNodeMenuFlags.Move ) )
            {
                items.Add( Item( "Move Up", "Move &Up", MoveUp, DataViewNodeMenuKey.Up ) );
                items.Add( Item( "Move Down", "Move &Down", MoveDown, DataViewNodeMenuKey.Down ) );
            }

            if ( ContextMenuFlags.HasFlag( DataViewNodeMenuFlags.Rename ) )
                items.Add( Item( "Rename", "Re&name", BeginRename, DataViewNodeMenuKey.N ) );

            if ( ContextMenuFlags.HasFlag( DataViewNodeMenuFlags.Expand ) )
                items.Add( Item( "Expand All", "&Expand All", () => ExpandAllNodes(), DataViewNodeMenuKey.A ) );

            foreach ( var (menuPath, item) in mCustomHandlers.Where( x => x.Menu != null ) )
            {
                DataViewNodeMenuItem parent = null;

                foreach ( var menu in menuPath.Split( '/' ) )
                {
                    var existing = parent == null ? items.Find( x => x.Name == menu ) : parent.FindChild( menu );
                    if ( existing == null )
                    {
                        existing = new DataViewNodeMenuItem { Name = menu, Text = menu };
                        if ( parent == null )
                            items.Add( existing );
                        else
                            parent.Items.Add( existing );
                    }

                    parent = existing;
                }

                if ( parent == null )
                    items.Add( item );
                else
                    parent.Items.Add( item );
            }

            if ( ContextMenuFlags.HasFlag( DataViewNodeMenuFlags.Delete ) )
                items.Add( Item( "Delete", "&Delete", Delete, DataViewNodeMenuKey.Delete ) );

            return items;

            static DataViewNodeMenuItem Item( string name, string text, Action action, DataViewNodeMenuKey key )
                => new() { Name = name, Text = text, Action = action, Shortcut = DataViewNodeMenuShortcut.Control( key ) };
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
        Convert = 0b10000000
    }
}
