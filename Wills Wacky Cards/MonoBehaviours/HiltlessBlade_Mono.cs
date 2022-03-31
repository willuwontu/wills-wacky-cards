using UnityEngine;
using WWC.Interfaces;
using UnboundLib.GameModes;
using UnboundLib;
using System.Collections.Generic;
using System;
using System.Linq;

namespace WWC.MonoBehaviours
{
    public class HiltlessBlade_Mono : ReversibleEffect, IPointEndHookHandler, IPointStartHookHandler, IGameStartHookHandler
    {
        public override void OnStart()
        {
            InterfaceGameModeHooksManager.instance.RegisterHooks(this);
            data = GetComponentInParent<CharacterData>();

            this.applyImmediately = false;
            this.SetLivesToEffect(int.MaxValue);
        }

        public void OnPointStart()
        {
            float multiplier = 1f;
            foreach (var card in player.data.currentCards.Where((cardInfo) => cardInfo.cardName.ToLower() == "Hiltless Blade".ToLower()))
            {
                multiplier += 1f;
            }
            gunStatModifier.bulletDamageMultiplier_mult = multiplier;
            this.ApplyModifiers();
        }

        public void OnPointEnd()
        {
            this.ClearModifiers();
        }

        public void OnGameStart()
        {
            UnityEngine.GameObject.Destroy(this);
        }


        public override void OnOnDestroy()
        {
            InterfaceGameModeHooksManager.instance.RemoveHooks(this);
        }
    }
}