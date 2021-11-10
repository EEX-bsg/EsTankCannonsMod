using System;
using Modding;
using UnityEngine;

namespace warning
{
	public class Mod : ModEntryPoint
	{
        public static readonly Guid ACMguid = new Guid("a4577151-2173-4084-a456-4b29e8d3e01f");
        public override void OnLoad()
		{
            // Called when the mod is loaded.
            GameObject master = new GameObject();
            if (!Mods.IsModLoaded(ACMguid))
            {
                ModConsole.Log("ACMÇ™ì«Ç›çûÇ‹ÇÍÇƒÇ¢Ç‹ÇπÇÒ");
                ModConsole.Log("ACM is not loaded");
                //master.AddComponent<CreateText>();
            }
            
		}
	}
    public class CreateText : MonoBehaviour
    {
        void Awake()
        {

        }
    }
}
