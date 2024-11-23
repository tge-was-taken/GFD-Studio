
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using GFDLibrary.Animations;
using GFDLibrary.Common;
using GFDLibrary.IO;
using GFDLibrary.Models;

namespace GFDLibrary.Effects
{
    public sealed class Epl : Resource
    {
        public override ResourceType ResourceType => ResourceType.Epl;

        public EplFlags Flags { get; set; }
        public Node RootNode { get; set; }
        public EplAnimation Animation { get; set; }
        public ushort Field40 { get; set; }
        public Vector4 Field60 { get; set; }
        public Vector4 Field70 { get; set; }

        public Epl() { }
        public Epl( uint Version ) : base( Version ) { }


        protected override void ReadCore( ResourceReader reader )
        {
            //     SetRandomBackColor();
            Logger.Debug( "Epl: Reading" );
            Flags = (EplFlags)reader.ReadInt32();
            RootNode = Node.ReadRecursive( reader, Version );
            Animation = reader.ReadResource<EplAnimation>( Version );
            if ( Version > 0x1105060 )
            {
                Field40 = reader.ReadUInt16();
            }
            if ( Version > 0x2110207 && ((int)Flags & 0x200) != 0 )
            {
                Field60 = reader.ReadVector4();
                Field70 = reader.ReadVector4();
            }
        }

        protected override void WriteCore( ResourceWriter writer )
        {
            //     SetRandomBackColor();
            writer.WriteInt32( (int)Flags );
            Node.WriteRecursive( writer, RootNode );
            writer.WriteResource( Animation );
            if ( Version > 0x1105060 )
            {
                writer.WriteUInt16( Field40 );
            }
            if ( Version > 0x2110207 && ( (int)Flags & 0x200 ) != 0 )
            {
                writer.WriteVector4( Field60 );
                writer.WriteVector4( Field70 );
            }
            //     //Assert(false, "Not implemented");
        }
    }
}
