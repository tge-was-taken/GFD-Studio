using System.Numerics;
using System;
using System.Collections.Generic;

namespace AtlusGfdLib
{
    public struct BoundingSphere
    {
        public Vector3 Center;
        public float Radius;

        public BoundingSphere( Vector3 center, float radius )
        {
            Center = center;
            Radius = radius;
        }

        public static BoundingSphere Calculate( IEnumerable<Vector3> vertices )
        {
            var boundingBox = BoundingBox.Calculate( vertices );

            Vector3 sphereCentre = new Vector3
            {
                X = ( float )0.5 * ( boundingBox.Min.X + boundingBox.Max.X ),
                Y = ( float )0.5 * ( boundingBox.Min.Y + boundingBox.Max.Y ),
                Z = ( float )0.5 * ( boundingBox.Min.Z + boundingBox.Max.Z )
            };

            float maxDistSq = 0.0f;
            foreach ( Vector3 vertex in vertices )
            {
                Vector3 fromCentre = vertex - sphereCentre;
                maxDistSq = Math.Max( maxDistSq, fromCentre.LengthSquared() );
            }

            float sphereRadius = ( float )Math.Sqrt( maxDistSq );

            return new BoundingSphere( sphereCentre, sphereRadius );
        }
    }
}