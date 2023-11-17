using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class LongWater : MonoBehaviour
{
    [SerializeField] GameObject level;
    [SerializeField] GameObject car;
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

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            float angle = level.transform.rotation.eulerAngles.z;
            carStop.StopCar(angle);
            collision.gameObject.transform.position = level.transform.position;
        }
    }
}
