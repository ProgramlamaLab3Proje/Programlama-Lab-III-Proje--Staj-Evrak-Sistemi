// JavaScript for StudentRecord view
/**
 * Verilen id'ye sahip input elemanını döndürür (yoksa null).
 * @param {string} id
 * @returns {HTMLInputElement | null}
 */
function getInput(id) {
    return /** @type {HTMLInputElement | null} */ (document.getElementById(id));
}

// Sadece rakam girilebilen kutular (staj gün sayısı, bina no, daire no)
document.querySelectorAll("input.digits-only").forEach(function (el) {
    const digitInput = /** @type {HTMLInputElement} */ (el);
    digitInput.addEventListener("input", function () {
        digitInput.value = digitInput.value.replace(/[^0-9]/g, "");
    });
});

// Tam "gg.aa.yyyy" (10 karakter) ve gerçek bir takvim günü olup olmadığını kontrol eder
/**
 * @param {string} dateStr
 * @returns {Date | null}
 */
function parseStrictTurkishDate(dateStr) {
    if (!dateStr || !/^\d{2}\.\d{2}\.\d{4}$/.test(dateStr)) return null;
    const parts = dateStr.split(".");
    const day = parseInt(parts[0], 10);
    const month = parseInt(parts[1], 10);
    const year = parseInt(parts[2], 10);

    if (year < 1900 || year > 2100 || month < 1 || month > 12 || day < 1 || day > 31) {
        return null;
    }

    const dateObj = new Date(year, month - 1, day);
    if (
        dateObj.getFullYear() !== year ||
        dateObj.getMonth() !== month - 1 ||
        dateObj.getDate() !== day
    ) {
        return null;
    }
    return dateObj;
}

// Hem takvimi gösteren hem de yazarken kendi kendine gün atamasını engelleyen fonksiyon
/**
 * @param {string} elementId
 */
function setupDateField(elementId) {
    const input = getInput(elementId);
    if (!input) return;

    let isTyping = false;

    const fp = /** @type {any} */ (window).flatpickr(input, {
        locale: "tr",
        dateFormat: "d.m.Y",
        allowInput: true,
        disableMobile: true,
        // Yazım ortasında yarım tarihleri kendi kendine tamamlamasını engeller
        parseDate: function (/** @type {string} */ datestr) {
            const validDate = parseStrictTurkishDate(datestr);
            return validDate || undefined;
        },
        onOpen: function () {
            isTyping = false;
        }
    });

    input.addEventListener("keydown", function (e) {
        isTyping = true;
        if (e.key === "Enter" && !parseStrictTurkishDate(input.value)) {
            e.preventDefault();
        }
    });

    // Klavyeden yazarken nokta koyma (yazım ortasında fp.setDate çağırmaz)
    input.addEventListener("input", function () {
        if (!isTyping) return;

        const cursorStart = input.selectionStart || 0;
        const digitsBeforeCursor = input.value.slice(0, cursorStart).replace(/\D/g, "").length;

        let v = input.value.replace(/\D/g, "");
        if (v.length > 8) v = v.slice(0, 8);

        let formatted = "";
        if (v.length > 0) {
            formatted = v.slice(0, 2);
            if (v.length > 2) {
                formatted += "." + v.slice(2, 4);
                if (v.length > 4) {
                    formatted += "." + v.slice(4, 8);
                }
            }
        }
        input.value = formatted;

        let newCursorPos = digitsBeforeCursor;
        if (digitsBeforeCursor > 2) newCursorPos += 1;
        if (digitsBeforeCursor > 4) newCursorPos += 1;
        input.setSelectionRange(newCursorPos, newCursorPos);
    });

    // Sadece kutudan çıkıldığında (blur) geçerli bir tarih varsa takvimi o güne eşitle
    input.addEventListener("blur", function () {
        isTyping = false;
        const validDate = parseStrictTurkishDate(input.value);
        if (validDate) {
            fp.setDate(validDate, false);
        }
    });
}

// 4 Tarih Kutusu tanımlandı
setupDateField("birth_date");
setupDateField("start_date");
setupDateField("end_date");
setupDateField("submission_date");

// Telefon numarası maskesi (+90 5xx xxx xx xx)
const phoneInput = getInput("phone_input");
if (phoneInput) {
    phoneInput.addEventListener("input", function () {
        let digits = phoneInput.value.replace(/\D/g, "");
        if (digits.startsWith("90")) digits = digits.slice(2);
        if (digits.startsWith("0")) digits = digits.slice(1);
        if (digits.length > 10) digits = digits.slice(0, 10);

        let formatted = "";
        if (digits.length > 0) {
            formatted = "+90 " + digits.slice(0, 3);
            if (digits.length > 3) formatted += " " + digits.slice(3, 6);
            if (digits.length > 6) formatted += " " + digits.slice(6, 8);
            if (digits.length > 8) formatted += " " + digits.slice(8, 10);
        }
        phoneInput.value = formatted;
    });
}

// Ek-16 belge boyutu (20 MB) ve format denetimi (Belge seçildiği anda otomatik çalışır)
const fileInput = getInput("UploadedFile");
const clientMessage = document.getElementById("clientMessage");
const maxFileSize = 20 * 1024 * 1024; // 20 MB
const allowedExtensions = [".doc", ".docx", ".pdf", ".jpg", ".jpeg", ".png"];

function validateUploadedFile() {
    if (!fileInput || !clientMessage) return true;

    const file = fileInput.files ? fileInput.files[0] : undefined;
    if (!file) {
        clientMessage.style.display = "none";
        return true;
    }

    const fileName = file.name.toLowerCase();
    const isValidExtension = allowedExtensions.some(ext => fileName.endsWith(ext));

    if (!isValidExtension) {
        clientMessage.className = "alert alert-danger mt-2 py-2 mb-0";
        clientMessage.textContent = "Geçersiz dosya formatı. Lütfen DOC, DOCX, PDF, JPG, JPEG veya PNG yükleyiniz.";
        clientMessage.style.display = "block";
        fileInput.value = "";
        return false;
    }

    if (file.size > maxFileSize) {
        clientMessage.className = "alert alert-danger mt-2 py-2 mb-0";
        clientMessage.textContent = "Dosya boyutu 20 MB sınırını aşamaz.";
        clientMessage.style.display = "block";
        fileInput.value = "";
        return false;
    }

    clientMessage.className = "alert alert-success mt-2 py-2 mb-0";
    clientMessage.textContent = "Belge formatı ve boyutu uygun.";
    clientMessage.style.display = "block";
    return true;
}

if (fileInput) {
    fileInput.addEventListener("change", validateUploadedFile);
}

// "Günleri Hesapla" butonuna tıklandığında üstteki öğrenci bilgisi kutularının zorunluluğunu devre dışı bırak,
// "Başvuruyu Kaydet" butonuna tıklandığında ise tüm öğrenci bilgisi kutularını zorunlu yap
const studentRequiredFields = /** @type {NodeListOf<HTMLElement>} */ (document.querySelectorAll(".student-required"));
const btnCalculate = document.getElementById("btn_calculate");
const btnSave = document.getElementById("btn_save");

// ASP.NET data-val (unobtrusive validation) özniteliklerini ilk yüklemede yedekle ve kaldır ki
// "Günleri Hesapla" butonuna basıldığında sayfayı yukarıdaki kutulara sürüklemesin
studentRequiredFields.forEach(field => {
    if (field.getAttribute("data-val") === "true") {
        field.dataset.wasVal = "true";
        field.setAttribute("data-val", "false");
    }
});

if (btnCalculate) {
    btnCalculate.addEventListener("click", function () {
        studentRequiredFields.forEach(field => {
            field.removeAttribute("required");
            field.setAttribute("data-val", "false");
        });
    });
}

if (btnSave) {
    btnSave.addEventListener("click", function () {
        studentRequiredFields.forEach(field => {
            field.setAttribute("required", "required");
            if (field.dataset.wasVal === "true") {
                field.setAttribute("data-val", "true");
            }
        });
    });
}

// Sayfa yeniden yüklendiğinde (post sonrası) kayıtlı adresi 6 kutuya geri dağıt
const savedAddress = getInput("hidden_full_address");
if (savedAddress && savedAddress.value) {
    const addrParts = savedAddress.value.split(",").map(p => p.trim());
    if (addrParts.length === 6) {
        const addrIds = ["addr_mahalle", "addr_cadde_sokak", "addr_bina", "addr_daire", "addr_ilce", "addr_il"];
        addrParts[2] = addrParts[2].replace(/^No:\s*/i, "");
        addrParts[3] = addrParts[3].replace(/^Daire:\s*/i, "");
        addrIds.forEach((id, i) => {
            const box = getInput(id);
            if (box) box.value = addrParts[i];
        });
    }
}

// 6 Parçalı Adres birleştirme (Mahalle, Cadde/Sokak, Bina No, Daire No, İlçe, İl)
const form = document.getElementById("internship_form");
if (form) {
    form.addEventListener("submit", function () {
        const mahalle = getInput("addr_mahalle")?.value.trim() || "";
        const caddeSokak = getInput("addr_cadde_sokak")?.value.trim() || "";
        const bina = getInput("addr_bina")?.value.trim() || "";
        const daire = getInput("addr_daire")?.value.trim() || "";
        const ilce = getInput("addr_ilce")?.value.trim() || "";
        const il = getInput("addr_il")?.value.trim() || "";

        if (mahalle || caddeSokak || bina || daire || ilce || il) {
            const fullAddress = `${mahalle}, ${caddeSokak}, No: ${bina}, Daire: ${daire}, ${ilce}, ${il}`;
            const hiddenAddr = getInput("hidden_full_address");
            if (hiddenAddr) {
                hiddenAddr.value = fullAddress;
            }
        }
    });
}

// Gün hesaplaması yapıldıysa sayfa yenilendiğinde en üste atmak yerine doğrudan hesaplama kartına odakla
const calcSection = document.getElementById("calculation_section");
if (calcSection && calcSection.dataset.scroll === "true") {
    calcSection.scrollIntoView({ behavior: "smooth", block: "start" });
}
