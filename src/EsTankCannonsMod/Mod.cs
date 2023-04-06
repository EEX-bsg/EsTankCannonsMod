using Modding;
using System.Xml.Serialization;
using Modding.Serialization;
using Modding.Modules;
using System.ComponentModel;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System;
using System.Collections;
using ETCM;

namespace ETCM
{
    public class Mod : ModEntryPoint
    {
        public static GameObject ETCMUI;

        public static void Log(string message)
        {
            ModConsole.Log("ETCM Log:" + message);
        }
        public static void Warning(string message)
        {
            ModConsole.Log("ETCM Warning:" + message);
        }
        public static void Error(string message)
        {
            ModConsole.Log("ETCM Error:" + message);
        }
        public override void OnLoad()
        {
            // Called when the mod is loaded.
            CustomModules.AddBlockModule<ETCMSoundBlockModule, ETCMSoundBlockBehaviour>("ETCMSoundBlockModule", canReload: true);
            CustomModules.AddBlockModule<ETCMAmmoUIModule, ETCMAmmoUIBehaviour>("ETCMAmmoUIModule", canReload: true);
            UnityEngine.Object.DontDestroyOnLoad(SingleInstance<ETCMmodule>.Instance);

            // ETCMUIオブジェクトを作成し、Canvasをアタッチする
            UnityEngine.Object.DontDestroyOnLoad(ETCMUI = new GameObject("ETCM UI"));
            Canvas canvas = ETCMUI.AddComponent<Canvas>();
            // Canvasの設定を行う
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 0;
            canvas.gameObject.layer = LayerMask.NameToLayer("HUD");
            // 画面サイズに応じてUIをスケーリングするためのコンポーネントをアタッチする
            ETCMUI.AddComponent<CanvasScaler>().scaleFactor = 1;
        }
    }
}