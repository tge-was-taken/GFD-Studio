using System.Collections.Generic;
using System.ComponentModel;
using GFDLibrary;

namespace GFDStudio.GUI.ViewModels
{
    public class AnimationPackViewModel : ResourceViewModel<AnimationPack>
    {
        public override TreeNodeViewModelMenuFlags ContextMenuFlags => 
            TreeNodeViewModelMenuFlags.Delete | TreeNodeViewModelMenuFlags.Export | TreeNodeViewModelMenuFlags.Replace;

        public override TreeNodeViewModelFlags NodeFlags => 
            TreeNodeViewModelFlags.Branch;

        public AnimationPackFlags Flags
        {
            get => Model.Flags;
            set => SetModelProperty( value );
        }

        [Browsable(false)]
        public ListViewModel<Animation> Animations { get; set; }

        [Browsable( false )]
        public ListViewModel<Animation> BlendAnimations { get; set; }

        [Browsable( false )]
        public AnimationExtraDataViewModel ExtraData { get; set; }

        protected internal AnimationPackViewModel( string text, AnimationPack resource ) : base( text, resource )
        {
        }

        protected override void InitializeCore()
        {
            RegisterExportHandler<AnimationPack>( path =>
            {
                var model = new Model( Version ) { AnimationPack = Model };
                model.Save( path );
            } );
            RegisterReplaceHandler<AnimationPack>( path => Resource.Load<Model>( path ).AnimationPack );
            RegisterModelUpdateHandler( () =>
            {
                var model = new AnimationPack( Version );
                model.Animations = Animations.Model;
                model.BlendAnimations = BlendAnimations.Model;

                if ( ExtraData != null && Nodes.Contains( ExtraData ) )
                    model.ExtraData = ExtraData.Model;

                return model;
            });
        }

        protected override void InitializeViewCore()
        {
            // Nothing to display if we only have raw data
            if ( Model.RawData != null )
                return;

            Animations = ( ListViewModel < Animation > )TreeNodeViewModelFactory.Create( "Animations", Model.Animations, new[] { new ListItemNameProvider<Animation>(( x, i ) => $"Animation {i}" ) });
            BlendAnimations = ( ListViewModel<Animation> )TreeNodeViewModelFactory.Create( "Blend Animations", Model.BlendAnimations, new[] { new ListItemNameProvider<Animation>( ( x, i ) => $"Animation {i}" ) } );

            if ( ExtraData != null )
            {
                ExtraData = ( AnimationExtraDataViewModel ) TreeNodeViewModelFactory.Create( "Extra Data", Model.ExtraData );
                Nodes.Add( ExtraData );
            }

            Nodes.Add( Animations );
            Nodes.Add( BlendAnimations );
        }
    }
}