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
using UnboundLib;
using UnboundLib.GameModes;
using UnboundLib.Cards;
using UnboundLib.Utils;
using WillsWackyCards.Cards;
using HarmonyLib;
using Photon.Pun;
using CardChoiceSpawnUniqueCardPatch.CustomCategories;

namespace WillsWackyCards
{
    [BepInDependency("com.willis.rounds.unbound", BepInDependency.DependencyFlags.HardDependency)]
    [BepInPlugin(ModId, ModName, Version)]
    [BepInProcess("Rounds.exe")]
    public class WillsWackyCards : BaseUnityPlugin
    {
        private const string ModId = "com.willuwontu.rounds.card";
        private const string ModName = "Wills Wacky Cards";
        public const string Version = "1.0.7"; // What version are we on (major.minor.patch)?

        void Awake()
        {
            var harmony = new Harmony(ModId);
            harmony.PatchAll();
        }
        void Start()
        {
            UnityEngine.Debug.Log("[WWC] Loading Cards");
            CustomCard.BuildCard<AmmoCache>();
            CustomCard.BuildCard<Shotgun>();
            CustomCard.BuildCard<SlowDeath>();
            CustomCard.BuildCard<Vampirism>();
            CustomCard.BuildCard<FastBall>();
            CustomCard.BuildCard<SlowBall>();
            CustomCard.BuildCard<Minigun>();
            UnityEngine.Debug.Log("[WWC] Cards Built");

            this.ExecuteAfterSeconds(0.4f, ChangeCards);
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
                                categories.Add(CustomCardCategories.instance.CardCategory("GunTypes"));
                                info.cardInfo.categories = categories.ToArray();
                                categories = new List<CardCategory>();
                                for (int i = 0; i < info.cardInfo.blacklistedCategories.Length; i++)
                                {
                                    categories.Add(info.cardInfo.blacklistedCategories[i]);
                                }
                                categories.Add(CustomCardCategories.instance.CardCategory("GunTypes"));
                                info.cardInfo.blacklistedCategories = categories.ToArray();
                            }
                            else
                            {
                                info.cardInfo.categories = new CardCategory[] { CustomCardCategories.instance.CardCategory("GunTypes") };
                                info.cardInfo.blacklistedCategories = new CardCategory[] { CustomCardCategories.instance.CardCategory("GunTypes") }; 
                            }
                            UnityEngine.Debug.Log("[WWC] Modified Flamethrower");
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
                                categories.Add(CustomCardCategories.instance.CardCategory("GunTypes"));
                                info.cardInfo.categories = categories.ToArray();
                                categories = new List<CardCategory>();
                                for (int i = 0; i < info.cardInfo.blacklistedCategories.Length; i++)
                                {
                                    categories.Add(info.cardInfo.blacklistedCategories[i]);
                                }
                                categories.Add(CustomCardCategories.instance.CardCategory("GunTypes"));
                                info.cardInfo.blacklistedCategories = categories.ToArray();
                            }
                            else
                            {
                                info.cardInfo.categories = new CardCategory[] { CustomCardCategories.instance.CardCategory("GunTypes") };
                                info.cardInfo.blacklistedCategories = new CardCategory[] { CustomCardCategories.instance.CardCategory("GunTypes") }; 
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
                                categories.Add(CustomCardCategories.instance.CardCategory("GunTypes"));
                                info.cardInfo.categories = categories.ToArray();
                                categories = new List<CardCategory>();
                                for (int i = 0; i < info.cardInfo.blacklistedCategories.Length; i++)
                                {
                                    categories.Add(info.cardInfo.blacklistedCategories[i]);
                                }
                                categories.Add(CustomCardCategories.instance.CardCategory("GunTypes"));
                                info.cardInfo.blacklistedCategories = categories.ToArray();
                            }
                            else
                            {
                                info.cardInfo.categories = new CardCategory[] { CustomCardCategories.instance.CardCategory("GunTypes") };
                                info.cardInfo.blacklistedCategories = new CardCategory[] { CustomCardCategories.instance.CardCategory("GunTypes") };
                            }
                            UnityEngine.Debug.Log("[WWC] Modified Comb");
                            break;
                        }
                    case "STARs":
                        {
                            UnityEngine.Debug.Log("[WWC] Found Star");
                            if (info.cardInfo.categories != null)
                            {
                                categories = new List<CardCategory>();
                                for (int i = 0; i < info.cardInfo.categories.Length; i++)
                                {
                                    categories.Add(info.cardInfo.categories[i]);
                                }
                                categories.Add(CustomCardCategories.instance.CardCategory("GunTypes"));
                                info.cardInfo.categories = categories.ToArray();
                                categories = new List<CardCategory>();
                                for (int i = 0; i < info.cardInfo.blacklistedCategories.Length; i++)
                                {
                                    categories.Add(info.cardInfo.blacklistedCategories[i]);
                                }
                                categories.Add(CustomCardCategories.instance.CardCategory("GunTypes"));
                                info.cardInfo.blacklistedCategories = categories.ToArray();
                            }
                            else
                            {
                                info.cardInfo.categories = new CardCategory[] { CustomCardCategories.instance.CardCategory("GunTypes") };
                                info.cardInfo.blacklistedCategories = new CardCategory[] { CustomCardCategories.instance.CardCategory("GunTypes") };
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
                                categories.Add(CustomCardCategories.instance.CardCategory("GunTypes"));
                                info.cardInfo.categories = categories.ToArray();
                                categories = new List<CardCategory>();
                                for (int i = 0; i < info.cardInfo.blacklistedCategories.Length; i++)
                                {
                                    categories.Add(info.cardInfo.blacklistedCategories[i]);
                                }
                                categories.Add(CustomCardCategories.instance.CardCategory("GunTypes"));
                                info.cardInfo.blacklistedCategories = categories.ToArray();
                            }
                            else
                            {
                                info.cardInfo.categories = new CardCategory[] { CustomCardCategories.instance.CardCategory("GunTypes") };
                                info.cardInfo.blacklistedCategories = new CardCategory[] { CustomCardCategories.instance.CardCategory("GunTypes") };
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
                                categories.Add(CustomCardCategories.instance.CardCategory("GunTypes"));
                                info.cardInfo.categories = categories.ToArray();
                                categories = new List<CardCategory>();
                                for (int i = 0; i < info.cardInfo.blacklistedCategories.Length; i++)
                                {
                                    categories.Add(info.cardInfo.blacklistedCategories[i]);
                                }
                                categories.Add(CustomCardCategories.instance.CardCategory("GunTypes"));
                                info.cardInfo.blacklistedCategories = categories.ToArray();
                            }
                            else
                            {
                                info.cardInfo.categories = new CardCategory[] { CustomCardCategories.instance.CardCategory("GunTypes") };
                                info.cardInfo.blacklistedCategories = new CardCategory[] { CustomCardCategories.instance.CardCategory("GunTypes") };
                            }
                            UnityEngine.Debug.Log("[WWC] Modified Rolling Thunder");
                            break;
                        }
                }

            }
            UnityEngine.Debug.Log("[WWC] Cards Modified");
        }
    }
}
