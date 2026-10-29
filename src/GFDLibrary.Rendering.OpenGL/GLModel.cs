using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using GFDLibrary.Animations;
using GFDLibrary.Effects;
using GFDLibrary.Materials;
using GFDLibrary.Models;
using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL4;
using Quaternion = System.Numerics.Quaternion;
using Vector3 = System.Numerics.Vector3;

namespace GFDLibrary.Rendering.OpenGL
{
    public class GLModel : IDisposable
    {
        public ModelPack ModelPack { get; }

        public List<GLNode> Nodes { get; }

        public Dictionary<string, GLBaseMaterial> Materials { get; }

        public Animation Animation { get; private set; }

        public Animation BlendAnimation { get; private set; }

        private readonly List<AnimationJoint> mMaterialControllers = new();

        public GLModel( ModelPack modelPack, MaterialTextureCreator textureCreator )
        {
            ModelPack = modelPack;

            var nodes = modelPack.Mesh.Nodes.ToList();
            Nodes = nodes.Select( x => new GLNode( x ) ).ToList();

            Materials = modelPack.Materials?.ToDictionary( x => x.Key, y => GLBaseMaterial.CreateGLMaterial( y.Value, textureCreator ) ) ??
                        new Dictionary<string, GLBaseMaterial>();

            foreach ( var glNode in Nodes )
            {
                glNode.Parent = Nodes.FirstOrDefault( x => x.Node == glNode.Node.Parent );
                if ( !glNode.Node.HasAttachments )
                    continue;

                foreach ( var attachment in glNode.Node.Attachments )
                {
                    if ( attachment.Type != NodeAttachmentType.Geometry )
                        continue;

                    var glMesh = new GLMesh( attachment.GetValue<Geometry>(), glNode.Node.WorldTransform, modelPack.Mesh.Bones, Nodes, Materials, modelPack.Version );
                    glNode.Geometries.Add( glMesh );
                }
            }
        }

        /// <summary>
        /// Walks an Epl attachment and hangs whatever it can draw off the node it is attached to.
        /// Only leaves carrying an embedded model are drawable; the rest describe particles and
        /// post effects that nothing here simulates.
        /// </summary>
        public void AddEplNodes( Epl epl, GLNode parentGlNode )
        {
            foreach ( var child in epl.RootNode.Children )
                AddEplNodeRecursive( child, parentGlNode );

            if ( epl.RootNode.HasAttachments )
                AddEplNodeAttachments( epl.RootNode, parentGlNode );
        }

        public void AddEplLeafNodes( EplLeaf eplLeaf, GLNode glNode ) => AddEplLeafModel( eplLeaf, glNode );

        private void AddEplNodeRecursive( Node eplNode, GLNode parentGlNode )
        {
            var glNode = Attach( eplNode, parentGlNode );

            AddEplNodeAttachments( eplNode, glNode );

            foreach ( var child in eplNode.Children )
                AddEplNodeRecursive( child, glNode );
        }

        private void AddEplNodeAttachments( Node eplNode, GLNode glNode )
        {
            if ( !eplNode.HasAttachments )
                return;

            foreach ( var attachment in eplNode.Attachments )
            {
                switch ( attachment.Type )
                {
                    case NodeAttachmentType.Geometry:
                        glNode.Geometries.Add( new GLMesh( attachment.GetValue<Geometry>(), glNode.WorldTransform, ModelPack.Mesh.Bones, Nodes, Materials, ModelPack.Version ) );
                        break;

                    case NodeAttachmentType.Epl:
                        AddEplNodes( attachment.GetValue<Epl>(), glNode );
                        break;

                    case NodeAttachmentType.EplLeaf:
                        AddEplLeafModel( attachment.GetValue<EplLeaf>(), glNode );
                        break;
                }
            }
        }

        private void AddEplLeafModel( EplLeaf eplLeaf, GLNode attachmentGlNode )
        {
            var embeddedModelPack = TryLoadModelPackFromEplLeaf( eplLeaf );
            if ( embeddedModelPack == null )
                return;

            if ( embeddedModelPack.Materials != null && embeddedModelPack.Textures != null )
            {
                foreach ( var pair in embeddedModelPack.Materials )
                {
                    if ( Materials.ContainsKey( pair.Key ) )
                        continue;

                    Materials[pair.Key] = GLBaseMaterial.CreateGLMaterial( pair.Value, ( material, textureName ) =>
                        embeddedModelPack.Textures.TryGetTexture( textureName, out var texture ) ? new GLTexture( texture ) : null );
                }
            }

            var embeddedRootNode = embeddedModelPack.Mesh.RootNode;

            if ( embeddedRootNode.HasAttachments )
                AddEmbeddedNodeAttachments( embeddedRootNode, attachmentGlNode, embeddedModelPack );

            foreach ( var child in embeddedRootNode.Children )
                AddEmbeddedNodeRecursive( child, attachmentGlNode, embeddedModelPack );
        }

        private void AddEmbeddedNodeRecursive( Node embeddedNode, GLNode parentGlNode, ModelPack embeddedModelPack )
        {
            var glNode = Attach( embeddedNode, parentGlNode );

            AddEmbeddedNodeAttachments( embeddedNode, glNode, embeddedModelPack );

            foreach ( var child in embeddedNode.Children )
                AddEmbeddedNodeRecursive( child, glNode, embeddedModelPack );
        }

        private void AddEmbeddedNodeAttachments( Node embeddedNode, GLNode glNode, ModelPack embeddedModelPack )
        {
            if ( !embeddedNode.HasAttachments )
                return;

            foreach ( var attachment in embeddedNode.Attachments )
            {
                if ( attachment.Type == NodeAttachmentType.Geometry )
                    glNode.Geometries.Add( new GLMesh( attachment.GetValue<Geometry>(), glNode.WorldTransform, embeddedModelPack.Mesh.Bones, Nodes, Materials, embeddedModelPack.Version ) );
            }
        }

        /// <summary>
        /// Adds a node under a parent, working its world transform out from the parent chain we
        /// are building rather than the one the node came from.
        /// </summary>
        private GLNode Attach( Node node, GLNode parentGlNode )
        {
            var glNode = new GLNode( node ) { Parent = parentGlNode };

            var transform = Matrix4x4.CreateFromQuaternion( node.Rotation ) * Matrix4x4.CreateScale( node.Scale );
            transform.Translation = node.Translation;

            glNode.CurrentTransform = transform;
            glNode.WorldTransform = glNode.CurrentTransform * parentGlNode.WorldTransform;

            Nodes.Add( glNode );
            return glNode;
        }

        private static ModelPack TryLoadModelPackFromEplLeaf( EplLeaf eplLeaf )
        {
            if ( eplLeaf.Data is not EplModel eplModel )
                return null;

            if ( eplModel.HasEmbeddedFile != 1 || eplModel.EmbeddedFile?.Data == null || eplModel.EmbeddedFile.Data.Length == 0 )
                return null;

            try
            {
                using var stream = new System.IO.MemoryStream( eplModel.EmbeddedFile.Data );
                return Resource.Load<ModelPack>( stream );
            }
            catch ( Exception exception )
            {
                Trace.TraceWarning( $"Epl leaf \"{eplLeaf.Name}\" holds a file that is not a model pack: {exception.Message}" );
                return null;
            }
        }

        public void LoadAnimation( Animation animation )
        {
            Animation = animation;

            foreach ( var glNode in Nodes )
            {
                glNode.Controllers.Clear();
                glNode.Controllers.AddRange( animation.Joints.Where( x => x.TargetKind == TargetKind.Node &&
                                                                               x.TargetName == glNode.Node.Name ) );
            }

            mMaterialControllers.Clear();
            mMaterialControllers.AddRange( animation.Joints.Where( x => x.TargetKind == TargetKind.Material ) );
        }

        /// <summary>
        /// A blend animation plays on top of whatever is already posing the model, so its
        /// controllers are kept apart from the main ones.
        /// </summary>
        public void LoadBlendAnimation( Animation animation )
        {
            BlendAnimation = animation;

            foreach ( var glNode in Nodes )
            {
                glNode.BlendControllers.Clear();
                glNode.BlendControllers.AddRange( animation.Joints.Where( x => x.TargetKind == TargetKind.Node &&
                                                                                    x.TargetName == glNode.Node.Name ) );
            }
        }

        public void UnloadAnimation()
        {
            Animation = null;
            BlendAnimation = null;

            mMaterialControllers.Clear();

            foreach ( var material in Materials.Values )
            {
                material.AnimatedAlpha = 1.0f;
                material.UVOffset = System.Numerics.Vector2.Zero;
                material.UVScale = System.Numerics.Vector2.One;
                material.UVRotation = 0f;
                material.UV1Offset = System.Numerics.Vector2.Zero;
                material.UV1Scale = System.Numerics.Vector2.One;
                material.UV1Rotation = 0f;
            }

            foreach ( var glNode in Nodes )
            {
                glNode.BlendControllers.Clear();

                // Calculate current transform
                var transform = Matrix4x4.CreateFromQuaternion( glNode.Node.Rotation ) * Matrix4x4.CreateScale( glNode.Node.Scale );
                transform.Translation   = glNode.Node.Translation;
                glNode.CurrentTransform = transform;

                // Calculate world transform
                glNode.WorldTransform =
                    glNode.Parent == null ? glNode.CurrentTransform : glNode.CurrentTransform * glNode.Parent.WorldTransform;
            }

            foreach ( var glNode in Nodes )
            {
                // Rebuild geometries
                for ( var i = 0; i < glNode.Geometries.Count; i++ )
                {
                    var oldGlMesh = glNode.Geometries[i];
                    if (oldGlMesh.Geometry != null)
                    {
                        glNode.Geometries[i] = new GLMesh( oldGlMesh.Geometry, glNode.WorldTransform, ModelPack.Mesh.Bones, Nodes, Materials, ModelPack.Version );
                    }

                    oldGlMesh.Dispose();
                }
            }
        }

        private GLShaderProgram GetTargetShader(ShaderRegistry shaderRegistry, GLMesh glMesh, Matrix4 view, Matrix4 projection, HashSet<ResourceType> shaderPrograms )
        {
            if ( typeof( GLMetaphorMaterial ).IsInstanceOfType( glMesh.Material )
                && shaderRegistry.mMetaphorShaders.TryGetValue( ( (GLMetaphorMaterial)glMesh.Material ).ParameterSet.ResourceType, out var metaphorShader ) )
            {
                if ( !shaderPrograms.Contains( ((GLMetaphorMaterial)glMesh.Material).ParameterSet.ResourceType ) )
                {
                    metaphorShader.Use();
                    metaphorShader.SetUniform( "uView", view );
                    metaphorShader.SetUniform( "uProjection", projection );
                }
                return metaphorShader;
            }
            return shaderRegistry.mDefaultShader;
        }

        /// <summary>
        /// Matches MAX_BONES in the vertex shaders. The widest single geometry seen across the test
        /// corpus uses 83 bones, so this leaves room without overrunning the uniform space.
        /// </summary>
        private const int MaxPaletteBones = 96;

        private float[] mBonePalette = new float[0];
        private readonly HashSet<string> mReportedOversizedMeshes = new();

        /// <summary>
        /// Fills the shader's bone palette for one geometry. Each bone goes up as the top three rows
        /// of its matrix, the fourth being constant for an affine transform, which is a quarter
        /// less to upload and keeps the array inside the smallest guaranteed uniform space.
        /// </summary>
        private void UploadBonePalette( GLMesh glMesh, GLShaderProgram shader )
        {
            if ( !glMesh.IsSkinned )
                return;

            var palette = glMesh.PaletteBones;

            if ( palette.Length > MaxPaletteBones )
            {
                if ( mReportedOversizedMeshes.Add( glMesh.Geometry?.MaterialName ?? "?" ) )
                    Trace.TraceWarning( $"Geometry uses {palette.Length} bones, more than the {MaxPaletteBones} the shader holds" );

                return;
            }

            var needed = palette.Length * 3 * 4;

            if ( mBonePalette.Length < needed )
                mBonePalette = new float[needed];

            for ( var i = 0; i < palette.Length; i++ )
            {
                var bone = ModelPack.Mesh.Bones[palette[i]];
                var matrix = bone.InverseBindMatrix * Nodes[bone.NodeIndex].WorldTransform;
                var at = i * 12;

                // The shader multiplies column vectors, so each row here is a column of the
                // matrix as System.Numerics lays it out.
                mBonePalette[at + 0] = matrix.M11;
                mBonePalette[at + 1] = matrix.M21;
                mBonePalette[at + 2] = matrix.M31;
                mBonePalette[at + 3] = matrix.M41;

                mBonePalette[at + 4] = matrix.M12;
                mBonePalette[at + 5] = matrix.M22;
                mBonePalette[at + 6] = matrix.M32;
                mBonePalette[at + 7] = matrix.M42;

                mBonePalette[at + 8] = matrix.M13;
                mBonePalette[at + 9] = matrix.M23;
                mBonePalette[at + 10] = matrix.M33;
                mBonePalette[at + 11] = matrix.M43;
            }

            shader.SetUniform( "uBones", mBonePalette, palette.Length * 3 );
        }

        private bool ShouldMeshShowAsSelected( DrawContext context, GLMesh glMesh )
        {
            return context.SelectedMesh == glMesh.Geometry || context.SelectedMaterial?.Name == glMesh.Geometry.MaterialName;
        }

        public void Draw( DrawContext context )
        {
            if ( Animation != null || BlendAnimation != null )
                AnimateNodes( context.AnimationTime );
            context.ShaderRegistry.mDefaultShader.Use();
            context.ShaderRegistry.mDefaultShader.SetUniform( "uView", context.Camera.View );
            context.ShaderRegistry.mDefaultShader.SetUniform( "uProjection", context.Camera.Projection );

            HashSet<ResourceType> shaderProgramsInUse = new();

            // List to hold transparent geometries and their world transforms
            List<Tuple<GLMesh, Matrix4, GLShaderProgram>> transparentMeshes = new();

            // Draw opaque objects first
            foreach ( var glNode in Nodes )
            {
                if ( !glNode.IsVisible )
                    continue;

                for ( var i = 0; i < glNode.Geometries.Count; i++ )
                {
                    var glMesh = glNode.Geometries[i];


                    GLShaderProgram targetShader = GetTargetShader( context.ShaderRegistry, glMesh, context.Camera.View, context.Camera.Projection, shaderProgramsInUse );
                    if ( !glMesh.Material.IsMaterialTransparent() ) // If opaque
                    {
                        if ( glMesh.Geometry != null )
                            targetShader.SetUniform( "uIsSelected", ShouldMeshShowAsSelected(context, glMesh ) );
                        UploadBonePalette( glMesh, targetShader );
                        glMesh.Draw( glNode.WorldTransform.ToOpenTK(), targetShader );
                    }
                    else // If transparent
                    {
                        transparentMeshes.Add( Tuple.Create( glMesh, glNode.WorldTransform.ToOpenTK(), targetShader ) );
                    }
                }
            }

            // Enable blending for transparent objects
            GL.Enable( EnableCap.Blend );

            // Sort transparent objects based on their distance from the camera
            transparentMeshes.Sort( ( a, b ) => OpenTK.Mathematics.Vector3.Distance( context.Camera.Translation, b.Item2.ExtractTranslation() ).CompareTo( OpenTK.Mathematics.Vector3.Distance( context.Camera.Translation, a.Item2.ExtractTranslation() ) ) );

            // Disable depth mask
            GL.DepthMask( false );

            // Then draw transparent objects
            foreach ( (var glMesh, var worldTransform, var shaderProgram) in transparentMeshes )
            {
                switch ( glMesh.Material.DrawMethod )
                {
                    case 1:
                        GL.BlendFunc( BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha );
                        break;
                    case 2:
                        GL.BlendFuncSeparate( BlendingFactorSrc.SrcAlpha, BlendingFactorDest.One, // RGB blending
                                              BlendingFactorSrc.Zero, BlendingFactorDest.One ); // Alpha blending
                        break;
                    case 4:
                        GL.BlendFunc( BlendingFactor.DstColor, BlendingFactor.Zero );
                        break;
                }

                if ( glMesh.Geometry != null )
                    shaderProgram.SetUniform( "uIsSelected", ShouldMeshShowAsSelected( context, glMesh ) );
                UploadBonePalette( glMesh, shaderProgram );
                glMesh.Draw( worldTransform, shaderProgram );
            }

            // Re-enable depth mask
            GL.DepthMask( true );

            // Disable blending after drawing transparent objects
            GL.Disable( EnableCap.Blend );
        }


        private void AnimateNodes( double animationTime )
        {
            foreach ( var glNode in Nodes )
            {
                var rotation    = glNode.Node.Rotation;
                var translation = glNode.Node.Translation;
                var scale       = glNode.Node.Scale;

                foreach ( var controller in glNode.Controllers )
                {
                    foreach ( var layer in controller.Layers )
                    {
                        Key curKey = null;
                        Key nextKey = null;

                        if ( controller != null )
                        {
                            (curKey, nextKey) = GetCurrentAndNextKeys( layer, animationTime );
                        }

                        if ( controller != null && curKey != null )
                        {
                            if ( layer.HasPRSKeyFrames )
                            {
                                var prsKey = ( PRSKey )curKey;
                                var nextPrsKey = ( PRSKey )nextKey;

                                if ( nextPrsKey != null )
                                {
                                    InterpolateKeys( animationTime, layer, ref rotation, ref translation, ref scale, prsKey, nextPrsKey, Animation.Duration );
                                }
                                else
                                {
                                    if ( prsKey.HasRotation )
                                        rotation = prsKey.Rotation;

                                    if ( prsKey.HasPosition )
                                        translation = prsKey.Position * layer.PositionScale;

                                    if ( prsKey.HasScale )
                                        scale = prsKey.Scale * layer.ScaleScale;
                                }
                            }
                            else
                            {
                                //Debugger.Break();
                            }
                        }
                    }
                }

                foreach ( var blendController in glNode.BlendControllers )
                {
                    foreach ( var layer in blendController.Layers )
                    {
                        Key curKey;
                        Key nextKey = null;

                        // Past the end of the blend animation the last key is held, rather than
                        // wrapping round and interpolating towards the start again.
                        if ( BlendAnimation.Duration > 0 && animationTime >= BlendAnimation.Duration )
                            curKey = layer.Keys.LastOrDefault();
                        else
                            (curKey, nextKey) = GetCurrentAndNextKeys( layer, animationTime );

                        if ( curKey == null || !layer.HasPRSKeyFrames )
                            continue;

                        var prsKey = ( PRSKey )curKey;
                        var nextPrsKey = ( PRSKey )nextKey;

                        if ( nextPrsKey != null )
                        {
                            var offsetRotation = Quaternion.Identity;
                            var offsetTranslation = Vector3.Zero;
                            var offsetScale = Vector3.Zero;

                            InterpolateKeys( animationTime, layer, ref offsetRotation, ref offsetTranslation, ref offsetScale, prsKey, nextPrsKey, BlendAnimation.Duration );

                            translation += offsetTranslation;
                            rotation = Quaternion.Concatenate( offsetRotation, rotation );
                            scale += offsetScale;
                        }
                        else
                        {
                            if ( prsKey.HasPosition )
                                translation += prsKey.Position * layer.PositionScale;

                            if ( prsKey.HasRotation )
                                rotation = Quaternion.Concatenate( prsKey.Rotation, rotation );

                            if ( prsKey.HasScale )
                                scale += prsKey.Scale * layer.ScaleScale;
                        }
                    }
                }

                // Calculate current transform
                var transform = Matrix4x4.CreateFromQuaternion( rotation ) * Matrix4x4.CreateScale( scale );
                transform.Translation   = translation;
                glNode.CurrentTransform = transform;

                // Calculate world transform
                glNode.WorldTransform =
                    glNode.Parent == null ? glNode.CurrentTransform : glNode.CurrentTransform * glNode.Parent.WorldTransform;
            }

            AnimateMaterials( animationTime );
        }

        /// <summary>
        /// Applies the opacity and uv transform tracks to the materials they name. Both are
        /// behind a material flag, which the game checks before it animates either.
        /// </summary>
        private void AnimateMaterials( double animationTime )
        {
            const int DiffusivityFlag = 1 << 6;
            const int UVAnimationFlag = 1 << 7;

            foreach ( var controller in mMaterialControllers )
            {
                if ( !Materials.TryGetValue( controller.TargetName, out var material ) )
                    continue;

                foreach ( var layer in controller.Layers )
                {
                    if ( layer.HasSingleKeyFrames && layer.KeyType == KeyType.Opacity )
                    {
                        if ( ( material.MatFlags & DiffusivityFlag ) == 0 )
                            continue;

                        AnimateOpacity( material, layer, animationTime );
                    }
                    else if ( layer.HasSingle5KeyFrames )
                    {
                        if ( ( material.MatFlags & UVAnimationFlag ) == 0 )
                            continue;

                        AnimateUVTransform( material, layer, animationTime );
                    }
                }
            }
        }

        private void AnimateOpacity( GLBaseMaterial material, AnimationLayer layer, double animationTime )
        {
            var (curKey, nextKey) = GetCurrentAndNextKeys( layer, animationTime );

            if ( curKey is not SingleKey singleKey )
                return;

            var alpha = singleKey.Value;

            if ( nextKey is SingleKey nextSingleKey )
            {
                var blend = BlendBetween( singleKey.Time, nextSingleKey.Time, animationTime );
                if ( blend.HasValue )
                    alpha = singleKey.Value + ( nextSingleKey.Value - singleKey.Value ) * blend.Value;
            }

            material.AnimatedAlpha = alpha;
        }

        private void AnimateUVTransform( GLBaseMaterial material, AnimationLayer layer, double animationTime )
        {
            var (curKey, nextKey) = GetCurrentAndNextKeys( layer, animationTime );

            if ( curKey is not Single5Key key )
                return;

            var offsetX = key.UVOffsetX;
            var offsetY = key.UVOffsetY;
            var scaleX = key.UVScaleX;
            var scaleY = key.UVScaleY;
            var rotation = key.UVRotation;

            // Only the two interpolating key types blend towards the next key; the NoInterp
            // pair hold their value until the next one starts.
            var interpolates = layer.KeyType == KeyType.UV0Transform || layer.KeyType == KeyType.UV1Transform;

            if ( interpolates && nextKey is Single5Key nextKeyValue )
            {
                var blend = BlendBetween( key.Time, nextKeyValue.Time, animationTime );

                if ( blend.HasValue )
                {
                    offsetX = key.UVOffsetX + ( nextKeyValue.UVOffsetX - key.UVOffsetX ) * blend.Value;
                    offsetY = key.UVOffsetY + ( nextKeyValue.UVOffsetY - key.UVOffsetY ) * blend.Value;
                    scaleX = key.UVScaleX + ( nextKeyValue.UVScaleX - key.UVScaleX ) * blend.Value;
                    scaleY = key.UVScaleY + ( nextKeyValue.UVScaleY - key.UVScaleY ) * blend.Value;
                    rotation = key.UVRotation + ( nextKeyValue.UVRotation - key.UVRotation ) * blend.Value;
                }
            }

            if ( layer.KeyType == KeyType.UV1Transform || layer.KeyType == KeyType.UV1TransformNoInterp )
            {
                material.UV1Offset = new System.Numerics.Vector2( offsetX, offsetY );
                material.UV1Scale = new System.Numerics.Vector2( scaleX, scaleY );
                material.UV1Rotation = rotation;
            }
            else
            {
                material.UVOffset = new System.Numerics.Vector2( offsetX, offsetY );
                material.UVScale = new System.Numerics.Vector2( scaleX, scaleY );
                material.UVRotation = rotation;
            }
        }

        /// <summary>
        /// How far between two key times the animation has got, or null when the pair gives
        /// nothing to blend over. A next key earlier than the current one has wrapped past the
        /// end of the animation.
        /// </summary>
        private float? BlendBetween( float keyTime, float nextKeyTime, double animationTime )
        {
            var nextTime = nextKeyTime < keyTime ? nextKeyTime + Animation.Duration : nextKeyTime;

            if ( nextTime <= keyTime )
                return null;

            return ( float )( ( animationTime - keyTime ) / ( nextTime - keyTime ) );
        }

        private static void InterpolateKeys( double animationTime, AnimationLayer layer, ref Quaternion rotation, ref Vector3 translation, ref Vector3 scale, PRSKey prsKey, PRSKey nextPrsKey, float duration )
        {
            var nextTime = ( nextPrsKey.Time < prsKey.Time
                ? ( nextPrsKey.Time + duration )
                : nextPrsKey.Time );

            var segmentLength = nextTime - prsKey.Time;

            if ( segmentLength <= 0 )
            {
                if ( prsKey.HasRotation )
                    rotation = prsKey.Rotation;

                if ( prsKey.HasPosition )
                    translation = prsKey.Position * layer.PositionScale;

                if ( prsKey.HasScale )
                    scale = prsKey.Scale * layer.ScaleScale;

                return;
            }

            var blend = ( float ) ( ( animationTime - prsKey.Time ) / segmentLength );

            if ( prsKey.HasRotation )
                rotation = Quaternion.Slerp( prsKey.Rotation, nextPrsKey.Rotation, blend );

            if ( prsKey.HasPosition )
            {
                translation = Vector3.Lerp( prsKey.Position * layer.PositionScale,
                                            nextPrsKey.Position * layer.PositionScale,
                                            blend );
            }

            if ( prsKey.HasScale )
            {
                scale = Vector3.Lerp( prsKey.Scale * layer.ScaleScale,
                                      nextPrsKey.Scale * layer.ScaleScale,
                                      blend );
            }
        }

        private static (Key curKey, Key nextKey) GetCurrentAndNextKeys( AnimationLayer layer, double animationTime )
        {
            Key curKey = null;
            Key nextKey = null;

            // Find most recent key
            foreach ( var key in layer.Keys )
            {
                if ( key.Time <= animationTime )
                    curKey = key;
            }

            // Find next key
            if ( curKey != null )
            {
                foreach ( var key in layer.Keys )
                {
                    if ( key != curKey && key.Time != curKey.Time && key.Time >= animationTime )
                    {
                        nextKey = key;
                        break;
                    }
                }

                if ( nextKey == null )
                {
                    nextKey = layer.Keys.FirstOrDefault( x => x.Time != curKey.Time );
                }
            }

            return ( curKey, nextKey );
        }

        #region IDisposable Support
        private bool mDisposed; // To detect redundant calls

        protected virtual void Dispose( bool disposing )
        {
            if ( !mDisposed )
            {
                if ( disposing )
                {
                    GLVertexArray.UnbindAll();
                    GL.BindBuffer( BufferTarget.ArrayBuffer, 0 );
                    GL.BindBuffer( BufferTarget.ElementArrayBuffer, 0 );
                    GL.BindTexture( TextureTarget.Texture2D, 0 );

                    foreach ( var glNode in Nodes )
                    {
                        glNode.Dispose();

                        foreach ( var geometry in glNode.Geometries )
                            geometry.Dispose();
                    }
                }

                mDisposed = true;
            }
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose( true );
        }
        #endregion
    }

    public class DrawContext
    {
        public ShaderRegistry ShaderRegistry { get; init; }
        public GLCamera Camera { get; init; }
        public double AnimationTime { get; init; }
        public Geometry SelectedMesh { get; init; }
        public Material SelectedMaterial { get; init; }
    }
}
