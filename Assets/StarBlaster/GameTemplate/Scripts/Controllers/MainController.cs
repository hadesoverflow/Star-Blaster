using System;
using DenkKits.AudioManager.Scripts;
using DenkKits.UIManager.Scripts.Base;
using DenkKits.UIManager.Scripts.UIView;
using UnityEngine;

namespace _GAME.Scripts.Controllers
{
    public class MainController : MonoBehaviour
    {
        private void Awake()
        {
            UIManager.Instance.ViewManager.ShowView(UIViewName.MainView);
            UIManager.Instance.HideTransition(() => {  });
        }

        private void Start()
        {
        }
    }
}