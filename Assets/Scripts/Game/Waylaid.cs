using UnityEngine;
using System.Collections.Generic;

public class Waylaid : MonoBehaviour
{
    public Map map;
    public Province selectedProvince;

    public List<WaylaidPlayer> players;

    void Awake()
    {
    }

    void Start()
    {
        InitMap();
        InitPlayers();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit) && hit.collider == map.gameObject.collider)
            {
                //Debug.Log("Mouseover at [" +  + ", " +  + "]");
                var x = (int)hit.point.x;
                var z = (int)hit.point.z;

                var prov = map.ProvinceAt(x, z);
                selectedProvince = prov;
            }
        }
    }

    // 50 fps
    void FixedUpdate()
    {

    }

    // Initialization

    void InitMap()
    {
        GameObject goMap = new GameObject("Map");
        map = goMap.AddComponent<Map>();
        map.InitMap(Resources.Load<Texture2D>("Textures/TestProvinces"));
    }

    void InitPlayers()
    {
        players = new List<WaylaidPlayer>();
        players.Add(new HumanPlayer(0, "Player 1"));
        players.Add(new AIPlayer(1, "AI 1"));

        // Tribes use negative ints as player nums to not interfere
        // with player numbers
        players.Add(new AITribe(-1, "Tribe 1"));

        map.SetProvinceOwner(map.Provinces[0], players[0]);
        map.SetProvinceOwner(map.Provinces[1], players[0]);
        map.SetProvinceOwner(map.Provinces[2], players[1]);
        map.SetProvinceOwner(map.Provinces[3], players[2]);
        
        map.UpdatePlayerProvinceMap(players);
    }

    // Utilities

    void MoveTextureTest()
    {
        var rangeTex = CreateRangeTex(11);
        GameObject.Find("TestRadius").renderer.material.mainTexture = rangeTex;
    }

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
