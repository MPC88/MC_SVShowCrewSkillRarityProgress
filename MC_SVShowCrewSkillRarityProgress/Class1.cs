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
        public const string pluginVersion = "1.0.1";

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
        private static void CrewSkillGetString_Post(CrewSkill __instance, CrewMember crewMember, ref string __result)
        {
            if (!tutorOnly.Value || PChar.Char.HasPerk(idTutor))
            {
                string additional = "";

                int max = crewMember.learningMode == 0 || crewMember.learningMode == 3 ? __instance.Rank(true) : __instance.Rank(true) + 1;
                int total = 0;
                __instance.skillBonus.ForEach(x => total += x.level);

                if (__instance.Rank(true) == crewMember.maxSkillLevel && total >= max)
                    additional = "(-%) ";
                else
                {
                    int progress = Mathf.RoundToInt(((__instance.value % 3000) / 3000f) * 100);
                    additional = "(" + progress + "%)";
                }

                __result = additional + __result;
            }
        }
    }
}
