﻿using System;
using System.Collections.Generic;
using System.Numerics;

namespace AtlusGfdLib
{
    public struct BoundingBox : IEquatable<BoundingBox>
    {
        public Vector3 Min;
        public Vector3 Max;

        public BoundingBox( Vector3 min, Vector3 max )
        {
            Min = min;
            Max = max;
        }

        public static BoundingBox Calculate( IEnumerable<Vector3> vertices )
        {
            Vector3 minExtent = new Vector3( float.MaxValue, float.MaxValue, float.MaxValue );
            Vector3 maxExtent = new Vector3( float.MinValue, float.MinValue, float.MinValue );
            foreach ( Vector3 vertex in vertices )
            {
                minExtent.X = Math.Min( minExtent.X, vertex.X );
                minExtent.Y = Math.Min( minExtent.Y, vertex.Y );
                minExtent.Z = Math.Min( minExtent.Z, vertex.Z );

                maxExtent.X = Math.Max( maxExtent.X, vertex.X );
                maxExtent.Y = Math.Max( maxExtent.Y, vertex.Y );
                maxExtent.Z = Math.Max( maxExtent.Z, vertex.Z );
            }

            return new BoundingBox( minExtent, maxExtent );
        }

        public override bool Equals( object obj )
        {
            if ( obj == null || obj.GetType() != typeof(BoundingBox))
                return false;

            return Equals( (BoundingBox)obj );
        }

        public bool Equals( BoundingBox other )
        {
            return Min == other.Min && Max == other.Max;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 11;
                hash = hash * 33 + Min.GetHashCode();
                hash = hash * 33 + Max.GetHashCode();
                return hash;
            }
        }
    }
}