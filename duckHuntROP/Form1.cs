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
using System.Media;

namespace duckHuntROP
{
    public partial class DuckHunt : Form
    {
        public DuckHunt()
        {
            InitializeComponent();
            this.Size = Screen.PrimaryScreen.Bounds.Size;
        }
        public DuckHunt(int resX, int resY)
        {
            InitializeComponent();
            this.Size = new Size(resX, resY);
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
        private Panel EndPanel;
        private Panel MenuPanel;
        private Panel StartPanel;
        private Panel OptionsPanel;
        private Panel ScorePanel;

        private ScoreTable st;

        private Timer FlyTimer;

        private PictureBox ShopPB;
        private PictureBox PlayPB;
        private PictureBox NewGamePB;
        private PictureBox BackPB;
        public static PictureBox ContinuePB;
        private PictureBox OptionsPB;
        private PictureBox SoundPB;
        private PictureBox LoadPB;
        private PictureBox SavePB;
        private PictureBox ApplyPB;
        private PictureBox ScorePB;
        private PictureBox EndPBStart;
        private PictureBox EndPBMenu;

        //pictureboxes fungujici jako buttony
        
        private PictureBox CoinPB;
        private Label CoinLabel;

        private TextBox LoadNameTB;
        private ComboBox SizePickerCB;

        private PictureBox kReloadPB;
        private PictureBox kEndPB;
        private PictureBox kBackPB;



        public static Keys kReload;
        public static Keys kEnd;
        public static Keys kBack;

        private bool kListening = false;
        private bool PlaySound = true;

        public static Gun CurrentGun;
        public static List<Gun> AllGuns;
        private List<Image> HurtLevels = new List<Image>();

        private List<Image> LockedGuns = new List<Image>();
        private List<Image> SelectedGuns = new List<Image>();

        public static Game CurrentGame;

        SoundPlayer sp = new SoundPlayer(Properties.Resources.gunshot);
        SoundPlayer sp1 = new SoundPlayer(Properties.Resources.reload1);
        SoundPlayer sp2 = new SoundPlayer(Properties.Resources.reload2);

        public static string NameGame;
        public static int Level = 1;
        public static int Coins = 50;
        private int CoinsPerRound = 0;
        public static List<Gun> UnlockedGuns = new List<Gun>();

        //tohle zmenit kdyz chci novou hru a mam vlastne novou hru

        //Co vsechno potrebuju ulozit pri loadovani hry?
        //1. Level
        //2. Coins
        //3. UnlockedGuns
        //4. CurrentGun?

        //Proc ne lockedGuns?
        //LockedGuns je jen UnlockedGuns.Intersect(AllGuns);
        //teda myslim :-D


        new public int Width { get; set; }
        new public int Height { get; set; }
        private void Form1_Load(object sender, EventArgs e)
        {
            this.BackgroundImage = Properties.Resources.bckImageNight;
            this.KeyDown += new KeyEventHandler(Form_KeyDown);
            this.KeyPreview = true;
            Width = this.Size.Width;
            Height = this.Size.Height;


            CurrentGame = new Game(true);
            MenuPanel = new Panel();
            MenuPanel.Size = new Size(Width / 6, Height - Height / 4);
            MenuPanel.Location = new Point(0, Height / 11);
            MenuPanel.BackColor = Color.Transparent;
            MenuPanel.Hide();

            st = new ScoreTable(this.Size);

            StartPanel = new Panel();
            StartPanel.Size = new Size(Width / 2, Height - Height / 4);
            StartPanel.Location = MenuPanel.Location;
            StartPanel.BackColor = Color.Transparent;
            StartPanel.Show();

            OptionsPanel = new Panel();
            OptionsPanel.Size = new Size(Width / 2 - Width / 6, Height - Height / 4);
            OptionsPanel.Location = new Point(Width / 6, 0);
            OptionsPanel.BackColor = Color.Transparent;
            OptionsPanel.Hide();

            ScorePanel = new Panel();
            ScorePanel.Size = new Size(Width / 2 - Width / 6, Height - Height / 4);
            ScorePanel.Location = new Point(Width / 6, 0);
            ScorePanel.BackColor = Color.Transparent;
            ScorePanel.Hide();

            PlayPB = new PictureBox();
            PlayPB.Size = new Size(Width / 10, Height / 10);
            PlayPB.Location = new Point(Width / 32, Height / 12);
            PlayPB.BackgroundImage = Properties.Resources.huntButton;
            PlayPB.BackgroundImageLayout = ImageLayout.Stretch;
            PlayPB.BackColor = Color.Transparent;
            PlayPB.Click += new EventHandler(Hunt_Click);

            ShopPB = new PictureBox();
            ShopPB.Size = new Size(Width / 10, Height / 10);
            ShopPB.Location = new Point(Width / 32, Height / 12 + Height / 8);
            ShopPB.BackgroundImage = Properties.Resources.shopButton;
            ShopPB.BackgroundImageLayout = ImageLayout.Stretch;
            ShopPB.BackColor = Color.Transparent;
            ShopPB.Click += new EventHandler(Shop_Click);

            BackPB = new PictureBox();
            BackPB.Size = new Size(Width / 10, Height / 20);
            BackPB.Location = new Point(Width / 32, Height / 40);
            BackPB.BackgroundImage = Properties.Resources.backButton;
            BackPB.BackgroundImageLayout = ImageLayout.Stretch;
            BackPB.BackColor = Color.Transparent;
            BackPB.Click += new EventHandler(Back_Click);

            ContinuePB = new PictureBox();
            ContinuePB.Size = new Size(Width / 10, Height / 20);
            ContinuePB.Location = new Point(Width / 32, Height / 40);
            ContinuePB.BackgroundImage = Properties.Resources.playButton;
            ContinuePB.BackgroundImageLayout = ImageLayout.Stretch;
            ContinuePB.BackColor = Color.Transparent;
            ContinuePB.Visible = false;
            ContinuePB.Click += new EventHandler(Continue_Click);

            NewGamePB = new PictureBox();
            NewGamePB.Size = new Size(Width / 10, Height / 10);
            NewGamePB.Location = PlayPB.Location;
            NewGamePB.BackgroundImage = Properties.Resources.newButton;
            NewGamePB.BackgroundImageLayout = ImageLayout.Stretch;
            NewGamePB.BackColor = Color.Transparent;
            NewGamePB.Click += new EventHandler(NewGame_Click);

            OptionsPB = new PictureBox();
            OptionsPB.Size = NewGamePB.Size;
            OptionsPB.Location = ShopPB.Location;
            OptionsPB.BackgroundImage = Properties.Resources.optionsButton;
            OptionsPB.BackgroundImageLayout = ImageLayout.Stretch;
            OptionsPB.BackColor = Color.Transparent;
            OptionsPB.Click += new EventHandler(Options_Click);

            EndPBStart = new PictureBox();
            EndPBStart.Size = NewGamePB.Size;
            EndPBStart.Location = new Point(ShopPB.Location.X, Height / 2);
            EndPBStart.BackgroundImage = Properties.Resources.leaveButton;
            EndPBStart.BackgroundImageLayout = ImageLayout.Stretch;
            EndPBStart.BackColor = Color.Transparent;
            EndPBStart.Click += new EventHandler(End_Click);

            EndPBMenu = new PictureBox();
            EndPBMenu.Size = EndPBStart.Size;
            EndPBMenu.Location = EndPBStart.Location;
            EndPBMenu.BackgroundImage = Properties.Resources.leaveButton;
            EndPBMenu.BackgroundImageLayout = ImageLayout.Stretch;
            EndPBMenu.BackColor = Color.Transparent;
            EndPBMenu.Click += new EventHandler(End_Click);

            ScorePB = new PictureBox();
            ScorePB.Size = NewGamePB.Size;
            ScorePB.Location = new Point(Width / 32, Height / 12 + Height / 4);
            ScorePB.BackgroundImage = Properties.Resources.score;
            ScorePB.BackgroundImageLayout = ImageLayout.Stretch;
            ScorePB.BackColor = Color.Transparent;
            ScorePB.Click += new EventHandler(Score_Click);

            kReloadPB = new PictureBox();
            kReloadPB.Size = new Size(Width / 16, Width / 16);
            kReloadPB.Location = new Point(Width / 16, Height / 16);
            kReloadPB.Name = "Reload";
            kReloadPB.Image = Properties.Resources.rKey;
            kReloadPB.SizeMode = PictureBoxSizeMode.StretchImage;
            kReloadPB.BackgroundImage = Properties.Resources.reloadKey;
            kReloadPB.BackgroundImageLayout = ImageLayout.Stretch;
            kReloadPB.Tag = "0";
            kReloadPB.Click += new EventHandler(Global_Click);

            kEndPB = new PictureBox();
            kEndPB.Size = kReloadPB.Size;
            kEndPB.Location = new Point(Width / 16 + kEndPB.Size.Width, Height / 16);
            kEndPB.Name = "End";
            kEndPB.Image = Properties.Resources.escapeKey;
            kEndPB.SizeMode = PictureBoxSizeMode.StretchImage;
            kEndPB.BackgroundImage = Properties.Resources.endKey;
            kEndPB.BackgroundImageLayout = ImageLayout.Stretch;
            kEndPB.Tag = "0";
            kEndPB.Click += new EventHandler(Global_Click);

            kBackPB = new PictureBox();
            kBackPB.Size = kReloadPB.Size;
            kBackPB.Location = new Point(Width / 16 + kBackPB.Size.Width * 2, Height / 16);
            kBackPB.Name = "Back";
            kBackPB.Image = Properties.Resources.leftKey;
            kBackPB.SizeMode = PictureBoxSizeMode.StretchImage;
            kBackPB.BackgroundImage = Properties.Resources.backKey;
            kBackPB.BackgroundImageLayout = ImageLayout.Stretch;
            kBackPB.Tag = "0";
            kBackPB.Click += new EventHandler(Global_Click);

            SoundPB = new PictureBox();
            SoundPB.Size = new Size(Width / 8, Height / 8);
            SoundPB.Location = new Point(Width / 16, Height / 4);
            SoundPB.BackgroundImageLayout = ImageLayout.Stretch;
            SoundPB.BackgroundImage = Properties.Resources.soundOnButton;
            SoundPB.Click += new EventHandler(Sound_Click);

            SavePB = new PictureBox();
            SavePB.Size = new Size(Width / 12, Height / 12);
            SavePB.Location = new Point(Width / 8, Height / 2);
            SavePB.BackgroundImage = Properties.Resources.saveButton;
            SavePB.BackgroundImageLayout = ImageLayout.Stretch;
            SavePB.Click += new EventHandler(Save_Click);

            LoadPB = new PictureBox();
            LoadPB.Size = new Size(Width / 12, Height / 12);
            LoadPB.Location = new Point(Width / 64 + Width / 8 +Width/10, Height / 2);
            LoadPB.BackgroundImage = Properties.Resources.loadButton;
            LoadPB.BackgroundImageLayout = ImageLayout.Stretch;
            LoadPB.Click += new EventHandler(Load_Click);

            LoadNameTB = new TextBox();
            LoadNameTB.Size = new Size(Width / 12, Height / 2);
            LoadNameTB.Location = new Point(Width / 64, Height / 2);
            LoadNameTB.Text = "Name";
            LoadNameTB.TabStop = false;

            ApplyPB = new PictureBox();
            ApplyPB.Size = new Size(Width / 24, Height / 24);
            ApplyPB.Location = new Point(Width/64 + Width/24+Width/8, Height / 2);
            ApplyPB.BackgroundImage = Properties.Resources.saveButton;
            ApplyPB.BackgroundImageLayout = ImageLayout.Stretch;
            ApplyPB.Click += new EventHandler(Size_Click);

            SizePickerCB = new ComboBox();
            SizePickerCB.Items.Add("640x360");
            SizePickerCB.Items.Add("960x540");
            SizePickerCB.Items.Add("1280x720");
            SizePickerCB.Items.Add("1600x900");
            SizePickerCB.Items.Add("1920x1080");
            SizePickerCB.Size = new Size(Width / 12, Height / 24);
            SizePickerCB.Location = new Point(Width / 16, Height/2);
            SizePickerCB.DropDownStyle = ComboBoxStyle.DropDownList;

            MenuPanel.Controls.Add(BackPB);
            MenuPanel.Controls.Add(PlayPB);
            MenuPanel.Controls.Add(ShopPB);
            MenuPanel.Controls.Add(EndPBMenu);

            StartPanel.Controls.Add(NewGamePB);
            StartPanel.Controls.Add(OptionsPB);
            StartPanel.Controls.Add(OptionsPanel);
            StartPanel.Controls.Add(ContinuePB);
            StartPanel.Controls.Add(ScorePB);
            StartPanel.Controls.Add(EndPBStart);

            OptionsPanel.Controls.Add(kReloadPB);
            OptionsPanel.Controls.Add(kEndPB);
            OptionsPanel.Controls.Add(kBackPB);
            OptionsPanel.Controls.Add(SoundPB);
            OptionsPanel.Controls.Add(SizePickerCB);
            OptionsPanel.Controls.Add(ApplyPB);

            ScorePanel.Controls.Add(SavePB);
            ScorePanel.Controls.Add(LoadPB);
            ScorePanel.Controls.Add(LoadNameTB);

            ScorePanel.Controls.Add(st);

            FlyZone = new Panel();
            FlyZone.Size = new Size(Width, Height - Height / 4);
            FlyZone.Location = new Point(0, Height / 11);
            FlyZone.BackColor = Color.Transparent;

            FlyZone.Click += new EventHandler(FlyZone_Click);
            FlyZone.Hide();

            ShopPanel = new Panel();
            ShopPanel.Size = new Size(Width - Width / 6, Height - Height / 4);
            ShopPanel.Location = new Point(Width / 6, Height / 11);
            ShopPanel.BackColor = Color.Transparent;
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
            BulletZone.Location = new Point(0, Height - Height / 13);
            BulletZone.BackColor = Color.Transparent;
            BulletZone.Hide();

            Controls.Add(ShopPanel);
            Controls.Add(FlyZone);
            Controls.Add(BulletZone);
            Controls.Add(MenuPanel);
            Controls.Add(StartPanel);
            Controls.Add(ScorePanel);

            HurtLevels.Add(Properties.Resources.Hurt1);
            HurtLevels.Add(Properties.Resources.Hurt2);
            HurtLevels.Add(Properties.Resources.Hurt3);

            LockedGuns.Add(Properties.Resources.gun1Selected);
            LockedGuns.Add(Properties.Resources.gun4Locked);
            LockedGuns.Add(Properties.Resources.gun8Locked);
            LockedGuns.Add(Properties.Resources.gun6Locked);
            LockedGuns.Add(Properties.Resources.gun7Locked);

            SelectedGuns.Add(Properties.Resources.gun1Selected);
            SelectedGuns.Add(Properties.Resources.gun4Selected);
            SelectedGuns.Add(Properties.Resources.gun8Selected);
            SelectedGuns.Add(Properties.Resources.gun6Selected);
            SelectedGuns.Add(Properties.Resources.gun7Selected);

            kReload = Keys.R;
            kEnd = Keys.Escape;
            kBack = Keys.Left;

            CoinPB = new PictureBox();
            CoinPB.Size = new Size(Width / 10, Height / 10);
            CoinPB.Location = new Point(Width / 2, Height / 20);
            CoinPB.BackgroundImage = Properties.Resources.coinImage;
            CoinPB.BackgroundImageLayout = ImageLayout.Stretch;
            CoinPB.Tag = "false";

            CoinLabel = new Label();
            CoinLabel.Size = new Size(Width / 10, Height / 10);
            CoinLabel.Location = new Point(Width / 2 + Width / 10, Height / 20);
            CoinLabel.BackColor = Color.Transparent;
            CoinLabel.ForeColor = Color.White;
            CoinLabel.TextAlign = ContentAlignment.MiddleCenter;
            CoinLabel.Font = new Font("Arial", Width/64 , FontStyle.Bold);
            CoinLabel.Text = "0";

            AllGuns = Gun.CreateGuns();
            UnlockedGuns.Add(AllGuns[0]);
            CurrentGun = AllGuns[0];
            for (int i = 0; i != AllGuns.Count; i++)
            {
                PictureBox gunPB = new PictureBox();
                gunPB.Size = new Size(Width / 4, Height / 20);
                gunPB.Location = new Point(Width / 8, Height / 16 * i + Height / 16);
                gunPB.BackgroundImage = LockedGuns[i];
                gunPB.BackgroundImageLayout = ImageLayout.Stretch;
                gunPB.BackColor = Color.Transparent;
                gunPB.Click += new EventHandler(Gun_Buy);
                gunPB.Tag = i.ToString();
                ShopPanel.Controls.Add(gunPB);
                gunPB.Show();
            }
            ShopPanel.Controls.Add(CoinPB);
            ShopPanel.Controls.Add(CoinLabel);
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
        private void Save_Click(object sender, EventArgs e)
        {
            if (st.Controls[0] is ListView)
            {
                if ((st.Controls[0] as ListView).SelectedItems.Count != 0) 
                {
                    if ((st.Controls[0] as ListView).SelectedItems[0].Text == string.Empty || (st.Controls[0] as ListView).SelectedItems[0].Text == null)
                    {
                        MessageBox.Show("Empty name field!");
                    }
                    else
                    {
                        CurrentGame.Save(Name, Coins, Level, AllGuns, CurrentGun, UnlockedGuns, kReload, kEnd, kBack);
                        CurrentGame.SaveGame("save.txt", (st.Controls[0] as ListView).SelectedItems[0].Text);
                    }
                } else
                {
                    if(LoadNameTB.Text == string.Empty || LoadNameTB.Text == null)
                    {
                        MessageBox.Show("Empty name field!");
                    } else
                    {
                        CurrentGame.Save(Name, Coins, Level, AllGuns, CurrentGun, UnlockedGuns, kReload, kEnd, kBack);
                        CurrentGame.SaveGame("save.txt", LoadNameTB.Text);
                        st.UpdateListView();
                        LoadNameTB.Text = string.Empty;
                    }
                }
            }
        }
        private void End_Click(object sender, EventArgs e)
        {
            DialogResult dr = MessageBox.Show("Are you sure you want to leave?\nAll your unsaved progress will be lost!", "Don't leave just yet!", MessageBoxButtons.YesNo);
            if (dr == DialogResult.Yes)
            {
                Application.Exit();
            }
        }
        private void Load_Click(object sender, EventArgs e) 
        {

            if (st.Controls[0] is ListView)
            {
                if ((st.Controls[0] as ListView).SelectedItems.Count != 0)
                {
                    if ((st.Controls[0] as ListView).SelectedItems[0].Text == string.Empty || (st.Controls[0] as ListView).SelectedItems[0].Text == null)
                    {
                        MessageBox.Show("Empty name box!");
                    }
                    else
                    {
                        CurrentGame.LoadGame("save.txt", (st.Controls[0] as ListView).SelectedItems[0].Text);
                        CurrentGame.Load(out NameGame, out Coins, out Level, out AllGuns, out CurrentGun, out UnlockedGuns, out kReload, out kEnd, out kBack);
                        ContinuePB.Show();
                    }
                }
            }
        }
        private void Score_Click(object sender, EventArgs e)
        {
            ScorePanel.Visible = !ScorePanel.Visible;
            OptionsPanel.Visible = false;
            ScorePanel.BringToFront();
            st.UpdateListView();
        }
        private void NewGame_Click(object sender, EventArgs e)
        {

            CurrentGame = new Game(true);
            CurrentGame.Load(out NameGame, out Coins, out Level, out AllGuns, out CurrentGun, out UnlockedGuns, out kReload, out kEnd, out kBack);
            StartPanel.Hide();
            ScorePanel.Hide();
            ScorePanel.Hide();
            MenuPanel.Show();
            ContinuePB.Show();
            int i = 0;
            foreach (object ob in ShopPanel.Controls)
            {
                if (ob is PictureBox)
                {
                    if((ob as PictureBox).Tag.ToString() != "false") { 
                        (ob as PictureBox).BackgroundImage = LockedGuns[i];
                        i++;
                    }
                }
            }
            (ShopPanel.Controls[0] as PictureBox).BackgroundImage = SelectedGuns[0];

        }
        private void Back_Click(object sender, EventArgs e)
        {
            StartPanel.Show();
            MenuPanel.Hide();
            ShopPanel.Hide();
            ScorePanel.Hide();
        }
        private void Size_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Normal;
            this.FormBorderStyle = FormBorderStyle.FixedToolWindow;
            int sizeX = 0, sizeY = 0;
            try
            {
                string text = SizePickerCB.Text;
                string[] sizes = text.Split('x');
                sizeX = Convert.ToInt32(sizes[0]);
                sizeY = Convert.ToInt32(sizes[1]);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
            ResizeAllControls(this, (float)sizeX / (float)Width, (float)sizeY / (float)Height);
            this.Width = sizeX;
            this.Height = sizeY;

        }
        private void ResizeAllControls(Control control, float scaleX, float scaleY)
        {
            control.Width = (int)(control.Width * scaleX);
            control.Height = (int)(control.Height * scaleY);

            control.Left = (int)(control.Left * scaleX);
            control.Top = (int)(control.Top * scaleY);

            foreach (Control childControl in control.Controls)
            {
                ResizeAllControls(childControl, scaleX, scaleY);
            }
        }
        private void Continue_Click(object sender, EventArgs e)
        {
            StartPanel.Hide();
            MenuPanel.Show();
            ShopPanel.Hide();
            ScorePanel.Hide();
        }
        private void Global_Click(object sender, EventArgs e)
        {
            LockAll();
            foreach (Control c in OptionsPanel.Controls)
            {
                if (c is PictureBox)
                {
                    if ((c as PictureBox).Tag != null)
                    {
                        (c as PictureBox).Tag = "0";
                    }
                }
            }

            kListening = true;
            (sender as PictureBox).Tag = "1";
        }
        private void Sound_Click(object sender, EventArgs e)
        {
            PlaySound = !PlaySound;
            if (PlaySound)
            {
                (sender as PictureBox).BackgroundImage = Properties.Resources.soundOnButton;
            }
            else
            {
                (sender as PictureBox).BackgroundImage = Properties.Resources.soundOffButton;

            }
        }
        private void Options_Click(object sender, EventArgs e)
        {
            OptionsPanel.Visible = !OptionsPanel.Visible;
            OptionsPanel.BringToFront();
            ScorePanel.Visible = false;
        }

        private void ToKeyImage(PictureBox sender, Keys key)
        {
            switch (key)
            {
                case Keys.A:
                    sender.Image = Properties.Resources.aKey;
                    break;
                case Keys.B:
                    sender.Image = Properties.Resources.bKey;
                    break;
                case Keys.C:
                    sender.Image = Properties.Resources.cKey;
                    break;
                case Keys.D:
                    sender.Image = Properties.Resources.dKey;
                    break;
                case Keys.E:
                    sender.Image = Properties.Resources.eKey;
                    break;
                case Keys.F:
                    sender.Image = Properties.Resources.fKey;
                    break;
                case Keys.G:
                    sender.Image = Properties.Resources.gKey;
                    break;
                case Keys.H:
                    sender.Image = Properties.Resources.hKey;
                    break;
                case Keys.I:
                    sender.Image = Properties.Resources.iKey;
                    break;
                case Keys.J:
                    sender.Image = Properties.Resources.jKey;
                    break;
                case Keys.K:
                    sender.Image = Properties.Resources.kKey;
                    break;
                case Keys.L:
                    sender.Image = Properties.Resources.lKey;
                    break;
                case Keys.M:
                    sender.Image = Properties.Resources.mKey;
                    break;
                case Keys.N:
                    sender.Image = Properties.Resources.nKey;
                    break;
                case Keys.O:
                    sender.Image = Properties.Resources.oKey;
                    break;
                case Keys.P:
                    sender.Image = Properties.Resources.pKey;
                    break;
                case Keys.Q:
                    sender.Image = Properties.Resources.qKey;
                    break;
                case Keys.R:
                    sender.Image = Properties.Resources.rKey;
                    break;
                case Keys.S:
                    sender.Image = Properties.Resources.sKey;
                    break;
                case Keys.T:
                    sender.Image = Properties.Resources.tKey;
                    break;
                case Keys.U:
                    sender.Image = Properties.Resources.uKey;
                    break;
                case Keys.V:
                    sender.Image = Properties.Resources.vKey;
                    break;
                case Keys.W:
                    sender.Image = Properties.Resources.wKey;
                    break;
                case Keys.X:
                    sender.Image = Properties.Resources.xKey;
                    break;
                case Keys.Y:
                    sender.Image = Properties.Resources.yKey;
                    break;
                case Keys.Z:
                    sender.Image = Properties.Resources.zKey;
                    break;
                case Keys.Escape:
                    sender.Image = Properties.Resources.escapeKey;
                    break;
                case Keys.Left:
                    sender.Image = Properties.Resources.leftKey;
                    break;
                case Keys.Right:
                    sender.Image = Properties.Resources.rightKey;
                    break;
                case Keys.Up:
                    sender.Image = Properties.Resources.upKey;
                    break;
                case Keys.Down:
                    sender.Image = Properties.Resources.downKey;
                    break;
                default:
                    sender.Image = null;
                    break;
            }
        }

        public void ChangeShop()
        {
            //UnlockedGuns
            //kazda unlockedGuns ma ID
            //id = pozice v ShopPanel.Controls[]
            try {
                CoinLabel.Text = Coins.ToString();
                foreach(Gun a in UnlockedGuns)
                {
                    (ShopPanel.Controls[a.ID] as PictureBox).BackgroundImage = a.Img;
                }
                (ShopPanel.Controls[CurrentGun.ID] as PictureBox).BackgroundImage = SelectedGuns[CurrentGun.ID];
            } 
            catch(Exception ex)
            {
                MessageBox.Show("ChangeShop doesnt work :-(" + ex.Message);
            }
        }
        private void Hunt_Click(object sender, EventArgs e)
        {
            FlyZone.Show();
            BulletZone.Show();
            MenuPanel.Hide();
            ShopPanel.Hide();
            ScorePanel.Hide();
            CoinsPerRound = 0;
            this.BackgroundImage = Properties.Resources.bckImage;
            SpawnDucks();
            CreateBullets();
            FlyTimer.Start();
        }
        public void End(bool win)
        {
            this.BackgroundImage = Properties.Resources.bckImageNight;
            FlyTimer.Stop();
            MenuPanel.Show();
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
            EndScreen(win);
            LockAll();
        }
        public void EndScreen(bool win)
        {
            EndPanel = new Panel();
            EndPanel.Size = new Size(Width / 4, Height / 4);
            EndPanel.Location = new Point(Width / 2 - Width / 8, Height / 2 - Height / 8);
            EndPanel.BackgroundImage = Properties.Resources.endScreen;
            EndPanel.BackgroundImageLayout = ImageLayout.Stretch;
            this.Controls.Add(EndPanel);
            Label EndLabel = new Label();
            if (win)
            {
                EndLabel.Text = "You did it! :-)\n";
            }
            else
            {
                EndLabel.Text = "Maybe next time! :-(\n";
            }
            EndLabel.Text += "You made: " + CoinsPerRound + " coins and you currently have " + Coins + " coins";

            EndLabel.MaximumSize = new Size(EndPanel.Width - EndPanel.Width / 16, EndPanel.Height - EndPanel.Height / 4);
            EndLabel.Location = new Point(EndPanel.Width / 32, EndPanel.Height / 8);
            EndLabel.BackColor = Color.Transparent;
            EndLabel.Font = new Font(EndLabel.Font.FontFamily, EndPanel.Height / 20);
            EndLabel.AutoSize = true;

            PictureBox EndButton = new PictureBox();
            EndButton.Size = new Size(EndPanel.Width / 8, EndPanel.Height / 8);
            EndButton.BackgroundImage = Properties.Resources.okButton;
            EndButton.BackgroundImageLayout = ImageLayout.Stretch;
            EndButton.BackColor = Color.Transparent;
            EndButton.Location = new Point(EndPanel.Width / 2 - EndPanel.Width / 16, EndPanel.Height / 2 + EndPanel.Height / 4);
            EndButton.Click += new EventHandler(EndButton_Click);
            EndPanel.Controls.Add(EndLabel);
            EndPanel.Controls.Add(EndButton);
        }
        private void EndButton_Click(object sender, EventArgs e)
        {
            PictureBox sndr = (sender as PictureBox);
            UnlockAll();
            sndr.Parent.Dispose();
        }
        public void LockAll()
        {
            MenuPanel.Enabled = false;
            StartPanel.Enabled = false;
        }
        public void UnlockAll()
        {
            MenuPanel.Enabled = true;
            StartPanel.Enabled = true;
        }
        public void Recoil()
        {
            Random rnd = new Random();
            this.Cursor = Cursor.Current;
            Cursor.Position = new Point(Cursor.Position.X + rnd.Next(-Width / 20, Width / 20), Cursor.Position.Y - rnd.Next(0, Height / 20));
            //pozdeji zmenit na currentgun.recoil nebo pridat parametr s recoilem :-)
        }
        public void SpawnDucks()
        {
            Random rnd = new Random();
            foreach (Duck Dck in Duck.CreateDucks(Level))
            {
                PictureBox DuckPB = new PictureBox();
                DuckPB.BackgroundImage = Dck.Img;
                DuckPB.Location = new Point(-FlyZone.Width / 32, FlyZone.Height - rnd.Next(FlyZone.Height / 16, FlyZone.Height));
                DuckPB.Size = new Size(Width / 16, Height / 16);
                DuckPB.Click += new EventHandler(Duck_Click);
                DuckPB.BackgroundImageLayout = ImageLayout.Stretch;
                DuckPB.Name = Dck.Speed.ToString();
                DuckPB.BackColor = Color.Transparent;
                DuckPB.Tag = Dck.Health + "|" + Dck.Coins + "|" + Dck.HurtLevel;
                FlyZone.Controls.Add(DuckPB);
            }
        }
        private async void Duck_Click(object sender, EventArgs e)
        {
            // Ammo check
            if (CurrentGun != null && CurrentGun.CanShoot())
            {
                PictureBox pb = (sender as PictureBox);
                if (PlaySound)
                {
                    sp.Play();
                }
                CurrentGun.Shoot();
                Recoil();
                ChangeBullets(false);
                if (pb.Tag != null)
                {
                    string[] pbParams = pb.Tag.ToString().Split('|');
                    int CurrentHealth = Convert.ToInt32(pbParams[0]);
                    int CurrentCoins = Convert.ToInt32(pbParams[1]);
                    int CurrentHurtLevel = Convert.ToInt32(pbParams[2]);
                    if (CurrentHealth - CurrentGun.Damage <= 0)
                    {
                        Coins += CurrentCoins;
                        CoinsPerRound += CurrentCoins;
                        pb.Click -= Duck_Click;
                        pb.Name = "0";
                        pb.Image = Properties.Resources.duckEnd;
                        await Task.Run(async () =>
                        {
                            await Task.Delay(100);
                            BeginInvoke(new Action(() => pb.Dispose()));
                        });
                    }
                    else
                    {
                        pb.SizeMode = PictureBoxSizeMode.StretchImage;
                        if (CurrentHurtLevel != HurtLevels.Count)
                        {
                            CurrentHurtLevel++;
                        }
                        pb.Image = HurtLevels[CurrentHurtLevel - 1];
                        pb.Tag = (CurrentHealth - CurrentGun.Damage).ToString() + "|" + CurrentCoins.ToString() + "|" + CurrentHurtLevel.ToString();

                    }
                }
            }
        }
        private void CreateBullets()
        {
            
            double multiplier = 1;
            
            if (CurrentGun.MaxAmmo > 8)
            {
                multiplier = 0.5;
            }
            if (CurrentGun.MaxAmmo > 20)
            {
                multiplier = 0.15;
            }
            for (int i = 0; i != CurrentGun.MaxAmmo; i++)
            {
                //pri hodne bullets vyjedou z bulletzone, moznost udelat to tak aby se bulletzone.width/pocbullets a mel bys jak velka ma bejt kazda kulka +-bulletzone.Width/20 nebo neco
                //done
                Panel Bullet = new Panel();
                Bullet.Size = new Size(Convert.ToInt32(BulletZone.Width / 15 * multiplier), BulletZone.Height);
                Bullet.Location = new Point(Convert.ToInt32((BulletZone.Width / 10 * i + BulletZone.Width / 20) * multiplier), 0);
                Bullet.BackColor = Color.DarkGoldenrod;
                BulletZone.Controls.Add(Bullet);
            }
        }
        private async void FlyZone_Click(object sender, EventArgs e)
        {
            if (CurrentGun != null && CurrentGun.CanShoot())
            {
                if (PlaySound)
                {
                    sp.Play();
                }
                CurrentGun.Shoot();
                Recoil();
                ChangeBullets(false);
            }
        }
        private void Form_KeyDown(object sender, KeyEventArgs e)
        {

            if (kListening && Game.FromKeys(e.KeyData) != string.Empty)
            {
                kListening = false;
                foreach (object ob in OptionsPanel.Controls)
                {
                    if (ob is PictureBox)
                    {
                        PictureBox pb = (PictureBox)ob;
                        if (pb.Tag != null)
                        {
                            if (pb.Tag.ToString() == "1")
                            {
                                pb.Tag = "0";
                                ToKeyImage(pb, e.KeyData);
                                UnlockAll();
                                switch (pb.Name)
                                {
                                    case "Reload":
                                        kReload = e.KeyData;
                                        break;
                                    case "End":
                                        kEnd = e.KeyData;
                                        break;
                                    case "Back":
                                        kBack = e.KeyData;
                                        break;

                                }
                            }
                        }
                    }
                }

            }
            else
            {
                if (e.KeyData == kReload && FlyZone.Visible)
                {
                    CurrentGun.Reload();
                    if (CurrentGun.CurrentAmmo != CurrentGun.MaxAmmo)
                    {
                        ChangeBullets(true);
                    } //if je pojisteni pred moznym bugem kdy uzivatel klikne R pred tim nez dobehne v CurrentGun.Shoot delayROF
                }
                if (e.KeyData == kEnd)
                {
                    DialogResult dr = MessageBox.Show("Are you sure you want to leave?\nAll your unsaved progress will be lost!", "Don't leave just yet!", MessageBoxButtons.YesNo);
                    if (dr == DialogResult.Yes)
                    {
                        Application.Exit();
                    }
                }
                if (e.KeyData == kBack && FlyZone.Visible)
                {
                    End(false);
                }
            }
        }
        private void Shop_Click(object sender, EventArgs e)
        {
            ShopPanel.Visible = !ShopPanel.Visible;
            ChangeShop();
        }
        private void Gun_Buy(object sender, EventArgs e)
        {
            //Gun list -> gun1, gun4, gun6;
            Gun gun = AllGuns[Convert.ToInt32((sender as PictureBox).Tag)];
            if (!UnlockedGuns.Contains(gun))
            {
                if (Coins >= gun.Cost)
                {
                    Coins -= gun.Cost;
                    UnlockedGuns.Add(gun);
                    (sender as PictureBox).BackgroundImage = gun.Img;
                    CoinLabel.Text = Coins.ToString();
                }
                else
                {
                    MessageBox.Show("Too expensive it costs " + gun.Cost + " and you only have " + Coins);
                }
            }
            else
            {
                (ShopPanel.Controls[CurrentGun.ID] as PictureBox).BackgroundImage = CurrentGun.Img;
                CurrentGun = gun;
                gun.CurrentAmmo = gun.MaxAmmo;
                (sender as PictureBox).BackgroundImage = SelectedGuns[Convert.ToInt32((sender as PictureBox).Tag)];
            }

        }
        private void DiscardBullets()
        {
            while (BulletZone.Controls.Count > 0)
            {
                BulletZone.Controls.RemoveAt(0);
            }
        }
        private async Task ChangeBullets(bool Plus)
        {
            if (Plus)
            {
                foreach (Control s in BulletZone.Controls)
                {
                    if (s is Panel)
                    {
                        await Task.Run(() => Task.Delay(CurrentGun.DelayReload / CurrentGun.MaxAmmo));
                        (s as Panel).BackColor = Color.DarkGoldenrod;
                        if (PlaySound)
                        {
                            sp1.Play();
                        }
                    }

                }
                if (PlaySound)
                {
                    sp2.Play();
                }
            }
            else
            {
                for (int i = BulletZone.Controls.Count - 1; i >= 0; i--)
                {
                    if (BulletZone.Controls[i] is Panel)
                    {
                        if ((BulletZone.Controls[i] as Panel).BackColor == Color.DarkGoldenrod)
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
