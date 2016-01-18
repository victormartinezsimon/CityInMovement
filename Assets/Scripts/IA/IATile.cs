using UnityEngine;
using System.Collections;

public class IATile : MonoBehaviour {

    public GameObject[] m_checkpoints;
    public bool[] _m_waysEditor;
    public bool[,] m_ways;

    public int entranceNorth = -1;
    public int entranceEast = -1;
    public int entranceWest = -1;
    public int entranceSouth = -1;

    [HideInInspector]
    public int[] tileNumber;

    public void buildFromArray()
    {
        int lenght = Mathf.RoundToInt(Mathf.Sqrt(_m_waysEditor.Length));
        m_ways = new bool[lenght, lenght];

        for(int i = 0; i < lenght * lenght; i++)
        {
            int x = i / lenght;
            int y = i % lenght;

            m_ways[x, y] = _m_waysEditor[i];
        }
    }
}
