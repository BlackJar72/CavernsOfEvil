using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;


namespace CevarnsOfEvil
{

    public class Player : Entity
    { 
        PlayerAct actor;
        MovePlayer mover;
        ToastController toastController;

        Damages? killer = null;

        [SerializeField] HealthBar healthBar;

        [SerializeField] AudioSource voice;
        [SerializeField] Sound hurtSound;
        [SerializeField] Sound deathSound;

        [SerializeField] GameObject deathMessage;
        [SerializeField] TMP_Text killedMessage;

        [SerializeField] public bool testingMode = false;

        public PlayerAct Actor { get { return actor; } }
        public MovePlayer Mover { get { return mover; } }
        public Damages? Killer { get { return killer;  }  set { killer = value; } }


        // Start is called before the first frame update
        void Start()
        {
            actor = GetComponent<PlayerAct>();
            mover = GetComponent<MovePlayer>();
            toastController = GetComponent<ToastController>();
#if UNITY_EDITOR
            if(testingMode)
            {
                Init();
            }
#endif
            Killer = null;
        }


        /// <summary>
        /// Initialize player data, to be called at start of new game along with 
        /// (or by?) GameData initialization.
        /// </summary>
        public static void Init()
        {
            Physics.gravity = new Vector3(0f, -15f, 0f);
            PlayerHealth.Init();
            PlayerAct.Init();
            Item.StaticInit();
            ItemStack.PotionInit();
            Armor.Init();
        }


        // Update is called once per frame
        void Update()
        {
            if(health.PlayerRegen()) healthBar.UpdateHealth(health);
            healthBar.UpdateStamina(actor);
        }


        private void LateUpdate()
        {
            if (health.ShouldDie() && (killer != null)) Die((Damages)killer);
            else killer = null;
        }


        public override Collider GetCollider()
        {
            return GetComponent<CharacterController>();
        }


        public override bool TakeDamage(ref Damages damage)
        {
            if (isDead) { return false; }
            else
            {
                actor.ActiveArmor.BeDamaged(damage);
                healthBar.UpdateHealth(health);
                hurtSound.Play(voice);
                return true;
            }
        }


        public override void ShowDamage()
        {
            healthBar.UpdateHealth(health);
        }


        public override void TakeHealingPoition(int healAmount)
        {
            health.BeHealed(healAmount, true);
            healthBar.UpdateHealth(health);
        }


        public override void GetAimParams(out AimParams aim)
        {
            actor.GetAimParams(out aim);
        }


        public override Transform GetTransform()
        {
            return actor.GetTransform();
        }


        public override void Die(Damages damages)
        {
            if (!isDead)
            {
                if (mover.dungeon != null)
                {
                    mover.dungeon.DeactivateMobs();
                }
                healthBar.UpdateHealth(health);
                deathMessage.SetActive(true);
                deathSound.Play(voice);
                actor.Die();
                actor.enabled = false;
                mover.enabled = false;
                //SaveGame.DeleteGame(GameData.SaveName);
                StartCoroutine(DeathPause());
                if(damages.attacker != null)
                {
                    killedMessage.text = "Killed by " + damages.attacker.EntityName;
                }
                base.Die(damages);
            }
        }


        private IEnumerator DeathPause()
        {
            yield return new WaitForSeconds(5);
            SceneManager.LoadScene("Start");
        }
    }


}