using UnityEngine;
using UnityEngine.EventSystems;

public class PrayPosition : MonoBehaviour
{
    internal int prayCount = 0;
    internal int sumOfPrays;
    int screenWidth;
    internal Vector3 startPos;
    internal void Init()
    {
        screenWidth = Screen.width;
        transform.position = new Vector2(screenWidth, transform.position.y);
        startPos = transform.position;
        sumOfPrays = transform.childCount;
        EventSystem.current.SetSelectedGameObject(gameObject);
    }
    internal void ResetPosition()
    {
        transform.position = startPos;
        prayCount = 0;
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        transform.position = new Vector2(transform.position.x - screenWidth * Time.fixedDeltaTime, transform.position.y);
    }
}
