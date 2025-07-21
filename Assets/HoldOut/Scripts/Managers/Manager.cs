using UnityEngine;

namespace HoldOut
{
    public abstract class ManagerBase : MonoBehaviour
    {
        public virtual bool Ready => false;

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
        protected bool _isInitialized = false;
        protected bool _isSetup = false;
        public override bool Ready => _instance != null && _isInitialized && _isSetup;

        public override void Initialize()
        {
            if (_instance == null)
            {
                _instance = this as T;
                DontDestroyOnLoad(gameObject);
                _isInitialized = true;
                Setup();
            }
            else
            {
                Debug.LogWarning($"Duplicate manager of type:{GetType()} found! Destroying newest!", this);
                DestroyImmediate(gameObject);
            }
        }

        protected virtual void Setup()
        {
            //Debug.Log($"Manager of type:{GetType()} succesfully initialized!", this);
        }
    }
}
