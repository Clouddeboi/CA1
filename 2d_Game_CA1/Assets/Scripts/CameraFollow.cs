using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    private Vector3 offset = new Vector3(0f, 0f, -10f);//offset from the camera to the player
    private float smoothTime = 0.25f;//time it will take camera to reach taregt (Player)
    private Vector3 velocity = Vector3.zero;

    [SerializeField] private Transform target;//choose target to follow

    // Update is called once per frame
    void Update()
    {
        Vector3 targetPosition = target.position + offset;//adds offset to position of target
        transform.position = Vector3.SmoothDamp(transform.position,targetPosition, ref velocity,smoothTime);
    }
}
