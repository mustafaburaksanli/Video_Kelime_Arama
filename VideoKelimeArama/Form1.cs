using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.OCR;
using Emgu.CV.Structure;
using Emgu.CV.UI;
using Emgu.CV.Util;
using System.Drawing;
using System;
using System.Windows.Forms;
using NAudio.Wave;
using System.Collections.Generic;
using Tesseract;
using System.Drawing.Imaging;
using System.Security.Cryptography.X509Certificates;
using System.Diagnostics;
namespace VideoKelimeArama
{
    public partial class Form1 : Form
    {
        // Gerekli deðiþkenlerin tanýmlanmasý
        private VideoCapture yakalama;
        private VideoCapture yakalamaAra;// Video yakalama nesnesi
        private Mat kare;                    // Video kareleri için matris
        private bool isDragging = false;
        private int previousProgressBarValue = -1;
        private TesseractEngine tesseract;
        IWavePlayer waveOutDevice;
        AudioFileReader audioFile;
        private string arananKelime;
        private double previousTimestamp = 0;
        Mat aramaCerceve = new Mat();
        public string secilenVideoYolu;

        public Form1()
        {
            InitializeComponent();
            //// ProgressBar sürükleme olaylarý
            //timerVideo.Interval = 40;
            //timerVideo.Tick += new EventHandler(timerVideo_Tick);

            progressBarvideo.MouseDown += ProgressBar_MouseDown;
            progressBarvideo.MouseMove += ProgressBar_MouseMove;
            progressBarvideo.MouseUp += ProgressBar_MouseUp;
        }

        private void btnDosyasec_Click(object sender, EventArgs e)
        {
            // Kullanýcýya bir video dosyasý seçme iþlemi sunuluyor
            OpenFileDialog dosyaAc = new OpenFileDialog();
            dosyaAc.Title = "Video Seç";
            dosyaAc.Filter = "Video Dosyalarý (*.mp4, *.avi, *.mkv)|*.mp4;*.avi;*.mkv|Tüm Dosyalar (*.*)|*.*";
            dosyaAc.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyVideos);

            if (dosyaAc.ShowDialog() == DialogResult.OK)
            {
                secilenVideoYolu = dosyaAc.FileName;



                if (System.IO.File.Exists(secilenVideoYolu))
                {
                    // Seçilen video dosyasýnýn yolunu etikete ekleyin
                    label1.Text = secilenVideoYolu;

                    // Video yakalama iþlemi baþlatýlýr
                    yakalama = new VideoCapture(secilenVideoYolu);
                    yakalamaAra = new VideoCapture(secilenVideoYolu);
                    // WaveOut waveOut = new WaveOut();

                    // Ses dosyasýný yükleyin ve oynatma nesnesine ekleyin
                    // AudioFileReader audioFile = new AudioFileReader(secilenVideoYolu);
                    // waveOut.Init(audioFile);
                    // Ses çalmayý baþlatýn
                    //  waveOut.Play();

                    HesaplaVeGuncelleVideoSuresi();
                    timerVideo.Start();

                }
                else
                {
                    label1.Text = "Video dosyasý mevcut deðil.";
                }
            }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            // PictureBox týklama olayý (þu anda boþ)
        }

        private void timerVideo_Tick(object sender, EventArgs e)
        {
            if (yakalama == null)
            {
                return;
            }

            if (!yakalama.IsOpened)
            {
                timerVideo.Stop();
                return;
            }

            // Çerçeve okuma iþlemini burada yapýn, bu þekilde çerçeve iþleme maliyeti bir kez ödenir
            Mat frame = new Mat();
            if (!yakalama.Read(frame) || frame.IsEmpty)
            {
                timerVideo.Stop();
                yakalama = null;
                frame.Dispose();
                return;
            }

            // Çerçeveyi boyutlandýr ve göster
            using (Mat boyutlanmisKare = new Mat())
            {
                CvInvoke.Resize(frame, boyutlanmisKare, pictureBox1.Size);
                pictureBox1.Image = boyutlanmisKare.ToImage<Bgr, byte>().ToBitmap();
                boyutlanmisKare.Dispose();
            }

            // ProgressBar'ý güncelle
            int toplamKareSayisi = (int)yakalama.Get(CapProp.FrameCount);
            int gecerliKareNo = (int)yakalama.Get(CapProp.PosFrames);
            progressBarvideo.Maximum = toplamKareSayisi;
            progressBarvideo.Value = gecerliKareNo;

            // Videonun ilerleme süresini hesaplayýn
            double fps = yakalama.Get(CapProp.Fps);
            double gecenSure = gecerliKareNo / fps;

            // Süreyi dakika ve saniye olarak formatlayýn
            int dakika = (int)gecenSure / 60;
            int saniye = (int)gecenSure % 60;

            // Etiketi güncelle
            lblSure.Text = $"{dakika:D2}:{saniye:D2}";
        }

        private async void btnAra_Click(object sender, EventArgs e)
        {
            arananKelime = txtAra.Text;
            int toplamKareSayisi = (int)yakalamaAra.Get(CapProp.FrameCount);
            double fps = yakalamaAra.Get(CapProp.Fps);

            if (yakalama != null)
            {
                // TimerAra'yý baþlat
                timerAra.Start();
                if (!string.IsNullOrEmpty(arananKelime))
                {
                    using (TesseractEngine tesseract = new TesseractEngine(@"./tessdata", "tur", EngineMode.Default))
                    {
                        while (yakalamaAra.Read(aramaCerceve))
                        {
                            if (aramaCerceve.IsEmpty)
                            {
                                // Video sona erdiðinde döngüden çýk
                                MessageBox.Show("Arama Bitti");
                                timerAra.Stop();
                                break;
                            }

                            await ProcessFrameAsync(aramaCerceve, tesseract);
                        }
                    }
                }

            }

        }

        private byte[] BitmapToByteArray(Bitmap bitmap)
        {
            using (var stream = new System.IO.MemoryStream())
            {
                bitmap.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                return stream.ToArray();
            }
        }
        private void btnDur_Click(object sender, EventArgs e)
        {
            if (yakalama != null)
            {
                timerVideo.Stop(); // Timer'ý durdur
            }
        }

        private void btnYenidenoynat_Click(object sender, EventArgs e)
        {
            timerVideo.Stop(); // Timer'ý durdur
            yakalama = new VideoCapture(label1.Text); // Yeni VideoCapture nesnesi oluþturun
            timerVideo.Start(); // Timer'ý tekrar baþlat
        }

        private void btnDuraklat_Click(object sender, EventArgs e)
        {
            if (yakalama != null)
            {
                timerVideo.Stop(); // Timer'ý durdur
                yakalama.Dispose(); // VideoCapture nesnesini temizleyin
                yakalama = new VideoCapture(label1.Text); // Yeni VideoCapture nesnesi oluþturun

                Mat ilkKare = new Mat();
                yakalama.Read(ilkKare);

                // Ayný hizalama iþlemini yapýn
                Mat boyutlanmisKare = new Mat();
                CvInvoke.Resize(ilkKare, boyutlanmisKare, new System.Drawing.Size(pictureBox1.Width, pictureBox1.Height));
                pictureBox1.Image = boyutlanmisKare.ToBitmap();

                ilkKare.Dispose(); // Mat nesnesini temizleyin
                boyutlanmisKare.Dispose(); // Mat nesnesini temizleyin
            }
        }

        private void progressBarvideo_Click(object sender, EventArgs e)
        {

        }

        private void lblSure_Click(object sender, EventArgs e)
        {

        }

        private void lstAranankelimekarsilik_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstAranankelimekarsilik.SelectedIndex >= 0)
            {
                // Seçilen öðeyi alýn
                string selectedItemText = lstAranankelimekarsilik.SelectedItem.ToString();

                // " -------->>>>>>" ifadesinin sonundan itibaren saniye deðerini alýn
                int indexOfSeparator = selectedItemText.LastIndexOf(" -------->>>>>>");
                if (indexOfSeparator >= 0)
                {
                    string timePart = selectedItemText.Substring(indexOfSeparator + " -------->>>>>>".Length);
                    string[] saatDakikaParcalari = timePart.Split(':');
                    int hedefDakika = int.Parse(saatDakikaParcalari[0]);
                    int hedefSaniye = int.Parse(saatDakikaParcalari[1]);
                    int hedefToplamSaniye = (hedefDakika * 60) + hedefSaniye;
                    double fps = yakalama.Get(CapProp.Fps);
                    int hedefKareNo = (int)(hedefToplamSaniye * fps);
                    progressBarvideo.Value = hedefKareNo;
                    yakalama.Set(CapProp.PosFrames, hedefKareNo);
                    lblSure.Text = timePart;
                }
            }
        }
        private void btnOynat_Click(object sender, EventArgs e)
        {
            if (yakalama != null)
            {
                timerVideo.Start(); // Timer'ý baþlat
            }
        }

        private void lblFbs_Click(object sender, EventArgs e)
        {

        }
        private void ProgressBar_MouseDown(object sender, MouseEventArgs e)
        {
            isDragging = true;
        }
        private void ProgressBar_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDragging)
            {
                Point cursor = progressBarvideo.PointToClient(Cursor.Position);
                int newValue = cursor.X * progressBarvideo.Maximum / progressBarvideo.Width;

                // Yalnýzca önceki deðer ile farklý bir deðeri ayarladýðýnýzda iþlem yapýn
                if (newValue != previousProgressBarValue)
                {
                    progressBarvideo.Value = Math.Max(0, Math.Min(progressBarvideo.Maximum, newValue));

                    if (yakalama != null)
                    {
                        yakalama.Set(CapProp.PosFrames, progressBarvideo.Value);
                    }

                    previousProgressBarValue = newValue;
                }
            }
        }
        private void ProgressBar_MouseUp(object sender, MouseEventArgs e)
        {
            isDragging = false;

            if (yakalama != null)
            {
                // Ýlerlemeyi ayarlayýn
                int newPosition = progressBarvideo.Value;
                yakalama.Set(CapProp.PosFrames, newPosition);

            }

        }

        private void lblVideoSureUzunluk_Click(object sender, EventArgs e)
        {

        }
        private void HesaplaVeGuncelleVideoSuresi()
        {
            if (yakalama != null)
            {
                double toplamKareSayisi = yakalama.Get(CapProp.FrameCount);
                double toplamSureSaniye = toplamKareSayisi / yakalama.Get(CapProp.Fps);

                // Ýstenen FPS deðeri (örneðin, 25 FPS) belirleyin
                double istenenFPS = yakalama.Get(CapProp.Fps);

                // Yeni timer interval deðerini hesaplayýn
                double yeniInterval = 1000.0 / istenenFPS;

                // Timer'ýn interval özelliðini ayarlayýn
                timerVideo.Interval = (int)Math.Round(yeniInterval);

                // Süreyi dakika ve saniye olarak formatlayýn
                int dakika = (int)toplamSureSaniye / 60;
                int saniye = (int)toplamSureSaniye % 60;

                // Etiketi güncelle
                lblVideoSureUzunluk.Text = $"{dakika:D2}:{saniye:D2}";
                lblFbs.Text = istenenFPS.ToString();
            }
            else
            {
                lblVideoSureUzunluk.Text = "00:00"; // Video yakalanmadýysa varsayýlan deðer
            }

        }



        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private async void timerAra_TickAsync(object sender, EventArgs e)
        {
            if (yakalamaAra != null)
            {
                // Video çerçevelerinin toplam sayýsýný alýn
                int toplamKareSayisi = (int)yakalamaAra.Get(CapProp.FrameCount);

                // Geçerli çerçeve numarasýný alýn
                int gecerliKareNo = (int)yakalamaAra.Get(CapProp.PosFrames);

                // Videonun FPS (kare hýzý) deðerini alýn
                double fps = yakalamaAra.Get(CapProp.Fps);

                // Geçen süreyi hesaplayýn (saniye cinsinden)
                double gecenSureSaniye = gecerliKareNo / fps;

                // Süreyi dakika ve saniye olarak formatlayýn
                int dakika = (int)gecenSureSaniye / 60;
                int saniye = (int)gecenSureSaniye % 60;

                // Etiketi güncelle
                lblAraSure.Text = $"{dakika:D2}:{saniye:D2}";
                progressBarAra.Maximum = toplamKareSayisi;
                progressBarAra.Value = gecerliKareNo;



            }
        }

        private async Task ProcessFrameAsync(Mat frame, TesseractEngine tesseract)
        {
            await Task.Run(() =>
               {
                   using (Image<Bgr, byte> image = frame.ToImage<Bgr, byte>())
                   {
                       // Görüntüye maskeleme ve güçlendirme uygula
                       // Örnek: Görüntüyü gri tonlamaya çevir ve adaptif eþikleme uygula
                       UMat grayImage = new UMat();
                       CvInvoke.CvtColor(image, grayImage, ColorConversion.Bgr2Gray);
                       CvInvoke.AdaptiveThreshold(grayImage, grayImage, 255, AdaptiveThresholdType.GaussianC, ThresholdType.Binary, 11, 2);

                       using (Bitmap enhancedBitmap = grayImage.ToImage<Gray, byte>().ToBitmap())
                       {
                           var byteArray = BitmapToByteArray(enhancedBitmap);
                           var pix = Tesseract.Pix.LoadFromMemory(byteArray);

                           using (Page page = tesseract.Process(pix))
                           {
                               string metin = page.GetText();

                               if (metin.Contains(arananKelime))
                               {
                                   // lstAranankelimekarsilik kontrolüne ana iþ parçacýðý üzerinden eriþim
                                   this.Invoke((MethodInvoker)delegate
                                   {
                                       lstAranankelimekarsilik.Items.Add(arananKelime + " -------->>>>>>" + lblAraSure.Text);
                                   });
                               }
                           }
                       }
                   }
               });
        }

        private void progressBar1_Click(object sender, EventArgs e)
        {

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

        //    MessageBox.Show($"Ses dosyasý dönüþtürüldü: {outputFilePath}");
        //}

    }
}
