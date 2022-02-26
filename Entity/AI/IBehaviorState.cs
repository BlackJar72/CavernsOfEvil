
namespace DLD
{
    public interface IBehaviorState
    {
        public abstract void StateEnter(EntityMob ownerIn);
        public abstract void StateExit(EntityMob ownerIn);
        public abstract bool StateUpdate(EntityMob ownerIn);
        public abstract void StateLateUpdate(EntityMob ownerIn);
        public abstract void StateFixedUpdate(EntityMob ownerIn);
        public abstract bool IsValidState(EntityMob ownerIn);
        public abstract IBehaviorState NextState(EntityMob ownerIn);
    }

}