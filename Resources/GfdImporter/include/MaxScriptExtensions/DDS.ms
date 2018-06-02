
/* 	Code for loading and saving DDS headers from within MaxScript
	Written by TGE. Please give credit where is due.
*/

DDSMagic		= 0x20534444
DDSHeaderSize	= 124

-- Flags
DDSFlagCaps 			= 0x00000001
DDSFlagHeight 			= 0x00000002
DDSFlagWidth 			= 0x00000004
DDSFlagPitch 			= 0x00000008
DDSFlagPixelFormat		= 0x00001000
DDSFlagMipMapCount		= 0x00020000
DDSFlagLinearSize 		= 0x00080000
DDSFlagDepth 			= 0x00800000
DDSFlagTexture			= (bit.or (bit.or (bit.or DDSFlagCaps DDSFlagHeight) DDSFlagWidth) DDSFlagPixelFormat)
DDSFlagTextureMipMap	= (bit.or DDSFlagTexture DDSFlagMipMapCount)

-- Caps 
DDSCapsComplex			= 0x00000008
DDSCapsTexture			= 0x00001000
DDSCapsMipMap			= 0x00400000
DDSCapsTextureMipMap	= bit.or (bit.or DDSCapsComplex DDSCapsMipMap) DDSCapsTexture

-- Pixel format 
DDSPixelFormatHeaderSize 		= 32
DDSPixelFormatFlagAlphaPixels 	= 0x00000001
DDSPixelFormatFlagAlpha 		= 0x00000002
DDSPixelFormatFlagFourCC 		= 0x00000004
DDSPixelFormatFlagRGB 			= 0x00000040
DDSPixelFormatFlagYUV 			= 0x00000200
DDSPixelFormatFlagLuminance 	= 0x00020000
DDSPixelFormatFourCCDXT1 		= 0x31545844
DDSPixelFormatFourCCDXT3 		= 0x33545844
DDSPixelFormatFourCCDXT5 		= 0x35545844

struct DDSHeader
(
	public Size,
	public Flags,
	public Height,
	public Width,
	public PitchOrLinearSize,
	public Depth,
	public MipMapCount,
	public Reserved1,
	public PixelFormatHeaderSize,
	public PixelFormatFlags,
	public PixelFormatFourCC,
	public PixelFormatRGBBitCount,
	public PixelFormatRBitMask,
	public PixelFormatGBitMask,
	public PixelFormatBBitMask,
	public PixelFormatABitMask,
	public Caps,
	public Caps2,
	public Caps3,
	public Caps4,
	public Reserved2,
			
	-- Initializes a DDS header with default settings
	on create do
	(
		Size = DDSHeaderSize
		Flags = DDSFlagTextureMipMap
		Height = 0
		Width = 0
		PitchOrLinearSize = 0
		Depth = 0
		MipMapCount = 1
		Reserved1 = #()
		for i = 1 to 11 do ( Reserved1[i] = 0 )
		PixelFormatHeaderSize = DDSPixelFormatHeaderSize
		PixelFormatFlags = DDSPixelFormatFlagFourCC
		PixelFormatFourCC = DDSPixelFormatFourCCDXT1
		PixelFormatRGBBitCount = 0
		PixelFormatRBitMask = 0
		PixelFormatGBitMask = 0
		PixelFormatBBitMask = 0
		PixelFormatABitMask = 0
		Caps = DDSCapsTextureMipMap
		Caps2 = 0
		Caps3 = 0
		Caps4 = 0
		Reserved2 = 0
	),
	
	-- Reads the DDS header data from the provided stream
	public fn ReadFromStream stream = (),
	
	-- Writes the DDS header data to the provided stream
	public fn WriteToStream stream = (),

	--
	-- Implementation
	--
	public fn ReadFromStream stream =
	(
		local magic = ReadLong stream
		if (magic != DDSMagic) then
		(
			throw "DDSHeader.FromStream: DDS signature is not valid.\n"
		)
		
		Size = ReadLong stream #unsigned
		
		if (Size != DDSHeaderSize) then
		(
			throw "DDSHeader.FromStream: DDSHeader.Size is not equal to 124.\n"
		)
		
		Flags = ReadLong stream #unsigned
		Height = ReadLong stream #unsigned
		Width = ReadLong stream #unsigned
		PitchOrLinearSize = ReadLong stream #unsigned
		Depth = ReadLong stream #unsigned
		MipMapCount = ReadLong stream #unsigned
		for i = 1 to 11 do ( Reserved1[i] = ReadLong stream #unsigned )
		PixelFormatHeaderSize = ReadLong stream #unsigned
		PixelFormatFlags = ReadLong stream #unsigned
		PixelFormatFourCC = ReadLong stream #unsigned
		PixelFormatRGBBitCount = ReadLong stream #unsigned
		PixelFormatRBitMask = ReadLong stream #unsigned
		PixelFormatGBitMask = ReadLong stream #unsigned
		PixelFormatBBitMask = ReadLong stream #unsigned
		PixelFormatABitMask = ReadLong stream #unsigned
		Caps = ReadLong stream #unsigned
		Caps2 = ReadLong stream #unsigned
		Caps3 = ReadLong stream #unsigned
		Caps4 = ReadLong stream #unsigned
		Reserved2 = ReadLong stream #unsigned
	),

	public fn WriteToStream stream =
	(
		WriteLong stream DDSMagic
		WriteLong stream Size
		WriteLong stream Flags
		WriteLong stream Height
		WriteLong stream Width
		WriteLong stream PitchOrLinearSize
		WriteLong stream Depth
		WriteLong stream MipMapCount
		for i = 1 to 11 do ( WriteLong stream Reserved1[i] )
		WriteLong stream PixelFormatHeaderSize
		WriteLong stream PixelFormatFlags
		WriteLong stream PixelFormatFourCC
		WriteLong stream PixelFormatRGBBitCount
		WriteLong stream PixelFormatRBitMask
		WriteLong stream PixelFormatGBitMask
		WriteLong stream PixelFormatBBitMask
		WriteLong stream PixelFormatABitMask
		WriteLong stream Caps
		WriteLong stream Caps2
		WriteLong stream Caps3
		WriteLong stream Caps4
		WriteLong stream Reserved2
	)
)
