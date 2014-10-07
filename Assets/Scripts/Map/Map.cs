using UnityEngine;
using System.Collections.Generic;

public class Province
{
    public int                  Number      { get; set; }
    public HashSet<Province>    Connections { get; set; }

    public Province(int num)
    {
        Number = num;
        Connections = new HashSet<Province>();
    }
}

public class Map : MonoBehaviour
{
    public Texture2D    provinceMapTex;
    public int          mapSize;

    private List<Province>              provinces;
    private Dictionary<Color, Province> colourProvinceMapping;
    private Dictionary<int, Province>   numProvinceMapping;

    // Use this for initialization
    void Start()
    {
        InitMap(Resources.Load<Texture2D>("Textures/TestProvinces"));
    }

    // Update is called once per frame
    void Update()
    {
    }

    // Initialization

    void InitMap(Texture2D map)
    {
        provinceMapTex = map;
        mapSize = provinceMapTex.width;
        InitMesh();
        GenerateProvinceMapping();
    }

    void GenerateProvinceMapping()
    {
        int provinceNum = 0;

        provinces = new List<Province>();
        colourProvinceMapping = new Dictionary<Color, Province>();
        numProvinceMapping = new Dictionary<int, Province>();

        for (int y = 0; y < mapSize; y++)
        {
            for (int x = 0; x < mapSize; x++)
            {
                Color curr = provinceMapTex.GetPixel(x, y);
                if (!colourProvinceMapping.ContainsKey(curr))
                {
                    var province = new Province(provinceNum);
                    provinces.Add(province);
                    colourProvinceMapping[curr] = province;
                    numProvinceMapping[provinceNum] = province;

                    Debug.Log("Adding Province [" + curr + "] = " + provinceNum);

                    provinceNum++;
                }
            }
        }

        // second pass to build connections now that we have provinces set up
        for (int y = 0; y < mapSize; y++)
        {
            for (int x = 0; x < mapSize; x++)
            {
                Color curr = provinceMapTex.GetPixel(x, y);
                var province = colourProvinceMapping[curr];

                // check neighbours of curr
                for (int yy = y - 1; yy <= y + 1; yy++)
                {
                    for (int xx = x - 1; xx <= x + 1; xx++)
                    {
                        if (xx < 0 || yy < 0 || xx >= mapSize || yy >= mapSize)
                            continue;

                        Color neighbour = provinceMapTex.GetPixel(xx, yy);
                        if (neighbour != curr)
                        {
                            var connection = colourProvinceMapping[neighbour];

                            if (!province.Connections.Contains(connection))
                            {
                                province.Connections.Add(connection);
                                Debug.Log("Adding Connection between " +
                                          province.Number + ":" + 
                                          connection.Number);
                            }
                        }
                    }
                }
            }
        }
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
        filt.mesh = mesh;
        rend.material = Resources.Load<Material>("Materials/UnlitMat");
        rend.material.mainTexture = provinceMapTex;
    }
}
