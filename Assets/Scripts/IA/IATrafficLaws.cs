using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class IATrafficLaws : MonoBehaviour {

    public avoid[] m_avoidRoutes;
    
    [System.Serializable]
    public class avoid
    {
        public int[] m_avoid;
    }

}
