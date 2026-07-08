using Emgu.CV;
using Xunit;

namespace VideoKelimeArama.Tests
{
    public class NesneTanimaTestleri
    {
        // Standart test görselinde (otobüs + yayalar) modelin ve YOLOX ızgara
        // çözme kodunun uçtan uca çalıştığını doğrular
        [Fact]
        public void OrnekGoruntude_OtobusVeInsanBulunur()
        {
            string modelYolu = Path.Combine(AppContext.BaseDirectory, "nesne", NesneTanimlayici.ModelDosyaAdi);
            string resimYolu = Path.Combine(AppContext.BaseDirectory, "varlik", "test_otobus.jpg");

            using var tanimlayici = new NesneTanimlayici(modelYolu);
            using Mat kare = CvInvoke.Imread(resimYolu);

            List<NesneTespiti> tespitler = tanimlayici.Tespit(kare);

            Assert.Contains(tespitler, t => t.Sinif == "otobüs");
            Assert.Contains(tespitler, t => t.Sinif == "insan");

            // Kutular görüntünün içinde olmalı
            foreach (NesneTespiti tespit in tespitler)
            {
                Assert.True(tespit.Kutu.Width > 0 && tespit.Kutu.Height > 0);
                Assert.True(tespit.Kutu.Right <= kare.Width * 1.1 && tespit.Kutu.Bottom <= kare.Height * 1.1);
            }
        }

        [Fact]
        public void TespitleriMetneCevir_SayarVeSiralar()
        {
            var tespitler = new List<NesneTespiti>
            {
                new() { Sinif = "insan", Guven = 0.9f },
                new() { Sinif = "araba", Guven = 0.8f },
                new() { Sinif = "insan", Guven = 0.7f }
            };

            Assert.Equal("2 insan, 1 araba", NesneTanimlayici.TespitleriMetneCevir(tespitler));
            Assert.Equal("", NesneTanimlayici.TespitleriMetneCevir(new List<NesneTespiti>()));
        }
    }
}
