using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace DLD
{

    public interface IArcher
    {
        public abstract bool ReadyToShoot { get; }
        public abstract void ShootGrabString();
        public abstract void ShootReleaseString();
        public abstract void ReadyAttackAngles();
        public abstract void AttackOver();
        public abstract void ArrowAttack();

    }

}