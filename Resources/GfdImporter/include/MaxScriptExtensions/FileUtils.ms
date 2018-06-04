
/* 	Code for easy file handling from within MaxScript
	Written by TGE. Please give credit where is due.
*/

struct FileInfo
(
	public FilePath,
	public FileExtension,
	public FileName,
	public FileDirectory,
	public FileExists,
	public FileStream,
	public FileLength
)

-- FileUtils is a utility class providing methods to ease working with files
struct FileUtils
(
	-- Creates and runs a file selection dialog.
	-- Returns the file path of the selected file; value is undefined if no file was selected.
	public fn RunOpenFileDialog aCaption aTypes aHistoryCategory = (),
	
	-- Creates a file and opens it for writing.
	-- Returns a writeable file stream.
	public fn CreateWriteableFileStream aFileName = (),
	
	-- Returns the length of given file stream.
	public fn GetFileStreamLength aFileStream = (),
	
	-- Returns extended info about a file path.
	-- If aFileMode is supplied then the file will be open using the specified mode.
	public fn GetFileInfo aFilePath aFileMode: = (),
	
	-- Copies a specified amount of bytes directly from the source file stream into the destination file stream
	public fn FileStreamBlockCopy aSrcFileStream aDestFileStream aNumBytesToCopy = (),
	
	-- Creates a new writeable file, copies a specified amount of bytes directly from the source file stream into the destination file stream and closes the new stream
	public fn FileStreamBlockCopyToFile aSrcFileStream aDestFilePath aNumBytesToCopy = (),
	
	--
	-- Implementation
	--
	public fn RunOpenFileDialog aCaption aTypes aHistoryCategory =
	(
		local fname = GetOpenFileName \ 
		caption:aCaption \
		types:aTypes \
		historyCategory:aHistoryCategory
		
		fname
	),
	
	public fn CreateWriteableFileStream aFileName =
	(
		local outFStream = CreateFile aFileName
		Close outFStream
	
		outFStream = FOpen aFileName "wb"
	),
	
	public fn GetFileStreamLength aFileStream =
	(
		local result = undefined
		if (aFileStream != undefined) then
		(
			FSeek aFileStream 0 #seek_end
			result = FTell aFileStream
			FSeek aFileStream 0 #seek_set
		)
		
		result
	),
	
	public fn GetFileInfo aFilePath aFileMode: =
	(
		local result = FileInfo()
		result.FilePath 		= aFilePath
		result.FileExtension 	= GetFilenameType 	aFilePath
		result.FileName 		= GetFileNameFile	aFilePath
		result.FileDirectory 	= GetFilenamePath	aFilePath
		result.FileExists 		= DoesFileExist 	aFilePath
		
		if (aFileMode != unsupplied and result.FileExists) then
		(
			result.FileStream = FOpen aFilePath aFileMode
			result.FileLength = GetFileStreamLength(result.FileStream)
		)
		
		result
	),
	
	public fn FileStreamBlockCopy aSrcFileStream aDestFileStream aNumBytesToCopy =
	(
		local numLongLongs = aNumBytesToCopy / 8
		local numBytesLeft = mod aNumBytesToCopy 8
		
		for i = 1 to numLongLongs do
		(
			WriteLongLong aDestFileStream (ReadLongLong aSrcFileStream)
		)
		
		for i = 1 to numBytesLeft do
		(
			WriteByte aDestFileStream (ReadByte aSrcFileStream)
		)
	),
	
	public fn FileStreamBlockCopyToFile aSrcFileStream aDestFilePath aNumBytesToCopy =
	(
		local destFileStream = FileUtils.CreateWriteableFileStream aDestFilePath
		FileUtils.FileStreamBlockCopy aSrcFileStream destFileStream aNumBytesToCopy
		FClose destFileStream
	)
)