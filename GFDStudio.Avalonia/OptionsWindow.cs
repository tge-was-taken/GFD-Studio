using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using GFDStudio.GUI;

namespace GFDStudio.Avalonia
{
    /// <summary>
    /// The settings that are not tied to one operation. The material retain toggles are asked
    /// for on the replace dialog instead, where they apply.
    /// </summary>
    public sealed class OptionsWindow : Window
    {
        public OptionsWindow( Config settings, Action<bool> applyTheme )
        {
            Title = "Options";
            Width = 560;
            SizeToContent = SizeToContent.Height;
            WindowStartupLocation = WindowStartupLocation.CenterOwner;

            var list = new StackPanel { Spacing = 10, Margin = new Thickness( 16 ) };

            list.Children.Add( Toggle(
                "Save replaced textures outside the model (Metaphor)",
                "From Metaphor onwards a texture can live in a file beside the model instead of " +
                "inside it. Has no effect on earlier games.",
                settings.SaveReplacedTexturesExternally, x => settings.SaveReplacedTexturesExternally = x ) );

            list.Children.Add( Toggle(
                "Use the dark theme",
                "Switches the window between the dark and light palettes.",
                settings.DarkMode, applyTheme ) );

            var close = new Button { Content = "Close", MinWidth = 80, IsCancel = true, HorizontalAlignment = HorizontalAlignment.Right, Margin = new Thickness( 16, 0, 16, 16 ) };
            close.Click += ( s, e ) => Close();

            var root = new DockPanel();
            DockPanel.SetDock( close, Dock.Bottom );
            root.Children.Add( close );
            root.Children.Add( new ScrollViewer { Content = list } );
            Content = root;
        }

        private static Control Toggle( string name, string description, bool initial, Action<bool> set )
        {
            var box = new CheckBox { Content = name, IsChecked = initial };
            box.IsCheckedChanged += ( s, e ) => set( box.IsChecked == true );

            var text = new StackPanel { Spacing = 2 };
            text.Children.Add( box );
            text.Children.Add( new TextBlock
            {
                Text = description,
                TextWrapping = TextWrapping.Wrap,
                Opacity = 0.75,
                Margin = new Thickness( 28, 0, 0, 0 ),
            } );

            return text;
        }
    }
}
