using UnityEngine;
using UnityEngine.UI;

namespace HoldOut
{
    public class PauseMenuController : MenuController
    {
        [Header("Components")]
        [SerializeField] private Button _resumeButton = null;
        [SerializeField] private Button _restartButton = null;
        [SerializeField] private Button _exitButton = null;

        protected override void OnShow()
        {
            base.OnShow();

            _resumeButton.onClick.AddListener(ResumeButtonClickedEventHandler);
            _restartButton.onClick.AddListener(RestartButtonClickedEventHandler);
            _exitButton.onClick.AddListener(ExitButtonClickedEventHandler);
        }

        protected override void OnHide()
        {
            _resumeButton.onClick.RemoveListener(ResumeButtonClickedEventHandler);
            _restartButton.onClick.RemoveListener(RestartButtonClickedEventHandler);
            _exitButton.onClick.RemoveListener(ExitButtonClickedEventHandler);

            base.OnHide();
        }

        private void ResumeButtonClickedEventHandler()
        {
            if (EventManager.Instance != null && EventManager.Instance.Ready)
            {
                EventManager.Instance.GameStateEvents.RaiseAttemptGameResume();
            }
        }

        private void RestartButtonClickedEventHandler()
        {
            if (EventManager.Instance != null && EventManager.Instance.Ready)
            {
                EventManager.Instance.GameStateEvents.RaiseAttemptGameRestart();
            }
        }

        private void ExitButtonClickedEventHandler()
        {
            if (EventManager.Instance != null && EventManager.Instance.Ready)
            {
                EventManager.Instance.GameStateEvents.RaiseAttemptGameExit();
            }
        }
    }
}