using System.Runtime.CompilerServices;
using EWL.Api;
using EWL.Modules;
using Modding;
using Modding.Modules;
using UnityEngine;

namespace ETCM.EwlBridge
{
    // EWL バックエンド。EWL の型はメソッド本体の中だけで使う(型の継承・フィールドに出さない)。
    // EWL 未導入でもこのアセンブリ自体はロードでき、下のメソッドが呼ばれない限り EWL は解決されない。
    public class Mod : ModEntryPoint
    {
        public override void OnLoad()
        {
            if (!Backend.UseEwl) return;
            Register();
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static void Register()
        {
            // EWL の標準モジュールを E砲名義で登録し直す(XML 側は modid なし)
            CustomModules.AddBlockModule<EwlWeaponModule, EwlWeaponBehaviour>("EwlWeapon", false);
            Backend.ConnectAmmo = ConnectAmmo;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static bool ConnectAmmo(GameObject block, AmmoFeed feed)
        {
            if (block.GetComponent<EwlWeaponBehaviour>() == null) return false;
            // シミュ開始でブロックが複製されるとリンクも複製される(Feed は複製されない)ので、あれば使い回す
            var link = block.GetComponent<EwlAmmoLink>();
            if (link == null) link = block.AddComponent<EwlAmmoLink>();
            link.Feed = feed;
            return true;
        }
    }

    // Weapon はシミュ開始時に作られるので、見つかるまで待ってから残弾表示につなぐ。
    internal sealed class EwlAmmoLink : MonoBehaviour
    {
        internal AmmoFeed Feed;
        private object _weapon; // EWL 型をフィールドに出さない
        private BlockBehaviour _block;

        private void Start()
        {
            _block = GetComponent<BlockBehaviour>();
        }

        private void Update()
        {
            if (_weapon != null || Feed == null || _block == null || !_block.isSimulating) return;
            TryLink();
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private void TryLink()
        {
            var all = Weapon.GetAll(gameObject);
            if (all.Length == 0) return;
            var w = all[0];
            _weapon = w;
            w.Fired += () => Feed.Fired();
            Feed.AmmoLeft = () => ((Weapon)_weapon).Ammo;
        }
    }
}
