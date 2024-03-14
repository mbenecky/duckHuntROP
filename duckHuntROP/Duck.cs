using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace duckHuntROP
{
    public class Duck
    {
        public int Health;
        public Image Img;
        public int Coins;
        public bool IsBoss;
        public int Speed;
        public int HurtLevel = 0;
        public Duck() { }
        public Duck(int level, Random rnd)
        {
            if (level % 5 != 0)
            {
                if(rnd.Next(0,2) == 0)
                {
                    this.Img = Properties.Resources.duck1;
                } else
                {
                    this.Img = Properties.Resources.duck2;
                }
                this.Health = rnd.Next(3, 5);
                this.Coins = rnd.Next(10, 30);
                this.Speed = rnd.Next(5, 20);
                this.IsBoss = false;
            }
            else
            {
                this.Img = Properties.Resources.duck6;
                this.Health = rnd.Next(30, 50);
                this.Coins = rnd.Next(50, 100);
                this.Speed = rnd.Next(1, 6);
                this.IsBoss = true;
            }
        }
        public static List<Duck> CreateDucks(int Level)
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
                list.Add(new Duck(Level, rnd));
            }
            return list;
        }
        public override string ToString()
        {
            return "\nHealth:" + this.Health + "\nCoins:" + this.Coins + "\nSpeed:" + this.Speed;
        }
    }

}