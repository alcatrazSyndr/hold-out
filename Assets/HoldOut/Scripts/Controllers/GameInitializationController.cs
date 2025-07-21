using System.Collections.Generic;
using UnityEngine;

namespace HoldOut
{
    public class GameInitializationController : MonoBehaviour
    {
        [Header("Data")]
        [SerializeField] private float _managerInitializationDelayTimer = 1f;

        [Header("Components")]
        [SerializeField] private List<ManagerBase> _managerList = new List<ManagerBase>();

        private void Start()
        {
            Invoke(nameof(InitializeManagers), _managerInitializationDelayTimer);
        }

        private void InitializeManagers()
        {
            for (int i = 0; i < _managerList.Count; i++)
            {
                _managerList[i].Initialize();
            }
        }
    }
}
