using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PrayCheck : MonoBehaviour
{
    private bool clickable = false;
    private bool isInside = false;
    [SerializeField] PrayPosition prayerPos;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "JobElement")
        {
            clickable = true;
            isInside = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "JobElement")
        {
            if (clickable)
            {
                StartCoroutine(ChangeColor(Color.red));
                prayerPos.ResetPosition();
            }
            isInside = false;
        }
    }
    void Update()
    {
        CheckClick();
    }
    private void CheckClick()
    {
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            if (!isInside)
            {
                StartCoroutine(ChangeColor(Color.red));
                prayerPos.ResetPosition();
            }
            else
            {
                StartCoroutine(ChangeColor(Color.green));
                clickable = false;
                prayerPos.prayCount += 1;
            }
        }
    }
    private IEnumerator ChangeColor(Color c)
    {
        GetComponent<Image>().color = c;
        yield return new WaitForSeconds(0.2f);
        GetComponent<Image>().color = Color.black;
    }
}
