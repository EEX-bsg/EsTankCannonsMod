using System;
using UnityEngine;
using UnityEngine.UI;

namespace ETCM
{
    /// <summary>
    /// タイムフューズの現在値を一時的に表示するローカル専用HUD。
    /// Mod.OnLoad()で「ETCM UI」Canvasへシングルトンとして1個だけ生成する。
    /// </summary>
    public class ETCMTimeFuseHUD : MonoBehaviour
    {
        public static ETCMTimeFuseHUD Instance;

        // 最終入力からこの秒数が経過したら自動で隠す
        private const float DisplayDuration = 2f;

        private readonly Color32 UIColor = new Color32(0, 255, 214, 255);

        private GameObject fuseUI;
        private Text fuseText;
        private float hideTime;
        private bool isShown;
        private float lastShownValue = float.NaN;

        void Awake()
        {
            Instance = this;
            GenerateUI();
            SetShown(false);
        }

        void Update()
        {
            if (!isShown) return;
            if (Time.unscaledTime >= hideTime)
            {
                SetShown(false);
            }
        }

        /// <summary>
        /// 指定した信管時間を表示し、自動消去タイマーを延長する。
        /// </summary>
        public void Show(float seconds)
        {
            if (fuseText == null) return;
            // 値が変わったときだけ文字列を作り直す（上下限に張り付いた場合の無駄を避ける）
            if (seconds != lastShownValue)
            {
                lastShownValue = seconds;
                fuseText.text = "FUZE " + seconds.ToString("F2") + " s";
            }
            hideTime = Time.unscaledTime + DisplayDuration;
            SetShown(true);
        }

        public void Hide()
        {
            SetShown(false);
        }

        private void SetShown(bool shown)
        {
            isShown = shown;
            if (fuseUI != null)
            {
                fuseUI.SetActive(shown);
            }
        }

        private void GenerateUI()
        {
            Font arial = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;

            fuseUI = new GameObject("TimeFuseUI");
            fuseUI.transform.SetParent(transform);
            fuseUI.layer = LayerMask.NameToLayer("HUD");
            RectTransform rect = fuseUI.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = new Vector2(260, 40);
            // 画面中心よりやや右。位置は固定でオフセット調整用スライダーは設けない
            rect.anchoredPosition = new Vector2(170, -20);

            fuseText = fuseUI.AddComponent<Text>();
            fuseText.text = "FUZE 0.00 s";
            fuseText.font = arial;
            fuseText.fontSize = 26;
            fuseText.fontStyle = FontStyle.BoldAndItalic;
            fuseText.color = UIColor;
            fuseText.alignment = TextAnchor.MiddleLeft;
        }
    }
}
