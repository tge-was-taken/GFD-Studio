using GFDLibrary.IO;

namespace GFDLibrary.Shaders
{
    public abstract class Shader : Resource
    {
        public ShaderType ShaderType { get; set; }

        public int DataLength => Data.Length;

        public ushort Field06 { get; set; }

        public uint Field08 { get; set; }

        public uint Field0C { get; set; }

        public uint Field10 { get; set; }

        public uint Field14 { get; set; }

        public uint Field18 { get; set; }

        public byte[] Data { get; set; }

        protected Shader() { }

        protected Shader( uint version ) : base( version )
        {
        }

        internal override void Read( ResourceReader reader )
        {
            ShaderType = ( ShaderType )reader.ReadUInt16();
            int size = reader.ReadInt32();
            Field06 = reader.ReadUInt16();
            Field08 = reader.ReadUInt32();
            Field0C = reader.ReadUInt32();
            Field10 = reader.ReadUInt32();
            Field14 = reader.ReadUInt32();
            Field18 = reader.ReadUInt32();
            Data = reader.ReadBytes( size );
        }

        internal override void Write( ResourceWriter writer )
        {
            writer.WriteUInt16( ( ushort ) ShaderType );
            writer.WriteInt32( Data.Length );
            writer.WriteUInt16( Field06 );
            writer.WriteUInt32( Field08 );
            writer.WriteUInt32( Field0C );
            writer.WriteUInt32( Field10 );
            writer.WriteUInt32( Field14 );
            writer.WriteUInt32( Field18 );
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

        internal override void Read( ResourceReader reader )
        {
            ShaderType2 = reader.ReadUInt32();
            base.Read( reader );
        }

        internal override void Write( ResourceWriter writer )
        {
            writer.WriteUInt32( ShaderType2 );
            base.Write( writer );
        }
    }

    public enum ShaderType
    {
        Vertex,
        Fragment
    }
}