using ModdingUtils.Extensions;
using Photon.Pun;

namespace WWC.MonoBehaviours
{
    public class ReversibleEffect : MonoBehaviourPun
    {

        public Player player;
        public CharacterStatModifiers stats;
        public Gun gun;
        public GunAmmo gunAmmo;
        public Gravity gravity;
        public HealthHandler health;
        public CharacterData data;
        public Block block;

        internal int livesToEffect = 1;
        private int livesEffected;

        public GunStatModifier gunStatModifier = new GunStatModifier();
        public GunAmmoStatModifier gunAmmoStatModifier = new GunAmmoStatModifier();
        public CharacterDataModifier characterDataModifier = new CharacterDataModifier();
        public CharacterStatModifiersModifier characterStatModifiersModifier = new CharacterStatModifiersModifier();
        public GravityModifier gravityModifier = new GravityModifier();
        public BlockModifier blockModifier = new BlockModifier();

        public bool applyImmediately = true;
        private bool modifiersActive;
        private bool wasActiveLastFrame = true;

        public int numEnemyPlayers
        {
            get
            {
                if (player == null) { return -1; }
                int num = ModdingUtils.Utils.PlayerStatus.GetNumberOfEnemyPlayers(player);
                if (num > 0)
                {
                    return num;
                }
                else
                {
                    return 1;
                }
            }
            set
            { }
        }

        public void Awake()
        {
            if (player is null)
            {
                player = gameObject.GetComponent<Player>();
            }
            gun = player.GetComponent<Holding>().holdable.GetComponent<Gun>();
            data = player.GetComponent<CharacterData>();
            health = player.GetComponent<HealthHandler>();
            gravity = player.GetComponent<Gravity>();
            block = player.GetComponent<Block>();
            gunAmmo = gun.GetComponentInChildren<GunAmmo>();
            stats = player.GetComponent<CharacterStatModifiers>();
            OnAwake();
        }
        public virtual void OnAwake()
        {

        }

        public void OnEnable()
        {
            if (livesEffected >= livesToEffect)
            {
                Destroy(this);
            }
            OnOnEnable();
        }

        public virtual void OnOnEnable()
        {

        }

        public void Start()
        {

            wasActiveLastFrame = ModdingUtils.Utils.PlayerStatus.PlayerAliveAndSimulated(player);

            OnStart();

            if (applyImmediately)
            {
                ApplyModifiers();
            }


        }
        public virtual void OnStart()
        {
            // this is where derived effects should modify each of the following:
            /* base.gunStatModifier
             * base.gunAmmoStatModifier
             * base.playerColorModifier
             * base.characterStatModifiersModifier
             * base.blockModifier
             * base.characterDataModifier
             * 
             * and optionally, if the effect should not be applied until later, base.applyImmediately
             */
        }

        void FixedUpdate()
        {
            OnFixedUpdate();
        }
        public virtual void OnFixedUpdate()
        {

        }
        void Update()
        {
            if (wasActiveLastFrame && !ModdingUtils.Utils.PlayerStatus.PlayerAliveAndSimulated(player))
            {
                livesEffected++;
            }


            if (livesEffected >= livesToEffect)
            {
                Destroy(this);
            }

            OnUpdate();
        }
        public virtual void OnUpdate()
        {

        }
        public void LateUpdate()
        {
            wasActiveLastFrame = ModdingUtils.Utils.PlayerStatus.PlayerAliveAndSimulated(player);

            OnLateUpdate();
        }
        public virtual void OnLateUpdate()
        {
        }
        public void OnDisable()
        {

            livesEffected++;

            if (livesEffected >= livesToEffect)
            {
                Destroy(this);
            }

        }
        public virtual void OnOnDisable()
        {

        }
        public void OnDestroy()
        {
            ClearModifiers();
            OnOnDestroy();
        }

        public virtual void OnOnDestroy()
        {
            // derived effects should put any necessary cleanup here
        }
        public void ApplyModifiers()
        {
            if (modifiersActive) { return; }
            gunStatModifier.ApplyGunStatModifier(gun);
            gunAmmoStatModifier.ApplyGunAmmoStatModifier(gunAmmo);
            characterStatModifiersModifier.ApplyCharacterStatModifiersModifier(stats);
            characterDataModifier.ApplyCharacterDataModifier(data);
            gravityModifier.ApplyGravityModifier(gravity);
            blockModifier.ApplyBlockModifier(block);
            modifiersActive = true;
        }
        public void ClearModifiers(bool clear = true)
        {
            if (!modifiersActive) { return; }
            gunStatModifier.RemoveGunStatModifier(gun, clear);
            gunAmmoStatModifier.RemoveGunAmmoStatModifier(gunAmmo, clear);
            characterStatModifiersModifier.RemoveCharacterStatModifiersModifier(stats, clear);
            characterDataModifier.RemoveCharacterDataModifier(data, clear);
            gravityModifier.RemoveGravityModifier(gravity, clear);
            blockModifier.RemoveBlockModifier(block, clear);
            modifiersActive = false;

        }
        public void Destroy()
        {
            ClearModifiers();
            UnityEngine.Object.Destroy(this);
        }

        public void SetLivesToEffect(int lives = 1)
        {
            livesToEffect = lives;
        }

    }
}