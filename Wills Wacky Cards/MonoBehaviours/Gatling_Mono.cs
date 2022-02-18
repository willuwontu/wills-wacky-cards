using UnityEngine;
using UnityEngine.UI;
using UnboundLib;
using WWC.Extensions;
using WWC.Interfaces;
using System;
using ModdingUtils.Extensions;

namespace WWC.MonoBehaviours
{
    public class Gatling_Mono : CounterReversibleEffect, IPointStartHookHandler, IGameStartHookHandler, IRoundStartHookHandler, IBattleStartHookHandler
    {
        private int trackedStacks = 0;

        private int stacks = 0;

        private int maxStacks = 20;

        public float rampUp = 1f;

        private float timeSinceShot = 1f;


        public override void OnStart()
        {
            // modifiers of CounterReversibleEffects should start off as noop
            base.gunStatModifier = new GunStatModifier();
            base.gunAmmoStatModifier = new GunAmmoStatModifier();
            base.characterStatModifiersModifier = new CharacterStatModifiersModifier();
            base.gravityModifier = new GravityModifier();
            base.blockModifier = new BlockModifier();
            base.characterDataModifier = new CharacterDataModifier();

            InterfaceGameModeHooksManager.instance.RegisterHooks(this);
            gun.ShootPojectileAction += OnShootProjectileAction;
            stats.OutOfAmmpAction -= OnReloadAction;
        }

        public override CounterStatus UpdateCounter()
        {
            if (trackedStacks != stacks)
            {
                trackedStacks = stacks;
                return CounterStatus.Apply;
            }
            return CounterStatus.Wait;
        }

        public override void UpdateEffects()
        {
            gunStatModifier.attackSpeed_mult = 1;
            for (int i = 0; i < trackedStacks; i++)
            {
                gunStatModifier.attackSpeed_mult *= rampUp;
            }
        }

        public override void Reset()
        {

        }

        public override void OnApply()
        {
            
        }

        public override void OnUpdate()
        {
            if (timeSinceShot > 0f)
            {
                timeSinceShot -= TimeHandler.deltaTime;
            }
            else
            {
                if (stacks > 0)
                {
                    stacks -= 1;
                    timeSinceShot += 0.1f;
                }

            }

            UpdateStats();
        }

        private void OnReloadAction(int maxAmmo)
        {
            var ratio = ((gunAmmo.reloadTime + gunAmmo.reloadTimeAdd) * gunAmmo.reloadTimeMultiplier)/1.5;
            var loss = Mathf.Clamp((int)(ratio * stacks), 0, stacks);
            stacks -= loss;
            UpdateStats();
        }

        private void OnShootProjectileAction(GameObject obj)
        {
            if (stacks < maxStacks)
            {
                stacks += 1;
                timeSinceShot = 1f;
                UpdateStats();
            }
        }

        private void CheckIfValid()
        {
            var haveGatling = false;
            for (int i = 0; i < player.data.currentCards.Count; i++)
            {
                if (player.data.currentCards[i].cardName == "Gatling Gun")
                {
                    haveGatling = true;
                    break;
                }
            }

            if (!haveGatling)
            {
                UnityEngine.GameObject.Destroy(this);
            }
        }

        public void OnPointStart()
        {
            stacks = 0;
            UpdateStats();
            CheckIfValid();
        }

        public void OnRoundStart()
        {
            CheckIfValid();
        }
        public void OnBattleStart()
        {
            CheckIfValid();
        }

        public void OnGameStart()
        {
            UnityEngine.Object.Destroy(this);
        }

        public override void OnOnDestroy()
        {
            stats.OutOfAmmpAction -= OnReloadAction;
            gun.ShootPojectileAction -= OnShootProjectileAction;
            InterfaceGameModeHooksManager.instance.RemoveHooks(this);
        }
    }
}