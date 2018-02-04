using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AtlusGfdEditor.GUI.Forms;
using AtlusGfdLibrary;

namespace AtlusGfdEditor.GUI.ViewModels
{
    public class TextureMapListViewModel : TreeNodeViewModel<List<TextureMap>>
    {
        public override TreeNodeViewModelMenuFlags ContextMenuFlags
            => TreeNodeViewModelMenuFlags.Delete | TreeNodeViewModelMenuFlags.Move;

        public override TreeNodeViewModelFlags NodeFlags
        {
            get
            {
                if ( Model == null )
                    return TreeNodeViewModelFlags.Leaf;

                return Model.Count > 0 ? TreeNodeViewModelFlags.Branch : TreeNodeViewModelFlags.Leaf;
            }
        }

        [Browsable(false)]
        public TextureMapViewModel DiffuseMapViewModel { get; set; }

        [Browsable( false )]
        public TextureMapViewModel NormalMapViewModel { get; set; }

        [Browsable( false )]
        public TextureMapViewModel SpecularMapViewModel { get; set; }

        [Browsable( false )]
        public TextureMapViewModel ReflectionMapViewModel { get; set; }

        [Browsable( false )]
        public TextureMapViewModel HighlightMapViewModel { get; set; }

        [Browsable( false )]
        public TextureMapViewModel GlowMapViewModel { get; set; }

        [Browsable( false )]
        public TextureMapViewModel NightMapViewModel { get; set; }

        [Browsable( false )]
        public TextureMapViewModel DetailMapViewModel { get; set; }

        [Browsable( false )]
        public TextureMapViewModel ShadowMapViewModel { get; set; }

        public TextureMapListViewModel( string text, List< TextureMap > resource ) : base( text, resource )
        {
        }

        protected override void InitializeCore()
        {
            RegisterCustomHandler( "Add New", () =>
            {
                var dialog = new CreateTextureMapDialog();
                if ( dialog.ShowDialog() != DialogResult.OK )
                    return;

                var textureMap = new TextureMap( dialog.Result.Name );
                Model[dialog.Result.Type] = textureMap;

            }, Keys.Control | Keys.A );

            RegisterModelUpdateHandler(() =>
            {
                var list = new List< TextureMap >( 9 );
                list.Add( Nodes.Contains( DiffuseMapViewModel ) ? DiffuseMapViewModel.Model : null );
                list.Add( Nodes.Contains( NormalMapViewModel ) ? NormalMapViewModel.Model : null );
                list.Add( Nodes.Contains( SpecularMapViewModel ) ? SpecularMapViewModel.Model : null );
                list.Add( Nodes.Contains( ReflectionMapViewModel ) ? ReflectionMapViewModel.Model : null );
                list.Add( Nodes.Contains( HighlightMapViewModel ) ? HighlightMapViewModel.Model : null );
                list.Add( Nodes.Contains( GlowMapViewModel ) ? GlowMapViewModel.Model : null );
                list.Add( Nodes.Contains( NightMapViewModel ) ? NightMapViewModel.Model : null );
                list.Add( Nodes.Contains( DetailMapViewModel ) ? DetailMapViewModel.Model : null );
                list.Add( Nodes.Contains( ShadowMapViewModel ) ? ShadowMapViewModel.Model : null );
                return list;
            } );
        }

        protected override void InitializeViewCore()
        {
            if ( Model[ 0 ] != null )
            {
                DiffuseMapViewModel = ( TextureMapViewModel ) TreeNodeViewModelFactory.Create( "Diffuse Map", Model[ 0 ] );
                Nodes.Add( DiffuseMapViewModel );
            }

            if ( Model[1] != null )
            {
                NormalMapViewModel = ( TextureMapViewModel )TreeNodeViewModelFactory.Create( "Normal Map", Model[1] );
                Nodes.Add( NormalMapViewModel );
            }

            if ( Model[2] != null )
            {
                SpecularMapViewModel = ( TextureMapViewModel )TreeNodeViewModelFactory.Create( "Specular Map", Model[2] );
                Nodes.Add( SpecularMapViewModel );
            }

            if ( Model[3] != null )
            {
                ReflectionMapViewModel = ( TextureMapViewModel )TreeNodeViewModelFactory.Create( "Reflection Map", Model[3] );
                Nodes.Add( ReflectionMapViewModel );
            }

            if ( Model[4] != null )
            {
                HighlightMapViewModel = ( TextureMapViewModel )TreeNodeViewModelFactory.Create( "Highlight Map", Model[4] );
                Nodes.Add( HighlightMapViewModel );
            }

            if ( Model[5] != null )
            {
                GlowMapViewModel = ( TextureMapViewModel )TreeNodeViewModelFactory.Create( "Glow Map", Model[5] );
                Nodes.Add( GlowMapViewModel );
            }

            if ( Model[6] != null )
            {
                NightMapViewModel = ( TextureMapViewModel )TreeNodeViewModelFactory.Create( "Night Map", Model[6] );
                Nodes.Add( NightMapViewModel );
            }

            if ( Model[7] != null )
            {
                DetailMapViewModel = ( TextureMapViewModel )TreeNodeViewModelFactory.Create( "Detail Map", Model[7] );
                Nodes.Add( DetailMapViewModel );
            }

            if ( Model[8] != null )
            {
                ShadowMapViewModel = ( TextureMapViewModel )TreeNodeViewModelFactory.Create( "Shadow Map", Model[8] );
                Nodes.Add( ShadowMapViewModel );
            }
        }

        private void CreateTextureMapViewModel( TextureMap textureMap, int index )
        {
            switch ( index )
            {
                case 0:
                    DiffuseMapViewModel = ( TextureMapViewModel )TreeNodeViewModelFactory.Create( "Diffuse Map", textureMap );
                    return;
                case 1:
                    NormalMapViewModel = ( TextureMapViewModel )TreeNodeViewModelFactory.Create( "Normal Map", textureMap );
                    return;
                case 2:
                    SpecularMapViewModel = ( TextureMapViewModel )TreeNodeViewModelFactory.Create( "Specular Map", textureMap );
                    return;
                case 3:
                    ReflectionMapViewModel = ( TextureMapViewModel )TreeNodeViewModelFactory.Create( "Reflection Map", textureMap );
                    return;
                case 4:
                    HighlightMapViewModel = ( TextureMapViewModel )TreeNodeViewModelFactory.Create( "Hightlight Map", textureMap );
                    return;
                case 5:
                    GlowMapViewModel = ( TextureMapViewModel )TreeNodeViewModelFactory.Create( "Glow Map", textureMap );
                    return;
                case 6:
                    NightMapViewModel = ( TextureMapViewModel )TreeNodeViewModelFactory.Create( "Night Map", textureMap );
                    return;
                case 7:
                    DetailMapViewModel = ( TextureMapViewModel )TreeNodeViewModelFactory.Create( "Detail Map", textureMap );
                    return;
                case 8:
                    ShadowMapViewModel = ( TextureMapViewModel )TreeNodeViewModelFactory.Create( "Shadow Map", textureMap );
                    return;
            }
        }
    }
}
