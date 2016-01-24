using UnityEngine;
using System.Collections;

public class DrawVectors : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        draw();
	}

    void draw()
    {
        Debug.DrawLine(transform.position, transform.position + transform.forward, Color.red);
        Debug.DrawLine(transform.position, transform.position + transform.up, Color.blue);
        Debug.DrawLine(transform.position, transform.position + transform.right, Color.green);
    }
}
