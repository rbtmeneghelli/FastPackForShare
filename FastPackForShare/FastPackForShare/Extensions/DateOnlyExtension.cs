using System.Collections.Frozen;
using System.Globalization;

namespace FastPackForShare.Extensions;

public sealed class DateOnlyExtension
{
    #region [Aplicando padrão Singleton do Design Pattern]

    private static readonly DateOnlyExtension Instance = new DateOnlyExtension();

    public static DateOnlyExtension GetLoadDateOnlyService()
    {
        return Instance;
    }

    #endregion

    public DateOnly GetDate() => DateOnly.FromDateTime(GetDateTimeNowFromBrazil());
    public DateOnly SetDateOnly(int year, int month, int day) => new DateOnly(year, month, day);
    public DateOnly ConvertDateTimeToDateOnly(DateTime dateTime) => DateOnly.FromDateTime(dateTime);
    public int NumberDaysOfLife(DateOnly birthDay) => GetDate().DayNumber - birthDay.DayNumber;
    public int GetAgeByDays(DateOnly birthDay) => Math.Abs(NumberDaysOfLife(birthDay) / 365);
    public int GetAgeByYear(DateOnly birthDay) => Math.Abs(GetDate().Year - birthDay.Year);
    public DateTime GetDateTimeFromString(string dateTime) => DateTime.ParseExact(dateTime, "yyyy-MM-dd", CultureInfo.InvariantCulture);

    public static DateTime FirstDayCurrentMonth()
    {
        return new DateTime(GetDateTimeNowFromBrazil().Year, GetDateTimeNowFromBrazil().Month, 1);
    }

    public DateTime GetNextUtilDay(DateTime dateTime)
    {
        // Caso tenha feriado nacional ou internacional, fazer uma consulta no BD pra isso...depois um IF para validar e somar 1 dia...
        var dictionary = new Dictionary<DayOfWeek, DateTime>
        {
            { DayOfWeek.Saturday, dateTime.AddDays(2) },
            { DayOfWeek.Sunday, dateTime.AddDays(1) }
        }.ToFrozenDictionary();

        return dictionary.TryGetValue(dateTime.DayOfWeek, out var dtResult) ? dtResult : dateTime;
    }

    public DateTime GetCurrentUtilDay()
    {
        return GetNextUtilDay(FirstDayCurrentMonth().AddDays(4));
    }

    /// <summary>
    /// Primeiro irei Obter a data e hora atual em GMT,
    /// Definir o fuso horário de São Paulo,
    /// Converte a data e hora atual para o fuso horário de São Paulo
    /// </summary>
    /// <returns></returns>
    public static DateTime GetDateTimeNowFromBrazil()
    {
        TimeZoneInfo tz = TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time");
        DateTime dateTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow.ToUniversalTime(), tz);
        return dateTime;
    }

    public static string GetShortDate()
    {
        return GetDateTimeNowFromBrazil().ToShortDateString();
    }

    public static string GetShortTime()
    {
        return GetDateTimeNowFromBrazil().ToShortTimeString();
    }

    public static int GetLastDayOfMonth()
    {
        return DateTime.DaysInMonth(GetDateTimeNowFromBrazil().Year, GetDateTimeNowFromBrazil().Month);
    }

    public static bool IsValidUntilDay(DateTime date)
    {
        IEnumerable<DateTime> holidayDaysOfYear = Enumerable.Empty<DateTime>();
        return date.DayOfWeek != DayOfWeek.Saturday &&
               date.DayOfWeek != DayOfWeek.Sunday &&
               !holidayDaysOfYear.Contains(date);
    }

    /// <summary>
    /// Esse metodo faz que seja retornado o 5º dia util valido do Mês
    /// </summary>
    /// <param name="holidayDaysOfYear"> Lista com datas que são feriados nacionais, locais ou etc... </param>
    /// <param name="untilDay"> Dia Util</param>
    /// <returns></returns>
    public static DateTime GetValidDayToPay(int untilDay = 5)
    {
        int untilDayCount = 0;
        int lastDayOfMonth = GetLastDayOfMonth();
        DateTime firstDayOfMonth = FirstDayCurrentMonth();

        while (untilDayCount < untilDay)
        {
            if (IsValidUntilDay(firstDayOfMonth))
            {
                untilDayCount++;
            }

            firstDayOfMonth = firstDayOfMonth.AddDays(1);

            if (firstDayOfMonth.Day > lastDayOfMonth)
            {
                break;
            }
        }

        return firstDayOfMonth.AddDays(-1);
    }

    public static bool IsAdultPerson(DateTime birthDate) => birthDate <= GetDateTimeNowFromBrazil().AddYears(-18);

    public static DateTime ConverterStringParaDateTime(string date, string format) => DateTime.ParseExact(date, format, System.Globalization.CultureInfo.InvariantCulture);

    public static string GetStringFromDateTime(DateTime dateTime)
    {
        return dateTime.ToString("yyyy-MM-dd");
    }

    public static string GetElapsedTime(DateTime? data)
    {
        if (data == null)
            return "sem data";

        var dataAgora = GetDateTimeNowFromBrazil();

        var dataInformada = data.Value;

        if (dataInformada > dataAgora)
            dataInformada = dataAgora;

        var minutos = (int)(dataAgora - dataInformada).TotalMinutes;

        if (minutos < 60)
            return $"{minutos} {(minutos == 1 ? "minuto" : "minutos")} atrás";

        var horas = (int)(dataAgora - dataInformada).TotalHours;
        if (horas < 24)
            return $"{horas} {(horas == 1 ? "hora" : "horas")} atrás";

        var dias = (int)(dataAgora - dataInformada).TotalDays;
        if (dias < 30)
            return $"{dias} {(dias == 1 ? "dia" : "dias")} atrás";

        var meses = ((dataAgora.Year - dataInformada.Year) * 12) + (dataAgora.Month - dataInformada.Month);
        if (meses < 12)
            return $"{meses} {(meses == 1 ? "mês" : "meses")} atrás";

        var anos = dataAgora.Year - dataInformada.Year;
        return $"{anos} {(anos == 1 ? "ano" : "anos")} atrás";
    }
}
