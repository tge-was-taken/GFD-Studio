using System;
using System.ComponentModel;
using System.Windows.Forms;
using GFDLibrary;
using GFDLibrary.Animations;
using GFDStudio.FormatModules;

namespace GFDStudio.GUI.DataViewNodes
{
    public class AnimationPackViewNode : ResourceViewNode<AnimationPack>
    {
        public override DataViewNodeMenuFlags ContextMenuFlags =>
            DataViewNodeMenuFlags.Delete | DataViewNodeMenuFlags.Export | DataViewNodeMenuFlags.Replace | DataViewNodeMenuFlags.Move;

        public override DataViewNodeFlags NodeFlags => 
            DataViewNodeFlags.Branch;

        public AnimationPackFlags Flags
        {
            get => Data.Flags;
            set => SetDataProperty( value );
        }

        [Browsable(false)]
        public AnimationListViewNode Animations { get; set; }

        [Browsable( false )]
        public AnimationListViewNode BlendAnimations { get; set; }

        [Browsable( false )]
        public AnimationBit29DataViewNode Bit29Data { get; set; }

        protected internal AnimationPackViewNode( string text, AnimationPack data ) : base( text, data )
        {
        }

        protected override void InitializeCore()
        {
            RegisterExportHandler<AnimationPack>( path => Data.Save( path ) );
            RegisterReplaceHandler<AnimationPack>( Resource.Load<AnimationPack> );
            RegisterModelUpdateHandler( () =>
            {
                var model = new AnimationPack( Version );
                model.Animations = Animations.Data;
                model.BlendAnimations = BlendAnimations.Data;

                if ( Bit29Data != null && Nodes.Contains( Bit29Data ) )
                    model.Bit29Data = Bit29Data.Data;

                return model;
            });
            RegisterCustomHandler( "Tools", "Retarget", () =>
            {
                var originalScene = ( Parent as ModelPackViewNode )?.Model?.Data ??
                                    ModuleImportUtilities.SelectImportFile<ModelPack>( "Select the original model file." )?.Model;

                if ( originalScene == null )
                    return;

                var newScene = ModuleImportUtilities.SelectImportFile<ModelPack>( "Select the new model file." )?.Model;
                if ( newScene == null )
                    return;    

                bool fixArms = MessageBox.Show( "Fix arms? If unsure, select No.", "Question", MessageBoxButtons.YesNo,
                                                MessageBoxIcon.Question, MessageBoxDefaultButton.Button2 ) == DialogResult.Yes;

                Data.Retarget( originalScene, newScene, fixArms );
            } );
            RegisterCustomHandler("Tools", "Convert to P5", () =>
            {
                Data.ConvertToP5();
            });
            RegisterCustomHandler("Tools", "Fix IDs", () =>
            {
                ImportModelAndFixTargetIds(Data);
            });
        }

        protected override void InitializeViewCore()
        {
            // Nothing to display if we only have raw data
            if ( Data.RawData != null )
                return;

            Animations = ( AnimationListViewNode )DataViewNodeFactory.Create( "Animations", Data.Animations, new[] { new ListItemNameProvider<Animation>(( x, i ) => $"Animation {i}" ) });
            BlendAnimations = ( AnimationListViewNode )DataViewNodeFactory.Create( "Blend Animations", Data.BlendAnimations, new[] { new ListItemNameProvider<Animation>( ( x, i ) => $"Animation {i}" ) } );

            if ( Bit29Data != null )
            {
                Bit29Data = ( AnimationBit29DataViewNode ) DataViewNodeFactory.Create( "Bit 29 Data", Data.Bit29Data );
                AddChildNode( Bit29Data );
            }

            AddChildNode( Animations );
            AddChildNode( BlendAnimations );
        }
        private static void ImportModelAndFixTargetIds(AnimationPack pack)
        {
            using (var dialog = new OpenFileDialog())
            {
                dialog.Filter = ModuleFilterGenerator.GenerateFilter(new[] { FormatModuleUsageFlags.Import }, typeof(ModelPack)).Filter;
                dialog.AutoUpgradeEnabled = true;
                dialog.CheckPathExists = true;
                dialog.Title = "Select a model file.";
                dialog.ValidateNames = true;
                dialog.AddExtension = true;

                if (dialog.ShowDialog() != DialogResult.OK)
                    return;

                foreach (var animation in pack.Animations)
                    try
                    {
                        var model = Resource.Load<ModelPack>(dialog.FileName);
                        if (model.Model != null)
                            animation.FixTargetIds(model.Model);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
            }
        }
    }
}