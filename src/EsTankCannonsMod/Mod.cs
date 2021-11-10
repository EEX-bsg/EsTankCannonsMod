using Modding;
using System.Xml.Serialization;
using Modding.Serialization;
using Modding.Modules;
using System.ComponentModel;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using System.Collections;

namespace ETCM
{
    public class Mod : ModEntryPoint
    {
        
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
            UnityEngine.Object.DontDestroyOnLoad(SingleInstance<ETCMmodule>.Instance);
        }
    }

    public class ETCMmodule : SingleInstance<ETCMmodule>
    {
        public override string Name
        {
            get
            {
                return "ETCMmodule";

            }
        }
        private IEnumerator CheckVersion()
        {
            //何となく1秒待機(処理順用)
            yield return new WaitForSeconds(1f);
            //Guidにはは自分のMod.xml内のIDを入れること
            Mod.Log("Version " + Mods.GetVersion(new Guid("50e63b55-b976-4009-82ab-66f989218122")));
        }
        public void Awake()
        {
            StartCoroutine(CheckVersion());
        }
    }

    [XmlRoot("ETCMSoundBlockModule")]
    [Reloadable]
    public class ETCMSoundBlockModule : BlockModule
    {
        [XmlElement("PlayKey")]
        [RequireToValidate]
        public MKeyReference PlayKey;

        [XmlArray("Sounds")]
        [RequireToValidate]
        [DefaultValue(null)]
        [CanBeEmpty]
        [XmlArrayItem("AudioClip", typeof(ResourceReference))]

        public object[] Sounds;

    }
    public class ETCMSoundBlockBehaviour : BlockModuleBehaviour<ETCMSoundBlockModule>
    {
        public MKey PlayKey;
        public int blockID;
        private AudioClip audioClip;
        private AudioSource audioSource;
        public List<string> SoundNames = new List<string>();
        public int pitch;

        public override void SafeAwake()
        {
            base.SafeAwake();
            blockID = BlockId;
            audioSource = gameObject.AddComponent<AudioSource>();
            try
            {
                PlayKey = GetKey(Module.PlayKey);
                object[] sounds = Module.Sounds;
                foreach (object obj in sounds)
                {
                    if (obj is ResourceReference)
                    {
                        ModAudioClip sound = (ModAudioClip)GetResource((ResourceReference)obj);
                        SoundNames.Add(sound.Name);
                    }
                    else
                    {
                        Mod.Error("サウンドファイルの形式が違うんよ");
                    }
                }
            }
            catch
            {
                Mod.Error("ブロックID" + blockID + "で読み込みエラー");
            }
        }
        public override void OnSimulateStart()
        {
            base.OnSimulateStart();
            SoundLoader(SoundNames[0]);
        }
        public override void SimulateUpdateAlways()
        {
            base.SimulateUpdateAlways();
            SoundPlayer();

        }
        private void SoundLoader(string SoundName)
        {
            try
            {
                audioClip = ModResource.GetAudioClip(SoundName);
                audioSource.clip = audioClip;
                audioSource.spatialBlend = 0;
            }
            catch (Exception ex)
            {
                Mod.Error("エラーだみょん　音データがロードできないみょん");
                Mod.Error(ex.ToString());
            }
        }
        private void SoundPlayer()
        {
            if (PlayKey.IsPressed || PlayKey.EmulationPressed())
            {
                audioSource.Stop();
                audioSource.Play();
            }

        }
    }
}