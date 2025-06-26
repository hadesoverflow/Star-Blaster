using System;
using _GAME.Scripts.Controllers;
using DenkKits.AudioManager.Scripts;
using DenkKits.UIManager.Scripts.Base;
using DenkKits.UIManager.Scripts.UIPopup;
using StarBlaster.GameTemplate.Scripts.Controllers;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace _GAME.Scripts.Popup
{
    public class SettingPopupParam
    {
        public bool showGroupBtn;
    }

    public class SettingPopup : UIPopup
    {
        [Serializable]
        struct SettingLanguageItemData
        {
            public int languageId;
            public Sprite icon;
            public string name;
        }

        [SerializeField] private Slider musicSetting;
        [SerializeField] private Slider audiSetting;
        [SerializeField] private GameObject groupBtn;

        public void OnClickExit()
        {
            AudioManager.Instance.PlaySfx(AudioName.UI_Click);
            Hide();
            UIManager.Instance.ShowTransition(() => { SceneManager.LoadScene(GameConstants.SceneMain); });
        }

        public void OnClickPlayAgain()
        {
            AudioManager.Instance.PlaySfx(AudioName.UI_Click);
            Hide();
            UIManager.Instance.ShowTransition(() => { SceneManager.LoadScene(GameConstants.SceneGame); });
        }

        protected override void OnShowing()
        {
            base.OnShowing();
            
            GameController.Instance?.PauseGame();
            musicSetting.SetValueWithoutNotify(AudioManager.Instance!.musicVolume);
            audiSetting.SetValueWithoutNotify(AudioManager.Instance!.audioVolume);

            var param = (SettingPopupParam)Parameter;
            if (param != null)
            {
                Debug.Log("SettingPopupParam: " + param.showGroupBtn);
                groupBtn.SetActive(param.showGroupBtn);
            }
        }

        protected override void OnHiding()
        {
            base.OnHiding();

            GameController.Instance?.ResumeGame();
            AudioManager.Instance.SaveAudioSetting();
        }

        public void OnClickClose()
        {
            AudioManager.Instance.PlaySfx(AudioName.UI_Click);
            Hide();
        }

        public void OnChangeMusic(float value)
        {
            AudioManager.Instance.SetMusicVolume(value);

            if (value > 0)
            {
                AudioManager.Instance.PlayMusic(AudioName.BGM_Menu);
            }
        }

        public void OnChangeAudio(float value)
        {
            AudioManager.Instance.SetAudioVolume(value);
        }
    }
}