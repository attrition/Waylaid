using UnityEngine;
using System.Collections.Generic;

public class Map : MonoBehaviour
{
    public int mapSize;

    public Texture2D provinceMapTex;
    public Texture2D playerProvincesTex;
    public Texture2D provinceBorderTex;

    public List<WaylaidPlayer>  Players     { get; set; }
    public List<Province>       Provinces   { get; private set; }

    private Dictionary<Color, Province> colourProvinceMapping;
    private Dictionary<int, Province> numProvinceMapping;
    private Province[] provinceGrid;

    #region Accessors

    // shift owner of province to incoming player
    public void SetProvinceOwner(Province province, WaylaidPlayer player, bool UpdateMap = true)
    {
        WaylaidPlayer oldPlayer = province.Owner;
        if (oldPlayer != null)
            oldPlayer.Provinces.Remove(province);

        province.Owner = player;
        player.Provinces.Add(province);

        if (UpdateMap)
            UpdatePlayerProvinceMap();
    }

    public Province ProvinceAt(int x, int y)
    {
        if ((x >= 0 && y >= 0) && (x < mapSize && y < mapSize))
            return provinceGrid[y * mapSize + x];

        return null;
    }

    public WaylaidPlayer PlayerAt(int x, int y)
    {
        var province = ProvinceAt(x, y);

        if (province == null)
            return null;

        return province.Owner;
    }

    #endregion

    #region Automatic stuff

    void Awake()
    {
    }

    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {

    }

    // 50 fps
    void FixedUpdate()
    {
    }

    #endregion

    #region Manual stuff

    // Player province map updating

    public void UpdatePlayerProvinceMap()
    {
        playerProvincesTex = new Texture2D(mapSize, mapSize);
        playerProvincesTex.filterMode = FilterMode.Point;
        playerProvincesTex.wrapMode = TextureWrapMode.Clamp;

        var colourData = new Color[mapSize * mapSize];
        for (int y = 0; y < mapSize; y++)
        {
            for (int x = 0; x < mapSize; x++)
            {
                var coord = y * mapSize + x;
                var province = provinceGrid[coord];
                colourData[coord] = Color.black;

                if (province.Owner != null)
                    colourData[coord] = province.Owner.Colour;                
            }
        }

        playerProvincesTex.SetPixels(colourData);
        playerProvincesTex.Apply();
        this.gameObject.renderer.material.mainTexture = playerProvincesTex;
    }

    // Initialization

    public void InitMap(Texture2D map)
    {
        provinceMapTex = map;
        mapSize = provinceMapTex.width;

        InitMesh();
        GenerateProvinceMapping();
    }

    void GenerateProvinceMapping()
    {
        int provinceNum = 0;

        Provinces = new List<Province>();
        colourProvinceMapping = new Dictionary<Color, Province>();
        numProvinceMapping = new Dictionary<int, Province>();

        provinceGrid = new Province[mapSize * mapSize];
        for (int y = 0; y < mapSize; y++)
        {
            for (int x = 0; x < mapSize; x++)
            {
                Color curr = provinceMapTex.GetPixel(x, y);
                if (!colourProvinceMapping.ContainsKey(curr))
                {
                    var province = new Province(provinceNum);

                    Provinces.Add(province);
                    colourProvinceMapping[curr] = province;
                    numProvinceMapping[provinceNum] = province;

                    //Debug.Log("Adding Province [" + curr + "] = " + provinceNum);

                    provinceNum++;
                }
            }
        }

        // second pass to build connections now that we have provinces set up
        // also build the border map from here
        provinceBorderTex = new Texture2D(mapSize, mapSize);
        var borderTexData = new Color[mapSize * mapSize];
        provinceBorderTex.wrapMode = TextureWrapMode.Clamp;

        // default texture to all white
        for (int i = 0; i < mapSize * mapSize; i++)
            borderTexData[i] = Color.white;

        for (int y = 0; y < mapSize; y++)
        {
            for (int x = 0; x < mapSize; x++)
            {
                Color curr = provinceMapTex.GetPixel(x, y);

                var province = colourProvinceMapping[curr];
                var coord = y * mapSize + x;
                provinceGrid[coord] = province;

                //// check neighbours of curr
                for (int yy = y - 1; yy <= y + 1; yy++)
                {
                    for (int xx = x - 1; xx <= x + 1; xx++)
                    {
                        if (xx < 0 || yy < 0 || xx >= mapSize || yy >= mapSize)
                            continue;

                        Color neighbour = provinceMapTex.GetPixel(xx, yy);
                        if (neighbour != curr)
                        {
                            borderTexData[coord] = Color.black;

                            var connection = colourProvinceMapping[neighbour];

                            if (!province.Connections.Contains(connection))
                            {
                                province.Connections.Add(connection);
                                //Debug.Log("Adding Connection between " +
                                //          province.Number + ":" +
                                //          connection.Number);
                            }
                        }
                    }
                }
            }
        }

        provinceBorderTex.SetPixels(borderTexData);
        provinceBorderTex.Apply();

        this.gameObject.renderer.material.SetTexture("_BorderTex", provinceBorderTex);
    }

    void InitMesh()
    {
        var mesh = new Mesh();
        mesh.name = "MapMesh";

        mesh.vertices = new[] {
            new Vector3(0, 0, 0),
            new Vector3(0, 0, mapSize),
            new Vector3(mapSize, 0, mapSize),
            new Vector3(mapSize, 0, 0),
        };

        mesh.triangles = new[] {
            0, 1, 2, 2, 3, 0
        };

        mesh.uv = new[] {
            new Vector2(0, 0),
            new Vector2(0, 1),
            new Vector2(1, 1),
            new Vector2(1, 0),
        };

        mesh.RecalculateNormals();

        var rend = this.gameObject.AddComponent<MeshRenderer>();
        var filt = this.gameObject.AddComponent<MeshFilter>();
        var coll = this.gameObject.AddComponent<MeshCollider>();

        filt.sharedMesh = mesh;
        coll.sharedMesh = mesh;

        rend.material = Resources.Load<Material>("Materials/Terrain");
        rend.material.mainTexture = provinceMapTex;
    }

    #endregion
}
