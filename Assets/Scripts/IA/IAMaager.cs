using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class IAMaager {

    #region singleton
    private static IAMaager m_instance;
    public static IAMaager getInstance()
    {
        if(m_instance == null)
        {
            m_instance = new IAMaager();
        }
        return m_instance;
    }
    #endregion
    public List<Node> listNodes;


    private IAMaager()
    {
        listNodes = new List<Node>();
    }
    public void reset()
    {
        listNodes.Clear();
    }

    public int AddNode(Transform position)
    {
        Node n = new Node();
        n.m_position = position;
        n.id = listNodes.Count;
        listNodes.Add(n);
        return n.id;
    }
    public void addConexion(int node, params int[] conexion)
    {
        Node n = listNodes[node];
        for(int i = 0; i < conexion.Length; i++)
        {
            n.m_nexts.Add(i);
        }
    }


    #region nodo
    public class Node
    {
        public Transform m_position;
        public List<int> m_nexts;
        public int id;
        public Node()
        {
            m_nexts = new List<int>();
        }
    }
    #endregion
}
