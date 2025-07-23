using UnityEngine;
using UnityEngine.SceneManagement;

namespace HoldOut
{
    public class GameManager : Manager<GameManager>
    {
        [Header("Runtime")]
        [SerializeField] private GameState _currentGameState = GameState.Initialization;
        public GameState CurrentGameState
        {
            get
            {
                return _currentGameState;
            }
        }

        #region Setup

        protected override void Setup()
        {
            base.Setup();

            if (EventManager.Instance != null && EventManager.Instance.Ready)
            {
                EventManager.Instance.GameStateEvents.OnGameInitialized += GameInitializedEventHandler;
                EventManager.Instance.GameStateEvents.OnAttemptGameStart += AttemptGameStartEventHandler;
                EventManager.Instance.GameStateEvents.OnAttemptGameResume += AttemptGameResumeEventHandler;
                EventManager.Instance.GameStateEvents.OnAttemptGameExit += AttemptGameExitEventHandler;
                EventManager.Instance.GameStateEvents.OnAttemptGameRestart += AttemptGameRestartEventHandler;

                EventManager.Instance.PlayerInputEvents.OnBackInput += PlayerBackInputEventHandler;
            }

            _isSetup = true;
        }

        #endregion

        #region Game Event Handlers

        private void GameInitializedEventHandler()
        {
            if (EventManager.Instance != null && EventManager.Instance.Ready)
            {
                EventManager.Instance.GameStateEvents.OnGameInitialized -= GameInitializedEventHandler;
            }

            if (_showDebug)
            {
                Debug.Log("Game Initialization complete!", this);
            }

            ChangeGameState(GameState.Loading);
            ChangeScene(Constants.MENU_SCENE_NAME);
        }

        private void AttemptGameStartEventHandler()
        {
            if (_currentGameState != GameState.Menu && _currentGameState != GameState.GameOver)
            {
                Debug.LogWarning($"Attempt to start game while current state is {_currentGameState}", this);
                return;
            }

            ChangeGameState(GameState.Loading);
            ChangeScene(Constants.GAME_SCENE_NAME);
        }

        private void AttemptGameResumeEventHandler()
        {
            if (_currentGameState != GameState.Pause)
            {
                return;
            }

            ChangeGameState(GameState.Game);
        }

        private void AttemptGameRestartEventHandler()
        {
            if (_currentGameState != GameState.Pause)
            {
                return;
            }

            ChangeGameState(GameState.Loading);
            ChangeScene(Constants.GAME_SCENE_NAME);
        }

        private void AttemptGameExitEventHandler()
        {
            if (_currentGameState != GameState.Pause)
            {
                return;
            }

            ChangeGameState(GameState.Loading);
            ChangeScene(Constants.MENU_SCENE_NAME);
        }

        #endregion

        #region Player Input Event Handlers

        private void PlayerBackInputEventHandler()
        {
            if (_currentGameState == GameState.Game)
            {
                ChangeGameState(GameState.Pause);
            }
            else if (_currentGameState == GameState.Pause)
            {
                ChangeGameState(GameState.Game);
            }
        }

        #endregion

        #region Scene Management

        private void ChangeScene(string sceneName)
        {
            SceneManager.activeSceneChanged += SceneManagerActiveSceneChangedEventHandler;
            SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
        }

        private void SceneManagerActiveSceneChangedEventHandler(Scene oldScene, Scene newScene)
        {
            SceneManager.activeSceneChanged -= SceneManagerActiveSceneChangedEventHandler;

            if (newScene != null)
            {
                if (newScene.name.Equals(Constants.MENU_SCENE_NAME))
                {
                    ChangeGameState(GameState.Menu);
                }
                else if (newScene.name.Equals(Constants.GAME_SCENE_NAME))
                {
                    ChangeGameState(GameState.Game);
                }
            }
        }

        #endregion

        #region Game State Management

        private void ChangeGameState(GameState newState)
        {
            if (_currentGameState == newState)
            {
                Debug.LogWarning($"Attempted changing game state from {_currentGameState} to {newState}", this);
                return;
            }

            var oldState = _currentGameState;
            _currentGameState = newState;
            if (EventManager.Instance != null && EventManager.Instance.Ready)
            {
                EventManager.Instance.GameStateEvents.RaiseGameStateChange(oldState, newState);
            }

            if (_showDebug)
            {
                Debug.Log($"Game state changed!\nOld state: {oldState}\nNew state: {newState}");
            }

            if (newState == GameState.Pause || newState == GameState.GameOver)
            {
                SetTimeScale(0f);
            }
            else
            {
                SetTimeScale(1f);
            }
        }

        #endregion

        #region Time Management

        private void SetTimeScale(float targetTimeScale)
        {
            Time.timeScale = targetTimeScale;
        }

        #endregion
    }
}
