using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarInputHandler : MonoBehaviour
{
    //Components
    Car car;

    //Awake is called when the script instance is being loaded.
    void Awake()
    {
        car = GetComponent<Car>();
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame and is frame dependent
    void Update()
    {
        if (car.carSpeed() > 0)
        {
            Vector2 inputVector = Vector2.zero;

            //Get input from Unity's input system.
            inputVector.x = Input.GetAxis("Horizontal");

            //Send the input to the car controller.
            car.SetInputVector(inputVector);
        }
    }
}
