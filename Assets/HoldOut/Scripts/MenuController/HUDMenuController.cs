using TMPro;
using Unity.Entities;
using UnityEngine;

namespace HoldOut
{
    public class HUDMenuController : MenuController
    {
        [Header("Components")]
        [SerializeField] private TextMeshProUGUI _statsText = null;

        [Header("Runtime")]
        [SerializeField] private float _deltaTime = 0f;
        [SerializeField] private int _currentFrameStep = 0;
        [SerializeField] private int _maxFrameStep = 3;
        [SerializeField] private float _lastFPS = 0f;

        protected override void OnShow()
        {
            base.OnShow();
        }

        protected override void OnHide()
        {
            base.OnHide();
        }

        private void Update()
        {
            if (!_isActive)
            {
                return;
            }

            // Calculate FPS
            _deltaTime += (Time.unscaledDeltaTime - _deltaTime) * 0.1f;
            float fps = _lastFPS;
            _currentFrameStep++;
            if (_currentFrameStep >= _maxFrameStep)
            {
                fps = 1.0f / _deltaTime;
                _currentFrameStep = 0;
                _lastFPS = fps;
            }

            // Get enemy count using ECS
            var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            EntityQuery query = entityManager.CreateEntityQuery(typeof(HoldOut.EnemyTag));
            int enemyCount = query.CalculateEntityCount();

            // Update UI text
            _statsText.text = $"Frames: {Mathf.CeilToInt(fps)}\nEnemies: {enemyCount}";
        }
    }
}