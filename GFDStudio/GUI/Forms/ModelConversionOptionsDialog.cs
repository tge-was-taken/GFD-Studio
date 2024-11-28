using GFDLibrary;
using GFDLibrary.Conversion;
using GFDLibrary.Materials;
using GFDLibrary.Models;
using System;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace GFDStudio.GUI.Forms
{
    public partial class ModelConversionOptionsDialog : Form
    {
        public ModelConverterOptionsViewModel ViewModel { get; } = new();

        public ModelConverterOptions GetModelConversionOptions() => ViewModel.MapToModelConverterOptions();

        public ModelConversionOptionsDialog(
            Assimp.Scene scene = default, 
            ModelPack originalModel = default)
        {
            InitializeComponent();
            var materialsNode = new TreeNode() { Text = "Materials" };
            if (scene is not null)
            {
                foreach ( var item in scene.Materials )
                {
                    var options = new ModelConverterMaterialOptionsViewModel( ViewModel )
                    {
                        Name = item.Name,
                        InheritDefaults = true,
                        Mesh = null,
                        Preset = null,
                    };

                    if ( originalModel?.Materials?.Any() ?? false )
                    {
                        if ( originalModel.Materials.TryGetValue( item.Name, out var referenceMat ) )
                        {
                            options.InheritDefaults = false;
                            options.Preset = new ModelConverterMaterialPresetViewModel( referenceMat );
                        }
                    }

                    var materialNode = new TreeNode()
                    {
                        Text = item.Name,
                        Tag = options
                    };
                    materialsNode.Nodes.Add( materialNode );
                    ViewModel.Materials.Add( options );
                }
            }

            var meshesNode = new TreeNode() { Text = "Meshes" };
            if ( scene is not null )
            {
                foreach ( var item in scene.Meshes )
                {
                    var options = new ModelConverterMeshOptionsViewModel()
                    {
                        Name = item.Name,
                        InheritDefaults = true,
                        InheritChannelSettingsDefaults = true,
                        GeometryFlags = null,
                        VertexAttributeFlags = null
                    };
                    if ( originalModel is not null )
                    {
                        var findResult = FindReferenceMeshInOriginalModel( scene, originalModel, item );
                        if ( findResult.Mesh is not null )
                        {
                            options.InheritDefaults = false;
                            options.GeometryFlags = findResult.Mesh.Flags;
                            options.VertexAttributeFlags = findResult.Mesh.VertexAttributeFlags;
                        }
                    }

                    var meshNode = new TreeNode()
                    {
                        Text = item.Name,
                        Tag = options
                    };
                    meshesNode.Nodes.Add( meshNode );
                    ViewModel.Meshes.Add( options );
                }
            }
            sceneTreeView.Nodes.AddRange( new[] { materialsNode, meshesNode } );
            sceneTreeView.AfterSelect += SceneTreeView_AfterSelect;
            if (originalModel is not null)
            {
                ViewModel.Version = originalModel.Version;
                foreach ( var mat in originalModel.Materials.Values )
                {
                    ViewModel.AdditionalMaterialPresets.Add( Material.ConvertToMaterialPreset( mat, mat ) );
                }
            }
            generalPropertyGrid.SelectedObject = ViewModel;
        }

        record ReferenceMeshFindResult(
            Mesh Mesh,
            bool ByMaterialName);

        private static ReferenceMeshFindResult FindReferenceMeshInOriginalModel( Assimp.Scene scene, ModelPack originalModel, Assimp.Mesh item )
        {
            var foundByMaterialName = false;
            GFDLibrary.Models.Mesh referenceMesh = default;
            if ( originalModel?.Model is not null )
            {
                foreach ( var node in originalModel.Model.Nodes )
                {
                    for ( int i = 0; i < node.Attachments.Count; i++ )
                    {
                        if ( node.Attachments[i].Type == NodeAttachmentType.Mesh )
                        {
                            var meshName = ModelConversionHelpers.GetMeshExportName( node.Name, i );
                            if ( item.Name == meshName )
                            {
                                referenceMesh = node.Attachments[i].GetValue<GFDLibrary.Models.Mesh>();
                                break;
                            }
                        }
                    }
                    if ( referenceMesh is not null )
                        break;
                }
            }

            if ( referenceMesh is null )
            {
                referenceMesh = originalModel.Model.Meshes
                    .Where( m => m.MaterialName == scene.Materials[item.MaterialIndex].Name )
                    .FirstOrDefault();
                foundByMaterialName = true;
            }

            return new(referenceMesh, foundByMaterialName );
        }

        private void SceneTreeView_AfterSelect( object sender, TreeViewEventArgs e )
        {
            scenePropertyGrid.SelectedObject = e.Node.Tag;
        }

        private void ModelConversionOptionsDialog_Load( object sender, EventArgs e )
        {

        }

        private void splitContainer1_SplitterMoved( object sender, SplitterEventArgs e )
        {

        }
    }
}
