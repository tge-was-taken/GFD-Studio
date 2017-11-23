using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using AtlusGfdLib;
using AtlusLibSharp.Graphics.RenderWare;
using AtlusLibSharp.Utilities;
using CSharpImageLibrary;

namespace AtlusGfdLibTesting
{

    interface IModelConverter<T>
    {
        Model ConvertFrom( T source );
    }

    public abstract class RmdSceneModelConverterBase : IModelConverter<RmdScene>
    {
        protected const string MATERIAL_POSTFIX = "_mat";
        protected const string TEXTURE_POSTFIX = ".dds";

        private Dictionary<string, Node> mLookup;

        protected RmdSceneModelConverterBase()
        {
        }

        public abstract Model ConvertFrom( RmdScene source );

        //
        // Scene conversion
        //
        protected Scene ConvertFrom( RwClumpNode rwClump )
        {
            var scene = new Scene( 0x01105070 );
            scene.RootNode = ConvertFromRecursive( rwClump.FrameList[0] );

            foreach ( var rwAtomic in rwClump.Atomics )
            {
                ConvertFrom( rwAtomic, rwClump.GeometryList, rwClump.FrameList );
            }

            var vertices = new List<Vector3>();
            foreach ( var vertex in scene.EnumerateGeometries().SelectMany( x => x.Vertices) )
            {
                vertices.Add( vertex );
            }

            scene.BoundingBox = BoundingBox.Calculate( vertices );
            scene.BoundingSphere = BoundingSphere.Calculate( scene.BoundingBox.Value, vertices );

            return scene;
        }

        //
        // Texture conversion
        //
        protected TextureDictionary ConvertFrom( RwTextureDictionaryNode rwTextureDictionary )
        {
            TextureDictionary textureDictionary = new TextureDictionary( 0x01105070 );

            foreach ( var rwTexture in rwTextureDictionary.Textures )
            {
                var texture = ConvertFrom( rwTexture );
                textureDictionary.Add( texture );
            }

            return textureDictionary;
        }

        protected Texture ConvertFrom( RwTextureNativeNode rwTextureNative )
        {
            var ddsData = ConvertToDDS( rwTextureNative );
            return new Texture( rwTextureNative.Name + TEXTURE_POSTFIX, TextureFormat.DDS, ddsData );
        }

        protected byte[] ConvertToDDS( RwTextureNativeNode texture )
        {
            var bitmap = texture.GetBitmap();

            var bitmapStream = new MemoryStream();
            bitmap.Save( bitmapStream, ImageFormat.Bmp );

            var image = new ImageEngineImage( bitmapStream );
            var ddsData = image.Save( new ImageFormats.ImageEngineFormatDetails( ImageEngineFormat.DDS_DXT1 ), MipHandling.KeepTopOnly );

            return ddsData;
        }

        // 
        // Material conversion
        //
        protected MaterialDictionary ConvertFrom( IEnumerable<RwMaterial> rwMaterials )
        {
            var materials = new MaterialDictionary( 0x01105070 );

            foreach ( var rwMaterial in rwMaterials )
            {
                var material = ConvertFrom( rwMaterial );
                if ( materials.ContainsKey( material.Name ) )
                    continue;
            }

            return materials;
        }

        protected Material ConvertFrom( RwMaterial rwMaterial )
        {
            string materialName = GetName( rwMaterial );

            Material material;

            if ( rwMaterial.IsTextured )
            {
                material = new Material( materialName );
                material.Ambient = new Vector4( 0.3921569f, 0.3921569f, 0.3921569f, 1f );
                material.Diffuse = new Vector4( 0.3921569f, 0.3921569f, 0.3921569f, 1f );
                material.DiffuseMap = new TextureMap( GetTextureName( rwMaterial ) );
                material.Emissive = new Vector4( 0, 0, 0, 0 );
                material.Field40 = 1;
                material.Field44 = 0.1f;
                material.DrawOrder = 0;
                material.Field49 = 1;
                material.Field4A = 0;
                material.Field4B = 1;
                material.Field4C = 0;
                material.Field50 = 0;
                material.Field5C = 0;
                material.Field6C = 0xfffffff8;
                material.Field70 = 0xfffffff8;
                material.Field90 = 0;
                material.Field92 = 4;
                material.Field94 = 1;
                material.Field96 = 0;
                material.Field98 = 0xffffffff;
                material.Flags = MaterialFlags.Flag1 | MaterialFlags.Flag2 | MaterialFlags.Flag20 | MaterialFlags.Flag40 | MaterialFlags.EnableLight | MaterialFlags.EnableLight2 | MaterialFlags.ReceiveShadow | MaterialFlags.HasDiffuseMap;
                material.GlowMap = null;
                material.HighlightMap = null;
                material.NightMap = null;
                material.Attributes = null;
                material.ReflectionMap = null;
                material.ShadowMap = null;
                material.Specular = new Vector4( 0, 0, 0, 0 );
                material.SpecularMap = null;
            }
            else
            {
                material = new Material( materialName );
            }

            return material;
        }

        protected string GetName( RwMaterial rwMaterial )
        {
            return rwMaterial.TextureReferenceNode.ReferencedTextureName + MATERIAL_POSTFIX;
        }

        protected string GetTextureName( RwMaterial rwMaterial )
        {
            return rwMaterial.TextureReferenceNode.ReferencedTextureName + TEXTURE_POSTFIX;
        }

        //
        // Node conversion
        //
        protected Node ConvertFrom( RwFrame rwFrame )
        {
            Matrix4x4.Decompose( rwFrame.Transform, out var scale, out var rotation, out var translation );

            Node node = new Node(
                GetName( rwFrame ),
                translation,
                rotation,
                scale );

            return node;
        }

        protected Node ConvertFromRecursive( RwFrame rwFrame )
        {
            var node = ConvertFrom( rwFrame );

            mLookup.Add( node.Name, node );

            if ( rwFrame.Parent != null )
                node.Parent = mLookup[GetName( rwFrame )];

            foreach ( var child in rwFrame.Children )
            {
                node.AddChildNode( ConvertFromRecursive( child ) );
            }

            return node;
        }

        protected string GetName( RwFrame rwFrame )
        {
            return rwFrame.HasHAnimExtension ? rwFrame.HAnimFrameExtensionNode.NameId.ToString() : "RootNode";
        }

        //
        // Geometry conversion
        //
        protected Geometry ConvertFrom( RwMesh rwMesh, RwGeometryNode rwGeometry )
        {
            var rwIndices = MeshUtilities.ToTriangleList( rwMesh.Indices, false );

            var geometry = new Geometry();
            geometry.TriangleIndexType = TriangleIndexType.UInt16;
            geometry.Triangles = new Triangle[rwIndices.Length / 3];

            uint y = 0;
            for ( int i = 0; i < geometry.Triangles.Length; i++ )
            {
                geometry.Triangles[i] = new Triangle( y++, y++, y++ );
            }

            geometry.MaterialName = GetName( rwGeometry.Materials[rwMesh.MaterialIndex] );
            geometry.Vertices = new Vector3[rwIndices.Length];
            geometry.Normals = new Vector3[rwIndices.Length];
            geometry.TexCoordsChannel0 = new Vector2[rwIndices.Length];

            for ( int i = 0; i < rwIndices.Length; i++ )
            {
                geometry.Vertices[i] = rwGeometry.Vertices[rwIndices[i]];
                geometry.Normals[i] = rwGeometry.Normals[rwIndices[i]];
                geometry.TexCoordsChannel0[i] = rwGeometry.TextureCoordinateChannels[0][rwIndices[i]];
            }

            geometry.BoundingBox = BoundingBox.Calculate( geometry.Vertices );
            geometry.BoundingSphere = BoundingSphere.Calculate( geometry.BoundingBox.Value, geometry.Vertices );

            return geometry;
        }

        protected void ConvertFrom( RwAtomicNode rwAtomic, RwGeometryListNode rwGeometryList, RwFrameListNode rwFrameList )
        {
            var rwGeometry = rwGeometryList[rwAtomic.GeometryIndex];
            var rwFrame = rwFrameList[rwAtomic.FrameIndex];
            var rwMeshList = ( RwMeshListNode )rwGeometry.ExtensionNodes.Find( x => x.Id == RwNodeId.RwMeshListNode );
            var node = mLookup[GetName( rwFrame )];

            foreach ( var rwMesh in rwMeshList.MaterialMeshes )
            {
                node.Attachments.Add( new NodeGeometryAttachment( ConvertFrom( rwMesh, rwGeometry ) ) );
            }
        }
    }

    public class RmdSceneFieldModelConverter : RmdSceneModelConverterBase
    {
        public override Model ConvertFrom( RmdScene source )
        {
            // Fields use 2 seperate clumps for the model and the collision
            var rwCollision = source.Clumps[0];
            var rwModel = source.Clumps[1];

            // Convert textures, materials and the level model
            var model = new Model( 0x01105070 );
            model.TextureDictionary = ConvertFrom( source.TextureDictionary );
            model.MaterialDictionary = ConvertFrom( rwModel.GeometryList.SelectMany( x => x.Materials ) );
            model.Scene = ConvertFrom( rwModel );

            // Convert collision
            var rwCollisionAtomic = rwCollision.Atomics[0];
            var rwCollisionGeometry = rwCollision.GeometryList[rwCollisionAtomic.GeometryIndex];
            var rwCollisionFrame = rwCollision.FrameList[rwCollisionAtomic.FrameIndex];

            var collisionNode = ConvertFrom( rwCollisionFrame );
            collisionNode.Name = "atari";
            collisionNode.Properties["fldCollisObj"] = new NodeBoolProperty( "fldCollisObj", true );

            foreach ( var rwMesh in ((RwMeshListNode)rwCollisionGeometry.ExtensionNodes.Find( x => x.Id == RwNodeId.RwMeshListNode)).MaterialMeshes)
            {
                collisionNode.Attachments.Add( new NodeGeometryAttachment( ConvertFrom( rwMesh, rwCollisionGeometry ) ) );
            }

            model.Scene.RootNode.AddChildNode( collisionNode );

            return model;
        }
    }
}
