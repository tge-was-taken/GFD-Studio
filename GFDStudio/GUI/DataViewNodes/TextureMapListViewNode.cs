using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using GFDLibrary.Models;
using GFDStudio.GUI.Forms;

namespace GFDStudio.GUI.DataViewNodes
{
    public class TextureMapListViewNode : DataViewNode<List<TextureMap>>
    {
        public override DataViewNodeMenuFlags ContextMenuFlags
            => DataViewNodeMenuFlags.Delete | DataViewNodeMenuFlags.Move;

        public override DataViewNodeFlags NodeFlags
        {
            get
            {
                if ( Data == null )
                    return DataViewNodeFlags.Leaf;

                return Data.Count > 0 ? DataViewNodeFlags.Branch : DataViewNodeFlags.Leaf;
            }
        }

        [Browsable(false)]
        public TextureMapViewNode DiffuseMap { get; set; }

        [Browsable( false )]
        public TextureMapViewNode NormalMap { get; set; }

        [Browsable( false )]
        public TextureMapViewNode SpecularMap { get; set; }

        [Browsable( false )]
        public TextureMapViewNode ReflectionMap { get; set; }

        [Browsable( false )]
        public TextureMapViewNode HighlightMap { get; set; }

        [Browsable( false )]
        public TextureMapViewNode GlowMap { get; set; }

        [Browsable( false )]
        public TextureMapViewNode NightMap { get; set; }

        [Browsable( false )]
        public TextureMapViewNode DetailMap { get; set; }

        [Browsable( false )]
        public TextureMapViewNode ShadowMap { get; set; }
        [Browsable( false )]
        public TextureMapViewNode TextureMap10 { get; set; }

        public TextureMapListViewNode( string text, List< TextureMap > data ) : base( text, data )
        {
        }

        protected override void InitializeCore()
        {
            RegisterCustomHandler( "Add", "New texture map", () =>
            {
                var dialog = new CreateTextureMapDialog();
                if ( dialog.ShowDialog() != DialogResult.OK || dialog.Result.Name.Length == 0 || dialog.Result.Type == -1 )
                    return;

                var textureMap = new TextureMap( dialog.Result.Name );
                Data[dialog.Result.Type] = textureMap;
                InitializeView( true );

            }, Keys.Control | Keys.A );

            RegisterModelUpdateHandler(() =>
            {
                var list = new List< TextureMap >( 10 );
                list.Add( Nodes.Contains( DiffuseMap ) ? DiffuseMap.Data : null );
                list.Add( Nodes.Contains( NormalMap ) ? NormalMap.Data : null );
                list.Add( Nodes.Contains( SpecularMap ) ? SpecularMap.Data : null );
                list.Add( Nodes.Contains( ReflectionMap ) ? ReflectionMap.Data : null );
                list.Add( Nodes.Contains( HighlightMap ) ? HighlightMap.Data : null );
                list.Add( Nodes.Contains( GlowMap ) ? GlowMap.Data : null );
                list.Add( Nodes.Contains( NightMap ) ? NightMap.Data : null );
                list.Add( Nodes.Contains( DetailMap ) ? DetailMap.Data : null );
                list.Add( Nodes.Contains( ShadowMap ) ? ShadowMap.Data : null );
                list.Add( Nodes.Contains( TextureMap10 ) ? TextureMap10.Data : null );
                return list;
            } );
        }

        protected override void InitializeViewCore()
        {
            if ( Data[ 0 ] != null )
            {
                string TexName = Data[0].METAPHOR_ParentMaterialParameterSet != null ? Data[0].METAPHOR_ParentMaterialParameterSet.GetTextureMap1Name() : "Diffuse Map";
                DiffuseMap = ( TextureMapViewNode ) DataViewNodeFactory.Create( TexName, Data[ 0 ] );
                AddChildNode( DiffuseMap );
            }

            if ( Data[1] != null )
            {
                string TexName = Data[1].METAPHOR_ParentMaterialParameterSet != null ? Data[1].METAPHOR_ParentMaterialParameterSet.GetTextureMap2Name() : "Normal Map";
                NormalMap = ( TextureMapViewNode )DataViewNodeFactory.Create( TexName, Data[1] );
                AddChildNode( NormalMap );
            }

            if ( Data[2] != null )
            {
                string TexName = Data[2].METAPHOR_ParentMaterialParameterSet != null ? Data[2].METAPHOR_ParentMaterialParameterSet.GetTextureMap3Name() : "Specular Map";
                SpecularMap = ( TextureMapViewNode )DataViewNodeFactory.Create( TexName, Data[2] );
                AddChildNode( SpecularMap );
            }

            if ( Data[3] != null )
            {
                string TexName = Data[3].METAPHOR_ParentMaterialParameterSet != null ? Data[3].METAPHOR_ParentMaterialParameterSet.GetTextureMap4Name() : "Reflection Map";
                ReflectionMap = ( TextureMapViewNode )DataViewNodeFactory.Create( TexName, Data[3] );
                AddChildNode( ReflectionMap );
            }

            if ( Data[4] != null )
            {
                string TexName = Data[4].METAPHOR_ParentMaterialParameterSet != null ? Data[4].METAPHOR_ParentMaterialParameterSet.GetTextureMap5Name() : "Highlight Map";
                HighlightMap = ( TextureMapViewNode )DataViewNodeFactory.Create( TexName, Data[4] );
                AddChildNode( HighlightMap );
            }

            if ( Data[5] != null )
            {
                string TexName = Data[5].METAPHOR_ParentMaterialParameterSet != null ? Data[5].METAPHOR_ParentMaterialParameterSet.GetTextureMap6Name() : "Glow Map";
                GlowMap = ( TextureMapViewNode )DataViewNodeFactory.Create( TexName, Data[5] );
                AddChildNode( GlowMap );
            }

            if ( Data[6] != null )
            {
                string TexName = Data[6].METAPHOR_ParentMaterialParameterSet != null ? Data[6].METAPHOR_ParentMaterialParameterSet.GetTextureMap7Name() : "Night Map";
                NightMap = ( TextureMapViewNode )DataViewNodeFactory.Create( TexName, Data[6] );
                AddChildNode( NightMap );
            }

            if ( Data[7] != null )
            {
                string TexName = Data[7].METAPHOR_ParentMaterialParameterSet != null ? Data[7].METAPHOR_ParentMaterialParameterSet.GetTextureMap8Name() : "Detail Map";
                DetailMap = ( TextureMapViewNode )DataViewNodeFactory.Create( TexName, Data[7] );
                AddChildNode( DetailMap );
            }

            if ( Data[8] != null )
            {
                string TexName = Data[8].METAPHOR_ParentMaterialParameterSet != null ? Data[8].METAPHOR_ParentMaterialParameterSet.GetTextureMap9Name() : "Shadow Map";
                ShadowMap = ( TextureMapViewNode )DataViewNodeFactory.Create( TexName, Data[8] );
                AddChildNode( ShadowMap );
            }
            if ( Data[9] != null )
            {
                string TexName = Data[9].METAPHOR_ParentMaterialParameterSet != null ? Data[9].METAPHOR_ParentMaterialParameterSet.GetTextureMap10Name() : "Intensity Map";
                TextureMap10 = (TextureMapViewNode)DataViewNodeFactory.Create( TexName, Data[9] );
                AddChildNode( TextureMap10 );
            }
        }

        private void CreateTextureMapViewModel( TextureMap textureMap, int index )
        {
            switch ( index )
            {
                case 0:
                    DiffuseMap = ( TextureMapViewNode )DataViewNodeFactory.Create( "Diffuse Map", textureMap );
                    return;
                case 1:
                    NormalMap = ( TextureMapViewNode )DataViewNodeFactory.Create( "Normal Map", textureMap );
                    return;
                case 2:
                    SpecularMap = ( TextureMapViewNode )DataViewNodeFactory.Create( "Specular Map", textureMap );
                    return;
                case 3:
                    ReflectionMap = ( TextureMapViewNode )DataViewNodeFactory.Create( "Reflection Map", textureMap );
                    return;
                case 4:
                    HighlightMap = ( TextureMapViewNode )DataViewNodeFactory.Create( "Hightlight Map", textureMap );
                    return;
                case 5:
                    GlowMap = ( TextureMapViewNode )DataViewNodeFactory.Create( "Glow Map", textureMap );
                    return;
                case 6:
                    NightMap = ( TextureMapViewNode )DataViewNodeFactory.Create( "Night Map", textureMap );
                    return;
                case 7:
                    DetailMap = ( TextureMapViewNode )DataViewNodeFactory.Create( "Detail Map", textureMap );
                    return;
                case 8:
                    ShadowMap = ( TextureMapViewNode )DataViewNodeFactory.Create( "Shadow Map", textureMap );
                    return;
            }
        }
    }
}
