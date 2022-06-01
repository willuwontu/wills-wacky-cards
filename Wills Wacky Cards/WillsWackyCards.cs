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
using Jotunn.Utils;
using UnboundLib.Networking;
using WWC.Cards;
using WWC.Cards.Curses;
using WWC.Cards.Testing;
using WWC.Interfaces;
using WWC.Extensions;
using WillsWackyManagers.Utils;
using WWC.MonoBehaviours;
using HarmonyLib;
using Photon.Pun;
using TMPro;
using CardChoiceSpawnUniqueCardPatch.CustomCategories;
using UnityEngine.UI;

namespace WWC
{
    [BepInDependency("com.willis.rounds.unbound", BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency("com.willuwontu.rounds.managers", BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency("pykess.rounds.plugins.moddingutils", BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency("pykess.rounds.plugins.cardchoicespawnuniquecardpatch", BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency("root.classes.manager.reborn", BepInDependency.DependencyFlags.HardDependency)]
    [BepInPlugin(ModId, ModName, Version)]
    [BepInProcess("Rounds.exe")]
    public class WillsWackyCards : BaseUnityPlugin
    {
        private const string ModId = "com.willuwontu.rounds.cards";
        private const string ModName = "Will's Wacky Cards";
        public const string Version = "1.9.10"; // What version are we on (major.minor.patch)?

        public const string ModInitials = "WWC";
        public const string CurseInitials = "Curse";
        public const string TestingInitials = "Testing";

        public static CardCategory TestCardCategory;

        public static WillsWackyCards instance { get; private set; }
        public static CardRemover remover;

        public static bool battleStarted = false;

        private bool debug = false;

        public static CardInfo.Rarity ScarceRarity
        {
            get
            {
                return RarityLib.Utils.RarityUtils.GetRarity("Scarce");
            }
        }

        public static CardInfo.Rarity EpicRarity
        {
            get
            {
                return RarityLib.Utils.RarityUtils.GetRarity("Epic");
            }
        }

        void Awake()
        {

        }
        void Start()
        {
            Unbound.RegisterCredits(ModName, new string[] { "willuwontu" }, new string[] { "github", "Ko-Fi" }, new string[] { "https://github.com/willuwontu/wills-wacky-cards", "https://ko-fi.com/willuwontu" });

            instance = this;

            var harmony = new Harmony(ModId);

            PluginInfo[] pluginInfos = BepInEx.Bootstrap.Chainloader.PluginInfos.Values.ToArray();
            foreach (PluginInfo info in pluginInfos)
            {
                Assembly mod = Assembly.LoadFile(info.Location);
                Type[] types = mod.GetTypes().Where(myType => myType.IsClass && !myType.IsAbstract && myType.IsSubclassOf(typeof(CustomCard))).ToArray();
                foreach (Type type in types)
                {
                    if (type.Name == "EggCard")
                    {
                        MethodInfo getRarity = type.GetMethod("GetRarity", BindingFlags.NonPublic | BindingFlags.Instance);
                        if (getRarity != null)
                        {
                            try
                            {
                                RarityLib.Utils.RarityUtils.AddRarity("E G G", 0.025f, new Color32(255, 243, 194, 255), new Color32(150, 140, 100, 255));
                            }
                            catch (Exception e)
                            {
                                UnityEngine.Debug.LogException(e);
                            }

                            HarmonyMethod eggRarity = new HarmonyMethod(typeof(WWC.Patches.Egg_Patch).GetMethod("EggRarity", BindingFlags.Static | BindingFlags.NonPublic));
                            harmony.Patch(getRarity, postfix: eggRarity);
                            if (eggRarity != null)
                            {
                                harmony.Patch(getRarity, postfix: eggRarity);
                            }
                        }
                    }
                }
            }

            harmony.PatchAll();

            remover = gameObject.AddComponent<CardRemover>();
            gameObject.AddComponent<InterfaceGameModeHooksManager>();

            CustomCard.BuildCard<AmmoCache>();
            CustomCard.BuildCard<Shotgun>();
            CustomCard.BuildCard<SlowDeath>();
            CustomCard.BuildCard<Vampirism>();
            CustomCard.BuildCard<BasicPhysics>();
            CustomCard.BuildCard<BasicAlternatePhysics>();
            //CustomCard.BuildCard<Minigun>();
            CustomCard.BuildCard<WildAim>();
            CustomCard.BuildCard<RunningShoes>();
            CustomCard.BuildCard<JumpingShoes>();
            CustomCard.BuildCard<Hex>();
            CustomCard.BuildCard<Gatling>();
            CustomCard.BuildCard<PlasmaRifle>();
            CustomCard.BuildCard<PlasmaShotgun>();
            CustomCard.BuildCard<UnstoppableForce>(cardInfo => { UnstoppableForce.card = cardInfo; });
            CustomCard.BuildCard<ImmovableObject>(cardInfo => { ImmovableObject.card = cardInfo; });
            CustomCard.BuildCard<HotPotato>();
            CustomCard.BuildCard<SavageWounds>();
            CustomCard.BuildCard<RitualisticSacrifice>();
            CustomCard.BuildCard<ForbiddenMagics>();
            CustomCard.BuildCard<PurifyingLight>();
            CustomCard.BuildCard<CursedKnowledge>();
            CustomCard.BuildCard<EnduranceTraining>();
            CustomCard.BuildCard<AdrenalineRush>();

            CustomCard.BuildCard<HolyWater>();
            CustomCard.BuildCard<CleansingRitual>();
            CustomCard.BuildCard<BulletPoweredJetpack>();

            CustomCard.BuildCard<Banishment>();
            CustomCard.BuildCard<Resolute>();
            CustomCard.BuildCard<DimensionalShuffle>();
            CustomCard.BuildCard<Boomerang>();
            CustomCard.BuildCard<FlySwatter>();
            CustomCard.BuildCard<AggressiveVenting>();
            CustomCard.BuildCard<WheelOfFortune>();

            {
                CustomCard.BuildCard<Antidote>();
                CustomCard.BuildCard<PoisonResistant>();
                CustomCard.BuildCard<StrongBody>();
                CustomCard.BuildCard<BurstingPoisons>();
            }

            { // Curses
                CustomCard.BuildCard<MomentaryConfusion>(cardInfo => { CurseManager.instance.RegisterCurse(cardInfo); });
                CustomCard.BuildCard<FumbledMags>(cardInfo => { CurseManager.instance.RegisterCurse(cardInfo); });
                CustomCard.BuildCard<ShakyBullets>(cardInfo => { CurseManager.instance.RegisterCurse(cardInfo); });
                CustomCard.BuildCard<Bleed>(cardInfo => { CurseManager.instance.RegisterCurse(cardInfo); });
                CustomCard.BuildCard<EasyTarget>(cardInfo => { CurseManager.instance.RegisterCurse(cardInfo); });
                CustomCard.BuildCard<WeakMind>(cardInfo => { CurseManager.instance.RegisterCurse(cardInfo); });
                CustomCard.BuildCard<PoisonousTrauma>(cardInfo => { CurseManager.instance.RegisterCurse(cardInfo); });
                //CustomCard.BuildCard<ErodingDarkness>(cardInfo => { CurseManager.instance.RegisterCurse(cardInfo); });
            }

            { // Curse Eater Class
                CustomCard.BuildCard<CurseEater>(cardInfo => { CurseEater.card = cardInfo; });
                CustomCard.BuildCard<GhostlyBody>(cardInfo => { GhostlyBody.card = cardInfo; });
                CustomCard.BuildCard<ShadowBullets>(cardInfo => { ShadowBullets.card = cardInfo; });
                CustomCard.BuildCard<SiphonCurses>(cardInfo => { SiphonCurses.card = cardInfo; });
                CustomCard.BuildCard<Flagellation>(cardInfo => { Flagellation.card = cardInfo; });
                CustomCard.BuildCard<RunicWards>(cardInfo => { RunicWards.card = cardInfo; });
                CustomCard.BuildCard<HiltlessBlade>(cardInfo => { HiltlessBlade.card = cardInfo; });
                CustomCard.BuildCard<CorruptedAmmunition>(cardInfo => { CorruptedAmmunition.card = cardInfo; });
            }

            { // Mechanic Class
                CustomCard.BuildCard<Mechanic>(cardInfo => { Mechanic.card = cardInfo; });
                CustomCard.BuildCard<ImprovedShieldCapacitors>(cardInfo => { ImprovedShieldCapacitors.card = cardInfo; });
                CustomCard.BuildCard<PortableFabricator>(cardInfo => { PortableFabricator.card = cardInfo; });
                CustomCard.BuildCard<CloningTanks>(cardInfo => { CloningTanks.card = cardInfo; });
                CustomCard.BuildCard<ChemicalAmmunition>(cardInfo => { ChemicalAmmunition.card = cardInfo; });
                CustomCard.BuildCard<CuttingLaser>(cardInfo => { CuttingLaser.card = cardInfo; });
                CustomCard.BuildCard<GreyGoo>(cardInfo => { GreyGoo.card = cardInfo; });
                CustomCard.BuildCard<GyroscopicStabilizers>(cardInfo => { GyroscopicStabilizers.card = cardInfo; });
                CustomCard.BuildCard<ImpactDissipators>(cardInfo => { ImpactDissipators.card = cardInfo; });
                CustomCard.BuildCard<ImprovedCycling>(cardInfo => { ImprovedCycling.card = cardInfo; });
                CustomCard.BuildCard<IntegratedTargeting>(cardInfo => { IntegratedTargeting.card = cardInfo; });
                CustomCard.BuildCard<JumpBoots>(cardInfo => { JumpBoots.card = cardInfo; });
                CustomCard.BuildCard<Omnitool>(cardInfo => { Omnitool.card = cardInfo; });
                CustomCard.BuildCard<ParticleWaveSequencer>(cardInfo => { ParticleWaveSequencer.card = cardInfo; });
                CustomCard.BuildCard<PersonalHammerspace>(cardInfo => { PersonalHammerspace.card = cardInfo; });
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

            //var networkEvents = gameObject.AddComponent<NetworkEventCallbacks>();
            //networkEvents.OnJoinedRoomEvent += OnJoinedRoomAction;
            //networkEvents.OnLeftRoomEvent += OnLeftRoomAction;
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

        public static void AddCardToPlayer(Player player, CardInfo card)
        {
            if (player is null || card is null)
            {
                return;
            }

            string cardID = card.gameObject.name;
            int playerID = player.playerID;

            NetworkingManager.RPC(typeof(WillsWackyCards), nameof(WillsWackyCards.URPCA_AddCardToPlayer), new object[] { playerID, cardID });
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
            while (stacks <= 20)
            {
                MomentumTracker.stacks = stacks;
                buildingCard = true;
                CustomCard.BuildCard<BuildImmovableObject>(cardInfo => { MomentumTracker.createdDefenseCards.Add(stacks, cardInfo); ModdingUtils.Utils.Cards.instance.AddHiddenCard(cardInfo); });
                CustomCard.BuildCard<BuildUnstoppableForce>(cardInfo => { MomentumTracker.createdOffenseCards.Add(stacks, cardInfo); ModdingUtils.Utils.Cards.instance.AddHiddenCard(cardInfo); buildingCard = false; });
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
            foreach (var player in PlayerManager.instance.players)
            {
                if (!ModdingUtils.Extensions.CharacterStatModifiersExtension.GetAdditionalData(player.data.stats).blacklistedCategories.Contains(SiphonCurses.siphonCard))
                {
                    ModdingUtils.Extensions.CharacterStatModifiersExtension.GetAdditionalData(player.data.stats).blacklistedCategories.Add(SiphonCurses.siphonCard);
                }
                int otherCurses = 0;

                foreach (var person in PlayerManager.instance.players.Where((pl) => pl.playerID != player.playerID).ToArray())
                {
                    var curses = CurseManager.instance.GetAllCursesOnPlayer(person);

                    otherCurses += curses.Count();

                    if (otherCurses > 2)
                    {
                        break;
                    }
                }

                if (otherCurses > 2)
                {
                    ModdingUtils.Extensions.CharacterStatModifiersExtension.GetAdditionalData(player.data.stats).blacklistedCategories.RemoveAll(category => category == SiphonCurses.siphonCard);
                    WillsWackyCards.instance.DebugLog($"[{WillsWackyCards.ModInitials}] Player {player.playerID} can siphon cards");
                }
            }
            yield break;
        }

        IEnumerator PlayerPickEnd(IGameModeHandler gm)
        {
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
