using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace DLD
{
    public abstract class Wand : Item 
    {
        [SerializeField] protected int maxCharges;
        [SerializeField] protected AdvancedBarScaler chargeScaler;

        public abstract int Charges { get; set; }

        public int MaxCharges { get { return maxCharges; } }


        public override void BeAcquired()
        {
            base.BeAcquired();
            Charges = maxCharges;
            chargeScaler.SetBar(1.0f);
            chargeScaler.Activate();
        }


        public override void BeRemoved()
        {
            base.BeRemoved();
            Charges = 0;
            chargeScaler.SetBar(0.0f);
            chargeScaler.Deactivate();
        }


        public bool ShouldTake()
        {
            return Charges < maxCharges;
        }

        public override void Init()
        {
            base.Init();
            if (equiped)
            {
                chargeScaler.Activate();
                chargeScaler.SetBar(Charges, maxCharges);
            }
        }


    }
}