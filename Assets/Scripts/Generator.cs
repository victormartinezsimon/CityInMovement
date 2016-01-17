using UnityEngine;
using System.Collections;


public class Generator : MonoBehaviour {

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
    public int m_width;
    public int m_height;

    public int Width
    {
        get { return m_width; }
        set { m_width = value; }
    }
    public int Height
    {
        get { return m_height; }
        set { m_height = value; }
    }

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

    private GameObject m_parent = null;

    // Use this for initialization
    void Start () {
        
    }
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.G))
        {
            GenerateCity();
        }
	}

    public void GenerateCity()
    {
        m_cityGenerator = new CityGenerator( 10, m_width, m_height);
        m_cityGenerator.Build();
        m_city = m_cityGenerator.getCity();
        instantiatePrefabs();
        generateIA();
    }
    private void instantiatePrefabs()
    {
        destroyPreviousPrefabs();
        m_parent = new GameObject("parent");
        m_gameObjects = new GameObject[m_width, m_height];
        float posX = 0;
        float posY = 0;

        Vector3 size = tile0.GetComponent<Renderer>().bounds.size;

        for (int i = 0; i < m_width; i++)
        {
            for(int j = 0; j < m_height; j++)
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
            case 22: m_gameObject = tile22;  break;
            case 23: m_gameObject = tile23;  break;
            case 24: m_gameObject = tile24; break;
            case 25: m_gameObject = tile25; break;
            case 30: m_gameObject = tile30;  break;
            case 31: m_gameObject = tile31;  break;
            case 32: m_gameObject = tile32;  break;
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
        if(m_parent != null)
        {
            Destroy(m_parent);
        }
    }

    private void generateIA()
    {
        //step1-> add all nodes to the ia
        IAMaager.getInstance().reset();
        for(int i = 0; i < m_width; i++)
        {
            for(int j = 0; j < m_height; j++)
            {
                GameObject go = m_gameObjects[i, j];
                IATile tile = go.GetComponent<IATile>();
                if(tile != null)
                {
                    tile.tileNumber = new int[tile.m_checkpoints.Length];
                    for(int node = 0; node < tile.m_checkpoints.Length; node++)
                    {
                        int id = IAMaager.getInstance().AddNode(go.transform);
                        tile.tileNumber[node] = id;
                    }
                }
            }
        }
        //step2-> add the conexions

    }
}
