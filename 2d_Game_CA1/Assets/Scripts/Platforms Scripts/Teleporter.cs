using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleporter : MonoBehaviour
{
    //Tutorial Followed Here: https://www.youtube.com/watch?v=0JXVT28KCIg&t=3s

    //Variables
    [SerializeField] private Transform destination;

    public Transform GetDestination()
    {
        return destination;
    }
}
