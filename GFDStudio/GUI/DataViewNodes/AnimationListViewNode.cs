using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using GFDLibrary;
using GFDLibrary.Animations;
using Ookii.Dialogs;
using Ookii.Dialogs.Wpf;

namespace GFDStudio.GUI.DataViewNodes
{
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
            RegisterCustomHandler( "Export", "All", () =>
            {
                var dialog = new VistaFolderBrowserDialog();
                {
                    if ( dialog.ShowDialog() != true )
                        return;

                    foreach ( AnimationViewNode animationViewModel in Nodes )
                        animationViewModel.Data.Save( Path.Combine( dialog.SelectedPath, animationViewModel.Text + ".ganm" ) );
                }
            } );
            RegisterCustomHandler( "Add", "New animation", () =>
            {
                Data.Add( new Animation() );
                InitializeView( true );
            } );

            base.InitializeCore();
        }
    }
}