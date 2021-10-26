using UnityEngine;
using UnityEngine.UI;
using UnboundLib;
using WillsWackyCards.Extensions;
using InControl;
using System;
using System.Collections;
using System.Collections.Generic;

namespace WillsWackyCards.MonoBehaviours
{
    public class SavageWoundsDamage_Mono : Hooked_Mono
    {
        public bool start = false;
        public float duration = 0f;

        private ModdingUtils.MonoBehaviours.ColorEffect colorEffect;

        private bool coroutineStarted;
        private CharacterData data;
        private Player player;

        private void Start()
        {
            data = GetComponentInParent<CharacterData>();
            HookedMonoManager.instance.hookedMonos.Add(this);
        }

        private void FixedUpdate()
        {
            if (!player)
            {
                if (!(data is null))
                {
                    player = data.player;

                    colorEffect = player.gameObject.AddComponent<ModdingUtils.MonoBehaviours.ColorEffect>();
                }
            }

            if (!(player is null) && player.gameObject.activeInHierarchy && !coroutineStarted)
            {
                coroutineStarted = true;
            }

            if (start)
            {
                colorEffect.SetColor(new Color(166f / 255f, 6f / 255f, 0f / 255f, 1f));
                colorEffect.ApplyColor();
                duration -= Time.deltaTime;

                if (duration <= 0)
                {
                    colorEffect.Destroy();
                    Destroy(this);
                }
            }
        }

        public override void OnPointEnd()
        {
            colorEffect.Destroy();
            Destroy(this);
        }

        private void OnDestroy()
        {
            HookedMonoManager.instance.hookedMonos.Remove(this);
            colorEffect.Destroy();
        }

        public void Destroy()
        {
            UnityEngine.Object.Destroy(this);
        }
    }
}