using UnityEngine;
using WillsWackyCards.Extensions;

namespace WillsWackyCards.MonoBehaviours
{
    public class Vampirism_Mono : MonoBehaviour
    {
        public float percentLifeDrain = 1f/16.5f;
        private float damage;
        private bool coroutineStarted;
        private CharacterData data;
        private Player target;

        private void Start()
        {
            data = GetComponentInParent<CharacterData>();
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

        public void Damage()
        {
            damage = target.data.maxHealth * percentLifeDrain;
            target.data.healthHandler.TakeDamageOverTime(damage * Vector2.up, transform.position, 1f, 0.25f, Color.black, null, data.player, true);
        }
        public void Destroy()
        {
            UnityEngine.Object.Destroy(this);
        }
    }
}