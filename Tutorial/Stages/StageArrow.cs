using UnityEngine;

namespace Tutorial
{
    public class StageArrow : MonoBehaviour
    {
        public void EnableArrow()
        {
            gameObject.SetActive(true);
        }

        public void DisableArrow()
        {
            gameObject.SetActive(false);
        }
    }
}
