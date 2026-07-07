using System.Drawing.Drawing2D;

namespace VideoKelimeArama
{
    // Uygulamanın renk paleti (koyu tema)
    internal static class Tema
    {
        public static readonly Color Arka = Color.FromArgb(24, 24, 37);
        public static readonly Color Yuzey = Color.FromArgb(30, 30, 46);
        public static readonly Color YuzeyAcik = Color.FromArgb(42, 42, 62);
        public static readonly Color Vurgu = Color.FromArgb(137, 116, 255);
        public static readonly Color VurguKoyu = Color.FromArgb(98, 82, 200);
        public static readonly Color Turkuaz = Color.FromArgb(0, 176, 155);
        public static readonly Color Kirmizi = Color.FromArgb(220, 76, 76);
        public static readonly Color Metin = Color.FromArgb(232, 232, 242);
        public static readonly Color MetinSoluk = Color.FromArgb(140, 146, 168);
        public static readonly Color VideoArka = Color.FromArgb(10, 10, 16);
    }

    internal static class CizimYardimci
    {
        public static GraphicsPath YuvarlakYol(Rectangle alan, int yaricap)
        {
            int cap = yaricap * 2;
            var yol = new GraphicsPath();
            if (cap <= 0 || alan.Width < cap || alan.Height < cap)
            {
                yol.AddRectangle(alan);
                return yol;
            }

            yol.AddArc(alan.X, alan.Y, cap, cap, 180, 90);
            yol.AddArc(alan.Right - cap, alan.Y, cap, cap, 270, 90);
            yol.AddArc(alan.Right - cap, alan.Bottom - cap, cap, cap, 0, 90);
            yol.AddArc(alan.X, alan.Bottom - cap, cap, cap, 90, 90);
            yol.CloseFigure();
            return yol;
        }
    }

    // Yuvarlatılmış köşeli, düz renkli, üzerine gelince aydınlanan düğme
    public class ModernButton : Button
    {
        private bool uzerinde;
        private bool basili;
        private Color normalRenk = Tema.YuzeyAcik;

        public Color NormalRenk
        {
            get => normalRenk;
            set { normalRenk = value; Invalidate(); }
        }

        public int Yaricap { get; set; } = 8;

        public ModernButton()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.UserPaint | ControlStyles.ResizeRedraw, true);
            FlatStyle = FlatStyle.Flat;
            FlatAppearance.BorderSize = 0;
            ForeColor = Tema.Metin;
            Cursor = Cursors.Hand;
        }

        private static Color Karistir(Color renk, Color hedef, double oran)
        {
            return Color.FromArgb(
                (int)(renk.R + (hedef.R - renk.R) * oran),
                (int)(renk.G + (hedef.G - renk.G) * oran),
                (int)(renk.B + (hedef.B - renk.B) * oran));
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            e.Graphics.Clear(Parent?.BackColor ?? Tema.Arka);

            Color arka = !Enabled ? Karistir(normalRenk, Tema.Yuzey, 0.6)
                       : basili ? Karistir(normalRenk, Color.Black, 0.2)
                       : uzerinde ? Karistir(normalRenk, Color.White, 0.12)
                       : normalRenk;
            Color yazi = Enabled ? ForeColor : Tema.MetinSoluk;

            using var yol = CizimYardimci.YuvarlakYol(new Rectangle(0, 0, Width - 1, Height - 1), Yaricap);
            using var firca = new SolidBrush(arka);
            e.Graphics.FillPath(firca, yol);

            TextRenderer.DrawText(e.Graphics, Text, Font, ClientRectangle, yazi,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);
        }

        protected override void OnMouseEnter(EventArgs e) { uzerinde = true; Invalidate(); base.OnMouseEnter(e); }
        protected override void OnMouseLeave(EventArgs e) { uzerinde = false; basili = false; Invalidate(); base.OnMouseLeave(e); }
        protected override void OnMouseDown(MouseEventArgs e) { basili = true; Invalidate(); base.OnMouseDown(e); }
        protected override void OnMouseUp(MouseEventArgs e) { basili = false; Invalidate(); base.OnMouseUp(e); }
        protected override void OnEnabledChanged(EventArgs e) { Invalidate(); base.OnEnabledChanged(e); }
    }

    // Hem arama ilerlemesi hem video sarma çubuğu olarak kullanılan modern çubuk.
    // ProgressBar ile aynı Maximum/Value arayüzünü sunar; değerleri taşmaya
    // karşı kendisi sınırlar.
    public class ModernBar : Control
    {
        private int maximum = 100;
        private int deger;

        public int Maximum
        {
            get => maximum;
            set
            {
                maximum = Math.Max(1, value);
                deger = Math.Min(deger, maximum);
                Invalidate();
            }
        }

        public int Value
        {
            get => deger;
            set { deger = Math.Clamp(value, 0, maximum); Invalidate(); }
        }

        public Color DoluRenk { get; set; } = Tema.Vurgu;
        public Color YolRenk { get; set; } = Tema.YuzeyAcik;
        public bool TutamacGoster { get; set; }

        public ModernBar()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.UserPaint | ControlStyles.ResizeRedraw, true);
            Height = 16;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            e.Graphics.Clear(Parent?.BackColor ?? Tema.Arka);

            const int yolKalinlik = 6;
            int yolY = (Height - yolKalinlik) / 2;

            using (var yol = CizimYardimci.YuvarlakYol(new Rectangle(0, yolY, Width - 1, yolKalinlik), yolKalinlik / 2))
            using (var firca = new SolidBrush(YolRenk))
            {
                e.Graphics.FillPath(firca, yol);
            }

            int doluGen = (int)((long)(Width - 1) * deger / maximum);
            if (doluGen > yolKalinlik)
            {
                using var dolu = CizimYardimci.YuvarlakYol(new Rectangle(0, yolY, doluGen, yolKalinlik), yolKalinlik / 2);
                using var firca = new SolidBrush(DoluRenk);
                e.Graphics.FillPath(firca, dolu);
            }

            if (TutamacGoster)
            {
                const int yaricap = 7;
                int x = Math.Clamp(doluGen - yaricap, 0, Math.Max(0, Width - yaricap * 2 - 1));
                using var firca = new SolidBrush(Color.White);
                e.Graphics.FillEllipse(firca, x, Height / 2 - yaricap, yaricap * 2, yaricap * 2);
            }
        }
    }
}
