using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CityGenerator {

    private int[,] m_world;
    private int m_securityDistance;
    private System.Random random;

    private int m_width;
    private int m_height;

    private List<Vector2> m_nextPoint;

    public int[,] getCity()
    {
        return m_world;
    }

    /*
        0  ->  empty
        20 ->  N-S
        21 ->  W-E
        22 ->  W-S
        23 ->  W-N
        24 ->  N-E
        25 ->  S-E
        30 ->  W-S-E
        31 ->  N-S-E
        32 ->  W-E-N
        33 ->  N-S-W
        40 ->  N-S-W-E (square)
        41 ->  N-S-W-E (circle) -> not now
        -1 ->  Not analized
    */

    public CityGenerator(int seed, int widht, int height)
    {
        if(widht < 3)
        {
            widht = 3;
        }
        if(height < 3)
        {
            height = 3;
        }
        m_width = widht;
        m_height = height;
        m_securityDistance = 2;
        m_world = new int[m_width, m_height];
        random = new System.Random(seed);
        initialization();
    }
    private void initialization()
    {
        m_nextPoint = new List<Vector2>();
        m_nextPoint.Add(new Vector2(m_width / 2, m_height / 2));
        for (int i = 0; i < m_width; i++)
        {
            for (int j = 0; j < m_height; ++j)
            {
                m_world[i, j] = -1;
            }
        }
    }

    public void Build()
    {
        cityBuild();
        buildM30();
    }
    private void cityBuild()
    {
        while(m_nextPoint.Count != 0)
        {
            Vector2 pos = m_nextPoint[0];
            m_nextPoint.RemoveAt(0);
            int x = (int)pos.x;
            int y = (int)pos.y;

            if(x < 0 || x >= m_width || y < 0 || y >= m_height)
            {
                continue;
            }

            HashSet<int> avaliable = getAllTiles();

            //north
            int north = m_world[x, y + 1];
            if (north == 21 || north == 23 || north == 24 || north == 32 || north == 0 )
            {
                avaliable.Remove(20);
                avaliable.Remove(23);
                avaliable.Remove(24);
                avaliable.Remove(31);
                avaliable.Remove(32);
                avaliable.Remove(33);
                avaliable.Remove(40);
                avaliable.Remove(41);
            }
            else
            {
                if(north != -1)
                {
                    avaliable.Remove(0);
                    avaliable.Remove(21);
                    avaliable.Remove(22);
                    avaliable.Remove(25);
                    avaliable.Remove(30);
                } else
                {
                    if((y + 1) <= (m_height - m_securityDistance - 1))
                    {
                        Vector2 v = new Vector2(x, y + 1);
                        if (!m_nextPoint.Contains(v))
                        {
                            m_nextPoint.Add(v);
                        }
                    }
                }
            }

            //south
            int south = m_world[x, y - 1];
            if (south == 21 || south == 22 || south == 25 || south == 30 || south == 0 )
            {
                avaliable.Remove(20);
                avaliable.Remove(22);
                avaliable.Remove(25);
                avaliable.Remove(30);
                avaliable.Remove(31);
                avaliable.Remove(33);
                avaliable.Remove(40);
                avaliable.Remove(41);
            }
            else
            {
                if(south != -1)
                {
                    avaliable.Remove(0);
                    avaliable.Remove(21);
                    avaliable.Remove(23);
                    avaliable.Remove(24);
                    avaliable.Remove(32);
                } else
                {
                    if ((y - 1) >= m_securityDistance)
                    {
                        Vector2 v = new Vector2(x, y - 1);
                        if (!m_nextPoint.Contains(v))
                        {
                            m_nextPoint.Add(v);
                        }
                    }
                }
            }

            //west
            int west = m_world[x - 1, y];
            if (west == 20 || west == 22 || west == 23 || west == 33 || west == 0 )
            {
                avaliable.Remove(21);
                avaliable.Remove(22);
                avaliable.Remove(23);
                avaliable.Remove(30);
                avaliable.Remove(32);
                avaliable.Remove(33);
                avaliable.Remove(40);
                avaliable.Remove(41);
            }
            else
            {
                if(west != -1)
                {
                    avaliable.Remove(0);
                    avaliable.Remove(20);
                    avaliable.Remove(24);
                    avaliable.Remove(25);
                    avaliable.Remove(31);
                } else
                {
                    if ((x - 1) >= ( m_securityDistance))
                    {
                        Vector2 v = new Vector2(x - 1, y);
                        if (!m_nextPoint.Contains(v))
                        {
                            m_nextPoint.Add(v);
                        }
                    }
                }
            }

            //east
            int east = m_world[x + 1, y];
            if (east == 20 || east == 24 || east == 25 || east == 31 || east == 0 )
            {
                avaliable.Remove(21);
                avaliable.Remove(24);
                avaliable.Remove(25);
                avaliable.Remove(30);
                avaliable.Remove(31);
                avaliable.Remove(32);
                avaliable.Remove(40);
                avaliable.Remove(41);
            }
            else
            {
                if(east != -1)
                {
                    avaliable.Remove(0);
                    avaliable.Remove(20);
                    avaliable.Remove(22);
                    avaliable.Remove(23);
                    avaliable.Remove(33);

                } else
                {
                    if((x + 1) <= (m_height - m_securityDistance - 1))
                    {
                        Vector2 v = new Vector2(x + 1, y);
                        if (!m_nextPoint.Contains(v))
                        {
                            m_nextPoint.Add(v);
                        }
                    }
                }
            }
            int selected = getRandomTile(avaliable);
            m_world[x, y] = selected;
        }
    }

    private HashSet<int> getAllTiles()
    {
        HashSet<int> avaliable = new HashSet<int>();
        avaliable.Add(0);
        avaliable.Add(20);
        avaliable.Add(21);
        avaliable.Add(22);
        avaliable.Add(23);
        avaliable.Add(24);
        avaliable.Add(25);
        avaliable.Add(30);
        avaliable.Add(31);
        avaliable.Add(32);
        avaliable.Add(33);
        avaliable.Add(40);
        //avaliable.Add(41);
        return avaliable;
    }

    private int getRandomTile(HashSet<int> avaliable)
    {
        List<int> listAvaliable = new List<int>();//we can do this without a list, but is easier like this

        HashSet<int>.Enumerator enumerator = avaliable.GetEnumerator();

        if(avaliable.Count == 0)
        {
            Debug.Log("oh oh!!!");
            return -10;
        }

        while (enumerator.MoveNext())
        {
            int val = enumerator.Current;
            int times = (val / 10) + 1; 

            for(int i = 0; i < times; i++)
            {
                listAvaliable.Add(val);
            }
        }

        return listAvaliable[random.Next(listAvaliable.Count)];

    }
    private void buildM30()
    {
        //bottom part
        for(int i = m_securityDistance; i < m_width - m_securityDistance; i++)
        {
            int top = m_world[i, m_securityDistance];
            if(top == 20 || top == 22 || top == 25 || top == 30 || top == 31 || top == 33 || top == 40 || top == 41)
            {
                m_world[i, m_securityDistance - 1] = 32;
            } else
            {
                m_world[i, m_securityDistance - 1] = 21;
            }
        }

        //top part
        for (int i = m_securityDistance; i < m_width - m_securityDistance; i++)
        {
            int bottom = m_world[i, m_height - m_securityDistance - 1];
            if (bottom == 20 || bottom == 23 || bottom == 24 || bottom == 31 || bottom == 32 || bottom == 33 || bottom == 40 || bottom == 41)
            {
                m_world[i, m_height -  m_securityDistance] = 30;
            }
            else
            {
                m_world[i, m_height - m_securityDistance] = 21;
            }
        }

        //left part
        for (int j = m_securityDistance; j < m_height - m_securityDistance; j++)
        {
            int right = m_world[m_securityDistance, j];
            if (right == 21 || right == 22 || right == 23 || right == 30 || right == 32 || right == 33 || right == 40 || right == 41)
            {
                m_world[m_securityDistance - 1, j] = 31;
            }
            else
            {
                m_world[m_securityDistance - 1, j] = 20;
            }
        }
        //right part
        for (int j = m_securityDistance; j < m_height - m_securityDistance; j++)
        {
            int right = m_world[m_width - m_securityDistance - 1, j];
            if (right == 21 || right == 24 || right == 25 || right == 30 || right == 31 || right == 32 || right == 40 || right == 41)
            {
                m_world[m_width - m_securityDistance, j] = 33;
            }
            else
            {
                m_world[m_width - m_securityDistance, j] = 20;
            }
        }


        //corners
        m_world[m_securityDistance - 1, m_securityDistance - 1] = 24;
        m_world[m_securityDistance - 1, m_height - m_securityDistance] = 25;
        m_world[m_width - m_securityDistance, m_securityDistance - 1] = 23;
        m_world[m_width - m_securityDistance, m_height - m_securityDistance] = 22;
    }

}
