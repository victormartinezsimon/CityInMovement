using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

public class MovementDebug : MonoBehaviour
{
    public float m_distanceAlpha = 0.2f;
    [HideInInspector]
    public int m_destiny;

    [HideInInspector]
    public int m_origin;

    private List<Vector3> listPosition;
    private Vector3 m_nextPoint0;
    private Vector3 m_nextPoint1;
    private Vector3 m_nextPoint2;
    private bool m_move = false;
    #region variables movement
    public float m_velocityBetweenPoints = 2;
    #endregion

    #region unity
    // Use this for initialization
    void Start()
    {
        listPosition = new List<Vector3>();
        getPath();
        m_nextPoint0 = transform.position;
    }
    // Update is called once per frame
    void Update()
    {
        doMovementTransform();
    }
    #endregion
    #region movement transform
    private void doMovementTransform()
    {
        if (!m_move) { return; }
        Vector3 vDirector = (m_nextPoint1 - m_nextPoint0).normalized;
        Vector3 deltaMovement = vDirector * m_velocityBetweenPoints * Time.deltaTime;

        Debug.DrawLine(this.transform.position, this.transform.position + vDirector);
        this.transform.up = vDirector;
        this.transform.position += deltaMovement;

        arriveToDestiny();
    }
    #endregion
    #region ia
    private void getPath()
    {
       IAManager.getInstance().giveMeRoute(m_origin, m_destiny, callbackIA);
    }

    private void callbackIA(List<Vector3> result)
    {
        for (int i = 1; i < result.Count; i++)
        {
            listPosition.Add(result[i]);
        }
        if (!m_move)
        {
            m_nextPoint1 = listPosition[0];
            m_nextPoint2 = listPosition[1];
            m_move = true;
        }
    }

    private void arriveToDestiny()
    {
        Vector3 distanceV3 = m_nextPoint1 - transform.position;
        float magnitude = distanceV3.magnitude;

        if (magnitude <= m_distanceAlpha)
        {
            m_nextPoint0 = transform.position;

            //recalculate destiny or not move if there is no more destinies
            listPosition.RemoveAt(0);
            if (listPosition.Count > 0)
            {
                m_nextPoint1 = listPosition[0];
                if (listPosition.Count > 1)
                {
                    m_nextPoint2 = listPosition[1];
                }
                m_move = true;
            }
            else
            {
                m_move = false;
                Debug.Log("from => " + m_origin + " to => " + m_destiny + " ARRIVED!!");
                Destroy(this.gameObject);
            }
        }
    }

    #endregion
}
