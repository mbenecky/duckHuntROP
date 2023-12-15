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
        private Panel FlyZone;
        private Panel BulletZone;
        private Panel ShopPanel;

        private Timer FlyTimer;
        
        private PictureBox ShopPB;
        private PictureBox PlayPB;
        //pictureboxes fungujici jako buttony


        private Gun CurrentGun;
        private List<Gun> UnlockedGuns =new List<Gun>();
        private List<Gun> AllGuns;

        private List<Image> HurtLevels = new List<Image>();

        public int Level = 4;
        private int Coins = 50;
        new public int Width { get; set; }
        new public int Height { get; set; }
        private void Form1_Load(object sender, EventArgs e)
        {
            this.BackgroundImage = Properties.Resources.bckImageNight;
            this.Size = Screen.PrimaryScreen.Bounds.Size;

            this.KeyDown += new KeyEventHandler(Form_KeyDown);

            Width = this.Size.Width;
            Height = this.Size.Height;

            PlayPB = new PictureBox();
            PlayPB.Size = new Size(Width / 10, Height / 10);
            PlayPB.Location = new Point(Width / 32, Height / 12);
            PlayPB.BackgroundImage = Properties.Resources.huntButton;
            PlayPB.BackgroundImageLayout = ImageLayout.Stretch;
            PlayPB.BackColor = Color.Transparent;
            PlayPB.Click += new EventHandler(Hunt_Click);

            ShopPB = new PictureBox();
            ShopPB.Size =new Size(Width / 10, Height / 10);
            ShopPB.Location = new Point(Width / 32, Height / 12 + Height / 8);
            ShopPB.BackgroundImage = Properties.Resources.huntButton;
            ShopPB.BackgroundImageLayout = ImageLayout.Stretch;
            ShopPB.BackColor = Color.Transparent;
            ShopPB.Click += new EventHandler(Shop_Click);


            FlyZone = new Panel();
            FlyZone.Size = new Size(Width, Height - Height / 4);
            FlyZone.Location = new Point(0, Height / 11);
            FlyZone.BackColor = Color.Transparent;
            FlyZone.Hide();

            ShopPanel = new Panel();
            ShopPanel.Size = new Size(Width-Width/6, Height - Height / 4);
            ShopPanel.Location = new Point(Width/6, Height / 11);
            ShopPanel.BackColor = Color.Red;
            ShopPanel.Hide();

            


            EndZone = new Panel();
            EndZone.Size = new Size(Width / 32, FlyZone.Height);
            EndZone.Location = new Point(FlyZone.Width - FlyZone.Width / 32, 0);
            EndZone.BackColor = Color.Transparent;
            FlyZone.Controls.Add(EndZone);

            FlyTimer = new Timer();
            FlyTimer.Interval = 100;
            FlyTimer.Tick += new EventHandler(FlyTimer_Tick);

            BulletZone = new Panel();
            BulletZone.Size = new Size(Width / 4, Height / 16);
            BulletZone.Location = new Point(0, Height - Height /13);
            BulletZone.BackColor = Color.Transparent;
            BulletZone.Hide();


            Controls.Add(ShopPanel);
            Controls.Add(FlyZone);
            Controls.Add(BulletZone);
            Controls.Add(PlayPB);
            Controls.Add(ShopPB);


            HurtLevels.Add(Properties.Resources.Hurt1);
            HurtLevels.Add(Properties.Resources.Hurt2);
            HurtLevels.Add(Properties.Resources.Hurt3);
            AllGuns = Gun.CreateGuns();
            UnlockedGuns.Add(AllGuns[0]);
            CurrentGun = AllGuns[0];
            for (int i = 0; i != AllGuns.Count; i++)
            {
                PictureBox gunPB = new PictureBox();
                gunPB.Size = new Size(Width / 4, Height / 20);
                gunPB.Location = new Point(Width / 8, Height / 16 * i + Height / 16);
                gunPB.BackgroundImage = AllGuns[i].Img;
                gunPB.BackgroundImageLayout = ImageLayout.Stretch;
                gunPB.BackColor = Color.Transparent;
                gunPB.Click += new EventHandler(Gun_Buy);
                gunPB.Tag = i.ToString();
                ShopPanel.Controls.Add(gunPB);
                gunPB.Show();
            }
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
            BulletZone.Show();
            PlayPB.Hide();
            ShopPanel.Hide();
            this.BackgroundImage = Properties.Resources.bckImage;
            SpawnDucks();
            CreateBullets();
            FlyTimer.Start();
        }
        public void End(bool win)
        {
            this.BackgroundImage = Properties.Resources.bckImageNight;
            FlyTimer.Stop();
            PlayPB.Show();
            FlyZone.Hide();
            DiscardBullets();
            BulletZone.Hide();
            CurrentGun.CurrentAmmo = CurrentGun.MaxAmmo;
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
        public void Recoil()
        {
            Random rnd = new Random();
            this.Cursor = Cursor.Current;
            Cursor.Position = new Point(Cursor.Position.X + rnd.Next(-Width/20,Width/20), Cursor.Position.Y-rnd.Next(0,Height/20) );
            //pozdeji zmenit na currentgun.recoil nebo pridat parametr s recoilem :-)
        }
        public void SpawnDucks()
        {
            Random rnd = new Random();
            foreach (Duck Dck in Duck.CreateDucks(Level))
            {
                PictureBox DuckPB = new PictureBox();
                DuckPB.BackgroundImage = Dck.Img;
                DuckPB.Location = new Point(FlyZone.Width / 32, FlyZone.Height - rnd.Next(FlyZone.Height / 16, FlyZone.Height));
                DuckPB.Size = new Size(Width / 16, Height / 16);
                DuckPB.Click += new EventHandler(Duck_Click);
                DuckPB.BackgroundImageLayout = ImageLayout.Stretch;
                DuckPB.Name = Dck.Speed.ToString();
                DuckPB.BackColor = Color.Transparent;
                DuckPB.Tag = Dck.Health + "|" + Dck.Coins+"|"+Dck.HurtLevel;
                FlyZone.Controls.Add(DuckPB);
            }
        }
        private async void Duck_Click(object sender, EventArgs e)
        {
            // Ammo check
            if (CurrentGun != null && CurrentGun.CanShoot())
            {
                PictureBox pb = (sender as PictureBox);
                CurrentGun.Shoot();
                Recoil();
                ChangeBullets(false);
                if(pb.Tag!= null)
                {
                    string[] pbParams = pb.Tag.ToString().Split('|');
                    int CurrentHealth = Convert.ToInt32(pbParams[0]);
                    int CurrentCoins = Convert.ToInt32(pbParams[1]);
                    int CurrentHurtLevel = Convert.ToInt32(pbParams[2]);
                    if(CurrentHealth - CurrentGun.Damage <=0)
                    {
                        Coins += CurrentCoins;
                        pb.Click -= Duck_Click;
                        pb.Name = "0";
                        pb.Image = Properties.Resources.duckEnd;
                        await Task.Run(async () =>
                        {
                            await Task.Delay(100);
                            BeginInvoke(new Action(() => pb.Dispose()));
                        });
                    } else
                    {
                        pb.SizeMode = PictureBoxSizeMode.StretchImage;
                        if(CurrentHurtLevel!=HurtLevels.Count)
                        {
                            CurrentHurtLevel++;
                        }
                        pb.Image = HurtLevels[CurrentHurtLevel-1];
                        pb.Tag = (CurrentHealth - CurrentGun.Damage).ToString() +"|"+CurrentCoins.ToString()+"|"+CurrentHurtLevel.ToString();
                       
                    }
                }
            }
       }

        
        private void CreateBullets()
        {
            for(int i = 0;i != CurrentGun.MaxAmmo;i++)
            {
                Panel Bullet = new Panel();
                Bullet.Size = new Size(BulletZone.Width/15, BulletZone.Height);
                Bullet.Location = new Point(BulletZone.Width/10*i+BulletZone.Width/20,0);
                Bullet.BackColor = Color.DarkGoldenrod;
                BulletZone.Controls.Add(Bullet);
            }
        }
        private void Form_KeyDown(object sender,KeyEventArgs e)
        {
            if(e.KeyData == Keys.R && FlyZone.Visible)
            {
                CurrentGun.Reload(); 
                if (CurrentGun.CurrentAmmo != CurrentGun.MaxAmmo)
                {
                    ChangeBullets(true);
                } //if je pojisteni pred moznym bugem kdy uzivatel klikne R pred tim nez dobehne v CurrentGun.Shoot delayROF
            }
        }
        private void Shop_Click(object sender, EventArgs e)
        {
            if(FlyZone.Visible == false)
            {
                ShopPanel.Visible = !ShopPanel.Visible;
            }
        }
        private void Gun_Buy(object sender,EventArgs e)
        {
            Gun gun = AllGuns[Convert.ToInt32((sender as PictureBox).Tag)];
            if (!UnlockedGuns.Contains(gun))
            {
                if (Coins >= gun.Cost)
                {
                    Coins -= gun.Cost;
                    UnlockedGuns.Add(gun);
                    MessageBox.Show("koupil");
                }
            } else
            {
                CurrentGun = gun;
                gun.CurrentAmmo = gun.MaxAmmo;
                MessageBox.Show("Vlastni");
            }
            
        }
        private void DiscardBullets()
        {
            while(BulletZone.Controls.Count > 0)
            {
                BulletZone.Controls.RemoveAt(0); 
            }
        }
        private async Task ChangeBullets(bool Plus)
        {
            if(Plus)
            {
                foreach(Control s in BulletZone.Controls)
                {
                    if(s is Panel)
                    {
                        await Task.Run(() => Task.Delay(CurrentGun.DelayReload / CurrentGun.MaxAmmo));
                        (s as Panel).BackColor = Color.DarkGoldenrod;
                    }
                }
            } else
            {
                for(int i = BulletZone.Controls.Count - 1; i >= 0; i--)
                {
                    if(BulletZone.Controls[i] is Panel)
                    {
                        if((BulletZone.Controls[i] as Panel).BackColor == Color.DarkGoldenrod)
                        {
                            (BulletZone.Controls[i] as Panel).BackColor = Color.LightGoldenrodYellow;
                            break;
                        }
                    }
                }
            }
        }
        
    }
}