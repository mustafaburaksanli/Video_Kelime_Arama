using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using System;
using System.Drawing;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Windows.Forms;
using NAudio.Wave;
using Tesseract;
using Whisper.net;

namespace VideoKelimeArama
{
    public partial class Form1 : Form
    {
        // Gerekli değişkenlerin tanımlanması
        private VideoCapture? yakalama;          // Oynatma için video yakalama nesnesi
        private MediaFoundationReader? sesOkuyucu; // Videonun ses izi
        private WaveOutEvent? sesCihazi;         // Sesi hoparlöre çalan cihaz

        // Oynatma saati: kare seçimi bu pürüzsüz saate göre yapılır; ses
        // akışının sıçramalı konumu yalnızca büyük sapmada düzeltme için okunur
        private readonly System.Diagnostics.Stopwatch oynatmaSaati = new();
        private double oynatmaSaatiTaban; // saat sıfırlandığında medyanın bulunduğu saniye
        private float sesDuzeyi = 1f;     // 0..1; yeni video açılınca da korunur
        private bool isDragging = false;
        private int previousProgressBarValue = -1;
        private CancellationTokenSource? aramaCts;
        public string? secilenVideoYolu;

        // Arama sonuçları ListBox'a bu nesne olarak eklenir; tıklamada metin
        // ayrıştırmak yerine doğrudan kare numarasına atlanır.
        private sealed class AramaSonucu
        {
            public string Kelime { get; init; } = "";
            public int KareNo { get; init; }
            public double Saniye { get; init; }
            public string Baglam { get; init; } = "";

            public override string ToString()
                => $"{SureFormatla(Saniye)}  ——>  {(Baglam.Length > 0 ? Baglam : Kelime)}";
        }

        // OCR dizininde (önbellek dosyasında) taranan tek karenin kaydı
        private sealed class OcrKayit
        {
            public int KareNo { get; set; }
            public double Saniye { get; set; }
            public string Metin { get; set; } = "";
        }

        // Video yanına kaydedilen dizin (görüntü için ".ocr.json", ses için
        // ".asr.json"); sonraki aramalar OCR/Whisper çalıştırmadan bu
        // metinler üzerinde yapılır
        // Tarama/eşleştirme algoritması değiştiğinde artırılır; sürümü
        // tutmayan eski dizinler sessizce eski sonuç vermesin diye geçersiz
        // sayılır ve video yeniden taranır
        private const int DizinSurumu = 2;

        private sealed class OcrDizin
        {
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
        private sealed class KelimeEslestirici
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
                        string kucukKelime = kultur.TextInfo.ToLower(kelime);
                        foreach (string parca in satir.Split(' ', StringSplitOptions.RemoveEmptyEntries))
                        {
                            string sozcuk = parca.Trim('.', ',', ';', ':', '!', '?', '"', '\'', '(', ')', '[', ']');
                            if (Math.Abs(sozcuk.Length - kelime.Length) <= bulanikEsik &&
                                LevenshteinMesafesi(kultur.TextInfo.ToLower(sozcuk), kucukKelime) <= bulanikEsik)
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

        public Form1()
        {
            InitializeComponent();

            progressBarvideo.MouseDown += ProgressBar_MouseDown;
            progressBarvideo.MouseMove += ProgressBar_MouseMove;
            progressBarvideo.MouseUp += ProgressBar_MouseUp;

            trbSes.MouseDown += SesCubugu_Fare;
            trbSes.MouseMove += SesCubugu_Fare;
        }

        // Ses düzeyi çubuğuna tıklama/sürükleme
        private void SesCubugu_Fare(object? sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left)
            {
                return;
            }

            int deger = Math.Clamp(e.X * trbSes.Maximum / Math.Max(1, trbSes.Width), 0, trbSes.Maximum);
            trbSes.Value = deger;
            sesDuzeyi = deger / 100f;
            if (sesCihazi != null)
            {
                sesCihazi.Volume = sesDuzeyi;
            }
        }

        private static string SureFormatla(double saniye)
        {
            int dakika = (int)saniye / 60;
            int sn = (int)saniye % 60;
            return $"{dakika:D2}:{sn:D2}";
        }

        private double OynatmaSaniyesi() => oynatmaSaatiTaban + oynatmaSaati.Elapsed.TotalSeconds;

        private void OynatmayiBaslat()
        {
            timerVideo.Start();
            sesCihazi?.Play();
            oynatmaSaati.Start();
        }

        private void OynatmayiDuraklat()
        {
            timerVideo.Stop();
            sesCihazi?.Pause();
            oynatmaSaatiTaban += oynatmaSaati.Elapsed.TotalSeconds; // saati dondur
            oynatmaSaati.Reset();
        }

        // Videonun ses izini oynatmaya hazırlar; ses izi yoksa video sessiz oynar
        private void SesiHazirla(string videoYolu)
        {
            SesiKapat();
            try
            {
                sesOkuyucu = new MediaFoundationReader(videoYolu);
                sesCihazi = new WaveOutEvent { DesiredLatency = 150 };
                sesCihazi.Init(sesOkuyucu);
                sesCihazi.Volume = sesDuzeyi;
            }
            catch
            {
                SesiKapat();
            }
        }

        private void SesiKapat()
        {
            sesCihazi?.Stop();
            sesCihazi?.Dispose();
            sesCihazi = null;
            sesOkuyucu?.Dispose();
            sesOkuyucu = null;
        }

        // Sesi ve oynatma saatini verilen karenin zamanına konumlandırır
        private void SesiKonumla(int kareNo)
        {
            if (yakalama == null)
            {
                return;
            }

            double fps = yakalama.Get(CapProp.Fps);
            if (fps <= 0) fps = 25;
            double saniye = kareNo / fps;

            oynatmaSaatiTaban = saniye;
            if (oynatmaSaati.IsRunning)
            {
                oynatmaSaati.Restart();
            }
            else
            {
                oynatmaSaati.Reset();
            }

            if (sesOkuyucu != null)
            {
                try
                {
                    sesOkuyucu.CurrentTime = TimeSpan.FromSeconds(saniye);
                }
                catch
                {
                    // Konumlanamazsa ses kaldığı yerden devam eder
                }
            }
        }

        private void btnDosyasec_Click(object sender, EventArgs e)
        {
            // Kullanıcıya bir video dosyası seçme işlemi sunuluyor
            OpenFileDialog dosyaAc = new OpenFileDialog();
            dosyaAc.Title = "Video Seç";
            dosyaAc.Filter = "Video Dosyaları (*.mp4, *.avi, *.mkv)|*.mp4;*.avi;*.mkv|Tüm Dosyalar (*.*)|*.*";
            dosyaAc.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyVideos);

            if (dosyaAc.ShowDialog() == DialogResult.OK)
            {
                secilenVideoYolu = dosyaAc.FileName;

                if (System.IO.File.Exists(secilenVideoYolu))
                {
                    // Seçilen video dosyasının yolunu etikete ekleyin
                    label1.Text = secilenVideoYolu;

                    // Video yakalama işlemi başlatılır
                    yakalama = new VideoCapture(secilenVideoYolu);
                    SesiHazirla(secilenVideoYolu);

                    HesaplaVeGuncelleVideoSuresi();
                    SesiKonumla(0);
                    OynatmayiBaslat();
                }
                else
                {
                    label1.Text = "Video dosyası mevcut değil.";
                }
            }
        }

        private void timerVideo_Tick(object sender, EventArgs e)
        {
            if (yakalama == null || isDragging)
            {
                return;
            }

            if (!yakalama.IsOpened)
            {
                timerVideo.Stop();
                return;
            }

            // Gösterilecek kare pürüzsüz oynatma saatine göre seçilir;
            // böylece görüntü akıcı kalır ve ses kayması birikmez
            if (oynatmaSaati.IsRunning)
            {
                double fpsSenkron = yakalama.Get(CapProp.Fps);
                if (fpsSenkron <= 0) fpsSenkron = 25;

                double saat = OynatmaSaniyesi();

                // Saat, sesin gerçek konumundan belirgin saptıysa düzelt
                if (sesOkuyucu != null && sesCihazi != null && sesCihazi.PlaybackState == PlaybackState.Playing)
                {
                    double sesSaniye = sesOkuyucu.CurrentTime.TotalSeconds;
                    if (Math.Abs(sesSaniye - saat) > 0.5)
                    {
                        oynatmaSaatiTaban = sesSaniye;
                        oynatmaSaati.Restart();
                        saat = sesSaniye;
                    }
                }

                int hedefKare = (int)(saat * fpsSenkron);
                int gecerliKare = (int)yakalama.Get(CapProp.PosFrames);

                if (hedefKare <= gecerliKare)
                {
                    return; // bu karenin zamanı henüz gelmedi
                }

                // Görüntü geride kaldıysa aradaki kareleri göstermeden atla
                for (int i = gecerliKare; i < hedefKare - 1; i++)
                {
                    if (!yakalama.Grab())
                    {
                        break;
                    }
                }
            }

            // Çerçeve okuma işlemini burada yapın, bu şekilde çerçeve işleme maliyeti bir kez ödenir
            using Mat frame = new Mat();
            if (!yakalama.Read(frame) || frame.IsEmpty)
            {
                timerVideo.Stop();
                sesCihazi?.Stop();
                oynatmaSaati.Reset();
                yakalama = null;
                return;
            }

            // Çerçeveyi en-boy oranını koruyarak boyutlandır ve göster
            Size alan = pictureBox1.ClientSize;
            if (alan.Width > 8 && alan.Height > 8)
            {
                double olcek = Math.Min((double)alan.Width / frame.Width, (double)alan.Height / frame.Height);
                var hedef = new Size(Math.Max(1, (int)(frame.Width * olcek)), Math.Max(1, (int)(frame.Height * olcek)));

                using Mat boyutlanmisKare = new Mat();
                CvInvoke.Resize(frame, boyutlanmisKare, hedef);
                using var goruntu = boyutlanmisKare.ToImage<Bgr, byte>();
                pictureBox1.Image?.Dispose();
                pictureBox1.Image = goruntu.ToBitmap();
            }

            // ProgressBar'ı güncelle
            int toplamKareSayisi = (int)yakalama.Get(CapProp.FrameCount);
            int gecerliKareNo = (int)yakalama.Get(CapProp.PosFrames);
            progressBarvideo.Maximum = toplamKareSayisi;
            progressBarvideo.Value = Math.Min(gecerliKareNo, toplamKareSayisi);

            // Videonun ilerleme süresini hesaplayın ve etiketi güncelleyin
            double fps = yakalama.Get(CapProp.Fps);
            lblSure.Text = SureFormatla(gecerliKareNo / fps);
        }

        private async void btnAra_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(secilenVideoYolu) || !System.IO.File.Exists(secilenVideoYolu))
            {
                MessageBox.Show("Önce bir video dosyası seçin.");
                return;
            }

            string arananKelime = txtAra.Text.Trim();
            if (arananKelime.Length == 0)
            {
                MessageBox.Show("Aranacak kelimeyi yazın.");
                return;
            }

            if (!System.IO.File.Exists(@"./tessdata/tur.traineddata"))
            {
                MessageBox.Show("OCR dil dosyası bulunamadı.\n\nUygulama klasöründe 'tessdata\\tur.traineddata' dosyası olmalı.",
                    "tessdata eksik", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            btnAra.Enabled = false;
            btnSesAra.Enabled = false;
            btnAraDurdur.Enabled = true;
            lstAranankelimekarsilik.Items.Clear();
            aramaCts = new CancellationTokenSource();

            // Progress<T> geri çağrıları UI iş parçacığında çalışır
            var ilerleme = new Progress<(int kareNo, int toplamKare, double saniye)>(p =>
            {
                progressBarAra.Maximum = p.toplamKare;
                progressBarAra.Value = Math.Min(p.kareNo, p.toplamKare);
                lblAraSure.Text = SureFormatla(p.saniye);
            });
            var sonucBildirimi = new Progress<AramaSonucu>(s => lstAranankelimekarsilik.Items.Add(s));

            string dizinYolu = secilenVideoYolu + ".ocr.json";

            try
            {
                var (bulunan, dizindenGeldi) = await Task.Run(() =>
                    AramaYap(secilenVideoYolu, dizinYolu, arananKelime, ilerleme, sonucBildirimi, aramaCts.Token));

                string kaynak = dizindenGeldi ? " (kayıtlı OCR dizininden)" : "";
                MessageBox.Show($"Arama bitti{kaynak}. {bulunan} sonuç bulundu.");
            }
            catch (OperationCanceledException)
            {
                MessageBox.Show("Arama iptal edildi.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Arama sırasında hata oluştu:\n" + ex.Message);
            }
            finally
            {
                aramaCts.Dispose();
                aramaCts = null;
                btnAra.Enabled = true;
                btnSesAra.Enabled = true;
                btnAraDurdur.Enabled = false;
            }
        }

        private void btnAraDurdur_Click(object sender, EventArgs e)
        {
            aramaCts?.Cancel();
        }

        private async void btnSesAra_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(secilenVideoYolu) || !System.IO.File.Exists(secilenVideoYolu))
            {
                MessageBox.Show("Önce bir video dosyası seçin.");
                return;
            }

            string arananKelime = txtAra.Text.Trim();
            if (arananKelime.Length == 0)
            {
                MessageBox.Show("Aranacak kelimeyi yazın.");
                return;
            }

            string modelYolu = @"./whisper/ggml-small.bin";
            if (!System.IO.File.Exists(modelYolu))
            {
                MessageBox.Show("Whisper ses modeli bulunamadı.\n\nUygulama klasöründe 'whisper\\ggml-small.bin' dosyası olmalı.\n" +
                    "İndirme adresi:\nhttps://huggingface.co/ggerganov/whisper.cpp/resolve/main/ggml-small.bin",
                    "Ses modeli eksik", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            btnAra.Enabled = false;
            btnSesAra.Enabled = false;
            btnAraDurdur.Enabled = true;
            lstAranankelimekarsilik.Items.Clear();
            aramaCts = new CancellationTokenSource();

            var ilerleme = new Progress<(int kareNo, int toplamKare, double saniye)>(p =>
            {
                progressBarAra.Maximum = Math.Max(1, p.toplamKare);
                progressBarAra.Value = Math.Min(p.kareNo, progressBarAra.Maximum);
                lblAraSure.Text = SureFormatla(p.saniye);
            });
            var sonucBildirimi = new Progress<AramaSonucu>(s => lstAranankelimekarsilik.Items.Add(s));

            string dizinYolu = secilenVideoYolu + ".asr.json";

            try
            {
                var (bulunan, dizindenGeldi) = await SesAramaYap(secilenVideoYolu, dizinYolu, arananKelime,
                    modelYolu, ilerleme, sonucBildirimi, aramaCts.Token);

                string kaynak = dizindenGeldi ? " (kayıtlı ses dizininden)" : "";
                MessageBox.Show($"Ses araması bitti{kaynak}. {bulunan} sonuç bulundu.");
            }
            catch (OperationCanceledException)
            {
                MessageBox.Show("Arama iptal edildi.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ses araması sırasında hata oluştu:\n" + ex.Message);
            }
            finally
            {
                aramaCts.Dispose();
                aramaCts = null;
                btnAra.Enabled = true;
                btnSesAra.Enabled = true;
                btnAraDurdur.Enabled = false;
            }
        }

        // Ses aramasının giriş noktası: geçerli bir ses dizini varsa onun
        // üzerinde anında arar; yoksa sesi çıkarıp Whisper ile yazıya döker,
        // dizini oluşturur ve kaydeder.
        private async Task<(int bulunan, bool dizindenGeldi)> SesAramaYap(string videoYolu, string dizinYolu,
            string arananKelime, string modelYolu, IProgress<(int, int, double)> ilerleme,
            IProgress<AramaSonucu> sonucBildirimi, CancellationToken iptal)
        {
            var eslestirici = new KelimeEslestirici(arananKelime);

            OcrDizin? dizin = DizinYukle(videoYolu, dizinYolu);
            if (dizin != null)
            {
                foreach (OcrKayit kayit in dizin.Kayitlar)
                {
                    iptal.ThrowIfCancellationRequested();
                    AramaSonucu? sonuc = eslestirici.Degerlendir(kayit);
                    if (sonuc != null)
                    {
                        sonucBildirimi.Report(sonuc);
                    }
                }

                ilerleme.Report((dizin.ToplamKare, dizin.ToplamKare, dizin.ToplamKare / dizin.Fps));
                return (eslestirici.BulunanSayisi, true);
            }

            dizin = await SesiDizinle(videoYolu, modelYolu, kayit =>
            {
                AramaSonucu? sonuc = eslestirici.Degerlendir(kayit);
                if (sonuc != null)
                {
                    sonucBildirimi.Report(sonuc);
                }
            }, ilerleme, iptal);

            try
            {
                System.IO.File.WriteAllText(dizinYolu, JsonSerializer.Serialize(dizin));
            }
            catch
            {
                // Dizin dosyası yazılamazsa önbelleksiz devam et
            }

            return (eslestirici.BulunanSayisi, false);
        }

        // Videonun sesini Whisper ile yazıya döker ve konuşma parçalarından
        // dizin oluşturur; her parça için kayitOlustu çağrılır.
        private async Task<OcrDizin> SesiDizinle(string videoYolu, string modelYolu, Action<OcrKayit> kayitOlustu,
            IProgress<(int, int, double)> ilerleme, CancellationToken iptal)
        {
            // Sonuçlara tıklanınca kare numarasına çevirebilmek için videonun FPS'i gerekli
            double fps;
            using (var video = new VideoCapture(videoYolu))
            {
                fps = video.Get(CapProp.Fps);
                if (fps <= 0) fps = 25;
            }

            // Sesi videodan çıkar (16 kHz mono; Whisper'ın beklediği biçim)
            var (ornekler, toplamSaniye) = await Task.Run(() => SesiCikar(videoYolu), iptal);

            var bilgi = new System.IO.FileInfo(videoYolu);
            var dizin = new OcrDizin
            {
                Surum = DizinSurumu,
                DosyaBoyutu = bilgi.Length,
                DosyaDegisme = bilgi.LastWriteTimeUtc,
                Fps = fps,
                ToplamKare = (int)(toplamSaniye * fps)
            };

            using var fabrika = WhisperFactory.FromPath(modelYolu);
            await using var islemci = fabrika.CreateBuilder()
                .WithLanguage("tr")
                .WithThreads(Math.Max(1, Environment.ProcessorCount - 1))
                .Build();

            await foreach (SegmentData parca in islemci.ProcessAsync(ornekler, iptal))
            {
                double saniye = parca.Start.TotalSeconds;
                var kayit = new OcrKayit
                {
                    KareNo = (int)(saniye * fps),
                    Saniye = saniye,
                    Metin = parca.Text.Trim()
                };
                dizin.Kayitlar.Add(kayit);
                kayitOlustu(kayit);
                ilerleme.Report(((int)(parca.End.TotalSeconds * fps), dizin.ToplamKare, parca.End.TotalSeconds));
            }

            ilerleme.Report((dizin.ToplamKare, dizin.ToplamKare, toplamSaniye));
            return dizin;
        }

        // Videonun ses izini 16 kHz mono float örneklere çevirir
        private static (float[] ornekler, double toplamSaniye) SesiCikar(string videoYolu)
        {
            using var okuyucu = new MediaFoundationReader(videoYolu);
            double toplamSaniye = okuyucu.TotalTime.TotalSeconds;

            var hedefFormat = new WaveFormat(16000, 16, 1);
            using var donusturucu = new MediaFoundationResampler(okuyucu, hedefFormat);

            using var bellek = new System.IO.MemoryStream();
            byte[] tampon = new byte[hedefFormat.AverageBytesPerSecond];
            int okunan;
            while ((okunan = donusturucu.Read(tampon, 0, tampon.Length)) > 0)
            {
                bellek.Write(tampon, 0, okunan);
            }

            byte[] bayt = bellek.ToArray();
            var ornekler = new float[bayt.Length / 2];
            for (int i = 0; i < ornekler.Length; i++)
            {
                ornekler[i] = BitConverter.ToInt16(bayt, i * 2) / 32768f;
            }

            return (ornekler, toplamSaniye);
        }

        private void btnYenidenDizinle_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(secilenVideoYolu))
            {
                MessageBox.Show("Önce bir video dosyası seçin.");
                return;
            }

            if (aramaCts != null)
            {
                MessageBox.Show("Arama sürerken dizin silinemez. Önce aramayı durdurun.");
                return;
            }

            if (MessageBox.Show("Bu videonun kayıtlı görüntü ve ses dizinleri silinecek;\nbir sonraki arama videoyu baştan tarayacak. Devam edilsin mi?",
                    "Yeniden Dizinle", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
            {
                return;
            }

            int silinen = 0;
            foreach (string uzanti in new[] { ".ocr.json", ".asr.json" })
            {
                string yol = secilenVideoYolu + uzanti;
                if (System.IO.File.Exists(yol))
                {
                    System.IO.File.Delete(yol);
                    silinen++;
                }
            }

            MessageBox.Show(silinen > 0
                ? "Kayıtlı dizinler silindi. Bir sonraki arama videoyu baştan tarayacak."
                : "Bu video için kayıtlı dizin zaten yok.");
        }

        private void btnSonucKaydet_Click(object sender, EventArgs e)
        {
            if (lstAranankelimekarsilik.Items.Count == 0)
            {
                MessageBox.Show("Kaydedilecek sonuç yok.");
                return;
            }

            using var kaydet = new SaveFileDialog
            {
                Title = "Sonuçları Kaydet",
                Filter = "Metin Dosyası (*.txt)|*.txt",
                FileName = "arama-sonuclari.txt"
            };

            if (kaydet.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            var satirlar = new List<string>
            {
                $"Video  : {secilenVideoYolu}",
                $"Aranan : {txtAra.Text}",
                ""
            };
            foreach (object oge in lstAranankelimekarsilik.Items)
            {
                satirlar.Add(oge.ToString() ?? "");
            }

            System.IO.File.WriteAllLines(kaydet.FileName, satirlar);
            MessageBox.Show("Sonuçlar kaydedildi.");
        }

        // Aramanın giriş noktası (arka plan iş parçacığında çalışır):
        // geçerli bir OCR dizini varsa onun üzerinde anında arar,
        // yoksa videoyu tarar, dizini oluşturur ve kaydeder.
        private (int bulunan, bool dizindenGeldi) AramaYap(string videoYolu, string dizinYolu, string arananKelime,
            IProgress<(int, int, double)> ilerleme, IProgress<AramaSonucu> sonucBildirimi,
            CancellationToken iptal)
        {
            var eslestirici = new KelimeEslestirici(arananKelime);

            OcrDizin? dizin = DizinYukle(videoYolu, dizinYolu);
            if (dizin != null)
            {
                // Video daha önce dizinlenmiş: OCR olmadan doğrudan metinlerde ara
                foreach (OcrKayit kayit in dizin.Kayitlar)
                {
                    iptal.ThrowIfCancellationRequested();
                    AramaSonucu? sonuc = eslestirici.Degerlendir(kayit);
                    if (sonuc != null)
                    {
                        sonucBildirimi.Report(sonuc);
                    }
                }

                ilerleme.Report((dizin.ToplamKare, dizin.ToplamKare, dizin.ToplamKare / dizin.Fps));
                return (eslestirici.BulunanSayisi, true);
            }

            // İlk arama: videoyu tararken eşleşmeleri anında bildir, dizini de oluştur
            dizin = VideoyuDizinle(videoYolu, kayit =>
            {
                AramaSonucu? sonuc = eslestirici.Degerlendir(kayit);
                if (sonuc != null)
                {
                    sonucBildirimi.Report(sonuc);
                }
            }, ilerleme, iptal);

            try
            {
                System.IO.File.WriteAllText(dizinYolu, JsonSerializer.Serialize(dizin));
            }
            catch
            {
                // Dizin dosyası yazılamazsa (salt okunur klasör vb.) önbelleksiz devam et
            }

            return (eslestirici.BulunanSayisi, false);
        }

        // Kayıtlı OCR dizinini okur; dosya yoksa, bozuksa ya da video
        // değiştiyse null döner (yeniden tarama gerekir)
        private static OcrDizin? DizinYukle(string videoYolu, string dizinYolu)
        {
            try
            {
                if (!System.IO.File.Exists(dizinYolu))
                {
                    return null;
                }

                var dizin = JsonSerializer.Deserialize<OcrDizin>(System.IO.File.ReadAllText(dizinYolu));
                if (dizin == null || dizin.Surum != DizinSurumu)
                {
                    return null;
                }

                var bilgi = new System.IO.FileInfo(videoYolu);
                if (dizin.DosyaBoyutu != bilgi.Length || dizin.DosyaDegisme != bilgi.LastWriteTimeUtc)
                {
                    return null;
                }

                return dizin;
            }
            catch
            {
                return null;
            }
        }

        // Videoyu saniyede bir kare OCR'layarak dizinler. Her taranan kare
        // için kayitOlustu çağrılır; UI'a yalnızca IProgress ile dokunur.
        private OcrDizin VideoyuDizinle(string videoYolu, Action<OcrKayit> kayitOlustu,
            IProgress<(int, int, double)> ilerleme, CancellationToken iptal)
        {
            using var video = new VideoCapture(videoYolu);
            double fps = video.Get(CapProp.Fps);
            if (fps <= 0) fps = 25;
            int toplamKare = (int)video.Get(CapProp.FrameCount);
            int adim = Math.Max(1, (int)Math.Round(fps)); // saniyede 1 kare tara

            var bilgi = new System.IO.FileInfo(videoYolu);
            var dizin = new OcrDizin
            {
                Surum = DizinSurumu,
                DosyaBoyutu = bilgi.Length,
                DosyaDegisme = bilgi.LastWriteTimeUtc,
                Fps = fps,
                ToplamKare = toplamKare
            };

            using var tesseract = new TesseractEngine(@"./tessdata", "tur", EngineMode.Default);
            tesseract.DefaultPageSegMode = PageSegMode.SparseText; // metin ekranın herhangi bir yerinde olabilir

            Mat? oncekiGri = null;
            try
            {
                for (int kareNo = 0; kareNo < toplamKare; kareNo += adim)
                {
                    iptal.ThrowIfCancellationRequested();

                    video.Set(CapProp.PosFrames, kareNo);
                    using var kare = new Mat();
                    if (!video.Read(kare) || kare.IsEmpty)
                    {
                        break;
                    }

                    // Zaman damgası etiketten değil, doğrudan kare numarasından hesaplanır
                    double saniye = kareNo / fps;
                    ilerleme.Report((kareNo, toplamKare, saniye));

                    using var gri = new Mat();
                    CvInvoke.CvtColor(kare, gri, ColorConversion.Bgr2Gray);

                    // Görüntü son OCR'lanan kareyle hemen hemen aynıysa OCR'ı atla
                    if (oncekiGri != null && oncekiGri.Size == gri.Size)
                    {
                        using var fark = new Mat();
                        CvInvoke.AbsDiff(gri, oncekiGri, fark);
                        if (CvInvoke.Mean(fark).V0 < 2.0)
                        {
                            continue;
                        }
                    }
                    oncekiGri?.Dispose();
                    oncekiGri = gri.Clone();

                    string metin = KareyiOkut(gri, tesseract);

                    var kayit = new OcrKayit { KareNo = kareNo, Saniye = saniye, Metin = metin };
                    dizin.Kayitlar.Add(kayit);
                    kayitOlustu(kayit);
                }
            }
            finally
            {
                oncekiGri?.Dispose();
            }

            ilerleme.Report((toplamKare, toplamKare, toplamKare / fps));
            return dizin;
        }

        // Gri tonlamalı kareyi Tesseract için hazırlayıp metne çevirir
        private static string KareyiOkut(Mat gri, TesseractEngine tesseract)
        {
            using var islenmis = new Mat();

            // Düşük çözünürlükte küçük yazılar için görüntüyü büyütmek OCR isabetini artırır
            if (gri.Height < 720)
            {
                CvInvoke.Resize(gri, islenmis, new Size(gri.Width * 2, gri.Height * 2), 0, 0, Inter.Cubic);
            }
            else
            {
                gri.CopyTo(islenmis);
            }

            using var esikli = new Mat();
            CvInvoke.AdaptiveThreshold(islenmis, esikli, 255, AdaptiveThresholdType.GaussianC, ThresholdType.Binary, 11, 2);

            using var goruntu = esikli.ToImage<Gray, byte>();
            using var bitmap = goruntu.ToBitmap();
            using var bellek = new System.IO.MemoryStream();
            bitmap.Save(bellek, System.Drawing.Imaging.ImageFormat.Png);

            using var pix = Pix.LoadFromMemory(bellek.ToArray());
            using Page sayfa = tesseract.Process(pix);
            return sayfa.GetText() ?? string.Empty;
        }

        private void btnDur_Click(object sender, EventArgs e)
        {
            if (yakalama != null)
            {
                OynatmayiDuraklat();
            }
        }

        private void btnYenidenoynat_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(secilenVideoYolu))
            {
                return;
            }

            OynatmayiDuraklat();
            yakalama?.Dispose();
            yakalama = new VideoCapture(secilenVideoYolu); // Baştan başlat
            SesiKonumla(0);
            OynatmayiBaslat();
        }

        private void btnDuraklat_Click(object sender, EventArgs e)
        {
            if (yakalama != null)
            {
                OynatmayiDuraklat();
            }
        }

        private void lstAranankelimekarsilik_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstAranankelimekarsilik.SelectedItem is not AramaSonucu sonuc || yakalama == null)
            {
                return;
            }

            // Videoyu ve sesi sonucun bulunduğu kareye konumlandır
            yakalama.Set(CapProp.PosFrames, sonuc.KareNo);
            SesiKonumla(sonuc.KareNo);
            if (progressBarvideo.Maximum > 0)
            {
                progressBarvideo.Value = Math.Min(sonuc.KareNo, progressBarvideo.Maximum);
            }
            lblSure.Text = SureFormatla(sonuc.Saniye);
        }

        private void btnOynat_Click(object sender, EventArgs e)
        {
            if (yakalama != null)
            {
                OynatmayiBaslat();
            }
        }

        private void ProgressBar_MouseDown(object? sender, MouseEventArgs e)
        {
            isDragging = true;
            FareKonumunaSar(); // tek tıklamada da o noktaya atla
        }

        private void ProgressBar_MouseMove(object? sender, MouseEventArgs e)
        {
            if (isDragging)
            {
                FareKonumunaSar();
            }
        }

        // Videoyu, imlecin sarma çubuğundaki konumuna karşılık gelen kareye taşır
        private void FareKonumunaSar()
        {
            Point cursor = progressBarvideo.PointToClient(Cursor.Position);
            int newValue = Math.Clamp(cursor.X * progressBarvideo.Maximum / Math.Max(1, progressBarvideo.Width),
                0, progressBarvideo.Maximum);

            if (newValue != previousProgressBarValue)
            {
                progressBarvideo.Value = newValue;
                yakalama?.Set(CapProp.PosFrames, newValue);
                previousProgressBarValue = newValue;
            }
        }

        private void ProgressBar_MouseUp(object? sender, MouseEventArgs e)
        {
            isDragging = false;

            if (yakalama != null)
            {
                // İlerlemeyi ayarlayın (ses de aynı ana sarılır)
                int newPosition = progressBarvideo.Value;
                yakalama.Set(CapProp.PosFrames, newPosition);
                SesiKonumla(newPosition);
            }
        }

        private void HesaplaVeGuncelleVideoSuresi()
        {
            if (yakalama != null)
            {
                double toplamKareSayisi = yakalama.Get(CapProp.FrameCount);
                double fps = yakalama.Get(CapProp.Fps);
                if (fps <= 0) fps = 25;
                double toplamSureSaniye = toplamKareSayisi / fps;

                // Timer kare süresinin yarısında tikler: kare seçimini saat
                // yaptığı için sık tik atlama/bekleme kararını inceltir
                timerVideo.Interval = Math.Max(10, (int)Math.Round(500.0 / fps));

                lblVideoSureUzunluk.Text = SureFormatla(toplamSureSaniye);
                lblFbs.Text = fps.ToString("0.##");
            }
            else
            {
                lblVideoSureUzunluk.Text = "00:00"; // Video yakalanmadıysa varsayılan değer
            }
        }

        [DllImport("dwmapi.dll")]
        private static extern int DwmSetWindowAttribute(IntPtr pencere, int oznitelik, ref int deger, int boyut);

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        private static extern IntPtr SendMessage(IntPtr pencere, int mesaj, IntPtr wParam, string lParam);

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);

            // Pencere başlık çubuğunu koyu temaya geçir (Windows 10 1809+)
            int koyu = 1;
            _ = DwmSetWindowAttribute(Handle, 20, ref koyu, sizeof(int));
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Arama kutusuna ipucu yazısı (EM_SETCUEBANNER)
            _ = SendMessage(txtAra.Handle, 0x1501, (IntPtr)1, "Aranacak kelimeyi yazın...");

            // Üst çubuktaki AKAS logosunu yükle
            string logoYolu = System.IO.Path.Combine(AppContext.BaseDirectory, "logo.png");
            if (System.IO.File.Exists(logoYolu))
            {
                picLogo.Image = Image.FromFile(logoYolu);
            }

            // Pencere ve görev çubuğu simgesi
            string ikonYolu = System.IO.Path.Combine(AppContext.BaseDirectory, "app.ico");
            if (System.IO.File.Exists(ikonYolu))
            {
                Icon = new Icon(ikonYolu);
            }
        }

        // Sonuç listesi öğelerini iki satır (zaman + bağlam) olarak çizer
        private void lstAranankelimekarsilik_DrawItem(object? sender, DrawItemEventArgs e)
        {
            if (e.Index < 0)
            {
                return;
            }

            bool secili = (e.State & DrawItemState.Selected) != 0;
            Color arka = secili ? Tema.VurguKoyu : (e.Index % 2 == 0 ? Tema.Yuzey : Tema.YuzeyAcik);
            using (var firca = new SolidBrush(arka))
            {
                e.Graphics.FillRectangle(firca, e.Bounds);
            }

            string zaman, metin;
            if (lstAranankelimekarsilik.Items[e.Index] is AramaSonucu sonuc)
            {
                zaman = SureFormatla(sonuc.Saniye);
                metin = sonuc.Baglam.Length > 0 ? sonuc.Baglam : sonuc.Kelime;
            }
            else
            {
                zaman = "";
                metin = lstAranankelimekarsilik.Items[e.Index].ToString() ?? "";
            }

            Color zamanRenk = secili ? Color.White : Tema.Vurgu;
            Color metinRenk = secili ? Color.White : Tema.Metin;

            using var kalinFont = new Font(Font, FontStyle.Bold);
            TextRenderer.DrawText(e.Graphics, zaman, kalinFont,
                new Rectangle(e.Bounds.X + 10, e.Bounds.Y + 5, e.Bounds.Width - 20, 18),
                zamanRenk, TextFormatFlags.Left);
            TextRenderer.DrawText(e.Graphics, metin, Font,
                new Rectangle(e.Bounds.X + 10, e.Bounds.Y + 23, e.Bounds.Width - 20, 18),
                metinRenk, TextFormatFlags.Left | TextFormatFlags.EndEllipsis);
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            aramaCts?.Cancel(); // Form kapanırken süren aramayı iptal et
            SesiKapat();
            base.OnFormClosing(e);
        }

        //private void btnSesAra_Click(object sender, EventArgs e)
        //{
        //    ConvertAudioFormat(secilenVideoYolu, @"C:\Users\mrcom\OneDrive\Masaüstü\Yeni klasör (12)");
        //}
        //static void ConvertAudioFormat(string inputFilePath, string outputFilePath)
        //{
        //    string ffmpegPath = @"C:\Users\mrcom\source\repos\VideoKelimeArama\VideoKelimeArama\ffmpeg\bin\ffmpeg.exe"; // ffmpeg.exe'nin dosya yolu

        //    ProcessStartInfo processStartInfo = new ProcessStartInfo
        //    {
        //        FileName = ffmpegPath,
        //        Arguments = $"-i {inputFilePath} -ar 44100 -ac 2 -ab 192k {outputFilePath}",
        //        RedirectStandardOutput = true,
        //        UseShellExecute = false,
        //        CreateNoWindow = true
        //    };

        //    using (Process process = new Process { StartInfo = processStartInfo })
        //    {
        //        process.Start();
        //        process.WaitForExit();
        //    }

        //    MessageBox.Show($"Ses dosyası dönüştürüldü: {outputFilePath}");
        //}

    }
}
