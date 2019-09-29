﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleHull2D : CollisionHull2D
{
    public CircleHull2D() : base(CollisionHullType2D.hull_circle) { }

    [Range(0f, 100f)]
    public float radius;

    public override bool TestCollisionVsCircle(CircleHull2D other)
    {
        //Collision passes if distance between them <= sumk of radii
        //optimzed collision if distance squared <= sum squared
        //1. get both centers
        //2. take difference between centers
        //3. distance^2 = dot_product(difference, difference)
        //4. take sum of radii
        //5. square sum of radii
        //6. compare distance^2 <= sum^2
        Vector2 thisPos, otherPos;
        thisPos = particle.position;
        otherPos = other.particle.position;

        Vector2 diff = thisPos - otherPos;
        float distance = Vector2.Dot(diff, diff);

        float sumRadii = radius + other.radius;
        sumRadii *= sumRadii;

        if (distance <= sumRadii)
            return true;
        else
            return false;
    }

    public override bool TestCollisionVsAABB(AxisAlignedBoundingBoxHull2D other)
    {
        //Collision Passes if closest point on box is within radius of circle
        //Calculate closest point on box by clamping center; closes point vs circle test
        Vector2 thisPos, otherPos;
        thisPos = particle.position;
        otherPos = other.getParticle().position;

        //find the closest point of on the rectangle to the circle
        float newX = Mathf.Clamp(thisPos.x, otherPos.x - other.length / 2f, otherPos.x + other.length / 2);
        float newY = Mathf.Clamp(thisPos.y, otherPos.y - other.height / 2f, otherPos.x + other.height / 2);
        Vector2 closestPoint = new Vector2(newX, newY);

        //act like it is now a circle, calculate "radius"
        Vector2 rectangleDiff = otherPos - closestPoint;
        float rectangleToClosest = Vector2.Dot(rectangleDiff, rectangleDiff);

        //calculate distance between particles
        Vector2 objDiff = thisPos - otherPos;
        float particleDistance = Vector2.Dot(objDiff, objDiff);

        //find the sum radii
        float sumRadii = radius + Mathf.Sqrt(rectangleToClosest);
        sumRadii *= sumRadii;

        //compare
        if (particleDistance <= sumRadii)
            return true;
        else
            return false;
    }
           
    public override bool TestCollisionVsOBB(ObjectBoundingBoxHull2D other)
    {
        //same as aabb but first


        Vector2 thisPos, otherPos;
        //move circle center into box's space by multiplying by its world transform inverse
        Vector4 rotatedPos = other.transform.localToWorldMatrix.inverse * new Vector4(particle.position.x, particle.position.y, 0 ,0);
        thisPos = new Vector2(rotatedPos.x, rotatedPos.y);
        otherPos = other.getParticle().position;

        //find the closest point of on the rectangle to the circle
        float newX = Mathf.Clamp(thisPos.x, otherPos.x - other.length / 2f, otherPos.x + other.length / 2);
        float newY = Mathf.Clamp(thisPos.y, otherPos.y - other.height / 2f, otherPos.x + other.height / 2);
        Vector2 closestPoint = new Vector2(newX, newY);

        //act like it is now a circle, calculate "radius"
        Vector2 rectangleDiff = otherPos - closestPoint;
        float rectangleToClosest = Vector2.Dot(rectangleDiff, rectangleDiff);

        //calculate distance between particles
        Vector2 objDiff = thisPos - otherPos;
        float particleDistance = Vector2.Dot(objDiff, objDiff);

        //find the sum radii
        float sumRadii = radius + Mathf.Sqrt(rectangleToClosest);
        sumRadii *= sumRadii;

        //compare
        if (particleDistance <= sumRadii)
            return true;
        else
            return false;
    }
}