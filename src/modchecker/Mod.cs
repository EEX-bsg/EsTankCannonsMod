using System;
using Modding;
using UnityEngine;
using Localisation;

namespace warning
{
    public class Mod : ModEntryPoint
    {
        public static GameObject WarningObject;
        public static bool isEnglish = true; //日本語以外は全部英語
        public static class ACMmod
        {
            public static readonly string name = "Add Custome Module Mod";
            public static readonly Guid id = new Guid("a4577151-2173-4084-a456-4b29e8d3e01f");
            public static readonly string workshopUrl = "https://steamcommunity.com/sharedfiles/filedetails/?id=1778796598";
        }


        public override void OnLoad()
        {
            if (SingleInstance<LocalisationManager>.Instance.currLangName == "日本語")
            {
                isEnglish = false;
            }
            bool isACMmodEnabled = Mods.IsModLoaded(ACMmod.id);
            if (!isACMmodEnabled)
            {
                UnityEngine.Object.DontDestroyOnLoad(WarningObject = new GameObject("ModRequirementNOticeUI"));
                WarningObject.AddComponent<warning.ModRequirementNotice>();
            }
        }

    }
}
