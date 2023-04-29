using Extensions;
using LoadingScreenView;
using UnityEngine;

namespace Interactions
{
    public class UpgradeResourceGeneratorView : ViewPanel
    {
        [SerializeField] private TextView _textView;

        public void UpdateView(int amount)
        {
            _textView.SetTextFormated(amount.ToString());
        }
    }
}
