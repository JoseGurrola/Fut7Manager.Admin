using Fut7Manager.Admin.Helpers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows.Data;
using System.Windows.Media;

namespace Fut7Manager.Admin.Converters {
    public class LeagueStatusToBrushConverter : IValueConverter {
        public Brush UpcomingBrush { get; set; } = Brushes.OrangeRed;
        public Brush InProgressBrush { get; set; } = Brushes.Yellow;
        public Brush PlayoffsBrush { get; set; } = Brushes.Cyan;
        public Brush FinishedBrush { get; set; } = Brushes.LightGreen;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            if (value is LeagueStatus status) {
                switch (status) {
                    case LeagueStatus.Upcoming:
                    return UpcomingBrush;
                    case LeagueStatus.InProgress:
                    return InProgressBrush;
                    case LeagueStatus.Playoffs:
                    return PlayoffsBrush;
                    case LeagueStatus.Finished:
                    return FinishedBrush;
                }
            }

            return Brushes.Gray;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
}
