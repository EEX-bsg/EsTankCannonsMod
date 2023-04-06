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
}