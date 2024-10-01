#pragma once

using namespace System;
using namespace System::Collections::Generic;
using namespace System::Numerics;

namespace GFDLibrary::Conversion::FbxSdk
{
	using namespace Models;
	using namespace Textures;
	using namespace Materials;

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
		FbxNode* ConvertToFbxNode(FbxScene* fbxScene, FbxPose* fbxBindPose, Node^ node);

		inline FbxDouble3 ConvertToFbxDouble3(Vector3 value)
		{
			return FbxDouble3(value.X, value.Y, value.Z);
		}
		inline FbxQuaternion ConvertToFbxQuaternion(Quaternion value)
		{
			return FbxQuaternion(value.X, value.Y, value.Z, value.W);
		}

		FbxManager* mManager;
		Dictionary<Node^, IntPtr>^ mNodeToFbxNodeLookup;
		FbxSdkModelPackExporterConfig^ mConfig;
		String^ mOutDir;
	};
}
