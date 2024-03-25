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
        private List<Gun> AllGuns;
        private Keys kReload;
        private Keys kEnd;
        private Keys kBack;
        public Game() { }
        public Game(bool baseVal)
        {
            if (baseVal)
            {
                Name = "default";
                Coins = 50;
                Level = 1;
                CurrentGun = Gun.CreateGuns()[0];
                UnlockedGuns = new List<Gun>();
                AllGuns = Gun.CreateGuns();
                UnlockedGuns.Add(CurrentGun);
                kReload = Keys.R;
                kEnd = Keys.Escape;
                kBack = Keys.Left;
            }
        }
        public void Load(out string Name, out int Coins, out int Level, out List<Gun> AllGuns, out Gun CurrentGun, out List<Gun> UnlockedGuns, out Keys kReload, out Keys kEnd, out Keys kBack)
        {
            Name = this.Name;
            Coins = this.Coins;
            Level = this.Level;
            CurrentGun = this.CurrentGun;
            UnlockedGuns = this.UnlockedGuns;
            AllGuns = Gun.CreateGuns();
            kReload = this.kReload;
            kEnd = this.kEnd;
            kBack = this.kBack;
        }
        //Hru -> neobsahuje nic -> LoadGame("save.txt", "jmeno");
        //Z hry se to musí dostat do Form1.cs tudíž funkcí Load();
        //Tímhle můžu mít zaráz více her načtených
        public void Save(string Name, int Coins, int Level, List<Gun> AllGuns, Gun CurrentGun, List<Gun> UnlockedGuns, Keys kReload, Keys kEnd, Keys kBack)
        {
            this.Name = Name;
            this.Coins = Coins;
            this.Level = Level;
            this.AllGuns = AllGuns;
            this.CurrentGun = CurrentGun;
            this.UnlockedGuns = UnlockedGuns;
            this.kReload = kReload;
            this.kEnd = kEnd;
            this.kBack = kBack;
        }
        public static void Hide(string path)
        {
            File.SetAttributes(path,  FileAttributes.Hidden);
        }
        public static void Show(string path)
        {
            File.SetAttributes(path,  FileAttributes.Normal);
        }
        public void NewGame()
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
        public void LoadGame(string path, string nameID)
        {
            string gamePath = "error;0;1;0;0;r;esc;left";
            bool found = true;
            try
            {
                if (!File.Exists(path))
                {
                    using (File.Create(path)) {
                        Hide(path);
                    }
                }
                Show(path);
                using (StreamReader sr = new StreamReader(path))
                {
                    if (!sr.ReadToEnd().Contains(nameID))
                    {
                        MessageBox.Show("Name not found!");
                        found = false;
                    }
                    string[] splitPath = File.ReadAllLines(path);
                    foreach (string a in splitPath)
                    {
                        if (a.StartsWith(nameID))
                        {
                            gamePath = a;
                            break;
                        }
                    }
                }
                if (found)
                {
                    //Každá hodnota uložena v tomhle formátu, jestli jinak, tak se neznám
                    //name;coins;level;currentGun(číslo v allguns);unlockedguns1|unlockedguns2;r;esc;left
                    string[] values = gamePath.Split(';');
                    this.Name = values[0];
                    this.Coins = Convert.ToInt32(values[1]);
                    this.Level = Convert.ToInt32(values[2]);
                    this.CurrentGun = AllGuns[Convert.ToInt32(values[3])];
                    string[] unlockedGunsID = values[4].Split('|');
                    List<Gun> gunList = new List<Gun>();
                    foreach (string a in unlockedGunsID)
                    {
                        //[0][1][2]
                        gunList.Add(AllGuns[Convert.ToInt32(a)]);
                    }
                    UnlockedGuns = gunList;
                    this.kReload = ToKeys(values[5]);
                    this.kEnd = ToKeys(values[6]);
                    this.kBack = ToKeys(values[7]);
                }
                Hide(path);
                return;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
                
            }
            Hide(path);
        }
        public void SaveGame(string path, string nameID)
        {
            
            try
            {
                if (!File.Exists(path))
                {
                    using (File.Create(path)) {
                        Hide(path);
                    }
                }
                Show(path);
                string[] lines = File.ReadAllLines(path);
                bool found = false;
                for (int i = 0; i < lines.Length; i++)
                {
                    if (lines[i].StartsWith(nameID))
                    {
                        lines[i] = GenerateSaveData(nameID);
                        found = true;
                        break;
                    }
                }
                if (!found)
                {
                    string newData = GenerateSaveData(nameID);
                    Array.Resize(ref lines, lines.Length + 1);
                    lines[lines.Length - 1] = newData;
                }
                File.WriteAllLines(path, lines);
                Hide(path);
                return;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
            Hide(path);
        }

        private string GenerateSaveData(string nameID)
        {
            //NA tenhle kod nesmis uz sahnout, jestli si myslis ze to nejak zahadne spravis aby to melo lepsi cas tak si to nemysli
            int CurrentGunIndex = 0;
            // Uložení v tomto formátu: name;coins;level;currentGun(číslo v allguns);unlockedguns1|unlockedguns2;r;esc;left
            for (int i = 0; i != AllGuns.Count; i++)
            {
                if (AllGuns[i].ID == CurrentGun.ID)
                {
                    CurrentGunIndex = i;
                    break;
                }
            }
            List<string> UnlockedGunsIndex = new List<string>();
            for (int i = 0; i != AllGuns.Count; i++)
            {
                foreach (Gun g in UnlockedGuns)
                {
                    if (AllGuns[i].ID == g.ID)
                    {
                        UnlockedGunsIndex.Add(i.ToString());
                    }
                }
            }
            string saveData = $"{nameID};{this.Coins};{this.Level};{CurrentGunIndex};";
            saveData += string.Join("|", UnlockedGunsIndex);
            saveData += $";{FromKeys(kReload)};{FromKeys(kEnd)};{FromKeys(kBack)}";
            return saveData;
        }
        public static Keys ToKeys(string key)
        {
            key = key.ToLower();
            switch (key)
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
        public static string FromKeys(Keys key)
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