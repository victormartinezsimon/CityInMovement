#define USE_POOL
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

    public delegate void callbackIA(List<Vector3> result);
    private delegate void callbackIAPoll(IASearcher searcher);

    private List<List<Vector3>> m_bannedNodes;

    private static int totalThreads = 5;
    private static Semaphore m_pool;
    List<IASearcher> m_avaliable;

    private IAManager()
    {
        listNodes = new List<GraphNode>();
        m_bannedNodes = new List<List<Vector3>>();
        m_avaliable = new List<IASearcher>();
        m_pool = new Semaphore(totalThreads, totalThreads);
    }
    public void reset()
    {
        listNodes.Clear();
        UnityEngine.Random.seed = Mathf.RoundToInt(DateTime.Now.Ticks);
        m_bannedNodes = new List<List<Vector3>>();
        m_pool = new Semaphore(totalThreads, totalThreads);
        m_avaliable.Clear();
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
    
    public void addBannedNodes(List<Vector3> bannedNodes)
    {
        m_bannedNodes.Add(bannedNodes);
    }
    public void constructionEnded()
    {
        for(int i = 0; i < totalThreads; i++)
        {
            IASearcher ia = new IASearcher(listNodes, m_bannedNodes);
            m_avaliable.Add(ia);
        }
    }
    
    #endregion
    #region GraphNode
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
    #region search methods
    private void giveMeRoute(int origin, int destiny, callbackIA callback)
    {
        #if USE_POOL
        long tStart = DateTime.UtcNow.Ticks;
        m_pool.WaitOne();
        long tStop = DateTime.UtcNow.Ticks;
        long diff = tStop - tStart;
        CityLog.getInstance().addTimeWait(diff);
        IASearcher ia = m_avaliable[0];
        m_avaliable.RemoveAt(0);
        ia.resetSearch(origin, destiny, callback, (IASearcher s) => { m_avaliable.Add(s); m_pool.Release(); });
        Thread t = new Thread(ia.makeSearch);
        t.Start();
        #else
          IASearcher searcher = new IASearcher(listNodes, m_bannedNodes);
            searcher.resetSearch(origin, destiny, callback, null);
            Thread t = new Thread(searcher.makeSearch);
            t.Start();
        #endif
    }
    public int giveMeRandomRoute(int origin, callbackIA callback)
    {
        int destiny = UnityEngine.Random.Range(0, listNodes.Count);
        giveMeRoute(origin,destiny , callback);
        return destiny;
    }
    public void getInitialPosition(out Vector3 position, out int m_id)
    {
        m_id = UnityEngine.Random.Range(0, listNodes.Count);
        position = listNodes[m_id].m_position;
    }
#endregion
    #region thread
    private class IASearcher
    {
        private callbackIA m_callback;
        private callbackIAPoll m_callbackPool;
        private List<GraphNode> m_Graph;
        private List<NodeAStar> m_orderedList;
        private List<internalNode> m_visited;
        private bool ended;
        private Vector3 m_destiny;
        private List<List<Vector3>> m_bannedNodes;

        private struct internalNode
        {
            public int id;
            public float cost;
        }

        public IASearcher(List<GraphNode> graph, List<List<Vector3>> bannedNodes)
        {
            m_Graph = graph;
            m_bannedNodes = bannedNodes;
        }

        public void resetSearch(int origin, int destiny, callbackIA callback, callbackIAPoll callbackPool)
        {
            m_destiny = m_Graph[destiny].m_position;
            m_callback = callback;
            m_callbackPool = callbackPool;
            m_visited = new List<internalNode>();
            m_orderedList = new List<NodeAStar>();
            ended = false;

            NodeAStar firstNode = new NodeAStar(0, 0, origin, m_Graph[origin].m_position);
            m_orderedList.Add(firstNode);
        }
        public void makeSearch()
        {
            long tStart = DateTime.UtcNow.Ticks;
            NodeAStar result = null;
            while (m_orderedList.Count != 0 && !ended)
            {
                NodeAStar n = m_orderedList[0];
                m_orderedList.RemoveAt(0);
                internalNode node = new internalNode();
                node.id = n.m_ID;
                node.cost = n.m_cost;
                m_visited.Add(node);

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
                    m_callback(result.m_path);
                }
                else
                {
                    m_callback(null);
                }
            }
            long tStop = DateTime.UtcNow.Ticks;
            long diff = tStop - tStart;
            CityLog.getInstance().addTimeIA(diff);
            if(m_callbackPool != null)
            {
                m_callbackPool(this);
            }
            //Debug.Log("Ticks to find route => " + diff);
        }

        private void addToList(NodeAStar actualNode, Vector3 transformToAdd, int newID)
        {
            float cost = actualNode.m_cost + 1;
            if (canAddToVisited(newID, cost) && !wayBanned(actualNode, transformToAdd))
            {
                float heuristic = calculateHeuristic(transformToAdd);
                NodeAStar node = new NodeAStar(cost, heuristic, newID, transformToAdd);
                node.m_path.AddRange(actualNode.m_path);
                node.m_path.Add(actualNode.m_position);
                m_orderedList.Add(node);
                m_orderedList.Sort();
            }
        }

        private float calculateHeuristic(Vector3 t)
        {
            return (m_destiny - t).magnitude;
        }

        private bool canAddToVisited(int id, float cost)
        {
            int listID = -1;
            for(int i = 0; i < m_visited.Count; i++)
            {
                if(m_visited[i].id == id)
                {
                    listID = i;
                    break;
                }
            }
            if(listID != -1)
            {
                if(cost < m_visited[listID].cost)
                {
                    m_visited.RemoveAt(listID);
                    return true;
                } else
                {
                    return false;
                }
            }
            return true;
        }

        private bool wayBanned(NodeAStar actualNode, Vector3 transformToAdd)
        {
            List<Vector3> actualPath = actualNode.m_path;
            for (int i = 0; i < m_bannedNodes.Count; i++)
            {
               if(actualPath.Contains(m_bannedNodes[i][0]))
                {
                    bool finded = false;
                    int min = Mathf.Max(0, actualPath.Count - 10);
                    
                    for(int idx = min; idx < actualPath.Count; idx++)
                    {
                        if(actualPath[idx] == m_bannedNodes[i][0])
                        {
                            finded = true;
                        }
                    }

                    if(finded && transformToAdd == m_bannedNodes[i][1])
                    {
                        return true;
                    }
                }
            }
            return false;
        }

    }
#endregion
}
