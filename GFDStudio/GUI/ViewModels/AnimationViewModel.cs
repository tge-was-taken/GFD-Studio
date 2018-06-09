using GFDLibrary;

namespace GFDStudio.GUI.ViewModels
{
    public class AnimationViewModel : TreeNodeViewModel<Animation>
    {
        public override TreeNodeViewModelMenuFlags ContextMenuFlags => 
            TreeNodeViewModelMenuFlags.Delete | TreeNodeViewModelMenuFlags.Export | TreeNodeViewModelMenuFlags.Move | TreeNodeViewModelMenuFlags.Replace;

        public override TreeNodeViewModelFlags NodeFlags => TreeNodeViewModelFlags.Leaf;

        public AnimationFlags Flags
        {
            get => Model.Flags;
            set => SetModelProperty( value );
        }

        public float Duration
        {
            get => Model.Duration;
            set => SetModelProperty( value );
        }

        public int ControllerCount => Model.Controllers.Count;

        //public List<AnimationController> Controllers { get; set; }

        //// 10
        //public List<AnimationFlag10000000DataEntry> Field10
        //{
        //    get => Model.Field10;
        //    set => SetModelProperty( value );
        //}

        //[Browsable(false)]
        //public AnimationExtraDataViewModel Field14 { get; set; }

        public BoundingBox? BoundingBox
        {
            get => Model.BoundingBox;
            set => SetModelProperty( value );
        }

        //public AnimationFlag80000000Data Field1C
        //{
        //    get => Model.Field1C;
        //    set => SetModelProperty( value );
        //}

        public UserPropertyCollection Properties
        {
            get => Model.Properties;
            set => SetModelProperty( value );
        }

        public float? Speed
        {
            get => Model.Speed;
            set => SetModelProperty( value );
        }

        protected internal AnimationViewModel( string text, Animation resource ) : base( text, resource )
        {
        }

        protected override void InitializeCore()
        {
            RegisterExportHandler<Animation>( path => Model.Save( path ) );
            RegisterReplaceHandler<Animation>( Resource.Load<Animation> );
        }
    }
}