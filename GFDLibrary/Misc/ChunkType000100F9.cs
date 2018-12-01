using System.Collections.Generic;
using GFDLibrary.IO;

namespace GFDLibrary.Misc
{
    public class ChunkType000100F9 : Resource
    {
        public override ResourceType ResourceType => ResourceType.ChunkType000100F9;

        public ChunkType000100F9()
        {
            Entry1List = new List<ChunkType000100F9Entry1>();
            Entry2List = new List<ChunkType000100F9Entry2>();
            Entry3List = new List<ChunkType000100F9Entry3>();
        }

        public ChunkType000100F9( uint version ) : base( version )
        {
            Entry1List = new List< ChunkType000100F9Entry1 >();
            Entry2List = new List< ChunkType000100F9Entry2 >();
            Entry3List = new List< ChunkType000100F9Entry3 >();
        }

        public int Field140 { get; set; }

        public float Field13C { get; set; }

        public float Field138 { get; set; }

        public float Field134 { get; set; }

        public float Field130 { get; set; }

        public List<ChunkType000100F9Entry1> Entry1List { get; set; }

        public List<ChunkType000100F9Entry2> Entry2List { get; set; }

        public List<ChunkType000100F9Entry3> Entry3List { get; set; }

        internal override void Read( ResourceReader reader )
        {
            Field140 = reader.ReadInt32();
            Field13C = reader.ReadSingle();
            Field138 = reader.ReadSingle();
            Field134 = reader.ReadSingle();
            Field130 = reader.ReadSingle();

            int entry1Count = reader.ReadInt32(); // r28
            int entry2Count = reader.ReadInt32(); // r21
            int entry3Count = reader.ReadInt32(); // r20

            for ( int i = 0; i < entry1Count; i++ )
            {
                var entry = new ChunkType000100F9Entry1();
                entry.Field34 = reader.ReadSingle();
                entry.Field38 = reader.ReadSingle();

                if ( Version > 0x1104120 )
                {
                    entry.Field3C = reader.ReadSingle();
                    entry.Field40 = reader.ReadSingle();
                }
                else
                {
                    entry.Field38 += 20.0f;
                    entry.Field3C = 0;
                    entry.Field40 = 1.0f;
                }

                if ( reader.ReadBoolean() )
                {
                    entry.NodeName = reader.ReadStringWithHash( Version );
                }
                else
                {
                    entry.Field10 = reader.ReadSingle();
                    entry.Field08 = reader.ReadSingle();
                    entry.Field04 = reader.ReadSingle();
                }

                Entry1List.Add( entry );
            }

            for ( int i = 0; i < entry2Count; i++ )
            {
                var entry = new ChunkType000100F9Entry2();

                entry.Field94 = reader.ReadInt16();

                if ( entry.Field94 == 0 )
                {
                    entry.Field84 = reader.ReadSingle();
                }
                else if ( entry.Field94 == 1 )
                {
                    entry.Field84 = reader.ReadSingle();
                    entry.Field88 = reader.ReadSingle();
                }

                entry.Field8C = reader.ReadMatrix4x4();
                if ( reader.ReadBoolean() )
                {
                    entry.NodeName = reader.ReadStringWithHash( Version );
                }

                Entry2List.Add( entry );
            }

            // r24 = 0x1104120

            for ( int i = 0; i < entry3Count; i++ )
            {
                var entry = new ChunkType000100F9Entry3();
                entry.Field00 = reader.ReadSingle();
                entry.Field04 = reader.ReadSingle();

                if ( Version <= 0x1104120 )
                {
                    entry.Field0C = reader.ReadInt16();
                    entry.Field0E = reader.ReadInt16();
                }
                else
                {
                    entry.Field08 = reader.ReadSingle();
                    entry.Field0C = reader.ReadInt16();
                    entry.Field0E = reader.ReadInt16();
                }

                Entry3List.Add( entry );
            }
        }

        internal override void Write( ResourceWriter writer )
        {
            writer.WriteInt32( Field140 );
            writer.WriteSingle( Field13C );
            writer.WriteSingle( Field138 );
            writer.WriteSingle( Field134 );
            writer.WriteSingle( Field130 );

            writer.WriteInt32( Entry1List.Count );
            writer.WriteInt32( Entry2List.Count );
            writer.WriteInt32( Entry3List.Count );

            foreach ( var entry in Entry1List )
            {
                writer.WriteSingle( entry.Field34 );
                writer.WriteSingle( entry.Field38 );

                if ( Version > 0x1104120 )
                {
                    writer.WriteSingle( entry.Field3C );
                    writer.WriteSingle( entry.Field40 );
                }

                if ( entry.NodeName != null )
                {
                    writer.WriteBoolean( true );
                    writer.WriteStringWithHash( Version, entry.NodeName );
                }
                else
                {
                    writer.WriteBoolean( false );
                    writer.WriteSingle( entry.Field10 );
                    writer.WriteSingle( entry.Field08 );
                    writer.WriteSingle( entry.Field04 );
                }
            }

            foreach ( var entry in Entry2List )
            {
                writer.WriteInt16( entry.Field94 );

                if ( entry.Field94 == 0 )
                {
                    writer.WriteSingle( entry.Field84 );
                }
                else if ( entry.Field94 == 1 )
                {
                    writer.WriteSingle( entry.Field84 );
                    writer.WriteSingle( entry.Field88 );
                }

                writer.WriteMatrix4x4( entry.Field8C );

                if ( entry.NodeName != null )
                {
                    writer.WriteBoolean( true );
                    writer.WriteStringWithHash( Version, entry.NodeName );
                }
                else
                {
                    writer.WriteBoolean( false );
                }
            }

            foreach ( var entry in Entry3List )
            {
                writer.WriteSingle( entry.Field00 );
                writer.WriteSingle( entry.Field04 );

                if ( Version <= 0x1104120 )
                {
                    writer.WriteInt16( entry.Field0C );
                    writer.WriteInt16( entry.Field0E );
                }
                else
                {
                    writer.WriteSingle( entry.Field08 );
                    writer.WriteInt16( entry.Field0C );
                    writer.WriteInt16( entry.Field0E );
                }
            }
        }
    }
}