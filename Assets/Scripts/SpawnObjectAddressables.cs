using System;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.SceneManagement;
public class SpawnObjectAddressables : MonoBehaviour
{
    public static SpawnObjectAddressables Instance;

    public event EventHandler OnEnvironmentInstantiated;
    [SerializeField] private AssetReference assetReference;
    GameObject scene;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
    }
    private void Start()
    {
        LoadScene();
        SceneManager.activeSceneChanged += SceneManager_activeSceneChanged;
    }

    private void LoadScene()
    {
        assetReference.LoadAssetAsync<GameObject>().Completed +=
           (asyncOperationHandle) =>
           {
               if (asyncOperationHandle.Status == AsyncOperationStatus.Succeeded)
               {
                   Debug.Log("Scene yüklendi");
                   scene = asyncOperationHandle.Result;
               }
               else
               {
                   Debug.Log("Scene yükleme baþarýsýz! Tekrar deneniyor");
                   LoadScene();
               }
           };
    }

    private void SceneManager_activeSceneChanged(Scene arg0, Scene arg1)
    {
        if (arg1.buildIndex == 1)
        {
            Instantiate(scene);
            OnEnvironmentInstantiated?.Invoke(this, EventArgs.Empty);
        }
    }
}
