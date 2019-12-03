﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleHull3D : CollisionHull3D
{
    public CircleHull3D() : base(CollisionHullType3D.hull_circle) { }

    [Range(0f, 100f)]
    float radius;

    private void Update()
    {
    }

    public override bool TestCollisionVsCircle(CircleHull3D other, ref Collision3D c)
    {
        //Collision passes if distance between them <= sumk of radii
        //optimzed collision if distance squared <= sum squared
        //1. get both centers
        //2. take difference between centers
        //3. distance^2 = dot_product(difference, difference)
        //4. take sum of radii
        //5. square sum of radii
        //6. compare distance^2 <= sum^2
        Vector3 thisPos, otherPos;
        thisPos = particle.position;
        radius = this.particle.size.x;
        otherPos = other.particle.position;

        Vector3 diff = otherPos - thisPos;
        float distance = Vector3.Dot(diff, diff);

        float sumRadii = radius + other.radius;
        sumRadii *= sumRadii;

        if (distance <= sumRadii)
        {
            //set collision
            c.a = this;
            c.b = other;
            //c.status = true;
            c.contactCount += 1;
            
            //contact = position + (thisPos + (dirr.normal * ((length - r2) * .5f) + (r1 * .5f)) ) 
            //Vector2 contact = thisPos + (diff.normalized * ((Mathf.Sqrt(distance) - other.radius) * .5f + (radius * .5f)));
            //Vector contact = thisPos + (diff.normalized * (diff.magnitude * .5f));
            Vector3 contact = thisPos + (diff.normalized * radius); //the point on the other circle's
            Vector3 normal = (thisPos - otherPos).normalized;

            //restitution 
            float restitution = Mathf.Max(c.a.getParticle().elasticity, c.b.getParticle().elasticity);

            //penetration
            float penetration = 1;
            //Create contact
            c.contact[0].setNew(contact, normal, restitution, penetration); //do restitution currently set to 1
            //Closing velocity equation from page 114 in book
            c.closingVelocity = Vector2.Dot(c.a.getParticle().posVelocity, (c.b.getParticle().position - c.a.getParticle().position)) +
               Vector3.Dot(c.b.getParticle().posVelocity, (c.a.getParticle().position - c.b.getParticle().position));

            return true;
        }

        else
        {
            return false;
        }

    }

    public override bool TestCollisionVsAABB(AxisAlignedBoundingBoxHull3D other, ref Collision3D c)
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
        radius = this.particle.size.x;
        Vector3 thisPos, otherPos;
        thisPos = particle.position;
        otherPos = other.getParticle().position;

		float otherLength = other.getParticle().size.x * .5f;
		float otherHeight = other.getParticle().size.y * .5f;
		float otherWidth = other.getParticle().size.z * .5f;

		//find the closest point of on the rectangle to the circle
		float newX = Mathf.Clamp(thisPos.x, otherPos.x - otherLength * 0.5f, otherPos.x + otherLength * 0.5f);
        float newY = Mathf.Clamp(thisPos.y, otherPos.y - otherHeight * 0.5f, otherPos.y + otherHeight * 0.5f);
        float newZ = Mathf.Clamp(thisPos.z, otherPos.z - otherWidth * 0.5f, otherPos.z + otherWidth * 0.5f);
        Vector3 closestPoint = new Vector3(newX, newY, newZ);

        //act like it is now a circle, calculate "radius"
        Vector3 rectangleDiff = otherPos - closestPoint;
        float rectangleToClosest = Vector3.Dot(rectangleDiff, rectangleDiff);

        //calculate distance between particles
        Vector3 objDiff = thisPos - otherPos;
        float particleDistance = Vector3.Dot(objDiff, objDiff);

        //find the sum radii
        float sumRadii = radius + Mathf.Sqrt(rectangleToClosest);
        sumRadii *= sumRadii;

        //compare
        if (particleDistance <= sumRadii)
        {
            Debug.Log("Collision from " + gameObject.name);
            return true;
        }
        else
        {
            return false;
        }

    }
           
    public override bool TestCollisionVsOBB(ObjectBoundingBoxHull3D other, ref Collision3D c)
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
        radius = this.particle.size.x;
        Vector3 otherPos;
        //move circle center into box's space by multiplying by its world transform inverse
        otherPos = other.getParticle().position;
        Vector3 rotatedPos = other.transform.localToWorldMatrix.inverse * (particle.position - otherPos);
        rotatedPos += otherPos;
        /*

        //find the closest point of on the rectangle to the circle
        float newX = Mathf.Clamp(rotatedPos.x, otherPos.x - other.length * .5f, otherPos.x + other.length * .5f);
        float newY = Mathf.Clamp(rotatedPos.y, otherPos.y - other.height * .5f, otherPos.y + other.height * .5f);
        Vector3 closestPoint = new Vector2(newX, newY);

        //act like it is now a circle, calculate "radius"
        Vector3 rectangleDiff = otherPos - closestPoint;
        float rectangleToClosest = Vector3.Dot(rectangleDiff, rectangleDiff);

        //calculate distance between particles
        Vector2 objDiff = rotatedPos - otherPos;
        float particleDistance = Vector3.Dot(objDiff, objDiff);

        //find the sum radii
        float sumRadii = radius + Mathf.Sqrt(rectangleToClosest);
        sumRadii *= sumRadii;

        //compare
        if (particleDistance <= sumRadii)
            return true;
        else */
            return false;
    }
}
