using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;
using System.Drawing;

namespace VideoKelimeArama
{
    // Bir karede bulunan tek nesne
    internal sealed class NesneTespiti
    {
        public string Sinif { get; init; } = "";
        public float Guven { get; init; }
        public RectangleF Kutu { get; init; } // kare (video) koordinatlarında
    }

    // YOLOX-nano (Apache 2.0) ile çevrimdışı nesne tespiti.
    // Model ham çıktı verir; ızgara çözme ve NMS burada yapılır.
    internal sealed class NesneTanimlayici : IDisposable
    {
        public const string ModelDosyaAdi = "yolox_nano.onnx";
        public const string ModelAdresi =
            "https://github.com/Megvii-BaseDetection/YOLOX/releases/download/0.1.1rc0/yolox_nano.onnx";

        private const int GirisBoyut = 416;
        private const int SinifSayisi = 80;

        private readonly InferenceSession oturum;
        private readonly string girisAdi;

        // COCO sınıfları (model sırasıyla) ve Türkçe karşılıkları
        public static readonly string[] SinifAdlari =
        {
            "insan", "bisiklet", "araba", "motosiklet", "uçak", "otobüs", "tren", "kamyon",
            "tekne", "trafik lambası", "yangın musluğu", "dur tabelası", "parkmetre", "bank",
            "kuş", "kedi", "köpek", "at", "koyun", "inek", "fil", "ayı", "zebra", "zürafa",
            "sırt çantası", "şemsiye", "el çantası", "kravat", "valiz", "frizbi", "kayak",
            "snowboard", "top", "uçurtma", "beyzbol sopası", "beyzbol eldiveni", "kaykay",
            "sörf tahtası", "tenis raketi", "şişe", "kadeh", "bardak", "çatal", "bıçak",
            "kaşık", "kase", "muz", "elma", "sandviç", "portakal", "brokoli", "havuç",
            "sosisli", "pizza", "çörek", "pasta", "sandalye", "koltuk", "saksı bitkisi",
            "yatak", "yemek masası", "klozet", "televizyon", "dizüstü bilgisayar", "fare",
            "kumanda", "klavye", "cep telefonu", "mikrodalga", "fırın", "ekmek kızartıcı",
            "lavabo", "buzdolabı", "kitap", "saat", "vazo", "makas", "oyuncak ayı",
            "saç kurutma makinesi", "diş fırçası"
        };

        static NesneTanimlayici()
        {
            // Windows, System32'de eski bir onnxruntime.dll taşır (Windows ML).
            // Yanlış sürümün sürece girmemesi için paketle gelen kitaplığı
            // önceden elle yükleriz; ad ile yapılan sonraki yüklemeler artık
            // bu modülü bulur.
            string yerel = System.IO.Path.Combine(AppContext.BaseDirectory,
                "runtimes", "win-x64", "native", "onnxruntime.dll");
            if (System.IO.File.Exists(yerel))
            {
                try { System.Runtime.InteropServices.NativeLibrary.Load(yerel); } catch { }
            }
        }

        public NesneTanimlayici(string modelYolu)
        {
            oturum = new InferenceSession(modelYolu);
            girisAdi = oturum.InputMetadata.Keys.First();
        }

        public List<NesneTespiti> Tespit(Mat kare, float guvenEsigi = 0.4f)
        {
            // Kareyi en-boy oranını koruyarak 416x416 tuvale yerleştir (letterbox)
            float olcek = Math.Min((float)GirisBoyut / kare.Width, (float)GirisBoyut / kare.Height);
            int genislik = Math.Max(1, (int)(kare.Width * olcek));
            int yukseklik = Math.Max(1, (int)(kare.Height * olcek));

            using var kucuk = new Mat();
            CvInvoke.Resize(kare, kucuk, new Size(genislik, yukseklik));

            using var tuval = new Mat(GirisBoyut, GirisBoyut, DepthType.Cv8U, 3);
            tuval.SetTo(new MCvScalar(114, 114, 114));
            using (var bolge = new Mat(tuval, new Rectangle(0, 0, genislik, yukseklik)))
            {
                kucuk.CopyTo(bolge);
            }

            // YOLOX girişi: BGR, 0-255 ham değerler, CHW düzeni
            using var goruntu = tuval.ToImage<Bgr, byte>();
            byte[,,] piksel = goruntu.Data;
            var tensor = new DenseTensor<float>(new[] { 1, 3, GirisBoyut, GirisBoyut });
            for (int y = 0; y < GirisBoyut; y++)
            {
                for (int x = 0; x < GirisBoyut; x++)
                {
                    tensor[0, 0, y, x] = piksel[y, x, 0];
                    tensor[0, 1, y, x] = piksel[y, x, 1];
                    tensor[0, 2, y, x] = piksel[y, x, 2];
                }
            }

            var girisler = new List<NamedOnnxValue> { NamedOnnxValue.CreateFromTensor(girisAdi, tensor) };
            using var sonuc = oturum.Run(girisler);
            float[] cikti = sonuc.First().AsEnumerable<float>().ToArray();

            // Izgara çözme: çıktı [1, 3549, 85] — stride 8/16/32 hücreleri art arda
            const int kanal = 5 + SinifSayisi;
            var adaylar = new List<NesneTespiti>();
            int hucre = 0;
            foreach (int adim in new[] { 8, 16, 32 })
            {
                int izgara = GirisBoyut / adim;
                for (int gy = 0; gy < izgara; gy++)
                {
                    for (int gx = 0; gx < izgara; gx++, hucre++)
                    {
                        int b = hucre * kanal;
                        float nesnelik = cikti[b + 4];
                        if (nesnelik < 0.1f)
                        {
                            continue;
                        }

                        int enIyiSinif = -1;
                        float enIyiSkor = 0f;
                        for (int s = 0; s < SinifSayisi; s++)
                        {
                            float skor = cikti[b + 5 + s];
                            if (skor > enIyiSkor)
                            {
                                enIyiSkor = skor;
                                enIyiSinif = s;
                            }
                        }

                        float guven = nesnelik * enIyiSkor;
                        if (guven < guvenEsigi || enIyiSinif < 0)
                        {
                            continue;
                        }

                        float merkezX = (cikti[b] + gx) * adim;
                        float merkezY = (cikti[b + 1] + gy) * adim;
                        float w = MathF.Exp(cikti[b + 2]) * adim;
                        float h = MathF.Exp(cikti[b + 3]) * adim;

                        // Letterbox ölçeğini geri al (dolgu sol üstte, kaydırma yok)
                        adaylar.Add(new NesneTespiti
                        {
                            Sinif = SinifAdlari[enIyiSinif],
                            Guven = guven,
                            Kutu = new RectangleF(
                                (merkezX - w / 2) / olcek, (merkezY - h / 2) / olcek,
                                w / olcek, h / olcek)
                        });
                    }
                }
            }

            return CakisanlariAyikla(adaylar, 0.45f);
        }

        // Aynı nesneye ait üst üste kutulardan en güvenilir olanı bırakır (NMS)
        private static List<NesneTespiti> CakisanlariAyikla(List<NesneTespiti> adaylar, float esik)
        {
            var secilenler = new List<NesneTespiti>();
            foreach (var aday in adaylar.OrderByDescending(t => t.Guven))
            {
                bool ortusuyor = secilenler.Any(s => s.Sinif == aday.Sinif && KesisimOrani(s.Kutu, aday.Kutu) > esik);
                if (!ortusuyor)
                {
                    secilenler.Add(aday);
                }
            }

            return secilenler;
        }

        private static float KesisimOrani(RectangleF a, RectangleF b)
        {
            var kesisim = RectangleF.Intersect(a, b);
            if (kesisim.IsEmpty)
            {
                return 0f;
            }

            float kesisimAlan = kesisim.Width * kesisim.Height;
            return kesisimAlan / (a.Width * a.Height + b.Width * b.Height - kesisimAlan);
        }

        // Tespit listesini dizin metnine çevirir: "2 insan, 1 araba"
        public static string TespitleriMetneCevir(List<NesneTespiti> tespitler)
        {
            if (tespitler.Count == 0)
            {
                return "";
            }

            return string.Join(", ", tespitler
                .GroupBy(t => t.Sinif)
                .OrderByDescending(g => g.Count())
                .Select(g => $"{g.Count()} {g.Key}"));
        }

        public void Dispose() => oturum.Dispose();
    }
}
