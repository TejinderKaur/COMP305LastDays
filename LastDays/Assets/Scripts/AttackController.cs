using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackController : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        //Debug.Log(other.gameObject.tag);
        if (other.gameObject.tag == "Enemy" || other.gameObject.tag == "Player")
        {
            gameObject.GetComponentInParent<CanHitController>().setTarget(other.gameObject.GetComponentInParent<CanHitController>());
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.tag == "Enemy" || other.gameObject.tag == "Player")
        {
            gameObject.GetComponentInParent<CanHitController>().setTarget(null);            
        }
    }
}
