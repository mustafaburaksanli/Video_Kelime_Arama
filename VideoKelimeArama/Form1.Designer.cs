namespace VideoKelimeArama
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            panelUst = new Panel();
            picLogo = new PictureBox();
            btnDosyasec = new ModernButton();
            label1 = new Label();
            lblFbsbaslik = new Label();
            lblFbs = new Label();
            panelSag = new Panel();
            lstAranankelimekarsilik = new ListBox();
            panelAramaAlt = new Panel();
            progressBarAra = new ModernBar();
            label5 = new Label();
            lblAraSure = new Label();
            btnSonucKaydet = new ModernButton();
            btnYenidenDizinle = new ModernButton();
            panelAramaUst = new Panel();
            txtAra = new TextBox();
            btnAra = new ModernButton();
            btnSesAra = new ModernButton();
            btnAraDurdur = new ModernButton();
            panelAlt = new Panel();
            progressBarvideo = new ModernBar();
            btnOynat = new ModernButton();
            btnDuraklat = new ModernButton();
            btnDur = new ModernButton();
            btnYenidenoynat = new ModernButton();
            lblSure = new Label();
            lblAyrac = new Label();
            lblVideoSureUzunluk = new Label();
            lblSes = new Label();
            trbSes = new ModernBar();
            panelVideo = new Panel();
            pictureBox1 = new PictureBox();
            timerVideo = new System.Windows.Forms.Timer(components);
            panelUst.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)picLogo).BeginInit();
            panelSag.SuspendLayout();
            panelAramaAlt.SuspendLayout();
            panelAramaUst.SuspendLayout();
            panelAlt.SuspendLayout();
            panelVideo.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            SuspendLayout();
            // 
            // panelUst
            // 
            panelUst.BackColor = Color.FromArgb(30, 30, 46);
            panelUst.Controls.Add(picLogo);
            panelUst.Controls.Add(btnDosyasec);
            panelUst.Controls.Add(label1);
            panelUst.Dock = DockStyle.Top;
            panelUst.Location = new Point(0, 0);
            panelUst.Name = "panelUst";
            panelUst.Size = new Size(1280, 166);
            panelUst.TabIndex = 0;
            // 
            // picLogo
            // 
            picLogo.BackColor = Color.Transparent;
            picLogo.Location = new Point(16, 23);
            picLogo.Name = "picLogo";
            picLogo.Size = new Size(142, 120);
            picLogo.SizeMode = PictureBoxSizeMode.Zoom;
            picLogo.TabIndex = 4;
            picLogo.TabStop = false;
            // 
            // btnDosyasec
            // 
            btnDosyasec.FlatStyle = FlatStyle.Flat;
            btnDosyasec.ForeColor = Color.FromArgb(232, 232, 242);
            btnDosyasec.Location = new Point(182, 63);
            btnDosyasec.Name = "btnDosyasec";
            btnDosyasec.NormalRenk = Color.FromArgb(137, 116, 255);
            btnDosyasec.Size = new Size(120, 40);
            btnDosyasec.TabIndex = 0;
            btnDosyasec.Text = "Dosya Seç";
            btnDosyasec.Yaricap = 8;
            btnDosyasec.Click += btnDosyasec_Click;
            // 
            // label1
            // 
            label1.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            label1.AutoEllipsis = true;
            label1.ForeColor = Color.FromArgb(140, 146, 168);
            label1.Location = new Point(322, 72);
            label1.Name = "label1";
            label1.Size = new Size(942, 25);
            label1.TabIndex = 1;
            label1.Text = "Henüz video seçilmedi";
            // 
            // lblFbsbaslik
            // 
            lblFbsbaslik.AutoSize = true;
            lblFbsbaslik.ForeColor = Color.FromArgb(140, 146, 168);
            lblFbsbaslik.Location = new Point(272, 54);
            lblFbsbaslik.Name = "lblFbsbaslik";
            lblFbsbaslik.Size = new Size(41, 23);
            lblFbsbaslik.TabIndex = 2;
            lblFbsbaslik.Text = "FPS:";
            // 
            // lblFbs
            // 
            lblFbs.AutoSize = true;
            lblFbs.ForeColor = Color.FromArgb(232, 232, 242);
            lblFbs.Location = new Point(312, 54);
            lblFbs.Name = "lblFbs";
            lblFbs.Size = new Size(27, 23);
            lblFbs.TabIndex = 3;
            lblFbs.Text = "—";
            // 
            // panelSag
            // 
            panelSag.BackColor = Color.FromArgb(30, 30, 46);
            panelSag.Controls.Add(lstAranankelimekarsilik);
            panelSag.Controls.Add(panelAramaAlt);
            panelSag.Controls.Add(panelAramaUst);
            panelSag.Dock = DockStyle.Right;
            panelSag.Location = new Point(880, 166);
            panelSag.Name = "panelSag";
            panelSag.Padding = new Padding(16, 12, 16, 12);
            panelSag.Size = new Size(400, 574);
            panelSag.TabIndex = 2;
            // 
            // lstAranankelimekarsilik
            // 
            lstAranankelimekarsilik.BackColor = Color.FromArgb(30, 30, 46);
            lstAranankelimekarsilik.BorderStyle = BorderStyle.None;
            lstAranankelimekarsilik.Dock = DockStyle.Fill;
            lstAranankelimekarsilik.DrawMode = DrawMode.OwnerDrawFixed;
            lstAranankelimekarsilik.ForeColor = Color.FromArgb(232, 232, 242);
            lstAranankelimekarsilik.IntegralHeight = false;
            lstAranankelimekarsilik.ItemHeight = 44;
            lstAranankelimekarsilik.Location = new Point(16, 122);
            lstAranankelimekarsilik.Name = "lstAranankelimekarsilik";
            lstAranankelimekarsilik.Size = new Size(368, 330);
            lstAranankelimekarsilik.TabIndex = 4;
            lstAranankelimekarsilik.DrawItem += lstAranankelimekarsilik_DrawItem;
            lstAranankelimekarsilik.SelectedIndexChanged += lstAranankelimekarsilik_SelectedIndexChanged;
            // 
            // panelAramaAlt
            // 
            panelAramaAlt.Controls.Add(progressBarAra);
            panelAramaAlt.Controls.Add(label5);
            panelAramaAlt.Controls.Add(lblAraSure);
            panelAramaAlt.Controls.Add(btnSonucKaydet);
            panelAramaAlt.Controls.Add(btnYenidenDizinle);
            panelAramaAlt.Dock = DockStyle.Bottom;
            panelAramaAlt.Location = new Point(16, 452);
            panelAramaAlt.Name = "panelAramaAlt";
            panelAramaAlt.Size = new Size(368, 110);
            panelAramaAlt.TabIndex = 5;
            // 
            // progressBarAra
            // 
            progressBarAra.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            progressBarAra.DoluRenk = Color.FromArgb(0, 176, 155);
            progressBarAra.Location = new Point(0, 12);
            progressBarAra.Maximum = 100;
            progressBarAra.Name = "progressBarAra";
            progressBarAra.Size = new Size(368, 10);
            progressBarAra.TabIndex = 0;
            progressBarAra.TutamacGoster = false;
            progressBarAra.Value = 0;
            progressBarAra.YolRenk = Color.FromArgb(42, 42, 62);
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.ForeColor = Color.FromArgb(140, 146, 168);
            label5.Location = new Point(0, 32);
            label5.Name = "label5";
            label5.Size = new Size(69, 23);
            label5.TabIndex = 1;
            label5.Text = "Tarama:";
            // 
            // lblAraSure
            // 
            lblAraSure.AutoSize = true;
            lblAraSure.ForeColor = Color.FromArgb(232, 232, 242);
            lblAraSure.Location = new Point(76, 32);
            lblAraSure.Name = "lblAraSure";
            lblAraSure.Size = new Size(50, 23);
            lblAraSure.TabIndex = 2;
            lblAraSure.Text = "00:00";
            // 
            // btnSonucKaydet
            // 
            btnSonucKaydet.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            btnSonucKaydet.FlatStyle = FlatStyle.Flat;
            btnSonucKaydet.ForeColor = Color.FromArgb(232, 232, 242);
            btnSonucKaydet.Location = new Point(0, 66);
            btnSonucKaydet.Name = "btnSonucKaydet";
            btnSonucKaydet.NormalRenk = Color.FromArgb(42, 42, 62);
            btnSonucKaydet.Size = new Size(180, 38);
            btnSonucKaydet.TabIndex = 1;
            btnSonucKaydet.Text = "Sonuçları Kaydet";
            btnSonucKaydet.Yaricap = 8;
            btnSonucKaydet.Click += btnSonucKaydet_Click;
            //
            // btnYenidenDizinle
            //
            btnYenidenDizinle.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnYenidenDizinle.FlatStyle = FlatStyle.Flat;
            btnYenidenDizinle.ForeColor = Color.FromArgb(232, 232, 242);
            btnYenidenDizinle.Location = new Point(188, 66);
            btnYenidenDizinle.Name = "btnYenidenDizinle";
            btnYenidenDizinle.NormalRenk = Color.FromArgb(42, 42, 62);
            btnYenidenDizinle.Size = new Size(180, 38);
            btnYenidenDizinle.TabIndex = 2;
            btnYenidenDizinle.Text = "Yeniden Dizinle";
            btnYenidenDizinle.Yaricap = 8;
            btnYenidenDizinle.Click += btnYenidenDizinle_Click;
            // 
            // panelAramaUst
            // 
            panelAramaUst.Controls.Add(txtAra);
            panelAramaUst.Controls.Add(btnAra);
            panelAramaUst.Controls.Add(btnSesAra);
            panelAramaUst.Controls.Add(btnAraDurdur);
            panelAramaUst.Dock = DockStyle.Top;
            panelAramaUst.Location = new Point(16, 12);
            panelAramaUst.Name = "panelAramaUst";
            panelAramaUst.Size = new Size(368, 110);
            panelAramaUst.TabIndex = 3;
            // 
            // txtAra
            // 
            txtAra.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtAra.BackColor = Color.FromArgb(42, 42, 62);
            txtAra.BorderStyle = BorderStyle.FixedSingle;
            txtAra.Font = new Font("Segoe UI", 11.25F);
            txtAra.ForeColor = Color.FromArgb(232, 232, 242);
            txtAra.Location = new Point(0, 8);
            txtAra.Name = "txtAra";
            txtAra.Size = new Size(368, 32);
            txtAra.TabIndex = 0;
            // 
            // btnAra
            // 
            btnAra.FlatStyle = FlatStyle.Flat;
            btnAra.ForeColor = Color.FromArgb(232, 232, 242);
            btnAra.Location = new Point(0, 56);
            btnAra.Name = "btnAra";
            btnAra.NormalRenk = Color.FromArgb(137, 116, 255);
            btnAra.Size = new Size(132, 38);
            btnAra.TabIndex = 1;
            btnAra.Text = "Görüntüde Ara";
            btnAra.Yaricap = 8;
            btnAra.Click += btnAra_Click;
            // 
            // btnSesAra
            // 
            btnSesAra.FlatStyle = FlatStyle.Flat;
            btnSesAra.ForeColor = Color.FromArgb(232, 232, 242);
            btnSesAra.Location = new Point(140, 56);
            btnSesAra.Name = "btnSesAra";
            btnSesAra.NormalRenk = Color.FromArgb(0, 176, 155);
            btnSesAra.Size = new Size(112, 38);
            btnSesAra.TabIndex = 2;
            btnSesAra.Text = "Seste Ara";
            btnSesAra.Yaricap = 8;
            btnSesAra.Click += btnSesAra_Click;
            // 
            // btnAraDurdur
            // 
            btnAraDurdur.Enabled = false;
            btnAraDurdur.FlatStyle = FlatStyle.Flat;
            btnAraDurdur.ForeColor = Color.FromArgb(232, 232, 242);
            btnAraDurdur.Location = new Point(260, 56);
            btnAraDurdur.Name = "btnAraDurdur";
            btnAraDurdur.NormalRenk = Color.FromArgb(220, 76, 76);
            btnAraDurdur.Size = new Size(108, 38);
            btnAraDurdur.TabIndex = 3;
            btnAraDurdur.Text = "Durdur";
            btnAraDurdur.Yaricap = 8;
            btnAraDurdur.Click += btnAraDurdur_Click;
            // 
            // panelAlt
            // 
            panelAlt.BackColor = Color.FromArgb(30, 30, 46);
            panelAlt.Controls.Add(progressBarvideo);
            panelAlt.Controls.Add(btnOynat);
            panelAlt.Controls.Add(btnDuraklat);
            panelAlt.Controls.Add(btnDur);
            panelAlt.Controls.Add(btnYenidenoynat);
            panelAlt.Controls.Add(lblSure);
            panelAlt.Controls.Add(lblAyrac);
            panelAlt.Controls.Add(lblVideoSureUzunluk);
            panelAlt.Controls.Add(lblFbsbaslik);
            panelAlt.Controls.Add(lblFbs);
            panelAlt.Controls.Add(lblSes);
            panelAlt.Controls.Add(trbSes);
            panelAlt.Dock = DockStyle.Bottom;
            panelAlt.Location = new Point(0, 640);
            panelAlt.Name = "panelAlt";
            panelAlt.Size = new Size(880, 100);
            panelAlt.TabIndex = 1;
            // 
            // progressBarvideo
            // 
            progressBarvideo.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            progressBarvideo.DoluRenk = Color.FromArgb(137, 116, 255);
            progressBarvideo.Location = new Point(16, 12);
            progressBarvideo.Maximum = 100;
            progressBarvideo.Name = "progressBarvideo";
            progressBarvideo.Size = new Size(848, 18);
            progressBarvideo.TabIndex = 0;
            progressBarvideo.TutamacGoster = true;
            progressBarvideo.Value = 0;
            progressBarvideo.YolRenk = Color.FromArgb(42, 42, 62);
            // 
            // btnOynat
            // 
            btnOynat.FlatStyle = FlatStyle.Flat;
            btnOynat.Font = new Font("Segoe UI", 11.25F);
            btnOynat.ForeColor = Color.FromArgb(232, 232, 242);
            btnOynat.Location = new Point(16, 44);
            btnOynat.Name = "btnOynat";
            btnOynat.NormalRenk = Color.FromArgb(137, 116, 255);
            btnOynat.Size = new Size(52, 40);
            btnOynat.TabIndex = 1;
            btnOynat.Text = "▶";
            btnOynat.Yaricap = 8;
            btnOynat.Click += btnOynat_Click;
            // 
            // btnDuraklat
            // 
            btnDuraklat.FlatStyle = FlatStyle.Flat;
            btnDuraklat.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold);
            btnDuraklat.ForeColor = Color.FromArgb(232, 232, 242);
            btnDuraklat.Location = new Point(76, 44);
            btnDuraklat.Name = "btnDuraklat";
            btnDuraklat.NormalRenk = Color.FromArgb(42, 42, 62);
            btnDuraklat.Size = new Size(52, 40);
            btnDuraklat.TabIndex = 2;
            btnDuraklat.Text = "❚❚";
            btnDuraklat.Yaricap = 8;
            btnDuraklat.Click += btnDuraklat_Click;
            // 
            // btnDur
            // 
            btnDur.FlatStyle = FlatStyle.Flat;
            btnDur.Font = new Font("Segoe UI", 11.25F);
            btnDur.ForeColor = Color.FromArgb(232, 232, 242);
            btnDur.Location = new Point(136, 44);
            btnDur.Name = "btnDur";
            btnDur.NormalRenk = Color.FromArgb(42, 42, 62);
            btnDur.Size = new Size(52, 40);
            btnDur.TabIndex = 3;
            btnDur.Text = "■";
            btnDur.Yaricap = 8;
            btnDur.Click += btnDur_Click;
            // 
            // btnYenidenoynat
            // 
            btnYenidenoynat.FlatStyle = FlatStyle.Flat;
            btnYenidenoynat.Font = new Font("Segoe UI", 11.25F);
            btnYenidenoynat.ForeColor = Color.FromArgb(232, 232, 242);
            btnYenidenoynat.Location = new Point(196, 44);
            btnYenidenoynat.Name = "btnYenidenoynat";
            btnYenidenoynat.NormalRenk = Color.FromArgb(42, 42, 62);
            btnYenidenoynat.Size = new Size(52, 40);
            btnYenidenoynat.TabIndex = 4;
            btnYenidenoynat.Text = "↻";
            btnYenidenoynat.Yaricap = 8;
            btnYenidenoynat.Click += btnYenidenoynat_Click;
            // 
            // lblSure
            // 
            lblSure.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            lblSure.AutoSize = true;
            lblSure.Font = new Font("Segoe UI", 10.5F, FontStyle.Bold);
            lblSure.ForeColor = Color.FromArgb(232, 232, 242);
            lblSure.Location = new Point(724, 54);
            lblSure.Name = "lblSure";
            lblSure.Size = new Size(57, 25);
            lblSure.TabIndex = 5;
            lblSure.Text = "00:00";
            // 
            // lblAyrac
            // 
            lblAyrac.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            lblAyrac.AutoSize = true;
            lblAyrac.Font = new Font("Segoe UI", 10.5F);
            lblAyrac.ForeColor = Color.FromArgb(140, 146, 168);
            lblAyrac.Location = new Point(776, 54);
            lblAyrac.Name = "lblAyrac";
            lblAyrac.Size = new Size(19, 25);
            lblAyrac.TabIndex = 6;
            lblAyrac.Text = "/";
            // 
            // lblVideoSureUzunluk
            // 
            lblVideoSureUzunluk.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            lblVideoSureUzunluk.AutoSize = true;
            lblVideoSureUzunluk.Font = new Font("Segoe UI", 10.5F);
            lblVideoSureUzunluk.ForeColor = Color.FromArgb(140, 146, 168);
            lblVideoSureUzunluk.Location = new Point(790, 54);
            lblVideoSureUzunluk.Name = "lblVideoSureUzunluk";
            lblVideoSureUzunluk.Size = new Size(56, 25);
            lblVideoSureUzunluk.TabIndex = 7;
            lblVideoSureUzunluk.Text = "00:00";
            //
            // lblSes
            //
            lblSes.AutoSize = true;
            lblSes.Font = new Font("Segoe UI", 10.5F);
            lblSes.ForeColor = Color.FromArgb(140, 146, 168);
            lblSes.Location = new Point(372, 52);
            lblSes.Name = "lblSes";
            lblSes.Text = "🔊";
            //
            // trbSes
            //
            trbSes.Location = new Point(408, 56);
            trbSes.Maximum = 100;
            trbSes.Name = "trbSes";
            trbSes.Size = new Size(110, 18);
            trbSes.TabIndex = 8;
            trbSes.TutamacGoster = true;
            trbSes.Value = 100;
            // 
            // panelVideo
            // 
            panelVideo.BackColor = Color.FromArgb(24, 24, 37);
            panelVideo.Controls.Add(pictureBox1);
            panelVideo.Dock = DockStyle.Fill;
            panelVideo.Location = new Point(0, 166);
            panelVideo.Name = "panelVideo";
            panelVideo.Padding = new Padding(14);
            panelVideo.Size = new Size(880, 474);
            panelVideo.TabIndex = 3;
            // 
            // pictureBox1
            // 
            pictureBox1.BackColor = Color.FromArgb(10, 10, 16);
            pictureBox1.Dock = DockStyle.Fill;
            pictureBox1.Location = new Point(14, 14);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(852, 446);
            pictureBox1.SizeMode = PictureBoxSizeMode.CenterImage;
            pictureBox1.TabIndex = 0;
            pictureBox1.TabStop = false;
            // 
            // timerVideo
            // 
            timerVideo.Tick += timerVideo_Tick;
            // 
            // Form1
            // 
            AcceptButton = btnAra;
            AutoScaleDimensions = new SizeF(120F, 120F);
            AutoScaleMode = AutoScaleMode.Dpi;
            BackColor = Color.FromArgb(24, 24, 37);
            ClientSize = new Size(1280, 740);
            Controls.Add(panelVideo);
            Controls.Add(panelAlt);
            Controls.Add(panelSag);
            Controls.Add(panelUst);
            Font = new Font("Segoe UI", 9.75F);
            ForeColor = Color.FromArgb(232, 232, 242);
            MinimumSize = new Size(1200, 660);
            Name = "Form1";
            Text = "AKAS — Video Kelime Bulucu";
            WindowState = FormWindowState.Maximized;
            Load += Form1_Load;
            panelUst.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)picLogo).EndInit();
            panelSag.ResumeLayout(false);
            panelAramaAlt.ResumeLayout(false);
            panelAramaAlt.PerformLayout();
            panelAramaUst.ResumeLayout(false);
            panelAramaUst.PerformLayout();
            panelAlt.ResumeLayout(false);
            panelAlt.PerformLayout();
            panelVideo.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private Panel panelUst;
        private PictureBox picLogo;
        private ModernButton btnDosyasec;
        private Label label1;
        private Label lblFbsbaslik;
        private Label lblFbs;
        private Panel panelSag;
        private ListBox lstAranankelimekarsilik;
        private Panel panelAramaAlt;
        private ModernBar progressBarAra;
        private Label label5;
        private Label lblAraSure;
        private ModernButton btnSonucKaydet;
        private ModernButton btnYenidenDizinle;
        private Panel panelAramaUst;
        private TextBox txtAra;
        private ModernButton btnAra;
        private ModernButton btnSesAra;
        private ModernButton btnAraDurdur;
        private Panel panelAlt;
        private ModernBar progressBarvideo;
        private ModernButton btnOynat;
        private ModernButton btnDuraklat;
        private ModernButton btnDur;
        private ModernButton btnYenidenoynat;
        private Label lblSure;
        private Label lblAyrac;
        private Label lblVideoSureUzunluk;
        private Label lblSes;
        private ModernBar trbSes;
        private Panel panelVideo;
        private PictureBox pictureBox1;
        private System.Windows.Forms.Timer timerVideo;
    }
}
