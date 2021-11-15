using UnityEngine;
using WWC.Extensions;

namespace WWC.MonoBehaviours
{
    public class Vampirism_Mono : Hooked_Mono
    {
        public float percentLifeDrain = 1f/16.5f;
        private float damage;
        private bool coroutineStarted;
        private CharacterData data;
        private Player target;
        private bool battleStarted = false;

        private void Start()
        {
            data = GetComponentInParent<CharacterData>();
            HookedMonoManager.instance.hookedMonos.Add(this);
        }

        private void Update()
        {
            if (!target)
            {
                if (!(data is null)) target = data.player;
            }

            if (!(target is null) && target.gameObject.activeInHierarchy && !coroutineStarted)
            {
                coroutineStarted = true;
                InvokeRepeating(nameof(Damage), 0, 1f);
            }

        }

        public override void OnPointEnd()
        {
            battleStarted = false;
        }

        public override void OnBattleStart()
        {
            battleStarted = true;
        }

        public override void OnGameStart()
        {
            UnityEngine.GameObject.Destroy(this);
        }

        public void Damage()
        {
            if (!battleStarted)
            {
                return;
            }
            damage = target.data.maxHealth * percentLifeDrain;
            target.data.healthHandler.TakeDamageOverTime(damage * Vector2.up, transform.position, 1f, 0.25f, Color.black, null, data.player, true);
        }
        public void Destroy()
        {
            UnityEngine.Object.Destroy(this);
        }
    }
}