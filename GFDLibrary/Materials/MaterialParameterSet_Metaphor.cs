using GFDLibrary.IO;
using System.Collections.Generic;
using System.Numerics;

namespace GFDLibrary.Materials
{
    public abstract class MaterialParameterSetBase : Resource
    {
        public abstract string GetParameterName();
    }

    // 7.HLSL (probably?)
    public sealed class MaterialParameterSetType0 : MaterialParameterSetBase
    {
        public Vector4 P0_0 { get; set; } // 0x90
        public float P0_1 { get; set; } // 0xa0
        public float P0_2 { get; set; } // 0xa4
        public float P0_3 { get; set; } // 0xa8
        public float P0_4 { get; set; } // 0xac
        public float P0_5 { get; set; } // 0xb0
        public uint P0_6 { get; set; } // 0xb4

        public override string GetParameterName() => "Parameter Type 0";

        public override ResourceType ResourceType => ResourceType.MaterialParameterSetType0;

        protected override void ReadCore( ResourceReader reader )
        {
            P0_0 = reader.ReadVector4();
            P0_1 = reader.ReadSingle();
            P0_2 = reader.ReadSingle();
            P0_3 = reader.ReadSingle();
            P0_4 = 1;
            if ( Version >= 0x2000004 )
                P0_4 = reader.ReadSingle();
            P0_5 = 1;
            if ( Version >= 0x2030001 )
                P0_5 = reader.ReadSingle();
            P0_6 = 1;
            if ( Version > 0x2110040 )
                P0_6 = reader.ReadUInt32();
            if ( Version == 0x2110140 )
                reader.ReadSingle(); // idk!!
        }

        protected override void WriteCore( ResourceWriter writer )
        {
            writer.WriteVector4( P0_0 );
            writer.WriteSingle( P0_1 );
            writer.WriteSingle( P0_2 );
            writer.WriteSingle( P0_3 );
            if ( Version >= 0x2000004 ) 
                writer.WriteSingle( P0_4 );
            if ( Version >= 0x2030001 )
                writer.WriteSingle( P0_5 );
            if ( Version > 0x2110040 )
                 writer.WriteUInt32( P0_6 );
            if ( Version == 0x2110140 )
                writer.WriteSingle( 0 );
        }
    }

    // 3.HLSL
    public sealed class MaterialParameterSetType1 : MaterialParameterSetBase
    {
        public Vector4 AmbientColor { get; set; } // 0x90
        public Vector4 DiffuseColor { get; set; } // 0xa0
        public Vector4 SpecularColor { get; set; } // 0xb0
        public Vector4 EmissiveColor { get; set; } // 0xc0
        public float Reflectivity { get; set; } // 0xd0
        public float LerpBlendRate { get; set; } // 0xd4

        public override string GetParameterName() => "Parameter Type 1";

        public override ResourceType ResourceType => ResourceType.MaterialParameterSetType1;

        protected override void ReadCore( ResourceReader reader )
        {
            AmbientColor = reader.ReadVector4();
            DiffuseColor = reader.ReadVector4();
            SpecularColor = reader.ReadVector4();
            EmissiveColor = reader.ReadVector4();
            Reflectivity = reader.ReadSingle();
            LerpBlendRate = reader.ReadSingle();
        }

        protected override void WriteCore( ResourceWriter writer )
        {
            writer.WriteVector4( AmbientColor );
            writer.WriteVector4( DiffuseColor );
            writer.WriteVector4( SpecularColor );
            writer.WriteVector4( EmissiveColor );
            writer.WriteSingle( Reflectivity );
            writer.WriteSingle( LerpBlendRate );
        }
    }

    public sealed class MaterialParameterSetType2_3_13 : MaterialParameterSetBase
    {
        public Vector4 P2_0 { get; set; } // 0x90
        public Vector4 P2_1 { get; set; } // 0xa0
        public Vector4 P2_2 { get; set; } // 0xb0
        public Vector4 P2_3 { get; set; } // 0xc0
        public Vector3 P2_4 { get; set; } // 0xd0
        public float P2_5 { get; set; } // 0xe0
        public float P2_6 { get; set; } // 0xe4
        public float P2_7 { get; set; } // 0xf0
        public float P2_8 { get; set; } // 0xf4
        public float P2_9 { get; set; } // 0xfc
        public float P2_10 { get; set; } // 0x100
        public uint P2_11 { get; set; } // 0x130
        public float P2_12 { get; set; } // 0x104
        public Vector3 P2_13 { get; set; } // 0x10c
        public float P2_14 { get; set; } // 0xec
        public float P2_15 { get; set; } // 0xdc
        public float P2_16 { get; set; } // 0xf8
        public float P2_17 { get; set; } // 0x118
        public float P2_18 { get; set; } // 0x11c
        public float P2_19 { get; set; } // 0x120
        public float P2_20 { get; set; } // 0x108
        public float P2_21 { get; set; } // 0xe8
        public float P2_22 { get; set; } // 0x128
        public float P2_23 { get; set; } // 0x12c

        public override string GetParameterName() => "Parameter Type 2";

        public override ResourceType ResourceType => ResourceType.MaterialParameterSetType2_3_13;

        protected override void ReadCore( ResourceReader reader )
        {
            P2_0 = reader.ReadVector4();
            P2_1 = reader.ReadVector4();
            P2_2 = reader.ReadVector4();
            P2_3 = reader.ReadVector4();
            P2_4 = reader.ReadVector3();
            P2_5 = reader.ReadSingle();
            P2_6 = reader.ReadSingle();
            P2_7 = reader.ReadSingle();
            P2_8 = reader.ReadSingle();
            P2_9 = reader.ReadSingle();
            P2_10 = reader.ReadSingle();
            P2_11 = reader.ReadUInt32();
            if ( Version > 0x200ffff )
            {
                P2_12 = reader.ReadSingle();
                P2_13 = reader.ReadVector3();
            }
            P2_14 = 0.5f;
            if ( Version >= 0x2030001 )
                P2_14 = reader.ReadSingle();
            P2_15 = 0.5f;
            if ( Version > 0x2090000 )
                P2_15 = reader.ReadSingle();
            P2_16 = 3f;
            if ( Version >= 0x2094001 )
                P2_16 = reader.ReadSingle();
            P2_17 = 1;
            P2_18 = -1;
            P2_19 = 0;
            if ( Version >= 0x2109501 )
            {
                P2_17 = reader.ReadSingle();
                P2_18 = reader.ReadSingle();
                P2_19 = reader.ReadSingle();
            }
            P2_20 = 0.1f;
            if ( Version >= 0x2109601 )
                P2_20 = reader.ReadSingle();
            if ( Version > 0x2110197 )
                P2_21 = reader.ReadSingle();
            if ( Version > 0x2110203 )
                P2_22 = reader.ReadSingle();
            if ( Version > 0x2110209 )
                P2_23 = reader.ReadSingle();
        }

        protected override void WriteCore( ResourceWriter writer )
        {
            writer.WriteVector4(P2_0);
            writer.WriteVector4(P2_1);
            writer.WriteVector4(P2_2);
            writer.WriteVector4(P2_3);
            writer.WriteVector3(P2_4);
            writer.WriteSingle(P2_5);
            writer.WriteSingle(P2_6);
            writer.WriteSingle(P2_7);
            writer.WriteSingle(P2_8);
            writer.WriteSingle(P2_9);
            writer.WriteSingle(P2_10);
            writer.WriteUInt32( P2_11 );
            if ( Version > 0x200ffff )
            {
                writer.WriteSingle(P2_12 );
                writer.WriteVector3( P2_13 );
            }
            if ( Version >= 0x2030001 )
                writer.WriteSingle( P2_14 );
            if ( Version > 0x2090000 )
                writer.WriteSingle( P2_15 );
            if ( Version >= 0x2094001 )
                writer.WriteSingle( P2_16 );
            if ( Version >= 0x2109501 )
            {
                writer.WriteSingle(P2_17);
                writer.WriteSingle(P2_18);
                writer.WriteSingle( P2_19 );
            }
            if ( Version >= 0x2109601 )
                writer.WriteSingle( P2_20 );
            if ( Version > 0x2110197 )
                writer.WriteSingle( P2_21 );
            if ( Version > 0x2110203 )
                writer.WriteSingle( P2_22 );
            if ( Version > 0x2110209 )
                writer.WriteSingle( P2_23 );
        }
    }

    public sealed class MaterialParameterSetType4 : MaterialParameterSetBase
    {
        public Vector4 P4_0 { get; set; } // 0x90
        public Vector4 P4_1 { get; set; } // 0xa0
        public float P4_2 { get; set; } // 0xb0
        public float P4_3 { get; set; } // 0xb4
        public float P4_4 { get; set; } // 0xb8
        public uint P4_5 { get; set; } // 0xcc
        public float P4_6 { get; set; } // 0xbc
        public float P4_7 { get; set; } // 0xc0
        public float P4_8 { get; set; } // 0xc4

        public override string GetParameterName() => "Parameter Type 4";

        public override ResourceType ResourceType => ResourceType.MaterialParameterSetType4;

        protected override void ReadCore( ResourceReader reader )
        {
            P4_0 = reader.ReadVector4();
            P4_1 = reader.ReadVector4();
            P4_2 = reader.ReadSingle();
            P4_3 = reader.ReadSingle();
            P4_4 = reader.ReadSingle();
            P4_5 = reader.ReadUInt32();
            P4_6 = 0.5f;
            if ( Version >= 0x2110184 )
                P4_6 = reader.ReadSingle();
            P4_7 = 1;
            if ( Version > 0x2110203 )
                P4_7 = reader.ReadSingle();
            if ( Version > 0x2110217 )
                P4_8 = reader.ReadSingle();
        }

        protected override void WriteCore( ResourceWriter writer )
        {
            writer.WriteVector4(P4_0);
            writer.WriteVector4(P4_1);
            writer.WriteSingle(P4_2);
            writer.WriteSingle(P4_3);
            writer.WriteSingle(P4_4);
            writer.WriteUInt32( P4_5 );
            if ( Version >= 0x2110184 )
                writer.WriteSingle( P4_6 );
            if ( Version > 0x2110203 )
                writer.WriteSingle( P4_7 );
            if ( Version > 0x2110217 )
                writer.WriteSingle( P4_8 );
        }
    }
    public sealed class MaterialParameterSetType5 : MaterialParameterSetBase
    {
        public float P5_0 { get; set; } // 0x90
        public float P5_1 { get; set; } // 0x94
        public float P5_2 { get; set; } // 0x98
        public float P5_3 { get; set; } // 0x9c
        public float P5_4 { get; set; } // 0xa0
        public float P5_5 { get; set; } // 0xa4
        public float P5_6 { get; set; } // 0xa8
        public float P5_7 { get; set; } // 0xac
        public float P5_8 { get; set; } // 0xb0
        public float P5_9 { get; set; } // 0xb4
        public float P5_10 { get; set; } // 0xb8
        public float P5_11 { get; set; } // 0xbc
        public float P5_12 { get; set; } // 0xc0
        public float P5_13 { get; set; } // 0xc4
        public uint P5_14 { get; set; } // 0xc8

        public override string GetParameterName() => "Parameter Type 5";

        public override ResourceType ResourceType => ResourceType.MaterialParameterSetType5;

        protected override void ReadCore( ResourceReader reader )
        {
            P5_0 = reader.ReadSingle();
            P5_1 = reader.ReadSingle();
            P5_2 = reader.ReadSingle();
            P5_3 = reader.ReadSingle();
            P5_4 = reader.ReadSingle();
            P5_5 = reader.ReadSingle();
            P5_6 = reader.ReadSingle();
            P5_7 = reader.ReadSingle();
            P5_8 = reader.ReadSingle();
            P5_9 = reader.ReadSingle();
            P5_10 = reader.ReadSingle();
            if ( Version >= 0x2110182 )
            {
                P5_11 = reader.ReadSingle();
                P5_12 = reader.ReadSingle();
            }
            if ( Version >= 0x2110205 )
                P5_13 = reader.ReadSingle();
            if ( Version >= 0x2110188 )
                P5_14 = reader.ReadUInt32();
        }

        protected override void WriteCore( ResourceWriter writer )
        {
            writer.WriteSingle( P5_0 );
            writer.WriteSingle( P5_1 );
            writer.WriteSingle( P5_2 );
            writer.WriteSingle( P5_3 );
            writer.WriteSingle( P5_4 );
            writer.WriteSingle( P5_5 );
            writer.WriteSingle( P5_6 );
            writer.WriteSingle( P5_7 );
            writer.WriteSingle( P5_8 );
            writer.WriteSingle( P5_9 );
            writer.WriteSingle( P5_10 );
            if ( Version >= 0x2110182 )
            {
                writer.WriteSingle(P5_11);
                writer.WriteSingle( P5_12 );
            }
            if ( Version >= 0x2110205 )
                writer.WriteSingle( P5_13 );
            if ( Version >= 0x2110188 )
                writer.WriteUInt32( P5_14 );
        }
    }

    public sealed class MaterialParameterSetType6 : MaterialParameterSetBase
    {
        public struct MaterialParameterSetType6_Field0
        {
            public Vector4 Field0 { get; set; }
            public float Field1 { get; set; }
            public float Field2 { get; set; }
            public float Field3 { get; set; }
            public float Field4 { get; set; }
        }
        private MaterialParameterSetType6_Field0[] mP6_0;
        public MaterialParameterSetType6_Field0[] P6_0 // 0x90
        {
            get => mP6_0;
            set => mP6_0 = value;
        }
        public float P6_1 { get; set; } // 0xd0
        public float P6_2 { get; set; } // 0xd4
        public float P6_3 { get; set; } // 0xd8
        public float P6_4 { get; set; } // 0xdc
        public uint P6_5 { get; set; } // 0xe0

        public override string GetParameterName() => "Parameter Type 6";

        public override ResourceType ResourceType => ResourceType.MaterialParameterSetType6;

        protected override void ReadCore( ResourceReader reader )
        {
            P6_0 = new MaterialParameterSetType6_Field0[2];
            for ( int i = 0; i < 2; i++ )
            {
                P6_0[i].Field0 = reader.ReadVector4();
                P6_0[i].Field1 = reader.ReadSingle();
                P6_0[i].Field2 = reader.ReadSingle();
                P6_0[i].Field3 = reader.ReadSingle();
                P6_0[i].Field4 = reader.ReadSingle();
            }
            P6_1 = reader.ReadSingle();
            if ( Version >= 0x2110021 ) {
                P6_2 = reader.ReadSingle();
                P6_3 = reader.ReadSingle();
            }
            P6_4 = reader.ReadSingle();
            P6_5 = reader.ReadUInt32();
        }

        protected override void WriteCore( ResourceWriter writer )
        {
            for (int i = 0; i < 2; i++ )
            {
                writer.WriteVector4(P6_0[i].Field0);
                writer.WriteSingle(P6_0[i].Field1);
                writer.WriteSingle(P6_0[i].Field2);
                writer.WriteSingle(P6_0[i].Field3);
                writer.WriteSingle( P6_0[i].Field4 );
            }
            writer.WriteSingle( P6_1 );
            if ( Version >= 0x2110021 )
            {
                writer.WriteSingle(P6_2 );
                writer.WriteSingle( P6_3 );
            }
            writer.WriteSingle(P6_4 );
            writer.WriteUInt32( P6_5 );
        }
    }

    public sealed class MaterialParameterSetType7 : MaterialParameterSetBase
    {
        public struct MaterialParameterSetType7_Field0
        {
            public float Field0 { get; set; }
            public float Field1 { get; set; }
            public float Field2 { get; set; }
            public float Field3 { get; set; }
            public float Field4 { get; set; }
            public float Field5 { get; set; }
        }
        private MaterialParameterSetType7_Field0[] mP7_0;
        public MaterialParameterSetType7_Field0[] P7_0 // 0x90
        {
            get => mP7_0;
            set => mP7_0 = value;
        }
        public float P7_1 { get; set; } // 0xf0
        public uint P7_2 { get; set; } // 0xf4

        public override string GetParameterName() => "Parameter Type 7";

        public override ResourceType ResourceType => ResourceType.MaterialParameterSetType7;

        protected override void ReadCore( ResourceReader reader )
        {
            P7_0 = new MaterialParameterSetType7_Field0[4];
            for ( int i = 0; i < 4; i++ )
            {
                P7_0[i].Field0 = reader.ReadSingle();
                P7_0[i].Field1 = reader.ReadSingle();
                P7_0[i].Field2 = reader.ReadSingle();
                P7_0[i].Field3 = reader.ReadSingle();
                P7_0[i].Field4 = reader.ReadSingle();
                P7_0[i].Field5 = reader.ReadSingle();
            }
            P7_1 = reader.ReadSingle();
            P7_2 = reader.ReadUInt32();
        }

        protected override void WriteCore( ResourceWriter writer )
        {
            for ( int i = 0; i < 4; i++ )
            {
                writer.WriteSingle( P7_0[i].Field0 );
                writer.WriteSingle( P7_0[i].Field1 );
                writer.WriteSingle( P7_0[i].Field2 );
                writer.WriteSingle( P7_0[i].Field3 );
                writer.WriteSingle( P7_0[i].Field4 );
                writer.WriteSingle( P7_0[i].Field5 );
            }
            writer.WriteSingle( P7_1 );
            writer.WriteUInt32( P7_2 );
        }
    }

    public sealed class MaterialParameterSetType8 : MaterialParameterSetBase
    {
        public float P8_0 { get; set; } // 0x90
        public float P8_1 {get; set;} // 0x94
        public float P8_2 {get; set;} // 0x98
        public float P8_3 {get; set;} // 0x9c
        public Vector4 P8_4 {get; set;} // 0xa0
        public float P8_5 {get; set;} // 0xb0
        public float P8_6 {get; set;} // 0xb4
        public float P8_7 {get; set;} // 0xb8
        public float P8_8 {get; set;} // 0xbc

        public override string GetParameterName() => "Parameter Type 8";

        public override ResourceType ResourceType => ResourceType.MaterialParameterSetType8;

        protected override void ReadCore( ResourceReader reader )
        {
            P8_0 = reader.ReadSingle();
            P8_1 = reader.ReadSingle();
            P8_2 = reader.ReadSingle();
            P8_3 = reader.ReadSingle();
            P8_4 = reader.ReadVector4();
            P8_5 = reader.ReadSingle();
            P8_6 = reader.ReadSingle();
            P8_7 = reader.ReadSingle();
            P8_8 = reader.ReadSingle();
        }

        protected override void WriteCore( ResourceWriter writer )
        {
            writer.WriteSingle(P8_0 );
            writer.WriteSingle(P8_1 );
            writer.WriteSingle(P8_2 );
            writer.WriteSingle(P8_3 );
            writer.WriteVector4(P8_4 );
            writer.WriteSingle(P8_5 );
            writer.WriteSingle(P8_6 );
            writer.WriteSingle(P8_7 );
            writer.WriteSingle( P8_8 );
        }
    }

    public sealed class MaterialParameterSetType9 : MaterialParameterSetBase
    {
        public float P9_0 { get; set; } // 0x90
        public float P9_1 { get; set; } // 0x94
        public float P9_2 { get; set; } // 0x98
        public float P9_3 { get; set; } // 0x9c
        public Vector4 P9_4 { get; set; } // 0xa0
        public Vector4 P9_5 { get; set; } // 0xb0
        public Vector4 P9_6 { get; set; } // 0xc0
        public Vector4 P9_7 { get; set; } // 0xd0
        public Vector3 P9_8 { get; set; } // 0xe0
        public float P9_9 { get; set; } // 0xec
        public float P9_10 { get; set; } // 0xf0
        public float P9_11 { get; set; } // 0xf4
        public float P9_12 { get; set; } // 0xf8
        public float P9_13 { get; set; } // 0xfc
        public float P9_14 { get; set; } // 0x100
        public float P9_15 { get; set; } // 0x104
        public float P9_16 { get; set; } // 0x108
        public uint P9_17 { get; set; } // 0x10c

        public override string GetParameterName() => "Parameter Type 9";

        public override ResourceType ResourceType => ResourceType.MaterialParameterSetType9;

        protected override void ReadCore( ResourceReader reader )
        {
            P9_0 = reader.ReadSingle();
            P9_1 = reader.ReadSingle();
            P9_2 = reader.ReadSingle();
            P9_3 = reader.ReadSingle();
            P9_4 = reader.ReadVector4();
            P9_5 = reader.ReadVector4();
            P9_6 = reader.ReadVector4();
            P9_7 = reader.ReadVector4();
            P9_8 = reader.ReadVector3();
            P9_9 = reader.ReadSingle();
            P9_10 = reader.ReadSingle();
            P9_11 = reader.ReadSingle();
            P9_12 = reader.ReadSingle();
            P9_13 = reader.ReadSingle();
            P9_14 = reader.ReadSingle();
            P9_15 = reader.ReadSingle();
            P9_16 = reader.ReadSingle();
            P9_17 = reader.ReadUInt32();
        }

        protected override void WriteCore( ResourceWriter writer )
        {
            writer.WriteSingle(P9_0 );
            writer.WriteSingle(P9_1 );
            writer.WriteSingle(P9_2 );
            writer.WriteSingle(P9_3 );
            writer.WriteVector4(P9_4 );
            writer.WriteVector4(P9_5 );
            writer.WriteVector4(P9_6 );
            writer.WriteVector4(P9_7 );
            writer.WriteVector3(P9_8 );
            writer.WriteSingle(P9_9 );
            writer.WriteSingle(P9_10 );
            writer.WriteSingle(P9_11 );
            writer.WriteSingle(P9_12 );
            writer.WriteSingle(P9_13 );
            writer.WriteSingle(P9_14 );
            writer.WriteSingle(P9_15 );
            writer.WriteSingle(P9_16 );
            writer.WriteUInt32( P9_17 );
        }
    }

    public sealed class MaterialParameterSetType10 : MaterialParameterSetBase
    {
        public Vector4 P10_0 { get; set; } // 0x90
        public float P10_1 { get; set; } // 0xa0
        public float P10_2 { get; set; } // 0xa4
        public uint P10_3 { get; set; } // 0xa8

        public override string GetParameterName() => "Parameter Type 10";

        public override ResourceType ResourceType => ResourceType.MaterialParameterSetType10;

        protected override void ReadCore( ResourceReader reader )
        {
            P10_0 = reader.ReadVector4();
            if ( Version >= 0x2110091 )
                P10_1 = reader.ReadSingle();
            P10_2 = reader.ReadSingle();
            if ( Version > 0x2110100 )
                P10_3 = reader.ReadUInt32();
        }

        protected override void WriteCore( ResourceWriter writer )
        {
            writer.WriteVector4( P10_0 );
            if ( Version >= 0x2110091 )
                writer.WriteSingle( P10_1 );
            writer.WriteSingle( P10_2 );
            if ( Version > 0x2110100 )
                writer.WriteUInt32( P10_3 );
        }
    }

    public sealed class MaterialParameterSetType11 : MaterialParameterSetBase
    {
        public Vector4 P11_0 { get; set; } // 0x90
        public float P11_1 { get; set; } // 0xa0

        public override string GetParameterName() => "Parameter Type 11";

        public override ResourceType ResourceType => ResourceType.MaterialParameterSetType11;

        protected override void ReadCore( ResourceReader reader )
        {
            P11_0 = reader.ReadVector4();
            if ( Version >= 0x2108001 )
                P11_1 = reader.ReadSingle();
        }

        protected override void WriteCore( ResourceWriter writer )
        {
            writer.WriteVector4( P11_0 );
            if ( Version >= 0x2108001 )
                writer.WriteSingle( P11_1 );
        }
    }

    public sealed class MaterialParameterSetType12 : MaterialParameterSetBase
    {
        public Vector4 P12_0 { get; set; } // 0x90
        public Vector4 P12_1 {get; set;} // 0xb0
        public Vector4 P12_2 {get; set;} // 0xc0
        public float P12_3 {get; set;} // 0xe8
        public float P12_4 {get; set;} // 0xf4
        public float P12_5 {get; set;} // 0xf8
        public uint P12_6 {get; set;} // 0x12c
        public float P12_7 {get; set;} // 0x108
        public Vector3 P12_8 {get; set;} // 0x110
        public float P12_9 {get; set;} // 0xf0
        public float P12_10 {get; set;} // 0xfc
        public float P12_11 { get; set; } // 0x11c
        public float P12_12 { get; set; } // 0x120
        public float P12_13 { get; set; } // 0x124
        public float P12_14 { get; set; } // 0x10c
        public Vector3 P12_15 { get; set; } // 0xd0
        public float P12_16 { get; set; } // 0xdc
        public float P12_17 { get; set; } // 0xe0
        public float P12_18 { get; set; } // 0xec
        public float P12_19 { get; set; } // 0xe4
        public Vector4 P12_20 { get; set; } // 0xa0
        public float P12_21 { get; set; } // 0x100
        public float P12_22 { get; set; } // 0x104

        public override string GetParameterName() => "Parameter Type 12";

        public override ResourceType ResourceType => ResourceType.MaterialParameterSetType12;

        protected override void ReadCore( ResourceReader reader )
        {
            P12_0 = reader.ReadVector4();
            P12_1 = reader.ReadVector4();
            P12_2 = reader.ReadVector4();
            P12_3 = reader.ReadSingle();
            P12_4 = reader.ReadSingle();
            P12_5 = reader.ReadSingle();
            P12_6 = reader.ReadUInt32();
            P12_7 = reader.ReadSingle();
            P12_8 = reader.ReadVector3();
            P12_9 = reader.ReadSingle();
            P12_10 = reader.ReadSingle();
            if ( Version >= 0x2109501 )
            {
                P12_11 = reader.ReadSingle();
                P12_12 = reader.ReadSingle();
                P12_13 = reader.ReadSingle();
            }
            if ( Version >= 0x2109601 )
                P12_14 = reader.ReadSingle();
            if ( Version >= 0x2109701 )
            {
                P12_15 = reader.ReadVector3();
                P12_16 = reader.ReadSingle();
                P12_17 = reader.ReadSingle();
                P12_18 = reader.ReadSingle();
                P12_19 = reader.ReadSingle(); 
            }
            if ( Version > 0x2110070 )
            {
                P12_20 = reader.ReadVector4();
                P12_21 = reader.ReadSingle();
                P12_22 = reader.ReadSingle();
            }
        }

        protected override void WriteCore( ResourceWriter writer )
        {
            writer.WriteVector4(P12_0);
            writer.WriteVector4(P12_1);
            writer.WriteVector4(P12_2);
            writer.WriteSingle(P12_3);
            writer.WriteSingle(P12_4);
            writer.WriteSingle(P12_5);
            writer.WriteUInt32(P12_6);
            writer.WriteSingle(P12_7);
            writer.WriteVector3(P12_8);
            writer.WriteSingle(P12_9);
            writer.WriteSingle( P12_10 );
            if ( Version >= 0x2109501 )
            {
                writer.WriteSingle(P12_11 );
                writer.WriteSingle(P12_12 );
                writer.WriteSingle( P12_13 );
            }
            if ( Version >= 0x2109601 )
                writer.WriteSingle( P12_14 );
            if ( Version >= 0x2109701 )
            {
                writer.WriteVector3(P12_15 );
                writer.WriteSingle(P12_16 );
                writer.WriteSingle(P12_17 );
                writer.WriteSingle(P12_18 );
                writer.WriteSingle( P12_19 );
            }
            if ( Version > 0x2110070 )
            {
                writer.WriteVector4(P12_20 );
                writer.WriteSingle(P12_21 );
                writer.WriteSingle( P12_22 );
            }
        }
    }

    public sealed class MaterialParameterSetType14 : MaterialParameterSetBase
    {
        public Vector4 P14_0 { get; set; } // 0x90
        public uint P14_1 { get; set; } // 0xa0

        public override string GetParameterName() => "Parameter Type 14";

        public override ResourceType ResourceType => ResourceType.MaterialParameterSetType14;

        protected override void ReadCore( ResourceReader reader )
        {
            P14_0 = reader.ReadVector4();
            P14_1 = reader.ReadUInt32();
        }

        protected override void WriteCore( ResourceWriter writer )
        {
            writer.WriteVector4(P14_0);
            writer.WriteUInt32( P14_1 );
        }
    }

    public sealed class MaterialParameterSetType15 : MaterialParameterSetBase
    {
        public struct MaterialParameterSetType15Layer
        {
            public float Field0 { get; set; } // tileSize
            public float Field1 { get; set; }
            public float Field2 { get; set; } // tileOffset
            public float Field3 { get; set; }
            public float Field4 { get; set; } // roughness
            public float Field5 { get; set; } // metallic
            public Vector3 Field6 { get; set; } // color?
        }
        private MaterialParameterSetType15Layer[] mP15_0;
        public MaterialParameterSetType15Layer[] P15_0 // 0x90
        {
            get => mP15_0;
            set => mP15_0 = value;
        }
        public uint P15_LayerCount { get; set; } // 0x2d0
        public float P15_TriPlanarScale { get; set; } // 0x2d4
        public uint P15_LerpBlendRate { get; set; } // 0x2d8

        public override string GetParameterName() => "Parameter Layered";

        public override ResourceType ResourceType => ResourceType.MaterialParameterSetType15;

        protected override void ReadCore( ResourceReader reader )
        {
            P15_0 = new MaterialParameterSetType15Layer[0x10];
            for ( int i = 0; i < 0x10; i++ )
            {
                P15_0[i].Field0 = reader.ReadSingle(); 
                P15_0[i].Field1 = reader.ReadSingle();
                P15_0[i].Field2 = reader.ReadSingle(); 
                P15_0[i].Field3 = reader.ReadSingle();
                P15_0[i].Field4 = reader.ReadSingle(); 
                P15_0[i].Field5 = reader.ReadSingle(); 
                P15_0[i].Field6 = reader.ReadVector3(); 
            }
            P15_LayerCount = reader.ReadUInt32();
            P15_TriPlanarScale = reader.ReadSingle();
            P15_LerpBlendRate = reader.ReadUInt32();
        }

        protected override void WriteCore( ResourceWriter writer )
        {
            for ( int i = 0; i < 0x10; i++ )
            {
                writer.WriteSingle(P15_0[i].Field0);
                writer.WriteSingle(P15_0[i].Field1);
                writer.WriteSingle(P15_0[i].Field2);
                writer.WriteSingle(P15_0[i].Field3);
                writer.WriteSingle(P15_0[i].Field4);
                writer.WriteSingle(P15_0[i].Field5);
                writer.WriteVector3( P15_0[i].Field6 );
            }
            writer.WriteUInt32(P15_LayerCount);
            writer.WriteSingle(P15_TriPlanarScale);
            writer.WriteUInt32( P15_LerpBlendRate );
        }
    }
}
