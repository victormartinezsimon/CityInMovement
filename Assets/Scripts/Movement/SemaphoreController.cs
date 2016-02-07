using UnityEngine;
using System.Collections;

public class SemaphoreController : MonoBehaviour {

    private enum SemaphoreStates { RouteOne, RouteTwo, FromRouteOne, FromRouteTwo };

    public GameObject[] routeOne;
    public GameObject[] routeTwo;
    public Color m_red;
    public Color m_green;
    public int m_minTimeChange = 1;
    public int m_maxTimeChange = 5;
    public float timeAllBlocked = 0.5f;
    private float timeSemaphone;
    private float timeChangeState;
    private SemaphoreStates state;
    private float timeAcum;
    

	// Use this for initialization
	void Start () {
        if (!Generator.getInstance().withSemaphores)
        {
           for(int i = 0; i < routeOne.Length; i++)
           {
                Destroy(routeOne[i]);
           }
            for (int i = 0; i < routeTwo.Length; i++)
            {
                Destroy(routeTwo[i]);
            }
            this.enabled = false;
            return;
        }
        timeSemaphone = Random.Range(m_minTimeChange, m_maxTimeChange) + Random.value;
        timeAcum = 0;
        state = (SemaphoreStates)Random.Range(0,4);
        changeSemaphores();
	}
	
	// Update is called once per frame
	void Update () {
        timeAcum += Time.deltaTime;
        if(timeAcum >= timeChangeState)
        {
            timeAcum = 0;
           
            switch (state)
            {
                case SemaphoreStates.RouteOne: state = SemaphoreStates.FromRouteOne; break;
                case SemaphoreStates.RouteTwo: state = SemaphoreStates.FromRouteTwo; break;
                case SemaphoreStates.FromRouteOne: state = SemaphoreStates.RouteTwo; break;
                case SemaphoreStates.FromRouteTwo: state = SemaphoreStates.RouteOne; break;
            }

            changeSemaphores();
        }
	}

    private void changeSemaphores()
    {
        Color oneColor = m_red;
        Color twoColor = m_red;
        bool oneEnable = false;
        bool twoEnable = false;

        switch (state)
        {
            case SemaphoreStates.RouteOne:
                oneColor = m_green;
                twoColor = m_red;
                oneEnable = true;
                twoEnable = false;
                timeChangeState = timeSemaphone;
                break;
            case SemaphoreStates.RouteTwo:
                oneColor = m_red;
                twoColor = m_green;
                oneEnable = false;
                twoEnable = true;
                timeChangeState = timeSemaphone;
                break;
            case SemaphoreStates.FromRouteTwo:
            case SemaphoreStates.FromRouteOne:
                oneColor = m_red;
                twoColor = m_red;
                oneEnable = false;
                twoEnable = false;
                timeChangeState = timeAllBlocked;
                break;

        }

        for(int i = 0; i < routeOne.Length; i++)
        {
            routeOne[i].GetComponent<Renderer>().material.color = oneColor;
            routeOne[i].GetComponent<Collider>().enabled = !oneEnable;
        }

        for (int i = 0; i < routeTwo.Length; i++)
        {
            routeTwo[i].GetComponent<Renderer>().material.color = twoColor;
            routeTwo[i].GetComponent<Collider>().enabled = !twoEnable;
        }

    }
    
    private void drawLines()
    {
        for(int i = 0; i < routeOne.Length; i++)
        {
            Debug.DrawLine(routeOne[i].transform.position, routeOne[i].transform.position + routeOne[i].transform.right, Color.red);
        }
        for (int i = 0; i < routeTwo.Length; i++)
        {
            Debug.DrawLine(routeTwo[i].transform.position, routeTwo[i].transform.position + routeTwo[i].transform.right, Color.red);
        }
    }

}
