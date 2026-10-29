using System;
using System.ComponentModel;
using System.Linq;
using GFDLibrary;
using GFDLibrary.Animations;
using GFDLibrary.Common;
using GFDLibrary.Conversion;
using GFDLibrary.Conversion.AssimpNet;
using GFDStudio.FormatModules;
using GFDStudio.GUI.TypeConverters;

namespace GFDStudio.GUI.DataViewNodes
{
    public class AnimationViewNode : DataViewNode<Animation>
    {
        private VariantUserPropertyList mProperties;

        public override DataViewNodeMenuFlags ContextMenuFlags => 
            DataViewNodeMenuFlags.Delete | DataViewNodeMenuFlags.Export | DataViewNodeMenuFlags.Move | DataViewNodeMenuFlags.Replace;

        public override DataViewNodeFlags NodeFlags => DataViewNodeFlags.Branch;

        public AnimationFlags Flags
        {
            get => Data.Flags;
            set => Set( x => x.Flags, value );
        }

        public float Duration
        {
            get => Data.Duration;
            set => Set( x => x.Duration, value );
        }

        [DisplayName( "Joint count" )]
        public int JointCount => Data.Joints.Count;

        [Browsable(false)]
        public ListViewNode<AnimationJoint> Joints { get; set; }

        //public List<AnimationJoint> Controllers { get; set; }

        //// 10
        //public List<AnimationFlag10000000DataEntry> Field10
        //{
        //    get => Model.Field10;
        //    set => SetDataProperty( value );
        //}

        //[Browsable(false)]
        //public AnimationExtraDataViewNode Field14 { get; set; }

        [DisplayName( "Bounding box" )]
        public BoundingBox? BoundingBox
        {
            get => Data.BoundingBox;
            set => Set( x => x.BoundingBox, value );
        }

        //public AnimationFlag80000000Data Field1C
        //{
        //    get => Model.Field1C;
        //    set => SetDataProperty( value );
        //}

        [Browsable( true )]
        [TypeConverter( typeof( ExpandableObjectConverter ) )]
        public VariantUserPropertyList Properties
        {
            get => mProperties;
            set
            {
                mProperties     = value;
                Data.Properties = Properties.Count == 0 ? null : new UserPropertyDictionary( Properties.Select( x => x.ToTypedUserProperty() ) );
            }
        }

        public float? Frequency
        {
            get => Data.Frequency;
            set => Set( x => x.Frequency, value );
        }

        protected internal AnimationViewNode( string text, Animation data ) : base( text, data )
        {
        }

        protected override void InitializeCore()
        {
            Properties = new VariantUserPropertyList( Data.Properties, () =>
            {
                Properties = mProperties;
                View?.RefreshSelection();
            } );
            RegisterExportHandler<Animation>( path => Data.Save( path ) );
            RegisterReplaceHandler<Animation>( Resource.Load<Animation> );
            RegisterReplaceHandler<AssimpScene>( file =>
            {
                var animation = AssimpNetAnimationConverter.ConvertFromAssimpScene( file, new AnimationConverterOptions() );
                var modelViewModel = Parent?.Parent as ModelPackViewNode;

                if ( modelViewModel?.Model != null )
                {
                    animation.FixTargetIds( modelViewModel.Model.Data );
                }
                else
                {
                    ImportModelAndFixTargetIds( animation );
                }

                return animation;
            } );
            RegisterCustomHandler( "Tools", "Fix IDs", () => ImportModelAndFixTargetIds( Data ) );
            RegisterCustomHandler( "Tools", "Retarget", () =>
            {
                var originalModel = ( Parent as ModelPackViewNode )?.Model?.Data ??
                                    ModuleImportUtilities.SelectImportFile<ModelPack>( "Select the original model file." )?.Model;

                if ( originalModel == null )
                    return;

                var newModel = ModuleImportUtilities.SelectImportFile<ModelPack>( "Select the new model file." )?.Model;
                if ( newModel == null )
                    return;

                bool fixArms = UserDialogs.Current.AskYesNo( "Fix arms? If unsure, select No.", "Question" );

                Data.Retarget( originalModel, newModel, fixArms );
            } );
        }

        protected override void InitializeViewCore()
        {
            Joints = ( ListViewNode<AnimationJoint> )DataViewNodeFactory.Create( "Joints", Data.Joints, new[] { new ListItemNameProvider<AnimationJoint>( ( x, i ) => x.TargetName ) } );
            AddChildNode( Joints );
        }

        private static void ImportModelAndFixTargetIds( Animation animation )
        {
            var result = UserDialogs.Current.OpenFile( new FileDialogRequest
            {
                Title = "Select a model file.",
                Filter = ModuleFilterGenerator.GenerateFilter( new[] { FormatModuleUsageFlags.Import }, typeof( ModelPack ) ).Filter,
            } );

            if ( result == null )
                return;

            try
            {
                var model = Resource.Load<ModelPack>( result.FileName );
                if ( model.Model != null )
                    animation.FixTargetIds( model.Model );
            }
            catch ( Exception e )
            {
                Console.WriteLine( e );
            }
        }
    }
}