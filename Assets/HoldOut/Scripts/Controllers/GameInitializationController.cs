using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HoldOut
{
    public class GameInitializationController : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] private List<ManagerBase> _managerList = new List<ManagerBase>();

        private void Start()
        {
            StartCoroutine(nameof(InitializeCRT));
        }

        private IEnumerator InitializeCRT()
        {
            for (int i = 0; i < _managerList.Count; i++)
            {
                _managerList[i].Initialize();
            }

            while (true)
            {
                var allReady = true;

                foreach (var manager in _managerList)
                {
                    if (!manager.Ready)
                    {
                        Debug.Log($"Manager:{manager.GetType()} not ready yet!");
                        allReady = false;
                        break;
                    }
                }

                if (allReady)
                {
                    break;
                }

                yield return new WaitForEndOfFrame();
            }

            if (EventManager.Instance != null && EventManager.Instance.Ready)
            {
                EventManager.Instance.GameStateEvents.RaiseGameInitialized();
            }
            yield break;
        }
    }
}
