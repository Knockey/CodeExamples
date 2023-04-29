using DG.Tweening;
using Extensions;
using UnityEngine;

namespace Interactions
{
    public class ResourceIncomeView : MonoBehaviour
    {
        [SerializeField] private float _height = 3f;
        [SerializeField, Min(0f)] private float _disableTime = 2f;
        [Header("Income text")]
        [SerializeField] private TextView _incomeText;
        [SerializeField] private string _1kSuffix = "K";

        public void Init(float incomeAmount)
        {
            if (incomeAmount < 0f)
                throw new System.ArgumentOutOfRangeException($"{nameof(incomeAmount)} can't be less, than 0! It equals {incomeAmount}");

            transform.DOMoveY(transform.position.y + _height, _disableTime * 0.95f);
            SetIncomeView(incomeAmount);

            var newColor = _incomeText.Text.color;
            newColor.a = 0f;
            _incomeText.Text.DOColor(newColor, _disableTime * 0.95f);

            Destroy(gameObject, _disableTime);
        }

        private void SetIncomeView(float valueToSet)
        {
            var currentValue = (int)valueToSet;
            var currentValueSuffix = "";
            var divider = 1000;
            var hundredDivider = 100;
            var floatPart = 0f;

            while (currentValue >= divider)
            {
                floatPart = currentValue % divider;
                floatPart /= hundredDivider;

                currentValue /= divider;
                currentValueSuffix += _1kSuffix;
            }

            var incomeText = currentValue + "." + (int)floatPart + currentValueSuffix;

            _incomeText.SetTextFormated(incomeText);
        }

    }
}
