using UnityEngine;
using System.Collections;

public class Waylaid : MonoBehaviour
{
    // Use this for initialization
    void Start()
    {
        MoveTextureTest();
    }

    // Update is called once per frame
    void Update()
    {

    }

    // Utilities

    void MoveTextureTest()
    {
        var rangeTex = CreateRangeTex(11);
        GameObject.Find("TestRadius").renderer.material.mainTexture = rangeTex;
    }

    // Initialization

    Texture2D CreateRangeTex(int range)
    {
        var size = range * 2 - 1;
        var mid = size / 2;

        var move = new Color[size * size];
        for (int i = 0; i < size * size; i++)
            move[i] = Color.black;

        var rangeTex = new Texture2D(size, size);
        rangeTex.SetPixels(move);
        rangeTex.filterMode = FilterMode.Point;
        rangeTex.wrapMode = TextureWrapMode.Clamp;

        var vmid = new Vector2(mid, mid);
        for (int y = mid - range; y <= mid + range; y++)
        {
            for (int x = mid - range; x <= mid + range; x++)
            {
                var vcurr = new Vector2(x, y);
                if ((vmid - vcurr).magnitude < range)
                    rangeTex.SetPixel(x, y, Color.white);
            }
        }
        rangeTex.Apply();

        return rangeTex;
    }
}
