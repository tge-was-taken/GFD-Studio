using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace UsefulThings.WPF
{
    /// <summary>
    /// Contains general things for WPF.
    /// </summary>
    public static class General
    {
        /// <summary>
        /// Performs the Mouse Drag Move on a Borderless window.
        /// Works with multimonitor and DPI Scaling to allow dragging a window incl to and from maximised.
        /// </summary>
        /// <param name="window">Borderless window to move.</param>
        /// <param name="e">Mouse events from Window_MouseLeftButtonDown event.</param>
        public static void DoBorderlessWindowDragMove(Window window, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left && window.WindowState == WindowState.Maximized)
            {
                // WORKING IN NON SCALED UNTIL THE END
                // Set position of window back up to mouse, since restoring the window goes back to its previous location.
                // Current cursor horizontal ratio location
                double DPIScale = UsefulThings.General.GetDPIScalingFactorFOR_CURRENT_MONITOR(window);
                Debug.WriteLine($"scale: {DPIScale}");

                var currentCursor = window.PointToScreen(e.GetPosition(window));
                var screens = System.Windows.Forms.Screen.AllScreens;
                var currentScreen = System.Windows.Forms.Screen.FromPoint(new System.Drawing.Point((int)currentCursor.X, (int)currentCursor.Y));
                var cursorDistFromLocalOrigin = Math.Abs(Math.Abs(currentCursor.X) - Math.Abs(currentScreen.WorkingArea.X));
                double localRatioOfCursorToScreen = cursorDistFromLocalOrigin / currentScreen.WorkingArea.Width;
                double newLocalLeft = cursorDistFromLocalOrigin - ((window.RestoreBounds.Width * DPIScale) * localRatioOfCursorToScreen);
                double newX = currentScreen.WorkingArea.X + newLocalLeft;

                window.WindowState = WindowState.Normal;
                window.Top = currentScreen.WorkingArea.Y / DPIScale + 1;
                window.Left = newX / DPIScale;

                window.DragMove();
            }
            else if (e.ChangedButton == MouseButton.Left)
            {
                if (e.ClickCount == 2)
                    window.WindowState = window.WindowState == WindowState.Normal ? WindowState.Maximized : WindowState.Normal;
                else
                    window.DragMove();
            }
        }

        /// <summary>
        /// Finds visual child of given element. Optionally matches name of FrameWorkElement.
        /// </summary>
        /// <typeparam name="T">Visual Container Object to search within.</typeparam>
        /// <param name="obj">Object type to search for. e.g. TextBox, Label, etc</param>
        /// <param name="itemName">Name of FrameWorkElement in XAML.</param>
        /// <returns>FrameWorkElement</returns>
        public static T FindVisualChild<T>(DependencyObject obj, string itemName = null) where T : FrameworkElement
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(obj, i);
                if (child != null && child is T && (itemName != null ? ((T)child).Name == itemName : true))
                    return (T)child;
                else
                {
                    T childOfChild = FindVisualChild<T>(child, itemName);
                    if (childOfChild != null)
                        return childOfChild;
                }
            }
            return null;
        }


        /// <summary>
        /// Draws line graph on a canvas.
        /// Adaptive, but only draws dataline.
        /// </summary>
        /// <param name="canvas">Canvas to draw on.</param>
        /// <param name="values">Y values.</param>
        public static void DrawGraph(Canvas canvas, Queue<double> values)
        {
            // Build adaptive points list
            double xSize = canvas.ActualWidth;
            double ySize = canvas.ActualHeight;

            // Draw points
            int numPoints = values.Count;
            double maxValue = values.Max();

            for (int i = 0; i < numPoints; i += 2)
            {
                Line line = new Line();
                line.Stroke = System.Windows.Media.Brushes.LightSteelBlue;
                line.StrokeThickness = 0.5;

                line.X1 = (i / numPoints) * xSize;
                line.X2 = ((i + 1) / numPoints ) * xSize;
                line.Y1 = (values.ElementAt(i) / maxValue) * ySize;
                line.Y2 = (values.ElementAt(i + 1) / maxValue) * ySize;
                
                canvas.Children.Add(line);
            }
        }
    }
}
