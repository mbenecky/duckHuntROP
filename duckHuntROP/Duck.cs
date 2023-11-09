using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace duckHuntROP
{
    public class Duck
    {
        public int Health;
        public Image Img;
        public int Coins;
        public bool IsBoss;
        public int Speed;
        public Duck() { }
        public Duck(int level, Random rnd)
        {
            if (level % 5 != 0)
            {
                this.Img = Properties.Resources.duck1;
                this.Health = rnd.Next(3, 5);
                this.Coins = rnd.Next(10, 30);
                this.Speed = rnd.Next(5, 20);
                this.IsBoss = false;
            }
            else
            {
                this.Img = Properties.Resources.duck6;
                this.Health = rnd.Next(15, 20);
                this.Coins = rnd.Next(50, 100);
                this.Speed = rnd.Next(1, 6);
                this.IsBoss = true;
            }
        }
        public override string ToString()
        {
            return "\nHealth:" + this.Health + "\nCoins:" + this.Coins + "\nSpeed:" + this.Speed;
        }
    }

}