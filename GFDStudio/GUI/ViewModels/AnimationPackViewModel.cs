using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;
using GFDLibrary;
using GFDStudio.FormatModules;
using Ookii.Dialogs;

namespace GFDStudio.GUI.ViewModels
{
    public class AnimationPackViewModel : ResourceViewModel<AnimationPack>
    {
        public override TreeNodeViewModelMenuFlags ContextMenuFlags =>
            TreeNodeViewModelMenuFlags.Delete | TreeNodeViewModelMenuFlags.Export | TreeNodeViewModelMenuFlags.Replace | TreeNodeViewModelMenuFlags.Move;

        public override TreeNodeViewModelFlags NodeFlags => 
            TreeNodeViewModelFlags.Branch;

        public AnimationPackFlags Flags
        {
            get => Model.Flags;
            set => SetModelProperty( value );
        }

        [Browsable(false)]
        public AnimationListViewModel Animations { get; set; }

        [Browsable( false )]
        public AnimationListViewModel BlendAnimations { get; set; }

        [Browsable( false )]
        public AnimationExtraDataViewModel ExtraData { get; set; }

        protected internal AnimationPackViewModel( string text, AnimationPack resource ) : base( text, resource )
        {
        }

        protected override void InitializeCore()
        {
            RegisterExportHandler<AnimationPack>( path => Model.Save( path ) );
            RegisterReplaceHandler<AnimationPack>( Resource.Load<AnimationPack> );
            RegisterModelUpdateHandler( () =>
            {
                var model = new AnimationPack( Version );
                model.Animations = Animations.Model;
                model.BlendAnimations = BlendAnimations.Model;

                if ( ExtraData != null && Nodes.Contains( ExtraData ) )
                    model.ExtraData = ExtraData.Model;

                return model;
            });
            RegisterCustomHandler( "Make Relative", () =>
            {
                var originalScene = ( Parent as ModelViewModel )?.Scene?.Model ??
                                    ModuleImportUtilities.SelectImportFile<Model>( "Select the original model file." )?.Scene;

                if ( originalScene == null )
                    return;

                var newScene = ModuleImportUtilities.SelectImportFile<Model>( "Select the new model file." )?.Scene;
                if ( newScene == null )
                    return;    

                bool fixArms = MessageBox.Show( "Fix arms? If unsure, select No.", "Question", MessageBoxButtons.YesNo,
                                                MessageBoxIcon.Question, MessageBoxDefaultButton.Button2 ) == DialogResult.Yes;

                Model.MakeTransformsRelative( originalScene, newScene, fixArms );
            } );
        }

        protected override void InitializeViewCore()
        {
            // Nothing to display if we only have raw data
            if ( Model.RawData != null )
                return;

            Animations = ( AnimationListViewModel )TreeNodeViewModelFactory.Create( "Animations", Model.Animations, new[] { new ListItemNameProvider<Animation>(( x, i ) => $"Animation {i}" ) });
            BlendAnimations = ( AnimationListViewModel )TreeNodeViewModelFactory.Create( "Blend Animations", Model.BlendAnimations, new[] { new ListItemNameProvider<Animation>( ( x, i ) => $"Animation {i}" ) } );

            if ( ExtraData != null )
            {
                ExtraData = ( AnimationExtraDataViewModel ) TreeNodeViewModelFactory.Create( "Extra Data", Model.ExtraData );
                Nodes.Add( ExtraData );
            }

            Nodes.Add( Animations );
            Nodes.Add( BlendAnimations );
        }
    }

    public class AnimationListViewModel : ListViewModel<Animation>
    {
        public AnimationListViewModel( string text, List<Animation> resource, ListItemNameProvider<Animation> nameProvider ) : base( text, resource, nameProvider )
        {
        }

        public AnimationListViewModel( string text, List<Animation> resource, IList<string> itemNames ) : base( text, resource, itemNames )
        {
        }

        protected override void InitializeCore()
        {
            RegisterAddHandler<Animation>( file =>
            {
                var animation = Resource.Load<Animation>( file );
                Model.Add( animation );
            } );
            RegisterCustomHandler( "Export All", () =>
            {
                using ( var dialog = new VistaFolderBrowserDialog() )
                {
                    if ( dialog.ShowDialog() != DialogResult.OK )
                        return;

                    foreach ( AnimationViewModel animationViewModel in Nodes )
                        animationViewModel.Model.Save( Path.Combine( dialog.SelectedPath, animationViewModel.Text + ".ganm" ) );
                }
            } );
            RegisterCustomHandler( "Add New", () =>
            {
                Model.Add( new Animation() );
                InitializeView( true );
            } );

            base.InitializeCore();
        }
    }
}