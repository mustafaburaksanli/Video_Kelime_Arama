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
            btnDosyasec = new Button();
            label1 = new Label();
            pictureBox1 = new PictureBox();
            timerVideo = new System.Windows.Forms.Timer(components);
            txtAra = new TextBox();
            label2 = new Label();
            btnAra = new Button();
            lstAranankelimekarsilik = new ListBox();
            progressBarvideo = new ProgressBar();
            btnOynat = new Button();
            btnDur = new Button();
            btnDuraklat = new Button();
            btnYenidenoynat = new Button();
            lblSure = new Label();
            lblFbsbaslik = new Label();
            lblFbs = new Label();
            lblVideoSureUzunluk = new Label();
            timerAra = new System.Windows.Forms.Timer(components);
            label3 = new Label();
            label4 = new Label();
            label5 = new Label();
            lblAraSure = new Label();
            progressBarAra = new ProgressBar();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            SuspendLayout();
            // 
            // btnDosyasec
            // 
            btnDosyasec.Location = new Point(12, 52);
            btnDosyasec.Name = "btnDosyasec";
            btnDosyasec.Size = new Size(75, 23);
            btnDosyasec.TabIndex = 0;
            btnDosyasec.Text = "Dosya Seç";
            btnDosyasec.UseVisualStyleBackColor = true;
            btnDosyasec.Click += btnDosyasec_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(12, 25);
            label1.Name = "label1";
            label1.Size = new Size(16, 15);
            label1.TabIndex = 1;
            label1.Text = "...";
            // 
            // pictureBox1
            // 
            pictureBox1.Location = new Point(12, 104);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(801, 430);
            pictureBox1.TabIndex = 2;
            pictureBox1.TabStop = false;
            pictureBox1.Click += pictureBox1_Click;
            // 
            // timerVideo
            // 
            timerVideo.Tick += timerVideo_Tick;
            // 
            // txtAra
            // 
            txtAra.Location = new Point(957, 547);
            txtAra.Name = "txtAra";
            txtAra.Size = new Size(180, 23);
            txtAra.TabIndex = 3;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(862, 555);
            label2.Name = "label2";
            label2.Size = new Size(85, 15);
            label2.TabIndex = 4;
            label2.Text = "Metin Arama : ";
            // 
            // btnAra
            // 
            btnAra.Location = new Point(1143, 547);
            btnAra.Name = "btnAra";
            btnAra.Size = new Size(75, 23);
            btnAra.TabIndex = 5;
            btnAra.Text = "Ara";
            btnAra.UseVisualStyleBackColor = true;
            btnAra.Click += btnAra_Click;
            // 
            // lstAranankelimekarsilik
            // 
            lstAranankelimekarsilik.FormattingEnabled = true;
            lstAranankelimekarsilik.ItemHeight = 15;
            lstAranankelimekarsilik.Location = new Point(862, 110);
            lstAranankelimekarsilik.Name = "lstAranankelimekarsilik";
            lstAranankelimekarsilik.Size = new Size(468, 424);
            lstAranankelimekarsilik.TabIndex = 6;
            lstAranankelimekarsilik.SelectedIndexChanged += lstAranankelimekarsilik_SelectedIndexChanged;
            // 
            // progressBarvideo
            // 
            progressBarvideo.Location = new Point(12, 547);
            progressBarvideo.Name = "progressBarvideo";
            progressBarvideo.Size = new Size(801, 23);
            progressBarvideo.TabIndex = 7;
            progressBarvideo.Click += progressBarvideo_Click;
            // 
            // btnOynat
            // 
            btnOynat.Location = new Point(15, 578);
            btnOynat.Name = "btnOynat";
            btnOynat.Size = new Size(75, 23);
            btnOynat.TabIndex = 8;
            btnOynat.Text = "Oynat";
            btnOynat.UseVisualStyleBackColor = true;
            btnOynat.Click += btnOynat_Click;
            // 
            // btnDur
            // 
            btnDur.Location = new Point(111, 578);
            btnDur.Name = "btnDur";
            btnDur.Size = new Size(75, 23);
            btnDur.TabIndex = 9;
            btnDur.Text = "Dur";
            btnDur.UseVisualStyleBackColor = true;
            btnDur.Click += btnDur_Click;
            // 
            // btnDuraklat
            // 
            btnDuraklat.Location = new Point(202, 578);
            btnDuraklat.Name = "btnDuraklat";
            btnDuraklat.Size = new Size(75, 23);
            btnDuraklat.TabIndex = 10;
            btnDuraklat.Text = "Duraklat";
            btnDuraklat.UseVisualStyleBackColor = true;
            btnDuraklat.Click += btnDuraklat_Click;
            // 
            // btnYenidenoynat
            // 
            btnYenidenoynat.Location = new Point(298, 578);
            btnYenidenoynat.Name = "btnYenidenoynat";
            btnYenidenoynat.Size = new Size(100, 23);
            btnYenidenoynat.TabIndex = 11;
            btnYenidenoynat.Text = "Yeniden Oynat";
            btnYenidenoynat.UseVisualStyleBackColor = true;
            btnYenidenoynat.Click += btnYenidenoynat_Click;
            // 
            // lblSure
            // 
            lblSure.AutoSize = true;
            lblSure.Location = new Point(594, 578);
            lblSure.Name = "lblSure";
            lblSure.Size = new Size(49, 15);
            lblSure.TabIndex = 12;
            lblSure.Text = "00:00:00";
            lblSure.Click += lblSure_Click;
            // 
            // lblFbsbaslik
            // 
            lblFbsbaslik.AutoSize = true;
            lblFbsbaslik.Location = new Point(730, 60);
            lblFbsbaslik.Name = "lblFbsbaslik";
            lblFbsbaslik.Size = new Size(35, 15);
            lblFbsbaslik.TabIndex = 13;
            lblFbsbaslik.Text = "FPS : ";
            // 
            // lblFbs
            // 
            lblFbs.AutoSize = true;
            lblFbs.Location = new Point(761, 60);
            lblFbs.Name = "lblFbs";
            lblFbs.Size = new Size(19, 15);
            lblFbs.TabIndex = 14;
            lblFbs.Text = "00";
            lblFbs.Click += lblFbs_Click;
            // 
            // lblVideoSureUzunluk
            // 
            lblVideoSureUzunluk.AutoSize = true;
            lblVideoSureUzunluk.Location = new Point(764, 578);
            lblVideoSureUzunluk.Name = "lblVideoSureUzunluk";
            lblVideoSureUzunluk.Size = new Size(49, 15);
            lblVideoSureUzunluk.TabIndex = 15;
            lblVideoSureUzunluk.Text = "00:00:00";
            lblVideoSureUzunluk.Click += lblVideoSureUzunluk_Click;
            // 
            // timerAra
            // 
            timerAra.Interval = 40;
            timerAra.Tick += timerAra_TickAsync;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(498, 578);
            label3.Name = "label3";
            label3.Size = new Size(99, 15);
            label3.TabIndex = 16;
            label3.Text = "Video İlerlemesi : ";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(673, 578);
            label4.Name = "label4";
            label4.Size = new Size(92, 15);
            label4.TabIndex = 17;
            label4.Text = "Video Uzunluk : ";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(670, 658);
            label5.Name = "label5";
            label5.Size = new Size(85, 15);
            label5.TabIndex = 18;
            label5.Text = "Arama Süresi : ";
            // 
            // lblAraSure
            // 
            lblAraSure.AutoSize = true;
            lblAraSure.Location = new Point(761, 658);
            lblAraSure.Name = "lblAraSure";
            lblAraSure.Size = new Size(49, 15);
            lblAraSure.TabIndex = 19;
            lblAraSure.Text = "00:00:00";
            // 
            // progressBarAra
            // 
            progressBarAra.Location = new Point(12, 623);
            progressBarAra.Name = "progressBarAra";
            progressBarAra.Size = new Size(801, 23);
            progressBarAra.TabIndex = 20;
            progressBarAra.Click += progressBar1_Click;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1370, 693);
            Controls.Add(progressBarAra);
            Controls.Add(lblAraSure);
            Controls.Add(label5);
            Controls.Add(label4);
            Controls.Add(label3);
            Controls.Add(lblVideoSureUzunluk);
            Controls.Add(lblFbs);
            Controls.Add(lblFbsbaslik);
            Controls.Add(lblSure);
            Controls.Add(btnYenidenoynat);
            Controls.Add(btnDuraklat);
            Controls.Add(btnDur);
            Controls.Add(btnOynat);
            Controls.Add(progressBarvideo);
            Controls.Add(lstAranankelimekarsilik);
            Controls.Add(btnAra);
            Controls.Add(label2);
            Controls.Add(txtAra);
            Controls.Add(pictureBox1);
            Controls.Add(label1);
            Controls.Add(btnDosyasec);
            Name = "Form1";
            Text = "Video Kelime Bulucu";
            Load += Form1_Load;
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button btnDosyasec;
        private Label label1;
        private PictureBox pictureBox1;
        private System.Windows.Forms.Timer timerVideo;
        private TextBox txtAra;
        private Label label2;
        private Button btnAra;
        private ListBox lstAranankelimekarsilik;
        private ProgressBar progressBarvideo;
        private Button btnOynat;
        private Button btnDur;
        private Button btnDuraklat;
        private Button btnYenidenoynat;
        private Label lblSure;
        private Label lblFbsbaslik;
        private Label lblFbs;
        private Label lblVideoSureUzunluk;
        private System.Windows.Forms.Timer timerAra;
        private Label label3;
        private Label label4;
        private Label label5;
        private Label lblAraSure;
        private ProgressBar progressBarAra;
    }
}