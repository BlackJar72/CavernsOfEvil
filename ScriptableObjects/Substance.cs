using UnityEngine;



namespace CevarnsOfEvil
{

    /// <summary>
    /// A collections of data relating to a Material, including the Material, and 
    /// other data such as PhysicMaterial and SoundType.  Basically, replaces 
    /// blocks from the DLD mod.
    /// </summary>
    [CreateAssetMenu(fileName = "Substance", menuName = "DLD/Substance", order = 100)]
    public class Substance : ScriptableObject
    {
        [SerializeField] Material material;
        [SerializeField] PhysicsMaterial physicMaterial;
        [SerializeField] GameObject hitParticles;

        [SerializeField] bool isLiquid = false;
        [SerializeField] int damage = 0;
        [SerializeField] DamageType damageType;

        [SerializeField] FloorEffect floorEffect;

        [SerializeField] float walkCost = 1.0f;
        [SerializeField] float swimCost = 1.0f;


        public Material Material => material;
        public PhysicsMaterial PhysicMaterial => physicMaterial;
        public GameObject HitParticles => hitParticles;
        public bool IsLiquid => isLiquid;
        public int Damage => damage;
        public DamageType TypeOfDamage => damageType;
        public FloorEffect Effect => floorEffect;
        public float WalkCost => walkCost;
        public float SwimCost => swimCost;


        public Damages GetDamage(EntityHealth victim) 
        { 
            return DamageUtils.CalcDamage(damage, victim.Armor, TypeOfDamage, null);
        }
    }

}
