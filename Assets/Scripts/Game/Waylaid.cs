using UnityEngine;
using System.Collections.Generic;

public class Waylaid : MonoBehaviour
{
    public Map map;
    public int selectedProvince;

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
        if (Input.GetMouseButtonDown(0))
        {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit) && hit.collider == map.gameObject.collider)
            {
                //Debug.Log("Mouseover at [" +  + ", " +  + "]");
                var x = (int)hit.point.x;
                var z = (int)hit.point.z;

                var prov = map.ProvinceAt(x, z);
                selectedProvince = prov.Number;

                // cycle province ownership via click
                var nextPlayer = 0;
                if (prov.Owner != null)
                    nextPlayer = (prov.Owner.Number + 1) % players.Count;
                
                map.SetProvinceOwner(prov, players[nextPlayer]);
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
        map.InitMap(Resources.Load<Texture2D>("Textures/UK"));

        var camPos = Camera.main.transform.position;
        camPos.x = map.mapSize / 2f;
        //camPos.z = map.mapSize / 2f;
        Camera.main.transform.position = camPos;
        //Camera.main.orthographicSize = map.mapSize / 2f;
    }

    void InitPlayers()
    {
        players = new List<WaylaidPlayer>();
        players.Add(new HumanPlayer(0, "Player 1"));
        players.Add(new AIPlayer(1, "AI 1"));
        players.Add(new AIPlayer(2, "AI 2"));
        players.Add(new AIPlayer(3, "AI 3"));

        // Tribes use negative ints as player nums to not interfere
        // with player numbers
        players.Add(new AITribe(-1, "Tribe 1"));

        // set maps players reference
        map.Players = players;

        // randomize players
        foreach (var province in map.Provinces)
            if (province.Number != 0) // 18 for EuropeLarge
                map.SetProvinceOwner(province, players[Random.Range(0, players.Count)], false);

        map.UpdatePlayerProvinceMap();
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
