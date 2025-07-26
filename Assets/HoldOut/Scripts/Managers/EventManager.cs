using System;
using UnityEngine;

namespace HoldOut
{
    public class EventManager : Manager<EventManager>
    {
        [Header("Runtime")]
        private GameStateEvents _gameStateEvents = null;
        public GameStateEvents GameStateEvents
        {
            get
            {
                return _gameStateEvents;
            }
        }
        private PlayerInputEvents _playerInputEvents = null;
        public PlayerInputEvents PlayerInputEvents
        {
            get
            {
                return _playerInputEvents;
            }
        }
        private PlayerControllerEvents _playerControllerEvents = null;
        public PlayerControllerEvents PlayerControllerEvents
        {
            get
            {
                return _playerControllerEvents;
            }
        }

        protected override void Setup()
        {
            base.Setup();

            _playerControllerEvents = new PlayerControllerEvents();
            _gameStateEvents = new GameStateEvents();
            _playerInputEvents = new PlayerInputEvents();

            _isSetup = true;
        }
    }

    public class PlayerControllerEvents
    {
        public event Action<PlayerController> OnPlayerControllerSpawned;
        public void RaisePlayerControllerSpawned(PlayerController playerController) => OnPlayerControllerSpawned?.Invoke(playerController);

        public event Action OnPlayerControllerDestroyed;
        public void RaisePlayerControllerDestroyed() => OnPlayerControllerDestroyed?.Invoke();
    }

    public class PlayerInputEvents
    {
        public event Action OnBackInput;
        public void RaiseBackInput() => OnBackInput?.Invoke();

        public event Action<Vector2> OnMovementInputChanged;
        public void RaiseMovementInputChange(Vector2 movementInput) => OnMovementInputChanged?.Invoke(movementInput);

        public event Action<float> OnCameraScrollInputChanged;
        public void RaiseCameraScrollInputChange(float scrollInput) => OnCameraScrollInputChanged?.Invoke(scrollInput);

        public event Action<bool> OnPrimaryAttackInputChanged;
        public void RaisePrimaryAttackInputChange(bool attackInput) => OnPrimaryAttackInputChanged?.Invoke(attackInput);
    }

    public class GameStateEvents
    {
        public event Action OnGameInitialized;
        public void RaiseGameInitialized() => OnGameInitialized?.Invoke();

        public event Action<GameState, GameState> OnGameStateChanged;
        public void RaiseGameStateChange(GameState oldState, GameState newState) => OnGameStateChanged?.Invoke(oldState, newState);

        public event Action OnAttemptGameStart;
        public void RaiseAttemptGameStart() => OnAttemptGameStart?.Invoke();

        public event Action OnAttemptGameResume;
        public void RaiseAttemptGameResume() => OnAttemptGameResume?.Invoke();

        public event Action OnAttemptGameExit;
        public void RaiseAttemptGameExit() => OnAttemptGameExit?.Invoke();

        public event Action OnAttemptGameRestart;
        public void RaiseAttemptGameRestart() => OnAttemptGameRestart?.Invoke();
    }
}
