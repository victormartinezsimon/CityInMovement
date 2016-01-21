using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Threading;

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
    public List<GraphNode> listNodes;

    public delegate void callbackIA(List<Vector3> result, List<Vector3> listDebug); 


    private IAManager()
    {
        listNodes = new List<GraphNode>();
    }
    public void reset()
    {
        listNodes.Clear();
    }
    #region Graph builder
    public int AddNode(Vector3 position)
    {
        GraphNode n = new GraphNode();
        n.m_position = position;
        n.m_ID = listNodes.Count;
        listNodes.Add(n);
        return n.m_ID;
    }
    public void addConexion(int origin, int destiny)
    {
        listNodes[origin].m_nexts.Add(destiny);
    }
    #endregion
    #region nodo
    public class GraphNode
    {
        public Vector3 m_position;
        public List<int> m_nexts;
        public int m_ID;
        public GraphNode()
        {
            m_nexts = new List<int>();
        }
    }
    #endregion

    #region nodeAStar
    private class NodeAStar : IComparable<NodeAStar>
    {
        public List<Vector3> m_path = new List<Vector3>();
        public float m_cost;
        public float m_heuristic;
        public Vector3 m_position;
        public int m_ID;

        public NodeAStar(float cost, float heuristic, int id, Vector3 position)
        {
            this.m_cost = cost;
            this.m_heuristic = heuristic;
            this.m_ID = id;
            this.m_position = position;
        }

        public float getTotalCost()
        {
            return m_cost + m_heuristic;
        }

        public int CompareTo(NodeAStar y)
        {
            NodeAStar x = this;
            if (x.getTotalCost() > y.getTotalCost())
            {
                return 1;
            }
            if (x.getTotalCost() < y.getTotalCost())
            {
                return -1;
            }
            return 0;
        }

    }
    #endregion

    public void giveMeRoute(int origin, int destiny, callbackIA callback)
    {
        IASearcher searcher = new IASearcher(origin, destiny, callback, listNodes);
        Thread t = new Thread(searcher.makeSearch);
        t.Start();
    }

    #region thread
    private class IASearcher
    {
        private callbackIA m_callback;
        private List<GraphNode> m_Graph;
        private List<NodeAStar> m_orderedList;
        private List<int> m_visited;
        private bool ended;
        private Vector3 m_destiny;
        private List<Vector3> orderInvestigated;

        public IASearcher(int origin, int destiny, callbackIA callback,List<GraphNode> graph)
        {
            m_Graph = graph;
            resetSearch(origin, destiny, callback);
        }
        public void resetSearch(int origin, int destiny, callbackIA callback)
        {
            m_destiny = m_Graph[destiny].m_position;
            m_callback = callback;
            m_visited = new List<int>();
            m_orderedList = new List<NodeAStar>();
            ended = false;
            orderInvestigated = new List<Vector3>();

            NodeAStar firstNode = new NodeAStar(0, 0, origin, m_Graph[origin].m_position);
            m_orderedList.Add(firstNode);
            orderInvestigated.Add(m_Graph[origin].m_position);
        }
        //maybe the algorithm is bad
        //because we dont recalculate if we know how to reach a node for a better path
        public void makeSearch()
        {
            long tStart = DateTime.UtcNow.Ticks;
            NodeAStar result = null;
            while (m_orderedList.Count != 0 && !ended)
            {
                NodeAStar n = m_orderedList[0];
                m_orderedList.RemoveAt(0);
                m_visited.Add(n.m_ID);

                if (n.m_position == m_destiny)
                {
                    ended = true;
                    result = n;
                    result.m_path.Add(result.m_position);
                    continue;
                }

                //add next to the list
                List<int> nexts = m_Graph[n.m_ID].m_nexts;
                for(int i = 0; i < nexts.Count; i++)
                {
                    addToList(n, m_Graph[nexts[i]].m_position, m_Graph[nexts[i]].m_ID);
                }
            }
            if(m_callback != null)
            {
                if(result != null)
                {
                    m_callback(result.m_path, orderInvestigated);
                }
                else
                {
                    m_callback(null, null);
                }
            }
            long tStop = DateTime.UtcNow.Ticks;
            long diff = tStop - tStart;
            Debug.Log("Ticks to find route => " + diff);
        }

        private void addToList(NodeAStar actualNode, Vector3 transform, int newID)
        {
            if (!m_visited.Contains(newID))
            {
                float cost = actualNode.m_cost + 1;
                float heuristic = calculateHeuristic(transform);
                NodeAStar node = new NodeAStar(cost, heuristic, newID, transform);
                node.m_path.AddRange(actualNode.m_path);
                node.m_path.Add(actualNode.m_position);
                m_orderedList.Add(node);
                m_orderedList.Sort();
                orderInvestigated.Add(transform);
            }
        }

        private float calculateHeuristic(Vector3 t)
        {
            return (m_destiny - t).magnitude;
        }
    }
    #endregion
}
