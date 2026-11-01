using GFDLibrary.IO;

namespace GFDLibrary.Animations
{
    public class KeyType22 : Key
    {
        [FormerName( "Field04" )]
        public int MotionType { get; set; }

        [FormerName( "Field08" )]

        public int Data0 { get; set; }

        [FormerName( "Field0C" )]

        public short Data1 { get; set; }

        [FormerName( "Field10" )]

        public float DataX { get; set; }

        [FormerName( "Field14" )]

        public float DataY { get; set; }

        [FormerName( "Field18" )]

        public float DataZ { get; set; }

        [FormerName( "Field1C" )]

        public int Next0 { get; set; }

        [FormerName( "Field20" )]

        public short Next1 { get; set; }

        [FormerName( "Field24" )]

        public float NextX { get; set; }

        [FormerName( "Field28" )]

        public float NextY { get; set; }

        [FormerName( "Field2C" )]

        public float NextZ { get; set; }

        [FormerName( "Field00" )]

        public byte Interpolation { get; set; }

        public KeyType22() : base( KeyType.Type22 )
        {
        }

        internal override void Read( ResourceReader reader )
        {
            MotionType = reader.ReadInt32();
            Data0 = reader.ReadInt32();
            Data1 = reader.ReadInt16();
            DataX = reader.ReadSingle();
            DataY = reader.ReadSingle();
            DataZ = reader.ReadSingle();
            Next0 = reader.ReadInt32();
            Next1 = reader.ReadInt16();
            NextX = reader.ReadSingle();
            NextY = reader.ReadSingle();
            NextZ = reader.ReadSingle();
            Interpolation = reader.ReadByte();
        }

        internal override void Write( ResourceWriter writer )
        {
            writer.WriteInt32( MotionType );
            writer.WriteInt32( Data0 );
            writer.WriteInt16( Data1 );
            writer.WriteSingle( DataX );
            writer.WriteSingle( DataY );
            writer.WriteSingle( DataZ );
            writer.WriteInt32( Next0 );
            writer.WriteInt16( Next1 );
            writer.WriteSingle( NextX );
            writer.WriteSingle( NextY );
            writer.WriteSingle( NextZ );
            writer.WriteByte( Interpolation );
        }
    }
}
