using UnityEngine;


namespace CevarnsOfEvil
{

    [CreateAssetMenu(menuName = "DLD/AI/Imp Fly Wander", fileName = "ImpFlyWander", order = 20)]
    public class ImpFlyWander : BehaviorObject
    {
        [SerializeField] BehaviorObject onSeePlayer;

        public override bool IsValidState(EntityMob ownerIn) => true;

        public override IBehaviorState NextState(EntityMob ownerIn)
            => EmptyState.Instance.NextState(ownerIn);

        public override bool StateUpdate(EntityMob ownerIn)
        {
            WingedImp imp = ownerIn as WingedImp;
            GameManager manager = imp.Dungeon.Manager;
            if (ownerIn.CanSeeTarget())
            {
                ownerIn.CurrentBehavior = onSeePlayer;
            }
            StepDataAI nextStep = manager.GetAIDataForFlying(ownerIn.transform.position,
                imp.transform.position + imp.DesiredDirection, imp);
            if (imp.ShouldTurn || !nextStep.Desireable || (imp.WanderUpdateTime < Time.time))
            {
                imp.WanderUpdateTime = Time.time + 2f + (Random.value * 2f);
                imp.DesiredDirection = FindTurnDirection() * imp.DesiredDirection;
                imp.ShouldTurn = false;
            }
            imp.FaceHeading();
            imp.Move();
            return true;
        }

        public override void StateEnter(EntityMob ownerIn)
        {
            WingedImp imp = ownerIn as WingedImp;
            if (imp.DesiredDirection == Vector3.zero)
            {
                imp.DesiredDirection = ownerIn.transform.forward;
            }
            imp.WanderUpdateTime = Time.time + 0.5f + (Random.value * 0.5f);
        }



        //public override void StateFixedUpdate(EntityMob ownerIn) { }
        // public override void StateLateUpdate(EntityMob ownerIn) { }


        private Quaternion FindTurnDirection()
        {
            switch (Random.Range(0, 5))
            {
                case 0:
                    return Quaternion.identity;
                case 1:
                    return Quaternion.Euler(0, -30, 0);
                case 2:
                    return Quaternion.Euler(0, 30, 0);
                case 3:
                    return Quaternion.Euler(0, Random.Range(0, 360), 0);
                case 4:
                default:
                    return Quaternion.Euler(0, Random.Range(-45f, 45f), 0);
            }
        }

    }

}