using ModdingUtils.Extensions;

namespace WWC.MonoBehaviours
{
    public abstract class CounterReversibleEffect : ReversibleEffect
    {
        public CounterStatus status;

        public CounterReversibleEffect()
        {
            livesToEffect = int.MaxValue; // this can be changed with CounterReversibleEffect.SetLivesToEffect(lives)
        }

        public abstract CounterStatus UpdateCounter();
        // this method is called on every frame and should update the counterValue field as well as return the current CounterStatus
        // this method can make use of (but is not limited to):
        /* base.player
         * base.gun
         * base.characterStatModifiers
         * base.gravity
         * base.block
         * base.data
         * base.health
         */
        public abstract void UpdateEffects();
        // this method should update the temporary modifiers (but does not have to update any of them):
        /* base.gunStatModifier
         * base.gunAmmoStatModifier
         * base.playerColorModifier
         * base.characterStatModifiersModifier
         * base.characterDataModifier
         */
        // it will only be called immediately after CounterStatus.Apply is set and should ideally use the value of counterValue to change the modifiers
        // IMPORTANT NOTE: the effects will always be cleared before they are applied, and thus this method cannot use any values previously set to them
        public abstract void OnApply();
        // this will be called immediately after the effects have been applied, can/should be used to call Reset();
        public abstract void Reset();
        // Reset will be called during OnEnable, OnDisable, and also at the beginning of each life of the player
        public virtual void OnRemove()
        {
            // this method should perform any ancillary cleanup when the effects are removed, which should not be required
        }

        // if cleanup needs to be done when the effect is destroyed (this should not be necessary) then it can be done by overriding the method "OnOnDestroy"

        public override void OnAwake()
        {
            // nothing else should happen during Awake and this method should not be hidden
        }

        public override void OnOnEnable()
        {
            Reset();
            ClearModifiers();
            OnRemove();
        }

        public override void OnStart()
        {
            // modifiers of CounterReversibleEffects should start off as noop
            base.gunStatModifier = new GunStatModifier();
            base.gunAmmoStatModifier = new GunAmmoStatModifier();
            base.characterStatModifiersModifier = new CharacterStatModifiersModifier();
            base.gravityModifier = new GravityModifier();
            base.blockModifier = new BlockModifier();
            base.characterDataModifier = new CharacterDataModifier();
        }

        public override void OnFixedUpdate()
        {
            // nothing should happen during FixedUpdate and this method should not be hidden
        }

        public override void OnUpdate()
        {
            UpdateStats();
        }

        public void UpdateStats()
        {
            // reset if the player is not alive / active
            if (!ModdingUtils.Utils.PlayerStatus.PlayerAliveAndSimulated(player))
            {
                Reset();
                ClearModifiers();
                OnRemove();
                return;
            }

            status = UpdateCounter();

            switch (status)
            {
                case CounterStatus.Apply:
                    ClearModifiers(); // modifiers are ALWAYS cleared before they are updated and applied
                    UpdateEffects();
                    ApplyModifiers();
                    OnApply();
                    break;
                case CounterStatus.Wait:
                    break;
                case CounterStatus.Remove:
                    ClearModifiers();
                    OnRemove();
                    break;
                case CounterStatus.Destroy:
                    OnRemove();
                    Destroy();
                    break;
                default:
                    break;
            }
        }

        public override void OnLateUpdate()
        {
        }
        public override void OnOnDisable()
        {
            Reset();
            ClearModifiers();
            OnRemove();
        }

        public enum CounterStatus
        {
            Apply,
            Wait,
            Remove,
            Destroy
        }
    }
}