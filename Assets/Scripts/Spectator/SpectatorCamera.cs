using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class SpectatorCamera : MonoBehaviourPunCallbacks
{
    #region movement
    private float spectatorMoveSpeed = 10f;
    #endregion
    [SerializeField] Transform spectatorGhost;
    Vector3 moveAmount;
    private bool isSpectator = false;
    private bool isTransformSetted = false;
    Camera cam;
    Vector3 lastDiePosition = new Vector3(0, 0, 0);
    bool checkRespawn = false;
    // Start is called before the first frame update
    void Start()
    {
        cam = GetComponent<Camera>();
        cam.enabled = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        if (changedProps.ContainsKey("alive") && targetPlayer == PhotonNetwork.LocalPlayer)
        {
            if (!(bool)changedProps["alive"])
            {
                OnSpectatorEnabled();
                checkRespawn = true;
            }
            else if ((bool)changedProps["alive"] && checkRespawn)
            {
                OnSpectatorDisabled();
            }
        }
    }

    private void OnSpectatorEnabled()
    {
        isSpectator = true;
        isTransformSetted = false;
        cam.enabled = true;
        GetComponent<AudioListener>().enabled = true;
        PlayerNicknameVisibility(false);
        foreach (GameObject rolePrefab in GameObject.FindGameObjectsWithTag("RolePrefab"))
        {
            rolePrefab.SetActive(false);
        }
    }

    private void OnSpectatorDisabled()
    {
        isSpectator = false;
        cam.enabled = false;
        GetComponent<AudioListener>().enabled = false;
        PlayerNicknameVisibility(true);
    }
    private void PlayerNicknameVisibility(bool visible)
    {
        foreach (var username in FindObjectsOfType<UsernameDisplay>())
        {
            username.transform.GetChild(0).gameObject.SetActive(visible);
        }
    }

    private void LateUpdate()
    {
        if (!isSpectator)
            return;
        SetGhostPosition();
        Movement();
    }

    private void Movement()
    {
        Vector2 movement = GameInput.Instance.GetMovementVectorNormalized();
        Vector3 moveDir = new Vector3(movement.x, 0, movement.y);
        moveDir = cam.transform.forward * moveDir.z + cam.transform.right * moveDir.x;
        if (GameInput.Instance.GetInputActions().Player.Sprint.IsPressed())
        {
            moveAmount = moveDir * spectatorMoveSpeed * 5 * Time.deltaTime;
        }
        else
        {
            moveAmount = moveDir * spectatorMoveSpeed * Time.deltaTime;
        }
        spectatorGhost.position += moveAmount;

        Quaternion rotation = Quaternion.Euler(0f, transform.eulerAngles.y * 5, 0f);
        spectatorGhost.rotation = Quaternion.Lerp(spectatorGhost.rotation, rotation, Time.deltaTime * 4f);

        Vector3 pos = spectatorGhost.position;
        pos.y = Mathf.Clamp(spectatorGhost.position.y, 5, 40);
        spectatorGhost.position = pos;
    }

    private void SetGhostPosition()
    {
        if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue("diePosition", out object value) && !isTransformSetted)
        {
            if ((Vector3)value != lastDiePosition)
            {
                spectatorGhost.position = (Vector3)value;
                isTransformSetted = true;
                lastDiePosition = spectatorGhost.position;
            }
        }
    }
}
