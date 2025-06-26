
namespace DenkKits.AudioManager.Scripts
{
    public enum AudioName
    {
        // Main Music

        NoSound = -1,

        #region BGM 1 - 10

        BGM_Menu = 0,
        BGM_GAMEPLAY = 1,

        #endregion

        #region UI SOUND 11 - 100

        UI_Click = 11,
        UI_Hover = 12,
        UI_Scroll = 13,
        UI_Confirm = 14,
        UI_Cancel = 15,
        UI_Notification = 16,
        UI_Alert = 17,
        UI_AlertError = 18,
        UI_AlertSuccess = 19,
        UI_AlertWarning = 20,
        UI_AlertInfo = 21,

        #endregion

        #region GAME PLAY > 100

        Gameplay_PlayerScore = 103,
        Gameplay_PlayerLose = 104,
        Gameplay_AsteroidExplo = 105,
        Gameplay_Shoot = 106,
        Gameplay_Leaser = 107,
        Gameplay_EarnSuperWeapon = 108,
       

        #endregion
    }

    public enum AudioType
    {
        SFX = 0,
        BGM = 1,
        AMB = 2,
    }
}