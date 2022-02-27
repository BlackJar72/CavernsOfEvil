using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace CevarnsOfEvil
{

    [System.Serializable]
    public enum DamageType
    {
        physical = 0,
        physicalPlus = 1,
        fire = 2,
        firePlus = 3,
        electric = 4,
        acid = 5,
        poison = 6,
        magic = 7,
        cold = 8
    }


    public struct Damages
    {
        public int shock;
        public int wound;
        public int toArmor;
        public DamageType type;
        public Entity attacker;
        public Damages(int s, int w, int a, DamageType type, Entity attacker)
        {
            shock = s;
            wound = w;
            toArmor = a;
            this.type = type;
            this.attacker = attacker;
        }
        public Damages(int s, int w, int a, Entity attacker)
        {
            shock = s;
            wound = w;
            toArmor = a;
            this.type = DamageType.physical;
            this.attacker = attacker;
        }
    }


    public static class DamageUtils
    {
        public static int RollDamage(int damageRating)
        {
            int half0 = damageRating / 2;
            int half1 = half0++;
            return half0 + Random.Range(0, half1) + Random.Range(0, half1);
        }


        public static float RollFloatDamage(int damageRating)
        {
            float half = (float)damageRating / 2f;
            return half + Random.Range(0, half) + Random.Range(0, half);
        }


        public static Damages CalcDamage(int damageRating, int armor, DamageType type, Entity attacker)
        {
            float roll = RollFloatDamage(damageRating);
            int damage = (int)Mathf.Max(1, ((float)(roll - armor))
                * (1.0f - Mathf.Min(armor / 40f, 0.5f)));
            return new Damages(damage, CalcWoulds(damage), Mathf.Max(1, ((int)roll - 10) / 10), type, attacker);
        }


        public static Damages CalcDamageNoArmor(int damageRating, DamageType type, Entity attacker)
        {
            int damage = RollDamage(damageRating);
            return new Damages(damage, CalcWoulds(damage), 0, type, attacker);
        }


        public static Damages CalcDamage(int damageRating, int armor, Entity attacker)
        {
            float roll = RollFloatDamage(damageRating);
            int damage = (int)Mathf.Max(1, ((float)(roll - armor))
                * (1.0f - Mathf.Min(armor / 40f, 0.5f)));
            return new Damages(damage, CalcWoulds(damage), Mathf.Max(1, ((int)roll - 10) / 10), DamageType.physical, attacker);
        }


        public static Damages CalcDamageNoArmor(int damageRating, Entity attacker)
        {
            int damage = RollDamage(damageRating);
            return new Damages(damage, CalcWoulds(damage), 0, DamageType.physical, attacker);
        }


        private static int CalcWoulds(int shock)
        {
            if (shock > 12)
            {
                return (shock - 10) / 2;
            } 
            else if(shock > 5) 
            {
                return 1;
            }
            return 0;
        }

    }
}
