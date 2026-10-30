using StajEvrakSistemi.Models;

namespace StajEvrakSistemi.Services;

public static class HolidayProvider
{
    public static readonly List<Holiday> Holidays2026_2027 = new()
    {
        // 2026 Güz Dönemi
        new Holiday { Date = new DateTime(2026, 10, 28), Name = "Cumhuriyet Bayramı Arifesi", IsHalfDay = true },
        new Holiday { Date = new DateTime(2026, 10, 29), Name = "Cumhuriyet Bayramı", IsHalfDay = false },
        new Holiday { Date = new DateTime(2027, 1, 1), Name = "Yılbaşı", IsHalfDay = false },

        // 2027 Bahar Dönemi
        new Holiday { Date = new DateTime(2027, 3, 8), Name = "Ramazan Bayramı Arifesi", IsHalfDay = true },
        new Holiday { Date = new DateTime(2027, 3, 9), Name = "Ramazan Bayramı 1. Gün", IsHalfDay = false },
        new Holiday { Date = new DateTime(2027, 3, 10), Name = "Ramazan Bayramı 2. Gün", IsHalfDay = false },
        new Holiday { Date = new DateTime(2027, 3, 11), Name = "Ramazan Bayramı 3. Gün", IsHalfDay = false },
        new Holiday { Date = new DateTime(2027, 4, 23), Name = "Ulusal Egemenlik ve Çocuk Bayramı", IsHalfDay = false },
        new Holiday { Date = new DateTime(2027, 5, 1), Name = "Emek ve Dayanışma Günü", IsHalfDay = false },
        new Holiday { Date = new DateTime(2027, 5, 16), Name = "Kurban Bayramı Arifesi", IsHalfDay = true },
        new Holiday { Date = new DateTime(2027, 5, 17), Name = "Kurban Bayramı 1. Gün", IsHalfDay = false },
        new Holiday { Date = new DateTime(2027, 5, 18), Name = "Kurban Bayramı 2. Gün", IsHalfDay = false },
        new Holiday { Date = new DateTime(2027, 5, 19), Name = "Kurban Bayramı 3. Gün & 19 Mayıs", IsHalfDay = false },
        new Holiday { Date = new DateTime(2027, 5, 20), Name = "Kurban Bayramı 4. Gün", IsHalfDay = false },

        // 2027 Yaz Stajı Dönemi
        new Holiday { Date = new DateTime(2027, 7, 15), Name = "Demokrasi ve Milli Birlik Günü", IsHalfDay = false },
        new Holiday { Date = new DateTime(2027, 8, 30), Name = "Zafer Bayramı", IsHalfDay = false }
    };
}