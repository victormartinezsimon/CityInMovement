using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.SceneManagement;

public class Generator : MonoBehaviour
{

    #region singleton
    private static Generator m_generator = null;
    public static Generator getInstance()
    {
        return m_generator;
    }
    void Awake()
    {
        if (m_generator != this && m_generator != null)
        {
            Destroy(this.gameObject);
        }
        m_generator = this;
        DontDestroyOnLoad(this.gameObject);
    }
    #endregion

    #region city variables
    public int m_tamBoard;

    public int TamBoard
    {
        get { return m_tamBoard; }
        set { m_tamBoard = value; }
    }
    public bool withSemaphores = true;
    #endregion

    #region city generator
    private CityGenerator m_cityGenerator;
    private int[,] m_city;
    private GameObject[,] m_gameObjects;
    #endregion

    #region prefabs
    [Header("GameObjects city")]
    public GameObject tile0;
    public GameObject tile20;
    public GameObject tile21;
    public GameObject tile22;
    public GameObject tile23;
    public GameObject tile24;
    public GameObject tile25;
    public GameObject tile30;
    public GameObject tile31;
    public GameObject tile32;
    public GameObject tile33;
    public GameObject tile40;

    #endregion

    #region cars
    [Header("Car variables")]
    public GameObject m_car;
    public int m_totalCars;
    #endregion
    private GameObject m_parent = null;
    private GameObject m_parentCar = null;
    public bool debugMode = false;
    

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene("Menu");
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            debugMode = !debugMode;
        }
        drawDebug();
    }
    #region city
    public void GenerateCity()
    {
        m_cityGenerator = new CityGenerator ((int)DateTime.Now.Ticks, TamBoard, TamBoard);
        m_cityGenerator.Build();
        m_city = m_cityGenerator.getCity();
        instantiatePrefabs();
        generateIA();
        instantiateCars();
        colocateCamera();
    }
    private void instantiatePrefabs()
    {
        destroyPreviousPrefabs();
        m_parent = new GameObject("parent");
        m_gameObjects = new GameObject[TamBoard, TamBoard];
        float posX = 0;
        float posY = 0;

        Vector3 size = tile0.GetComponent<Renderer>().bounds.size;

        for (int i = 0; i < TamBoard; i++)
        {
            for (int j = 0; j < TamBoard; j++)
            {
                int tile = m_city[i, j];
                GameObject goInstantiated = instantiateSpecificPrefab(tile, posX, posY);
                goInstantiated.name += "[" + i + "," + j + "]";
                m_gameObjects[i, j] = goInstantiated;
                goInstantiated.transform.parent = m_parent.transform;
                posY += size.y;
            }
            posY = 0;
            posX += size.x;
        }
    }
    private GameObject instantiateSpecificPrefab(int tile, float posX, float posY)
    {
        GameObject m_gameObject = null;

        switch (tile)
        {
            case -1: m_gameObject = tile0; break;
            case 0: m_gameObject = tile0; break;
            case 20: m_gameObject = tile20; break;
            case 21: m_gameObject = tile21; break;
            case 22: m_gameObject = tile22; break;
            case 23: m_gameObject = tile23; break;
            case 24: m_gameObject = tile24; break;
            case 25: m_gameObject = tile25; break;
            case 30: m_gameObject = tile30; break;
            case 31: m_gameObject = tile31; break;
            case 32: m_gameObject = tile32; break;
            case 33: m_gameObject = tile33; break;
            case 40: m_gameObject = tile40; break;
            case 41: m_gameObject = tile40; break;
        }

        m_gameObject = Instantiate(m_gameObject) as GameObject;
        m_gameObject.transform.position = new Vector3(posX, posY, 0);
        m_gameObject.name = tile.ToString();
        return m_gameObject;
    }
    private void destroyPreviousPrefabs()
    {
        if (m_parent != null)
        {
            Destroy(m_parent);
        }
        if (m_parentCar != null)
        {
            Destroy(m_parentCar);
        }
    }
    #endregion
    #region ia
    private void generateIA()
    {
        //step1-> add all nodes to the ia
        IAManager.getInstance().reset();
        for (int i = 0; i < TamBoard; i++)
        {
            for (int j = 0; j < TamBoard; j++)
            {
                GameObject go = m_gameObjects[i, j];
                IATile tile = go.GetComponent<IATile>();
                if (tile != null)
                {
                    tile.tileNumber = new int[tile.m_checkpoints.Length];
                    for (int node = 0; node < tile.m_checkpoints.Length; node++)
                    {
                        int id = IAManager.getInstance().AddNode(tile.m_checkpoints[node].transform.position);
                        tile.tileNumber[node] = id;
                        tile.m_checkpoints[node].name = id.ToString();
                    }

                    IATrafficLaws laws = go.GetComponent<IATrafficLaws>();
                    if (laws != null)
                    {
                        for(int lawIndex = 0; lawIndex < laws.m_avoidRoutes.Length; lawIndex++)
                        {
                            IATrafficLaws.avoid a = laws.m_avoidRoutes[lawIndex];
                            List<Vector3> toAdd = new List<Vector3>();
                            for(int avoidIndex = 0; avoidIndex < a.m_avoid.Length; avoidIndex++)
                            {
                                int index = a.m_avoid[avoidIndex];
                                Vector3 realPosition = tile.m_checkpoints[index].transform.position;
                                toAdd.Add(realPosition);
                            }
                            IAManager.getInstance().addBannedNodes(toAdd);
                        }
                    }
                }
            }
        }
        //step2-> add the conexions
        for (int i = 0; i < TamBoard; i++)
        {
            for (int j = 0; j < TamBoard; j++)
            {
                GameObject go = m_gameObjects[i, j];
                IATile tile = go.GetComponent<IATile>();
                if (tile != null)
                {
                    tile.buildFromArray();
                    int tileType = m_city[i, j];

                    for (int nodei = 0; nodei < tile.m_checkpoints.Length; nodei++)
                    {
                        for (int nodej = 0; nodej < tile.m_checkpoints.Length; nodej++)
                        {
                            if (tile.m_ways[nodei, nodej])
                            {
                                IAManager.getInstance().addConexion(tile.tileNumber[nodei], tile.tileNumber[nodej]);
                            }
                        }
                    }
                    addExternalConexion(i, j, tile);
                }
            }
        }
        IAManager.getInstance().constructionEnded();
    }
    private void addExternalConexion(int i, int j, IATile tile)
    {
        int tileType = m_city[i, j];
        IATile north = m_gameObjects[i, j + 1].GetComponent<IATile>();
        IATile south = m_gameObjects[i, j - 1].GetComponent<IATile>();
        IATile east = m_gameObjects[i + 1, j].GetComponent<IATile>();
        IATile west = m_gameObjects[i - 1, j].GetComponent<IATile>();

        switch (tileType)
        {
            case 20:
                IAManager.getInstance().addConexion(tile.tileNumber[2], north.tileNumber[north.entranceSouth]);
                IAManager.getInstance().addConexion(tile.tileNumber[5], south.tileNumber[south.entranceNorth]);
                break;
            case 21:
                IAManager.getInstance().addConexion(tile.tileNumber[2], west.tileNumber[west.entranceEast]);
                IAManager.getInstance().addConexion(tile.tileNumber[5], east.tileNumber[east.entranceWest]);
                break;

            case 22:
                IAManager.getInstance().addConexion(tile.tileNumber[2], south.tileNumber[south.entranceNorth]);
                IAManager.getInstance().addConexion(tile.tileNumber[7], west.tileNumber[west.entranceEast]);
                break;

            case 23:
                IAManager.getInstance().addConexion(tile.tileNumber[2], west.tileNumber[west.entranceEast]);
                IAManager.getInstance().addConexion(tile.tileNumber[7], north.tileNumber[north.entranceSouth]);
                break;

            case 24:
                IAManager.getInstance().addConexion(tile.tileNumber[2], north.tileNumber[north.entranceSouth]);
                IAManager.getInstance().addConexion(tile.tileNumber[7], east.tileNumber[east.entranceWest]);
                break;

            case 25:
                IAManager.getInstance().addConexion(tile.tileNumber[2], east.tileNumber[east.entranceWest]);
                IAManager.getInstance().addConexion(tile.tileNumber[7], south.tileNumber[south.entranceNorth]);
                break;

            case 30:
                IAManager.getInstance().addConexion(tile.tileNumber[2], east.tileNumber[east.entranceWest]);
                IAManager.getInstance().addConexion(tile.tileNumber[6], west.tileNumber[west.entranceEast]);
                IAManager.getInstance().addConexion(tile.tileNumber[9], south.tileNumber[south.entranceNorth]);
                break;

            case 31:
                IAManager.getInstance().addConexion(tile.tileNumber[2], north.tileNumber[north.entranceSouth]);
                IAManager.getInstance().addConexion(tile.tileNumber[6], south.tileNumber[south.entranceNorth]);
                IAManager.getInstance().addConexion(tile.tileNumber[9], east.tileNumber[east.entranceWest]);
                break;

            case 32:
                IAManager.getInstance().addConexion(tile.tileNumber[2], west.tileNumber[west.entranceEast]);
                IAManager.getInstance().addConexion(tile.tileNumber[6], east.tileNumber[east.entranceWest]);
                IAManager.getInstance().addConexion(tile.tileNumber[9], north.tileNumber[north.entranceSouth]);
                break;

            case 33:
                IAManager.getInstance().addConexion(tile.tileNumber[2], south.tileNumber[south.entranceNorth]);
                IAManager.getInstance().addConexion(tile.tileNumber[6], north.tileNumber[north.entranceSouth]);
                IAManager.getInstance().addConexion(tile.tileNumber[9], west.tileNumber[west.entranceEast]);
                break;

            case 40:
                IAManager.getInstance().addConexion(tile.tileNumber[2], east.tileNumber[east.entranceWest]);
                IAManager.getInstance().addConexion(tile.tileNumber[5], north.tileNumber[north.entranceSouth]);
                IAManager.getInstance().addConexion(tile.tileNumber[8], west.tileNumber[west.entranceEast]);
                IAManager.getInstance().addConexion(tile.tileNumber[11], south.tileNumber[south.entranceNorth]);
                break;
        }


    }
    #endregion
    #region car
    private void instantiateCars()
    {
        m_parentCar = new GameObject("parentCars");
        
        for (int i = 0; i < m_totalCars; i++)
        {
          
            GameObject go = Instantiate(m_car);
            Vector3 position;
            int id;
            IAManager.getInstance().getInitialPosition(out position, out id);
            position.z = -2;
            go.transform.position = position;
            Movement mv = go.GetComponent<Movement>();
            if (mv != null)
            {
                mv.m_destinyID = id;
            }
            go.transform.parent = m_parentCar.transform;

        }
    }
    #endregion

    private void colocateCamera()
    {
        Vector3 size = tile0.GetComponent<Renderer>().bounds.size;
        Vector3 newpos = new Vector3(size.x * TamBoard / 2, size.y * TamBoard / 2, -10);
        Camera.main.transform.position = newpos;

        CameraController cc = Camera.main.GetComponent<CameraController>();
        if(cc != null)
        {
            float[] limit = new float[4];
            limit[0] = 0;
            limit[1] = size.x * TamBoard - size.x / 2;
            limit[2] = 0;
            limit[3] = size.y * TamBoard - size.y / 2;
            cc.limitValues = limit;
        }
    }

    void drawDebug()
    {
        if (!debugMode) { return; }
        IAManager mngr = IAManager.getInstance();

        for (int i = 0; i < mngr.listNodes.Count; i++)
        {
            IAManager.GraphNode n = mngr.listNodes[i];
            for (int destiny = 0; destiny < n.m_nexts.Count; destiny++)
            {
                int id = n.m_nexts[destiny];
                Vector3 origin = n.m_position;
                Vector3 dst = mngr.listNodes[id].m_position;
                Debug.DrawLine(origin, dst, Color.red);
            }
        }
    }
}
