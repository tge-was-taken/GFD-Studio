using System;
using GFDLibrary.IO;

namespace GFDLibrary
{
    public sealed class Epl : Resource
    {
        public override ResourceType ResourceType => ResourceType.Epl;

        // Temporarily just store the raw data
        public byte[] Raw { get; set; }

        //public int Field20 { get; set; }

        //public Node Field2C { get; set; }

        //public object Field30 { get; set; }

        //public short Field40 { get; set; }

        public Epl()
        {

        }

        public Epl( uint version ) : base( version )
        {

        }

        internal static Epl Read( ResourceReader reader, uint version, out bool skipProperties )
        {
            var epl = new Epl();

            var startOffset = reader.Position;
            var skipEplData = false;
            skipProperties = false;

            // Hacks galore
            if ( reader.Position == 0x1a67d5 || reader.Position == 0x1b09ef )
            {
                reader.SeekCurrent( 0x27C9 );
                skipEplData = true;
            }
            else if ( reader.Position == 0x1a907c || reader.Position == 0x1b3296 )
            {
                reader.SeekCurrent( 0x3568 );
                skipEplData = true;
            }
            else if ( reader.Position == 0x1ac6c2 || reader.Position == 0x1b68dc )
            {
                reader.SeekCurrent( 0x3571 );
                skipEplData = true;
            }
            else
            {
                var foundProperty = false;
                int testCount;
                const int MAX_TEST_COUNT = 1000000;
                for ( testCount = 0; testCount < MAX_TEST_COUNT; testCount++ )
                {
                    var test = reader.ReadUInt32();

                    if ( test == 0x42697030 ||// Bip01
                         test == 0x726F6F74 ||// root
                         test == 0x726F74E0 ||// rot
                         test == 0x62206C20 ||// b l
                         test == 0x62206C5F ||// b l_
                         test == 0x62207220 ||// b r
                         test == 0x6220725F ||// b r_
                         test == 0x68656164 ||// head
                         test == 0x685F415F ||// h_A_
                         test == 0x685F425F ||// h_B_
                         test == 0x685F435F ||// h_C_
                         test == 0x625F6D61 ||// b_man
                         test == 0x625F6865 ||// b_he
                         test == 0x62206465 ||// b de
                         test == 0x626F6479 ||// body
                         test == 0x62207461 ||// b ta
                         test == 0x6F626A5F ||// obj_
                         test == 0x6D616E64 ||// mand
                         test == 0x625F6E6F ||// b_no
                         test == 0x68206C20 ||// h l
                         test == 0x68207220 ||// h r
                         test == 0x68206C5F ||// h l_
                         test == 0x6820725F ||// h r_
                         test == 0x685F6C5F ||// h_l_
                         test == 0x685F725F ||// h_r_
                         test == 0x73686164 )  //shad
                    {
                        break;
                    }
                    else if ( test == 0x67666448 ) // gfdH
                    {
                        foundProperty = true;
                        reader.SeekCurrent( -1 );
                        break;
                    }

                    reader.SeekCurrent( -3 );
                }

                skipProperties = !foundProperty;
                reader.SeekCurrent( -0xE );
                if ( testCount == MAX_TEST_COUNT )
                {
                    throw new Exception( "Can't handle particle attachment" );
                }
            }

            if ( !skipEplData )
            {
                var endOffset = reader.Position;
                var byteCount = ( int )( endOffset - startOffset );
                reader.SeekBegin( startOffset );
                epl.Raw = reader.ReadBytes( byteCount );
            }

            //epl.Field20 = ReadInt32() | 4;
            //epl.Field2C = ReadNodeRecursive( version );
            //epl.Field30 = ReadEplAnimation( version );

            //if ( version > 0x1105060 )
            //{
            //    epl.Field40 = ReadShort();
            //}

            return epl;
        }

        internal override void Read( ResourceReader reader )
        {
            throw new System.NotImplementedException();
        }

        internal override void Write( ResourceWriter writer )
        {
            writer.WriteBytes( Raw );
        }
    }
}