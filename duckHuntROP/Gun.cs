﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace duckHuntROP
{
    public class Gun
    {
        public int ID;
        public int Damage;
        public int CurrentAmmo;
        public int MaxAmmo;
        public int DelayROF;
        public int DelayReload;
        public int Cost;
        private bool Reloading = false;
        public Image Img;
        public Gun() { }
        public Gun(int damage, int currentAmmo, int maxAmmo, int delayROF,int delayReload, int cost, Image img, int iD)
        {
            Damage = damage;
            CurrentAmmo = currentAmmo;
            MaxAmmo = maxAmmo;
            DelayROF = delayROF;
            DelayReload = delayReload;
            Cost = cost;
            Img = img;
            ID = iD;    
        }
        public async Task Shoot()
        {
            this.CurrentAmmo--;
            Reloading = true;
            await Task.Delay(this.DelayROF);
            Reloading = false;
        }
        public async Task Reload()
        {
            if (!Reloading && CurrentAmmo != MaxAmmo)
            {
                Reloading = true;

                await Task.Delay(this.DelayReload);

                Reloading = false;
                CurrentAmmo = MaxAmmo;
            }
        }
        public bool CanShoot()
        {
            if (CurrentAmmo > 0 && !Reloading) return true;
            return false;
        }
        public static List<Gun> CreateGuns()
        {
            List<Gun> list = new List<Gun>();
            list.Add(new Gun(5, 4, 4, 100, 3000,0, Properties.Resources.gun1, 0));
            list.Add(new Gun(6, 6, 6, 100, 4000,100 ,Properties.Resources.gun4,1));
            list.Add(new Gun(5, 10, 10, 100, 4000, 1000, Properties.Resources.gun8, 2));
            list.Add(new Gun(2, 16, 16, 30, 3000, 7000, Properties.Resources.gun6,3));
            list.Add(new Gun(1, 60, 60, 20, 7000, 10000, Properties.Resources.gun7, 4));
            return list;

        }
    }
}
