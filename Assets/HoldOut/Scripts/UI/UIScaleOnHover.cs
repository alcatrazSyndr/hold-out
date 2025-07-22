using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

namespace HoldOut
{
    public class UIScaleOnHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [Header("Data")]
        [SerializeField] private float _scaleFactor = 1f;
        [SerializeField] private float _scaleTimer = 0.25f;
        [SerializeField] private AnimationCurve _scaleCurve = null;

        [Header("Components")]
        [SerializeField] private RectTransform _scaledRectTransform = null;

        [Header("Runtime")]
        [SerializeField] private IEnumerator _scaleCRT = null;

        private void OnDisable()
        {
            if (_scaleCRT != null)
            {
                StopCoroutine(_scaleCRT);
                _scaleCRT = null;
            }

            _scaledRectTransform.localScale = Vector3.one;
        }

        private void OnEnable()
        {
            if (_scaleCRT != null)
            {
                StopCoroutine(_scaleCRT);
                _scaleCRT = null;
            }

            _scaledRectTransform.localScale = Vector3.one;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (_scaleCRT != null)
            {
                StopCoroutine(_scaleCRT);
                _scaleCRT = null;
            }

            _scaleCRT = ScaleCRT(Vector3.one * _scaleFactor);
            StartCoroutine(_scaleCRT);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (_scaleCRT != null)
            {
                StopCoroutine(_scaleCRT);
                _scaleCRT = null;
            }

            _scaleCRT = ScaleCRT(Vector3.one);
            StartCoroutine(_scaleCRT);
        }

        private IEnumerator ScaleCRT(Vector3 targetScale)
        {
            var startScale = _scaledRectTransform.localScale;
            var timer = 0f;
            while (timer < _scaleTimer)
            {
                var interpolation = Mathf.Clamp01(timer / _scaleTimer);
                var currentScale = Vector3.Lerp(startScale, targetScale, _scaleCurve.Evaluate(interpolation));

                _scaledRectTransform.localScale = currentScale;

                timer += Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }

            _scaledRectTransform.localScale = targetScale;
            _scaleCRT = null;

            yield break;
        }
    }
}
