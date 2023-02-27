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

        [SerializeField] HealthBar healthBar;

        [SerializeField] AudioSource voice;
        [SerializeField] Sound hurtSound;
        [SerializeField] Sound deathSound;

        [SerializeField] GameObject deathMessage;
        [SerializeField] TMP_Text killedMessage;

        [SerializeField] public bool testingMode = false;

        [SerializeField] Transform[] shoulders = new Transform[2];

        private bool godmode = false;

        public PlayerAct Actor { get { return actor; } }
        public MovePlayer Mover { get { return mover; } }


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


        public void ToggleGodMode() {
            godmode = !godmode;
        }


        // Update is called once per frame
        void Update()
        {
            if(health.ShouldDie) Die(new Damages());
            else if(health.PlayerRegen()) {
                healthBar.UpdateHealth(health);
            }
            healthBar.UpdateStamina(actor);
        }


        public override Collider GetCollider()
        {
            return GetComponent<CharacterController>();
        }


        public override bool TakeDamage(ref Damages damage)
        {
            if (isDead || godmode) { return false; }
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


        public override void BeHitByEnviroDamage(int damage, DamageType type) {
           mover.BeHitByEnviroDamage(damage, type, ref health);
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


        public bool IsShoulderVisible(Transform monsterEyes) {
            return ((Vector3.Dot(monsterEyes.forward, shoulders[0].position - monsterEyes.position) > 0)
                 || (Vector3.Dot(monsterEyes.forward, shoulders[1].position - monsterEyes.position) > 0));
        }


    }


}