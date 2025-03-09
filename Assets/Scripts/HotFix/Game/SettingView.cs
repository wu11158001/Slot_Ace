using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SettingView : BasePopUpView
{
    [Space(30)]
    [Header("設置")]
    [SerializeField] TMP_Dropdown Language_Dd;
    [SerializeField] Button Music_Btn;
    [SerializeField] Button Sound_Btn;

    protected override void Awake()
    {
        base.Awake();

        // 設置語言選單內容
        Utils.I.SetOptionsToDropdown(
            dropdown: Language_Dd,
            options: new() { "繁體中文", "English" });

        Language_Dd.value = LanguageManager.I.CurrLanguage;

        // 音樂/音效開關
        Music_Btn.image.sprite =
            AudioManager.I.IsBGMOpen() ?
            AssetsManager.I.SOManager.Setting_SO.SpriteList[1] :
            AssetsManager.I.SOManager.Setting_SO.SpriteList[0];

        Sound_Btn.image.sprite =
            AudioManager.I.IsSoundOpen() ?
            AssetsManager.I.SOManager.Setting_SO.SpriteList[1] :
            AssetsManager.I.SOManager.Setting_SO.SpriteList[0];
    }

    private void Start()
    {
        // 語言選單
        Language_Dd.onValueChanged.AddListener((value) =>
        {
            LanguageManager.I.ChangeLanguage(value);
        });

        // 音樂按鈕
        Music_Btn.onClick.AddListener(() =>
        {
            bool isMusic = AudioManager.I.IsBGMOpen();
            AudioManager.I.BGMSwitch(!isMusic);

            Music_Btn.image.sprite =
                !isMusic ?
                AssetsManager.I.SOManager.Setting_SO.SpriteList[1] :
                AssetsManager.I.SOManager.Setting_SO.SpriteList[0];
        });

        // 音效按鈕
        Sound_Btn.onClick.AddListener(() =>
        {
            bool isSound = AudioManager.I.IsSoundOpen();
            AudioManager.I.SoundSwitch(!isSound);

            Sound_Btn.image.sprite =
                !isSound ?
                AssetsManager.I.SOManager.Setting_SO.SpriteList[1] :
                AssetsManager.I.SOManager.Setting_SO.SpriteList[0];
        });
    }
}
