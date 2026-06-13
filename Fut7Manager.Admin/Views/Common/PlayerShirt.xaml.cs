using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Fut7Manager.Admin.Views.Common {
    public partial class PlayerShirt : UserControl {
        public PlayerShirt() {
            InitializeComponent();
        }

        public Brush ShirtColor
        {
            get => (Brush)GetValue(ShirtColorProperty);
            set => SetValue(ShirtColorProperty, value);
        }

        public static readonly DependencyProperty ShirtColorProperty =
            DependencyProperty.Register(
                nameof(ShirtColor),
                typeof(Brush),
                typeof(PlayerShirt),
                new PropertyMetadata(Brushes.Gray));


        public int? JerseyNumber
        {
            get => (int?)GetValue(JerseyNumberProperty);
            set => SetValue(JerseyNumberProperty, value);
        }

        public static readonly DependencyProperty JerseyNumberProperty =
            DependencyProperty.Register(
                nameof(JerseyNumber),
                typeof(int?),
                typeof(PlayerShirt),
                new PropertyMetadata(null));

        public Brush NumberBrush
        {
            get {
                if (ShirtColor is SolidColorBrush solid) {
                    Color c = solid.Color;

                    double brightness =
                        (c.R * 0.299) +
                        (c.G * 0.587) +
                        (c.B * 0.114);

                    return brightness > 180
                        ? Brushes.Black
                        : Brushes.White;
                }

                return Brushes.White;
            }
        }
    }
}