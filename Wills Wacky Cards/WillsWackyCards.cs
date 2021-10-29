using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using UnityEngine;
using BepInEx;
using BepInEx.Configuration;
using UnboundLib;
using UnboundLib.Utils.UI;
using UnboundLib.GameModes;
using UnboundLib.Cards;
using UnboundLib.Utils;
using UnboundLib.Networking;
using WillsWackyCards.Cards;
using WillsWackyCards.Cards.Curses;
using WillsWackyCards.Extensions;
using WillsWackyManagers.Utils;
using WillsWackyCards.MonoBehaviours;
using HarmonyLib;
using Photon.Pun;
using TMPro;
using CardChoiceSpawnUniqueCardPatch.CustomCategories;

namespace WillsWackyCards
{
    [BepInDependency("com.willis.rounds.unbound", BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency("com.willuwontu.rounds.managers", BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency("pykess.rounds.plugins.moddingutils", BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency("pykess.rounds.plugins.cardchoicespawnuniquecardpatch", BepInDependency.DependencyFlags.HardDependency)]
    [BepInPlugin(ModId, ModName, Version)]
    [BepInProcess("Rounds.exe")]
    public class WillsWackyCards : BaseUnityPlugin
    {
        private const string ModId = "com.willuwontu.rounds.cards";
        private const string ModName = "Will's Wacky Cards";
        public const string Version = "1.3.2"; // What version are we on (major.minor.patch)?

        public const string ModInitials = "WWC";
        public const string CurseInitials = "Curse";

        public static ConfigEntry<bool> enableTableFlip;

        public static WillsWackyCards instance { get; private set; }
        public static CardRemover remover;

        void Awake()
        {
            var harmony = new Harmony(ModId);
            harmony.PatchAll();
        }
        void Start()
        {
            Unbound.RegisterCredits(ModName, new string[] { "willuwontu" }, new string[] { "github", "Ko-Fi" }, new string[] { "https://github.com/willuwontu/wills-wacky-cards", "https://ko-fi.com/willuwontu" });

            instance = this;
            instance.gameObject.name = "WillsWackyCards";

            enableTableFlip = Config.Bind("Wills Wacky Cards", "Enabled", true, "Enable table flip and reroll.");

            gameObject.AddComponent<HookedMonoManager>();
            remover = gameObject.AddComponent<CardRemover>();

            UnityEngine.Debug.Log("[WWC] Loading Cards");
            CustomCard.BuildCard<AmmoCache>();
            CustomCard.BuildCard<Shotgun>();
            CustomCard.BuildCard<SlowDeath>();
            CustomCard.BuildCard<Vampirism>();
            CustomCard.BuildCard<FastBall>();
            CustomCard.BuildCard<SlowBall>();
            //CustomCard.BuildCard<Minigun>();
            CustomCard.BuildCard<WildAim>();
            CustomCard.BuildCard<RunningShoes>();
            CustomCard.BuildCard<JumpingShoes>();
            CustomCard.BuildCard<PastaShells>(cardInfo => { CurseManager.instance.RegisterCurse(cardInfo); });
            CustomCard.BuildCard<CrookedLegs>(cardInfo => { CurseManager.instance.RegisterCurse(cardInfo); });
            CustomCard.BuildCard<Bleed>(cardInfo => { CurseManager.instance.RegisterCurse(cardInfo); });
            CustomCard.BuildCard<DrivenToEarth>(cardInfo => { CurseManager.instance.RegisterCurse(cardInfo); });
            CustomCard.BuildCard<Misfire>(cardInfo => { CurseManager.instance.RegisterCurse(cardInfo); });
            CustomCard.BuildCard<SlowReflexes>(cardInfo => { CurseManager.instance.RegisterCurse(cardInfo); });
            CustomCard.BuildCard<CounterfeitAmmo>(cardInfo => { CurseManager.instance.RegisterCurse(cardInfo); });
            CustomCard.BuildCard<NeedleBullets>(cardInfo => { CurseManager.instance.RegisterCurse(cardInfo); });
            CustomCard.BuildCard<EasyTarget>(cardInfo => { CurseManager.instance.RegisterCurse(cardInfo); });
            CustomCard.BuildCard<WildShots>(cardInfo => { CurseManager.instance.RegisterCurse(cardInfo); });
            CustomCard.BuildCard<Hex>();
            CustomCard.BuildCard<Gatling>();
            CustomCard.BuildCard<PlasmaRifle>();
            CustomCard.BuildCard<PlasmaShotgun>();
            CustomCard.BuildCard<UnstoppableForce>();
            CustomCard.BuildCard<ImmovableObject>();
            CustomCard.BuildCard<HotPotato>();
            CustomCard.BuildCard<TableFlip>(CardInfo => RerollManager.instance.tableFlipCard = CardInfo);
            CustomCard.BuildCard<Reroll>(CardInfo => RerollManager.instance.rerollCard = CardInfo);
            CustomCard.BuildCard<MomentaryConfusion>(cardInfo => { CurseManager.instance.RegisterCurse(cardInfo); });
            CustomCard.BuildCard<SavageWounds>();
            CustomCard.BuildCard<RitualisticSacrifice>();
            CustomCard.BuildCard<ForbiddenMagics>();
            CustomCard.BuildCard<PurifyingLight>();
            CustomCard.BuildCard<CursedKnowledge>();
            CustomCard.BuildCard<EnduranceTraining>();
            CustomCard.BuildCard<AdrenalineRush>();
            CustomCard.BuildCard<RunicWards>();
            CustomCard.BuildCard<HiltlessBlade>();
            UnityEngine.Debug.Log("[WWC] Cards Built");
            

            this.ExecuteAfterSeconds(0.4f, ChangeCards);

            StartCoroutine(BuildMomentumCards());

            GameModeManager.AddHook(GameModeHooks.HookGameEnd, GameEnd);
            GameModeManager.AddHook(GameModeHooks.HookGameStart, GameStart);
            GameModeManager.AddHook(GameModeHooks.HookBattleStart, BattleStart);
            GameModeManager.AddHook(GameModeHooks.HookPlayerPickStart, PlayerPickStart);
            GameModeManager.AddHook(GameModeHooks.HookPlayerPickEnd, PlayerPickEnd);
            GameModeManager.AddHook(GameModeHooks.HookPointStart, PointStart);
            GameModeManager.AddHook(GameModeHooks.HookPointEnd, PointEnd);
            GameModeManager.AddHook(GameModeHooks.HookPickStart, PickStart);
            GameModeManager.AddHook(GameModeHooks.HookPickEnd, PickEnd);
            GameModeManager.AddHook(GameModeHooks.HookRoundStart, RoundStart);
            GameModeManager.AddHook(GameModeHooks.HookRoundEnd, RoundEnd);

            var networkEvents = gameObject.AddComponent<NetworkEventCallbacks>();
            networkEvents.OnJoinedRoomEvent += OnJoinedRoomAction;
            networkEvents.OnLeftRoomEvent += OnLeftRoomAction;

            Unbound.RegisterMenu(ModName, () => { }, NewGUI, null, false);
            Unbound.RegisterHandshake(ModId, OnHandShakeCompleted);
        }

        private void OnJoinedRoomAction()
        {

        }

        private void OnLeftRoomAction()
        {

        }

        IEnumerator BuildMomentumCards()
        {
            var stacks = 0;
            yield return StartCoroutine(WaitFor.Frames(7));
            while (stacks <= 20)
            {
                MomentumTracker.stacks = stacks;
                //UnityEngine.Debug.Log($"[{WillsWackyCards.ModInitials}][Debugging] {cardInfo.cardName} assigned to slot {MomentumTracker.stacks}"); 
                CustomCard.BuildCard<BuildImmovableObject>(cardInfo => { MomentumTracker.createdDefenseCards.Add(stacks, cardInfo); ModdingUtils.Utils.Cards.instance.AddHiddenCard(cardInfo); });
                CustomCard.BuildCard<BuildUnstoppableForce>(cardInfo => { MomentumTracker.createdOffenseCards.Add(stacks, cardInfo); ModdingUtils.Utils.Cards.instance.AddHiddenCard(cardInfo); });
                yield return StartCoroutine(WaitFor.Frames(7));
                stacks++;
            }
            yield return StartCoroutine(WaitFor.Frames(7));
            MomentumTracker.stacks = 0;

            //foreach (var cardInfo in MomentumTracker.createdDefenseCards)
            //{
            //    UnityEngine.Debug.Log($"Defense Card {cardInfo.Key}: {cardInfo.Value.cardName}, Stack Count: {cardInfo.Value.GetComponent<MomentumCard_Mono>().stacks}");
            //}
            //foreach (var cardInfo in MomentumTracker.createdOffenseCards)
            //{
            //    UnityEngine.Debug.Log($"Offense Card {cardInfo.Key}: {cardInfo.Value.cardName}, Stack Count: {cardInfo.Value.GetComponent<MomentumCard_Mono>().stacks}");
            //}
        }

        private static void NewGUI(GameObject menu)
        {
            MenuHandler.CreateText($"{ModName} Options", menu, out TextMeshProUGUI _);
            MenuHandler.CreateText(" ", menu, out TextMeshProUGUI _);
            MenuHandler.CreateToggle(enableTableFlip.Value, "Enable Table Flip and Reroll", menu, value => { enableTableFlip.Value = value; OnHandShakeCompleted(); } );
        }

        private static void OnHandShakeCompleted()
        {
            if (PhotonNetwork.IsMasterClient)
            {
                NetworkingManager.RPC_Others(typeof(WillsWackyCards), nameof(SyncSettings),
                    new[] { enableTableFlip.Value });
            }
        }

        [UnboundRPC]
        private static void SyncSettings(bool tableFlipEnabled)
        {
            enableTableFlip.Value = tableFlipEnabled;
        }

        public static class WaitFor
        {
            public static IEnumerator Frames(int frameCount)
            {
                if (frameCount <= 0)
                {
                    throw new ArgumentOutOfRangeException("frameCount", "Cannot wait for less that 1 frame");
                }

                while (frameCount > 0)
                {
                    frameCount--;
                    yield return null;
                }
            }
        }

        IEnumerator RoundStart(IGameModeHandler gm)
        {
            foreach (var hookedMono in HookedMonoManager.instance.hookedMonos)
            {
                hookedMono.OnRoundStart();
            }
            yield break;
        }

        IEnumerator RoundEnd(IGameModeHandler gm)
        {
            foreach (var hookedMono in HookedMonoManager.instance.hookedMonos)
            {
                hookedMono.OnRoundEnd();
            }
            yield break;
        }

        IEnumerator PointStart(IGameModeHandler gm)
        {
            foreach (var player in PlayerManager.instance.players)
            {
                player.data.stats.GetAdditionalData().shieldsRemaining = player.data.currentCards.Where((cardInfo) => cardInfo.cardName == "Runic Wards").Count();
            }
            foreach (var hookedMono in HookedMonoManager.instance.hookedMonos)
            {
                //UnityEngine.Debug.Log($"[{ModInitials}][Debugging] Running OnPointStart");
                hookedMono.OnPointStart();
            }
            yield break;
        }

        IEnumerator PointEnd(IGameModeHandler gm)
        {
            foreach (var player in PlayerManager.instance.players)
            {
                var plasma = player.gameObject.GetComponent<PlasmaWeapon_Mono>();
                if (plasma)
                {
                    plasma.canShoot = false;
                    player.data.weaponHandler.gun.GetAdditionalData().beginCharge = false;
                    player.data.weaponHandler.gun.currentCharge = 0f;
                }
            }
            foreach (var hookedMono in HookedMonoManager.instance.hookedMonos)
            {
                //UnityEngine.Debug.Log($"[{ModInitials}][Debugging] Running OnPointEnd");
                hookedMono.OnPointEnd();
            }
            yield break;
        }

        IEnumerator PlayerPickStart(IGameModeHandler gm)
        {
            foreach (var hookedMono in HookedMonoManager.instance.hookedMonos)
            {
                hookedMono.OnPlayerPickStart();
            }
            yield break;
        }

        IEnumerator PlayerPickEnd(IGameModeHandler gm)
        {
            foreach (var hookedMono in HookedMonoManager.instance.hookedMonos)
            {
                hookedMono.OnPlayerPickEnd();
            }
            yield break;
        }

        IEnumerator PickStart(IGameModeHandler gm)
        {
            foreach (var hookedMono in HookedMonoManager.instance.hookedMonos)
            {
                hookedMono.OnPickStart();
            }
            yield break;
        }

        IEnumerator PickEnd(IGameModeHandler gm)
        {
            foreach (var hookedMono in HookedMonoManager.instance.hookedMonos)
            {
                hookedMono.OnPickEnd();
            }
            yield break;
        }

        IEnumerator BattleStart(IGameModeHandler gm)
        {
            foreach (var player in PlayerManager.instance.players)
            {
                var minigun = player.gameObject.GetComponent<Minigun_Mono>();
                if (minigun)
                {
                    minigun.heat = 0f;
                    minigun.overheated = false;
                    player.data.weaponHandler.gun.GetAdditionalData().overHeated = false;
                    var gunAmmo = player.data.weaponHandler.gun.GetComponentInChildren<GunAmmo>();
                    gunAmmo.ReloadAmmo();
                }
                var plasma = player.gameObject.GetComponent<PlasmaWeapon_Mono>();
                if (plasma)
                {
                    plasma.canShoot = true;
                }
            }
            foreach (var hookedMono in HookedMonoManager.instance.hookedMonos)
            {
                hookedMono.OnBattleStart();
            }
            yield break;
        }
        IEnumerator GameStart(IGameModeHandler gm)
        {
            MomentumTracker.stacks = 0;

            foreach (var player in PlayerManager.instance.players)
            {
                if (!ModdingUtils.Extensions.CharacterStatModifiersExtension.GetAdditionalData(player.data.stats).blacklistedCategories.Contains(Minigun.componentCategory))
                {
                    ModdingUtils.Extensions.CharacterStatModifiersExtension.GetAdditionalData(player.data.stats).blacklistedCategories.Add(Minigun.componentCategory);
                }
                if (!enableTableFlip.Value)
                {
                    if (!ModdingUtils.Extensions.CharacterStatModifiersExtension.GetAdditionalData(player.data.stats).blacklistedCategories.Contains(TableFlip.tableFlipCategory))
                    {
                        ModdingUtils.Extensions.CharacterStatModifiersExtension.GetAdditionalData(player.data.stats).blacklistedCategories.Add(TableFlip.tableFlipCategory);
                    }                    
                }
                else
                {
                    ModdingUtils.Extensions.CharacterStatModifiersExtension.GetAdditionalData(player.data.stats).blacklistedCategories.RemoveAll((category) => category == TableFlip.tableFlipCategory);
                }
            }
            foreach (var hookedMono in HookedMonoManager.instance.hookedMonos)
            {
                hookedMono.OnGameStart();
            }
            yield break;
        }

        IEnumerator GameEnd(IGameModeHandler gm)
        {
            DestroyAll<Minigun_Mono>();
            DestroyAll<Vampirism_Mono>();
            DestroyAll<Gatling_Mono>();
            DestroyAll<Misfire_Mono>();
            foreach (var hookedMono in HookedMonoManager.instance.hookedMonos)
            {
                hookedMono.OnGameEnd();
            }
            yield break;
        }
        void DestroyAll<T>() where T : UnityEngine.Object
        {
            var objects = GameObject.FindObjectsOfType<T>();
            for (int i = objects.Length - 1; i >= 0; i--)
            {
                UnityEngine.Debug.Log($"Attempting to Destroy {objects[i].GetType().Name} number {i}");
                UnityEngine.Object.Destroy(objects[i]);
            }
        }

        private static void ChangeCards()
        {
            UnityEngine.Debug.Log("[WWC] Modifying Cards");
            List<CardCategory> categories;
            //CardInfo cardtest = CardManager.cards.Values.First(card => card.cardInfo.name.ToUpper() == "PONG").cardInfo;

            foreach (var info in CardManager.cards.Values.ToArray())
            {
                switch (info.cardInfo.cardName.ToUpper())
                {
                    case "DECAY":
                        {
                            UnityEngine.Debug.Log("[WWC] Found Decay");
                            if (info.cardInfo.categories != null)
                            {
                                categories = new List<CardCategory>();
                                for (int i = 0; i < info.cardInfo.categories.Length; i++)
                                {
                                    categories.Add(info.cardInfo.categories[i]);
                                }
                                categories.Add(CustomCardCategories.instance.CardCategory("Decay"));
                                info.cardInfo.categories = categories.ToArray();
                                categories = new List<CardCategory>();
                                for (int i = 0; i < info.cardInfo.blacklistedCategories.Length; i++)
                                {
                                    categories.Add(info.cardInfo.blacklistedCategories[i]);
                                }
                                categories.Add(CustomCardCategories.instance.CardCategory("Decay"));
                                info.cardInfo.blacklistedCategories = categories.ToArray();
                            }
                            else
                            {
                                info.cardInfo.categories = new CardCategory[] { CustomCardCategories.instance.CardCategory("Decay") };
                                info.cardInfo.blacklistedCategories = new CardCategory[] { CustomCardCategories.instance.CardCategory("Decay") }; 
                            }
                            UnityEngine.Debug.Log("[WWC] Modified Decay");
                            break;
                        }
                    case "FLAMETHROWER":
                        {
                            UnityEngine.Debug.Log("[WWC] Found Flamethrower");
                            if (info.cardInfo.categories != null)
                            {
                                categories = new List<CardCategory>();
                                for (int i = 0; i < info.cardInfo.categories.Length; i++)
                                {
                                    categories.Add(info.cardInfo.categories[i]);
                                }
                                categories.Add(CustomCardCategories.instance.CardCategory("GunType"));
                                info.cardInfo.categories = categories.ToArray();
                                categories = new List<CardCategory>();
                                for (int i = 0; i < info.cardInfo.blacklistedCategories.Length; i++)
                                {
                                    categories.Add(info.cardInfo.blacklistedCategories[i]);
                                }
                                categories.Add(CustomCardCategories.instance.CardCategory("WWC Gun Type"));
                                info.cardInfo.blacklistedCategories = categories.ToArray();
                            }
                            else
                            {
                                info.cardInfo.categories = new CardCategory[] { CustomCardCategories.instance.CardCategory("GunType") };
                                info.cardInfo.blacklistedCategories = new CardCategory[] { CustomCardCategories.instance.CardCategory("WWC Gun Type") }; 
                            }
                            UnityEngine.Debug.Log("[WWC] Modified Flamethrower");
                            break;
                        }
                    case "FIRE HYDRANT":
                        {
                            UnityEngine.Debug.Log("[WWC] Found Fire Hydrant");
                            if (info.cardInfo.categories != null)
                            {
                                categories = new List<CardCategory>();
                                for (int i = 0; i < info.cardInfo.categories.Length; i++)
                                {
                                    categories.Add(info.cardInfo.categories[i]);
                                }
                                categories.Add(CustomCardCategories.instance.CardCategory("GunType"));
                                info.cardInfo.categories = categories.ToArray();
                                categories = new List<CardCategory>();
                                for (int i = 0; i < info.cardInfo.blacklistedCategories.Length; i++)
                                {
                                    categories.Add(info.cardInfo.blacklistedCategories[i]);
                                }
                                categories.Add(CustomCardCategories.instance.CardCategory("WWC Gun Type"));
                                info.cardInfo.blacklistedCategories = categories.ToArray();
                            }
                            else
                            {
                                info.cardInfo.categories = new CardCategory[] { CustomCardCategories.instance.CardCategory("GunType") };
                                info.cardInfo.blacklistedCategories = new CardCategory[] { CustomCardCategories.instance.CardCategory("WWC Gun Type") };
                            }
                            UnityEngine.Debug.Log("[WWC] Modified Fire Hydrant");
                            break;
                        }
                    case "PONG":
                        {
                            UnityEngine.Debug.Log("[WWC] Found Pong");
                            if (info.cardInfo.categories != null)
                            {
                                categories = new List<CardCategory>();
                                for (int i = 0; i < info.cardInfo.categories.Length; i++)
                                {
                                    categories.Add(info.cardInfo.categories[i]);
                                }
                                categories.Add(CustomCardCategories.instance.CardCategory("GunType"));
                                info.cardInfo.categories = categories.ToArray();
                                categories = new List<CardCategory>();
                                for (int i = 0; i < info.cardInfo.blacklistedCategories.Length; i++)
                                {
                                    categories.Add(info.cardInfo.blacklistedCategories[i]);
                                }
                                categories.Add(CustomCardCategories.instance.CardCategory("WWC Gun Type"));
                                info.cardInfo.blacklistedCategories = categories.ToArray();
                            }
                            else
                            {
                                info.cardInfo.categories = new CardCategory[] { CustomCardCategories.instance.CardCategory("GunType") };
                                info.cardInfo.blacklistedCategories = new CardCategory[] { CustomCardCategories.instance.CardCategory("WWC Gun Type") }; 
                            }
                            UnityEngine.Debug.Log("[WWC] Modified Pong");
                            break;
                        }
                    case "COMB":
                        {
                            UnityEngine.Debug.Log("[WWC] Found Comb");
                            if (info.cardInfo.categories != null)
                            {
                                categories = new List<CardCategory>();
                                for (int i = 0; i < info.cardInfo.categories.Length; i++)
                                {
                                    categories.Add(info.cardInfo.categories[i]);
                                }
                                categories.Add(CustomCardCategories.instance.CardCategory("GunType"));
                                info.cardInfo.categories = categories.ToArray();
                                categories = new List<CardCategory>();
                                for (int i = 0; i < info.cardInfo.blacklistedCategories.Length; i++)
                                {
                                    categories.Add(info.cardInfo.blacklistedCategories[i]);
                                }
                                categories.Add(CustomCardCategories.instance.CardCategory("WWC Gun Type"));
                                info.cardInfo.blacklistedCategories = categories.ToArray();
                            }
                            else
                            {
                                info.cardInfo.categories = new CardCategory[] { CustomCardCategories.instance.CardCategory("GunType") };
                                info.cardInfo.blacklistedCategories = new CardCategory[] { CustomCardCategories.instance.CardCategory("WWC Gun Type") };
                            }
                            UnityEngine.Debug.Log("[WWC] Modified Comb");
                            break;
                        }
                    case "STAR":
                        {
                            UnityEngine.Debug.Log("[WWC] Found Star");
                            if (info.cardInfo.categories != null)
                            {
                                categories = new List<CardCategory>();
                                for (int i = 0; i < info.cardInfo.categories.Length; i++)
                                {
                                    categories.Add(info.cardInfo.categories[i]);
                                }
                                categories.Add(CustomCardCategories.instance.CardCategory("No Minigun"));
                                info.cardInfo.categories = categories.ToArray();
                                categories = new List<CardCategory>();
                                for (int i = 0; i < info.cardInfo.blacklistedCategories.Length; i++)
                                {
                                    categories.Add(info.cardInfo.blacklistedCategories[i]);
                                }
                                categories.Add(CustomCardCategories.instance.CardCategory("No Minigun"));
                                info.cardInfo.blacklistedCategories = categories.ToArray();
                            }
                            else
                            {
                                info.cardInfo.categories = new CardCategory[] { CustomCardCategories.instance.CardCategory("No Minigun") };
                                info.cardInfo.blacklistedCategories = new CardCategory[] { CustomCardCategories.instance.CardCategory("No Minigun") };
                            }
                            UnityEngine.Debug.Log("[WWC] Modified Star");
                            break;
                        }
                    case "HAWK":
                        {
                            UnityEngine.Debug.Log("[WWC] Found Hawk");
                            if (info.cardInfo.categories != null)
                            {
                                categories = new List<CardCategory>();
                                for (int i = 0; i < info.cardInfo.categories.Length; i++)
                                {
                                    categories.Add(info.cardInfo.categories[i]);
                                }
                                categories.Add(CustomCardCategories.instance.CardCategory("GunType"));
                                info.cardInfo.categories = categories.ToArray();
                                categories = new List<CardCategory>();
                                for (int i = 0; i < info.cardInfo.blacklistedCategories.Length; i++)
                                {
                                    categories.Add(info.cardInfo.blacklistedCategories[i]);
                                }
                                categories.Add(CustomCardCategories.instance.CardCategory("WWC Gun Type"));
                                info.cardInfo.blacklistedCategories = categories.ToArray();
                            }
                            else
                            {
                                info.cardInfo.categories = new CardCategory[] { CustomCardCategories.instance.CardCategory("GunType") };
                                info.cardInfo.blacklistedCategories = new CardCategory[] { CustomCardCategories.instance.CardCategory("WWC Gun Type") };
                            }
                            UnityEngine.Debug.Log("[WWC] Modified Hawk");
                            break;
                        }
                    case "ROLLING THUNDER":
                        {
                            UnityEngine.Debug.Log("[WWC] Found Rolling Thunder");
                            if (info.cardInfo.categories != null)
                            {
                                categories = new List<CardCategory>();
                                for (int i = 0; i < info.cardInfo.categories.Length; i++)
                                {
                                    categories.Add(info.cardInfo.categories[i]);
                                }
                                categories.Add(CustomCardCategories.instance.CardCategory("GunType"));
                                info.cardInfo.categories = categories.ToArray();
                                categories = new List<CardCategory>();
                                for (int i = 0; i < info.cardInfo.blacklistedCategories.Length; i++)
                                {
                                    categories.Add(info.cardInfo.blacklistedCategories[i]);
                                }
                                categories.Add(CustomCardCategories.instance.CardCategory("WWC Gun Type"));
                                info.cardInfo.blacklistedCategories = categories.ToArray();
                            }
                            else
                            {
                                info.cardInfo.categories = new CardCategory[] { CustomCardCategories.instance.CardCategory("GunType") };
                                info.cardInfo.blacklistedCategories = new CardCategory[] { CustomCardCategories.instance.CardCategory("WWC Gun Type") };
                            }
                            UnityEngine.Debug.Log("[WWC] Modified Rolling Thunder");
                            break;
                        }
                    case "LASER":
                        {
                            UnityEngine.Debug.Log("[WWC] Found Laser");
                            if (info.cardInfo.categories != null)
                            {
                                categories = new List<CardCategory>();
                                for (int i = 0; i < info.cardInfo.categories.Length; i++)
                                {
                                    categories.Add(info.cardInfo.categories[i]);
                                }
                                categories.Add(CustomCardCategories.instance.CardCategory("GunType"));
                                info.cardInfo.categories = categories.ToArray();
                                categories = new List<CardCategory>();
                                for (int i = 0; i < info.cardInfo.blacklistedCategories.Length; i++)
                                {
                                    categories.Add(info.cardInfo.blacklistedCategories[i]);
                                }
                                categories.Add(CustomCardCategories.instance.CardCategory("WWC Gun Type"));
                                info.cardInfo.blacklistedCategories = categories.ToArray();
                            }
                            else
                            {
                                info.cardInfo.categories = new CardCategory[] { CustomCardCategories.instance.CardCategory("GunType") };
                                info.cardInfo.blacklistedCategories = new CardCategory[] { CustomCardCategories.instance.CardCategory("WWC Gun Type") };
                            }
                            UnityEngine.Debug.Log("[WWC] Modified Laserr");
                            break;
                        }
                    case "FRAGMENTATION":
                        {
                            UnityEngine.Debug.Log("[WWC] Found Fragmentation");
                            if (info.cardInfo.categories != null)
                            {
                                categories = new List<CardCategory>();
                                for (int i = 0; i < info.cardInfo.categories.Length; i++)
                                {
                                    categories.Add(info.cardInfo.categories[i]);
                                }
                                categories.Add(CustomCardCategories.instance.CardCategory("GunType"));
                                info.cardInfo.categories = categories.ToArray();
                                categories = new List<CardCategory>();
                                for (int i = 0; i < info.cardInfo.blacklistedCategories.Length; i++)
                                {
                                    categories.Add(info.cardInfo.blacklistedCategories[i]);
                                }
                                categories.Add(CustomCardCategories.instance.CardCategory("WWC Gun Type"));
                                info.cardInfo.blacklistedCategories = categories.ToArray();
                            }
                            else
                            {
                                info.cardInfo.categories = new CardCategory[] { CustomCardCategories.instance.CardCategory("GunType") };
                                info.cardInfo.blacklistedCategories = new CardCategory[] { CustomCardCategories.instance.CardCategory("WWC Gun Type") };
                            }
                            UnityEngine.Debug.Log("[WWC] Modified Fragmentation");
                            break;
                        }
                    case "FIREWORKS":
                        {
                            UnityEngine.Debug.Log("[WWC] Found Fireworks");
                            if (info.cardInfo.categories != null)
                            {
                                categories = new List<CardCategory>();
                                for (int i = 0; i < info.cardInfo.categories.Length; i++)
                                {
                                    categories.Add(info.cardInfo.categories[i]);
                                }
                                categories.Add(CustomCardCategories.instance.CardCategory("GunType"));
                                info.cardInfo.categories = categories.ToArray();
                                categories = new List<CardCategory>();
                                for (int i = 0; i < info.cardInfo.blacklistedCategories.Length; i++)
                                {
                                    categories.Add(info.cardInfo.blacklistedCategories[i]);
                                }
                                categories.Add(CustomCardCategories.instance.CardCategory("WWC Gun Type"));
                                info.cardInfo.blacklistedCategories = categories.ToArray();
                            }
                            else
                            {
                                info.cardInfo.categories = new CardCategory[] { CustomCardCategories.instance.CardCategory("GunType") };
                                info.cardInfo.blacklistedCategories = new CardCategory[] { CustomCardCategories.instance.CardCategory("WWC Gun Type") };
                            }
                            UnityEngine.Debug.Log("[WWC] Modified Fireworks");
                            break;
                        }
                    case "SPLITTING ROUNDS":
                        {
                            UnityEngine.Debug.Log("[WWC] Found Splitting Rounds");
                            if (info.cardInfo.categories != null)
                            {
                                categories = new List<CardCategory>();
                                for (int i = 0; i < info.cardInfo.categories.Length; i++)
                                {
                                    categories.Add(info.cardInfo.categories[i]);
                                }
                                categories.Add(CustomCardCategories.instance.CardCategory("No Minigun"));
                                info.cardInfo.categories = categories.ToArray();
                                categories = new List<CardCategory>();
                                for (int i = 0; i < info.cardInfo.blacklistedCategories.Length; i++)
                                {
                                    categories.Add(info.cardInfo.blacklistedCategories[i]);
                                }
                                categories.Add(CustomCardCategories.instance.CardCategory("No Minigun"));
                                info.cardInfo.blacklistedCategories = categories.ToArray();
                            }
                            else
                            {
                                info.cardInfo.categories = new CardCategory[] { CustomCardCategories.instance.CardCategory("No Minigun") };
                                info.cardInfo.blacklistedCategories = new CardCategory[] { CustomCardCategories.instance.CardCategory("No Minigun") };
                            }
                            UnityEngine.Debug.Log("[WWC] Modified Splitting Rounds");
                            break;
                        }
                    case "EMPOWER":
                        {
                            UnityEngine.Debug.Log("[WWC] Found Empower");
                            if (info.cardInfo.categories != null)
                            {
                                categories = new List<CardCategory>();
                                for (int i = 0; i < info.cardInfo.categories.Length; i++)
                                {
                                    categories.Add(info.cardInfo.categories[i]);
                                }
                                categories.Add(CustomCardCategories.instance.CardCategory("GunType"));
                                info.cardInfo.categories = categories.ToArray();
                                categories = new List<CardCategory>();
                                for (int i = 0; i < info.cardInfo.blacklistedCategories.Length; i++)
                                {
                                    categories.Add(info.cardInfo.blacklistedCategories[i]);
                                }
                                categories.Add(CustomCardCategories.instance.CardCategory("WWC Gun Type"));
                                info.cardInfo.blacklistedCategories = categories.ToArray();
                            }
                            else
                            {
                                info.cardInfo.categories = new CardCategory[] { CustomCardCategories.instance.CardCategory("GunType") };
                                info.cardInfo.blacklistedCategories = new CardCategory[] { CustomCardCategories.instance.CardCategory("WWC Gun Type") };
                            }
                            UnityEngine.Debug.Log("[WWC] Modified Empower");
                            break;
                        }
                }

            }
            UnityEngine.Debug.Log("[WWC] Cards Modified");
        }
        public class CardRemover : MonoBehaviour
        {
            public void DelayedRemoveCard(Player player, string cardName, int frames = 10)
            {
                StartCoroutine(RemoveCard(player, cardName, frames));
            }

            IEnumerator RemoveCard(Player player, string cardName, int frames = 10)
            {
                yield return StartCoroutine(WaitFor.Frames(frames));

                for (int i = player.data.currentCards.Count - 1; i >= 0; i--)
                {
                    if (player.data.currentCards[i].cardName == cardName)
                    {
                        ModdingUtils.Utils.Cards.instance.RemoveCardFromPlayer(player, i);
                        break;
                    }
                }
            }
        }
    }
}
