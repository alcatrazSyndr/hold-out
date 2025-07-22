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

        #region Setup / Game Initialization

        protected override void Setup()
        {
            base.Setup();

            if (EventManager.Instance != null && EventManager.Instance.Ready)
            {
                EventManager.Instance.GameStateEvents.OnGameInitialized += OnGameInitialized;
            }

            _isSetup = true;
        }

        private void OnGameInitialized()
        {
            if (EventManager.Instance != null && EventManager.Instance.Ready)
            {
                EventManager.Instance.GameStateEvents.OnGameInitialized -= OnGameInitialized;
            }

            Debug.Log("Game Initialization complete!", this);

            ChangeGameState(GameState.Loading);
            ChangeScene(Constants.MENU_SCENE_NAME);
        }

        #endregion

        #region Scene Management

        private void ChangeScene(string sceneName)
        {
            SceneManager.activeSceneChanged += SceneManagerActiveSceneChangedEventHandler;
            SceneManager.LoadSceneAsync(Constants.MENU_SCENE_NAME, LoadSceneMode.Single);
        }

        private void SceneManagerActiveSceneChangedEventHandler(Scene oldScene, Scene newScene)
        {
            SceneManager.activeSceneChanged -= SceneManagerActiveSceneChangedEventHandler;

            if (newScene != null && newScene.name.Equals(Constants.MENU_SCENE_NAME))
            {
                ChangeGameState(GameState.Menu);
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

            Debug.Log($"Game state changed!\nOld state: {oldState}\nNew state: {newState}");
        }

        #endregion
    }
}
