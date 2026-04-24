using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Fut7Manager.Admin.Views.Common {
    /// <summary>
    /// Interaction logic for LoadingSpinner.xaml
    /// </summary>
    /// 
    /*modo se uso basico
        <common:LoadingSpinner Width="50"
                       Height="50"
                       Thickness="5"
                       SpinnerBrush="#10B981"
                       TrackBrush="#E5E7EB"/>
    
    modo de uso completo
        <Grid>

        <!-- CONTENIDO -->
        <Grid>
            <!-- tu UI -->
        </Grid>

        <!-- LOADING -->
        <Border Background="#80FFFFFF"
                Visibility="{Binding IsLoading, Converter={StaticResource BoolToVisibilityConverter}}">

            <StackPanel HorizontalAlignment="Center"
                        VerticalAlignment="Center">

                <common:LoadingSpinner Width="50" Height="50"/>

                <TextBlock Text="Cargando..."
                           Margin="0,10,0,0"
                           Foreground="Gray"
                           HorizontalAlignment="Center"/>
            </StackPanel>

        </Border>

    </Grid>
    */
    public partial class LoadingSpinner : UserControl {
        public LoadingSpinner() {
            InitializeComponent();
        }

        // 🎨 Color del spinner
        public Brush SpinnerBrush
        {
            get => (Brush)GetValue(SpinnerBrushProperty);
            set => SetValue(SpinnerBrushProperty, value);
        }

        public static readonly DependencyProperty SpinnerBrushProperty =
            DependencyProperty.Register(nameof(SpinnerBrush), typeof(Brush), typeof(LoadingSpinner),
                new PropertyMetadata(new SolidColorBrush((Color)ColorConverter.ConvertFromString("#10B981"))));

        // 🎨 Color del fondo (track)
        public Brush TrackBrush
        {
            get => (Brush)GetValue(TrackBrushProperty);
            set => SetValue(TrackBrushProperty, value);
        }

        public static readonly DependencyProperty TrackBrushProperty =
            DependencyProperty.Register(nameof(TrackBrush), typeof(Brush), typeof(LoadingSpinner),
                new PropertyMetadata(new SolidColorBrush((Color)ColorConverter.ConvertFromString("#E5E7EB"))));

        // 📏 Grosor
        public double Thickness
        {
            get => (double)GetValue(ThicknessProperty);
            set => SetValue(ThicknessProperty, value);
        }

        public static readonly DependencyProperty ThicknessProperty =
            DependencyProperty.Register(nameof(Thickness), typeof(double), typeof(LoadingSpinner),
                new PropertyMetadata(4.0));
    }
}
