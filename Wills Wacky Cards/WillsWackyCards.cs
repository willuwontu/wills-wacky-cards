﻿using System;
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
using Jotunn.Utils;
using UnboundLib.Networking;
using WWC.Cards;
using WWC.Cards.Curses;
using WWC.Cards.Testing;
using WWC.Interfaces;
using WWC.Extensions;
using WWC.UnityTools;
using WillsWackyManagers.Utils;
using WillsWackyManagers.UnityTools;
using WWC.MonoBehaviours;
using HarmonyLib;
using Photon.Pun;
using TMPro;
using CardChoiceSpawnUniqueCardPatch.CustomCategories;
using UnityEngine.UI;
using Nullmanager;
using InControl;

namespace WWC
{
    [BepInDependency("com.willis.rounds.unbound")]
    [BepInDependency("com.willuwontu.rounds.managers")]
    [BepInDependency("pykess.rounds.plugins.moddingutils")]
    [BepInDependency("pykess.rounds.plugins.cardchoicespawnuniquecardpatch")]
    [BepInDependency("root.classes.manager.reborn")]
    [BepInDependency("root.rarity.lib")]
    [BepInDependency("root.cardtheme.lib")]
    [BepInDependency("com.rounds.willuwontu.gunchargepatch")]
    [BepInDependency("com.rounds.willuwontu.ActionHelper")]
    [BepInDependency("pykess.rounds.plugins.pickncards")]
    [BepInDependency("com.root.player.time")]
    [BepInDependency("com.Root.Null")]
    [BepInPlugin(ModId, ModName, Version)]
    [BepInProcess("Rounds.exe")]
    public class WillsWackyCards : BaseUnityPlugin
    {
        private const string ModId = "com.willuwontu.rounds.cards";
        private const string ModName = "Will's Wacky Cards";
        public const string Version = "1.11.23"; // What version are we on (major.minor.patch)?

        public const string ModInitials = "WWC";
        public const string CurseInitials = "Curse";
        public const string TestingInitials = "Testing";

        public static CardCategory TestCardCategory;

        public static WillsWackyCards instance { get; private set; }
        public static CardRemover remover;

        public AssetBundle WWCAssets { get; private set; }

        public static bool battleStarted = false;

        private bool debug = false;

        void Awake()
        {
            instance = this;

            WWCAssets = AssetUtils.LoadAssetBundleFromResources("wwcstuff", typeof(WillsWackyCards).Assembly);

            var harmony = new Harmony(ModId);
            harmony.PatchAll();

            //UnityEngine.Debug.Log("WWC Awake");

            Unbound.RegisterCredits(ModName, new string[] { "willuwontu" }, new string[] { "github", "Ko-Fi" }, new string[] { "https://github.com/willuwontu/wills-wacky-cards", "https://ko-fi.com/willuwontu" });

            remover = gameObject.AddComponent<CardRemover>();
            gameObject.AddComponent<InterfaceGameModeHooksManager>();

            //PlayerActionsHelper.PlayerActionManager.RegisterPlayerAction(new PlayerActionsHelper.ActionInfo("ToggleFlight", new KeyBindingSource(Key.Key1), new DeviceBindingSource(InputControlType.Action3)));
        }
        void Start()
        {
            { // Build the cards
                Mechanic.cardBase = WillsWackyManagers.WillsWackyManagers.instance.WWMAssets.LoadAsset<GameObject>("MechanicCardBase");
                GameObject cardLoader = WWCAssets.LoadAsset<GameObject>("WWC CardManager");
                foreach (CardBuilder classManager in cardLoader.GetComponentsInChildren<CardBuilder>())
                {
                    classManager.BuildCards();
                }
                //foreach (WillsWackyManagers.UnityTools.CardBuilder cardManager in cardLoader.GetComponentsInChildren<WillsWackyManagers.UnityTools.CardBuilder>())
                //{
                //    cardManager.BuildCards();
                //}
            }

            NullManager.instance.RegesterOnAddCallback(OnNullAdd);

            //try
            //{
            //    foreach (var device in InControl.InputManager.Devices)
            //    {
            //        UnityEngine.Debug.Log(device);
            //    }
            //    InControl.InputManager.OnDeviceAttached += (device) => UnityEngine.Debug.Log("Device Attached");
            //    InControl.InputManager.OnDeviceDetached += (device) => UnityEngine.Debug.Log("Device Removed");
            //}
            //catch
            //{

            //}
            //Disabled but nice to know that we can do this.

            //PluginInfo[] pluginInfos = BepInEx.Bootstrap.Chainloader.PluginInfos.Values.ToArray();
            //foreach (PluginInfo info in pluginInfos)
            //{
            //    Assembly mod = Assembly.LoadFile(info.Location);
            //    Type[] types = mod.GetTypes().Where(myType => myType.IsClass && !myType.IsAbstract && myType.IsSubclassOf(typeof(CustomCard))).ToArray();
            //    foreach (Type type in types)
            //    {
            //        if (type.Name == "EggCard")
            //        {
            //            MethodInfo getRarity = type.GetMethod("GetRarity", BindingFlags.NonPublic | BindingFlags.Instance);
            //            if (getRarity != null)
            //            {
            //                RarityLib.Utils.RarityUtils.AddRarity("E G G", 0.025f, new Color32(255, 243, 194, 255), new Color32(150, 140, 100, 255));

            //                HarmonyMethod eggRarity = new HarmonyMethod(typeof(WWC.Patches.Egg_Patch).GetMethod("EggRarity", BindingFlags.Static | BindingFlags.NonPublic));
            //                harmony.Patch(getRarity, postfix: eggRarity);
            //                if (eggRarity != null)
            //                {
            //                    harmony.Patch(getRarity, postfix: eggRarity);
            //                }
            //            }
            //        }
            //    }
            //}

            ////CustomCard.BuildCard<AmmoCache>();
            //CustomCard.BuildCard<Shotgun>();
            //CustomCard.BuildCard<SlowDeath>();
            //CustomCard.BuildCard<Vampirism>();
            //CustomCard.BuildCard<BasicPhysics>();
            //CustomCard.BuildCard<BasicAlternatePhysics>();
            ////CustomCard.BuildCard<Minigun>();
            //CustomCard.BuildCard<WildAim>();
            //CustomCard.BuildCard<RunningShoes>();
            //CustomCard.BuildCard<JumpingShoes>();
            //CustomCard.BuildCard<Hex>();
            //CustomCard.BuildCard<Gatling>();
            //CustomCard.BuildCard<PlasmaRifle>();
            //CustomCard.BuildCard<PlasmaShotgun>();
            //CustomCard.BuildCard<UnstoppableForce>(cardInfo => { UnstoppableForce.card = cardInfo; });
            //CustomCard.BuildCard<ImmovableObject>(cardInfo => { ImmovableObject.card = cardInfo; });
            //CustomCard.BuildCard<HotPotato>();
            //CustomCard.BuildCard<SavageWounds>();
            //CustomCard.BuildCard<RitualisticSacrifice>();
            //CustomCard.BuildCard<ForbiddenMagics>();
            //CustomCard.BuildCard<PurifyingLight>();
            //CustomCard.BuildCard<CursedKnowledge>();
            //CustomCard.BuildCard<EnduranceTraining>();
            //CustomCard.BuildCard<AdrenalineRush>();

            //CustomCard.BuildCard<HolyWater>();
            //CustomCard.BuildCard<CleansingRitual>();
            //CustomCard.BuildCard<BulletPoweredJetpack>();

            //CustomCard.BuildCard<Banishment>();
            //CustomCard.BuildCard<Resolute>();
            //CustomCard.BuildCard<DimensionalShuffle>();
            //CustomCard.BuildCard<Boomerang>();
            //CustomCard.BuildCard<FlySwatter>();
            //CustomCard.BuildCard<AggressiveVenting>();
            //CustomCard.BuildCard<WheelOfFortune>(card => { WheelOfFortune.card = card; });

            //{
            //    CustomCard.BuildCard<Antidote>();
            //    CustomCard.BuildCard<PoisonResistant>();
            //    CustomCard.BuildCard<StrongBody>();
            //    CustomCard.BuildCard<BurstingPoisons>();
            //}

            //{ // Curses
            //    CustomCard.BuildCard<MomentaryConfusion>(cardInfo => { CurseManager.instance.RegisterCurse(cardInfo); });
            //    CustomCard.BuildCard<FumbledMags>(cardInfo => { CurseManager.instance.RegisterCurse(cardInfo); });
            //    CustomCard.BuildCard<ShakyBullets>(cardInfo => { CurseManager.instance.RegisterCurse(cardInfo); });
            //    CustomCard.BuildCard<Bleed>(cardInfo => { CurseManager.instance.RegisterCurse(cardInfo); });
            //    CustomCard.BuildCard<EasyTarget>(cardInfo => { CurseManager.instance.RegisterCurse(cardInfo); });
            //    CustomCard.BuildCard<WeakMind>(cardInfo => { CurseManager.instance.RegisterCurse(cardInfo); });
            //    CustomCard.BuildCard<PoisonousTrauma>(cardInfo => { CurseManager.instance.RegisterCurse(cardInfo); });
            //    //CustomCard.BuildCard<ErodingDarkness>(cardInfo => { CurseManager.instance.RegisterCurse(cardInfo); });
            //}

            { // Curse Eater Class
                //CustomCard.BuildCard<CurseEater>(cardInfo => { CurseEater.card = cardInfo; });
                //CustomCard.BuildCard<GhostlyBody>(cardInfo => { GhostlyBody.card = cardInfo; });
                //CustomCard.BuildCard<ShadowBullets>(cardInfo => { ShadowBullets.card = cardInfo; });
                //CustomCard.BuildCard<SiphonCurses>(cardInfo => { SiphonCurses.card = cardInfo; });
                //CustomCard.BuildCard<Flagellation>(cardInfo => { Flagellation.card = cardInfo; });
                //CustomCard.BuildCard<RunicWards>(cardInfo => { RunicWards.card = cardInfo; });
                //CustomCard.BuildCard<HiltlessBlade>(cardInfo => { HiltlessBlade.card = cardInfo; });
                //CustomCard.BuildCard<CorruptedAmmunition>(cardInfo => { CorruptedAmmunition.card = cardInfo; });
            }

            if (debug)
            {
                // Build Testing Cards
                TestCardCategory = CustomCardCategories.instance.CardCategory("Testing Cards");

                CustomCard.BuildCard<RemoveAll>();
                CustomCard.BuildCard<RemoveTestingCards>();
                CustomCard.BuildCard<RemoveLast>();
                CustomCard.BuildCard<RemoveFirst>();

                CustomCard.BuildCard<DoRoundStart>();
                CustomCard.BuildCard<DoGameStart>();
                CustomCard.BuildCard<DoBattleStart>();
                CustomCard.BuildCard<DoPointStart>();
                CustomCard.BuildCard<DoPickStart>();
                CustomCard.BuildCard<DoPlayerPickStart>();
                CustomCard.BuildCard<DoRoundEnd>();
                CustomCard.BuildCard<DoGameEnd>();
                CustomCard.BuildCard<DoPointEnd>();
                CustomCard.BuildCard<DoPickEnd>();
                CustomCard.BuildCard<DoPlayerPickEnd>();
                CustomCard.BuildCard<DoInitStart>();
                CustomCard.BuildCard<DoInitEnd>();
                CustomCard.BuildCard<SimulateRoundStart>();
                CustomCard.BuildCard<SimulateRoundEnd>();
                CustomCard.BuildCard<SimulateRound>();
                CustomCard.BuildCard<SimulatePickPhase>();

                //CustomCard.BuildCard<AddPointToTeam>();
                //CustomCard.BuildCard<AddRoundToTeam>();
                //CustomCard.BuildCard<ResetTeamScores>(); 
            }

            this.ExecuteAfterSeconds(0.4f, () => { StartCoroutine(BuildMomentumCards()); });

            this.ExecuteAfterSeconds(0.4f, ChangeCards);

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

            ModdingUtils.Utils.Cards.instance.AddCardValidationFunction(OnlyUnstoppableOrImmovable);

            //var networkEvents = gameObject.AddComponent<NetworkEventCallbacks>();
            //networkEvents.OnJoinedRoomEvent += OnJoinedRoomAction;
            //networkEvents.OnLeftRoomEvent += OnLeftRoomAction;

            //CardInfo ensnareCard = UnityEngine.Resources.Load<GameObject>("0 cards_needsnetworking/Ensnare").GetComponent<CardInfo>();
            //CardInfo remnantCard = UnityEngine.Resources.Load<GameObject>("0 cards_needsnetworking/Remnant").GetComponent<CardInfo>();
            //CardInfo blackHoleCard = UnityEngine.Resources.Load<GameObject>("1 cards_boring/BlackHole").GetComponent<CardInfo>();

            //ObservableCollection<CardInfo> activeCards = (ObservableCollection<CardInfo>)typeof(CardManager).GetField("activeCards", BindingFlags.Default | BindingFlags.Static | BindingFlags.NonPublic).GetValue(null);

            //List<CardInfo> hiddenVanillaCards = new List<CardInfo>() { ensnareCard, remnantCard, blackHoleCard };

            //CardInfo[] defaultCards = ((CardInfo[])typeof(CardManager).GetField("defaultCards", BindingFlags.Default | BindingFlags.Static | BindingFlags.NonPublic).GetValue(null)).Concat(hiddenVanillaCards).ToArray();
            //typeof(CardManager).GetField("defaultCards", BindingFlags.Default | BindingFlags.Static | BindingFlags.NonPublic).SetValue(null, defaultCards);

            //foreach (var card in hiddenVanillaCards)
            //{
            //    CardManager.cards.Add(card.gameObject.name, new Card("Vanilla", Unbound.config.Bind("Cards: " + "Vanilla", card.gameObject.name, true), card));
            //    Photon.Pun.PhotonNetwork.PrefabPool.RegisterPrefab(card.gameObject.name, card.gameObject);
            //    activeCards.Add(card);
            //}
        }

        public CardThemeColor.CardThemeColorType PumpkinOrange => CardThemeLib.CardThemeLib.instance.CreateOrGetType("Pumpkin Orange", new CardThemeColor() { bgColor = new Color32(229, 127, 0, 200), targetColor = new Color32(229 * 3/4, 127 * 3 / 4, 0, 200) });
        public CardThemeColor.CardThemeColorType FieryOrange => CardThemeLib.CardThemeLib.instance.CreateOrGetType("Fiery Orange", new CardThemeColor() { bgColor = new Color32(229, 74, 0, 200), targetColor = new Color32(229 * 3 / 4, 74 * 2 / 4, 0, 200) });

        #region NullCardHandling

        private void OnNullAdd(NullCardInfo card, Player player)
        {
            UnityEngine.Debug.Log($"Null Added, {player.GetNullCount()} null cards");

            CharacterStatModifiers stats = player.data.stats;
            var nullData = stats.GetAdditionalData().nullData;
            int nullcount = player.GetNullCount();

            if (nullData.damageRedCards > 0)
            {
                stats.GetAdditionalData().DamageReduction += ((0.5f * Mathf.Log10(nullData.damageRedCards * nullcount + 1)) - (0.5f * Mathf.Log10(nullData.damageRedCards * (nullcount -1) + 1)));
            }
            
            stats.GetAdditionalData().poisonResistance *= nullData.poisonResMult;
            stats.GetAdditionalData().willpower *= nullData.willPowerMult;

            UpdateNullStatsForPlayer(player);
        }

        public static void UpdateNullStatsForPlayer(Player player)
        {
            var nullData = player.data.stats.GetAdditionalData().nullData;
            int nullcount = player.GetNullCount();
            List<CardInfoStat> stats = new List<CardInfoStat>();

            if (nullData.damageRedCards > 0)
            {
                stats.Add(new CardInfoStat()
                {
                    positive = true,
                    stat = "Damage Reduction",
                    amount = $"+{(int)(((0.5f * Mathf.Log10(nullData.damageRedCards * (nullcount + 1) + 1)) - (0.5f * Mathf.Log10(nullData.damageRedCards * nullcount + 1))) * 100)}%",
                    simepleAmount = CardInfoStat.SimpleAmount.notAssigned
                });
            }
            if (nullData.poisonResMult != 1f)
            {
                stats.Add(new CardInfoStat()
                {
                    positive = true,
                    stat = "Poison Resistance",
                    amount = $"+{(int)((1f - nullData.poisonResMult) * 100)}%",
                    simepleAmount = CardInfoStat.SimpleAmount.notAssigned
                });
            }
            if (nullData.willPowerMult != 1f)
            {
                stats.Add(new CardInfoStat()
                {
                    positive = true,
                    stat = "Willpower",
                    amount = $"+{(int)((nullData.willPowerMult - 1f) * 100)}%",
                    simepleAmount = CardInfoStat.SimpleAmount.notAssigned
                });
            }

            NullManager.instance.SetAdditionalNullStats(player, "WWC", stats.ToArray());
        }

        #endregion

        private bool OnlyUnstoppableOrImmovable(Player player, CardInfo card)
        {
            if ((card != ImmovableObject.card) || (card != UnstoppableForce.card))
            {
                return true;
            }

            if (((List<GameObject>)CardChoice.instance.GetFieldValue("spawnedCards")).Select(obj => obj.GetComponent<CardInfo>()).Any(c => (c == ImmovableObject.card) || (c == UnstoppableForce.card)))
            {
                return false;
            }

            return true;
        }

        private void OnJoinedRoomAction()
        {

        }

        private void OnLeftRoomAction()
        {

        }

        public void DebugLog(object message)
        {
            if (debug)
            {
                UnityEngine.Debug.Log(message);
            }
        }

        public static void SendDebugLog(object message, bool masterClientOnly = false)
        {
            if (!(masterClientOnly && !(PhotonNetwork.IsMasterClient || PhotonNetwork.OfflineMode)))
            {
                return;
            }
            NetworkingManager.RPC(typeof(WillsWackyCards), nameof(WillsWackyCards.URPCA_DebugLog), message);
        }

        [UnboundRPC]
        public static void URPCA_DebugLog(object message)
        {
            WillsWackyCards.instance.DebugLog(message);
        }

        public static void AddCardToPlayer(Player player, CardInfo card)
        {
            if (player is null || card is null)
            {
                return;
            }

            string cardID = card.gameObject.name;
            int playerID = player.playerID;

            NetworkingManager.RPC(typeof(WillsWackyCards), nameof(WillsWackyCards.URPCA_AddCardToPlayer), playerID, cardID);
        }

        [UnboundRPC]
        public static void URPCA_AddCardToPlayer(int playerId, string cardName)
        {
            var player = PlayerManager.instance.GetPlayerWithID(playerId);
            CardInfo card = CardManager.cards.Values.Select(c => c.cardInfo).Where(c => c.gameObject.name == cardName).First();

            if (card)
            {
                ModdingUtils.Utils.Cards.instance.AddCardToPlayer(player, card, false, "", 0, 0, true);
            }
        }

        public void TriggerGameModeHook(string key)
        {
            StartCoroutine(ITriggerGameModeHook(key));
        }

        private IEnumerator ITriggerGameModeHook(string key)
        {
            yield return GameModeManager.TriggerHook(key);
            yield break;
        }

        IEnumerator BuildMomentumCards()
        {
            var stacks = 0;
            var buildingCard = false;
            yield return StartCoroutine(WaitFor.Frames(7));
            while (stacks <= MomentumTracker.maxStacks)
            {
                MomentumTracker.stacks = stacks;
                buildingCard = true;
                CustomCard.BuildCard<BuildImmovableObject>(cardInfo => 
                { 
                    MomentumTracker.createdDefenseCards.Add(stacks, cardInfo); 
                    ModdingUtils.Utils.Cards.instance.AddHiddenCard(cardInfo);
                    cardInfo.cardName = ImmovableObject.card.cardName;
                    //cardInfo.gameObject.AddComponent<MomentumPhoton>(); 
                });
                CustomCard.BuildCard<BuildUnstoppableForce>(cardInfo => 
                { 
                    MomentumTracker.createdOffenseCards.Add(stacks, cardInfo); 
                    ModdingUtils.Utils.Cards.instance.AddHiddenCard(cardInfo); 
                    //cardInfo.gameObject.AddComponent<MomentumPhoton>();
                    cardInfo.cardName = UnstoppableForce.card.cardName;
                    buildingCard = false; 
                });
                yield return new WaitUntil(() => !buildingCard);
                yield return WaitFor.Frames(2);
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
            yield break;
        }

        IEnumerator RoundEnd(IGameModeHandler gm)
        {
            yield break;
        }

        IEnumerator PointStart(IGameModeHandler gm)
        {
            yield break;
        }

        IEnumerator PointEnd(IGameModeHandler gm)
        {
            battleStarted = false;
            yield break;
        }

        IEnumerator PlayerPickStart(IGameModeHandler gm)
        {
            RarityLib.Utils.RarityUtils.AjustCardRarityModifier(WWC.Cards.ImmovableObject.card, 0.25f, 0f);
            RarityLib.Utils.RarityUtils.AjustCardRarityModifier(WWC.Cards.UnstoppableForce.card, 0.25f, 0f);
            MomentumTracker.rarityBuff += 0.25f;

            yield break;
        }

        IEnumerator PlayerPickEnd(IGameModeHandler gm)
        {
            MomentumTracker.offenseFlag = false;
            MomentumTracker.defenseFlag = false;
            yield break;
        }

        IEnumerator PickStart(IGameModeHandler gm)
        {
            yield break;
        }

        IEnumerator PickEnd(IGameModeHandler gm)
        {
            yield break;
        }

        IEnumerator BattleStart(IGameModeHandler gm)
        {
            battleStarted = true;
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
            }
            yield break;
        }
        IEnumerator GameStart(IGameModeHandler gm)
        {
            MomentumTracker.stacks = 0;
            MomentumTracker.rarityBuff = 0;

            MissedOpportunities.cardsSeen = new Dictionary<int, List<CardInfo>>();
            //AlteringTheDeal.cardsTaken = new Dictionary<Player, int>();

            foreach (var player in PlayerManager.instance.players)
            {
                if (!ModdingUtils.Extensions.CharacterStatModifiersExtension.GetAdditionalData(player.data.stats).blacklistedCategories.Contains(Minigun.componentCategory))
                {
                    ModdingUtils.Extensions.CharacterStatModifiersExtension.GetAdditionalData(player.data.stats).blacklistedCategories.Add(Minigun.componentCategory);
                }
                if (!ModdingUtils.Extensions.CharacterStatModifiersExtension.GetAdditionalData(player.data.stats).blacklistedCategories.Contains(TestCardCategory))
                {
                    ModdingUtils.Extensions.CharacterStatModifiersExtension.GetAdditionalData(player.data.stats).blacklistedCategories.Add(TestCardCategory);
                }
            }

            yield break;
        }

        IEnumerator GameEnd(IGameModeHandler gm)
        {
            DestroyAll<Minigun_Mono>();
            DestroyAll<Vampirism_Mono>();
            DestroyAll<Gatling_Mono>();
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
            WillsWackyCards.instance.DebugLog($"[{WillsWackyCards.ModInitials}] Modifying Cards");
            List<CardCategory> categories;
            //CardInfo cardtest = CardManager.cards.Values.First(card => card.cardInfo.name.ToUpper() == "PONG").cardInfo;

            foreach (var info in CardManager.cards.Values.ToArray())
            {
                switch (info.cardInfo.cardName.ToUpper())
                {
                    case "FLAMETHROWER":
                        {
                            WillsWackyCards.instance.DebugLog($"[{WillsWackyCards.ModInitials}] Found Flamethrower");
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
                            WillsWackyCards.instance.DebugLog($"[{WillsWackyCards.ModInitials}] Modified Flamethrower");
                            break;
                        }
                    case "FIRE HYDRANT":
                        {
                            WillsWackyCards.instance.DebugLog($"[{WillsWackyCards.ModInitials}] Found Fire Hydrant");
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
                            WillsWackyCards.instance.DebugLog($"[{WillsWackyCards.ModInitials}] Modified Fire Hydrant");
                            break;
                        }
                    case "PONG":
                        {
                            WillsWackyCards.instance.DebugLog($"[{WillsWackyCards.ModInitials}] Found Pong");
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
                            WillsWackyCards.instance.DebugLog($"[{WillsWackyCards.ModInitials}] Modified Pong");
                            break;
                        }
                    case "COMB":
                        {
                            WillsWackyCards.instance.DebugLog($"[{WillsWackyCards.ModInitials}] Found Comb");
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
                            WillsWackyCards.instance.DebugLog($"[{WillsWackyCards.ModInitials}] Modified Comb");
                            break;
                        }
                    case "STAR":
                        {
                            WillsWackyCards.instance.DebugLog($"[{WillsWackyCards.ModInitials}] Found Star");
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
                            WillsWackyCards.instance.DebugLog($"[{WillsWackyCards.ModInitials}] Modified Star");
                            break;
                        }
                    case "HAWK":
                        {
                            WillsWackyCards.instance.DebugLog($"[{WillsWackyCards.ModInitials}] Found Hawk");
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
                            WillsWackyCards.instance.DebugLog($"[{WillsWackyCards.ModInitials}] Modified Hawk");
                            break;
                        }
                    case "ROLLING THUNDER":
                        {
                            WillsWackyCards.instance.DebugLog($"[{WillsWackyCards.ModInitials}] Found Rolling Thunder");
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
                            WillsWackyCards.instance.DebugLog($"[{WillsWackyCards.ModInitials}] Modified Rolling Thunder");
                            break;
                        }
                    case "LASER":
                        {
                            WillsWackyCards.instance.DebugLog($"[{WillsWackyCards.ModInitials}] Found Laser");
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
                            WillsWackyCards.instance.DebugLog($"[{WillsWackyCards.ModInitials}] Modified Laserr");
                            break;
                        }
                    case "FRAGMENTATION":
                        {
                            WillsWackyCards.instance.DebugLog($"[{WillsWackyCards.ModInitials}] Found Fragmentation");
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
                            WillsWackyCards.instance.DebugLog($"[{WillsWackyCards.ModInitials}] Modified Fragmentation");
                            break;
                        }
                    case "FIREWORKS":
                        {
                            WillsWackyCards.instance.DebugLog($"[{WillsWackyCards.ModInitials}] Found Fireworks");
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
                            WillsWackyCards.instance.DebugLog($"[{WillsWackyCards.ModInitials}] Modified Fireworks");
                            break;
                        }
                    case "SPLITTING ROUNDS":
                        {
                            WillsWackyCards.instance.DebugLog($"[{WillsWackyCards.ModInitials}] Found Splitting Rounds");
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
                            WillsWackyCards.instance.DebugLog($"[{WillsWackyCards.ModInitials}] Modified Splitting Rounds");
                            break;
                        }
                    case "EMPOWER":
                        {
                            WillsWackyCards.instance.DebugLog($"[{WillsWackyCards.ModInitials}] Found Empower");
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
                            WillsWackyCards.instance.DebugLog($"[{WillsWackyCards.ModInitials}] Modified Empower");
                            break;
                        }
                }

            }
            WillsWackyCards.instance.DebugLog($"[{WillsWackyCards.ModInitials}] Cards Modified");
        }
        public class CardRemover : MonoBehaviour
        {
            public void DelayedRemoveCard(Player player, string cardName, int frames = 10, bool silent = false)
            {
                StartCoroutine(RemoveCard(player, cardName, frames, silent));
            }

            IEnumerator RemoveCard(Player player, string cardName, int frames = 10, bool silent = false)
            {
                yield return StartCoroutine(WaitFor.Frames(frames));

                for (int i = player.data.currentCards.Count - 1; i >= 0; i--)
                {
                    if (player.data.currentCards[i].cardName == cardName)
                    {
                        if (silent)
                        {
                            NetworkingManager.RPC(typeof(CardRemover), nameof(SilentRemove), player.playerID, i);
                            break;
                        }
                        else
                        {
                            ModdingUtils.Utils.Cards.instance.RemoveCardFromPlayer(player, i);
                            break;
                        }
                    }
                }
            }
            [UnboundRPC]
            public static void SilentRemove(int playerId, int cardId)
            {
                Player player = PlayerManager.instance.players.Find(p => p.playerID == playerId);
                CardInfo removed = player.data.currentCards[cardId];
                player.data.currentCards.RemoveAt(cardId);
                CardBar cardbar = ModdingUtils.Utils.CardBarUtils.instance.PlayersCardBar(player);
                for (int num = cardbar.transform.childCount - 1; num >= 0; num--)
                {
                    if (((CardInfo)cardbar.transform.GetChild(num).gameObject.GetComponent<CardBarButton>().GetFieldValue("card")) == removed)
                    {
                        Destroy(cardbar.transform.GetChild(num).gameObject);
                        break;
                    }
                }
            }
        }
    }
}
