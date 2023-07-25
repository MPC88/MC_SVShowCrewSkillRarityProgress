using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;

namespace MC_SVShowCrewSkillRarityProgress
{
    [BepInPlugin(pluginGuid, pluginName, pluginVersion)]
    public class Main : BaseUnityPlugin
    {
        public const string pluginGuid = "mc.starvalor.showcrewskillrarityprogress";
        public const string pluginName = "SV Show Crew Skill Rarity Progress";
        public const string pluginVersion = "1.0.0";

        private const int idTutor = 314;

        public static ConfigEntry<bool> tutorOnly;

        private static ManualLogSource log = BepInEx.Logging.Logger.CreateLogSource(pluginName);

        public void Awake()
        {
            Harmony.CreateAndPatchAll(typeof(Main));

            tutorOnly = Config.Bind<bool>(
                "Config",
                "Show only with tutor?",
                false,
                "Only shows rarity progress if player has tutor perk.");
        }

        [HarmonyPatch(typeof(CrewSkill), nameof(CrewSkill.GetString))]
        [HarmonyPostfix]
        private static void CrewSkillGetString_Post(CrewSkill __instance, ref string __result)
        {
            if (!tutorOnly.Value || PChar.Char.HasPerk(idTutor))
            {
                int progress = Mathf.RoundToInt(((__instance.value % 3000) / 3000f) * 100);

                __result = "(" + progress + "%) " + __result;
            }
        }
    }
}
