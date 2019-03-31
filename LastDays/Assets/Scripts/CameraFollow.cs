using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour {

    //reference to the player
    public Transform playerPosition;
    //reference to the board
    public GameObject boundary;
    private float boundary_l_x;
    private float boundary_r_x;
    private float boundary_u_y;
    private float boundary_d_y;

    private float x_offset = 8;
    private float y_offset = 5;

    void Start() {
        boundary_l_x = boundary.GetComponent<Renderer>().bounds.center.x - (boundary.GetComponent<Renderer>().bounds.size.x/2);
        boundary_r_x = boundary.GetComponent<Renderer>().bounds.center.x + (boundary.GetComponent<Renderer>().bounds.size.x/2);
        boundary_u_y = boundary.GetComponent<Renderer>().bounds.center.y + (boundary.GetComponent<Renderer>().bounds.size.y/2);
        boundary_d_y = boundary.GetComponent<Renderer>().bounds.center.y - (boundary.GetComponent<Renderer>().bounds.size.y/2);
        /* Debug.Log("bounds x: " + boundary_l_x + " : " + boundary_r_x);
        Debug.Log("bounds y: " + boundary_u_y + " : " + boundary_d_y);
        
        Debug.Log("player x, y :" + playerPosition.position.x  + " y:" + playerPosition.position.y);*/
    }
	// Update is called once per frame
	void Update () {
        // Set camera position to player position
        if (playerPosition.position.x > boundary_l_x + x_offset && playerPosition.position.x  < boundary_r_x - x_offset) {
            transform.position = new Vector3(playerPosition.position.x, transform.position.y, transform.position.z);
        }
        if (playerPosition.position.y - y_offset > boundary_d_y && playerPosition.position.y + y_offset < boundary_u_y) {
            transform.position = new Vector3(transform.position.x, playerPosition.position.y, transform.position.z);
        }
        
	}
}