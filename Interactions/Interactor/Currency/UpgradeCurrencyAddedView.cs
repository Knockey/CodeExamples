using DG.Tweening;
using Extensions;
using System.Collections;
using UnityEngine;

namespace Interactions.Resources
{
    public class UpgradeCurrencyAddedView : MonoBehaviour
    {
        [SerializeField] private TextView _textView;
        [SerializeField, Min(0f)] private float _delayTime = 0.25f;
        [SerializeField, Min(0f)] private float _moveTime = 0.5f;

        private void OnDestroy()
        {
            DOTween.Kill(this);
        }

        public void Init(int currency, Vector3 endPoint)
        {
            _textView.SetTextFormated(currency.ToString());
            StartCoroutine(MoveTowardsEndPoint(endPoint));
        }

        private IEnumerator MoveTowardsEndPoint(Vector3 endPoint)
        {
            yield return new WaitForSeconds(_delayTime);

            transform.DOLocalMove(endPoint, _moveTime);
            Destroy(gameObject, _moveTime * 1.1f);
        }
    }
}
