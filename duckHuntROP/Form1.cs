using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace duckHuntROP
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        protected override CreateParams CreateParams
        {
            get
            {
                var cp = base.CreateParams;
                cp.ExStyle |= 0x02000000;    // Double-buffering pro vic smooth obrazky
                return cp;
            }
        }

        private PictureBox PlayPB;
        new public int Width { get; set; }
        new public int Height { get; set; }
        private void Form1_Load(object sender, EventArgs e)
        {
            this.BackgroundImage = Properties.Resources.bckImageNight;
            this.Size = Screen.PrimaryScreen.Bounds.Size;
            
            Width = this.Size.Width;
            Height = this.Size.Height;

            PlayPB = new PictureBox();
            PlayPB.Size = new Size(Width/10,Height/10);
            PlayPB.Location = new Point(Width/4,Height/16);
            PlayPB.BackgroundImage = Properties.Resources.huntButton;
            PlayPB.BackgroundImageLayout = ImageLayout.Stretch;
            PlayPB.BackColor = Color.Transparent;
            PlayPB.Click += new EventHandler(Hunt_Click);
            Controls.Add(PlayPB);

        }
        private void Hunt_Click(object sender, EventArgs e)
        {
            PlayPB.Hide();
            this.BackgroundImage = Properties.Resources.bckImage;
        }
    }
}
