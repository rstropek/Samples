using System;
using System.Drawing;
using System.Windows.Forms;

namespace _06._01_WinForms_Rendering
{
    public partial class Form1 : Form
    {
        //private Timer ticker;

        public Form1()
        {
            InitializeComponent();

            #region Ticker
            /*
            ticker = new Timer();
            ticker.Interval = 1000;
            ticker.Tick += new EventHandler(ticker_Tick);
            ticker.Enabled = true;
            */
            #endregion
        }

        #region Tick
        /*
        void ticker_Tick(object sender, EventArgs e)
        {
            Invalidate(new Rectangle(5, 5, 75, 15));
        }
        */
        #endregion

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.FillRectangle(Brushes.Gray, e.ClipRectangle);
            e.Graphics.DrawRectangle(Pens.Black, e.ClipRectangle.X, e.ClipRectangle.Y,
              e.ClipRectangle.Width - 1, e.ClipRectangle.Height - 1);

            e.Graphics.DrawString(DateTime.Now.ToString("hh:mm:ss"),
              SystemFonts.DialogFont, Brushes.Black, 5, 5);

            base.OnPaint(e);
        }
    }
}
