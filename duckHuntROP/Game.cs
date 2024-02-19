using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace duckHuntROP
{
    public class Game
    {
        public string Name { get; set; }
        private int Coins;
        private int Level;
        private Gun CurrentGun;
        private List<Gun> UnlockedGuns;
        private Keys kReload;
        private Keys kEnd;
        private Keys kBack;
        public Game()
        {
            Name = "default";
            Coins = 50;
            Level = 1;
            CurrentGun = Gun.CreateGuns()[0];
            UnlockedGuns = new List<Gun>();
            UnlockedGuns.Add(CurrentGun);
            kReload = Keys.R;
            kEnd = Keys.Escape;
            kBack = Keys.Left;
        }
        public Game(string gameString)
        {

        }
        public void Load(ref string Name, ref int Coins, ref int Level, ref List<Gun> AllGuns,ref Gun CurrentGun, ref List<Gun> UnlockedGuns, ref Keys kReload, ref Keys kEnd, ref Keys kBack)
        {
            Name = this.Name;
            Coins = this.Coins;
            Level = this.Level;
            CurrentGun = this.CurrentGun;
            UnlockedGuns = this.UnlockedGuns;
            AllGuns = Gun.CreateGuns();
            kReload = this.kReload;
            kBack = this.kBack;
            kEnd = this.kEnd;
        }
        public void LoadGame(string path, string nameID)
        {
            try
            {
                using (StreamReader sr = new StreamReader(path))
                {
                   Game game = new Game(sr.ReadToEnd().Split('\n'));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
                return; 
            }
        }
        public void SaveGame(string path)
        {

        }
        public Keys ToKeys(string key)
        {
            key = key.ToLower();
            switch(key)
            {
                case "a": return Keys.A;
                case "b": return Keys.B;
                case "c": return Keys.C;
                case "d": return Keys.D;
                case "e": return Keys.E;
                case "f": return Keys.F;
                case "g": return Keys.G;
                case "h": return Keys.H;
                case "i": return Keys.I;
                case "j": return Keys.J;
                case "k": return Keys.K;
                case "l": return Keys.L;
                case "m": return Keys.M;
                case "n": return Keys.N;
                case "o": return Keys.O;
                case "p": return Keys.P;
                case "q": return Keys.Q;
                case "r": return Keys.R;
                case "s": return Keys.S;
                case "t": return Keys.T;
                case "u": return Keys.U;
                case "v": return Keys.V;
                case "w": return Keys.W;
                case "x": return Keys.X;
                case "y": return Keys.Y;
                case "z": return Keys.Z;
                case "esc": return Keys.Escape;
                case "left": return Keys.Left;
                case "up": return Keys.Up;
                case "down": return Keys.Down;
                case "right": return Keys.Right;
                default: return Keys.None;
            }
        }
        public string FromKeys(Keys key)
        {
            switch (key)
            {
                case Keys.A: return "a";
                case Keys.B: return "b";
                case Keys.C: return "c";
                case Keys.D: return "d";
                case Keys.E: return "e";
                case Keys.F: return "f";
                case Keys.G: return "g";
                case Keys.H: return "h";
                case Keys.I: return "i";
                case Keys.J: return "j";
                case Keys.K: return "k";
                case Keys.L: return "l";
                case Keys.M: return "m";
                case Keys.N: return "n";
                case Keys.O: return "o";
                case Keys.P: return "p";
                case Keys.Q: return "q";
                case Keys.R: return "r";
                case Keys.S: return "s";
                case Keys.T: return "t";
                case Keys.U: return "u";
                case Keys.V: return "v";
                case Keys.W: return "w";
                case Keys.X: return "x";
                case Keys.Y: return "y";
                case Keys.Z: return "z";
                case Keys.Escape: return "esc";
                case Keys.Left: return "left";
                case Keys.Up: return "up";
                case Keys.Down: return "down";
                case Keys.Right: return "right";
                default: return string.Empty;
            }
        }
    }
}
