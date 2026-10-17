using System.Runtime.CompilerServices;
using Modding;
using Modding.Modules;
using skpCustomModule;
using UnityEngine;

namespace ETCM.AcmBridge
{
    // ACM バックエンド。ACM の型はメソッド本体の中だけで使う(型の継承・フィールドに出さない)。
    // ACM 未導入でもこのアセンブリ自体はロードでき、下のメソッドが呼ばれない限り ACM は解決されない。
    // ACM 経路は凍結(EWL design-policy §6.4)。挙動を変える修正はしない。
    public class Mod : ModEntryPoint
    {
        public override void OnLoad()
        {
            if (!Backend.UseAcm) return;
            Register();
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static void Register()
        {
            // ACM 自身の登録と同じ型・同じ canReload を、E砲名義で登録し直す(XML 側は modid なし)
            CustomModules.AddBlockModule<AdBlockModule, AdBlockBehaviour>("AdBlockProp", true);
            CustomModules.AddBlockModule<AdShootingProp, AdShootingBehavour>("AdShootingProp", true);
            Backend.ConnectAmmo = ConnectAmmo;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static bool ConnectAmmo(GameObject block, AmmoFeed feed)
        {
            var s = block.GetComponent<AdShootingBehavour>();
            if (s == null) return false;
            s.OnFire += () => feed.Fired();
            s.OnMagazineReload += () => feed.ReloadStarted();
            Component c = s; // クロージャのフィールド型に ACM 型を出さない
            feed.AmmoLeft = () => ((AdShootingBehavour)c).AmmoLeft;
            feed.AmmoStock = () => ((AdShootingBehavour)c).AmmoStock;
            return true;
        }
    }
}
