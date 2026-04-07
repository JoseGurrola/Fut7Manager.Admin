using System;
using System.Collections.Generic;
using System.Text;

namespace Fut7Manager.Admin.Helpers {
    public static  class LeagueStatusHelper {
        public static string GetDisplayName(LeagueStatus status) {
            switch (status) {
                case LeagueStatus.Upcoming:
                return "Por iniciar";
                case LeagueStatus.InProgress:
                return "En progreso";
                case LeagueStatus.Playoffs:
                return "Liguilla";
                case LeagueStatus.Finished:
                return "Finalizada";
                default:
                return "Desconocido";
            }
        }
    }
}
