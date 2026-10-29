using GFDLibrary.IO;

namespace GFDLibrary.Animations
{
    public class Single5Key : Key
    {
        [FormerName( "Field00" )]
        public float UVOffsetX { get; set; }

        [FormerName( "Field04" )]
        public float UVOffsetY { get; set; }

        [FormerName( "Field08" )]
        public float UVScaleX { get; set; }

        [FormerName( "Field0C" )]
        public float UVScaleY { get; set; }

        [FormerName( "Field10" )]
        public float UVRotation { get; set; }

        public Single5Key() : this( KeyType.UV0Transform ) { }

        public Single5Key( KeyType type ) : base( type )
        {
        }

        internal override void Read( ResourceReader reader )
        {
            UVOffsetX = reader.ReadSingle();
            UVOffsetY = reader.ReadSingle();
            UVScaleX = reader.ReadSingle();
            UVScaleY = reader.ReadSingle();
            UVRotation = reader.ReadSingle();
        }

        internal override void Write( ResourceWriter writer )
        {
            writer.WriteSingle( UVOffsetX );
            writer.WriteSingle( UVOffsetY );
            writer.WriteSingle( UVScaleX );
            writer.WriteSingle( UVScaleY );
            writer.WriteSingle( UVRotation );
        }
    }
}
