#pragma once

using namespace System;
using namespace System::Collections::Generic;
using namespace System::Numerics;

namespace GFDLibrary::Conversion::FbxSdk
{
	using namespace Common;
	using namespace Models;
	using namespace Textures;
	using namespace Materials;
	using namespace Cameras;
	using namespace Lights;
	using namespace Effects;

	public ref class FbxSdkModelPackExporterConfig
	{
	public:
		inline FbxSdkModelPackExporterConfig()
		{
		}
	};

	public ref class FbxSdkModelPackExporterException : public Exception
	{
	public:
		inline FbxSdkModelPackExporterException(String^ message) : Exception(message) {}
	};

	public ref class FbxSdkModelPackExporter sealed
	{
	public:
		FbxSdkModelPackExporter();
		~FbxSdkModelPackExporter();

		static void ExportFile(ModelPack^ model, String^ path, FbxSdkModelPackExporterConfig^ config);
		void Export(ModelPack^ model, String^ path, FbxSdkModelPackExporterConfig^ config);

	private:
		void Reset();
		void ExportFbxScene(FbxScene* scene, String^ path);
		FbxScene* ConvertToFbxScene(ModelPack^ modelPack);
		void ConvertNode(FbxNode* fbxParentNode, Node^ parentNode);
		void ConvertModel(FbxNode* fbxParentNode,Node^ parentNode, Model^ model, int typeIndex);
		void ConvertNode(FbxNode* fbxParentNode, Node^ parentNode, Node^ node, int typeIndex);
		void ConvertMesh(FbxNode* fbxParentNode, Node^ parentNode, Mesh^ mesh, int typeIndex);
		void ConvertCamera(FbxNode* fbxParentNode, Node^ parentNode, Camera^ mesh, int typeIndex);
		void ConvertLight(FbxNode* fbxParentNode,Node^ parentNode, Light^ mesh, int typeIndex);
		void ConvertEpl(FbxNode* fbxParentNode, Node^ parentNode, Epl^ mesh, int typeIndex);
		void ConvertEplLeaf(FbxNode* fbxParentNode, Node^ parentNode, EplLeaf^ mesh, int typeIndex);
		void ConvertMorph(FbxNode* fbxParentNode, Node^ parentNode, Morph^ mesh, int typeIndex);

		FbxManager* mFbxManager;
		FbxScene* mFbxScene;
		FbxPose* mFbxBindPose;

		ModelPack^ mModelPack;
		Model^ mModel;
		List<Node^>^ mModelNodes;

		Dictionary<Node^, IntPtr>^ mNodeToFbxNodeLookup;
		Dictionary<int, IntPtr>^ mNodeIndexToFbxNodeLookup;
		Dictionary<int, IntPtr>^ mNodeIndexToFbxClusterLookup;
		Dictionary<String^, IntPtr>^ mTextureNameToFbxFileTexture;
		Dictionary<String^, IntPtr>^ mMaterialNameToFbxSurfaceMaterial;

		FbxSdkModelPackExporterConfig^ mConfig;
		String^ mPath;
		String^ mFileName;
		String^ mBaseDirectoryPath;
		String^ mTextureBaseDirectoryPath;
		String^ mTextureBaseRelativeDirectoryPath;
	};
}
