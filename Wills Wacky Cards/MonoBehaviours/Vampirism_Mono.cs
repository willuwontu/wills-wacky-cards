using UnityEngine;
using WWC.Extensions;
using WWC.Interfaces;

namespace WWC.MonoBehaviours
{
    public class Vampirism_Mono : MonoBehaviour, IBattleStartHookHandler, IPointEndHookHandler, IGameStartHookHandler
    {
        public float percentLifeDrain = 1f/16.5f;
        private float damage;
        private CharacterData data;
        private Player target;
        private bool battleStarted = false;
        private float lastTriggered = 0f;

        private void Start()
        {
            data = GetComponentInParent<CharacterData>();
            target = data.player;
            InterfaceGameModeHooksManager.instance.RegisterHooks(this);
        }

        private void Update()
        {
            if (Time.time > (lastTriggered + 1f))
            {
                Damage();
            }
        }

        public void OnPointEnd()
        {
            battleStarted = false;
        }

        public void OnBattleStart()
        {
            battleStarted = true;
        }

        public void OnGameStart()
        {
            UnityEngine.GameObject.Destroy(this.gameObject);
        }

        public void Damage()
        {
            if (!battleStarted)
            {
                return;
            }
            damage = target.data.maxHealth * percentLifeDrain;
            target.data.healthHandler.TakeDamageOverTime(damage * Vector2.up, transform.position, 1f, 0.25f, Color.black, null, null, true);
        }
        public void Destroy()
        {
            UnityEngine.Object.Destroy(this.gameObject);
        }
        private void OnDestroy()
        {
            InterfaceGameModeHooksManager.instance.RemoveHooks(this);
        }
    }
}