using UnityEngine;
using WWC.Extensions;
using UnboundLib.GameModes;
using UnboundLib;
using System.Collections.Generic;
using System;
using System.Linq;
using WWC.Interfaces;

namespace WWC.MonoBehaviours
{
    public class SiphonCurses_Mono : ReversibleEffect, IPointStartHookHandler, IPointEndHookHandler, IGameStartHookHandler
    {
        public CardInfo hiltlessBlade;

        private float multiplier = 1f;

        public override void OnStart()
        {
            InterfaceGameModeHooksManager.instance.RegisterHooks(this);
            this.applyImmediately = false;
            this.SetLivesToEffect(int.MaxValue);
        }

        public void OnPointStart()
        {
            multiplier = 1f;
            foreach (var card in player.data.currentCards.Where((cardInfo) => cardInfo.cardName.ToLower() == "Siphon Curses".ToLower()))
            {
                multiplier *= 1.8f;
            }
            this.characterStatModifiersModifier.movementSpeed_mult = multiplier;
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