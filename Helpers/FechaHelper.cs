namespace PublicidadDinamicaWeb.Helpers
{
    // Esto ayuda a formatear las fechas en la zona horaria de Costa Rica, ya que el servidor puede estar en una zona horaria diferente.
    public static class FechaHelper
    {
        private static readonly TimeZoneInfo CostaRicaTimeZone =
            TimeZoneInfo.FindSystemTimeZoneById("Central America Standard Time");

        public static DateTime AHoraCostaRica(DateTime fechaUtc)
        {
            return TimeZoneInfo.ConvertTimeFromUtc(fechaUtc, CostaRicaTimeZone);
        }

        public static string FormatoCR(DateTime fechaUtc)
        {
            var fecha = AHoraCostaRica(fechaUtc);
            return fecha.ToString("dd/MM/yyyy hh:mm tt");
        }

        public static string SoloFechaCR(DateTime fechaUtc)
        {
            var fecha = AHoraCostaRica(fechaUtc);
            return fecha.ToString("dd/MM/yyyy");
        }
    }
}