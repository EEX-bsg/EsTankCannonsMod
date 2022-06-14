using System;
using Modding;
using UnityEngine;

namespace warning
{
    public class Mod : ModEntryPoint
    {

        public override void OnLoad()
        {
            UnityEngine.Object.DontDestroyOnLoad(SingleInstance<CreateWarningGUI>.Instance);
        }

    }
    public class CreateTextArea : SingleInstance<CreateTextArea>
    {
        public override string Name
        {
            get
            {
                return "CreateTextArea";
            }
        }
        private string textToEdit = "AddCustomModuleMod(ACMmod) is not loaded.\nPlease try the following.\n1. Subscribe to ACM on SteamWorkshop.\n2. Open the Mods tab in Besiege's title scene.\n3. Turn on the ACMmod and E'sTankCannonsMod.\n4. Restart Besiege.\nIf the problem persists, contact @EEX_bsg.";
        private GUIStyle style = new GUIStyle();
        private int width = Screen.width;
        private int height = Screen.height;
        void OnGUI()
        {
            style.fontSize = width / 40;
            style.normal.textColor = Color.red;
            // テキストエリアを表示する
            GUI.TextArea(new Rect(width / 10, height / 10, width - (width / 10), height - (height / 10)), textToEdit, style);
        }
        internal static void destroyMe()
        {
            Destroy(Instance);
        }
    }
    public class CreateWarningGUI : SingleInstance<CreateWarningGUI>
    {
        public override string Name
        {
            get
            {
                return "CreateWarningGUI";
            }
        }
        private static readonly Guid ACMguid = new Guid("a4577151-2173-4084-a456-4b29e8d3e01f");
        private static bool w = true;


        private void warning()
        {
            if (!Mods.IsModLoaded(ACMguid))
            {
                w = false;
                Debug.LogError("ETCM-ModChecker: ACM is not loaded");
                DontDestroyOnLoad(SingleInstance<CreateTextArea>.Instance);

            }
        }

        void Update()
        {
            if (w)
            {
                if (!(StatMaster.inMenu || StatMaster.isMainMenu))
                {
                    warning();
                }
            }
            else
            {
                if (StatMaster.inMenu || StatMaster.isMainMenu)
                {
                    CreateTextArea.destroyMe();
                    w = true;
                }
            }
        }
    }
}
