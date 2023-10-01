using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpPad : MonoBehaviour
{
    //Tutorial followed from: https://www.youtube.com/watch?v=0e3Ld6-RzIU

    //jumpad variables
    private float bounce = 30f;

    private void OnCollisionEnter2D(Collision2D collision)//check if something collided with object
    {
        if(collision.gameObject.CompareTag("Player"))//object collided set to as player
        {
            collision.gameObject.GetComponent<Rigidbody2D>().AddForce(Vector2.up * bounce, ForceMode2D.Impulse);//direction multiplied by bounce value
        }
    }
}
