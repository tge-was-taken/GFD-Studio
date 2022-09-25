﻿using System.ComponentModel;
using GFDLibrary;
using GFDLibrary.Animations;

namespace GFDStudio.GUI.DataViewNodes
{
    public class AnimationBit29DataViewNode : DataViewNode<AnimationBit29Data>
    {
        public override DataViewNodeMenuFlags ContextMenuFlags =>
            DataViewNodeMenuFlags.Delete | DataViewNodeMenuFlags.Export | DataViewNodeMenuFlags.Replace;

        public override DataViewNodeFlags NodeFlags => DataViewNodeFlags.Branch;

        [Browsable( false )]
        public AnimationViewNode Field00 { get; set; }

        public float Field10
        {
            get => Data.Field10;
            set => SetDataProperty( value );
        }

        [Browsable( false )]
        public AnimationViewNode Field04 { get; set; }

        public float Field14
        {
            get => Data.Field14;
            set => SetDataProperty( value );
        }

        [Browsable( false )]
        public AnimationViewNode Field08 { get; set; }

        public float Field18
        {
            get => Data.Field18;
            set => SetDataProperty( value );
        }

        [Browsable( false )]
        public AnimationViewNode Field0C { get; set; }

        public float Field1C
        {
            get => Data.Field1C;
            set => SetDataProperty( value );
        }

        protected internal AnimationBit29DataViewNode( string text, AnimationBit29Data data ) : base( text, data )
        {
        }

        protected override void InitializeCore()
        {
            RegisterExportHandler<AnimationBit29Data>( path => Data.Save( path ) );
            RegisterReplaceHandler<AnimationBit29Data>( Resource.Load<AnimationBit29Data> );
            RegisterModelUpdateHandler( () => new AnimationBit29Data
            {
                Field00 = Field00.Data,
                Field10 = Field10,
                Field04 = Field04.Data,
                Field14 = Field14,
                Field08 = Field08.Data,
                Field18 = Field18,
                Field0C = Field0C.Data,
                Field1C = Field1C
            } );
        }

        protected override void InitializeViewCore()
        {
            Field00 = ( AnimationViewNode ) DataViewNodeFactory.Create( "Animation 1", Data.Field00 );
            Field04 = ( AnimationViewNode ) DataViewNodeFactory.Create( "Animation 2", Data.Field04 );
            Field08 = ( AnimationViewNode ) DataViewNodeFactory.Create( "Animation 3", Data.Field08 );
            Field0C = ( AnimationViewNode ) DataViewNodeFactory.Create( "Animation 4", Data.Field0C );

            Nodes.AddRange( new[] { Field00, Field04, Field08, Field0C } );
        }
    }
}