using Xunit;

namespace VideoKelimeArama.Tests
{
    public class KelimeEslestiriciTestleri
    {
        private static OcrKayit Kayit(string metin, double saniye = 0, int kareNo = 0)
            => new() { Metin = metin, Saniye = saniye, KareNo = kareNo };

        [Fact]
        public void TamEslesme_SonucuZamanVeBaglamlaDondurur()
        {
            var eslestirici = new KelimeEslestirici("nato");

            AramaSonucu? sonuc = eslestirici.Degerlendir(Kayit("NATO LİDERLER ZİRVESİ", saniye: 10, kareNo: 250));

            Assert.NotNull(sonuc);
            Assert.Equal(10, sonuc!.Saniye);
            Assert.Equal(250, sonuc.KareNo);
            Assert.Equal("NATO LİDERLER ZİRVESİ", sonuc.Baglam);
        }

        [Fact]
        public void TurkceHarfDuyarsizligi_NoktaliBuyukIileEslesir()
        {
            // tr-TR kurallarında "İSTANBUL"un küçüğü "istanbul"dur
            var eslestirici = new KelimeEslestirici("istanbul");

            Assert.NotNull(eslestirici.Degerlendir(Kayit("İSTANBUL BOĞAZI")));
        }

        [Fact]
        public void KisaKelimede_BulanikEslesme_Devrede_Degil()
        {
            var eslestirici = new KelimeEslestirici("nato"); // 4 harf: yalnızca tam eşleşme

            Assert.Null(eslestirici.Degerlendir(Kayit("NAFO ZİRVESİ")));
        }

        [Fact]
        public void OrtaUzunlukta_BirHarfHata_Yakalanir()
        {
            var eslestirici = new KelimeEslestirici("ankara"); // 5-8 harf: 1 harf payı

            Assert.NotNull(eslestirici.Degerlendir(Kayit("ANKERA TOPLANTISI")));
        }

        [Fact]
        public void OrtaUzunlukta_IkiHarfHata_Reddedilir()
        {
            var eslestirici = new KelimeEslestirici("ankara");

            Assert.Null(eslestirici.Degerlendir(Kayit("ANKORE TOPLANTISI")));
        }

        [Fact]
        public void UzunKelimede_IkiHarfHata_Yakalanir()
        {
            var eslestirici = new KelimeEslestirici("cumhurbaskani"); // 8+ harf: 2 harf payı

            Assert.True(eslestirici.SozcukUyuyorMu("cumhurbeskeni"));
        }

        [Fact]
        public void BoslukluAramada_Bulanik_Devrede_Degil()
        {
            Assert.NotNull(new KelimeEslestirici("nato zirvesi")
                .Degerlendir(Kayit("BUGÜN NATO ZİRVESİ BAŞLADI")));

            Assert.Null(new KelimeEslestirici("nato zirvesi")
                .Degerlendir(Kayit("BUGÜN NATO ZİRVESE BAŞLADI")));
        }

        [Fact]
        public void ArdisikEslesmeler_TekSonucSayilir()
        {
            var eslestirici = new KelimeEslestirici("nato");

            Assert.NotNull(eslestirici.Degerlendir(Kayit("NATO", saniye: 10)));
            Assert.Null(eslestirici.Degerlendir(Kayit("NATO", saniye: 11)));    // 2 sn penceresinde
            Assert.Null(eslestirici.Degerlendir(Kayit("NATO", saniye: 12.5))); // pencere kaydıkça uzar
            Assert.NotNull(eslestirici.Degerlendir(Kayit("NATO", saniye: 20)));

            Assert.Equal(2, eslestirici.BulunanSayisi);
        }

        [Fact]
        public void Baglam_KelimeninGectigiSatirdanGelir()
        {
            var eslestirici = new KelimeEslestirici("nato");

            AramaSonucu? sonuc = eslestirici.Degerlendir(Kayit("İLK SATIR\nNATO ZİRVESİ\nSON SATIR"));

            Assert.Equal("NATO ZİRVESİ", sonuc!.Baglam);
        }

        [Fact]
        public void Baglam_FazlaBosluklarTeklenir_UzunSatirKirpilir()
        {
            var eslestirici = new KelimeEslestirici("nato");
            string uzunSatir = "NATO   " + string.Join("  ", Enumerable.Repeat("kelime", 20));

            AramaSonucu? sonuc = eslestirici.Degerlendir(Kayit(uzunSatir));

            Assert.NotNull(sonuc);
            Assert.DoesNotContain("  ", sonuc!.Baglam);
            Assert.True(sonuc.Baglam.Length <= 60);
            Assert.EndsWith("...", sonuc.Baglam);
        }

        [Fact]
        public void SozcukUyuyorMu_NoktalamayiTemizler_EkleriKapsar()
        {
            var eslestirici = new KelimeEslestirici("nato");

            Assert.True(eslestirici.SozcukUyuyorMu("'NATO'"));
            Assert.True(eslestirici.SozcukUyuyorMu("NATO'NUN")); // ek almış hali alt dizi olarak
            Assert.False(eslestirici.SozcukUyuyorMu("OTAN"));
            Assert.False(eslestirici.SozcukUyuyorMu(""));
        }
    }
}
