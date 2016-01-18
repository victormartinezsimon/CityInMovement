using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class IAManager {

    #region singleton
    private static IAManager m_instance;
    public static IAManager getInstance()
    {
        if(m_instance == null)
        {
            m_instance = new IAManager();
        }
        return m_instance;
    }
    #endregion
    public List<Node> listNodes;


    private IAManager()
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
    public void addConexion(int origin, int destiny)
    {
        listNodes[origin].m_nexts.Add(destiny);
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
