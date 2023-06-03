using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class MovePiece : MonoBehaviour, IPointerDownHandler
{
    string pieceState = "idle";
    int clickCount = 0;
    [SerializeField] ParticleSystem particle;
    Vector2 startPos;
    bool triggered;
    // Start is called before the first frame update
    void Start()
    {
        startPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (pieceState != "placed" && clickCount == 1)
        {
            Vector2 mousePosition = new Vector2(Mouse.current.position.ReadValue().x, Mouse.current.position.ReadValue().y);
            transform.position = mousePosition;
        }


    }
    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.name == gameObject.name + "Place" && clickCount == 2)
        {
            GetComponent<BoxCollider2D>().enabled = false;
            transform.position = other.gameObject.transform.position;
            particle.transform.localPosition = other.gameObject.transform.localPosition;
            particle.Play();
            FixBridge.pieceCount += 1;
            pieceState = "placed";
            triggered = true;
        }
        else if (other.gameObject.name != gameObject.name + "Place" && clickCount == 2 && !triggered)
        {
            clickCount = 0;
            transform.position = startPos;
            pieceState = "idle";
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if(other.gameObject.name == gameObject.name + "Place")
        {
            triggered = false;
        }
    }
    public void OnPointerDown(PointerEventData pointerEventData)
    {
        if (clickCount < 2)
            clickCount += 1;
    }

}
