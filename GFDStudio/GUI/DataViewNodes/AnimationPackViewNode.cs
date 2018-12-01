using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;
using GFDLibrary;
using GFDLibrary.Animations;
using GFDStudio.FormatModules;
using Ookii.Dialogs;

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
        public AnimationExtraDataViewNode ExtraData { get; set; }

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

                if ( ExtraData != null && Nodes.Contains( ExtraData ) )
                    model.ExtraData = ExtraData.Data;

                return model;
            });
            RegisterCustomHandler( "Retarget", () =>
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
        }

        protected override void InitializeViewCore()
        {
            // Nothing to display if we only have raw data
            if ( Data.RawData != null )
                return;

            Animations = ( AnimationListViewNode )DataViewNodeFactory.Create( "Animations", Data.Animations, new[] { new ListItemNameProvider<Animation>(( x, i ) => $"Animation {i}" ) });
            BlendAnimations = ( AnimationListViewNode )DataViewNodeFactory.Create( "Blend Animations", Data.BlendAnimations, new[] { new ListItemNameProvider<Animation>( ( x, i ) => $"Animation {i}" ) } );

            if ( ExtraData != null )
            {
                ExtraData = ( AnimationExtraDataViewNode ) DataViewNodeFactory.Create( "Extra Data", Data.ExtraData );
                Nodes.Add( ExtraData );
            }

            Nodes.Add( Animations );
            Nodes.Add( BlendAnimations );
        }
    }

    public class AnimationListViewNode : ListViewNode<Animation>
    {
        public AnimationListViewNode( string text, List<Animation> data, ListItemNameProvider<Animation> nameProvider ) : base( text, data, nameProvider )
        {
        }

        public AnimationListViewNode( string text, List<Animation> data, IList<string> itemNames ) : base( text, data, itemNames )
        {
        }

        protected override void InitializeCore()
        {
            RegisterAddHandler<Animation>( file =>
            {
                var animation = Resource.Load<Animation>( file );
                Data.Add( animation );
            } );
            RegisterCustomHandler( "Export All", () =>
            {
                using ( var dialog = new VistaFolderBrowserDialog() )
                {
                    if ( dialog.ShowDialog() != DialogResult.OK )
                        return;

                    foreach ( AnimationViewNode animationViewModel in Nodes )
                        animationViewModel.Data.Save( Path.Combine( dialog.SelectedPath, animationViewModel.Text + ".ganm" ) );
                }
            } );
            RegisterCustomHandler( "Add New", () =>
            {
                Data.Add( new Animation() );
                InitializeView( true );
            } );

            base.InitializeCore();
        }
    }
}