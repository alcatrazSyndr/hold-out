using UnityEngine;

namespace HoldOut
{
    public class MenuController : MonoBehaviour
    {
        [Header("Data")]
        [SerializeField] private MenuType _menuType = MenuType.Null;
        public MenuType MenuType
        {
            get
            {
                return _menuType;
            }
        }

        [Header("Components")]
        [SerializeField] private Canvas _menuCanvas = null;

        [Header("Runtime")]
        [SerializeField] protected bool _isActive = false;

        public void Show()
        {
            if (_isActive)
            {
                return;
            }

            _menuCanvas.gameObject.SetActive(true);
            _menuCanvas.enabled = true;
            _isActive = true;

            OnShow();
        }

        protected virtual void OnShow()
        {

        }

        public void Hide()
        {
            if (!_isActive)
            {
                return;
            }

            _menuCanvas.gameObject.SetActive(false);
            _menuCanvas.enabled = false;
            _isActive = false;

            OnHide();
        }

        protected virtual void OnHide()
        {

        }
    }
}
