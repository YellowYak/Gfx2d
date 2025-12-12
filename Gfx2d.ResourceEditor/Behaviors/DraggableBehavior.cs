using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows.Media;

namespace Gfx2d.ResourceEditor.Behaviors
{
    /// <summary>
    /// Attached behavior that makes an element draggable within a Canvas
    /// </summary>
    public static class DraggableBehavior
    {
        private static bool _isDragging = false;
        private static Point _dragStartPoint;
        private static TransformGroup? _originalTransform;
        private static TranslateTransform? _dragTransform;

        #region IsDraggable Attached Property
        public static readonly DependencyProperty IsDraggableProperty =
            DependencyProperty.RegisterAttached(
                "IsDraggable",
                typeof(bool),
                typeof(DraggableBehavior),
                new PropertyMetadata(false, OnIsDraggableChanged));

        public static bool GetIsDraggable(DependencyObject obj) => (bool)obj.GetValue(IsDraggableProperty);

        public static void SetIsDraggable(DependencyObject obj, bool value) => obj.SetValue(IsDraggableProperty, value);
        #endregion

        #region CellSize Attached Property
        public static readonly DependencyProperty CellSizeProperty =
            DependencyProperty.RegisterAttached(
                "CellSize",
                typeof(double),
                typeof(DraggableBehavior),
                new PropertyMetadata(50.0));

        public static double GetCellSize(DependencyObject obj) => (double)obj.GetValue(CellSizeProperty);

        public static void SetCellSize(DependencyObject obj, double value) => obj.SetValue(CellSizeProperty, value);
        #endregion

        #region UpdatePositionCommand Attached Property
        public static readonly DependencyProperty UpdatePositionCommandProperty =
            DependencyProperty.RegisterAttached(
                "UpdatePositionCommand",
                typeof(ICommand),
                typeof(DraggableBehavior),
                new PropertyMetadata(null));

        public static ICommand GetUpdatePositionCommand(DependencyObject obj) => (ICommand)obj.GetValue(UpdatePositionCommandProperty);

        public static void SetUpdatePositionCommand(DependencyObject obj, ICommand value) => obj.SetValue(UpdatePositionCommandProperty, value);
        #endregion

        private static void OnIsDraggableChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is FrameworkElement element)
            {
                // Unsubscribe from old events
                element.MouseLeftButtonDown -= Element_MouseLeftButtonDown;
                element.MouseMove -= Element_MouseMove;
                element.MouseLeftButtonUp -= Element_MouseLeftButtonUp;

                // Subscribe to new events if enabled
                if ((bool)e.NewValue)
                {
                    element.MouseLeftButtonDown += Element_MouseLeftButtonDown;
                    element.MouseMove += Element_MouseMove;
                    element.MouseLeftButtonUp += Element_MouseLeftButtonUp;

                    // Change cursor to indicate it's draggable
                    element.Cursor = Cursors.Hand;
                }
                else
                {
                    element.Cursor = Cursors.Arrow;
                }
            }
        }

        private static void Element_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var element = sender as FrameworkElement;
            if (element == null) return;

            var canvas = element.GetParentCanvas();
            if (canvas == null) return;

            _isDragging = true;
            _dragStartPoint = e.GetPosition(canvas);

            // Set up a temporary TranslateTransform for dragging
            // We need to augment the existing RenderTransform
            if (element.RenderTransform is TransformGroup existingGroup)
            {
                _originalTransform = existingGroup;
                _dragTransform = new TranslateTransform(0, 0);

                var newGroup = new TransformGroup();
                foreach (Transform t in existingGroup.Children)
                {
                    newGroup.Children.Add(t);
                }
                newGroup.Children.Add(_dragTransform);
                element.RenderTransform = newGroup;
            }
            else if (element.RenderTransform != null && element.RenderTransform != Transform.Identity)
            {
                _dragTransform = new TranslateTransform(0, 0);
                var newGroup = new TransformGroup();
                newGroup.Children.Add(element.RenderTransform);
                newGroup.Children.Add(_dragTransform);
                _originalTransform = null;
                element.RenderTransform = newGroup;
            }
            else
            {
                _dragTransform = new TranslateTransform(0, 0);
                element.RenderTransform = _dragTransform;
                _originalTransform = null;
            }

            // Ensure we receive mouse events even if cursor leaves the element
            element.CaptureMouse();
            element.Cursor = Cursors.Hand;

            e.Handled = true;
        }

        private static void Element_MouseMove(object sender, MouseEventArgs e)
        {
            if (!_isDragging || _dragTransform == null) return;

            var element = sender as FrameworkElement;
            if (element == null) return;

            var canvas = element.GetParentCanvas();
            if (canvas == null) return;

            // Get current mouse position relative to canvas
            Point currentPoint = e.GetPosition(canvas);

            // Calculate delta from start
            double deltaX = currentPoint.X - _dragStartPoint.X;
            double deltaY = currentPoint.Y - _dragStartPoint.Y;

            // Update the drag transform
            _dragTransform.X = deltaX;
            _dragTransform.Y = deltaY;

            e.Handled = true;
        }

        private static void Element_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (!_isDragging) return;

            var element = sender as FrameworkElement;
            if (element == null) return;

            _isDragging = false;
            element.ReleaseMouseCapture();
            element.Cursor = Cursors.Hand;

            // Calculate the new position
            var canvas = element.GetParentCanvas();
            if (canvas != null && _dragTransform != null)
            {
                double cellSize = GetCellSize(element);

                // Get the current bound position
                double currentLeft = Canvas.GetLeft(element);
                double currentTop = Canvas.GetTop(element);

                if (double.IsNaN(currentLeft)) currentLeft = 0;
                if (double.IsNaN(currentTop)) currentTop = 0;

                // Add the drag delta
                double newLeft = currentLeft + _dragTransform.X;
                double newTop = currentTop + _dragTransform.Y;

                // Constrain to canvas bounds
                newLeft = Math.Max(0, Math.Min(newLeft, canvas.ActualWidth));
                newTop = Math.Max(0, Math.Min(newTop, canvas.ActualHeight));

                // Convert to grid coordinates
                double gridX = newLeft / cellSize;
                double gridY = newTop / cellSize;

                // Execute the command to update the ViewModel
                var command = GetUpdatePositionCommand(element);
                if (command != null && command.CanExecute(null))
                {
                    command.Execute(new Point(gridX, gridY));
                }
            }

            // Restore the original transform
            if (_originalTransform != null)
            {
                element.RenderTransform = _originalTransform;
            }
            else if (element.RenderTransform is TransformGroup group && group.Children.Count > 1)
            {
                // Remove just the drag transform we added
                element.RenderTransform = group.Children[0];
            }
            else
            {
                element.RenderTransform = Transform.Identity;
            }

            _dragTransform = null;
            _originalTransform = null;

            e.Handled = true;
        }

        private static Canvas? GetParentCanvas(this DependencyObject element)
        {
            DependencyObject parent = VisualTreeHelper.GetParent(element);
            while (parent != null && !(parent is Canvas))
            {
                parent = VisualTreeHelper.GetParent(parent);
            }
            return parent as Canvas;
        }
    }
}