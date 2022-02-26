using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace DLD
{

    public class HealthBar : MonoBehaviour
    {
        [SerializeField] BarScaler shock;
        [SerializeField] BarScaler wound;
        [SerializeField] BarScaler stamina;

        public void UpdateHealth(EntityHealth health)
        {
            shock.SetBar(health.RelatvieShock);
            wound.SetBar(health.RelativeHealth);
        }

        public void UpdateStamina(PlayerAct player)
        {
            stamina.SetBar(player.Stamina / PlayerAct.baseStamina);
        }
    }
}