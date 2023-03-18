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

            // ETCMUI�I�u�W�F�N�g���쐬���ACanvas���A�^�b�`����
            UnityEngine.Object.DontDestroyOnLoad(ETCMUI = new GameObject("ETCM UI"));
            Canvas canvas = ETCMUI.AddComponent<Canvas>();
            // Canvas�̐ݒ���s��
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 0;
            canvas.gameObject.layer = LayerMask.NameToLayer("HUD");
            // ��ʃT�C�Y�ɉ�����UI���X�P�[�����O���邽�߂̃R���|�[�l���g���A�^�b�`����
            ETCMUI.AddComponent<CanvasScaler>().scaleFactor = 1;
        }
    }
}