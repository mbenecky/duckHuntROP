using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace duckHuntROP
{
    public class Gun
    {
        public int Damage;
        public int CurrentAmmo;
        public int MaxAmmo;
        public int DelayROF;
        public int DelayReload;
        private bool Reloading = false;
        public Image Img;
        public Gun() { }
        public Gun(int damage, int currentAmmo, int maxAmmo, int delayROF,int delayReload,Image img)
        {
            Damage = damage;
            CurrentAmmo = currentAmmo;
            MaxAmmo = maxAmmo;
            DelayROF = delayROF;
            DelayReload = delayReload;
            Img = img;
        }
        public void Shoot()
        {
            Reloading = true;
            Task.Factory.StartNew(async () =>
            {
                await Task.Delay(this.DelayROF);
                Reloading = false;
            });
        }
        public void Reload()
        {
            if(Reloading == false)
            {
                Reloading = true;
                Task.Factory.StartNew(async () =>
                {
                    await Task.Delay(this.DelayReload);
                    Reloading = false;
                });
            }
        }
        public bool CanShoot()
        {
            if (CurrentAmmo > 0 || !Reloading) return true;
            return false;
        }
        public static List<Gun> CreateGuns()
        {
            List<Gun> list = new List<Gun>();
            list.Add(new Gun(5, 4, 4, 100, 4000, Properties.Resources.gun1));
            list.Add(new Gun(5, 6, 6, 100, 4000, Properties.Resources.gun4));

            return list;

        }
    }
}
