using GFDLibrary.IO;

namespace GFDLibrary.Shaders
{
    public abstract class Shader : Resource
    {
        public ShaderType ShaderType { get; set; }

        public int DataLength => Data.Length;

        public ushort Field06 { get; set; }

        public uint MatFlags0 { get; set; }

        public uint MatFlags1 { get; set; }

        public uint MatFlags2 { get; set; }

        public uint Texcoord0 { get; set; }

        public uint Texcoord1 { get; set; }

        public byte[] Data { get; set; }

        protected Shader() { }

        protected Shader( uint version ) : base( version )
        {
        }

        protected override void ReadCore( ResourceReader reader )
        {
            ShaderType = ( ShaderType )reader.ReadUInt16();
            int size = reader.ReadInt32();
            Field06 = reader.ReadUInt16();
            MatFlags0 = reader.ReadUInt32();
            MatFlags1 = reader.ReadUInt32();
            MatFlags2 = reader.ReadUInt32();
            Texcoord0 = reader.ReadUInt32();
            Texcoord1 = reader.ReadUInt32();
            Data = reader.ReadBytes( size );
        }

        protected override void WriteCore( ResourceWriter writer )
        {
            writer.WriteUInt16( ( ushort ) ShaderType );
            writer.WriteInt32( Data.Length );
            writer.WriteUInt16( Field06 );
            writer.WriteUInt32( MatFlags0 );
            writer.WriteUInt32( MatFlags1 );
            writer.WriteUInt32( MatFlags2 );
            writer.WriteUInt32( Texcoord0 );
            writer.WriteUInt32( Texcoord1 );
            writer.WriteBytes( Data );
        }
    }

    public sealed class ShaderPS3 : Shader
    {
        public override ResourceType ResourceType => ResourceType.ShaderPS3;

        public ShaderPS3()
        {

        }

        public ShaderPS3( uint version ) : base( version )
        {
        }
    }

    public sealed class ShaderPSP2 : Shader
    {
        public override ResourceType ResourceType => ResourceType.ShaderPSP2;

        public ShaderPSP2()
        {

        }

        public ShaderPSP2( uint version ) : base( version )
        {
        }
    }

    public sealed class ShaderPS4 : Shader
    {
        public override ResourceType ResourceType => ResourceType.ShaderPS4;

        public uint ShaderType2 { get; set; }

        public ShaderPS4()
        {
            
        }

        public ShaderPS4(uint version) : base(version)
        {    
        }

        protected override void ReadCore( ResourceReader reader )
        {
            ShaderType2 = reader.ReadUInt32();
            base.ReadCore( reader );
        }

        protected override void WriteCore( ResourceWriter writer )
        {
            writer.WriteUInt32( ShaderType2 );
            base.WriteCore( writer );
        }
    }

    public sealed class ShaderMetaphor : Shader
    {
        public override ResourceType ResourceType => ResourceType.ShaderMetaphorDX11;
        public ushort Field06_2 { get; set; }
        public uint Field1C { get; set; }
        public ShaderMetaphor() { }
        public ShaderMetaphor( uint version ) : base (version) { }
        protected override void ReadCore( ResourceReader reader )
        {
            ShaderType = (ShaderType)reader.ReadUInt16();
            int size = reader.ReadInt32();
            Field06 = reader.ReadUInt16();
            Field06_2 = reader.ReadUInt16();
            MatFlags0 = reader.ReadUInt32();
            MatFlags1 = reader.ReadUInt32();
            MatFlags2 = reader.ReadUInt32();
            Texcoord0 = reader.ReadUInt32();
            Texcoord1 = reader.ReadUInt32();
            Field1C = reader.ReadUInt32();
            Data = reader.ReadBytes( size );
        }

        protected override void WriteCore( ResourceWriter writer )
        {
            writer.WriteUInt16( (ushort)ShaderType );
            writer.WriteInt32( Data.Length );
            writer.WriteUInt16( Field06 );
            writer.WriteUInt16( Field06_2 );
            writer.WriteUInt32( MatFlags0 );
            writer.WriteUInt32( MatFlags1 );
            writer.WriteUInt32( MatFlags2 );
            writer.WriteUInt32( Texcoord0 );
            writer.WriteUInt32( Texcoord1 );
            writer.WriteUInt32( Field1C );
            writer.WriteBytes( Data );
        }
    }

    public enum ShaderType
    {
        Vertex,
        Fragment
    }
}