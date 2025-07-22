using System.Collections.Generic;
using UnityEngine;

namespace HoldOut
{
    public class MenuManager : Manager<MenuManager>
    {
        [Header("Components")]
        [SerializeField] private List<MenuController> _menuControllerList = new List<MenuController>();

        [Header("Runtime")]
        private Dictionary<MenuType, MenuController> _menuTypeControllerDict = new Dictionary<MenuType, MenuController>();

        #region Setup

        protected override void Setup()
        {
            base.Setup();

            SetupMenuDictionary();

            _isSetup = true;
        }

        private void SetupMenuDictionary()
        {
            for (int i = 0; i < _menuControllerList.Count; i++)
            {
                var menuController = _menuControllerList[i];
                menuController.Hide();
                if (!_menuTypeControllerDict.ContainsKey(menuController.MenuType))
                {
                    _menuTypeControllerDict.Add(menuController.MenuType, menuController);
                }
            }

            if (GameManager.Instance != null && GameManager.Instance.Ready)
            {
                GameStateChangedHandler(GameState.Initialization, GameManager.Instance.CurrentGameState);
            }
            if (EventManager.Instance != null && EventManager.Instance.Ready)
            {
                EventManager.Instance.GameStateEvents.OnGameStateChanged += GameStateChangedHandler;
            }
        }

        #endregion

        #region Menu Management

        private void GameStateChangedHandler(GameState oldState, GameState newState)
        {
            if (_showDebug)
            {
                Debug.Log($"Attempting show new menu with game state:{newState}");
            }

            switch (newState)
            {
                case GameState.Menu:
                    ShowMenu(MenuType.MainMenu);
                    break;
                case GameState.Game:
                    ShowMenu(MenuType.HUDMenu);
                    break;
                case GameState.Pause:
                    ShowMenu(MenuType.PauseMenu);
                    break;
                case GameState.GameOver:
                    ShowMenu(MenuType.GameOverMenu);
                    break;
                default:
                    ShowMenu(MenuType.LoadingMenu);
                    break;
            }
        }

        private void ShowMenu(MenuType menuType)
        {
            foreach (var menuTypeKey in _menuTypeControllerDict.Keys)
            {
                if (menuTypeKey != menuType)
                {
                    HideMenu(menuTypeKey);
                }
            }

            if (_menuTypeControllerDict.ContainsKey(menuType))
            {
                _menuTypeControllerDict[menuType].Show();
            }
        }

        private void HideMenu(MenuType menuType)
        {
            if (_menuTypeControllerDict.ContainsKey(menuType))
            {
                _menuTypeControllerDict[menuType].Hide();
            }
        }

        #endregion
    }
}
