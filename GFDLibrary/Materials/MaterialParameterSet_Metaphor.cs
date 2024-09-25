using GFDLibrary.IO;
using System.Numerics;

namespace GFDLibrary.Materials
{
    public sealed class MaterialParameterSetType0 : Resource
    {
        public override ResourceType ResourceType => ResourceType.MaterialParameterSetType0;

        protected override void ReadCore( ResourceReader reader )
        {
            Vector3 DiffuseRGB = reader.ReadVector3();
            float DiffuseAlpha = 1;
            if ( Version >= 0x2000004 )
                DiffuseAlpha = reader.ReadSingle();
            Vector4 DiffuseColor = new Vector4( DiffuseRGB, DiffuseAlpha );
            float Field40 = 1;
            if ( Version >= 0x2030001 )
                Field40 = reader.ReadSingle(); // Reflectivity
            float Field44 = 1;
            if ( Version > 0x2110040 )
                Field40 = reader.ReadSingle(); // Diffusitivity
            if ( Version == 0x2110140 )
                reader.ReadSingle(); // idk
            reader.ReadVector4();
            Vector4 AmbientColor = new Vector4( 1, 1, 1, 1 );
            Vector4 SpecularColor = new Vector4( 1, 1, 1, 1 );
            Vector4 EmissiveColor = new Vector4( 1, 1, 1, 1 );
        }

        protected override void WriteCore( ResourceWriter writer )
        {
            throw new System.NotImplementedException();
        }
    }

    public sealed class MaterialParameterSetType1 : Resource
    {
        public override ResourceType ResourceType => ResourceType.MaterialParameterSetType1;

        protected override void ReadCore( ResourceReader reader )
        {
            var AmbientColor = reader.ReadVector4();
            var DiffuseColor = reader.ReadVector4();
            var SpecularColor = reader.ReadVector4();
            var EmissiveColor = reader.ReadVector4();
            var Field40 = reader.ReadSingle();
            reader.ReadSingle(); // LerpBlendRate
        }

        protected override void WriteCore( ResourceWriter writer )
        {
            throw new System.NotImplementedException();
        }
    }

    public sealed class MaterialParameterSetType2_3_13 : Resource
    {
        public override ResourceType ResourceType => ResourceType.MaterialParameterSetType2_3_13;

        protected override void ReadCore( ResourceReader reader )
        {
            for ( int i = 0; i < 4; i++ )
                reader.ReadVector4();
            reader.ReadVector3();
            for ( int i = 0; i < 6; i++ )
                reader.ReadSingle();
            reader.ReadUInt32();
            if ( Version > 0x200ffff )
            {
                reader.ReadSingle();
                reader.ReadVector3();
            }
            float FieldEC = 0.5f;
            if ( Version >= 0x2030001 )
                FieldEC = reader.ReadSingle();
            float FieldDC = 0.5f;
            if ( Version > 0x2090000 )
                FieldDC = reader.ReadSingle();
            float FieldF8 = 3f;
            if ( Version >= 0x2094001 )
                FieldF8 = reader.ReadSingle();
            float Field118 = 1;
            float Field11C = -1;
            float Field120 = 0;
            if ( Version >= 0x2109501 )
            {
                Field118 = reader.ReadSingle();
                Field11C = reader.ReadSingle();
                Field120 = reader.ReadSingle();
            }
            float Field108 = 0.1f;
            if ( Version >= 0x2109601 )
                Field108 = reader.ReadSingle();
            if ( Version > 0x2110197 )
                reader.ReadSingle(); // FieldE8
            if ( Version > 0x2110203 )
                reader.ReadSingle(); // Field128
            if ( Version > 0x2110209 )
                reader.ReadSingle(); // Field12C
        }

        protected override void WriteCore( ResourceWriter writer )
        {
            throw new System.NotImplementedException();
        }
    }

    public sealed class MaterialParameterSetType4 : Resource
    {
        public override ResourceType ResourceType => ResourceType.MaterialParameterSetType4;

        protected override void ReadCore( ResourceReader reader )
        {
            var AmbientColor = reader.ReadVector4();
            var DiffuseColor = reader.ReadVector4();
            // I don't think these actually set emissive/specular, value of material fields
            // depends on the material type
            Vector4 _Specular = new Vector4( reader.ReadVector3(), 0.5f );
            Vector4 _Emissive = new Vector4( 1, 1, 1, (float)reader.ReadUInt32() );
            if ( Version >= 0x2110184 )
                _Specular.W = reader.ReadSingle();
            var SpecularColor = _Specular;
            if ( Version > 0x2110203 )
                _Emissive.X = reader.ReadSingle();
            if ( Version > 0x2110217 )
                _Emissive.Y = reader.ReadSingle();
            var EmissiveColor = _Emissive;
        }

        protected override void WriteCore( ResourceWriter writer )
        {
            throw new System.NotImplementedException();
        }
    }
    public sealed class MaterialParameterSetType5 : Resource
    {
        public override ResourceType ResourceType => ResourceType.MaterialParameterSetType5;

        protected override void ReadCore( ResourceReader reader )
        {
            // Ambient.X -> Specular.Z
            for ( int i = 0; i < 11; i++ )
                reader.ReadSingle();
            // Specular.W -> Emissive.X
            if ( Version >= 0x2110182 )
                for ( int i = 0; i < 2; i++ )
                    reader.ReadSingle();
            if ( Version >= 0x2110205 )
                reader.ReadSingle();
            if ( Version >= 0x2110188 )
                reader.ReadUInt32();
        }

        protected override void WriteCore( ResourceWriter writer )
        {
            throw new System.NotImplementedException();
        }
    }

    public sealed class MaterialParameterSetType6 : Resource
    {
        public override ResourceType ResourceType => ResourceType.MaterialParameterSetType6;

        protected override void ReadCore( ResourceReader reader )
        {
            for ( int i = 0; i < 2; i++ )
            {
                reader.ReadVector4();
                reader.ReadSingle();
                reader.ReadSingle();
                reader.ReadSingle();
                reader.ReadSingle();
            }
            if ( Version < 0x2110021 )
                reader.ReadSingle();
            else
            {
                reader.ReadSingle();
                reader.ReadSingle();
                reader.ReadSingle();
            }
            reader.ReadSingle();
            reader.ReadUInt32();
        }

        protected override void WriteCore( ResourceWriter writer )
        {
            throw new System.NotImplementedException();
        }
    }

    public sealed class MaterialParameterSetType7 : Resource
    {
        public override ResourceType ResourceType => ResourceType.MaterialParameterSetType7;

        protected override void ReadCore( ResourceReader reader )
        {
            for ( int i = 0; i < 4; i++ )
            {
                reader.ReadSingle();
                reader.ReadSingle();
                reader.ReadSingle();
                reader.ReadSingle();
                reader.ReadSingle();
                reader.ReadSingle();
            }
            reader.ReadSingle(); // FieldF0
            reader.ReadUInt32(); // FieldF4
        }

        protected override void WriteCore( ResourceWriter writer )
        {
            throw new System.NotImplementedException();
        }
    }

    public sealed class MaterialParameterSetType8 : Resource
    {
        public override ResourceType ResourceType => ResourceType.MaterialParameterSetType8;

        protected override void ReadCore( ResourceReader reader )
        {
            reader.ReadSingle();
            reader.ReadSingle();
            reader.ReadSingle();
            reader.ReadSingle();
            reader.ReadVector4();
            reader.ReadSingle();
            reader.ReadSingle();
            reader.ReadSingle();
            reader.ReadSingle();
        }

        protected override void WriteCore( ResourceWriter writer )
        {
            throw new System.NotImplementedException();
        }
    }

    public sealed class MaterialParameterSetType9 : Resource
    {
        public override ResourceType ResourceType => ResourceType.MaterialParameterSetType9;

        protected override void ReadCore( ResourceReader reader )
        {
            reader.ReadSingle();
            reader.ReadSingle();
            reader.ReadSingle();
            reader.ReadSingle();
            reader.ReadVector4();
            reader.ReadVector4();
            reader.ReadVector4();
            reader.ReadVector4();
            reader.ReadVector3();
            reader.ReadSingle();
            reader.ReadSingle();
            reader.ReadSingle();
            reader.ReadSingle();
            reader.ReadSingle();
            reader.ReadSingle();
            reader.ReadSingle();
            reader.ReadSingle();
            reader.ReadUInt32();
        }

        protected override void WriteCore( ResourceWriter writer )
        {
            throw new System.NotImplementedException();
        }
    }

    public sealed class MaterialParameterSetType10 : Resource
    {
        public override ResourceType ResourceType => ResourceType.MaterialParameterSetType10;

        protected override void ReadCore( ResourceReader reader )
        {
            reader.ReadVector4();
            if ( Version >= 0x2110091 )
                reader.ReadSingle();
            reader.ReadSingle();
            if ( Version > 0x2110100 )
                reader.ReadUInt32();
        }

        protected override void WriteCore( ResourceWriter writer )
        {
            throw new System.NotImplementedException();
        }
    }

    public sealed class MaterialParameterSetType11 : Resource
    {
        public override ResourceType ResourceType => ResourceType.MaterialParameterSetType11;

        protected override void ReadCore( ResourceReader reader )
        {
            reader.ReadVector4();
            if ( Version >= 0x2108001 )
                reader.ReadSingle();
        }

        protected override void WriteCore( ResourceWriter writer )
        {
            throw new System.NotImplementedException();
        }
    }

    public sealed class MaterialParameterSetType12 : Resource
    {
        public override ResourceType ResourceType => ResourceType.MaterialParameterSetType12;

        protected override void ReadCore( ResourceReader reader )
        {
            reader.ReadVector4();
            reader.ReadVector4();
            reader.ReadVector4();
            reader.ReadSingle();
            reader.ReadSingle();
            reader.ReadSingle();
            reader.ReadUInt32();
            reader.ReadSingle();
            reader.ReadVector3();
            reader.ReadSingle();
            reader.ReadSingle();
            if ( Version >= 0x2109501 )
            {
                reader.ReadSingle();
                reader.ReadSingle();
                reader.ReadSingle();
            }
            if ( Version >= 0x2109601 )
                reader.ReadSingle();
            if ( Version >= 0x2109701 )
            {
                reader.ReadVector3();
                reader.ReadSingle();
                reader.ReadSingle();
                reader.ReadSingle();
                reader.ReadSingle();
            }
            if ( Version > 0x2110070 )
            {
                reader.ReadVector4();
                reader.ReadSingle();
                reader.ReadSingle();
            }
        }

        protected override void WriteCore( ResourceWriter writer )
        {
            throw new System.NotImplementedException();
        }
    }

    public sealed class MaterialParameterSetType14 : Resource
    {
        public override ResourceType ResourceType => ResourceType.MaterialParameterSetType14;

        protected override void ReadCore( ResourceReader reader )
        {
            reader.ReadVector4();
            reader.ReadUInt32();
        }

        protected override void WriteCore( ResourceWriter writer )
        {
            throw new System.NotImplementedException();
        }
    }

    public sealed class MaterialParameterSetType15 : Resource
    {
        public override ResourceType ResourceType => ResourceType.MaterialParameterSetType15;

        protected override void ReadCore( ResourceReader reader )
        {
            for ( int i = 0; i < 0x10; i++ )
            {
                // layer layers[16];
                reader.ReadSingle(); // tileSize
                reader.ReadSingle();
                reader.ReadSingle(); // tileOffset
                reader.ReadSingle();
                reader.ReadSingle(); // roughness
                reader.ReadSingle(); // metallic
                reader.ReadVector3(); // color?
            }
            reader.ReadUInt32(); // layerCount
            reader.ReadSingle(); // triPlanarScale
            reader.ReadUInt32(); // aTestRef/lerpBlendRate
        }

        protected override void WriteCore( ResourceWriter writer )
        {
            throw new System.NotImplementedException();
        }
    }
}
