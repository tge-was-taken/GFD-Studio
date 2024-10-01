#include "pch.h"

#include "FbxSdkModelPackExporter.h"
#include "Utf8String.h"

/*
	FBX MODEL EXPORTER NOTES:
	- 3DS Max is very picky about the ordering of layers & layer elements in meshes.
	It assumes the following order:
	Layer 0: (maps to Map Channel 1)
		- FbxGeometryElementNormal
		- FbxGeometryElementMaterial (IndexToDirect, 1 index with value 0)
		- FbxGeometryElementColor (first color channel, if used)
		- FbxGeometryElementUV (first UV channel, if used)
	Layer 1: (maps to Map Channel 2)
		- FbxGeometryElementUV (second UV channel, if used)
	LAYER 2: (Maps to Map Channel 3)
		- FbxGeometryElementUV (second COLOR (!!!!) channel, if used)

	- EvaluateGlobalTransform() is broken. DONT USE IT
	It occasionally returns the wrong matrix, this is a known issue (just google it)
*/

// TODO: 
// - cache converted node world transform
// - export animations
// - fix vertex colors in max
// - add unique names to root bones to prevent name clashes?

using namespace System;
using namespace System::Collections::Generic;
using namespace System::Numerics;

namespace GFDLibrary::Conversion::FbxSdk
{
	using namespace Models;
	using namespace Textures;
	using namespace Materials;

	const double RAD_TO_DEG = 180.0 / Math::PI;

	FbxSdkModelPackExporter::FbxSdkModelPackExporter()
	{
		// Create manager
		mManager = FbxManager::Create();
		if (!mManager)
			gcnew Exception("Failed to create FBX Manager");

		mNodeToFbxNodeLookup = gcnew Dictionary<Node^, IntPtr>();
	}

	FbxSdkModelPackExporter::~FbxSdkModelPackExporter()
	{
		// Destroy manager
		mManager->Destroy();
	}

	void FbxSdkModelPackExporter::Reset()
	{
		mNodeToFbxNodeLookup->Clear();
	}

	FbxNode* FbxSdkModelPackExporter::ConvertToFbxNode(FbxScene* fbxScene, FbxPose* fbxBindPose, Node^ node)
	{
		IntPtr fbxNodePtr;
		if (mNodeToFbxNodeLookup->TryGetValue(node, fbxNodePtr))
			return (FbxNode*)fbxNodePtr.ToPointer();

		// Create node
		auto fbxNode = FbxNode::Create(fbxScene, Utf8String(node->Name).ToCStr());
		mNodeToFbxNodeLookup->Add(node, (IntPtr)fbxNode);

		// Setup transform
		fbxNode->LclRotation.Set(ConvertToFbxQuaternion(node->Rotation).DecomposeSphericalXYZ());
		fbxNode->LclScaling.Set(ConvertToFbxDouble3(node->Scale));
		fbxNode->LclTranslation.Set(ConvertToFbxDouble3(node->Translation));
		fbxNode->SetPreferedAngle(fbxNode->LclRotation.Get());

		// Set parent
		FbxNode* fbxParentNode;
		if (node->Parent != nullptr)
			fbxParentNode = ConvertToFbxNode(fbxScene, fbxBindPose, node->Parent);
		else
			fbxParentNode = fbxScene->GetRootNode();

		fbxParentNode->AddChild(fbxNode);

		// Create Skeleton node attribute for this node
		auto fbxSkeleton = FbxSkeleton::Create(fbxScene, "");

		// 3ds Max always sets the skeleton type to limb node
		fbxSkeleton->SetSkeletonType(FbxSkeleton::EType::eLimbNode);
		fbxNode->SetNodeAttribute(fbxSkeleton);

		// Add to bind pose
		fbxBindPose->Add(fbxNode, fbxNode->EvaluateGlobalTransform());
	}

	FbxScene* FbxSdkModelPackExporter::ConvertToFbxScene(ModelPack^ modelPack)
	{
		// Create FBX scene
		auto fbxScene = FbxScene::Create(mManager, "");
		if (!fbxScene)
			gcnew Exception("Failed to create FBX scene");

		auto& fbxGlobalSettings = fbxScene->GetGlobalSettings();
		fbxGlobalSettings.SetAxisSystem(FbxAxisSystem::DirectX);
		fbxGlobalSettings.SetSystemUnit(FbxSystemUnit::m);

		// 3ds Max a bind pose. The name is taken from it as well.
		auto fbxBindPose = FbxPose::Create(fbxScene, "BIND_POSES");
		fbxBindPose->SetIsBindPose(true);
		fbxScene->AddPose(fbxBindPose);

		auto fRootNode = ConvertToFbxNode(fbxScene, fbxBindPose, modelPack->Model->RootNode);
		fbxBindPose->Add(fRootNode, fRootNode->EvaluateGlobalTransform());

		return fbxScene;
	}

	void FbxSdkModelPackExporter::ExportFbxScene(FbxScene* fbxScene, String^ path)
	{
		// Create exporter
		auto fbxExporter = FbxExporter::Create(mManager, "");
		if (!fbxExporter->SetFileExportVersion(FBX_2014_00_COMPATIBLE))
			gcnew Exception("Failed to set FBX export version");

		// Initialize exporter
		//auto fileFormat = mManager->GetIOPluginRegistry()->GetNativeWriterFormat();
		if (!fbxExporter->Initialize(Utf8String(path).ToCStr(), -1, mManager->GetIOSettings()))
		{
			auto errorMsg = gcnew String(fbxExporter->GetStatus().GetErrorString());
			gcnew Exception("Failed to initialize FBX exporter: " + errorMsg);
		}

		// Export scene
		fbxExporter->Export(fbxScene);

		// Destroy exporter
		fbxExporter->Destroy();
	}

	void FbxSdkModelPackExporter::ExportFile(ModelPack^ modelPack, String^ path, FbxSdkModelPackExporterConfig^ config)
	{
		auto exp = gcnew FbxSdkModelPackExporter();
		exp->Export(modelPack, path, config);
	}

	void FbxSdkModelPackExporter::Export(ModelPack^ modelPack, String^ path, FbxSdkModelPackExporterConfig^ config)
	{
		Reset();

		mConfig = config;
		mOutDir = System::IO::Path::GetDirectoryName(path);

		// Create IO settings
		auto fIos = FbxIOSettings::Create(mManager, IOSROOT);
		fIos->SetBoolProp(EXP_FBX_MATERIAL, true);
		fIos->SetBoolProp(EXP_FBX_TEXTURE, true);
		fIos->SetBoolProp(EXP_FBX_EMBEDDED, false);
		fIos->SetBoolProp(EXP_FBX_SHAPE, true);
		fIos->SetBoolProp(EXP_FBX_GOBO, true);
		fIos->SetBoolProp(EXP_FBX_ANIMATION, true);
		fIos->SetBoolProp(EXP_FBX_GLOBAL_SETTINGS, true);
		mManager->SetIOSettings(fIos);

		// Create scene
		auto fbxScene = ConvertToFbxScene(modelPack);
		ExportFbxScene(fbxScene, path);
	}
}