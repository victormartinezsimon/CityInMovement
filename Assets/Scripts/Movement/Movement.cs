using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

public class Movement : MonoBehaviour {

    public float m_distanceToReachPoint = 0.1f;
    public int m_marginAskRoute = 10;
    public float m_maxVelocity = 10f;
    private float m_actualVelocity;

    [Header("ray variables")]
    public float m_timeBetweenRays = 0.2f;
    public float increaseMinDistance = 2;
    public float m_increaseRayDistance = 2;
    private float m_timeAcum;
    private float m_minDistance;
    private float m_rayDistance;
    public LayerMask m_mask;

    [HideInInspector]
    public int m_destinyID;

    private List<Vector3> listPosition;
    private Mutex m_mutex;
    private Vector3 m_nextPoint0;
    private Vector3 m_nextPoint1;
    private Vector3 m_nextPoint2;
    private bool m_move = false;
    
    private bool responseFromIA;
    private string m_name;
    private float constantZ;

    #region unity
    // Use this for initialization
    void Start() {
        m_mutex = new Mutex(true);
        listPosition = new List<Vector3>();
        m_mutex.ReleaseMutex();
        constantZ = transform.position.z;
        getPath();
        m_nextPoint0 = transform.position;
        responseFromIA = true;
        initializeRays();
    }
    // Update is called once per frame
    void Update()
    {
        RayManagement();
        doMovementTransform();
    }
    #endregion
    #region movement transform
    private void initializeRays()
    {
        m_timeAcum = -Random.value;
        m_minDistance = GetComponent<Renderer>().bounds.size.x * increaseMinDistance;
        m_rayDistance = m_minDistance * m_increaseRayDistance;
        m_actualVelocity = m_maxVelocity;
    }

    private void RayManagement()
    {
        m_timeAcum += Time.deltaTime;
        if(m_timeAcum >= m_timeBetweenRays)
        {
            m_timeAcum = 0;
            RaycastHit hit;
            Ray r = new Ray(this.transform.position, this.transform.forward);
            if(Physics.Raycast(r, out hit, m_rayDistance, m_mask))
            {
                Vector3 right = hit.transform.right;
                Vector3 vDirector = (m_nextPoint1 - transform.position).normalized;
                float angle = Vector3.Angle(right, vDirector);

                //in this case, this is another semaphore, not a semaphore that affect us
                //we must be sure is not another car
                if(hit.transform.tag != "Player")
                {
                    if (angle < 170 || angle > 190)
                    {
                        m_actualVelocity = m_maxVelocity;
                        return;
                    }
                }
                
                float distance = hit.distance;
                if(distance < m_minDistance)
                {
                    m_actualVelocity = 0;
                }
                else
                {
                    m_actualVelocity = m_maxVelocity * distance / m_rayDistance;
                }
                
                Debug.DrawLine(this.transform.position, hit.transform.position, Color.red);
            }
            else
            {
                m_actualVelocity = m_maxVelocity;
            }
        }
    }

    private void doMovementTransform()
    {
        if (!m_move) { return; }
       
        Vector3 vDirector = (m_nextPoint1 - transform.position).normalized;
        Vector3 deltaMovement = vDirector * m_actualVelocity * Time.deltaTime;

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
        m_name = m_destinyID + " => ";
        m_destinyID = IAManager.getInstance().giveMeRandomRoute(m_destinyID, callbackIA);
        m_name += m_destinyID;
        this.name = m_name;
        responseFromIA = false;
    }

    private void callbackIA(List<Vector3> result)
    {
        responseFromIA = true;
        for (int i = 1; i < result.Count; i++)
        {
            m_mutex.WaitOne();
            Vector3 vToAdd = new Vector3(result[i].x, result[i].y, constantZ);
            listPosition.Add(vToAdd);
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
        if(magnitude <= m_distanceToReachPoint)
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
