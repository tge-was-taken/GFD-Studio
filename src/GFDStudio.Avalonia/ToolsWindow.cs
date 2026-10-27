using System;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using GFDStudio.IO;

namespace GFDStudio.Avalonia
{
    /// <summary>
    /// The jobs that run over a directory of files, each with room to say what it does. A menu
    /// gives no space for that, and these are destructive enough to be worth explaining.
    /// </summary>
    public sealed class ToolsWindow : Window
    {
        private sealed record Tool( string Name, string Description, Action Run );

        private static readonly Tool[] Tools =
        {
            new( "Retarget animation packs",
                 "Rewrites every GAP under a directory to follow a different model's skeleton. " +
                 "Asks for the original and the new model first. Replaces the files in place.",
                 BatchTools.RetargetAnimationPacks ),

            new( "Rescale or reposition animation packs",
                 "Applies a scale, position and rotation to every GAP under a directory. " +
                 "Replaces the files in place.",
                 BatchTools.RescaleAnimationPacks ),

            new( "Convert animation packs to Persona 5",
                 "Converts every GAP under a directory from the Royal format to the original. " +
                 "Replaces the files in place.",
                 BatchTools.ConvertAnimationPacksToP5 ),

            new( "Convert model materials",
                 "Applies one material preset to every GMD under a directory. Asks for the " +
                 "conversion options first. Replaces the files in place.",
                 BatchTools.ConvertModelMaterials ),

            new( "Copy a split animation pack to many models",
                 "Takes the a, b and c halves of a Persona 5 battle GAP and writes a copy " +
                 "retargeted to each model under a directory.",
                 BatchTools.CopySplitAnimationPackToModels ),

            new( "Export textures from models",
                 "Writes every texture of every model under a directory into a folder of your " +
                 "choosing, keeping each texture's own format where it has one.",
                 BatchTools.ExportTexturesFromModels ),

            new( "Replace textures in models",
                 "Looks for a DDS of the same name in a folder of replacements and swaps it into " +
                 "every model under a directory. Replaces the files in place.",
                 BatchTools.ReplaceTexturesInModels ),
        };

        public ToolsWindow()
        {
            Title = "Tools";
            Width = 620;
            Height = 620;
            WindowStartupLocation = WindowStartupLocation.CenterOwner;

            var list = new StackPanel { Spacing = 8, Margin = new Thickness( 16 ) };

            foreach ( var tool in Tools )
                list.Children.Add( Card( tool ) );

            var close = new Button { Content = "Close", MinWidth = 80, IsCancel = true, HorizontalAlignment = HorizontalAlignment.Right, Margin = new Thickness( 16, 0, 16, 16 ) };
            close.Click += ( s, e ) => Close();

            var root = new DockPanel();
            DockPanel.SetDock( close, Dock.Bottom );
            root.Children.Add( close );
            root.Children.Add( new ScrollViewer { Content = list } );
            Content = root;
        }

        private Control Card( Tool tool )
        {
            var run = new Button { Content = "Run", MinWidth = 70, VerticalAlignment = VerticalAlignment.Center };
            run.Click += ( s, e ) => tool.Run();

            var text = new StackPanel { Spacing = 2 };
            text.Children.Add( new TextBlock { Text = tool.Name, FontWeight = FontWeight.SemiBold, TextWrapping = TextWrapping.Wrap } );
            text.Children.Add( new TextBlock { Text = tool.Description, TextWrapping = TextWrapping.Wrap, Opacity = 0.75 } );

            var row = new Grid { ColumnDefinitions = new ColumnDefinitions( "*,Auto" ), ColumnSpacing = 12 };
            Grid.SetColumn( text, 0 );
            Grid.SetColumn( run, 1 );
            row.Children.Add( text );
            row.Children.Add( run );

            return new Border
            {
                Padding = new Thickness( 12 ),
                CornerRadius = new CornerRadius( 4 ),
                Background = new SolidColorBrush( Color.FromArgb( 40, 255, 255, 255 ) ),
                Child = row,
            };
        }
    }
}
