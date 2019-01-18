using System;
using System.Numerics;
using GFDLibrary.IO;

namespace GFDLibrary.Lights
{
    public sealed class Light : Resource
    {
        public override ResourceType ResourceType => ResourceType.Light;

        public LightFlags Flags { get; set; }

        public Vector4 AmbientColor { get; set; }

        public Vector4 DiffuseColor { get; set; }

        public Vector4 SpecularColor { get; set; }

        public LightType Type { get; set; }

        public float Field20 { get; set; }

        public float Field04 { get; set; }

        public float Field08 { get; set; }

        public float Field10 { get; set; }

        public float AttenuationStart { get; set; }

        public float AttenuationEnd { get; set; }

        public float Field60 { get; set; }

        public float Field64 { get; set; }

        public float Field68 { get; set; }

        public float AngleInnerCone { get; set; }

        public float AngleOuterCone { get; set; }

        public Light()
        {
            Flags = LightFlags.Bit1;
            AmbientColor = Vector4.One;
            DiffuseColor = Vector4.One;
            SpecularColor = Vector4.One;
            Type = LightType.Point;
            Field08 = 1;

        }

        public Light(uint version) : base(version)
        {
            
        }

        internal override void Read( ResourceReader reader, long endPosition = -1 )
        {
            if ( Version > 0x1104190 )
            {
                Flags = ( LightFlags )reader.ReadInt32();
            }

            Type = ( LightType )reader.ReadInt32();
            AmbientColor = reader.ReadVector4();
            DiffuseColor = reader.ReadVector4();
            SpecularColor = reader.ReadVector4();

            switch ( Type )
            {
                case LightType.Type1:
                    Field20 = reader.ReadSingle(); // 0
                    Field04 = reader.ReadSingle(); // 0
                    Field08 = reader.ReadSingle(); // 1
                    break;

                case LightType.Point:
                    Field10 = reader.ReadSingle(); // 0
                    Field04 = reader.ReadSingle(); // 0
                    Field08 = reader.ReadSingle(); // 0

                    if ( Flags.HasFlag( LightFlags.Bit2 ) )
                    {
                        AttenuationStart = reader.ReadSingle(); // attenuation start?
                        AttenuationEnd = reader.ReadSingle(); // attenuation end?
                    }
                    else
                    {
                        Field60 = reader.ReadSingle(); // 0
                        Field64 = reader.ReadSingle(); // 0
                        Field68 = reader.ReadSingle(); // 0
                    }
                    break;

                case LightType.Spot:
                    Field20 = reader.ReadSingle(); // 0
                    Field04 = reader.ReadSingle(); // 0
                    Field08 = reader.ReadSingle(); // 1
                    AngleInnerCone = reader.ReadSingle(); // 0.08377809
                    AngleOuterCone = reader.ReadSingle(); // 0.245575309
                    goto case LightType.Point;
            }
        }

        internal override void Write( ResourceWriter writer )
        {
            if ( Version > 0x1104190 )
            {
                writer.WriteInt32( ( int )Flags );
            }

            writer.WriteInt32( ( int )Type );
            writer.WriteVector4( AmbientColor );
            writer.WriteVector4( DiffuseColor );
            writer.WriteVector4( SpecularColor );

            switch ( Type )
            {
                case LightType.Type1:
                    writer.WriteSingle( Field20 );
                    writer.WriteSingle( Field04 );
                    writer.WriteSingle( Field08 );
                    break;
                case LightType.Point:
                    writer.WriteSingle( Field10 );
                    writer.WriteSingle( Field04 );
                    writer.WriteSingle( Field08 );

                    if ( Flags.HasFlag( LightFlags.Bit2 ) )
                    {
                        writer.WriteSingle( AttenuationStart );
                        writer.WriteSingle( AttenuationEnd );
                    }
                    else
                    {
                        writer.WriteSingle( Field60 );
                        writer.WriteSingle( Field64 );
                        writer.WriteSingle( Field68 );
                    }
                    break;
                case LightType.Spot:
                    writer.WriteSingle( Field20 );
                    writer.WriteSingle( Field08 );
                    writer.WriteSingle( Field04 );
                    writer.WriteSingle( AngleInnerCone );
                    writer.WriteSingle( AngleOuterCone );
                    goto case LightType.Point;
            }
        }
    }

    [Flags]
    public enum LightFlags
    {
        Bit1 = 1 << 0,
        Bit2 = 1 << 1,
        Bit3 = 1 << 2,
    }

    public enum LightType
    {
        Type1 = 1,
        Point = 2,
        Spot = 3,
    }
}