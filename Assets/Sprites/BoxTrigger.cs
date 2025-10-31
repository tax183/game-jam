using UnityEngine;

public class BoxTrigger : MonoBehaviour
{
    public Animator coverAnimator;   // اسحبي Animator حق Cover هنا
    private bool hasOpened = false;  // يفتح مرة واحدة فقط

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (hasOpened) return;
        if (other.CompareTag("Player"))
        {
            coverAnimator.Play("CoverOpen", 0, 0f);
            hasOpened = true;
        }
    }
}
