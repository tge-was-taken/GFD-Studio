using System.Numerics;
using System;
using System.Collections.Generic;

namespace AtlusGfdLib
{
    /// <summary>
    /// Represents a sphere consisting of a center vector and a sphere radius used for calculations.
    /// </summary>
    public struct BoundingSphere : IEquatable<BoundingSphere>
    {
        /// <summary>
        /// The center vector of the sphere.
        /// </summary>
        public Vector3 Center;

        /// <summary>
        /// The radius of the sphere.
        /// </summary>
        public float Radius;

        /// <summary>
        /// Creates a new <see cref="BoundingSphere"/> whose values are set to the specified values.
        /// </summary>
        /// <param name="center">The value to set the center vector to.</param>
        /// <param name="radius">The value to set the sphere radius to.</param>
        public BoundingSphere( Vector3 center, float radius )
        {
            Center = center;
            Radius = radius;
        }

        /// <summary>
        /// Calculates and creates a new <see cref="BoundingSphere"/> from vertices.
        /// </summary>
        /// <param name="vertices">The vertices used to calculate the components.</param>
        /// <returns>A new <see cref="BoundingBox"/> calculated form the specified vertices.</returns>
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

        public static BoundingSphere Calculate( BoundingBox boundingBox )
        {
            Vector3 sphereCentre = new Vector3
            {
                X = ( float )0.5 * ( boundingBox.Min.X + boundingBox.Max.X ),
                Y = ( float )0.5 * ( boundingBox.Min.Y + boundingBox.Max.Y ),
                Z = ( float )0.5 * ( boundingBox.Min.Z + boundingBox.Max.Z )
            };

            float maxDistSq = 0.0f;
            var vertices = new List<Vector3>() { boundingBox.Min, boundingBox.Max };
            foreach ( Vector3 vertex in vertices )
            {
                Vector3 fromCentre = vertex - sphereCentre;
                maxDistSq = Math.Max( maxDistSq, fromCentre.LengthSquared() );
            }

            float sphereRadius = ( float )Math.Sqrt( maxDistSq );

            return new BoundingSphere( sphereCentre, sphereRadius );
        }

        public override bool Equals( object obj )
        {
            if ( obj == null || obj.GetType() != typeof( BoundingSphere ) )
                return false;

            return Equals( ( BoundingSphere )obj );
        }

        public bool Equals( BoundingSphere other )
        {
            return Center == other.Center && Radius == other.Radius;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 11;
                hash = hash * 33 + Center.GetHashCode();
                hash = hash * 33 + Radius.GetHashCode();
                return hash;
            }
        }

        public override string ToString()
        {
            return $"{Center} {Radius}";
        }
    }
}