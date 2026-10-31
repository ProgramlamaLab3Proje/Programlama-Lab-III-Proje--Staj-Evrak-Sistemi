namespace StajEvrakSistemi.Models;

public class CalendarCalculationResult
{
    // Tatiller ve seçili olmayan günler düşüldükten sonra kalan net iş günü
    public int CalculatedWorkDays { get; set; }

    // Staj başlangıcına en az 30 gün var mı?
    public bool Is30DayRuleValid { get; set; }

    // 30 gün kuralı ihlal edildiyse sistemin önerdiği en erken başlangıç tarihi
    public DateTime? SuggestedStartDate { get; set; }

    // Kural ihlalleri varsa hata mesajları listesi
    // Kural ihlalleri varsa hata mesajları listesi
    public List<string> ErrorMessages { get; set; } = new();

    // Hiçbir hata yoksa başvuru tarihsel olarak geçerlidir
    public bool IsValid => Is30DayRuleValid && ErrorMessages.Count == 0;
}