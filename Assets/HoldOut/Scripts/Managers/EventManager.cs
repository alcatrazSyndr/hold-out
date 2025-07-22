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

        protected override void Setup()
        {
            base.Setup();

            _gameStateEvents = new GameStateEvents();

            _isSetup = true;
        }
    }

    public class GameStateEvents
    {
        public event Action OnGameInitialized;
        public void RaiseGameInitialized() => OnGameInitialized?.Invoke();

        public event Action<GameState, GameState> OnGameStateChanged;
        public void RaiseGameStateChange(GameState oldState, GameState newState) => OnGameStateChanged?.Invoke(oldState, newState);
    }
}
