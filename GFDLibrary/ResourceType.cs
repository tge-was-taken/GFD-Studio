﻿namespace GFDLibrary
{
    public enum ResourceType
    {
        Invalid,
        ModelPack,
        ShaderCachePS3,
        ShaderCachePSP2 = 4,
        ShaderCachePS4 = 6,

        // These are all used as intermediary output formats
        TextureMap = 'R' << 24 | 'I' << 16 | 'G' << 8,
        TextureDictionary,
        Texture,
        ShaderPS3,
        ShaderPSP2,
        ShaderPS4,
        Model,
        Node,
        UserPropertyDictionary,
        Morph,
        MorphTarget,
        MorphTargetList,
        MaterialDictionary,
        ChunkType000100F9,
        ChunkType000100F8,
        AnimationPack,
        MaterialAttribute,
        Material,
        Light,
        Mesh,
        Camera,
        Epl,
        EplLeaf,
        Animation,
        AnimationBit29Data,
        AnimationLayer,
        AnimationController,

        EplAnimation,
        EplAnimationController,
        EplLeafDataHeader,
        EplEmbeddedFile,
        EplParticleEmitter,
        EplLeafCommonData,
        EplLeafCommonData2,
        EplDummy,
        EplParticle,
        EplFlashPolygon,
        EplFlashPolygonRadiation,
        EplFlashPolygonExplosion,
        EplFlashPolygonRing,
        EplFlashPolygonSplash,
        EplFlashPolygonCylinder,
        EplCirclePolygon,
        EplCirclePolygonRing,
        EplCirclePolygonTrajectory,
        EplCirclePolygonFill,
        EplCirclePolygonHoop,
        EplLightningPolygon,
        EplLightningPolygonRod,
        EplLightningPolygonBall,
        EplTrajectoryPolygon,
        EplWindPolygon,
        EplWindPolygonSpiral,
        EplWindPolygonExplosion,
        EplWindPolygonBall,
        EplModel,
        EplModel3DData,
        EplModel2DData,
        EplBoardPolygon,
        EplSquareBoardPolygon,
        EplRectangleBoardPolygon,
        EplObjectParticles,
        EplGlitterPolygon,
        EplGlitterPolygonExplosion,
        EplGlitterPolygonSplash,
        EplGlitterPolygonCylinder,
        EplGlitterPolygonWall,
        EplSmokeEffectParams,
        EplExplosionEffectParams,
        EplSpiralEffectParams,
        EplBallEffectParams,
        EplCircleEffectParams,
        EplStraightLineEffectParams,
        EplCamera,
        EplCameraMeshParams,
        EplCameraQuakeParams,
        EplLight,
        EplLightMeshData,
        EplLightSceneData,
        EplPostEffect,
        EplPostEffectRadiationBlurData,
        EplPostEffectStraightBlurData,
        EplPostEffectNoiseBlurData,
        EplPostEffectDistortionBlurData,
        EplPostEffectFillData,
        EplPostEffectLensFlareData,
        EplPostEffectColorCorrectionData,
        EplPostEffectMonotoneData,
        EplHelper,
        EplDirectionalParticles,
    }
}