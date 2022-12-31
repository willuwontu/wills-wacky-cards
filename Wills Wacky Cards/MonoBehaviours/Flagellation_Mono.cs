using UnityEngine;
using WWC.Interfaces;
using UnboundLib.GameModes;
using UnboundLib;
using System.Collections.Generic;
using System;
using System.Linq;

namespace WWC.MonoBehaviours
{
    public class Flagellation_Mono : ReversibleEffect, IPointEndHookHandler, IPointStartHookHandler, IGameStartHookHandler
    {
        public CardInfo hiltlessBlade;

        private float multiplier = 1f;

        public override void OnStart()
        {
            InterfaceGameModeHooksManager.instance.RegisterHooks(this);
            data = GetComponentInParent<CharacterData>();
            this.applyImmediately = false;
            this.SetLivesToEffect(int.MaxValue);
        }

        public void OnPointStart()
        {
            multiplier = 1f;
            foreach (var card in player.data.currentCards.Where((cardInfo) => cardInfo.cardName.ToLower() == "Flagellation".ToLower()))
            {
                multiplier *= 2.5f;
            }
            this.characterDataModifier.maxHealth_mult = multiplier;
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