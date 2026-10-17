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
        // E'sWeaponsLib(未公開のため警告ウィンドウには出さない)
        private static readonly Guid EwlId = new Guid("0d219960-b202-4906-b29d-8d90a61a89b8");


        public override void OnLoad()
        {
            if (SingleInstance<LocalisationManager>.Instance.currLangName == "日本語")
            {
                isEnglish = false;
            }
            // EWL が入っていれば武装バックエンドは EWL になるので ACM は不要
            bool isBackendEnabled = Mods.IsModLoaded(ACMmod.id) || Mods.IsModLoaded(EwlId);
            if (!isBackendEnabled)
            {
                UnityEngine.Object.DontDestroyOnLoad(WarningObject = new GameObject("ModRequirementNOticeUI"));
                WarningObject.AddComponent<warning.ModRequirementNotice>();
            }
        }

    }
}
