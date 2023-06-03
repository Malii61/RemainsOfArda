using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ShufflePieces : MonoBehaviour
{
    List<Vector2> piecesPos = new List<Vector2>();
    void Start()
    {
        for (int i = 3; i < 10; i++)
        {
            piecesPos.Add(transform.GetChild(i).GetChild(0).position);
        }
        var shuffledPos = piecesPos.OrderBy(a => Random.Range(0, 7)).ToList();
        for (int i = 3; i < 10; i++)
        {
            transform.GetChild(i).GetChild(0).position = shuffledPos[i - 3];
        }
    }
}
