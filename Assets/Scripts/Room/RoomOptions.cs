using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;

public class RoomOptions : MonoBehaviour
{
    private float dayLong = 4;
    private float nightLong = 8;
    public static int maxPlayer = 10;
    public static bool isRoomPublic = true;
    [SerializeField] Button publicButton;
    [SerializeField] Button privateButton;
    PhotonView PV;
    // Start is called before the first frame update
    void Start()
    {
        PV = GetComponent<PhotonView>();
        TimeSystem.dayTime = 0.01f;
        TimeSystem.nightTime = 2;
        PV.RPC(nameof(RPC_SetDayLong), RpcTarget.Others, dayLong, nightLong);
        publicButton.image.color = ColorBlock.defaultColorBlock.selectedColor;
        privateButton.image.color = ColorBlock.defaultColorBlock.disabledColor;
    }
    public void OnSliderValueChanged(TextMeshProUGUI sliderValue)
    {
        //setting the night and day long depending on the slider value
        Slider slider = sliderValue.GetComponentInParent<Slider>();
        sliderValue.text = slider.value.ToString();
        if (slider.transform.name == "DayLongSlider")
        {
            TimeSystem.dayTime = slider.value;
            dayLong = slider.value;
            PV.RPC(nameof(RPC_SetDayLong), RpcTarget.Others, dayLong, nightLong);
        }
        else if (slider.transform.name == "NightLongSlider")
        {
            TimeSystem.nightTime = slider.value;
            nightLong = slider.value;
            PV.RPC(nameof(RPC_SetDayLong), RpcTarget.Others, dayLong, nightLong);
        }
        else
            maxPlayer = (int)slider.value;
    }
    void RPC_SetDayLong(float dayLong, float nightLong)
    {
        TimeSystem.dayTime = dayLong;
        TimeSystem.nightTime = nightLong;
    }
    public void OnClick_RoomTypeButton(Button otherButton)
    {
        otherButton.image.color = ColorBlock.defaultColorBlock.disabledColor;
        if (otherButton.name == "Private")
        {
            publicButton.image.color = ColorBlock.defaultColorBlock.selectedColor;
            isRoomPublic = true;
        }
        else
        {
            privateButton.image.color = ColorBlock.defaultColorBlock.selectedColor;
            isRoomPublic = false;
        }

    }
}
