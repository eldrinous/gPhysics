﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum rotationUpdate
{
    ROTATION_EULER_EXPLICIT,
    ROTATION_KINEMATIC
}

public enum positionUpdate
{
    POSITION_EULER_EXPLICIT,
    POSITION_KINEMATIC,
}

public class particle2D : MonoBehaviour
{
    //step 1
    public Vector2 position, posVelocity, posAcceleration;
    public float rotation, rotVelocity, rotAcceleration;

    [SerializeField]
    rotationUpdate rotationMode;
    [SerializeField]
    positionUpdate positionMode;

    float rotMagnitudeModifier;
    float posMagnitudeModifier;

    [SerializeField]
    bool circulerMode;

    float currentTime = 0;

    public Slider positionSlider;
    public Slider rotationSlider;
    //step 2
    void updatePositionEulerExplicit(float dt)
    {
        //x(t + dt) = x(t) + v(t)dt
        //Euler's Method
        //F(t+dt) = F(t) + f(t)dt
        //               + (dF/dt)dt
        position += posVelocity * dt;

        //**** more to do here ****
        //v(t*dt) = v(t) + a(t)dt
        posVelocity += posAcceleration * dt;
    }
    void updatePositionKinematic(float dt)
    {
        //x(t+dt) = x(t) + v(t)dt + (1/2) * a(t) * dt^2
        position += posVelocity * dt +  .5f * posAcceleration * (dt * dt);
        posVelocity += posAcceleration * dt;
    }

    void updateRotationEulerExplicit(float dt)
    {
  
        rotation += rotVelocity * dt;

        rotVelocity += rotAcceleration * dt;
    }

    void updateRotationKinematic(float dt)
    {
        rotation += rotVelocity * dt + .5f * rotAcceleration * (dt * dt);
        rotVelocity += (rotAcceleration * dt);
    }


    // Start is called before the first frame update
    void Start()
    {
        if(circulerMode)
        {
            posVelocity = new Vector2(1, 0);
        }
        posMagnitudeModifier = positionSlider.value;
        rotMagnitudeModifier = rotationSlider.value;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float dt = Time.fixedDeltaTime;
        posMagnitudeModifier = positionSlider.value;
        rotMagnitudeModifier = rotationSlider.value;

        /*
        updatePositionEulerExplicit(Time.fixedDeltaTime);
        transform.position = position;

        //step 4
        posAcceleration.x = -Mathf.Sin(Time.fixedTime);

        */
        //step 3

        switch (rotationMode)
        {
            case rotationUpdate.ROTATION_EULER_EXPLICIT:
                updateRotationEulerExplicit(dt);
                break;
            case rotationUpdate.ROTATION_KINEMATIC:
                updateRotationKinematic(dt);
                break;
        }
        transform.rotation = Quaternion.Euler(0,0,rotation);
        
        switch(positionMode)
        {
            case positionUpdate.POSITION_EULER_EXPLICIT:
                updatePositionEulerExplicit(dt);
                break;
            case positionUpdate.POSITION_KINEMATIC:
                updatePositionKinematic(dt);
                break;
        }
        transform.position = position;

        posAcceleration.x = -Mathf.Sin(currentTime);

        if(circulerMode)
        {
            posAcceleration.y = -Mathf.Cos(currentTime);
        }
    
        rotAcceleration = -Mathf.Sin(currentTime);

        currentTime += dt;
    }

    public void resetData()
    {
       
        transform.position *= 0;

        position *= 0;
        posVelocity = new Vector2(1, 0);
        posAcceleration *= 0;

        rotation = 0;
        rotVelocity = 1;
        rotAcceleration = 0;

        currentTime = 0;
    }
}
