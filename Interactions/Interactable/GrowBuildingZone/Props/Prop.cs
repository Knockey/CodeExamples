using UnityEngine;

[RequireComponent(typeof(Animator))]
public class Prop : MonoBehaviour 
{
    public void Enable()
    {
        gameObject.SetActive(true);
    }

    public void Disable()
    {
        gameObject.SetActive(false);
    }
}
