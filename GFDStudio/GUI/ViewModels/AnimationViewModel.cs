using System;
using System.Windows.Forms;
using GFDLibrary;
using GFDLibrary.Processing.Models;
using GFDStudio.FormatModules;

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
            RegisterReplaceHandler<Assimp.Scene>( file =>
            {
                var animation = AnimationConverter.ConvertFromAssimpScene( file, new AnimationConverterOptions() );
                var modelViewModel = Parent?.Parent as ModelViewModel;

                if ( modelViewModel?.Scene != null )
                {
                    animation.FixTargetIds( modelViewModel.Scene.Model );
                }
                else
                {
                    ImportModelAndFixTargetIds( animation );
                }

                return animation;
            } );
            RegisterCustomHandler( "Fix IDs", () => ImportModelAndFixTargetIds( Model ) );
        }

        private static void ImportModelAndFixTargetIds( Animation animation )
        {
            using ( var dialog = new OpenFileDialog() )
            {
                dialog.Filter = ModuleFilterGenerator.GenerateFilter( new[] { FormatModuleUsageFlags.Import }, typeof( Model ) );
                dialog.AutoUpgradeEnabled = true;
                dialog.CheckPathExists = true;
                dialog.Title = "Select a model file.";
                dialog.ValidateNames = true;
                dialog.AddExtension = true;

                if ( dialog.ShowDialog() != DialogResult.OK )
                    return;

                try
                {
                    var model = Resource.Load<Model>( dialog.FileName );
                    if ( model.Scene != null )
                        animation.FixTargetIds( model.Scene );
                }
                catch ( Exception e )
                {
                    Console.WriteLine( e );
                }
            }
        }
    }
}