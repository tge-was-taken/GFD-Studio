
/* 	Persona 4: DAN & Persona 5 model and animation importer (*.gmd; *.gfs; *.gap) written in MaxScript
    Script by TGE. Please give credit where is due.
    Special thanks to PolarSoda, mrdudesworkshop
*/

FileIn "MaxScriptExtensions\\Utils.ms"
FileIn "MaxScriptExtensions\\FileUtils.ms"
FileIn "MaxScriptExtensions\\FileStreamReader.ms"
FileIn "MaxScriptExtensions\\DDS.ms"
FileIn "MaxScriptExtensions\\Stack.ms"
FileIn "GfdFormatDeclaration.ms"

if (heapSize < 200000) then
    heapSize = 2000000 -- allow ~ 40 MB instead of just 7.5 MB. Prevents "Runtime Error: Out of scripter memory"

-- Loader for Gfd formatted files
struct GfdFileLoader
(
    private Y_TO_Z_UP_MATRIX = matrix3 [1, 0, 0] [0, 0, 1] [0, -1, 0] [0, 0, 0],
    
    private mModel = undefined,
    private mFileInfo = undefined,
    private mReader = undefined,
    private mLoadExternalTextures = false,
    
    public ApplyNormals = true,
    public ApplyColors = true,
    public ApplySkin = true,
    public ApplyYToZUp = true,
    public OverwriteExistingTextures = false,
    public ExportTexturesOnly = false,
    public LoadAnimationOnly = false,
    
    -- Forward declarations
    public fn IsValidFile aFilePath = (),
    public fn Load aFilePath = (),
    
    -- Animation
    private fn ReadAnimationPackage chunkDesc = (),
    private fn ReadAnimation version = (),
    private fn ReadAnimationExtraData version = (),
    private fn ReadAnimationController version = (),
    private fn ReadAnimationKeyframeTrack version = (),
    private fn ReadAnimationKeyframe version type = (),
    
    -- Texture 
    private fn ReadTextureList chunkDesc = (),
    private fn ReadFieldTexturePack chunkDesc = (),
    private fn ReadFieldTexture ddsOutPath texPakStream = (),
    
    -- Material
    private fn ReadMaterialList chunkDesc = (),
    private fn TestMaterialEndSkip endSkip isLast = (),
    
    -- Scene 
    private fn ReadScene chunkDesc = (),
    private fn ReadNode = (),
    private fn ReadGeometry = (),
    private fn ReadMorphTargetList version = (),
    private fn ReadCamera = (),
    private fn ReadLight = (),
    private fn ReadParticle = (),
    private fn ReadUserProperties = (),
    
    -- Setup
    public fn SetupAnimation anim isBlend = (),
    public fn SetupScene gfdModel = (),
    private fn SetupNodeBone gfdNode = (),
    
    -- Static
    -- Returns bool indicating whether the file is valid or not.
    public fn IsValidFile filePath =
    (
        local result = false
        local reader = FileStreamReader()
        reader.SetFile filePath true
        
        if (reader.GetFileStreamLength() > 4) then
        (
            local fileMagic = reader.ReadU32()
            reader.SeekCur -4
            if (fileMagic == GfdModelMagic) then
                result = true
        )
        
        reader.Close()
        
        result
    ),
    
    -- Returns GfdModel instance
    public fn Load aFilePath =
    (
        mFileInfo = FileUtils.GetFileInfo aFilePath
        
        -- Set up reader
        mReader = FileStreamReader()
        mReader.SetFile aFilePath true
        mReader.SeekCur(16) -- header
        
        -- Start reading
        mModel = GfdModel()
        if ((mFileInfo.FileExtension == ".GFS") and (DoesDirectoryExist (mFileInfo.FileDirectory + "textures"))) then
        (
            mLoadExternalTextures = true
        )
        do
        (
            local desc = GfdChunkDescriptor()
            desc.StartOffset = mReader.GetPosition()
            desc.Version = mReader.ReadU32()			
            desc.Type = mReader.ReadU32()
            desc.Size = mReader.ReadU32()
            Format "GfdFileLoader.Load: Chunk % ver: % sz: % at %\n" (HexStr(desc.Type)) (HexStr(desc.Version)) (HexStr(desc.Size)) (HexStr(desc.StartOffset))
            
            case (desc.Type) of
            (
                GfdChunkType_AnimationPackage:
                (
                    try
                    (
                        mModel.AnimationPackage = ReadAnimationPackage(desc)
                    )
                    catch
                    (
                        -- /shrug
                        Format "GfdFileLoader.Load: Failed to load animation package; skipping\n"
                        mReader.SeekSet( desc.StartOffset + desc.Size );
                    )
                    
                    if (LoadAnimationOnly) then
                    (
                        /* HACK: Force stop */
                        desc.Type = GfdChunkType_FileEnd
                    )
                )
                
                GfdChunkType_TextureList:
                (
                    if (not LoadAnimationOnly) then
                    (
                        if (not mLoadExternalTextures) then 
                        (
                            ReadTextureList(desc)
                        )
                        else
                        (
                            ReadFieldTexturePack(desc)
                            mReader.SeekCur (desc.Size - 12)
                        )
                    )
                    else
                    (
                        mReader.SeekCur (desc.Size - 12)
                    )
                )
                
                GfdChunkType_MaterialList:
                (
                    if ((not ExportTexturesOnly) and (not LoadAnimationOnly)) then
                    (
                        ReadMaterialList(desc)
                    )
                    else
                    (
                        mReader.SeekCur (desc.Size - 12)
                    )
                )
                
                GfdChunkType_Scene:
                (
                    if ((not ExportTexturesOnly) and (not LoadAnimationOnly)) then
                    (
                        ReadScene(desc)
                    )
                    else
                    (
                        mReader.SeekCur (desc.Size - 12)
                    )
                )
                
                GfdChunkType_FileEnd:
                    Format "GfdFileLoader.Load: GFS end marker chunk\n"
                
                default:
                (
                    Format "GfdFileLoader.Load: Skipping unknown chunk type %\n" (HexStr(desc.Type))
                    if (desc.Size != 0) then mReader.SeekCur (desc.Size - 12)
                )
            )
        
            Format "\n"
        ) while (desc.Type != GfdChunkType_FileEnd and mReader.GetPosition() < mReader.GetFileStreamLength())
        
        mReader.Close()
        
        mModel
    ),
    
    -- Returns string
    private fn ReadStringWithHash version =
    (
        local str = mReader.ReadU16PrefixedLengthString()
        
        if ( version > 0x1080000 ) then
        (
            local hash = mReader.ReadU32()
        )
        
        -- Return
        str
    ),
    
    -- Returns bounding box
    private fn ReadBoundingBox =
    (
        local bbox = GfdBoundingBox()
        bbox.MaxExtents = mReader.ReadVec3F32()
        bbox.MinExtents = mReader.ReadVec3F32()
        
        -- Return
        bbox
    ),
    
    -- Returns GfdAnimationPackage instance
    private fn ReadAnimationPackage chunkDesc = 
    (
        mReader.SeekCur(4)
        local animationPackage = GfdAnimationPackage()
        
        if ( chunkDesc.Version > 0x1104950 ) then
        (
            animationPackage.Flags = mReader.ReadU32()
        )
        
        animationPackage.AnimationCount = mReader.ReadU32()
        for i = 1 to animationPackage.AnimationCount do
        (
            local animation = ReadAnimation( chunkDesc.Version )
            animationPackage.Animations[i] = animation
            
            -- Hack, return early bc we can't read EPLs right now
            if ((bit.and animation.Flags GfdAnimationFlags_Flag10000000) != 0) then
            (
                return animationPackage
            )
        )
        
        animationPackage.BlendAnimationCount = mReader.ReadU32()
        for i = 1 to animationPackage.BlendAnimationCount do
        (
            local animation = ReadAnimation( chunkDesc.Version )
            animationPackage.BlendAnimations[i] = animation
        )
        
        if ( (bit.and animationPackage.Flags GfdAnimationPackageFlags_Flag4) != 0 ) then
        (
            animationPackage.ExtraData = ReadAnimationExtraData( chunkDesc.Version )
        )
        
        return animationPackage
    ),
    
    -- Returns GfdAnimationExtraData instance
    private fn ReadAnimationExtraData version =
    (
        local extraData = GfdAnimationExtraData()
        
        extraData.Field00 = ReadAnimation( version )
        extraData.Field10 = mReader.ReadF32()
        extraData.Field04 = ReadAnimation( version )
        extraData.Field14 = mReader.ReadF32()
        extraData.Field08 = ReadAnimation( version )
        extraData.Field18 = mReader.ReadF32()
        extraData.Field0C = ReadAnimation( version )
        extraData.Field1C = mReader.ReadF32()
        
        return extraData
    ),
    
    -- Returns GfdAnimation instance
    private fn ReadAnimation version = 
    (
        local animation = GfdAnimation()
        
        if ( version > 0x1104110 ) then
        (
            animation.Flags = mReader.ReadU32()
        )
        
        animation.Duration = mReader.ReadF32()        
        animation.ControllerCount = mReader.ReadU32()
        Format "\t\tGfdFileLoader.ReadAnimation: flg: % duration: % ControllerCount: %\n" (HexStr(animation.Flags)) animation.Duration animation.ControllerCount
        
        for i = 1 to animation.ControllerCount do
        (
            local controller = ReadAnimationController( version )
            animation.Controllers[i] = controller
        )
        
        if ( (bit.and animation.Flags GfdAnimationFlags_Flag10000000) != 0 ) then
        (
            -- Hack, return early bc we can't read EPLs right now
            Format "\t\tGfdFileLoader.ReadAnimation: EPL data present! Giving up on reading any more animations\n"
            return animation
            
            local count = mReader.ReadU32()
            for i = 1 to count do
            (
                local entry = GfdAnimationFlag10000000DataEntry()
                entry.Field00 = ReadEpl( version )
                entry.Field04 = ReadStringWithHash()
                
                animation.Field10[i] = entry
            )
        )
        
        if ( (bit.and animation.Flags GfdAnimationFlags_Flag20000000) != 0 ) then
        (
            animation.Field14 = ReadAnimationExtraData( version )
        )
        
        if ( (bit.and animation.Flags GfdAnimationFlags_Flag80000000) != 0 ) then
        (
            animation.Field1C = GfdAnimationFlag80000000Data()
            animation.Field1C.Field00 = mReader.ReadU32()
            animation.Field1C.Field04 = ReadStringWithHash( version )
            animation.Field1C.Field20 = ReadAnimationKeyframeTrack( version )
        )
        
        if ( (bit.and animation.Flags GfdAnimationFlags_HasBoundingBox) != 0 ) then
        (
            animation.BoundingBox = ReadBoundingBox()
        )
        
        if ( (bit.and animation.Flags GfdAnimationFlags_Flag2000000) != 0 ) then
        (
            animation.Field24 = mReader.ReadF32()
        )
        
        if ( (bit.and animation.Flags GfdAnimationFlags_HasProperties) != 0 ) then
        (
            animation.Properties = ReadUserProperties()
        )
        
        -- return
        animation
    ),
    
    -- Return GfdAnimationController instance
    private fn ReadAnimationController version =
    (
        local controller = GfdAnimationController()
        controller.Type = mReader.ReadU16()
        controller.TargetId = mReader.ReadU32() + 1;
        controller.TargetName = ReadStringWithHash( version )
        controller.TrackCount = mReader.ReadU32()
        
        Format "\t\t\tGfdFileLoader.ReadAnimationController: type: % id: % name: % track count: %\n" \
            controller.Type controller.TargetId controller.TargetName controller.TrackCount
        
        for i = 1 to controller.TrackCount do
        (
            local keyframeTrack = ReadAnimationKeyframeTrack( version )
            controller.Tracks[i] = keyframeTrack
        )
        
        -- Return
        controller
    ),
    
    -- Return GfdAnimationKeyframeTrack instance
    private fn ReadAnimationKeyframeTrack version =
    (
        local track = GfdAnimationKeyframeTrack()
        track.KeyframeType = mReader.ReadU32()
        track.KeyframeCount = mReader.ReadU32()
        
        --Format "\t\t\t\tGfdFileLoader.ReadAnimationKeyframeTrack: type: % count: %\n" \
        --    track.KeyframeType track.KeyframeCount
        
        for i = 1 to track.KeyframeCount do
        (
            track.KeyframeTimings[i] = mReader.ReadF32()
        )
        
        for i = 1 to track.KeyframeCount do
        (
            local keyframe = ReadAnimationKeyframe version track.KeyframeType
            track.Keyframes[i] = keyframe
        )
        
        if ( track.KeyframeType == GfdAnimationKeyframeType_Type26 or 
             track.KeyframeType == GfdAnimationKeyframeType_PRSHalf or 
             track.KeyframeType == GfdAnimationKeyframeType_PRHalf or
             track.KeyframeType == GfdAnimationKeyframeType_PHalf or
             track.KeyframeType == GfdAnimationKeyframeType_RHalf or
             track.KeyframeType == GfdAnimationKeyframeType_SHalf ) then
        (
            track.BasePosition = mReader.ReadVec3F32()
            track.BaseScale = mReader.ReadVec3F32()
        )
        
        -- Return
        track
    ),
    
    -- Returns GfdAnimationKeyframe instance
    private fn ReadAnimationKeyframe version type =
    (
        local keyframe = undefined
        case ( type ) of
        (
            (GfdAnimationKeyframeType_PRSingle):
            (
                keyframe = GfdAnimationKeyframePRSingle()
                keyframe.Position = mReader.ReadVec3F32()
                keyframe.Rotation = mReader.ReadQuatF32()
            )
            
            (GfdAnimationKeyframeType_PRSSingle):
            (
                keyframe = GfdAnimationKeyframePRSSingle()
                keyframe.Position = mReader.ReadVec3F32()
                keyframe.Rotation = mReader.ReadQuatF32()
                keyframe.Scale = mReader.ReadVec3F32()
            )
            
            (GfdAnimationKeyframeType_Type03):
                mReader.SeekCur( 0xC )
            
            (GfdAnimationKeyframeType_Type04):
                mReader.SeekCur( 0x10 )
            
            (GfdAnimationKeyframeType_Morph):
            (
                keyframe = GfdAnimationKeyframeMorph();
                keyframe.ChannelValue = mReader.ReadF32();
            )
            
            (GfdAnimationKeyframeType_Type06):
                mReader.SeekCur( 0x0C )
            
            (GfdAnimationKeyframeType_Type07):
                mReader.SeekCur( 0x0C )
            
            (GfdAnimationKeyframeType_Type08):
                mReader.SeekCur( 0x0C )
            
            (GfdAnimationKeyframeType_Type09):
                mReader.SeekCur( 0x04 )
            
            (GfdAnimationKeyframeType_Type10):
                mReader.SeekCur( 0x10)
            
            (GfdAnimationKeyframeType_Type11):
                mReader.SeekCur( 0x04 )
            
            (GfdAnimationKeyframeType_Type12):
                mReader.SeekCur( 0x04 )
            
            (GfdAnimationKeyframeType_Type13):
                mReader.SeekCur( 0x14 )
            
            (GfdAnimationKeyframeType_Type14):
                mReader.SeekCur( 0x0C )
            
            (GfdAnimationKeyframeType_Type15):
                mReader.SeekCur( 0x04 )
            
            (GfdAnimationKeyframeType_Type16):
                mReader.SeekCur( 0x04 )
            
            (GfdAnimationKeyframeType_Type17):
                mReader.SeekCur( 0x05 )
            
            (GfdAnimationKeyframeType_Type18):
                mReader.SeekCur( 0x0D )
            
            (GfdAnimationKeyframeType_Type19):
                mReader.SeekCur( 0x05 )
            
            (GfdAnimationKeyframeType_Type20):
                mReader.SeekCur( 0x14 )
            
            (GfdAnimationKeyframeType_Type21):
                mReader.SeekCur( 0x14 )
            
            (GfdAnimationKeyframeType_Type22):
                mReader.SeekCur( 0x29 )
            
            (GfdAnimationKeyframeType_Type23):
                mReader.SeekCur( 0x04 )
            
            (GfdAnimationKeyframeType_Type24):
                mReader.SeekCur( 0x04 )
            
            (GfdAnimationKeyframeType_Type25):
                mReader.SeekCur( 0x04 )
            
            (GfdAnimationKeyframeType_Type26):
                mReader.SeekCur( 0x0E )
            
            (GfdAnimationKeyframeType_PRSHalf):
            (
                keyframe = GfdAnimationKeyframePRSHalf()
                keyframe.Position = mReader.ReadVec3F16()
                keyframe.Rotation = mReader.ReadQuatF16()
                keyframe.Scale = mReader.ReadVec3F16()
            )
            
            (GfdAnimationKeyframeType_PRHalf):
            (
                keyframe = GfdAnimationKeyframePRSHalf()
                keyframe.Position = mReader.ReadVec3F16()
                keyframe.Rotation = mReader.ReadQuatF16()
            )
            
            (GfdAnimationKeyframeType_Type29):
                mReader.SeekCur( 0x04 )
            
            (GfdAnimationKeyframeType_Type30):
                mReader.SeekCur( 0x04 )
            
            (GfdAnimationKeyframeType_PHalf): -- Used in P3D/P5D
            (
                keyframe = GfdAnimationKeyframePHalf();
                keyframe.Position = mReader.ReadVec3F16();
            )
            
            (GfdAnimationKeyframeType_RHalf): -- Used in P3D/P5D
            (
                keyframe = GfdAnimationKeyframeRHalf();
                keyframe.Rotation = mReader.ReadQuatF16();
            )
            
            (GfdAnimationKeyframeType_SHalf): -- Used in P3D/P5D
            (
                keyframe = GfdAnimationKeyframeSHalf();
                keyframe.Scale = mReader.ReadVec3F16();
            )
            
            default:
            (
                Format "\t\t\t\tGfdFileLoader.ReadAnimationController: Unknown keyframe type % at %\n" type (HexStr(mReader.GetPosition()))
                throw "Unknown animation key frame type";
            )
        )
        
        -- Return
        keyframe
    ),
        
    -- Texture List 	
    private fn ReadTextureList chunkDesc =
    (
        mModel.Textures = #()
        local field0 = mReader.ReadU32()
        if (field0 != 0) then
        (
            Format "\tGfdFileLoader.ReadTextureList: Unexpected value for field0: %\n" field0
        )
        
        local numTextures = mReader.ReadU32()
        Format "\tGfdFileLoader.ReadTextureList: Loading % textures\n\n" numTextures
        
        for i = 1 to numTextures do
        (
            local texture = GfdTexture()
            texture.Name = mReader.ReadU16PrefixedLengthString()
            texture.PixelFormat = mReader.ReadU16()
            texture.DataLength = mReader.ReadU32()
            Format "\tGfdFileLoader.ReadTextureList: Texture \"%\" fmt % sz % off %\n" texture.Name (HexStr(texture.PixelFormat)) (HexStr(texture.DataLength)) (HexStr(mReader.GetPosition()))
            
            local textureDDSPath = (mFileInfo.FileDirectory + texture.Name)
                
            if (OverwriteExistingTextures or (not (DoesFileExist textureDDSPath))) then
            (
                Format "\tGfdFileLoader.ReadTextureList: Writing to %\n\n" textureDDSPath
                FileUtils.FileStreamBlockCopyToFile (mReader.GetFileStream()) textureDDSPath texture.DataLength
            )
            else
            (
                Format "\tGfdFileLoader.ReadTextureList: % already exists, skipping\n\n" textureDDSPath
                mReader.SeekCur(texture.DataLength)
            )
            
            local footer = mReader.ReadU32()
            if (footer != GfdTextureFooterMagic) then
            (
                throw "\tGfdFileLoader.ReadTextureListChunk: Unexpected end of texture payload data"
            )
            
            append mModel.Textures texture
        )
    ),
    
    private fn ReadFieldTexturePack chunkDesc =
    (
        -- Create texture pack file name
        local texturePackName = replace (mFileInfo.FileName + "_00.bin") 1 1 "tex"
        texturePackName = replace (texturePackName) 12 0 "0"
        
        -- Create texture pack full path
        local texturePackPath = mFileInfo.FileDirectory + "textures\\" + texturePackName
        local texPakFInfo = FileUtils.GetFileInfo texturePackPath
        local texPakReader = FileStreamReader()
        texPakReader.SetFile texturePackPath false
        
        Format "\tGfdFileLoader.ReadFieldTexturePack: Reading field texture pack %\n" texturePackPath
        
        while (true) do
        (
            texPakReader.SetBigEndian(false)
            local pakEntryName = texPakReader.ReadFixedLengthString 252
            local pakEntryDataSize = texPakReader.ReadU32()
            local pakEntryDataEnd = texPakReader.GetPosition() + pakEntryDataSize
            pakEntryDataEnd = bit.and (pakEntryDataEnd + 63) (bit.not 63) -- make sure to align the offset
            
            Format "\n\tGfdFileLoader.ReadFieldTexturePack: Pack entry: % size:%\n" pakEntryName (HexStr(pakEntryDataSize))
            
            if (pakEntryDataSize == 0) then
            (
                Format "\tGfdFileLoader.ReadFieldTexturePack: End of file marker read\n"
                exit
            )
            
            if ((GetFilenameType pakEntryName) != ".dds") then
            (
                Format "\tGfdFileLoader.ReadFieldTexturePack: Not a dds file, skipping.\n"
            )
            else
            (
                local ddsPath = (texPakFInfo.FileDirectory + pakEntryName)
                if (OverwriteExistingTextures or (not (DoesFileExist ddsPath))) then
                (
                    ReadFieldTexture ddsPath texPakReader
                )
                else
                (
                    Format "\tGfdFileLoader.ReadFieldTexturePack: % already exists, skipping\n\n" ddsPath
                )
            )
            
            texPakReader.SeekSet(pakEntryDataEnd)
        )
    ),
    
    private fn ReadFieldTexture ddsOutPath texPakReader =
    (
        -- Read field texture header
        texPakReader.SetBigEndian(true)
        local headerStart = texPakReader.GetPosition()
        local magic = texPakReader.ReadU32()
        local dataSize = texPakReader.ReadU32()
        local field3 = texPakReader.ReadU32()
        local field4 = texPakReader.ReadU32()
        local dataStart = texPakReader.ReadU32()
        local dataSize2 = texPakReader.ReadU32()
        local field7 = texPakReader.ReadU32()
        local field8 = texPakReader.ReadU32()
        local width = texPakReader.ReadU16()
        local height = texPakReader.ReadU16()
        local field11 = texPakReader.ReadU16()
        
        texPakReader.SeekSet(headerStart + dataStart)
        
        Format "\tGfdFileLoader.ReadFieldTexture: Converting texture sz: % f3: % f4: % sz2: % f7: % f8: % w: % h: % f11: %\n\n" \
                    (HexStr(dataSize)) (HexStr(field3)) (HexStr(field4)) (HexStr(dataSize2)) (HexStr(field7)) \
                    (HexStr(field8)) width height (HexStr(field11))
            
        -- Create dds header						
        local ddsHdr = DDSHeader()
        ddsHdr.Width = width
        ddsHdr.Height = height
        ddsHdr.MipMapCount = (bit.shift (bit.and field7 0xFF0000) -16)
                        
        if ((bit.and field7 0x1000000) == 0x1000000) then
        (
            ddsHdr.PixelFormatFourCC = DDSPixelFormatFourCCDXT3
            bit.or ddsHdr.PixelFormatFlags DDSPixelFormatFlagAlphaPixels
        )
        else if ((bit.and field7 0x8000000) == 0x8000000) then -- Could also be a cubemap
        (
            ddsHdr.PixelFormatFourCC = DDSPixelFormatFourCCDXT5
            bit.or ddsHdr.PixelFormatFlags DDSPixelFormatFlagAlphaPixels
        )
        
        -- Write dds data
        local ddsStream = FileUtils.CreateWriteableFileStream(ddsOutPath)
        ddsHdr.WriteToStream(ddsStream)
        FileUtils.FileStreamBlockCopy (texPakReader.GetFileStream()) ddsStream dataSize
    ),
    
    -- Material List		
    private fn ReadMaterialList chunkDesc = 
    (
        mModel.Materials = #()
        local field0 = mReader.ReadU32()
        if (field0 != 0) then
        (
            Format "\tGfdFileLoader.ReadMaterialList: Unexpected value for field0: %\n" field0
        )
    
        local numMaterials = mReader.ReadU32()
        Format "\tGfdFileLoader.ReadMaterialList: Loading % materials\n" numMaterials
        
        for i = 1 to numMaterials do 
        (
            material = GfdMaterial()
            material.Name = mReader.ReadU16PrefixedLengthString()
            material.Hash = mReader.ReadU32()
            material.Flags = mReader.ReadU32()
            if ( chunkDesc.Version < 0x1104000 ) then
            (
                material.Flags = bit.and material.Flags 0x7FFFFFFF
            )
            
            material.Ambient = mReader.ReadVec4F32()
            material.Diffuse = mReader.ReadVec4F32()
            material.Specular = mReader.ReadVec4F32()
            material.Emissive = mReader.ReadVec4F32()
            material.Field40 = mReader.ReadF32()
            material.Field44 = mReader.ReadF32()
            
            if ( chunkDesc.Version < 0x1103040 ) then
            (
                material.Field48 = mReader.ReadU16()
                material.Field49 = mReader.ReadU16()
                material.Field4A = mReader.ReadU16()
                material.Field4B = mReader.ReadU16()
                material.Field4C = mReader.ReadU16()
                
                if ( chunkDesc.Version > 0x108011b ) then
                (
                    material.Field4D = mReader.ReadU16()
                )
            )
            else
            (
                material.Field48 = mReader.ReadU8()
                material.Field49 = mReader.ReadU8()
                material.Field4A = mReader.ReadU8()
                material.Field4B = mReader.ReadU8()
                material.Field4C = mReader.ReadU8()
                material.Field4D = mReader.ReadU8()
            )
            
            material.Field90 = mReader.ReadU16()
            material.Field92 = mReader.ReadU16()
            
            if ( chunkDesc.Version <= 0x1104800 ) then
            (
                material.Field94 = 1
                material.Field96 = mReader.ReadS32()
            )
            else
            (
                material.Field94 = mReader.ReadS16()
                material.Field96 = mReader.ReadS16()
            )
            
            material.Field5C = mReader.ReadS16()
            material.Field6C = mReader.ReadU32()
            material.Field70 = mReader.ReadU32()
            material.Field50 = mReader.ReadS16()
            
            if ( chunkDesc.Version <= 0x1105070 or chunkDesc.Version >= 0x1105090 ) then
            (
                material.Field98 = mReader.ReadU32()
            )
            
            Format "\n\tGfdFileLoader.ReadMaterialList: Material % hash: % flgs: % off %\n" material.Name (HexStr(material.Hash)) (HexStr(material.Flags)) (HexStr(mReader.GetPosition()))

            for j = 1 to GfdMaterialTextureSlotCount do
            (
                local textureSlot = undefined
                local bitMask = bit.shift GfdMaterialFlags_UseDiffuseTextureSlot (j - 1)
                if ((bit.and material.Flags bitMask) == bitMask) then
                (
                    textureSlot = GfdMaterialTextureSlot()
                    textureSlot.TextureName = mReader.ReadU16PrefixedLengthString()
                    textureSlot.Hash = mReader.ReadU32()
                    mReader.SeekCur(0x48)
                    
                    Format "\t\tGfdFileLoader.ReadMaterialList: % \"%\"\n" (GfdMaterialTextureSlotNames[j]) textureSlot.TextureName
                )
                
                material.TextureSlots[j] = textureSlot
            )
            
            if ( ( bit.and material.Flags GfdMaterialFlags_HasAttributes ) == GfdMaterialFlags_HasAttributes ) then
            (
                local attributeCount = mReader.ReadU32()
                
                Format "\t\tGfdFileLoader.ReadMaterialList: Reading % attributes\n" attributeCount
                
                for j = 1 to attributeCount do
                (
                    local attributeFlags = mReader.ReadU32()
                    local attributeId = bit.and attributeFlags 0xFFFF
                    Format "\t\tGfdFileLoader.ReadMaterialList: Reading attribute % at %\n" (HexStr(attributeFlags)) (HexStr(mReader.GetPosition()))
                    
                    case ( attributeId ) of
                    (
                        0:
                        (
                            if ( chunkDesc.Version > 0x1104500 ) then
                            (
                                mReader.SeekCur( 40 )
                            )
                            else if ( chunkDesc.Version > 0x1104220 ) then
                            (
                                mReader.SeekCur( 39 )
                                if ( chunkDesc.Version > 0x1104260 ) then
                                (
                                    mReader.SeekCur( 1 )
                                )
                            )
                            else
                            (
                                mReader.SeekCur( 32 )
                            )
                        )
                        
                        1:
                        (
                            mReader.SeekCur( 48 )
                            
                            if ( chunkDesc.Version <= 0x1104500 ) then
                            (
                                mReader.SeekCur( 1 )
                                
                                if ( chunkDesc.Version > 0x1104180 ) then
                                (
                                    mReader.SeekCur( 1 )
                                )
                                
                                if ( chunkDesc.Version > 0x1104210 ) then
                                (
                                    mReader.SeekCur( 1 )
                                )
                                
                                if ( chunkDesc.Version > 0x1104400 ) then
                                (
                                    mReader.SeekCur( 1 )
                                )
                            )
                            else
                            (
                                mReader.SeekCur( 4 )
                            )
                        )
                        
                        2:
                        (
                            mReader.SeekCur( 8 )
                        )
                        
                        3:
                        (
                            mReader.SeekCur( 52 )
                        )
                        
                        4:
                        (
                            mReader.SeekCur( 81 )
                        )
                        
                        5:
                        (
                            mReader.SeekCur( 68 )
                        )
                        
                        6:
                        (
                            mReader.SeekCur( 12 )
                        )
                        
                        7:
                        (
                        )
                    )
                )
            )
            
            append mModel.Materials material
        )
    ),
    
    -- Scene
    private fn ReadScene chunkDesc =
    (
        mModel.Scene = GfdScene()
        mModel.Scene.Field0 = mReader.ReadU32()
        mModel.Scene.Flags = mReader.ReadU32()
        
        Format "\tGfdFileLoader.ReadScene: Scene field0: % flags: %\n" mModel.Scene.Field0 mModel.Scene.Flags
        
        if ((bit.and mModel.Scene.Flags GfdSceneFlags_Skinned) == GfdSceneFlags_Skinned) then
        (
            Format "\tGfdFileLoader.ReadScene: Skinning is used\n"
            
            mModel.Scene.UsedBoneCount = mReader.ReadU32()
            
            Format "\tGfdFileLoader.ReadScene: % used bones\n" mModel.Scene.UsedBoneCount
            
            mModel.Scene.InvBindMatrices = mReader.ReadMtx44Array mModel.Scene.UsedBoneCount
            mModel.Scene.UsedBoneMap 	= mReader.ReadU16Array   mModel.Scene.UsedBoneCount
        )
        
        if ((bit.and mModel.Scene.Flags GfdSceneFlags_HasBBox) == GfdSceneFlags_HasBBox) then
        (
            Format "\tGfdFileLoader.ReadScene: Bbox data present. Skipping.\n"
            mReader.SeekCur(24) -- Bbox data
        )
        
        if ((bit.and mModel.Scene.Flags GfdSceneFlags_HasBSphere) == GfdSceneFlags_HasBSphere) then
        (
            Format "\tGfdFileLoader.ReadScene: BSphere data present. Skipping.\n"
            mReader.SeekCur(16) -- Bbox data
        )
        
        -- Create root node
        local gfdRootNode = ReadNode( chunkDesc.Version )
            
        if ( gfdRootNode.ChildCount > 0 ) then
        (
            local parentStack = StackContainer()
            parentStack.PushItem(gfdRootNode)
                
            local remChildCountStack = StackContainer()
            remChildCountStack.PushItem(gfdRootNode.ChildCount)
            
            while (not remChildCountStack.IsEmpty()) do
            (
                local node = ReadNode( chunkDesc.Version )
                node.Parent = parentStack.PeekItem()
                
                -- Check remaining children to read of parent
                local remChildCount = remChildCountStack.PopItem()
                remChildCount -= 1
                
                if (remChildCount == 0) then
                (
                    parentStack.PopItem()
                )
                else
                (
                    remChildCountStack.PushItem(remChildCount)
                )

                /* Add to parent's children list */
                append node.Parent.Children node
                
                if (node.ChildCount > 0) then
                (
                    parentStack.PushItem(node)
                    remChildCountStack.PushItem(node.ChildCount)
                )
            )
        )
        
        local curNodeStack = StackContainer()
        curNodeStack.PushItem(gfdRootNode)
        
        while (not curNodeStack.IsEmpty()) do
        (
            local curNode = curNodeStack.PopItem()			
            append mModel.Scene.Nodes curNode
            
            for i = 1 to curNode.ChildCount do
            (
                curNodeStack.PushItem (curNode.Children[i])
            )
        )
    ),
            
    -- Returns GfdNode instance
    private fn ReadNode version =
    (
        Format "\n\t\tGfdFileLoader.ReadNode: Reading node at %\n" (HexStr(mReader.GetPosition()))
        node = GfdNode()
        node.Name = mReader.ReadU16PrefixedLengthString()
        node.Hash = mReader.ReadU32()
        node.Position = mReader.ReadVec3F32()
        node.Rotation = mReader.ReadQuatF32()
        node.Scale = mReader.ReadVec3F32()
            
        if ( version <= 0x1090000 ) then
        (
            mReader.SeekCur( 1 )
        )
            
        node.AttachmentCount = mReader.ReadU32()

        Format "\t\tGfdFileLoader.ReadNode: Node \"%\" hash: % attachmentCount: % \n" node.Name (HexStr(node.Hash)) node.AttachmentCount	
        local skipProperties = false
        for i = 1 to node.AttachmentCount do
        (
            local attachment = GfdNodeAttachment()
            attachment.Type = mReader.ReadU32()
            
            Format "\n\t\tGfdFileLoader.ReadNode: Attachment #% Type: % \n" i attachment.Type
            case (attachment.Type) of
            (
                4: -- Mesh
                (
                    attachment.Object = ReadGeometry( version )
                )
                    
                5: -- Camera
                (
                    attachment.Object = ReadCamera( version )
                )
                    
                6: -- Light
                (
                    attachment.Object = ReadLight( version )
                )
                
                7: -- Epl particle
                (
                    local offset = mReader.GetPosition()
                    print (HexStr(offset))
                        
                    if ((offset == 0x1a67d5) OR (offset == 0x1b09ef)) then
                    (
                        mReader.SeekCur(0x27C9)
                    )
                    else if ((offset == 0x1a907c) OR (offset == 0x1b3296)) then
                    (
                        mReader.SeekCur(0x3568)
                    )
                    else if ((offset == 0x1ac6c2) OR (offset == 0x1b68dc)) then
                    (
                        mReader.SeekCur(0x3571)
                    )
                    else
                    (
                        local testCount = 0
                        local foundProperty = false
                        while ( testCount < 1000000 ) do 
                        (
                            --Format "\n\t\tGfdFileLoader.ReadNode: Test %\n" (HexStr(mReader.GetPosition()))
                            testCount = testCount + 1
                            local test = mReader.ReadU32()
                                                    
                            if (test == 0x42697030 or -- Bip01
                                test == 0x726F6F74 or -- root
                                test == 0x726F74E0 or -- rot
                                test == 0x62206C20 or -- b l 
                                test == 0x62206C5F or -- b l_
                                test == 0x62207220 or -- b r 
                                test == 0x6220725F or -- b r_
                                test == 0x68656164 or -- head
                                test == 0x685F415F or -- h_A_
                                test == 0x685F425F or -- h_B_
                                test == 0x685F435F or -- h_C_
                                test == 0x625F6D61 or -- b_man
                                test == 0x625F6865 or -- b_he
                                test == 0x62206465 or -- b de
                                test == 0x626F6479 or -- body
                                test == 0x62207461 or -- b ta
                                test == 0x6F626A5F or -- obj_
                                test == 0x6D616E64 or -- mand
                                test == 0x625F6E6F or -- b_no
                                test == 0x68206C20 or -- h l 
                                test == 0x68207220 or -- h r 
                                test == 0x68206C5F or -- h l_
                                test == 0x6820725F or -- h r_
                                test == 0x685F6C5F or -- h_l_
                                test == 0x685F725F or -- h_r_
                                test == 0x73686164 )  -- shad
                                then
                            (
                                exit
                            )
                            else if ( test == 0x67666448 ) then -- gfdH
                            (
                                foundProperty = true
                                mReader.SeekCur( -1 )
                                exit
                            )
                            else
                            (
                                mReader.SeekCur( -3 )
                            )
                        )
                        
                        --Format "\n\t\tGfdFileLoader.ReadNode: Test %\n" (HexStr(mReader.GetPosition()))
                        
                        mReader.SeekCur( -0xE )
                        
                        if ( not foundProperty ) then
                            skipProperties = true
                        
                        if ( testCount == 1000000 ) then 
                        (
                            throw "\t\tGfdFileLoader.ReadNode: Can't handle this attachment type\n"
                        )
                        
                        exit
                    )
                    --ReadParticle()
                )
                
                9: -- Morpher
                (
                    local morphTargetCount = mReader.ReadU32()
                    mReader.SeekCur( 4 * morphTargetCount ) -- ints
                    local morphMaterialName = mReader.ReadU16PrefixedLengthString()
                    local morphMaterialNameHash = mReader.ReadU32()
                )

                default:
                (
                    throw "\t\tGfdFileLoader.ReadNode: Unknown attachment type\n"
                )
            )
            
            node.Attachments[i] = attachment
        )
        
        if ( version > 0x1060000 and not skipProperties ) then
        (
            /* Read user properties */
            local hasProps = mReader.ReadU8() == 1
            if ( hasProps ) then
            (
                node.UserProperties = ReadUserProperties()
            )
        )
        
        if ( version > 0x1104230 ) then
        (
            local lastFloatVal = (mReader.ReadF32())
            if (lastFloatVal != 1.0f) then
                Format "\t\tGfdFileLoader.ReadNode: Last float value isn't 1.0f: %\n" lastFloatVal
        )
        
        node.ChildCount = mReader.ReadU32()

        -- Return
        node
    ),
    
    -- Returns GfdGeometry instance
    private fn ReadGeometry version = 
    (
        geometry = GfdGeometry()
        geometry.Flags = mReader.ReadU32()
        geometry.VertexFormat = mReader.ReadU32()
        
        if ( (bit.and geometry.Flags GfdGeometryFlags_HasTriangles) == GfdGeometryFlags_HasTriangles ) then
        (
            geometry.FaceCount = mReader.ReadU32()
            geometry.IndexType = mReader.ReadU16()
        )
        
        geometry.VertexCount = mReader.ReadU32()
        
        if ( version > 0x1103020 ) then
        (
            geometry.Field7 = mReader.ReadU32()
        )
        
        geometry.VertexTexCoordChannels = #(#(), #(), #(), #(), #(), #(), #(), #())

        Format "\n\t\t\tGfdFileLoader.ReadGeometry: F % FVF % FC % FT % VC % VT % [FVF Flags] NRM % TAN % CLR % TEX1 % TEX2 % TEX3 % TEX4 % TEX5 % TEX6 % TEX7 % TEX8 % WGHT %\n" \
            (HexStr(geometry.Flags)) (HexStr(geometry.VertexFormat)) geometry.FaceCount geometry.IndexType geometry.VertexCount geometry.Field7 \
            ((bit.and geometry.VertexFormat GfdGeometryVertexFlags_Normal) == GfdGeometryVertexFlags_Normal) \
            ((bit.and geometry.VertexFormat GfdGeometryVertexFlags_Tangent) == GfdGeometryVertexFlags_Tangent) \
            ((bit.and geometry.VertexFormat GfdGeometryVertexFlags_Color) == GfdGeometryVertexFlags_Color) \
            ((bit.and geometry.VertexFormat GfdGeometryVertexFlags_Tex1) == GfdGeometryVertexFlags_Tex1) \
            ((bit.and geometry.VertexFormat GfdGeometryVertexFlags_Tex2) == GfdGeometryVertexFlags_Tex2) \
            ((bit.and geometry.VertexFormat GfdGeometryVertexFlags_Tex3) == GfdGeometryVertexFlags_Tex3) \
            ((bit.and geometry.VertexFormat GfdGeometryVertexFlags_Tex4) == GfdGeometryVertexFlags_Tex4) \
            ((bit.and geometry.VertexFormat GfdGeometryVertexFlags_Tex5) == GfdGeometryVertexFlags_Tex5) \
            ((bit.and geometry.VertexFormat GfdGeometryVertexFlags_Tex6) == GfdGeometryVertexFlags_Tex6) \
            ((bit.and geometry.VertexFormat GfdGeometryVertexFlags_Tex7) == GfdGeometryVertexFlags_Tex7) \
            ((bit.and geometry.VertexFormat GfdGeometryVertexFlags_Tex8) == GfdGeometryVertexFlags_Tex8) \
            ((bit.and geometry.Flags 1) == 1) 
            
        Format "\t\t\tGfdFileLoader.ReadGeometry: Vertex start %\n" (HexStr(mReader.GetPosition()))			
        for i = 1 to geometry.VertexCount do
        (
            if ((bit.and geometry.VertexFormat GfdGeometryVertexFlags_Position) == GfdGeometryVertexFlags_Position) then
                geometry.VertexPositions[i] = mReader.ReadVec3F32()
            
            if ((bit.and geometry.VertexFormat GfdGeometryVertexFlags_Normal) == GfdGeometryVertexFlags_Normal) then
                geometry.VertexNormals[i] = mReader.ReadVec3F32()
            
            if ((bit.and geometry.VertexFormat GfdGeometryVertexFlags_Tangent) == GfdGeometryVertexFlags_Tangent) then
                geometry.VertexTangents[i] = mReader.ReadVec3F32()
            
            if ((bit.and geometry.VertexFormat GfdGeometryVertexFlags_Binormal) == GfdGeometryVertexFlags_Binormal) then
                mReader.SeekCur( 12 )
            
            if ((bit.and geometry.VertexFormat GfdGeometryVertexFlags_Color) == GfdGeometryVertexFlags_Color) then
                geometry.VertexColors[i] = mReader.ReadU32()
            
            for j = 1 to 8 do
            (
                local bitMask = bit.shift GfdGeometryVertexFlags_Tex1 (j - 1)
                if ((bit.and geometry.VertexFormat bitMask) == bitMask) then
                (
                    local texCoord = mReader.ReadVec2F32()
                    geometry.VertexTexCoordChannels[j][i] = [texCoord.X, (texCoord.Y * -1) + 1, 0]
                )
            )
            
            if ((bit.and geometry.VertexFormat GfdGeometryVertexFlags_Color2) == GfdGeometryVertexFlags_Color2) then
                mReader.SeekCur( 4 )

            if ((bit.and geometry.Flags GfdGeometryFlags_HasVertexWeights) == GfdGeometryFlags_HasVertexWeights) then
            (
                /*
                local weightTotal = 0.0f
                local numUsed = 0
                */
                geometry.VertexWeights[i] = #()
                for j = 1 to 4 do
                (
                    local weight = mReader.ReadF32()
                    geometry.VertexWeights[i][j] = weight
                    
                    /*
                    weightTotal += weight
                    
                    if (weight > 0.0f) then
                        numused += 1
                    */
                )
                
                /*
                if (weightTotal < 0.998) then
                (
                    geometry.VertexWeights[i][numUsed] += (1.0f - weightTotal)
                )
                */
                
                geometry.VertexWeightIndices[i] = #()
                for j = 1 to 4 do
                    geometry.VertexWeightIndices[i][4 - (j - 1)] = (mReader.ReadU8()) + 1
            )
        )
        
        if ( (bit.and geometry.Flags GfdGeometryFlags_HasMorphTargets) == GfdGeometryFlags_HasMorphTargets ) then
        (
            geometry.MorphTargetList = ReadMorphTargetList( version );
        )
        
        if ( (bit.and geometry.Flags GfdGeometryFlags_HasTriangles) == GfdGeometryFlags_HasTriangles ) then
        (
            Format "\t\t\tGfdFileLoader.ReadGeometry: Faces start %\n" (HexStr(mReader.GetPosition()))
                
            if ( geometry.IndexType == GfdGeometryIndexType_UInt16 ) then
            (
                geometry.FaceIndices = mReader.ReadVec3U16Array geometry.FaceCount
            )
            else if ( geometry.IndexType == GfdGeometryIndexType_UInt32 ) then
            (
                geometry.FaceIndices = mReader.ReadVec3U32Array geometry.FaceCount
            )
        
            for i = 1 to geometry.FaceCount do
                geometry.FaceIndices[i] = geometry.FaceIndices[i] + 1
        )
        
        if ( (bit.and geometry.Flags GfdGeometryFlags_HasMaterial) == GfdGeometryFlags_HasMaterial ) then
        (
            geometry.MaterialName = mReader.ReadU16PrefixedLengthString()
            geometry.MaterialHash = mReader.ReadU32()
        )
        
        if ( (bit.and geometry.Flags GfdGeometryFlags_HasBoundingBox) == GfdGeometryFlags_HasBoundingBox ) then
        (
            mReader.SeekCur( 24 )
        )
        
        if ( (bit.and geometry.Flags GfdGeometryFlags_HasBoundingSphere) == GfdGeometryFlags_HasBoundingSphere ) then
        (
            mReader.SeekCur( 16 )
        )
        
        if ( (bit.and geometry.Flags GfdGeometryFlags_1000) == GfdGeometryFlags_1000 ) then
        (
            mReader.SeekCur( 8 ) -- 2 floats
        )
        
        Format "\t\t\tGfdFileLoader.ReadGeometry: Used material: \"%\" hash: %\n" geometry.MaterialName (HexStr(geometry.MaterialHash))
        Format "\t\t\tGfdFileLoader.ReadGeometry: Geometry end at %\n" (HexStr(mReader.GetPosition()))
            
        -- Return
        geometry
    ),
    
    private fn ReadMorphTargetList version =
    (
        local list = GfdMorphTargetList();
        list.Flags = mReader.ReadU32();
        local count = mReader.ReadU32();
        
        for i = 1 to count do
        (
            local target = GfdMorphTarget();
            target.Flags = mReader.ReadU32();
            local vertexCount = mReader.ReadU32();
            target.VertexPositionOffsets = mReader.ReadVec3F32Array( vertexCount );
            list.Targets[i] = target;
        )
        
        -- Return
        list;
    ),
    
    -- Returns GfdCamera instance
    private fn ReadCamera version =
    (
        camera = GfdCamera()
        camera.Transform = mReader.ReadMtx44()
        camera.Field180 = mReader.ReadF32()
        camera.Field184 = mReader.ReadF32()
        camera.Field188 = mReader.ReadF32()
        camera.Field18C = mReader.ReadF32()
        
        if ( version > 0x1104060 ) then
        (
            camera.Field190 = mReader.ReadF32()
        )
        
        -- Return
        camera
    ),
    
    -- Returns GfdLight instance
    private fn ReadLight version =
    (
        light = GfdLight()
        
        light.Flags = 0
        if ( version > 0x1104190 ) then
        (
            light.Flags = mReader.ReadU32()
        )
        
        light.Type = mReader.ReadU32()
        light.Field30 = mReader.ReadVec4F32()
        light.Field40 = mReader.ReadVec4F32()
        light.Field50 = mReader.ReadVec4F32()
        
        case ( light.Type ) of 
        (
            1:
            (
                light.Field20 = mReader.ReadF32()
                light.Field04 = mReader.ReadF32()
                light.Field08 = mReader.ReadF32()
            )
            
            2:
            (
                light.Field10 = mReader.ReadF32()
                light.Field04 = mReader.ReadF32()
                light.Field08 = mReader.ReadF32()
                
                if ( ( bit.and light.Flags 2 ) == 2 ) then
                (
                    light.Field6C = mReader.ReadF32()
                    light.Field70 = mReader.ReadF32()
                )
                else
                (
                    light.Field60 = mReader.ReadF32()
                    light.Field64 = mReader.ReadF32()
                    light.Field68 = mReader.ReadF32()
                )
            )
            
            3:
            (
                light.Field20 = mReader.ReadF32()
                light.Field08 = mReader.ReadF32()
                light.Field04 = mReader.ReadF32()
                light.Field74 = mReader.ReadF32()
                light.Field78 = mReader.ReadF32()
                
                light.Field10 = mReader.ReadF32()
                light.Field04 = mReader.ReadF32()
                light.Field08 = mReader.ReadF32()
                
                if ( ( bit.and light.Flags 2 ) == 2 ) then
                (
                    light.Field6C = mReader.ReadF32()
                    light.Field70 = mReader.ReadF32()
                )
                else
                (
                    light.Field60 = mReader.ReadF32()
                    light.Field64 = mReader.ReadF32()
                    light.Field68 = mReader.ReadF32()
                )
            )
        )
        
        -- Return
        light
    ),
    
    private fn ReadEpl =
    (
        throw "GfdFileLoader.ReadEpl: Not implemented!"
        
        local numEffect = mReader.ReadU32()
        mReader.SeekCur(0x2F)
        
        for i = 1 to numEffect do
        (
            mReader.SeekCur(0x3E)
            local effectName = mReader.ReadU16PrefixedLengthString()
            print effectName
            mReader.SeekCur(0x14C)
            local textureName = mReader.ReadU16PrefixedLengthString()
            print textureName
            local textureHash = mReader.ReadU32()
            local unk1 = mReader.ReadU32()
            local unk2 = mReader.ReadU32()
            local textureDataSize = mReader.ReadU32()
            FileCopyBlockToFile m_Stream (mFileInfo.FileDirectory + textureName) textureDataSize
            
            Print (HexStr(mReader.GetPosition()))
            
            mReader.SeekCur(1)
        )
        
        -- 0xCC = 5 particles
        mReader.SeekCur(0xCC)
    ),
    
    -- Returns GfdUserProperty list
    private fn ReadUserProperties = 
    (
        local userProperties = #()
        local numProps = mReader.ReadU32()
        for i = 1 to numProps do
        (
            local prop = GfdUserProperty()
            prop.Type = mReader.ReadU32()
            prop.Key = mReader.ReadU16PrefixedLengthString()
            prop.Hash = mReader.ReadU32()
            prop.Size = mReader.ReadU32()
            
            case (prop.Type) of
            (
                GfdUserPropertyType_Int:
                    prop.Value = mReader.ReadU32()
                
                GfdUserPropertyType_Float:
                    prop.Value = mReader.ReadF32()
                
                GfdUserPropertyType_Bool:
                (
                    prop.Value = mReader.ReadU8()
                    if (prop.Value != 0 and prop.Value != 1) then
                    (
                        Format "\t\tGfdFileLoader.ReadUserProperties: GfdUserPropertyType_Bool value outside of boolean range. Value %\n" prop.Value
                    )
                    else
                    (
                        prop.Value = (prop.Value == 1)
                    )
                )
                
                GfdUserPropertyType_String:
                    prop.Value = mReader.ReadFixedLengthString(prop.Size - 1)
                
                GfdUserPropertyType_ByteVector3:
                    prop.Value = mReader.ReadVec3U8()
                
                GfdUserPropertyType_ByteVector4:
                    prop.Value = mReader.ReadVec4U8()
                
                GfdUserPropertyType_Vector3:
                    prop.Value = mReader.ReadVec3F32()
                
                GfdUserPropertyType_Vector4:
                    prop.Value = mReader.ReadVec4F32()
                
                GfdUserPropertyType_ByteArray:
                    prop.Value = mReader.ReadU8Array prop.Size
                
                default:
                (
                    Format "\t\tGfdFileLoader.ReadUserProperties: Unknown user property type %\n" prop.Type
                    mReader.SeekCur attrib.Size
                )
            )
            
            append userProperties prop
            
            Format "\t\tGfdFileLoader.ReadUserProperties: User property %  type: % hash: % size: % value: %\n" prop.Key prop.Type (HexStr(prop.Hash)) prop.Size prop.Value
        )
        
        -- Return
        userProperties
    ),
    
    -- Setup
    public fn SetupAnimation anim isBlend =
    (
        if ( anim == undefined or anim.Duration == 0f ) then
        (
            return false
        )
        
        local fps = 30
        local numAnimFrames = (anim.Duration * fps) as integer
        animationRange = interval 0 numAnimFrames
        Format "GfdFileLoader.SetupAnimation: Setting up animation with % frames\n" numAnimFrames
            
        with animate on
        (
            for controller in anim.Controllers do
            (
                if ((controller.Tracks.Count == 0)) then
                    continue 
                
                local curBoneNode = GetNodeByName controller.TargetName exact: true
                if (curBoneNode == undefined) then 
                    continue
                
                for track in controller.Tracks do
                (
                    Format "GfdFileLoader.SetupAnimation: Setting up keyframes for % keyFrameCount: %\n" controller.TargetName track.KeyframeCount
                    
                    if ( isBlend ) then
                    (
                        -- Save current transforms so we can blend them with the blend transforms
                        local savedTransforms = #()
                        for keyIdx = 1 to track.KeyframeCount do
                        (
                            local curTime = track.KeyframeTimings[keyIdx] * fps
                            
                            at time curTime
                            (
                                savedTransform = #( curBoneNode.Controller.Position, curBoneNode.Controller.Rotation, curBoneNode.Controller.Scale )
                                append savedTransforms savedTransform
                            )
                        )
                    )
                    
                    for keyIdx = 1 to track.KeyframeCount do
                    (
                        local curTime = track.KeyframeTimings[keyIdx] * fps
                        local curKey = track.Keyframes[keyIdx]
                        
                        if (curKey == undefined) then 
                            continue
                        
                        at time curTime
                        (
                            case ( track.KeyframeType ) of
                            (
                                (GfdAnimationKeyframeType_PRSingle):
                                (
                                    if ( not isBlend ) then
                                    (
                                        curBoneNode.Controller.Rotation = inverse curKey.Rotation
                                        curBoneNode.Controller.Position = curKey.Position
                                    )
                                    else
                                    (
                                        curBoneNode.Controller.Rotation = savedTransforms[keyIdx][2] * inverse curKey.Rotation
                                        curBoneNode.Controller.Position = savedTransforms[keyIdx][1] + curKey.Position
                                    )
                                )
                                
                                (GfdAnimationKeyframeType_PRSSingle):
                                (
                                    if ( not isBlend ) then
                                    (
                                        curBoneNode.Controller.Rotation = inverse curKey.Rotation
                                        curBoneNode.Controller.Position = curKey.Position
                                        curBoneNode.Controller.Scale = curKey.Scale
                                    )
                                    else
                                    (
                                        curBoneNode.Controller.Rotation = savedTransforms[keyIdx][2] * inverse curKey.Rotation
                                        curBoneNode.Controller.Position = savedTransforms[keyIdx][1] + curKey.Position
                                        curBoneNode.Controller.Scale = savedTransforms[keyIdx][3] + curKey.Scale
                                    )
                                )
                                
                                (GfdAnimationKeyframeType_Morph):
                                (
                                    -- Find mesh
                                    local targetId = 1;
                                    local meshIndex = 1;
                                    local meshNode = undefined;
                                    local morphTargetIndex = 1;
                                    while ( true ) do
                                    (                                        
                                        -- Search for base mesh
                                        meshNode = GetNodeByName (controller.TargetName + "_Mesh" + meshIndex as string) exact: true;
                                        if ( meshNode == undefined ) then
                                        (
                                            targetId = -1;
                                            exit;
                                        )
                                        
                                        if ( targetId == controller.TargetId ) then
                                        (
                                            exit;
                                        )
                                        
                                        targetId += 1;
                                        morphTargetIndex = 1;
                                        while ( true ) do
                                        (
                                            -- Search for morph target
                                            local morphTargetNode = GetNodeByName (controller.TargetName + "_Mesh" + meshIndex as string + "_MorphTarget" + morphTargetIndex as string) exact: true;
                                            if ( morphTargetNode == undefined ) then
                                            (
                                                morphTargetIndex = -1;
                                                exit;
                                            )
                                            
                                            if ( targetId == controller.TargetId ) then
                                            (
                                                exit;
                                            )

                                            morphTargetIndex += 1;
                                            targetId += 1;
                                        )
                                        
                                        if ( targetId == controller.TargetId ) then
                                            exit;
                                        
                                        morphTargetIndex = -1;
                                        meshIndex += 1;
                                    )
                                    
                                    if ( targetId  == controller.TargetId ) then
                                    (
                                        local meshNodeName = controller.TargetName + "_Mesh" + meshIndex as string;
                                        local meshNode = GetNodeByName meshNodeName exact: true
                                        if ( meshNode != undefined and morphTargetIndex != -1 ) then
                                        (
                                            --Format "%_MorphTarget%\n" meshNodeName morphTargetIndex;
                                            WM3_MC_SetValue meshNode.Morpher morphTargetIndex (curKey.ChannelValue * 10);
                                        )
                                    )
                                )
                                
                                (GfdAnimationKeyframeType_PRSHalf):
                                (
                                    if ( not isBlend ) then
                                    (
                                        curBoneNode.Controller.Rotation = inverse curKey.Rotation
                                        curBoneNode.Controller.Position = track.BasePosition * curKey.Position
                                        curBoneNode.Controller.Scale = track.BaseScale * curKey.Scale
                                    )
                                    else
                                    (
                                        curBoneNode.Controller.Rotation = savedTransforms[keyIdx][2] * inverse curKey.Rotation
                                        curBoneNode.Controller.Position = savedTransforms[keyIdx][1] + ( track.BasePosition * curKey.Position )
                                        curBoneNode.Controller.Scale = savedTransforms[keyIdx][3] + ( track.BaseScale * curKey.Scale )
                                    )
                                )
                                
                                (GfdAnimationKeyframeType_PRHalf):
                                (
                                    if ( not isBlend ) then
                                    (
                                        curBoneNode.Controller.Rotation = inverse curKey.Rotation
                                        curBoneNode.Controller.Position = track.BasePosition * curKey.Position
                                    )
                                    else
                                    (
                                        curBoneNode.Controller.Rotation = savedTransforms[keyIdx][2] * inverse curKey.Rotation
                                        curBoneNode.Controller.Position = savedTransforms[keyIdx][1] + ( track.BasePosition * curKey.Position )
                                    )
                                )
                                
                                (GfdAnimationKeyframeType_PHalf):
                                (
                                    if ( not isBlend ) then
                                    (
                                        curBoneNode.Controller.Position = track.BasePosition * curKey.Position;
                                    )
                                    else
                                    (
                                        curBoneNode.Controller.Position = savedTransforms[keyIdx][1] + ( track.BasePosition * curKey.Position )
                                    )
                                )
                                
                                (GfdAnimationKeyframeType_RHalf):
                                (
                                    if ( not isBlend ) then
                                    (
                                        curBoneNode.Controller.Rotation = inverse curKey.Rotation
                                    )
                                    else
                                    (
                                        curBoneNode.Controller.Rotation = savedTransforms[keyIdx][2] * inverse curKey.Rotation
                                    )
                                )
                                
                                (GfdAnimationKeyframeType_SHalf):
                                (
                                    if ( not isBlend ) then
                                    (
                                        curBoneNode.Controller.Scale = track.BaseScale * curKey.Scale
                                    )
                                    else
                                    (
                                        curBoneNode.Controller.Rotation = savedTransforms[keyIdx][2] * inverse curKey.Rotation
                                        curBoneNode.Controller.Scale = savedTransforms[keyIdx][3] + ( track.BaseScale * curKey.Scale )
                                    )
                                )
                            )
                        )
                    )
                )
            )
        )
    ),
    
    public fn SetupScene gfdModel =
    (
        for gfdNode in gfdModel.Scene.Nodes do
        (
            if ( gfdNode.Name != "RootNode" ) then 
            (
                gfdNode.MaxBoneNode = SetupNodeBone(gfdNode)
            )
        )
        
        for gfdNodeIndex = 1 to gfdModel.Scene.Nodes.Count do
        (
            local gfdNode = gfdModel.Scene.Nodes[gfdNodeIndex]
            
            Format "GfdFileLoader.SetupScene: Node % id %\n" gfdNode.Name gfdNodeIndex
            
            local meshAttachmentIndex = 1;
            
            for attachmentIdx = 1 to gfdNode.Attachments.Count do
            (
                if (gfdNode.Attachments[attachmentIdx].Type != 4) 
                    then continue
                
                local gfdMesh = gfdNode.Attachments[attachmentIdx].Object
                
                local meshCount = 1;
                if ( gfdMesh.MorphTargetList != undefined ) then
                    meshCount = gfdMesh.MorphTargetList.Targets.Count;
                
                for meshIndex = 1 to meshCount do
                (
                    local vertexPositions = gfdMesh.VertexPositions;
                    if ( meshIndex != 1 and gfdMesh.MorphTargetList != undefined ) then
                    (
                        -- Make copy of vertex positions list
                        local temp = #();
                        for pos in vertexPositions do
                            append temp pos;
                        
                        vertexPositions = temp;
                        
                        -- Transform vertex positions
                        target = gfdMesh.MorphTargetList.Targets[ meshIndex ];
                        for i = 1 to target.VertexPositionOffsets.Count do
                            vertexPositions[ i ] += target.VertexPositionOffsets[ i ];
                    )
                    
                    -- Build appropriate name for mesh
                    local meshName = (gfdNode.Name + "_Mesh" + meshAttachmentIndex as string);
                    if ( meshIndex > 1 ) then
                        meshName += ( "_MorphTarget" + ( meshIndex - 1 ) as string );
                    
                    -- Create max mesh node
                    local maxMeshNode = Mesh Vertices:vertexPositions Faces:gfdMesh.FaceIndices Name:meshName;
                    maxMeshNode.Parent = gfdNode.MaxBoneNode
                    maxMeshNode.BackfaceCull = on;
                    
                    Format "GfdFileLoader.SetupScene: Setting up mesh for %\n" maxMeshNode.Name
                    
                    /* Apply vertex normals */
                    if (ApplyNormals) then
                    (
                        for j = 1 to gfdMesh.VertexNormals.Count do 
                            SetNormal maxMeshNode j gfdMesh.VertexNormals[j]
                    )

                    --if (not ((bit.and gfdMesh.Flags2 0x80000000) == 0x80000000)) then
                    (
                        /* Transform mesh to model space */
                        --CenterPivot maxMeshNode
                        maxMeshNode.Transform  = gfdNode.MaxBoneNode.Transform
                        
                        /*
                        local idx = finditem gfdModel.Scene.UsedBoneMap (gfdNodeIndex - 1)
                        
                        if ( idx != 0 ) then
                        (
                            maxMeshNode.Transform = ( inverse gfdModel.Scene.InvBindMatrices[idx] )
                            if ( gfdNode.Parent != undefined ) then
                            (
                                maxMeshNode.Transform *= ( inverse gfdNode.Parent.MaxBoneNode.Trasnform )
                            )
                        )
                        else
                        (
                            maxMeshNode.Transform  = gfdNode.MaxBoneNode.Transform
                        )
                        */
                    )
                    
                    /* Set up material */
                    maxMeshNode.Material = StandardMaterial()
                    maxMeshNode.Material.ShowInViewport = true
                    maxMeshNode.Material.Name = gfdMesh.MaterialName			
                    
                    /* Apply up to 8 texture coordinate channels */
                    MeshOp.SetNumMaps maxMeshNode 8
                    for i = 1 to 8 do
                    (
                        MeshOp.DefaultMapFaces maxMeshNode i
                        for j = 1 to gfdMesh.VertexTexCoordChannels[i].Count do
                        (
                            MeshOp.SetMapVert maxMeshNode i j gfdMesh.VertexTexCoordChannels[i][j]
                        )
                    )
                    
                    /* Apply vertex colors */
                    if (ApplyColors and ((bit.and gfdMesh.VertexFormat GfdGeometryVertexFlags_Color) == GfdGeometryVertexFlags_Color)) then
                    (
                        setNumCPVVerts maxMeshNode gfdMesh.VertexCount
                        setCVertMode maxMeshNode true
                        setShadeCVerts maxMeshNode true
                        defaultVCFaces maxMeshNode
                        Meshop.SupportVAlphas maxMeshNode
                        
                        for j = 1 to gfdMesh.VertexColors.Count do
                        (
                            local clr = gfdMesh.VertexColors[j]
                            local b = bit.and clr 0xFF
                            local g = bit.shift (bit.and clr 0xFF00) -8
                            local r = bit.shift (bit.and clr 0xFF0000) -16
                            local a = bit.shift (bit.and clr 0xFF000000) -24
                            SetVertColor maxMeshNode j [r, g, b]
                            Meshop.SetVAlpha maxMeshNode j (a as float / 255f)
                        )
                    )

                    /* Apply material textures */
                    if (gfdModel.Materials != undefined) then
                    (
                        local gfdMaterial = undefined
                        for mat in gfdModel.Materials do
                        (
                            if (mat.Hash == gfdMesh.MaterialHash) then
                            (
                                gfdMaterial = mat
                            )
                        )
                    
                        if (gfdMaterial != undefined) then
                        (	
                            maxMeshNode.Material.AdLock 	= off
                            maxMeshNode.Material.Ambient 	= [gfdMaterial.Ambient.X * 255, gfdMaterial.Ambient.Y * 255, gfdMaterial.Ambient.Y * 255]
                            maxMeshNode.Material.Diffuse 	= [gfdMaterial.Diffuse.X * 255, gfdMaterial.Diffuse.Y * 255, gfdMaterial.Diffuse.Z * 255]						
                            maxMeshNode.Material.Specular 	= [gfdMaterial.Specular.X * 255, gfdMaterial.Specular.Y * 255, gfdMaterial.Specular.Z * 255]						
                            --maxMeshNode.Material.UseSelfIllumColor = true
                            maxMeshNode.Material.SelfIllumColor = [gfdMaterial.Emissive.X * 255, gfdMaterial.Emissive.Y * 255, gfdMaterial.Emissive.Z * 255]
                            
                            local textureDirectory = mFileInfo.FileDirectory
                            if (mLoadExternalTextures) then
                                textureDirectory += "textures\\"
                                        
                            if (gfdMaterial.TextureSlots[GfdMaterialDiffuseTextureSlotID] != undefined) then
                            (
                                maxMeshNode.Material.DiffuseMap = BitmapTexture Filename:(textureDirectory + gfdMaterial.TextureSlots[GfdMaterialDiffuseTextureSlotID].TextureName) AlphaSource:0 MonoOutput:1
                            )
                            
                            if (gfdMaterial.TextureSlots[GfdMaterialNormalTextureSlotID] != undefined) then
                            (
                                maxMeshNode.Material.BumpMapAmount = 100
                                maxMeshNode.Material.BumpMap = Normal_Bump()
                                maxMeshNode.Material.BumpMap.Normal_map = BitmapTexture Filename:(textureDirectory + gfdMaterial.TextureSlots[GfdMaterialNormalTextureSlotID].TextureName) AlphaSource:0 MonoOutput:1
                            )
                            
                            if (gfdMaterial.TextureSlots[GfdMaterialSpecularTextureSlotID] != undefined) then
                            (
                                maxMeshNode.Material.SpecularLevelMapAmount = 25
                                maxMeshNode.Material.SpecularLevelMap = BitmapTexture Filename:(textureDirectory + gfdMaterial.TextureSlots[GfdMaterialSpecularTextureSlotID].TextureName) AlphaSource:0 MonoOutput:1
                            )
                            
                            if (gfdMaterial.TextureSlots[GfdMaterialReflectionTextureSlotID] != undefined) then
                            (
                                maxMeshNode.Material.ReflectionMap = BitmapTexture Filename:(textureDirectory + gfdMaterial.TextureSlots[GfdMaterialReflectionTextureSlotID].TextureName) AlphaSource:0 MonoOutput:1
                            )
                            
                            if (gfdMaterial.TextureSlots[GfdMaterialHighlightTextureSlotID] != undefined) then
                            (
                                maxMeshNode.Material.SelfIllumMap = BitmapTexture Filename:(textureDirectory + gfdMaterial.TextureSlots[GfdMaterialHighlightTextureSlotID].TextureName) AlphaSource:0 MonoOutput:1
                            )
                        )
                    )
                    
                    /* Apply skin */
                    if (ApplySkin) then
                    (						
                        if (((Bit.And gfdMesh.Flags 1) == 1)) then
                        (
                            Format "GfdFileLoader.SetupScene: Setting up skin modifier\n"
                            
                            Max modify mode
                            Select maxMeshNode
                            skinMod = Skin()
                            AddModifier maxMeshNode skinMod
                            
                            /* Get used skinning nodes */
                            for i = 1 to gfdModel.Scene.UsedBoneCount do
                            (
                                SkinOps.AddBone skinMod gfdModel.Scene.Nodes[gfdModel.Scene.UsedBoneMap[i] + 1].MaxBoneNode 0
                            )
                            
                            /* Set the weight for each of them */
                            ModPanel.SetCurrentObject skinMod
                            for i = 1 to gfdMesh.VertexCount do 
                            (
                                SkinOps.SetVertexWeights skinMod i gfdMesh.VertexWeightIndices[i] gfdMesh.VertexWeights[i]
                            )
                            
                            Max create mode
                        )
                    )
                    
                    if ( meshIndex == 1 ) then
                    (
                        -- Set up morpher modifier for base mesh
                        gfdMesh.MaxMeshNode = maxMeshNode;
                        max modify mode;
                        Select maxMeshNode;
                        local morpherModifier = morpher();
                        morpherModifier.Autoload_of_targets = 1;
                        AddModifier maxMeshNode morpherModifier;
                    )
                    else
                    (
                        -- Add morph target to the base mesh's morpher modifier
                        WM3_MC_BuildFromNode gfdMesh.MaxMeshNode.morpher ( meshIndex - 1 ) maxMeshNode;
                        
                        -- Hide the morph target mesh
                        Hide maxMeshNode;
                    )
                    
                    if ( meshIndex == meshCount ) then
                        meshAttachmentIndex += 1;
                )
            )
        )
        
        /*
        if (ApplyYToZUp) then
        (
            Rotate gfdModel.Scene.Nodes[2].MaxBoneNode (AngleAxis 90 [1,0,0])
        )
        */
    ),
    
    private fn SetupNodeBone gfdNode = 
    (
        /* Create max bone node for GfdNode */
        
        --local tfm = ((scalematrix gfdNode.Scale) *  (gfdNode.Rotation as matrix3)) * (transmatrix gfdNode.Position) 
    
        local tfm = (Inverse gfdNode.Rotation) as matrix3
        tfm *= ScaleMatrix gfdNode.Scale
        tfm.row4 = gfdNode.Position
        
        if ( (gfdNode.Parent == undefined or gfdNode.Parent.MaxBoneNode == undefined) and ApplyYToZUp ) then
        (
            tfm *= Y_TO_Z_UP_MATRIX
        )
            
        local newBone = BoneSys.CreateBone tfm.row4 (tfm.row4 + 0.01 * (normalize tfm.row1)) (normalize tfm.row3)
        newBone.Name = gfdNode.Name
        newBone.Width  = 0.01
        newBone.Height = 0.01
        newBone.Transform = tfm
        newBone.SetBoneEnable false 0
        newBone.Wirecolor = red
        newbone.Showlinks = true
        newBone.Pos.Controller      = TCB_position()
        newBone.Rotation.Controller = TCB_rotation()
                          
        if ( gfdNode.Parent != undefined and gfdNode.Parent.MaxBoneNode != undefined ) do 
        (
            newBone.Parent = gfdNode.Parent.MaxBoneNode
            newBone.Transform *= gfdNode.Parent.MaxBoneNode.transform
        )
        
        for prop in gfdNode.UserProperties do
        (
            SetUserProp newBone prop.Key (prop.Value as string)
        )
        
        newBone
    )
)
