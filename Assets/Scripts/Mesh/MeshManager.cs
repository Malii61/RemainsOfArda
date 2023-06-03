using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MeshManager : MonoBehaviour
{
    [SerializeField] Image mesh;
    [SerializeField] Sprite[] meshImages;
    private int order = 0;
    public static string meshName;
    private void Start()
    {
        meshName = meshImages[order].name;
    }
    public void onClickLeftArrow()
    {
        if (order == 0)
        {
            mesh.sprite = meshImages[meshImages.Length - 1];
            order = meshImages.Length - 1;
        }
        else
        {
            mesh.sprite = meshImages[order - 1];
            order--;
        }
        meshName = meshImages[order].name;

    }
    public void onClickRightArrow()
    {
        if (order == meshImages.Length - 1)
        {
            mesh.sprite = meshImages[0];
            order = 0;
        }
        else
        {
            mesh.sprite = meshImages[order + 1];
            order++;
        }
        meshName = meshImages[order].name;
    }
}
