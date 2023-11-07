using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinishScript : MonoBehaviour
{

    [SerializeField] GameObject car;
    [SerializeField] GameObject level;
    Car carStop;

    // Start is called before the first frame update
    void Start()
    {
        carStop = car.GetComponent<Car>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            carStop.StopCar();
            collision.gameObject.transform.position = level.transform.position;
            collision.gameObject.transform.rotation = level.transform.rotation;
        }
    }

}
