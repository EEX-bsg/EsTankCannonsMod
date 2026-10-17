using System;
using System.ComponentModel;
using System.Xml.Serialization;
using UnityEngine;
using Modding;
using Modding.Serialization;
using Modding.Modules;
using Modding.Blocks;

namespace ETCM
{
    /// <summary>
    /// シミュレーション実行中に、同じブロック上のタイムフューズ用MSliderを
    /// 2個のMKeyで増減させるモジュール。
    /// ACMへは一切依存せず、同一スライダーを共有することだけで接続する。
    /// </summary>
    [XmlRoot("ETCMTimeFuseControlModule")]
    [Reloadable]
    public class ETCMTimeFuseControlModule : BlockModule
    {
        [XmlElement("TimeFuseSlider")]
        [RequireToValidate]
        public MSliderReference TimeFuseSlider;

        [XmlElement("IncreaseKey")]
        [RequireToValidate]
        public MKeyReference IncreaseKey;

        [XmlElement("DecreaseKey")]
        [RequireToValidate]
        public MKeyReference DecreaseKey;

        /// <summary>1回の押下で変化させる秒数。ユーザーには露出しない。</summary>
        [XmlElement("Step")]
        [DefaultValue(0.05f)]
        [Reloadable]
        public float Step = 0.05f;
    }

    public class ETCMTimeFuseControlBehaviour : BlockModuleBehaviour<ETCMTimeFuseControlModule>
    {
        // floatへの繰り返し加算による誤差蓄積を避けるため、整数刻みで計算する
        private const int TicksPerSecond = 100;

        private MSlider timeFuseSlider;
        private MKey increaseKey;
        private MKey decreaseKey;

        private bool isOwnerSame;
        private bool prevIncreasePressed;
        private bool prevDecreasePressed;
        private bool hudShownByThis;

        public override void SafeAwake()
        {
            base.SafeAwake();
            try
            {
                timeFuseSlider = GetSlider(Module.TimeFuseSlider);
                increaseKey = GetKey(Module.IncreaseKey);
                decreaseKey = GetKey(Module.DecreaseKey);
            }
            catch
            {
                Mod.Error("TimeFuseControl mapper reference error. BlockName : " + name);
                Destroy(this);
                return;
            }
        }

        public override void OnSimulateStart()
        {
            base.OnSimulateStart();
            UpdateOwnerFlag();
            prevIncreasePressed = false;
            prevDecreasePressed = false;
            hudShownByThis = false;
        }

        public override void OnSimulateStop()
        {
            base.OnSimulateStop();
            if (hudShownByThis && ETCMTimeFuseHUD.Instance != null)
            {
                ETCMTimeFuseHUD.Instance.Hide();
            }
            hudShownByThis = false;
        }

        public override void SimulateUpdateAlways()
        {
            base.SimulateUpdateAlways();

            // メニュー中はKeyInputController.UpdateKeys()がスキップされ、
            // キー状態が前の値のまま固まる。押下として扱わない。
            if (StatMaster.inMenu || StatMaster.stopHotkeys) return;

            // IsPressed は MP ホストでクライアントの操作が1フレームしか立たず取りこぼすことがあるので、IsHeld の変化で取る。
            // 実キーとエミュレーション(ロジックブロック等)は別経路。
            bool increasePressed = increaseKey.IsHeld || increaseKey.EmulationHeld(true);
            bool decreasePressed = decreaseKey.IsHeld || decreaseKey.EmulationHeld(true);

            // 自前のエッジ検出。false→trueの変化があったフレームだけを1押下とする
            bool increaseEdge = increasePressed && !prevIncreasePressed;
            bool decreaseEdge = decreasePressed && !prevDecreasePressed;
            prevIncreasePressed = increasePressed;
            prevDecreasePressed = decreasePressed;

            // 同時押しは相殺。どちらも無ければ何もしない
            if (increaseEdge == decreaseEdge) return;

            ApplyStep(increaseEdge);
        }

        private void ApplyStep(bool increase)
        {
            int step = Mathf.Max(1, Mathf.RoundToInt(Module.Step * TicksPerSecond));
            int minimum = Mathf.RoundToInt(timeFuseSlider.Min * TicksPerSecond);
            int maximum = Mathf.RoundToInt(timeFuseSlider.Max * TicksPerSecond);
            int current = Mathf.RoundToInt(timeFuseSlider.Value * TicksPerSecond);

            current += increase ? step : -step;
            // MSliderのsetterはクランプしないので明示的に丸める
            current = Mathf.Clamp(current, minimum, maximum);

            float newValue = current / (float)TicksPerSecond;
            // ValueChangedが発火する正規経路を使う。SetValue()/ApplyValue()は使わない
            timeFuseSlider.Value = newValue;

            if (!isOwnerSame) return;
            if (ETCMTimeFuseHUD.Instance == null) return;
            ETCMTimeFuseHUD.Instance.Show(newValue);
            hudShownByThis = true;
        }

        private void UpdateOwnerFlag()
        {
            if (StatMaster.isMP)
            {
                ushort blockPlayerID = BlockBehaviour.ParentMachine.PlayerID;
                ushort localPlayerID = PlayerMachine.GetLocal().Player.NetworkId;
                isOwnerSame = blockPlayerID == localPlayerID;
            }
            else
            {
                isOwnerSame = true;
            }
        }
    }
}
