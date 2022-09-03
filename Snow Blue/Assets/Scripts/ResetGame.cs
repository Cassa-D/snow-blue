using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetGame : MonoBehaviour
{
    public GameObject player;
    private Vector3 startPosition;
    
    // Start is called before the first frame update
    void Start()
    {
        startPosition = player.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("r"))
        {
            player.transform.position = startPosition;
            player.transform.rotation = Quaternion.Euler(Vector3.zero);
            player.GetComponent<Movement>().enabled = true;
            player.GetComponent<Movement>().startImpulse = 0;
            player.GetComponent<Rigidbody>().velocity = Vector3.zero;
        }
    }
}
