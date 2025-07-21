using NUnit.Framework.Constraints;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace HoldOut
{
    public class GameManager : Manager<GameManager>
    {
        [Header("Runtime")]
        [SerializeField] private GameState _currentGameState = GameState.Initialization;

        [Header("Events")]
        public UnityEvent<GameState, GameState> OnGameStateChanged = new UnityEvent<GameState, GameState>();

        protected override void Setup()
        {
            base.Setup();

            _isSetup = true;
        }

        public void PostGameInitialize()
        {
            Debug.Log("Game Initialization complete! Launching menu scene!", this);

            ChangeGameState(GameState.Loading);
            SceneManager.activeSceneChanged += SceneManagerActiveSceneChangedEventHandler;
            SceneManager.LoadSceneAsync(Constants.MENU_SCENE_NAME, LoadSceneMode.Single);
        }

        private void SceneManagerActiveSceneChangedEventHandler(Scene oldScene, Scene newScene)
        {
            if (oldScene != null && oldScene.name.Equals(Constants.INITIALIZATION_SCENE_NAME) && newScene != null && newScene.name.Equals(Constants.MENU_SCENE_NAME))
            {
                ChangeGameState(GameState.Menu);
            }
        }

        private void ChangeGameState(GameState newState)
        {
            var oldState = _currentGameState;
            _currentGameState = newState;
            OnGameStateChanged?.Invoke(oldState, newState);

            Debug.Log($"Game state changed!\nOld state: {oldState}\nNew state: {newState}");
        }
    }
}
