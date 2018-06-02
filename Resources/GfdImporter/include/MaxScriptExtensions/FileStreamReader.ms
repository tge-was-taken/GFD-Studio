
/* 	Code for easy binary file parsing tailored towards 3d model formats from within MaxScript
	Written by TGE. Please give credit where is due.
*/

FileIn "FileUtils.ms"

-- The FileStreamReader provides extensive reading functions for use with files and file streams
struct FileStreamReader 
(
	private m_StreamLength,
	private m_Stream,
	private m_IsBigEndian,
	private m_DisableRangeCheck,
	
	-- Initializes a FileStreamReader
	on create do
	(
		m_Stream = undefined
		m_StreamLength = 0
		m_IsBigEndian = false
		m_DisableRangeCheck = false
	),

	-- Gets the position in the stream
	public fn GetPosition = (),
	
	-- Gets the length of the file stream
	public fn GetFileStreamLength = (),
	
	-- Gets if the reader is in big endian mode
	public fn GetIsBigEndian = (),
	
	-- Gets the file stream
	public fn GetFileStream = (),
	
	-- Sets the file to read
	public fn SetFile aFilePath aIsBigEndian = (),
	
	-- Sets the file stream to read
	public fn SetFileStream aStream aIsBigEndian = (),
	
	-- Sets the boolean value indicating if the reader is in big endian mode
	public fn SetBigEndian aIsBigEndian = (),
	
	-- Seeks to a position in the stream relative to the start position 
	public fn SeekSet aPosition = (),
	
	-- Seeks to a position in the stream relative to the current position 
	public fn SeekCur aPosition = (),
	
	-- Closes the file stream 
	public fn Close = (),
	
	-- Read primitive values
	public fn ReadS8 = (),
	public fn ReadU8 = (),
	public fn ReadS16 = (),
	public fn ReadU16 = (),
	public fn ReadS32 = (),
	public fn ReadU32 = (),
	public fn ReadF16 = (),
	public fn ReadF32 = (),
	
	-- Read Vector2 values
	private fn ReadVec2 aElementSize aElementReadCallback = (),
	public fn ReadVec2S8 = (),	
	public fn ReadVec2U8 = (),
	public fn ReadVec2S16 = (),	
	public fn ReadVec2U16 = (),	
	public fn ReadVec2S32 = (),	
	public fn ReadVec2U32 = (),	
	public fn ReadVec2F16 = (),	
	public fn ReadVec2F32 = (),
	
	-- Read Vector3 values
	private fn ReadVec3 aElementSize aElementReadCallback = (),
	public fn ReadVec3S8 = (),	
	public fn ReadVec3U8 = (),	
	public fn ReadVec3S16 = (),	
	public fn ReadVec3U16 = (),	
	public fn ReadVec3S32 = (),	
	public fn ReadVec3U32 = (),	
	public fn ReadVec3F16 = (),	
	public fn ReadVec3F32= (),
	
	-- Read Vector4 values
	private fn ReadVec4 aElementSize aElementReadCallback = (),
	public fn ReadVec4S8 = (),	
	public fn ReadVec4U8 = (),	
	public fn ReadVec4S16 = (),	
	public fn ReadVec4U16 = (),	
	public fn ReadVec4S32 = (),	
	public fn ReadVec4U32 = (),	
	public fn ReadVec4F16 = (),	
	public fn ReadVec4F32= (),
	
	-- Read Quaternion values
	public fn ReadQuatF16 = (),	
	public fn ReadQuatF32 = (),
	
	-- Read Matrix values
	public fn ReadMtx43 = (),			
	public fn ReadMtx44 = (),
	
	-- Read strings
	public fn ReadCString = (),
	public fn ReadCStringPtr = (),
	public fn ReadFixedLengthString aStringLength = (),
	public fn ReadU8PrefixedLengthString = (),	
	public fn ReadU16PrefixedLengthString = (),	
	public fn ReadU32PrefixedLengthString = (),
	
	-- Read generic array
	public fn ReadArray aNumElements aElementSize aElementReadCallback = (),	
	
	-- Read primitive array
	public fn ReadS8Array aNumElements = (),	
	public fn ReadU8Array aNumElements = (),	
	public fn ReadS16Array aNumElements = (),	
	public fn ReadU16Array aNumElements = (),	
	public fn ReadS32Array aNumElements = (),
	public fn ReadU32Array aNumElements = (),	
	public fn ReadF16Array aNumElements = (),	
	public fn ReadF32Array aNumElements = (),	
	public fn ReadMtx43Array aNumElements = (),
	public fn ReadMtx44Array aNumElements = (),	
	
	-- Read Vector2 array
	public fn ReadVec2S8Array aNumElements = (),	
	public fn ReadVec2U8Array aNumElements = (),	
	public fn ReadVec2S16Array aNumElements = (),	
	public fn ReadVec2U16Array aNumElements = (),	
	public fn ReadVec2S32Array aNumElements = (),	
	public fn ReadVec2U32Array aNumElements = (),	
	public fn ReadVec2F16Array aNumElements = (),
	public fn ReadVec2F32Array aNumElements = (),
	
	-- Read Vector3 array
	public fn ReadVec3S8Array aNumElements = (),
	public fn ReadVec3U8Array aNumElements = (),	
	public fn ReadVec3S16Array aNumElements = (),	
	public fn ReadVec3U16Array aNumElements = (),	
	public fn ReadVec3S32Array aNumElements = (),	
	public fn ReadVec3U32Array aNumElements = (),	
	public fn ReadVec3F16Array aNumElements = (),
	public fn ReadVec3F32Array aNumElements = (),
	
	-- Read Vector4 array
	public fn ReadVec4S8Array aNumElements = (),
	public fn ReadVec4U8Array aNumElements = (),
	public fn ReadVec4S16Array aNumElements = (),
	public fn ReadVec4U16Array aNumElements = (),
	public fn ReadVec4S32Array aNumElements = (),
	public fn ReadVec4U32Array aNumElements = (),
	public fn ReadVec4F16Array aNumElements = (),
	public fn ReadVec4F32Array aNumElements = (),
	
	-- Debug assertion functions
	private fn AssertStreamBoundsForRead aReadSize = (),
	private fn AssertStreamBoundsForSeek aSeekLength = (),
	
	--
	-- Implementation
	--
	public fn GetPosition =
	(
		if (m_Stream == undefined) then
		(
			throw "FileStreamReader.GetPosition: m_Stream is undefined." 
		)
		else
		(
			return FTell m_Stream
		)
	),
	
	public fn GetFileStreamLength =
	(
		m_StreamLength
	),
	
	public fn GetIsBigEndian =
	(
		m_IsBigEndian
	),
	
	public fn GetFileStream =
	(
		m_Stream
	),
	
	public fn SetFile aFilePath aIsBigEndian = 
	(
		m_Stream = FOpen aFilePath "rb"
		m_StreamLength = FileUtils.GetFileStreamLength m_Stream
		m_IsBigEndian = aIsBigEndian
	),
	
	public fn SetFileStream aStream aIsBigEndian = 
	(
		m_Stream = aStream
		m_StreamLength = FileUtils.GetFileStreamLength m_Stream
		m_IsBigEndian = aIsBigEndian
	),
	
	public fn SetBigEndian aIsBigEndian =
	(
		m_IsBigEndian = aIsBigEndian
	),
	
		
	public fn SeekSet aPosition =
	(
		AssertStreamBoundsForSeek(aPosition)
		FSeek m_Stream aPosition #seek_set
	),
	
	public fn SeekCur aPosition =
	(
		AssertStreamBoundsForSeek((GetPosition()) + aPosition)
		FSeek m_Stream aPosition #seek_cur
	),
	
	public fn Close =
	(
		FClose m_Stream
		m_StreamLength = 0
		m_Stream = undefined
		m_DisableRangeCheck = false
	),
	
	public fn ReadS8 = 
	(
		if (not m_DisableRangeCheck) then AssertStreamBoundsForRead(1)
		ReadByte m_Stream #signed
	),
	
	public fn ReadU8 = 
	(
		if (not m_DisableRangeCheck) then AssertStreamBoundsForRead(1)
		ReadByte m_Stream #unsigned
	),
	
	public fn ReadS16 = 
	(
		if (not m_DisableRangeCheck) then AssertStreamBoundsForRead(2)
		local ret = ReadShort m_Stream #signed
		
		if (m_IsBigEndian) then
			ret = Bit.SwapBytes ret 1 2
		
		ret
	),
	
	public fn ReadU16 = 
	(
		if (not m_DisableRangeCheck) then AssertStreamBoundsForRead(2)
		local ret = ReadShort m_Stream #unsigned
		
		if (m_IsBigEndian) then
			ret = Bit.SwapBytes ret 1 2
		
		ret
	),
	
	public fn ReadS32 = 
	(
		if (not m_DisableRangeCheck) then AssertStreamBoundsForRead(4)
		local ret = ReadLong m_Stream #signed
		
		if (m_IsBigEndian) then
			ret = Bit.SwapBytes (Bit.SwapBytes ret 1 4) 2 3
		
		ret
	),
	
	public fn ReadU32 = 
	(
		if (not m_DisableRangeCheck) then AssertStreamBoundsForRead(4)
		local ret = ReadLong m_Stream #signed
		
		if (m_IsBigEndian) then
			ret = Bit.SwapBytes (Bit.SwapBytes ret 1 4) 2 3
		
		ret
	),
	
	public fn ReadF16 =
	(
		if (not m_DisableRangeCheck) then AssertStreamBoundsForRead(2)
		local N = 0
		if (m_IsBigEndian) then
		(
			local BH = ReadByte m_Stream #unsigned
			local BL = ReadByte m_Stream #unsigned
			N = BH * 256 + BL
		)
		else
		(
			N = ReadShort m_Stream #unsigned
		)
		
		local S = Floor((Mod N 65536) / 32768)
		local Ef = Floor((Mod N 32768) / 1024)
		local M = Mod N 1024
		local result = 0
		
		if (Ef == 0) AND (M == 0) 
			then result = ( (-1.0)^S * 0.0 )
		
		else if (Ef == 0) AND (M != 0) 
			then result = ( (-1.0)^S * 2.0^-14 * (M / 2.0^10) )
		
		else if (Ef > 0) AND (Ef < 31) 
			then result = ( (-1.0)^S * 2.0^(Ef-15) * (1 + M/2.0^10) )
		
		else if (Ef == 31) AND (M == 0) 
			then result = ( (-1.0)^S * 1/0.0 )
		
		result
	),
	
	public fn ReadF32 =
	(
		if (not m_DisableRangeCheck) then AssertStreamBoundsForRead(4)
		local ret = undefined
		if (m_IsBigEndian) then
		(
			ret = Bit.IntAsFloat (Bit.SwapBytes (Bit.SwapBytes (ReadLong m_Stream #unsigned) 1 4) 2 3)
		)
		else
		(
			ret = ReadFloat m_Stream
		)
		
		ret
	),
	
	private fn ReadVec2 aElementSize aElementReadCallback =
	(
		if (not m_DisableRangeCheck) then 
		(
			AssertStreamBoundsForRead(aElementSize * 2)
			m_DisableRangeCheck = true
		)
		
		local ret = [aElementReadCallback(), aElementReadCallback(), 0]
		if (m_DisableRangeCheck) then 
			m_DisableRangeCheck = false
		
		ret
	),
	
	private fn ReadVec3 aElementSize aElementReadCallback =
	(
		if (not m_DisableRangeCheck) then 
		(
			AssertStreamBoundsForRead(aElementSize * 3)
			m_DisableRangeCheck = true
		)
		
		local ret = [aElementReadCallback(), aElementReadCallback(), aElementReadCallback()]
		if (m_DisableRangeCheck) then 
			m_DisableRangeCheck = false
		
		ret
	),
	
	private fn ReadVec4 aElementSize aElementReadCallback =
	(
		if (not m_DisableRangeCheck) then 
		(
			AssertStreamBoundsForRead(aElementSize * 4)
			m_DisableRangeCheck = true
		)
		
		local ret = [aElementReadCallback(), aElementReadCallback(), aElementReadCallback(), aElementReadCallback()]
		if (m_DisableRangeCheck) then 
			m_DisableRangeCheck = false
		
		ret
	),
	
	public fn ReadVec2S8 =
	(
		ReadVec2 1 ReadS8
	),
	
	public fn ReadVec2U8 =
	(
		ReadVec2 1 ReadU8
	),
	
	public fn ReadVec2S16 =
	(
		ReadVec2 2 ReadS16
	),
	
	public fn ReadVec2U16 =
	(
		ReadVec2 2 ReadU16
	),
	
	public fn ReadVec2S32 =
	(
		ReadVec2 4 ReadS32
	),
	
	public fn ReadVec2U32 =
	(
		ReadVec2 4 ReadU32
	),
	
	public fn ReadVec2F16 =
	(
		ReadVec2 2 ReadF16
	),
	
	public fn ReadVec2F32 =
	(
		ReadVec2 4 ReadF32
	),
	
	-- Vector 3
	public fn ReadVec3S8 =
	(
		ReadVec3 1 ReadS8
	),
	
	public fn ReadVec3U8 =
	(
		ReadVec3 1 ReadU8
	),
	
	public fn ReadVec3S16 =
	(
		ReadVec3 2 ReadS16
	),
	
	public fn ReadVec3U16 =
	(
		ReadVec3 2 ReadU16
	),
	
	public fn ReadVec3S32 =
	(
		ReadVec3 4 ReadS32
	),
	
	public fn ReadVec3U32 =
	(
		ReadVec3 4 ReadU32
	),
	
	public fn ReadVec3F16 =
	(
		ReadVec3 2 ReadF16
	),
	
	public fn ReadVec3F32=
	(
		ReadVec3 4 ReadF32
	),
	
	-- Vector 4
	public fn ReadVec4S8 =
	(
		ReadVec4 1 ReadS8
	),
	
	public fn ReadVec4U8 =
	(
		ReadVec4 1 ReadU8
	),
	
	public fn ReadVec4S16 =
	(
		ReadVec4 2 ReadS16
	),
	
	public fn ReadVec4U16 =
	(
		ReadVec4 2 ReadU16
	),
	
	public fn ReadVec4S32 =
	(
		ReadVec4 4 ReadS32
	),
	
	public fn ReadVec4U32 =
	(
		ReadVec4 4 ReadU32
	),
	
	public fn ReadVec4F16 =
	(
		ReadVec4 2 ReadF16
	),
	
	public fn ReadVec4F32=
	(
		ReadVec4 4 ReadF32
	),
	
	public fn ReadQuatF16 =
	(
		if (not m_DisableRangeCheck) then 
		(
			AssertStreamBoundsForRead(8)
			m_DisableRangeCheck = true
		)
		
		local ret = quat (ReadF16()) (ReadF16()) (ReadF16()) (ReadF16())
		if (m_DisableRangeCheck) then 
			m_DisableRangeCheck = false
				
		ret
	),
    
	public fn ReadQuatF16NoW =
	(
		if (not m_DisableRangeCheck) then 
		(
			AssertStreamBoundsForRead(8)
			m_DisableRangeCheck = true
		)
		
		local ret = quat (ReadF16()) (ReadF16()) (ReadF16()) (1.0f)
		if (m_DisableRangeCheck) then 
			m_DisableRangeCheck = false
				
		ret
	),
	
	public fn ReadQuatF32 =
	(
		if (not m_DisableRangeCheck) then 
		(
			AssertStreamBoundsForRead(16)
			m_DisableRangeCheck = true
		)
		
		local ret = quat (ReadF32()) (ReadF32()) (ReadF32()) (ReadF32())
		if (m_DisableRangeCheck) then 
			m_DisableRangeCheck = false
				
		ret
	),

	public fn ReadMtx43 =
	(
		if (not m_DisableRangeCheck) then 
		(
			AssertStreamBoundsForRead(48)
			m_DisableRangeCheck = true
		)
		
		local ret = matrix3 (ReadVec4F32()) (ReadVec4F32()) (ReadVec4F32())
		if (m_DisableRangeCheck) then 
			m_DisableRangeCheck = false
				
		ret
	),
			
	public fn ReadMtx44 =
	(
		if (not m_DisableRangeCheck) then 
		(
			AssertStreamBoundsForRead(64)
			m_DisableRangeCheck = true
		)
		
		local ret = matrix3 (ReadVec4F32()) (ReadVec4F32()) (ReadVec4F32()) (ReadVec4F32())
		if (m_DisableRangeCheck) then 
			m_DisableRangeCheck = false
				
		ret
	),
	
	public fn ReadCString =
	(
		local str = ""
		local char = bit.intaschar ReadU8()
		while (char != '\0') do
		(
			str += char
			char = bit.intaschar ReadU8()
		)
		
		str
	),
	
	public fn ReadCStringPtr =
	(
		local ptr = ReadU32()
		local seekBack = GetPosition()
		local str = undefined
		
		if (ptr != 0) then
		(
			SeekSet(ptr)
			str = ReadCString()
			SeekSet(seekBack)
		)
		
		str
	),
	
	public fn ReadFixedLengthString aStringLength =
	(
		if (not m_DisableRangeCheck) then 
		(
			AssertStreamBoundsForRead(aStringLength)
			m_DisableRangeCheck = true
		)
		
		local str = ""
		for i = 1 to aStringLength do
			str += bit.intaschar (ReadU8())
		
		if (m_DisableRangeCheck) then 
			m_DisableRangeCheck = false
		
		str
	),
	
	public fn ReadU8PrefixedLengthString =
	(
		local len = ReadU8()
		if (not m_DisableRangeCheck) then 
		(
			AssertStreamBoundsForRead(len)
			m_DisableRangeCheck = true
		)
		
		local ret = ReadFixedLengthString len

		if (m_DisableRangeCheck) then 
			m_DisableRangeCheck = false	

		ret
	),
	
	public fn ReadU16PrefixedLengthString =
	(
		local len = ReadU16()
		if (not m_DisableRangeCheck) then 
		(
			AssertStreamBoundsForRead(len)
			m_DisableRangeCheck = true
		)
		
		local ret = ReadFixedLengthString len

		if (m_DisableRangeCheck) then 
			m_DisableRangeCheck = false	

		ret
	),
	
	public fn ReadU32PrefixedLengthString =
	(
		local len = ReadU32()
		if (not m_DisableRangeCheck) then 
		(
			AssertStreamBoundsForRead(len)
			m_DisableRangeCheck = true
		)
		
		local ret = ReadFixedLengthString len

		if (m_DisableRangeCheck) then 
			m_DisableRangeCheck = false	

		ret
	),
	
	public fn ReadArray aNumElements aElementSize aElementReadCallback =
	(
		if (not m_DisableRangeCheck) then
		(
			AssertStreamBoundsForRead(aNumElements * aElementSize)
			m_DisableRangeCheck = true
		)
		
		local arr = #()
		arr[aNumElements] = 0
		for i = 1 to aNumElements do
			arr[i] = aElementReadCallback()
		if (m_DisableRangeCheck) then m_DisableRangeCheck = false
		
		arr
	),
	
	public fn ReadS8Array aNumElements =
	(
		ReadArray aNumElements 1 ReadS8
	),
	
	public fn ReadU8Array aNumElements =
	(
		ReadArray aNumElements 1 ReadU8
	),	
	
	public fn ReadS16Array aNumElements =
	(
		ReadArray aNumElements 2 ReadS16
	),
	
	public fn ReadU16Array aNumElements =
	(
		ReadArray aNumElements 2 ReadU16
	),
	
	public fn ReadS32Array aNumElements =
	(
		ReadArray aNumElements 4 ReadS32
	),
	
	public fn ReadU32Array aNumElements =
	(
		ReadArray aNumElements 4 ReadU32
	),
	
	public fn ReadF16Array aNumElements =
	(
		ReadArray aNumElements 2 ReadF16
	),
	
	public fn ReadF32Array aNumElements =
	(
		ReadArray aNumElements 4 ReadF32
	),
	
	public fn ReadMtx43Array aNumElements =
	(
		ReadArray aNumElements 48 ReadMtx43
	),

	public fn ReadMtx44Array aNumElements =
	(
		ReadArray aNumElements 64 ReadMtx44
	),
	
	public fn ReadVec2S8Array aNumElements =
	(
		ReadArray aNumElements 3 ReadVec2S8
	),
	
	public fn ReadVec2U8Array aNumElements =
	(
		ReadArray aNumElements 3 ReadVec2U8
	),
	
	public fn ReadVec2S16Array aNumElements =
	(
		ReadArray aNumElements 6 ReadVec2S16
	),
	
	public fn ReadVec2U16Array aNumElements =
	(
		ReadArray aNumElements 6 ReadVec2U16
	),
	
	public fn ReadVec2S32Array aNumElements =
	(
		ReadArray aNumElements 12 ReadVec2S32
	),
	
	public fn ReadVec2U32Array aNumElements =
	(
		ReadArray aNumElements 12 ReadVec2U32
	),
	
	public fn ReadVec2F16Array aNumElements =
	(
		ReadArray aNumElements 6 ReadVec2F16
	),	

	public fn ReadVec2F32Array aNumElements =
	(
		ReadArray aNumElements 12 ReadVec2F32
	),	
	
	public fn ReadVec3S8Array aNumElements =
	(
		ReadArray aNumElements 3 ReadVec3S8
	),
	
	public fn ReadVec3U8Array aNumElements =
	(
		ReadArray aNumElements 3 ReadVec3U8
	),
	
	public fn ReadVec3S16Array aNumElements =
	(
		ReadArray aNumElements 6 ReadVec3S16
	),
	
	public fn ReadVec3U16Array aNumElements =
	(
		ReadArray aNumElements 6 ReadVec3U16
	),
	
	public fn ReadVec3S32Array aNumElements =
	(
		ReadArray aNumElements 12 ReadVec3S32
	),
	
	public fn ReadVec3U32Array aNumElements =
	(
		ReadArray aNumElements 12 ReadVec3U32
	),
	
	public fn ReadVec3F16Array aNumElements =
	(
		ReadArray aNumElements 6 ReadVec3F16
	),	

	public fn ReadVec3F32Array aNumElements =
	(
		ReadArray aNumElements 12 ReadVec3F32
	),	
	
	public fn ReadVec4S8Array aNumElements =
	(
		ReadArray aNumElements 3 ReadVec4S8
	),
	
	public fn ReadVec4U8Array aNumElements =
	(
		ReadArray aNumElements 3 ReadVec4U8
	),
	
	public fn ReadVec4S16Array aNumElements =
	(
		ReadArray aNumElements 6 ReadVec4S16
	),
	
	public fn ReadVec4U16Array aNumElements =
	(
		ReadArray aNumElements 6 ReadVec4U16
	),
	
	public fn ReadVec4S32Array aNumElements =
	(
		ReadArray aNumElements 12 ReadVec4S32
	),
	
	public fn ReadVec4U32Array aNumElements =
	(
		ReadArray aNumElements 12 ReadVec4U32
	),
	
	public fn ReadVec4F16Array aNumElements =
	(
		ReadArray aNumElements 6 ReadVec4F16
	),	

	public fn ReadVec4F32Array aNumElements =
	(
		ReadArray aNumElements 12 ReadVec4F32
	),
	
	private fn AssertStreamBoundsForRead aReadSize =
	(
		if (m_Stream == undefined) then
		(
			throw "FileStreamReader.AssertStreamBoundsForRead: m_Stream is undefined." 
		)
		else if (aReadSize < 0) then
		(
			throw "FileStreamReader.AssertStreamBoundsForRead: aReadSize is negative." 
		)
		else if ((FTell m_Stream) + aReadSize > m_StreamLength) then
		(
			throw "FileStreamReader.AssertStreamBoundsForRead: Tried to read past end of stream. Stream offset (hex): " + HexStr(GetPosition())
		)
	),
	
	private fn AssertStreamBoundsForSeek aSeekLength =
	(
		if (m_Stream == undefined) then
		(
			throw "FileStreamReader.AssertStreamBoundsForSeek: m_Stream is undefined." 
		)
		else if (aSeekLength < 0) then
		(
			throw ("FileStreamReader.AssertStreamBoundsForSeek: Tried to seek past beginning of stream. Stream offset: " + HexStr(GetPosition()))
		)
		else if (aSeekLength > m_StreamLength) then
		(
			throw ("FileStreamReader.AssertStreamBoundsForSeek: Tried to seek past end of stream. Stream offset: " + HexStr(GetPosition()))
		)
	)
)