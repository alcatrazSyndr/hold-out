using UnityEngine;
using UnityEngine.UI;

namespace HoldOut
{
    public class MainMenuController : MenuController
    {
        [Header("Components")]
        [SerializeField] private Button _startGameButton = null;
        [SerializeField] private Button _exitGameButton = null;

        protected override void OnShow()
        {
            base.OnShow();

            _startGameButton.onClick.AddListener(StartGameButtonClickedEventHandler);
            _exitGameButton.onClick.AddListener(ExitGameButtonClickedEventHandler);
        }

        protected override void OnHide()
        {
            _startGameButton.onClick.RemoveListener(StartGameButtonClickedEventHandler);
            _exitGameButton.onClick.RemoveListener(ExitGameButtonClickedEventHandler);

            base.OnHide();
        }

        private void StartGameButtonClickedEventHandler()
        {
            if (EventManager.Instance != null && EventManager.Instance.Ready)
            {
                EventManager.Instance.GameStateEvents.RaiseAttemptGameStart();
            }
        }

        private void ExitGameButtonClickedEventHandler()
        {
            Application.Quit();
        }
    }
}