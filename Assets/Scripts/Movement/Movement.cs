﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

public class Movement : MonoBehaviour {

    public float m_maxVelocity = 10;
    public float m_mase = 10;
    public float m_distanceAlpha = 0.2f;
    public int m_marginAskRoute = 10;
    [HideInInspector]
    public int m_destiny;

    private List<Vector3> listPosition;
    private Mutex m_mutex;
    private Vector3 m_nextPoint0;
    private Vector3 m_nextPoint1;
    private Vector3 m_nextPoint2;
    private bool m_move = false;
    #region variables movement
    public float m_velocityBetweenPoints;
    #endregion

    public float timeBetweenPoints = 1;

    public bool debug;
    private bool responseFromIA;
    private string m_name;

    #region strange bug
    private bool IBreakTheRules;
    private List<List<Vector3>> allMyCheckPoints;
    private GameObject parentBreakRules;
    private Vector3 dontMove;
    #endregion
    #region unity
    // Use this for initialization
    void Start() {
        m_mutex = new Mutex(true);
        listPosition = new List<Vector3>();
        m_mutex.ReleaseMutex();
        getPath();
        m_nextPoint0 = transform.position;
        responseFromIA = true;
        IBreakTheRules = false;
        allMyCheckPoints = new List<List<Vector3>>();
    }
    // Update is called once per frame
    void Update() {
        if (IBreakTheRules) {
            if (Input.GetKeyDown(KeyCode.R))
            {
                StopAllCoroutines();
                StartCoroutine(paintPath());

            }
            transform.position = dontMove;
            return;
        }
        doMovementTransform();
        if (Input.GetKeyDown(KeyCode.D))
        {
            debug = !debug;
        }
        if (Input.GetKey(KeyCode.S))
        {
            if (debug)
            {
                transform.position = m_nextPoint1;
                arriveToDestiny();
            }
        }
    }
    #endregion
    #region movement transform
    private void doMovementTransform()
    {
        if (!m_move) { return; }
       
        Vector3 vDirector = (m_nextPoint1 - transform.position).normalized;
        Vector3 deltaMovement = vDirector * m_velocityBetweenPoints * Time.deltaTime;

        //Debug.DrawLine(this.transform.position, this.transform.position + vDirector, Color.magenta);
        this.transform.up = vDirector;
        this.transform.position += deltaMovement;

        arriveToDestiny();
    }
    private void rotateToDestiny()
    {
        Vector3 v1 = transform.forward;
        Vector3 v2 = m_nextPoint1 - transform.position;
        float newAngle = Vector3.Angle(v1, v2);
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, newAngle));
    }
    #endregion
    #region ia
    private void getPath()
    {
        m_name = m_destiny + " => ";
        m_destiny = IAManager.getInstance().giveMeRandomRoute(m_destiny, callbackIA);
        m_name += m_destiny;
        this.name = m_name;
        responseFromIA = false;
        //IAManager.getInstance().giveMeRoute(1, 0, callbackIA);
    }

    private void callbackIA(List<Vector3> result)
    {
        responseFromIA = true;
        allMyCheckPoints.Add(result);
        for (int i = 1; i < result.Count; i++)
        {
            m_mutex.WaitOne();
            listPosition.Add(result[i]);
            m_mutex.ReleaseMutex();
        }
        if(!m_move)
        {
            m_mutex.WaitOne();
            m_nextPoint1 = listPosition[0];
            m_nextPoint2 = listPosition[1];
            m_move = true;
            m_mutex.ReleaseMutex();
        }
    }

    private void arriveToDestiny()
    {
        Vector3 distanceV3 = m_nextPoint1 - transform.position;
        float magnitude = distanceV3.magnitude;
        if(magnitude <= m_distanceAlpha)
        {
            m_nextPoint0 = transform.position;

            //recalculate destiny or not move if there is no more destinies
            m_mutex.WaitOne();
            listPosition.RemoveAt(0);
            if(listPosition.Count > 0)
            {
                m_nextPoint1 = listPosition[0];
                if(listPosition.Count > 1)
                {
                    m_nextPoint2 = listPosition[1];
                }
                m_move = true;
            }
            else
            {
                Debug.Log("Without destiny!!!!");
                m_move = false;
                getPath();
            }
            m_mutex.ReleaseMutex();

            //recalculate another path
            m_mutex.WaitOne();
            if (listPosition.Count < m_marginAskRoute && responseFromIA)
            {
                getPath();
            }
            m_mutex.ReleaseMutex();
        }
    }

    #endregion
    #region strange bug
    public void autoDestroy()
    {
        if(!IBreakTheRules)
        {
            Destroy(this.gameObject);
        }
    }
    void OnTriggerEnter(Collider other)
    {
        if (!debug) { return; }
        IBreakTheRules = true;
        int count = this.transform.parent.childCount;
        for(int i = 0; i< count; i++)
        {
            this.transform.parent.GetChild(i).SendMessage("autoDestroy");
        }
        dontMove = this.transform.position;
        StartCoroutine(paintPath());
    }

    IEnumerator paintPath()
    {
        if(parentBreakRules != null)
        {
            Destroy(parentBreakRules);
        }
        parentBreakRules = new GameObject("Break Rules");
        yield return new WaitForEndOfFrame();

        for(int i = 0; i < allMyCheckPoints.Count; i++)
        {
            Color c = new Color(Random.value, Random.value, Random.value);

            for(int j = 0; j < allMyCheckPoints[i].Count; j++)
            {
                GameObject go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                go.transform.position = allMyCheckPoints[i][j];
                go.name = i.ToString() + "-" + j.ToString();
                go.transform.localScale = new Vector3(0.4f, 0.4f, 0.4f);
                go.transform.parent = parentBreakRules.transform;
                go.GetComponent<Renderer>().material.color = c;
                yield return new WaitForSeconds(0.2f);
            }
        }

        /*
        for(int i = 0; i < allMyCheckPoints.Count; i++)
        {
            GameObject go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            go.transform.position = allMyCheckPoints[i];
            go.name = i.ToString();
            go.transform.localScale = new Vector3(0.4f, 0.4f, 0.4f);
            go.transform.parent = parentBreakRules.transform;
            go.GetComponent<Renderer>().material.color = Color.red;
            yield return new WaitForSeconds(0.4f);
        }

        for (int i = 0; i < listPosition.Count; i++)
        {
            GameObject go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            go.transform.position = listPosition[i];
            go.name = i.ToString();
            go.transform.localScale = new Vector3(0.4f, 0.4f, 0.4f);
            go.transform.parent = parentBreakRules.transform;
            go.GetComponent<Renderer>().material.color = Color.yellow;
            yield return new WaitForSeconds(0.4f);
        }
        */
    }
    #endregion
}
