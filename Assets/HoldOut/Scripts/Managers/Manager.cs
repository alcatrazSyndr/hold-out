using UnityEngine;

namespace HoldOut
{
    public abstract class ManagerBase : MonoBehaviour
    {
        public abstract void Initialize();
    }

    public class Manager<T> : ManagerBase where T : MonoBehaviour
    {
        protected static T _instance = null;
        public static T Instance 
        {
            get
            {
                return _instance;
            }
        }
        protected static bool _isInitialized = false;
        protected static bool _isSetup = false;
        public static bool Ready
        {
            get
            {
                return _instance != null && _isInitialized && _isSetup; 
            }
        }

        public override void Initialize()
        {
            if (_instance == null)
            {
                _instance = this as T;
                DontDestroyOnLoad(gameObject);
                Setup();
            }
            else
            {
                _instance = null;
                DestroyImmediate(gameObject);
            }
        }

        protected virtual void Setup()
        {
            Debug.Log($"Manager of type:{GetType()} succesfully initialized!", this);
        }
    }
}
