using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace Fut7Manager.Admin.Controls {
    public class LoadingContainer : ContentControl {
        static LoadingContainer() {
            DefaultStyleKeyProperty.OverrideMetadata(
                typeof(LoadingContainer),
                new FrameworkPropertyMetadata(typeof(LoadingContainer)));
        }

        public static readonly DependencyProperty IsLoadingProperty =
            DependencyProperty.Register(nameof(IsLoading), typeof(bool), typeof(LoadingContainer), new PropertyMetadata(false));

        public bool IsLoading
        {
            get => (bool)GetValue(IsLoadingProperty);
            set => SetValue(IsLoadingProperty, value);
        }
    }
}
