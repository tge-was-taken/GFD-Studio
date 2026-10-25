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
        public ModelConverterOptionsViewModel ViewModel { get; }

        public ModelConverterOptions GetModelConversionOptions() => ViewModel.MapToModelConverterOptions();

        public ModelConversionOptionsDialog(
            Assimp.Scene scene = default,
            ModelPack originalModel = default )
        {
            InitializeComponent();

            ViewModel = ModelConverterOptionsBuilder.Build( scene, originalModel );

            var materialsNode = new TreeNode { Text = "Materials" };
            foreach ( var options in ViewModel.Materials )
                materialsNode.Nodes.Add( new TreeNode { Text = options.Name, Tag = options } );

            var meshesNode = new TreeNode { Text = "Meshes" };
            foreach ( var options in ViewModel.Meshes )
                meshesNode.Nodes.Add( new TreeNode { Text = options.Name, Tag = options } );

            sceneTreeView.Nodes.AddRange( new[] { materialsNode, meshesNode } );
            sceneTreeView.AfterSelect += SceneTreeView_AfterSelect;
            generalPropertyGrid.SelectedObject = ViewModel;
        }

        private void SceneTreeView_AfterSelect( object sender, TreeViewEventArgs e )
        {
            scenePropertyGrid.SelectedObject = e.Node.Tag;
        }
    }
}
