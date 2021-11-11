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
    public class MomentaryConfusion_Mono : Hooked_Mono
    {
        public int chance = 0;
        public float duration = 0f;
        public float bufferTime = 8f;
        public float timeBetweenChecks = 2f;

        private static System.Random random = new System.Random();
        private bool swapped = false;
        private float swapTimeLeft = 0f;
        private float timeSinceSwap = 0f;
        private float timeSinceCheck = 0f;
        private ModdingUtils.MonoBehaviours.ColorEffect colorEffect;

        private bool coroutineStarted;
        private CharacterData data;
        private Player player;
        private PlayerActions controls = null;
        private List<BindingSource> controlsA = new List<BindingSource>();
        private List<BindingSource> controlsB = new List<BindingSource>();
        //private List<PlayerAction> actions;
        private PlayerAction actionA;
        private PlayerAction actionB;

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
                    controls = data.playerActions;
                }
            }

            if (!(player is null) && player.gameObject.activeInHierarchy && !coroutineStarted)
            {
                coroutineStarted = true;
            }

            if (!swapped && controls != null)
            {
                timeSinceSwap += Time.deltaTime;
                if (timeSinceSwap >= bufferTime)
                {
                    if (timeSinceCheck >= timeBetweenChecks)
                    {
                        timeSinceCheck = 0;
                        var roll = random.Next(100);
                        if (roll <= chance)
                        {
                            UnityEngine.Debug.Log($"[{WillsWackyCards.ModInitials}][Hex] Player {player.playerID} Confusion Curse activated with a roll of {roll} and a chance of {chance}%.");
                            SwapControls();
                        }
                    }
                    else
                    {
                        timeSinceCheck += Time.deltaTime;
                    }
                }
            }
            else
            {
                swapTimeLeft -= Time.deltaTime;

                if (swapTimeLeft <= 0)
                {
                    UndoSwap();
                }
            }
        }

        public override void OnRoundEnd()
        {
            if (swapped)
            {
                UndoSwap();
            }
        }

        public override void OnGameStart()
        {
            UnityEngine.GameObject.Destroy(this);
        }

        private void SwapControls()
        {
            swapped = true;
            timeSinceSwap = 0f;
            timeSinceCheck = 0f;
            swapTimeLeft = duration;

            var actions = new List<PlayerAction>();
            controlsA.Clear();
            controlsB.Clear();
            controlsA = new List<BindingSource>();
            controlsB = new List<BindingSource>();

            actions.Add(controls.Block);
            actions.Add(controls.Fire);
            actions.Add(controls.Left);
            actions.Add(controls.Right);
            actions.Add(controls.Up);
            actions.Add(controls.Down);
            actions.Add(controls.Jump);

            actionA = actions[random.Next(actions.Count)];
            actions.Remove(actionA);
            actionB = actions[random.Next(actions.Count)];
            actions.Remove(actionB);

            colorEffect = player.gameObject.AddComponent<ModdingUtils.MonoBehaviours.ColorEffect>();

            colorEffect.SetColor(new Color(177f / 255f, 199f / 255f, 32f / 255f, 1f));
            colorEffect.ApplyColor();

            foreach (var binding in actionA.Bindings)
            {
                controlsA.Add(binding);
            }
            actionA.ClearBindings();
            foreach (var binding in actionB.Bindings)
            {
                controlsB.Add(binding);
            }
            actionB.ClearBindings();

            foreach (var binding in controlsA)
            {
                actionB.AddBinding(binding);
            }
            foreach (var binding in controlsB)
            {
                actionA.AddBinding(binding);
            }
        }

        private void UndoSwap()
        {
            swapped = false;
            actionA.ClearBindings();
            actionB.ClearBindings();

            foreach (var binding in controlsA)
            {
                actionA.AddBinding(binding);
            }
            foreach (var binding in controlsB)
            {
                actionB.AddBinding(binding);
            }

            colorEffect.Destroy();
        }

        private void OnDestroy()
        {
            if (swapped)
            {
                try
                {
                    UndoSwap();
                }
                catch (NullReferenceException)
                {
                    UnityEngine.Debug.Log($"[{WillsWackyCards.ModInitials}][Debugging] Player {player.playerID} has issues with undoing the swap.");
                }
            }
            HookedMonoManager.instance.hookedMonos.Remove(this);
        }

        public void Destroy()
        {
            UnityEngine.Object.Destroy(this);
        }
    }
}