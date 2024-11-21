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

        public int Iterations { get; set; }

        public float Deceleration { get; set; }

        public float MaxSteps { get; set; }

        public float TransferTolerance { get; set; }

        public float VelocityTolerance { get; set; }

        public List<ChunkType000100F9Entry1> Entry1List { get; set; }

        public List<ChunkType000100F9Entry2> Entry2List { get; set; }

        public List<ChunkType000100F9Entry3> Entry3List { get; set; }

        protected override void ReadCore( ResourceReader reader )
        {
            Iterations = reader.ReadInt32();
            Deceleration = reader.ReadSingle();
            MaxSteps = reader.ReadSingle();
            TransferTolerance = reader.ReadSingle();
            VelocityTolerance = reader.ReadSingle();

            int entry1Count = reader.ReadInt32(); // r28
            int entry2Count = reader.ReadInt32(); // r21
            int entry3Count = reader.ReadInt32(); // r20

            for ( int i = 0; i < entry1Count; i++ )
            {
                var entry = new ChunkType000100F9Entry1();
                entry.SphereRadius = reader.ReadSingle();
                entry.Mass = reader.ReadSingle();

                if ( Version > 0x1104120 )
                {
                    entry.RestoringForce = reader.ReadSingle();
                    entry.WindRate = reader.ReadSingle();
                }
                else
                {
                    entry.Mass += 20.0f;
                    entry.RestoringForce = 0;
                    entry.WindRate = 1.0f;
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

                entry.CapsuleType = reader.ReadInt16();

                if ( entry.CapsuleType == 0 )
                {
                    entry.CapsuleRadius = reader.ReadSingle();
                }
                else if ( entry.CapsuleType == 1 )
                {
                    entry.CapsuleRadius = reader.ReadSingle();
                    entry.CapsuleHeight = reader.ReadSingle();
                }

                entry.Matrix = reader.ReadMatrix4x4();
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
                entry.LengthSq = reader.ReadSingle();
                entry.AngularLimit = reader.ReadSingle();

                if ( Version <= 0x1104120 )
                {
                    entry.ParentBoneIndex = reader.ReadInt16();
                    entry.ChildBoneIndex = reader.ReadInt16();
                }
                else
                {
                    entry.ChainThickness = reader.ReadSingle();
                    entry.ParentBoneIndex = reader.ReadInt16();
                    entry.ChildBoneIndex = reader.ReadInt16();
                }

                Entry3List.Add( entry );
            }
        }

        protected override void WriteCore( ResourceWriter writer )
        {
            writer.WriteInt32( Iterations );
            writer.WriteSingle( Deceleration );
            writer.WriteSingle( MaxSteps );
            writer.WriteSingle( TransferTolerance );
            writer.WriteSingle( VelocityTolerance );

            writer.WriteInt32( Entry1List.Count );
            writer.WriteInt32( Entry2List.Count );
            writer.WriteInt32( Entry3List.Count );

            foreach ( var entry in Entry1List )
            {
                writer.WriteSingle( entry.SphereRadius );
                writer.WriteSingle( entry.Mass );

                if ( Version > 0x1104120 )
                {
                    writer.WriteSingle( entry.RestoringForce );
                    writer.WriteSingle( entry.WindRate );
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
                writer.WriteInt16( entry.CapsuleType );

                if ( entry.CapsuleType == 0 )
                {
                    writer.WriteSingle( entry.CapsuleRadius );
                }
                else if ( entry.CapsuleType == 1 )
                {
                    writer.WriteSingle( entry.CapsuleRadius );
                    writer.WriteSingle( entry.CapsuleHeight );
                }

                writer.WriteMatrix4x4( entry.Matrix );

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
                writer.WriteSingle( entry.LengthSq );
                writer.WriteSingle( entry.AngularLimit );

                if ( Version <= 0x1104120 )
                {
                    writer.WriteInt16( entry.ParentBoneIndex );
                    writer.WriteInt16( entry.ChildBoneIndex );
                }
                else
                {
                    writer.WriteSingle( entry.ChainThickness );
                    writer.WriteInt16( entry.ParentBoneIndex );
                    writer.WriteInt16( entry.ChildBoneIndex );
                }
            }
        }
    }
}