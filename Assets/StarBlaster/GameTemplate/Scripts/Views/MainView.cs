using _GAME.Scripts.Popup;
using DenkKits.AudioManager.Scripts;
using DenkKits.UIManager.Scripts.Base;
using DenkKits.UIManager.Scripts.UIPopup;
using DenkKits.UIManager.Scripts.UIView;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace _GAME.Scripts.Views
{
    public class MainView : UIView
    {
        [SerializeField] private TextMeshProUGUI highScoreText;

        protected override void OnShowing()
        {
            base.OnShowing();
            highScoreText.text = PlayerPrefs.GetInt(GameConstants.PlayerPrefsHighScore, 0).ToString();
        }

        public void QuitGame()
        {
            Application.Quit();
        }
 
  
        #region MAIN UI BUTTON CALLBACK
        public void OnClickPlayGame()
        {
            Hide();
            AudioManager.Instance.PlaySfx(AudioName.UI_Click);
            UIManager.Instance.ShowTransition(() => { SceneManager.LoadScene(GameConstants.SceneGame); });
        }

        public void OnClickOpenSetting()
        {
            var param = new SettingPopupParam
            {
                showGroupBtn = false
            };
            AudioManager.Instance.PlaySfx(AudioName.UI_Click);
            UIManager.Instance.PopupManager.ShowPopup(UIPopupName.SettingPopup, param);
        }

        
        public void OnClickLeaderBoard()
        {
            AudioManager.Instance.PlaySfx(AudioName.UI_Click);
            UIManager.Instance.PopupManager.ShowPopup(UIPopupName.LeaderBoardPopup);
        }
        #endregion
      
    }
}