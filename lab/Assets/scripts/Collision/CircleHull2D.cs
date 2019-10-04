﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleHull2D : CollisionHull2D
{
    public CircleHull2D() : base(CollisionHullType2D.hull_circle) { }

    [Range(0f, 100f)]
    public float radius;

    private void Update()
    {
        if (other.getTypeHull() == CollisionHullType2D.hull_obb)
        {
            if (TestCollisionVsOBB((ObjectBoundingBoxHull2D)other))
            {
                GetComponent<MeshRenderer>().material = green;
            }
            else
            {
                GetComponent<MeshRenderer>().material = red;
            }

        }
        else if (other.getTypeHull() == CollisionHullType2D.hull_aabb)
        {
            if (TestCollisionVsAABB((AxisAlignedBoundingBoxHull2D)other))
            {
                GetComponent<MeshRenderer>().material = green;
            }
            else
            {
                GetComponent<MeshRenderer>().material = red;
            }
        }
        else
        {
            if (TestCollisionVsCircle((CircleHull2D)other))
            {
                GetComponent<MeshRenderer>().material = green;
            }
            else
            {
                GetComponent<MeshRenderer>().material = red;
            }
        }
    }

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
        //1. get both centers
        //2. find closest point as = circle center maxed by length and width of rectangle
        //3. find difference between center of box and closest point
        //4. distance of closest point = square root of (dot product(difference))
        //5. get difference between two centers
        //6. center_distance^2 = dot_product(difference of two centers)
        //7. take sum of radius and closest point distance
        //8. square sum total
        //9. compare distance^2 <= sum^2

        Vector2 thisPos, otherPos;
        thisPos = particle.position;
        otherPos = other.getParticle().position;

        //find the closest point of on the rectangle to the circle
        float newX = Mathf.Clamp(thisPos.x, otherPos.x - other.length * .5f, otherPos.x + other.length * .5f);
        float newY = Mathf.Clamp(thisPos.y, otherPos.y - other.height * .5f, otherPos.y + other.height * .5f);
        Vector2 closestPoint = new Vector2(newX, newY);

        //act like it is now a circle, calculate "radius"
        Vector2 closestToCircleDiff = thisPos - closestPoint;
        float distance = Vector2.Dot(closestToCircleDiff, closestToCircleDiff);
        //compare
        if (distance <= radius*radius)
            return true;
        else
            return false;
    }
           
    public override bool TestCollisionVsOBB(ObjectBoundingBoxHull2D other)
    {
        //same as aabb but first
        //1. find new position center of circle rotated around by multuplying by other's world transform inverse
        //2. find closest point as = circle center maxed by length and width of rectangle
        //3. find difference between center of box and closest point
        //4. distance of closest point = square root of (dot product(difference))
        //5. get difference between two centers
        //6. center_distance^2 = dot_product(difference of two centers)
        //7. take sum of radius and closest point distance
        //8. square sum total
        //9. compare distance^2 <= sum^2

        Vector2 otherPos;
        //move circle center into box's space by multiplying by its world transform inverse
        otherPos = other.getParticle().position;
        Vector2 rotatedPos = other.transform.worldToLocalMatrix * (particle.position - otherPos);
        rotatedPos += otherPos;

        //find the closest point of on the rectangle to the circle
        float newX = Mathf.Clamp(rotatedPos.x, otherPos.x - other.length * .5f, otherPos.x + other.length * .5f);
        float newY = Mathf.Clamp(rotatedPos.y, otherPos.y - other.height * .5f, otherPos.y + other.height * .5f);
        Vector2 closestPoint = new Vector2(newX, newY);

        //act like it is now a circle, calculate "radius"
        Vector2 rectangleDiff = otherPos - closestPoint;
        float rectangleToClosest = Vector2.Dot(rectangleDiff, rectangleDiff);

        //calculate distance between particles
        Vector2 objDiff = rotatedPos - otherPos;
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
