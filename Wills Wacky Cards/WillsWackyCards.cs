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
using WWC.Cards;
using WWC.Cards.Curses;
using WWC.Cards.Testing;
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
    [BepInPlugin(ModId, ModName, Version)]
    [BepInProcess("Rounds.exe")]
    public class WillsWackyCards : BaseUnityPlugin
    {
        private const string ModId = "com.willuwontu.rounds.cards";
        private const string ModName = "Will's Wacky Cards";
        public const string Version = "1.6.1"; // What version are we on (major.minor.patch)?

        public const string ModInitials = "WWC";
        public const string CurseInitials = "Curse";
        public const string TestingInitials = "Testing";

        public static CardCategory TestCardCategory;

        public static WillsWackyCards instance { get; private set; }
        public static CardRemover remover;

        public static bool battleStarted = false;

        GameObject ColorTester;

        private bool debug = false;

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

            gameObject.AddComponent<HookedMonoManager>();
            remover = gameObject.AddComponent<CardRemover>();

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
            //CustomCard.BuildCard<ErodingDarkness>(cardInfo => { CurseManager.instance.RegisterCurse(cardInfo); });
            CustomCard.BuildCard<DrivenToEarth>(cardInfo => { CurseManager.instance.RegisterCurse(cardInfo); });
            CustomCard.BuildCard<Misfire>(cardInfo => { CurseManager.instance.RegisterCurse(cardInfo); });
            CustomCard.BuildCard<UncomfortableDefense>(cardInfo => { CurseManager.instance.RegisterCurse(cardInfo); });
            CustomCard.BuildCard<CounterfeitAmmo>(cardInfo => { CurseManager.instance.RegisterCurse(cardInfo); });
            CustomCard.BuildCard<NeedleBullets>(cardInfo => { CurseManager.instance.RegisterCurse(cardInfo); });
            CustomCard.BuildCard<EasyTarget>(cardInfo => { CurseManager.instance.RegisterCurse(cardInfo); });
            CustomCard.BuildCard<WildShots>(cardInfo => { CurseManager.instance.RegisterCurse(cardInfo); });
            CustomCard.BuildCard<ShakyBullets>(cardInfo => { CurseManager.instance.RegisterCurse(cardInfo); });
            CustomCard.BuildCard<RabbitsFoot>(cardInfo => { CurseManager.instance.RegisterCurse(cardInfo); });
            CustomCard.BuildCard<LuckyClover>(cardInfo => { CurseManager.instance.RegisterCurse(cardInfo); });
            CustomCard.BuildCard<DefectiveTrigger>(cardInfo => { CurseManager.instance.RegisterCurse(cardInfo); });
            CustomCard.BuildCard<MisalignedSights>(cardInfo => { CurseManager.instance.RegisterCurse(cardInfo); });
            CustomCard.BuildCard<Damnation>(cardInfo => { CurseManager.instance.RegisterCurse(cardInfo); });
            CustomCard.BuildCard<FragileBody>(cardInfo => { CurseManager.instance.RegisterCurse(cardInfo); });
            CustomCard.BuildCard<AmmoRegulations>(cardInfo => { CurseManager.instance.RegisterCurse(cardInfo); });
            CustomCard.BuildCard<AirResistance>(cardInfo => { CurseManager.instance.RegisterCurse(cardInfo); });
            CustomCard.BuildCard<LeadBullets>(cardInfo => { CurseManager.instance.RegisterCurse(cardInfo); });
            CustomCard.BuildCard<AnimePhysics>(cardInfo => { CurseManager.instance.RegisterCurse(cardInfo); });
            CustomCard.BuildCard<TakeANumber>(cardInfo => { CurseManager.instance.RegisterCurse(cardInfo); });
            CustomCard.BuildCard<HeavyShields>(cardInfo => { CurseManager.instance.RegisterCurse(cardInfo); });
            CustomCard.BuildCard<Hex>();
            CustomCard.BuildCard<Gatling>();
            CustomCard.BuildCard<PlasmaRifle>();
            CustomCard.BuildCard<PlasmaShotgun>();
            CustomCard.BuildCard<UnstoppableForce>();
            CustomCard.BuildCard<ImmovableObject>();
            CustomCard.BuildCard<HotPotato>();
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
            CustomCard.BuildCard<CorruptedAmmunition>();
            CustomCard.BuildCard<HolyWater>();
            CustomCard.BuildCard<CleansingRitual>();
            CustomCard.BuildCard<BulletPoweredJetpack>();
            CustomCard.BuildCard<CurseEater>();
            CustomCard.BuildCard<GhostlyBody>();
            CustomCard.BuildCard<ShadowBullets>();
            CustomCard.BuildCard<SiphonCurses>();
            CustomCard.BuildCard<Flagellation>();
            CustomCard.BuildCard<Banishment>();
            CustomCard.BuildCard<Resolute>();
            CustomCard.BuildCard<DimensionalShuffle>();
            CustomCard.BuildCard<Boomerang>();

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

            //ColorTester = CreateColorTester();
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
            foreach (var hookedMono in HookedMonoManager.instance.hookedMonos)
            {
                //UnityEngine.Debug.Log($"[{ModInitials}][Debugging] Running OnPointStart");
                hookedMono.OnPointStart();
            }
            yield break;
        }

        IEnumerator PointEnd(IGameModeHandler gm)
        {
            battleStarted = false;
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
                if (!ModdingUtils.Extensions.CharacterStatModifiersExtension.GetAdditionalData(player.data.stats).blacklistedCategories.Contains(CurseEater.CurseEaterClass))
                {
                    ModdingUtils.Extensions.CharacterStatModifiersExtension.GetAdditionalData(player.data.stats).blacklistedCategories.Add(CurseEater.CurseEaterClass);
                }
                if (!ModdingUtils.Extensions.CharacterStatModifiersExtension.GetAdditionalData(player.data.stats).blacklistedCategories.Contains(TestCardCategory))
                {
                    ModdingUtils.Extensions.CharacterStatModifiersExtension.GetAdditionalData(player.data.stats).blacklistedCategories.Add(TestCardCategory);
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

        private GameObject CreateColorTester()
        {
            // The Base Game Object
            var cTest = new GameObject();
            cTest.name = "Color Tester Canvas";
            DontDestroyOnLoad(cTest);
            var canvas = cTest.AddComponent<Canvas>();
            var scaler = cTest.AddComponent<CanvasScaler>();
            var caster = cTest.AddComponent<GraphicRaycaster>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.additionalShaderChannels = AdditionalCanvasShaderChannels.None;
            canvas.sortingOrder = 1;
            var camera = cTest.AddComponent<Camera>();
            canvas.worldCamera = camera;
            //canvas.renderMode = RenderMode.ScreenSpaceCamera;
            camera.enabled = false;
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            scaler.matchWidthOrHeight = 0.5f;

            // The background
            var background = new GameObject();
            {
                background.transform.parent = cTest.transform;
                background.name = "Background";
                var backImage = background.AddComponent<Image>();
                backImage.color = new Color(0.5f, 0.5f, 0.5f, 1f);
                var rect = background.GetOrAddComponent<RectTransform>();
                rect.anchorMin = new Vector2(0, 0);
                rect.anchorMax = new Vector2(1, 1);
                rect.offsetMin = new Vector2(20f, 20f);
                rect.offsetMax = new Vector2(-20f, -20f);
            }

            // The scroll area, so we can see all the colors
            var scrollView = new GameObject();
            var viewport = new GameObject();
            var content = new GameObject();
            {
                scrollView.name = "Scroll View";
                scrollView.transform.parent = background.transform;
                var rect = scrollView.GetOrAddComponent<RectTransform>();
                rect.anchorMin = new Vector2(0, 0);
                rect.anchorMax = new Vector2(1, 1);
                rect.offsetMin = Vector2.zero;
                rect.offsetMax = Vector2.zero;
                var scrollScroll = scrollView.GetOrAddComponent<ScrollRect>();
                scrollScroll.inertia = false;
                scrollScroll.movementType = ScrollRect.MovementType.Clamped;
                scrollScroll.horizontal = false;
                scrollScroll.scrollSensitivity = 50f;

                // Scrollbar
                {

                }

                viewport.transform.parent = scrollView.transform;
                viewport.name = "Viewport";
                var viewportRect = viewport.GetOrAddComponent<RectTransform>();
                scrollScroll.viewport = viewportRect;

                content.transform.parent = viewport.transform;
                content.name = "Content";
                var contentRect = content.GetOrAddComponent<RectTransform>();
                scrollScroll.content = contentRect;
            }

            // Viewport Settings
            {
                var rect = viewport.GetOrAddComponent<RectTransform>();
                rect.anchorMin = new Vector2(0, 0);
                rect.anchorMax = new Vector2(1, 1);
                rect.offsetMin = Vector2.zero;
                rect.offsetMax = Vector2.zero;
                var image = viewport.AddComponent<Image>();
                image.color = new Color(1f, 1f, 1f, 1f/255f);
                var mask = viewport.AddComponent<Mask>();
            }

            // Content Settings
            {
                var rect = content.GetOrAddComponent<RectTransform>();
                rect.anchorMin = new Vector2(0, 0);
                rect.anchorMax = new Vector2(1, 1);
                rect.offsetMin = Vector2.zero;
                rect.offsetMax = Vector2.zero;

                var grid = content.AddComponent<GridLayoutGroup>();
                grid.padding = new RectOffset(20, 20, 10, 10);
                grid.cellSize = new Vector2(500f, 500f);
                grid.spacing = new Vector2(10f, 10f);
                grid.startCorner = GridLayoutGroup.Corner.UpperLeft;
                grid.startAxis = GridLayoutGroup.Axis.Horizontal;
                grid.childAlignment = TextAnchor.MiddleCenter;
                grid.constraint = GridLayoutGroup.Constraint.Flexible;

                var fitter = content.AddComponent<ContentSizeFitter>();
                fitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
                fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

                var image = content.AddComponent<Image>();
                image.color = new Color(0.6f, 0.6f, 0.6f, 0.7f);
            }

            void CreateTestGrid(GameObject parent, PlayerSkin skin, string teamName)
            {
                var container = new GameObject();
                container.transform.parent = parent.transform;
                container.name = teamName;
                Color[] colors = new Color[] { skin.color, skin.backgroundColor, skin.winText, skin.particleEffect };
                string[] colorTypes = new string[] { "Color", "Background", "Win", "Particle" };

                // Add a Vertical Layout Group
                {
                    var VLG = container.AddComponent<VerticalLayoutGroup>();
                    VLG.padding = new RectOffset(0, 0, 0, 0);
                    VLG.spacing = 0f;
                    VLG.childAlignment = TextAnchor.UpperCenter;
                    VLG.childControlHeight = false;
                    VLG.childControlWidth = true;
                    VLG.childForceExpandHeight = false;
                    VLG.childForceExpandWidth = true;
                }
                
                RectTransform CreateHeadingBox(GameObject parent, Color color, string colorType)
                {
                    // Box container is a Vertical Layout Group for its children
                    var box = new GameObject();
                    box.transform.parent = parent.transform;
                    box.name = colorType + " Color";
                    var rect = box.GetOrAddComponent<RectTransform>();
                    {
                        var VLG = box.AddComponent<VerticalLayoutGroup>();
                        VLG.padding = new RectOffset(0, 0, 0, 0);
                        VLG.spacing = 0f;
                        VLG.childAlignment = TextAnchor.MiddleCenter;
                        VLG.childControlHeight = false;
                        VLG.childControlWidth = true;
                        VLG.childForceExpandHeight = false;
                        VLG.childForceExpandWidth = true;
                    }
                    

                    // The first item is the contrast of that Color with White
                    var whiteContrast = new GameObject();
                    whiteContrast.transform.parent = box.transform;
                    whiteContrast.name = "White";
                    {
                        var tempRect = whiteContrast.GetOrAddComponent<RectTransform>();
                        tempRect.sizeDelta = new Vector2(100f, 100f * 1f / 3f);
                        var tempText = whiteContrast.AddComponent<TextMeshProUGUI>();
                        tempText.enableAutoSizing = true;
                        tempText.autoSizeTextContainer = false;
                        tempText.alignment = TextAlignmentOptions.Center;
                        tempText.fontSizeMax = 36f;
                        tempText.fontSizeMin = 10;
                        tempText.enableWordWrapping = false;
                        tempText.overflowMode = TextOverflowModes.Truncate;
                        tempText.color = Color.white;

                        tempText.text = string.Format("{0:F1} : 1", ColorContrast(Color.white, color));
                    }

                    // The second item is the name of the color
                    var colorName = new GameObject();
                    colorName.transform.parent = box.transform;
                    colorName.name = "Name";
                    {
                        var tempRect = colorName.GetOrAddComponent<RectTransform>();
                        tempRect.sizeDelta = new Vector2(100f, 100f * 1f / 3f);
                        var tempText = colorName.AddComponent<TextMeshProUGUI>();
                        tempText.enableAutoSizing = true;
                        tempText.autoSizeTextContainer = false;
                        tempText.alignment = TextAlignmentOptions.Center;
                        tempText.fontSizeMax = 36f;
                        tempText.fontSizeMin = 10;
                        tempText.enableWordWrapping = false;
                        tempText.overflowMode = TextOverflowModes.Truncate;
                        tempText.color = color;

                        tempText.text = colorType;
                    }

                    // The lastt item is the contrast of that Color with black
                    var blackContrast = new GameObject();
                    blackContrast.transform.parent = box.transform;
                    blackContrast.name = "Black";
                    {
                        var tempRect = blackContrast.GetOrAddComponent<RectTransform>();
                        tempRect.sizeDelta = new Vector2(100f, 33f);
                        var tempText = blackContrast.AddComponent<TextMeshProUGUI>();
                        tempText.enableAutoSizing = true;
                        tempText.autoSizeTextContainer = false;
                        tempText.alignment = TextAlignmentOptions.Center;
                        tempText.fontSizeMax = 36f;
                        tempText.fontSizeMin = 10;
                        tempText.enableWordWrapping = false;
                        tempText.overflowMode = TextOverflowModes.Truncate;
                        tempText.color = Color.black;

                        tempText.text = string.Format("{0:F1} : 1", ColorContrast(Color.black, color));
                        tempRect.sizeDelta = new Vector2(100f, 33f);
                    }

                    whiteContrast.GetComponent<RectTransform>().sizeDelta = new Vector2(100f, 33f);
                    colorName.GetComponent<RectTransform>().sizeDelta = new Vector2(100f, 33f);

                    return rect;
                }

                RectTransform CreateColorBox(GameObject parent, Color textColor, string textColorType, Color color, string colorColorType)
                {
                    var colors = GetContrastingColors(textColor, color, 3f);

                    if (RelativeLuminance(textColor) < RelativeLuminance(color))
                    {
                        colors = new Color[] { colors[1], colors[0] };
                    }

                    // Box container is a Vertical Layout Group for its children
                    var box = new GameObject();
                    box.transform.parent = parent.transform;
                    box.name = textColorType + " on " + colorColorType;
                    var rect = box.GetOrAddComponent<RectTransform>();
                    {
                        var VLG = box.AddComponent<VerticalLayoutGroup>();
                        VLG.padding = new RectOffset(0, 0, 0, 0);
                        VLG.spacing = 0f;
                        VLG.childAlignment = TextAnchor.MiddleCenter;
                        VLG.childControlHeight = true;
                        VLG.childControlWidth = true;
                        VLG.childForceExpandHeight = true;
                        VLG.childForceExpandWidth = true;
                    }

                    var image = box.AddComponent<Image>();
                    image.color = colors[1];


                    // The first item is the contrast of that Color with White
                    var colorContrast = new GameObject();
                    colorContrast.transform.parent = box.transform;
                    colorContrast.name = "Contrast";
                    {
                        var text = colorContrast.AddComponent<TextMeshProUGUI>();
                        text.enableAutoSizing = true;
                        text.autoSizeTextContainer = false;
                        text.alignment = TextAlignmentOptions.Center;
                        text.fontSizeMax = 36f;
                        text.fontSizeMin = 10;
                        text.enableWordWrapping = false;
                        text.overflowMode = TextOverflowModes.Truncate;
                        text.color = colors[0];

                        text.text = string.Format("{0:F1} : 1", ColorContrast(colors[0], colors[1]));
                    }

                    return rect;
                }

                // Create the Header row object
                var headerRow = new GameObject();
                headerRow.transform.parent = container.transform;
                headerRow.name = "Header Row";
                {
                    var rect = headerRow.GetOrAddComponent<RectTransform>();
                    rect.sizeDelta = new Vector2(500f, 100f);
                }

                // Populate the Header Row
                {
                    var titleBox = new GameObject();
                    titleBox.transform.parent = headerRow.transform;
                    titleBox.name = "Team";
                    {
                        var rect = titleBox.GetOrAddComponent<RectTransform>();
                        rect.anchorMin = new Vector2(0, 0);
                        rect.anchorMax = new Vector2(0.2f, 1);
                        rect.offsetMin = Vector2.zero;
                        rect.offsetMax = Vector2.zero;

                        var VLG = titleBox.AddComponent<VerticalLayoutGroup>();
                        VLG.padding = new RectOffset(0, 0, 0, 0);
                        VLG.spacing = 0f;
                        VLG.childAlignment = TextAnchor.MiddleCenter;
                        VLG.childControlHeight = false;
                        VLG.childControlWidth = true;
                        VLG.childForceExpandHeight = false;
                        VLG.childForceExpandWidth = true;
                    }

                    var titleName = new GameObject();
                    titleName.transform.parent = titleBox.transform;
                    titleName.name = "Name";
                    {
                        var rect = titleName.GetOrAddComponent<RectTransform>();
                        rect.anchorMin = new Vector2(0, 0);
                        rect.anchorMax = new Vector2(1f, 1);
                        rect.offsetMin = Vector2.zero;
                        rect.offsetMax = Vector2.zero;

                        var text = titleName.AddComponent<TextMeshProUGUI>();
                        text.text = teamName;
                        text.enableAutoSizing = true;
                        text.autoSizeTextContainer = false;
                        text.fontSizeMax = 36f;
                        text.fontSizeMin = 10f;


                    }

                    for (int i = 0; i < colors.Length; i++)
                    {
                        try
                        {
                            var rect = CreateHeadingBox(headerRow, colors[i], colorTypes[i]);
                            rect.anchorMin = new Vector2(0.2f * (i + 1), 0);
                            rect.anchorMax = new Vector2(0.2f * (i + 2), 1);
                            rect.offsetMin = Vector2.zero;
                            rect.offsetMax = Vector2.zero;
                        }
                        catch (Exception e)
                        {
                            UnityEngine.Debug.LogException(e);
                        }
                    }
                }

                //Create the other rows
                {
                    for (int i = 0; i < colors.Length; i++)
                    {
                        var row = new GameObject();
                        row.transform.parent = container.transform;
                        row.name = colorTypes[i] + " Row";
                        {
                            var rect = row.GetOrAddComponent<RectTransform>();
                            rect.sizeDelta = new Vector2(500f, 100f);
                        }

                        try
                        {
                            var rowHeader = CreateHeadingBox(row, colors[i], colorTypes[i]);
                            rowHeader.anchorMin = new Vector2(0, 0);
                            rowHeader.anchorMax = new Vector2(0.2f, 1);
                            rowHeader.offsetMin = Vector2.zero;
                            rowHeader.offsetMax = Vector2.zero;
                        }
                        catch (Exception e)
                        {
                            UnityEngine.Debug.LogException(e);
                        }

                        for (int i2 = 0; i2 < colors.Length; i2++)
                        {
                            try
                            {
                                var rect = CreateColorBox(row, colors[i2], colorTypes[i2], colors[i], colorTypes[i]);
                                rect.anchorMin = new Vector2(0.2f * (i2 + 1), 0);
                                rect.anchorMax = new Vector2(0.2f * (i2 + 2), 1);
                                rect.offsetMin = Vector2.zero;
                                rect.offsetMax = Vector2.zero;
                            }
                            catch (Exception e)
                            {
                                UnityEngine.Debug.LogException(e);
                            }
                        }
                    }
                }
            }

            for (int i = 0; i < extraSkinBases.Length; i++)
            {
                CreateTestGrid(content, extraSkinBases[i], GetTeamColorName(i));
            }

            return cTest;
        }

        float RelativeLuminance(Color color)
        {
            float ColorPartValue(float part)
            {
                return part <= 0.03928f ? part / 12.92f : Mathf.Pow((part + 0.055f) / 1.055f, 2.4f);
            }
            var r = ColorPartValue(color.r);
            var g = ColorPartValue(color.g);
            var b = ColorPartValue(color.b);

            var l = 0.2126f * r + 0.7152f * g + 0.0722f * b;
            return l;
        }

        private float ColorContrast(Color a, Color b)
        {
            float result = 0f;
            var La = RelativeLuminance(a) + 0.05f;
            var Lb = RelativeLuminance(b) + 0.05f;

            result = Mathf.Max(La, Lb) / Mathf.Min(La, Lb);

            return result;
        }

        private float ColorContrast(float luminanceA, Color b)
        {
            float result = 0f;
            var La = luminanceA + 0.05f;
            var Lb = RelativeLuminance(b) + 0.05f;

            result = Mathf.Max(La, Lb) / Mathf.Min(La, Lb);

            return result;
        }

        private float ColorContrast(Color a, float luminanceB)
        {
            float result = 0f;
            var La = RelativeLuminance(a) + 0.05f;
            var Lb = luminanceB + 0.05f;

            result = Mathf.Max(La, Lb) / Mathf.Min(La, Lb);

            return result;
        }

        /// <summary>
        /// Checks to see if a pair of colors contrast enough to be readable and returns a modified set if not.
        /// </summary>
        /// <param name="backgroundColor">The background color for the text to go on.</param>
        /// <param name="textColor">The intended text color.</param>
        /// <returns>A pair of contrasting colors, the lightest as the first color.</returns>
        public Color[] GetContrastingColors(Color backgroundColor, Color textColor, float ratio)
        {
            Color[] colors = new Color[2];

            var backL = RelativeLuminance(backgroundColor);
            var textL = RelativeLuminance(textColor);

            if (textL > backL)
            {
                colors[0] = textColor;
                colors[1] = backgroundColor;
            }
            else
            {
                colors[1] = textColor;
                colors[0] = backgroundColor;
            }

            // See if we have good enough contrast already
            if (!(ColorContrast(backgroundColor, textColor) < ratio))
            {
                return colors;
            }

            Color.RGBToHSV(colors[0], out var lightH, out var lightS, out var lightV);
            Color.RGBToHSV(colors[1], out var darkH, out var darkS, out var darkV);

            // If the darkest color can be darkened enough to have enough contrast after brightening the color.
            if (ColorContrast(Color.HSVToRGB(darkH, darkS, 0f), Color.HSVToRGB(lightH, lightS, 1f)) >= ratio)
            {
                var lightDiff = 1f - lightV;
                var darkDiff = darkV;

                var steps = new float[] { 0.12f, 0.1f, 0.08f, 0.05f, 0.04f, 0.03f, 0.02f, 0.01f, 0.005f };
                var step = 0;

                var lightRatio = (lightDiff / (lightDiff + darkDiff));
                var darkRatio = (darkDiff / (lightDiff + darkDiff));

                while (ColorContrast(Color.HSVToRGB(lightH, lightS, lightV), Color.HSVToRGB(darkH, darkS, darkV)) < ratio)
                {
                    while (ColorContrast(Color.HSVToRGB(lightH, lightS, lightV + lightRatio * steps[step]), Color.HSVToRGB(darkH, darkS, darkV - darkRatio * steps[step])) > ratio && step < steps.Length - 1)
                    {
                        step++;
                    }
                    lightV += lightRatio * steps[step];
                    darkV -= darkRatio * steps[step];
                }

                colors[0] = Color.HSVToRGB(lightH, lightS, lightV);
                colors[1] = Color.HSVToRGB(darkH, darkS, darkV);
            }
            // Fall back to using white.
            else
            {
                colors[0] = Color.white;

                var lightL = RelativeLuminance(colors[0]);

                while (ColorContrast(lightL, Color.HSVToRGB(darkH, darkS, darkV)) < ratio)
                {
                    darkV -= 0.01f;
                }

                colors[1] = Color.HSVToRGB(darkH, darkS, darkV);
            }

            return colors;
        }

        private static readonly PlayerSkin[] extraSkinBases = new PlayerSkin[]
        {
            // TEAM 1
            new PlayerSkin()
            {
                color = new Color(0.6392157f, 0.2862745f, 0.1686275f, 1f),
                backgroundColor = new Color(0.3490196f, 0.2392157f, 0.2117647f, 1f),
                winText = new Color(0.9137255f, 0.4980392f, 0.3568628f, 1f),
                particleEffect = new Color(0.6f, 0.2588235f, 0.09803922f, 1f)
            },
            // TEAM 2
            new PlayerSkin()
            {
                color = new Color(0.1647059f, 0.3098039f, 0.5843138f, 1f),
                backgroundColor = new Color(0.2196078f, 0.254902f, 0.3098039f, 1f),
                winText = new Color(0.3568628f, 0.6f, 0.9137255f, 1f),
                particleEffect = new Color(0.09803922f, 0.3215686f, 0.6039216f, 1f)
            },
            // TEAM 3
            new PlayerSkin()
            {
                color = new Color(0.6313726f, 0.2705882f, 0.2705882f, 1f),
                backgroundColor = new Color(0.3490196f, 0.2117647f, 0.2117647f, 1f),
                winText = new Color(0.9137255f, 0.3568628f, 0.3568628f, 1f),
                particleEffect = new Color(0.6039216f, 0.09803922f, 0.09803922f, 1f)
            },
            // TEAM 4
            new PlayerSkin()
            {
                color = new Color(0.2627451f, 0.5372549f, 0.3254902f, 1f),
                backgroundColor = new Color(0.2196078f, 0.3098039f, 0.2784314f, 1f),
                winText = new Color(0f, 0.6862745f, 0.2666667f, 1f),
                particleEffect = new Color(0.07843138f, 0.5529412f, 0.2784314f, 1f)
            },
            // TEAM 5
            new PlayerSkin()
            {
                color = new Color(0.6235294f, 0.6392157f, 0.172549f, 1f),
                backgroundColor = new Color(0.345098f, 0.3490196f, 0.2117647f, 1f),
                winText = new Color(0.8941177f, 0.9137255f, 0.3568628f, 1f),
                particleEffect = new Color(0.5882353f, 0.6039216f, 0.09803922f, 1f)
            },
            // TEAM 6
            new PlayerSkin()
            {
                color = new Color(0.3607843f, 0.172549f, 0.6392157f, 1f),
                backgroundColor = new Color(0.2666667f, 0.2117647f, 0.3490196f, 1f),
                winText = new Color(0.5803922f, 0.3568628f, 0.9137255f, 1f),
                particleEffect = new Color(0.3019608f, 0.09803922f, 0.6039216f, 1f)
            },
            // TEAM 7
            new PlayerSkin()
            {
                color = new Color(0.6392157f, 0.172549f, 0.3960784f, 1f),
                backgroundColor = new Color(0.3490196f, 0.2117647f, 0.345098f, 1f),
                winText = new Color(0.9137255f, 0.3568628f, 0.5960785f, 1f),
                particleEffect = new Color(0.6039216f, 0.09803922f, 0.282353f, 1f)
            },
            // TEAM 8
            new PlayerSkin()
            {
                color = new Color(0.172549f, 0.6392157f, 0.6117647f, 1f),
                backgroundColor = new Color(0.2117647f, 0.3490196f, 0.4117647f, 1f),
                winText = new Color(0.3568628f, 0.9137255f, 0.8705882f, 1f),
                particleEffect = new Color(0.09803922f, 0.6039216f, 0.6156863f, 1f)
            },
            // TEAM 9
            new PlayerSkin()
            {
                color = new Color(0.6392157f, 0.3607843f, 0.2705882f, 1f),
                backgroundColor = new Color(0.3490196f, 0.282353f, 0.2666667f, 1f),
                winText = new Color(0.9137255f, 0.6039216f, 0.5019608f, 1f),
                particleEffect = new Color(0.6f, 0.3215686f, 0.1921569f, 1f)
            },
            // TEAM 10
            new PlayerSkin()
            {
                color = new Color(0.254902f, 0.3686275f, 0.5843138f, 1f),
                backgroundColor = new Color(0.2666667f, 0.282353f, 0.3098039f, 1f),
                winText = new Color(0.5019608f, 0.682353f, 0.9137255f, 1f),
                particleEffect = new Color(0.1921569f, 0.372549f, 0.6039216f, 1f)
            },
            // TEAM 11
            new PlayerSkin()
            {
                color = new Color(0.6313726f, 0.3686275f, 0.3686275f, 1f),
                backgroundColor = new Color(0.3490196f, 0.2666667f, 0.2666667f, 1f),
                winText = new Color(0.9137255f, 0.5019608f, 0.5019608f, 1f),
                particleEffect = new Color(0.6039216f, 0.1921569f, 0.1921569f, 1f)
            },
            // TEAM 12
            new PlayerSkin()
            {
                color = new Color(0.345098f, 0.5372549f, 0.3882353f, 1f),
                backgroundColor = new Color(0.2666667f, 0.3098039f, 0.2941177f, 1f),
                winText = new Color(0.1058824f, 0.6862745f, 0.3333333f, 1f),
                particleEffect = new Color(0.1647059f, 0.5529412f, 0.3294118f, 1f)
            },
            // TEAM 13
            new PlayerSkin()
            {
                color = new Color(0.627451f, 0.6392157f, 0.3764706f, 1f),
                backgroundColor = new Color(0.345098f, 0.3490196f, 0.3215686f, 1f),
                winText = new Color(0.9019608f, 0.9137255f, 0.6470588f, 1f),
                particleEffect = new Color(0.5921569f, 0.6039216f, 0.2901961f, 1f)
            },
            // TEAM 14
            new PlayerSkin()
            {
                color = new Color(0.4196078f, 0.2745098f, 0.6392157f, 1f),
                backgroundColor = new Color(0.2980392f, 0.2666667f, 0.3490196f, 1f),
                winText = new Color(0.6666667f, 0.5019608f, 0.9137255f, 1f),
                particleEffect = new Color(0.3568628f, 0.1921569f, 0.6039216f, 1f)
            },
            // TEAM 15
            new PlayerSkin()
            {
                color = new Color(0.6392157f, 0.2745098f, 0.4470588f, 1f),
                backgroundColor = new Color(0.3490196f, 0.2666667f, 0.345098f, 1f),
                winText = new Color(0.9137255f, 0.5019608f, 0.6784314f, 1f),
                particleEffect = new Color(0.6039216f, 0.1921569f, 0.3411765f, 1f)
            },
            // TEAM 16
            new PlayerSkin()
            {
                color = new Color(0.2745098f, 0.6392157f, 0.6156863f, 1f),
                backgroundColor = new Color(0.2745098f, 0.3686275f, 0.4117647f, 1f),
                winText = new Color(0.5019608f, 0.9137255f, 0.8784314f, 1f),
                particleEffect = new Color(0.1960784f, 0.6039216f, 0.6156863f, 1f)
            },
            // TEAM 17
            new PlayerSkin()
            {
                color = new Color(0.4392157f, 0.1960784f, 0.1137255f, 1f),
                backgroundColor = new Color(0.2470588f, 0.1686275f, 0.1490196f, 1f),
                winText = new Color(0.7137255f, 0.3882353f, 0.2784314f, 1f),
                particleEffect = new Color(0.4f, 0.172549f, 0.0627451f, 1f)
            },
            // TEAM 18
            new PlayerSkin()
            {
                color = new Color(0.1058824f, 0.2f, 0.3843137f, 1f),
                backgroundColor = new Color(0.145098f, 0.172549f, 0.2078431f, 1f),
                winText = new Color(0.2784314f, 0.4666667f, 0.7137255f, 1f),
                particleEffect = new Color(0.0627451f, 0.2117647f, 0.4039216f, 1f)
            },
            // TEAM 19
            new PlayerSkin()
            {
                color = new Color(0.4313726f, 0.1843137f, 0.1843137f, 1f),
                backgroundColor = new Color(0.2470588f, 0.1490196f, 0.1490196f, 1f),
                winText = new Color(0.7137255f, 0.2784314f, 0.2784314f, 1f),
                particleEffect = new Color(0.4039216f, 0.0627451f, 0.0627451f, 1f)
            },
            // TEAM 20
            new PlayerSkin()
            {
                color = new Color(0.1647059f, 0.3372549f, 0.2039216f, 1f),
                backgroundColor = new Color(0.145098f, 0.2078431f, 0.1882353f, 1f),
                winText = new Color(0f, 0.4862745f, 0.1882353f, 1f),
                particleEffect = new Color(0.04705882f, 0.3529412f, 0.1764706f, 1f)
            },
            // TEAM 21
            new PlayerSkin()
            {
                color = new Color(0.427451f, 0.4392157f, 0.1176471f, 1f),
                backgroundColor = new Color(0.2431373f, 0.2470588f, 0.1490196f, 1f),
                winText = new Color(0.6980392f, 0.7137255f, 0.2784314f, 1f),
                particleEffect = new Color(0.3921569f, 0.4039216f, 0.0627451f, 1f)
            },
            // TEAM 22
            new PlayerSkin()
            {
                color = new Color(0.2470588f, 0.1176471f, 0.4392157f, 1f),
                backgroundColor = new Color(0.1882353f, 0.1490196f, 0.2470588f, 1f),
                winText = new Color(0.4509804f, 0.2784314f, 0.7137255f, 1f),
                particleEffect = new Color(0.2f, 0.0627451f, 0.4039216f, 1f)
            },
            // TEAM 23
            new PlayerSkin()
            {
                color = new Color(0.4392157f, 0.1176471f, 0.2705882f, 1f),
                backgroundColor = new Color(0.2470588f, 0.1490196f, 0.2431373f, 1f),
                winText = new Color(0.7137255f, 0.2784314f, 0.4627451f, 1f),
                particleEffect = new Color(0.4039216f, 0.0627451f, 0.1882353f, 1f)
            },
            // TEAM 24
            new PlayerSkin()
            {
                color = new Color(0.1176471f, 0.4392157f, 0.4196078f, 1f),
                backgroundColor = new Color(0.1568628f, 0.2627451f, 0.3098039f, 1f),
                winText = new Color(0.2784314f, 0.7137255f, 0.6784314f, 1f),
                particleEffect = new Color(0.0627451f, 0.4039216f, 0.4156863f, 1f)
            },
            // TEAM 25
            new PlayerSkin()
            {
                color = new Color(0.4392157f, 0.2470588f, 0.1843137f, 1f),
                backgroundColor = new Color(0.2470588f, 0.2f, 0.1882353f, 1f),
                winText = new Color(0.7137255f, 0.4705882f, 0.3882353f, 1f),
                particleEffect = new Color(0.4f, 0.2117647f, 0.1254902f, 1f)
            },
            // TEAM 26
            new PlayerSkin()
            {
                color = new Color(0.1647059f, 0.2392157f, 0.3843137f, 1f),
                backgroundColor = new Color(0.1803922f, 0.1882353f, 0.2078431f, 1f),
                winText = new Color(0.3882353f, 0.5294118f, 0.7137255f, 1f),
                particleEffect = new Color(0.1254902f, 0.2470588f, 0.4039216f, 1f)
            },
            // TEAM 27
            new PlayerSkin()
            {
                color = new Color(0.4313726f, 0.2509804f, 0.2509804f, 1f),
                backgroundColor = new Color(0.2470588f, 0.1882353f, 0.1882353f, 1f),
                winText = new Color(0.7137255f, 0.3882353f, 0.3882353f, 1f),
                particleEffect = new Color(0.4039216f, 0.1254902f, 0.1254902f, 1f)
            },
            // TEAM 28
            new PlayerSkin()
            {
                color = new Color(0.2156863f, 0.3372549f, 0.2431373f, 1f),
                backgroundColor = new Color(0.1803922f, 0.2078431f, 0.1960784f, 1f),
                winText = new Color(0.07450981f, 0.4862745f, 0.2352941f, 1f),
                particleEffect = new Color(0.1019608f, 0.3529412f, 0.2078431f, 1f)
            },
            // TEAM 29
            new PlayerSkin()
            {
                color = new Color(0.427451f, 0.4392157f, 0.254902f, 1f),
                backgroundColor = new Color(0.2431373f, 0.2470588f, 0.227451f, 1f),
                winText = new Color(0.7019608f, 0.7137255f, 0.5019608f, 1f),
                particleEffect = new Color(0.3921569f, 0.4039216f, 0.1921569f, 1f)
            },
            // TEAM 30
            new PlayerSkin()
            {
                color = new Color(0.2862745f, 0.1882353f, 0.4392157f, 1f),
                backgroundColor = new Color(0.2117647f, 0.1882353f, 0.2470588f, 1f),
                winText = new Color(0.5176471f, 0.3882353f, 0.7137255f, 1f),
                particleEffect = new Color(0.2352941f, 0.1254902f, 0.4039216f, 1f)
            },
            // TEAM 31
            new PlayerSkin()
            {
                color = new Color(0.4392157f, 0.1882353f, 0.3058824f, 1f),
                backgroundColor = new Color(0.2470588f, 0.1882353f, 0.2431373f, 1f),
                winText = new Color(0.7137255f, 0.3882353f, 0.5294118f, 1f),
                particleEffect = new Color(0.4039216f, 0.1254902f, 0.227451f, 1f)
            },
            // TEAM 32
            new PlayerSkin()
            {
                color = new Color(0.1882353f, 0.4392157f, 0.4196078f, 1f),
                backgroundColor = new Color(0.2078431f, 0.2784314f, 0.3098039f, 1f),
                winText = new Color(0.3882353f, 0.7137255f, 0.682353f, 1f),
                particleEffect = new Color(0.1294118f, 0.4039216f, 0.4156863f, 1f)
            }

        };
        public static string GetTeamColorName(int teamID)
        {
            // team names as colors

            switch (teamID)
            {
                case 0:
                    return "Orange";
                case 1:
                    return "Blue";
                case 2:
                    return "Red";
                case 3:
                    return "Green";
                case 4:
                    return "Yellow";
                case 5:
                    return "Purple";
                case 6:
                    return "Magenta";
                case 7:
                    return "Cyan";
                case 8:
                    return "Tangerine";
                case 9:
                    return "Light Blue";
                case 10:
                    return "Peach";
                case 11:
                    return "Lime";
                case 12:
                    return "Light Yellow";
                case 13:
                    return "Orchid";
                case 14:
                    return "Pink";
                case 15:
                    return "Aquamarine";
                case 16:
                    return "Dark Orange";
                case 17:
                    return "Dark Blue";
                case 18:
                    return "Dark Red";
                case 19:
                    return "Dark Green";
                case 20:
                    return "Dark Yellow";
                case 21:
                    return "Indigo";
                case 22:
                    return "Telemagenta";
                case 23:
                    return "Teal";
                case 24:
                    return "Burnt Orange";
                case 25:
                    return "Midnight Blue";
                case 26:
                    return "Maroon";
                case 27:
                    return "Evergreen";
                case 28:
                    return "Gold";
                case 29:
                    return "Violet";
                case 30:
                    return "Ruby";
                case 31:
                    return "Dark Cyan";
                default:
                    return (teamID + 1).ToString();
            }
        }
    }
}
