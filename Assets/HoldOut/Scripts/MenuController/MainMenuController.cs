using UnityEngine;
using UnityEngine.UI;

namespace HoldOut
{
    public class MainMenuController : MenuController
    {
        [Header("Components")]
        [SerializeField] private Button _startGameButton = null;

        protected override void OnShow()
        {
            base.OnShow();

            _startGameButton.onClick.AddListener(StartGameButtonClickedHandler);
        }

        protected override void OnHide()
        {
            _startGameButton.onClick.RemoveListener(StartGameButtonClickedHandler);

            base.OnHide();
        }

        private void StartGameButtonClickedHandler()
        {
            if (EventManager.Instance != null && EventManager.Instance.Ready)
            {
                EventManager.Instance.GameStateEvents.RaiseAttemptGameStart();
            }
        }
    }
}