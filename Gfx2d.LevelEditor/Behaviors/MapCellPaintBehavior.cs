using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Gfx2d.LevelEditor.Behaviors
{
    /// <summary>
    /// Attached behavior that enables click-and-drag painting on map cells
    /// </summary>
    public static class MapCellPaintBehavior
    {
        private static bool _isLeftMouseDown = false;
        private static bool _isRightMouseDown = false;

        #region PaintCommand Attached Property
        public static readonly DependencyProperty PaintCommandProperty =
            DependencyProperty.RegisterAttached(
                "PaintCommand",
                typeof(ICommand),
                typeof(MapCellPaintBehavior),
                new PropertyMetadata(null, OnPaintCommandChanged));

        public static ICommand GetPaintCommand(DependencyObject obj)
        {
            return (ICommand)obj.GetValue(PaintCommandProperty);
        }

        public static void SetPaintCommand(DependencyObject obj, ICommand value)
        {
            obj.SetValue(PaintCommandProperty, value);
        }
        #endregion

        #region EraseCellCommand Attached Property
        public static readonly DependencyProperty EraseCellCommandProperty =
            DependencyProperty.RegisterAttached(
                "EraseCellCommand",
                typeof(ICommand),
                typeof(MapCellPaintBehavior),
                new PropertyMetadata(null));

        public static ICommand GetEraseCellCommand(DependencyObject obj)
        {
            return (ICommand)obj.GetValue(EraseCellCommandProperty);
        }

        public static void SetEraseCellCommand(DependencyObject obj, ICommand value)
        {
            obj.SetValue(EraseCellCommandProperty, value);
        }
        #endregion

        private static void OnPaintCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is Border border)
            {
                // Unsubscribe from old events
                border.MouseLeftButtonDown -= Border_MouseLeftButtonDown;
                border.MouseRightButtonDown -= Border_MouseRightButtonDown;
                border.MouseEnter -= Border_MouseEnter;
                border.MouseLeftButtonUp -= Border_MouseLeftButtonUp;
                border.MouseRightButtonUp -= Border_MouseRightButtonUp;

                // Subscribe to new events if command is not null
                if (e.NewValue != null)
                {
                    border.MouseLeftButtonDown += Border_MouseLeftButtonDown;
                    border.MouseRightButtonDown += Border_MouseRightButtonDown;
                    border.MouseEnter += Border_MouseEnter;
                    border.MouseLeftButtonUp += Border_MouseLeftButtonUp;
                    border.MouseRightButtonUp += Border_MouseRightButtonUp;

                    // Also handle the case where mouse is released outside the grid
                    var window = Window.GetWindow(border);
                    if (window != null)
                    {
                        window.MouseLeftButtonUp -= Window_MouseLeftButtonUp;
                        window.MouseRightButtonUp -= Window_MouseRightButtonUp;
                        window.MouseLeftButtonUp += Window_MouseLeftButtonUp;
                        window.MouseRightButtonUp += Window_MouseRightButtonUp;
                    }
                }
            }
        }

        private static void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _isLeftMouseDown = true;
            ExecutePaintCommand(sender);
        }

        private static void Border_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            _isRightMouseDown = true;
            e.Handled = true; // Prevent context menu from appearing
            ExecuteEraseCommand(sender);
        }

        private static void Border_MouseEnter(object sender, MouseEventArgs e)
        {
            // Paint if left dragging, erase if right dragging
            if (_isLeftMouseDown)
            {
                ExecutePaintCommand(sender);
            }
            else if (_isRightMouseDown)
            {
                ExecuteEraseCommand(sender);
            }
        }

        private static void Border_MouseLeftButtonUp(object sender, MouseButtonEventArgs e) => _isLeftMouseDown = false;

        private static void Border_MouseRightButtonUp(object sender, MouseButtonEventArgs e) => _isRightMouseDown = false;

        private static void Window_MouseLeftButtonUp(object sender, MouseButtonEventArgs e) => _isLeftMouseDown = false;

        private static void Window_MouseRightButtonUp(object sender, MouseButtonEventArgs e) => _isRightMouseDown = false;

        private static void ExecutePaintCommand(object sender)
        {
            if (sender is Border border)
            {
                var command = GetPaintCommand(border);

                // Pass the MapCellViewModel as the command parameter
                if (command != null && command.CanExecute(border.DataContext))
                {
                    command.Execute(border.DataContext);
                }
            }
        }

        private static void ExecuteEraseCommand(object sender)
        {
            if (sender is Border border)
            {
                var command = GetEraseCellCommand(border);

                // Pass the MapCellViewModel as the command parameter
                if (command != null && command.CanExecute(border.DataContext))
                {
                    command.Execute(border.DataContext);
                }
            }
        }
    }
}