using System.ComponentModel;
using GFDLibrary;

namespace GFDStudio.GUI.ViewModels
{
    public class AnimationExtraDataViewModel : TreeNodeViewModel<AnimationExtraData>
    {
        public override TreeNodeViewModelMenuFlags ContextMenuFlags =>
            TreeNodeViewModelMenuFlags.Delete | TreeNodeViewModelMenuFlags.Export | TreeNodeViewModelMenuFlags.Replace;

        public override TreeNodeViewModelFlags NodeFlags => TreeNodeViewModelFlags.Branch;

        [Browsable( false )]
        public AnimationViewModel Field00 { get; set; }

        public float Field10
        {
            get => Model.Field10;
            set => SetModelProperty( value );
        }

        [Browsable( false )]
        public AnimationViewModel Field04 { get; set; }

        public float Field14
        {
            get => Model.Field14;
            set => SetModelProperty( value );
        }

        [Browsable( false )]
        public AnimationViewModel Field08 { get; set; }

        public float Field18
        {
            get => Model.Field18;
            set => SetModelProperty( value );
        }

        [Browsable( false )]
        public AnimationViewModel Field0C { get; set; }

        public float Field1C
        {
            get => Model.Field1C;
            set => SetModelProperty( value );
        }

        protected internal AnimationExtraDataViewModel( string text, AnimationExtraData resource ) : base( text, resource )
        {
        }

        protected override void InitializeCore()
        {
            RegisterExportHandler<AnimationExtraData>( path => Model.Save( path ) );
            RegisterReplaceHandler<AnimationExtraData>( Resource.Load<AnimationExtraData> );
            RegisterModelUpdateHandler( () => new AnimationExtraData
            {
                Field00 = Field00.Model,
                Field10 = Field10,
                Field04 = Field04.Model,
                Field14 = Field14,
                Field08 = Field08.Model,
                Field18 = Field18,
                Field0C = Field0C.Model,
                Field1C = Field1C
            } );
        }

        protected override void InitializeViewCore()
        {
            Field00 = ( AnimationViewModel ) TreeNodeViewModelFactory.Create( "Animation 1", Model.Field00 );
            Field04 = ( AnimationViewModel ) TreeNodeViewModelFactory.Create( "Animation 2", Model.Field04 );
            Field08 = ( AnimationViewModel ) TreeNodeViewModelFactory.Create( "Animation 3", Model.Field08 );
            Field0C = ( AnimationViewModel ) TreeNodeViewModelFactory.Create( "Animation 4", Model.Field0C );

            Nodes.AddRange( new[] { Field00, Field04, Field08, Field0C } );
        }
    }
}