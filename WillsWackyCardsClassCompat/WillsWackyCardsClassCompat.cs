using System.Collections.Generic;
using BepInEx;
using HarmonyLib;
using UnboundLib;


namespace WWCC
{
    [BepInDependency("com.willis.rounds.unbound", BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency("com.willuwontu.rounds.managers", BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency("pykess.rounds.plugins.moddingutils", BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency("pykess.rounds.plugins.cardchoicespawnuniquecardpatch", BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency("com.willuwontu.rounds.cards", BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency("fluxxfield.rounds.plugins.classesmanager", BepInDependency.DependencyFlags.HardDependency)]
    [BepInPlugin(ModId, ModName, Version)]
    [BepInProcess("Rounds.exe")]
    public class WillsWackyCardsClassCompat : BaseUnityPlugin
    {
        private const string ModId = "com.willuwontu.rounds.cardsclasscompat";
        private const string ModName = "Will's Wacky Cards Classes Compatibility Handler";
        public const string Version = WWC.WillsWackyCards.Version; // What version are we on (major.minor.patch)?

        public const string ModInitials = "WWCC";

        public const string CurseEaterClassName = "Curse Eater";

        public static WillsWackyCardsClassCompat instance;

        void Awake()
        {

        }

        private void Start()
        {
            instance = this;

            ClassesManager.ClassesManager.Instance.AddClassProgressionCategories(new List<string>
            {
                CurseEaterClassName
            });

            var harmony = new Harmony(ModId);
            harmony.PatchAll();
        }
    }

}
