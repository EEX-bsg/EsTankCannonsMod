using System;
using Modding;
using Modding.Modules;
using System.Xml.Serialization;
using UnityEngine;

namespace ETCM
{
    // 武装バックエンド(ACM / EWL)の選択。ブロック XML は両方のモジュールを modid なしで併記し、
    // 採用側はブリッジ DLL が本物の型を、不採用側はここでスタブを E砲名義で登録する。
    // このアセンブリは ACM・EWL の型を一切参照しない(未導入でもロードできるように)。
    public static class Backend
    {
        public static readonly Guid AcmId = new Guid("a4577151-2173-4084-a456-4b29e8d3e01f");
        public static readonly Guid EwlId = new Guid("0d219960-b202-4906-b29d-8d90a61a89b8");

        // 両方導入時は EWL 優先
        // ponytail: 設定ファイルでの上書きは未実装。A/B 比較で必要になったら ModIO の設定を読む。
        public static bool UseEwl { get { return Mods.IsModLoaded(EwlId); } }
        public static bool UseAcm { get { return !UseEwl && Mods.IsModLoaded(AcmId); } }

        /// <summary>採用側ブリッジが設定する。ブロックの武装を残弾表示につなぐ。つなげたら true。</summary>
        public static Func<GameObject, AmmoFeed, bool> ConnectAmmo;

        internal static void RegisterStubs()
        {
            if (!UseAcm)
            {
                CustomModules.AddBlockModule<AdShootingPropStub, AdShootingPropStubBehaviour>("AdShootingProp", false);
                CustomModules.AddBlockModule<AdBlockPropStub, AdBlockPropStubBehaviour>("AdBlockProp", false);
            }
            if (!UseEwl)
            {
                CustomModules.AddBlockModule<EwlWeaponStub, EwlWeaponStubBehaviour>("EwlWeapon", false);
                CustomModules.AddBlockModule<EwlBlockPhysicsStub, EwlBlockPhysicsStubBehaviour>("EwlBlockPhysics", false);
            }
            Mod.Log("Weapon backend: " + (UseEwl ? "EWL" : UseAcm ? "ACM" : "none"));
        }
    }

    /// <summary>残弾表示に渡す口。ブリッジが中身を差し込む。</summary>
    public sealed class AmmoFeed
    {
        public Func<int> AmmoLeft = () => 0;
        public Func<int> AmmoStock = () => 0;
        public Action Fired = () => { };
        public Action ReloadStarted = () => { };
    }

    // 不採用側の名前を受け止める空モジュール。未知の子要素は読み込み時に無視される。
    [XmlRoot("AdShootingProp")] public class AdShootingPropStub : BlockModule { }
    [XmlRoot("AdBlockProp")] public class AdBlockPropStub : BlockModule { }
    [XmlRoot("EwlWeapon")] public class EwlWeaponStub : BlockModule { }
    [XmlRoot("EwlBlockPhysics")] public class EwlBlockPhysicsStub : BlockModule { }
    public class AdShootingPropStubBehaviour : BlockModuleBehaviour<AdShootingPropStub> { }
    public class AdBlockPropStubBehaviour : BlockModuleBehaviour<AdBlockPropStub> { }
    public class EwlWeaponStubBehaviour : BlockModuleBehaviour<EwlWeaponStub> { }
    public class EwlBlockPhysicsStubBehaviour : BlockModuleBehaviour<EwlBlockPhysicsStub> { }
}
