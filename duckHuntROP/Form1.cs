using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Data;
using System.Drawing;
using System.Drawing.Text;
using System.Linq;
using System.Reflection;
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
        private Panel EndZone;
        private PictureBox PlayPB;
        private Panel FlyZone;
        private Timer FlyTimer;
        private Gun CurrentGun;
        private List<Gun> UnlockedGuns =new List<Gun>();
        private List<Gun> AllGuns;
        public int Level = 4;
        new public int Width { get; set; }
        new public int Height { get; set; }
        private void Form1_Load(object sender, EventArgs e)
        {
            this.BackgroundImage = Properties.Resources.bckImageNight;
            this.Size = Screen.PrimaryScreen.Bounds.Size;

            Width = this.Size.Width;
            Height = this.Size.Height;

            PlayPB = new PictureBox();
            PlayPB.Size = new Size(Width / 10, Height / 10);
            PlayPB.Location = new Point(Width / 32, Height / 12);
            PlayPB.BackgroundImage = Properties.Resources.huntButton;
            PlayPB.BackgroundImageLayout = ImageLayout.Stretch;
            PlayPB.BackColor = Color.Transparent;
            PlayPB.Click += new EventHandler(Hunt_Click);

            FlyZone = new Panel();
            FlyZone.Size = new Size(Width, Height - Height / 4);
            FlyZone.Location = new Point(0, Height / 11);
            FlyZone.BackColor = Color.Transparent;
            FlyZone.Hide();

            EndZone = new Panel();
            EndZone.Size = new Size(Width / 32, FlyZone.Height);
            EndZone.Location = new Point(FlyZone.Width - FlyZone.Width / 32, 0);
            EndZone.BackColor = Color.Transparent;
            FlyZone.Controls.Add(EndZone);

            FlyTimer = new Timer();
            FlyTimer.Interval = 100;
            FlyTimer.Tick += new EventHandler(FlyTimer_Tick);
            Controls.Add(FlyZone);
            Controls.Add(PlayPB);



            AllGuns = Gun.CreateGuns();
            UnlockedGuns.Add(AllGuns[0]);
            CurrentGun = AllGuns[0];
        }
        private void FlyTimer_Tick(object sender, EventArgs e)
        {
            foreach (object ob in FlyZone.Controls)
            {
                if (FlyZone.Controls.Count == 1)
                {
                    End(true);
                }
                if (ob is PictureBox)
                {
                    PictureBox pb = (ob as PictureBox);
                    if (pb.Bounds.IntersectsWith(EndZone.Bounds))
                    {
                        End(false);
                    }
                    pb.Location = new Point(Convert.ToInt32(pb.Name) + pb.Location.X, pb.Location.Y);
                }
            }
        }
        private void Hunt_Click(object sender, EventArgs e)
        {
            FlyZone.Show();
            PlayPB.Hide();
            this.BackgroundImage = Properties.Resources.bckImage;
            SpawnDucks();
            FlyTimer.Start();
        }
        public void End(bool win)
        {
            this.BackgroundImage = Properties.Resources.bckImageNight;
            FlyTimer.Stop();
            PlayPB.Show();
            FlyZone.Hide();

            if (win)
            {
                Level++;
            }
            else
            {
                for (int i = 0; i != FlyZone.Controls.Count; i++)
                {

                    object ob = FlyZone.Controls[i];
                    if (ob is PictureBox)
                    {
                        (ob as PictureBox).Dispose();
                        i--;
                    }
                }
            }
        }

        public void SpawnDucks()
        {
            Random rnd = new Random();
            foreach (Duck Dck in CreateDucks())
            {
                PictureBox DuckPB = new PictureBox();
                DuckPB.BackgroundImage = Dck.Img;
                DuckPB.Location = new Point(FlyZone.Width / 32, FlyZone.Height - rnd.Next(FlyZone.Height / 16, FlyZone.Height));
                DuckPB.Size = new Size(Width / 16, Height / 16);
                DuckPB.Click += new EventHandler(Duck_Click);
                DuckPB.BackgroundImageLayout = ImageLayout.Stretch;
                DuckPB.Name = Dck.Speed.ToString();
                DuckPB.BackColor = Color.Transparent;
                DuckPB.Tag = Dck.Health + "|" + Dck.Coins;
                FlyZone.Controls.Add(DuckPB);
            }
        }
        private void Duck_Click(object sender, EventArgs e)
        {
            //ammo check



            //Smrt kachny   :-(
            //  I
            //  I
            //  V
            PictureBox pb = (sender as PictureBox);
            pb.Click -= Duck_Click;
            pb.Name = "0";
            pb.Image = Properties.Resources.duckEnd;
            Task.Factory.StartNew(async () => {
                await Task.Delay(100);
                BeginInvoke(new Action(() => pb.Dispose()));
            });

            //anonymni funkce v asynchronni anonymni funkci
        }

        public List<Duck> CreateDucks()
        {
            Random rnd = new Random();
            List<Duck> list = new List<Duck>();
            int till;
            if (Level % 5 == 0)
            {
                till = Level / 5;
            }
            else { till = Level; }
            for (int i = 0; i != till; i++)
            {
                list.Add(new Duck(this.Level, rnd));
            }
            return list;
        }
    }
}