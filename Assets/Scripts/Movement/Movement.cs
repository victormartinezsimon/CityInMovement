﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

public class Movement : MonoBehaviour {

    public float m_maxVelocity = 10;
    public float m_distanceAlpha = 0.2f;
    public int m_marginAskRoute = 10;
    public float m_velocityBetweenPoints;

    [HideInInspector]
    public int m_destiny;

    private List<Vector3> listPosition;
    private Mutex m_mutex;
    private Vector3 m_nextPoint0;
    private Vector3 m_nextPoint1;
    private Vector3 m_nextPoint2;
    private bool m_move = false;
    

    public bool debug;
    private bool responseFromIA;
    private string m_name;

    #region unity
    // Use this for initialization
    void Start() {
        m_mutex = new Mutex(true);
        listPosition = new List<Vector3>();
        m_mutex.ReleaseMutex();
        getPath();
        m_nextPoint0 = transform.position;
        responseFromIA = true;
       
    }
    // Update is called once per frame
    void Update() {
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
        this.transform.forward = vDirector;
       
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
}
