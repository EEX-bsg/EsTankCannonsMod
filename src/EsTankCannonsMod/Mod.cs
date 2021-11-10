using Modding;
using System.Xml.Serialization;
using Modding.Serialization;
using Modding.Modules;
using System.ComponentModel;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

namespace ETCM
{
    public class Mod : ModEntryPoint
    {
        public override void OnLoad()
        {
            // Called when the mod is loaded.
            Log("Loaded correctly");
            CustomModules.AddBlockModule<ETCMSoundBlockModule, ETCMSoundBlockBehaviour>("ETCMSoundBlockModule", canReload: true);
        }
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