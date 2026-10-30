using StajEvrakSistemi.Models;

namespace StajEvrakSistemi.Services;

public class InternshipCalendarService
{
    public CalendarCalculationResult Calculate(
        DateTime startDate,
        DateTime endDate,
        List<DayOfWeek> selectedDays,
        DateTime submissionDate,
        int declaredDaysInForm = 0)
    {
        var result = new CalendarCalculationResult();

        // 30 Gün Önce Teslim Kontrolü
        var daysDifference = (startDate.Date - submissionDate.Date).TotalDays;
        if (daysDifference < 30)
        {
            result.Is30DayRuleValid = false;
            result.SuggestedStartDate = submissionDate.Date.AddDays(30);
            result.ErrorMessages.Add($"Staj evrakı başlangıçtan en az 30 gün önce teslim edilmelidir. Önerilen başlangıç tarihi: {result.SuggestedStartDate:dd.MM.yyyy}");
        }
        else
        {
            result.Is30DayRuleValid = true;
        }

        // Seçili günleri tara ve resmi tatilleri düş
        int netDays = 0;
        var holidays = HolidayProvider.Holidays2026_2027;

        for (var date = startDate.Date; date <= endDate.Date; date = date.AddDays(1))
        {
            // Öğrencinin seçtiği haftalık günlerden biri mi?
            if (!selectedDays.Contains(date.DayOfWeek))
                continue;

            // Resmi tatil mi? (Tam gün tatiller doğrudan düşülür)
            var holiday = holidays.FirstOrDefault(h => h.Date == date);
            if (holiday != null && !holiday.IsHalfDay)
                continue;

            netDays++;
        }

        result.CalculatedWorkDays = netDays;

        // Formdaki beyan edilen gün sayısı ile eşit mi?
        if (declaredDaysInForm > 0 && netDays != declaredDaysInForm)
        {
            result.ErrorMessages.Add($"Hesaplanan staj iş günü ({netDays}) ile formda beyan edilen gün sayısı ({declaredDaysInForm}) uyuşmuyor!");
        }

        return result;
    }
}