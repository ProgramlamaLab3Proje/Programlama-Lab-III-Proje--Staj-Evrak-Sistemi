using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StajEvrakSistemi.Models;
using StajEvrakSistemi.Services;

namespace StajEvrakSistemi.Pages;

public class StudentModel : PageModel
{
    private readonly InternshipCalendarService _calendarService = new();

    [BindProperty]
    public DateTime? StartDate { get; set; }

    [BindProperty]
    public DateTime? EndDate { get; set; }

    [BindProperty]
    public DateTime? SubmissionDate { get; set; }

    [BindProperty]
    public List<string> SelectedDays { get; set; } = new();

    // Hesaplama sonucunu arayüze taşımak için
    public CalendarCalculationResult? CalculationResult { get; set; }

    public void OnGet()
    {
    }

    public IActionResult OnPost()
    {
        // temel tarihlerin girilip girilmediğini kontrol etmek için
        if (StartDate.HasValue && EndDate.HasValue && SubmissionDate.HasValue)
        {
            // seçilen günleri DayOfWeek türüne çevir
            var selectedDaysEnum = SelectedDays
                .Select(d => Enum.Parse<DayOfWeek>(d))
                .ToList();

            // takvim hesap motorunu çalıştır
            CalculationResult = _calendarService.Calculate(
                StartDate.Value,
                EndDate.Value,
                selectedDaysEnum,
                SubmissionDate.Value
            );
        }

        return Page();
    }
}