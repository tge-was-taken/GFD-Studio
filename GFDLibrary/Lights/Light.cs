using System;
using System.Numerics;
using GFDLibrary.IO;

namespace GFDLibrary.Lights
{
    public sealed class Light : Resource
    {
        public override ResourceType ResourceType => ResourceType.Light;

        public LightFlags Flags { get; set; }

        public Vector4 Field30 { get; set; }

        public Vector4 Field40 { get; set; }

        public Vector4 Field50 { get; set; }

        public LightType Type { get; set; }

        public float Field20 { get; set; }

        public float Field04 { get; set; }

        public float Field08 { get; set; }

        public float Field10 { get; set; }

        public float Field6C { get; set; }

        public float Field70 { get; set; }

        public float Field60 { get; set; }

        public float Field64 { get; set; }

        public float Field68 { get; set; }

        public float Field74 { get; set; }

        public float Field78 { get; set; }

        public Light()
        {
            
        }

        public Light(uint version) : base(version)
        {
            
        }

        internal override void Read( ResourceReader reader )
        {
            if ( Version > 0x1104190 )
            {
                Flags = ( LightFlags )reader.ReadInt32();
            }

            Type = ( LightType )reader.ReadInt32();
            Field30 = reader.ReadVector4();
            Field40 = reader.ReadVector4();
            Field50 = reader.ReadVector4();

            switch ( Type )
            {
                case LightType.Type1:
                    Field20 = reader.ReadSingle();
                    Field04 = reader.ReadSingle();
                    Field08 = reader.ReadSingle();
                    break;

                case LightType.Sky:
                    Field10 = reader.ReadSingle();
                    Field04 = reader.ReadSingle();
                    Field08 = reader.ReadSingle();

                    if ( Flags.HasFlag( LightFlags.Flag2 ) )
                    {
                        Field6C = reader.ReadSingle();
                        Field70 = reader.ReadSingle();
                    }
                    else
                    {
                        Field60 = reader.ReadSingle();
                        Field64 = reader.ReadSingle();
                        Field68 = reader.ReadSingle();
                    }
                    break;

                case LightType.Type3:
                    Field20 = reader.ReadSingle();
                    Field08 = reader.ReadSingle();
                    Field04 = reader.ReadSingle();
                    Field74 = reader.ReadSingle();
                    Field78 = reader.ReadSingle();
                    goto case LightType.Sky;
            }
        }

        internal override void Write( ResourceWriter writer )
        {
            if ( Version > 0x1104190 )
            {
                writer.WriteInt32( ( int )Flags );
            }

            writer.WriteInt32( ( int )Type );
            writer.WriteVector4( Field30 );
            writer.WriteVector4( Field40 );
            writer.WriteVector4( Field50 );

            switch ( Type )
            {
                case LightType.Type1:
                    writer.WriteSingle( Field20 );
                    writer.WriteSingle( Field04 );
                    writer.WriteSingle( Field08 );
                    break;
                case LightType.Sky:
                    writer.WriteSingle( Field10 );
                    writer.WriteSingle( Field04 );
                    writer.WriteSingle( Field08 );

                    if ( Flags.HasFlag( LightFlags.Flag2 ) )
                    {
                        writer.WriteSingle( Field6C );
                        writer.WriteSingle( Field70 );
                    }
                    else
                    {
                        writer.WriteSingle( Field60 );
                        writer.WriteSingle( Field64 );
                        writer.WriteSingle( Field68 );
                    }
                    break;
                case LightType.Type3:
                    writer.WriteSingle( Field20 );
                    writer.WriteSingle( Field08 );
                    writer.WriteSingle( Field04 );
                    writer.WriteSingle( Field74 );
                    writer.WriteSingle( Field78 );
                    goto case LightType.Sky;
            }
        }
    }

    [Flags]
    public enum LightFlags
    {
        Flag2 = 1 << 1,
    }

    public enum LightType
    {
        Type1 = 1,
        Sky = 2,
        Type3 = 3,
    }
}