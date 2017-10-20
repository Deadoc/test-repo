using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interactivity;

namespace UPSBatteryController.Presentation.Behaviors
{
    public class MouseActionBehavior : Behavior<FrameworkElement>
    {
        public static readonly DependencyProperty IsEnabledProperty = DependencyProperty.Register(
            "IsEnabled", typeof(bool), typeof(MouseActionBehavior), new PropertyMetadata(true));

        public bool IsEnabled
        {
            get { return (bool)GetValue(IsEnabledProperty); }
            set { SetValue(IsEnabledProperty, value); }
        }

        public static readonly DependencyProperty MouseLeftButtonClickCommandProperty = DependencyProperty.Register(
            "MouseLeftButtonClickCommand", typeof(ICommand), typeof(MouseActionBehavior), new PropertyMetadata(default(ICommand)));

        public ICommand MouseLeftButtonClickCommand
        {
            get { return (ICommand)GetValue(MouseLeftButtonClickCommandProperty); }
            set { SetValue(MouseLeftButtonClickCommandProperty, value); }
        }

        public static readonly DependencyProperty MouseLeftButtonClickCommandParameterProperty = DependencyProperty.Register(
            "MouseLeftButtonClickCommandParameter", typeof(object), typeof(MouseActionBehavior), new PropertyMetadata(default(object)));

        public object MouseLeftButtonClickCommandParameter
        {
            get { return (object)GetValue(MouseLeftButtonClickCommandParameterProperty); }
            set { SetValue(MouseLeftButtonClickCommandParameterProperty, value); }
        }

        public static readonly DependencyProperty MouseRightButtonClickCommandProperty = DependencyProperty.Register(
            "MouseRightButtonClickCommand", typeof(ICommand), typeof(MouseActionBehavior), new PropertyMetadata(default(ICommand)));

        public ICommand MouseRightButtonClickCommand
        {
            get { return (ICommand)GetValue(MouseRightButtonClickCommandProperty); }
            set { SetValue(MouseRightButtonClickCommandProperty, value); }
        }

        public static readonly DependencyProperty MouseRightButtonClickCommandParameterProperty = DependencyProperty.Register(
            "MouseRightButtonClickCommandParameter", typeof(object), typeof(MouseActionBehavior), new PropertyMetadata(default(object)));

        public object MouseRightButtonClickCommandParameter
        {
            get { return (object)GetValue(MouseRightButtonClickCommandParameterProperty); }
            set { SetValue(MouseRightButtonClickCommandParameterProperty, value); }
        }

        protected override void OnAttached()
        {
            base.OnAttached();
            
            AssociatedObject.PreviewMouseLeftButtonDown += AssociatedObjectOnMouseLeftButtonDown;
            AssociatedObject.PreviewMouseLeftButtonUp += AssociatedObjectOnMouseLeftButtonUp;
            AssociatedObject.PreviewMouseRightButtonDown += AssociatedObjectOnMouseRightButtonDown;
            AssociatedObject.PreviewMouseRightButtonUp += AssociatedObjectOnMouseRightButtonUp;
        }

        private DateTime _leftMouseDownStart = DateTime.Now;
        
        private void AssociatedObjectOnMouseLeftButtonUp(object sender, MouseButtonEventArgs mouseButtonEventArgs)
        {
            var dif = DateTime.Now - _leftMouseDownStart;
            if (dif.TotalMilliseconds < 300 && MouseLeftButtonClickCommand != null && 
                MouseLeftButtonClickCommand.CanExecute(MouseLeftButtonClickCommandParameter))
            {
                MouseLeftButtonClickCommand.Execute(MouseLeftButtonClickCommandParameter);
            }

            AssociatedObject.ReleaseMouseCapture();
        }

        private void AssociatedObjectOnMouseLeftButtonDown(object sender, MouseButtonEventArgs mouseButtonEventArgs)
        {
            if (IsEnabled)
            {
                _leftMouseDownStart = DateTime.Now;
                AssociatedObject.CaptureMouse();
            }
        }

        private DateTime _rightMouseDownStart = DateTime.Now;

        private void AssociatedObjectOnMouseRightButtonUp(object sender, MouseButtonEventArgs mouseButtonEventArgs)
        {
            var dif = DateTime.Now - _rightMouseDownStart;
            if (dif.TotalMilliseconds < 300 && MouseRightButtonClickCommand != null &&
                MouseRightButtonClickCommand.CanExecute(MouseRightButtonClickCommandParameter))
            {
                MouseRightButtonClickCommand.Execute(MouseRightButtonClickCommandParameter);
            }

            AssociatedObject.ReleaseMouseCapture();
        }

        private void AssociatedObjectOnMouseRightButtonDown(object sender, MouseButtonEventArgs mouseButtonEventArgs)
        {
            if (IsEnabled)
            {
                _rightMouseDownStart = DateTime.Now;
                AssociatedObject.CaptureMouse();
            }
        }
    }
}
