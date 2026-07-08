using System.Globalization;

namespace VideoKelimeArama
{
    // Saniyeyi dd:ss biçiminde yazar
    internal static class Sure
    {
        public static string Formatla(double saniye)
        {
            int dakika = (int)saniye / 60;
            int sn = (int)saniye % 60;
            return $"{dakika:D2}:{sn:D2}";
        }
    }

    // Arama sonuçları ListBox'a bu nesne olarak eklenir; tıklamada metin
    // ayrıştırmak yerine doğrudan kare numarasına atlanır.
    internal sealed class AramaSonucu
    {
        public string Kelime { get; init; } = "";
        public int KareNo { get; init; }
        public double Saniye { get; init; }
        public string Baglam { get; init; } = "";

        public override string ToString()
            => $"{Sure.Formatla(Saniye)}  ——>  {(Baglam.Length > 0 ? Baglam : Kelime)}";
    }

    // OCR/ses dizininde (önbellek dosyasında) taranan tek karenin kaydı
    internal sealed class OcrKayit
    {
        public int KareNo { get; set; }
        public double Saniye { get; set; }
        public string Metin { get; set; } = "";
    }

    // Video yanına kaydedilen dizin (görüntü için ".ocr.json", ses için
    // ".asr.json"); sonraki aramalar OCR/Whisper çalıştırmadan bu
    // metinler üzerinde yapılır
    internal sealed class OcrDizin
    {
        // Tarama/eşleştirme algoritması değiştiğinde artırılır; sürümü
        // tutmayan eski dizinler sessizce eski sonuç vermesin diye geçersiz
        // sayılır ve video yeniden taranır
        public const int GuncelSurum = 2;

        public int Surum { get; set; }
        public long DosyaBoyutu { get; set; }
        public DateTime DosyaDegisme { get; set; }
        public double Fps { get; set; }
        public int ToplamKare { get; set; }
        public List<OcrKayit> Kayitlar { get; set; } = new();
    }

    // Kayıtları sırayla değerlendirir: Türkçe harf duyarsız eşleşme,
    // OCR hatalarına toleranslı bulanık eşleşme ve ekranda kalan
    // kelimenin ardışık karelerini tek sonuç sayma
    internal sealed class KelimeEslestirici
    {
        private readonly string kelime;
        private readonly int bulanikEsik;
        private readonly CultureInfo kultur = CultureInfo.GetCultureInfo("tr-TR");
        private double sonBulunanSaniye = double.NegativeInfinity;

        public int BulunanSayisi { get; private set; }

        public KelimeEslestirici(string kelime)
        {
            this.kelime = kelime;
            // Kısa kelimede tam eşleşme; uzadıkça OCR için 1-2 harf hata payı
            bulanikEsik = kelime.Any(char.IsWhiteSpace) || kelime.Length <= 4 ? 0
                        : kelime.Length <= 8 ? 1 : 2;
        }

        public AramaSonucu? Degerlendir(OcrKayit kayit)
        {
            string? baglam = EslesmeBul(kayit.Metin);
            if (baglam == null)
            {
                return null;
            }

            bool yeniGorunum = kayit.Saniye - sonBulunanSaniye > 2.0;
            sonBulunanSaniye = kayit.Saniye;
            if (!yeniGorunum)
            {
                return null;
            }

            BulunanSayisi++;
            return new AramaSonucu { Kelime = kelime, KareNo = kayit.KareNo, Saniye = kayit.Saniye, Baglam = baglam };
        }

        // Eşleşme varsa kelimenin geçtiği satırı (bağlamı) döndürür, yoksa null
        private string? EslesmeBul(string metin)
        {
            foreach (string hamSatir in metin.Split('\n'))
            {
                string satir = hamSatir.Trim();
                if (satir.Length == 0)
                {
                    continue;
                }

                bool eslesti = kultur.CompareInfo.IndexOf(satir, kelime, CompareOptions.IgnoreCase) >= 0;

                if (!eslesti && bulanikEsik > 0)
                {
                    foreach (string parca in satir.Split(' ', StringSplitOptions.RemoveEmptyEntries))
                    {
                        if (SozcukUyuyorMu(parca))
                        {
                            eslesti = true;
                            break;
                        }
                    }
                }

                if (eslesti)
                {
                    return BaglamKisalt(satir);
                }
            }

            return null;
        }

        // Tek bir sözcüğün aranan kelimeyle uyuşup uyuşmadığı (bulanık dahil);
        // kare üzerinde kelime vurgulama da bunu kullanır
        public bool SozcukUyuyorMu(string sozcuk)
        {
            sozcuk = sozcuk.Trim('.', ',', ';', ':', '!', '?', '"', '\'', '(', ')', '[', ']');
            if (sozcuk.Length == 0)
            {
                return false;
            }

            if (kultur.CompareInfo.IndexOf(sozcuk, kelime, CompareOptions.IgnoreCase) >= 0)
            {
                return true;
            }

            return bulanikEsik > 0 &&
                   Math.Abs(sozcuk.Length - kelime.Length) <= bulanikEsik &&
                   LevenshteinMesafesi(kultur.TextInfo.ToLower(sozcuk), kultur.TextInfo.ToLower(kelime)) <= bulanikEsik;
        }

        // Satırdaki fazla boşlukları teke indirir, uzunsa sonunu kırpar
        private static string BaglamKisalt(string satir)
        {
            satir = string.Join(' ', satir.Split(' ', StringSplitOptions.RemoveEmptyEntries));
            return satir.Length <= 60 ? satir : satir[..57] + "...";
        }

        // İki sözcük arasındaki düzenleme (ekle/sil/değiştir) mesafesi
        private static int LevenshteinMesafesi(string a, string b)
        {
            int[] onceki = new int[b.Length + 1];
            int[] simdiki = new int[b.Length + 1];
            for (int j = 0; j <= b.Length; j++)
            {
                onceki[j] = j;
            }

            for (int i = 1; i <= a.Length; i++)
            {
                simdiki[0] = i;
                for (int j = 1; j <= b.Length; j++)
                {
                    int maliyet = a[i - 1] == b[j - 1] ? 0 : 1;
                    simdiki[j] = Math.Min(Math.Min(simdiki[j - 1] + 1, onceki[j] + 1), onceki[j - 1] + maliyet);
                }
                (onceki, simdiki) = (simdiki, onceki);
            }

            return onceki[b.Length];
        }
    }
}
