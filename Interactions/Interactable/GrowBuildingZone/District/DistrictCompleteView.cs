using Extensions;
using LoadingScreenView;
using UnityEngine;

namespace Interactions
{
    public class DistrictCompleteView : ViewPanel
    {
        [SerializeField] private TextView _valueText;

        public void Enable(int value)
        {
            base.EnableView();
            _valueText.SetTextFormated(value.ToString());
        }
    }
}
