using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace ClipboardCrop {
    public partial class PopupSlider : UserControl {
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(
            "Value", typeof(double), typeof(PopupSlider), new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault)
        );

        public static readonly DependencyProperty DefaultProperty = DependencyProperty.Register(
            "Default", typeof(double), typeof(PopupSlider), new PropertyMetadata(0d)
        );

        public static readonly DependencyProperty MinimumProperty = DependencyProperty.Register(
            "Minimum", typeof(double), typeof(PopupSlider), new PropertyMetadata(0d)
        );

        public static readonly DependencyProperty MaximumProperty = DependencyProperty.Register(
            "Maximum", typeof(double), typeof(PopupSlider), new PropertyMetadata(1d)
        );

        public double Value {
            get => (double)GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }

        public double Default {
            get => (double)GetValue(DefaultProperty);
            set => SetValue(DefaultProperty, value);
        }

        public double Minimum {
            get => (double)GetValue(MinimumProperty);
            set => SetValue(MinimumProperty, value);
        }

        public double Maximum {
            get => (double)GetValue(MaximumProperty);
            set => SetValue(MaximumProperty, value);
        }

        public PopupSlider() {
            InitializeComponent();
        }

        private void ResetValue(object sender, RoutedEventArgs e) {
            Value = Default;
        }
    }
}
