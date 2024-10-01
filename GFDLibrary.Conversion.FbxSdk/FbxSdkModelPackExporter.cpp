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
using namespace System::Linq;

namespace GFDLibrary::Conversion::FbxSdk
{
	using namespace Common;
	using namespace Models;
	using namespace Textures;
	using namespace Materials;
	using namespace Cameras;
	using namespace Lights;
	using namespace Effects;

	const double RAD_TO_DEG = 180.0 / Math::PI;

	FbxSdkModelPackExporter::FbxSdkModelPackExporter()
	{
		// Create manager
		mFbxManager = FbxManager::Create();
		if (!mFbxManager)
			throw gcnew Exception("Failed to create FBX Manager");

		mNodeToFbxNodeLookup = gcnew Dictionary<Node^, IntPtr>();
		mNodeIndexToFbxNodeLookup = gcnew Dictionary<int, IntPtr>();
		mNodeIndexToFbxClusterLookup = gcnew Dictionary<int, IntPtr>();
		mTextureNameToFbxFileTexture = gcnew Dictionary<String^, IntPtr>();
		mMaterialNameToFbxSurfaceMaterial = gcnew Dictionary<String^, IntPtr>();
	}

	FbxSdkModelPackExporter::~FbxSdkModelPackExporter()
	{
		// Destroy manager
		mFbxManager->Destroy();
	}

	void FbxSdkModelPackExporter::Reset()
	{
		mNodeToFbxNodeLookup->Clear();
		mNodeIndexToFbxNodeLookup->Clear();
		mNodeIndexToFbxClusterLookup->Clear();
		mTextureNameToFbxFileTexture->Clear();
		mMaterialNameToFbxSurfaceMaterial->Clear();
	}

	// Type conversions
	static FbxDouble3 ConvertToFbxDouble3(Vector3 value)
	{
		return FbxDouble3(value.X, value.Y, value.Z);
	}

	static FbxQuaternion ConvertToFbxQuaternion(Quaternion value)
	{
		return FbxQuaternion(value.X, value.Y, value.Z, value.W);
	}

	static FbxVector4 ConvertToFbxVector4(Vector3 value)
	{
		return FbxVector4(value.X, value.Y, value.Z);
	}


	static void ConvertUserPropertiesToUDP3DSMAXProperty(FbxNode* fbxNode, Node^ node)
	{
		System::Text::StringBuilder^ udpBuilder = gcnew System::Text::StringBuilder();

		for each (auto prop in node->Properties)
		{
			switch (prop.Value->ValueType)
			{
			case UserPropertyValueType::Int:
			{
				auto intProperty = safe_cast<UserIntProperty^>(prop.Value);
				udpBuilder->AppendFormat("{0}=int({1})&cr;&lf;", prop.Key, intProperty->Value);
				break;
			}
			case UserPropertyValueType::Float:
			{
				auto floatProperty = safe_cast<UserFloatProperty^>(prop.Value);
				udpBuilder->AppendFormat("{0}=float({1})&cr;&lf;", prop.Key, floatProperty->Value);
				break;
			}
			case UserPropertyValueType::Bool:
			{
				auto boolProperty = safe_cast<UserBoolProperty^>(prop.Value);
				udpBuilder->AppendFormat("{0}=bool({1})&cr;&lf;", prop.Key, boolProperty->Value ? "true" : "false");
				break;
			}
			case UserPropertyValueType::String:
			{
				auto stringProperty = safe_cast<UserStringProperty^>(prop.Value);
				udpBuilder->AppendFormat("{0}=string({1})&cr;&lf;", prop.Key, stringProperty->Value);
				break;
			}
			case UserPropertyValueType::ByteVector3:
			{
				auto byteVector3Property = safe_cast<UserByteVector3Property^>(prop.Value);
				udpBuilder->AppendFormat("{0}=bytevector3({1},{2},{3})&cr;&lf;",
					prop.Key,
					byteVector3Property->Value.X,
					byteVector3Property->Value.Y,
					byteVector3Property->Value.Z);
				break;
			}
			case UserPropertyValueType::ByteVector4:
			{
				auto byteVector4Property = safe_cast<UserByteVector4Property^>(prop.Value);
				udpBuilder->AppendFormat("{0}=bytevector4({1},{2},{3},{4})&cr;&lf;",
					prop.Key,
					byteVector4Property->Value.X,
					byteVector4Property->Value.Y,
					byteVector4Property->Value.Z,
					byteVector4Property->Value.W);
				break;
			}
			case UserPropertyValueType::Vector3:
			{
				auto vector3Property = safe_cast<UserVector3Property^>(prop.Value);
				udpBuilder->AppendFormat("{0}=vector3({1},{2},{3})&cr;&lf;",
					prop.Key,
					vector3Property->Value.X,
					vector3Property->Value.Y,
					vector3Property->Value.Z);
				break;
			}
			case UserPropertyValueType::Vector4:
			{
				auto vector4Property = safe_cast<UserVector4Property^>(prop.Value);
				udpBuilder->AppendFormat("{0}=vector4({1},{2},{3},{4})&cr;&lf;",
					prop.Key,
					vector4Property->Value.X,
					vector4Property->Value.Y,
					vector4Property->Value.Z,
					vector4Property->Value.W);
				break;
			}
			case UserPropertyValueType::ByteArray:
			{
				auto byteArrayProperty = safe_cast<UserByteArrayProperty^>(prop.Value);
				System::String^ base64String = System::Convert::ToBase64String(byteArrayProperty->Value);
				udpBuilder->AppendFormat("{0}=bytearray({1})&cr;&lf;", prop.Key, base64String);
				break;
			}
			default:
				throw gcnew System::NotSupportedException("Unsupported user property type");
			}
		}

		// Create the UDP3DSMAX property
		auto udpProperty = FbxProperty::Create(fbxNode, FbxStringDT, "UDP3DSMAX");
		udpProperty.ModifyFlag(FbxPropertyFlags::eUserDefined, true);
		if (!udpProperty.Set(FbxString(Utf8String(udpBuilder->ToString()).ToCStr())))
		{
			throw gcnew FbxSdkModelPackExporterException(String::Format("Failed to set UDP3DSMAX property {0} on node {1}",
				udpBuilder->ToString(),
				node->Name));
		}
	}

	static void ConvertUserPropertiesToFbxProperties(GFDLibrary::Models::Node^ node, fbxsdk::FbxNode* fbxNode)
	{
		for each (auto prop in node->Properties)
		{
			switch (prop.Value->ValueType)
			{
			case UserPropertyValueType::Int:
			{
				auto intProperty = safe_cast<UserIntProperty^>(prop.Value);
				auto fbxProperty = FbxProperty::Create(fbxNode, FbxIntDT, Utf8String(prop.Key).ToCStr());
				fbxProperty.Set(intProperty->Value);
				break;
			}
			case UserPropertyValueType::Float:
			{
				auto floatProperty = safe_cast<UserFloatProperty^>(prop.Value);
				auto fbxProperty = FbxProperty::Create(fbxNode, FbxDoubleDT, Utf8String(prop.Key).ToCStr());
				fbxProperty.Set(floatProperty->Value);
				break;
			}
			case UserPropertyValueType::Bool:
			{
				auto boolProperty = safe_cast<UserBoolProperty^>(prop.Value);
				auto fbxProperty = FbxProperty::Create(fbxNode, FbxBoolDT, Utf8String(prop.Key).ToCStr());
				fbxProperty.Set(boolProperty->Value);
				break;
			}
			case UserPropertyValueType::String:
			{
				auto stringProperty = safe_cast<UserStringProperty^>(prop.Value);
				auto fbxProperty = FbxProperty::Create(fbxNode, FbxStringDT, Utf8String(prop.Key).ToCStr());
				fbxProperty.Set(Utf8String(stringProperty->Value).ToCStr());
				break;
			}
			// TODO does not exist
			//case UserPropertyValueType::ByteVector3:
			//{
			//	auto byteVector3Property = safe_cast<UserByteVector3Property^>(prop.Value);
			//	auto fbxProperty = FbxProperty::Create(fbxNode, FbxInt3DT, Utf8String(prop.Key).ToCStr());
			//	fbxProperty.Set(FbxInt3(byteVector3Property->Value.X, byteVector3Property->Value.Y, byteVector3Property->Value.Z));
			//	break;
			//}
			// TODO does not exist
			//case UserPropertyValueType::ByteVector4:
			//{
			//	auto byteVector4Property = safe_cast<UserByteVector4Property^>(prop.Value);
			//	auto fbxProperty = FbxProperty::Create(fbxNode, FbxInt4DT, Utf8String(prop.Key).ToCStr());
			//	fbxProperty.Set(FbxInt4(byteVector4Property->Value.X, byteVector4Property->Value.Y, byteVector4Property->Value.Z, byteVector4Property->Value.W));
			//	break;
			//}
			case UserPropertyValueType::Vector3:
			{
				auto vector3Property = safe_cast<UserVector3Property^>(prop.Value);
				auto fbxProperty = FbxProperty::Create(fbxNode, FbxDouble3DT, Utf8String(prop.Key).ToCStr());
				fbxProperty.Set(FbxDouble3(vector3Property->Value.X, vector3Property->Value.Y, vector3Property->Value.Z));
				break;
			}
			case UserPropertyValueType::Vector4:
			{
				auto vector4Property = safe_cast<UserVector4Property^>(prop.Value);
				auto fbxProperty = FbxProperty::Create(fbxNode, FbxDouble4DT, Utf8String(prop.Key).ToCStr());
				fbxProperty.Set(FbxDouble4(vector4Property->Value.X, vector4Property->Value.Y, vector4Property->Value.Z, vector4Property->Value.W));
				break;
			}
			// TODO does not exist
			//case UserPropertyValueType::ByteArray:
			//{
			//	auto byteArrayProperty = safe_cast<UserByteArrayProperty^>(prop.Value);
			//	auto fbxProperty = FbxProperty::Create(fbxNode, FbxStringDT, Utf8String(prop.Key).ToCStr());
			//	// Convert byte array to Base64 string
			//	System::String^ base64String = System::Convert::ToBase64String(byteArrayProperty->Value);
			//	fbxProperty.Set(Utf8String(base64String).ToCStr());
			//	break;
			//}
			default:
				// Handle unknown property type or throw an exception
				throw gcnew System::NotSupportedException("Unsupported user property type");
			}
		}
	}

	// Generic FBX helpers

	static FbxLayer* GetFbxMeshLayer(fbxsdk::FbxMesh* fMesh, int layer)
	{
		auto fLayer = fMesh->GetLayer(layer);
		if (fLayer == nullptr)
		{
			while (fMesh->CreateLayer() != layer);
			fLayer = fMesh->GetLayer(layer);
		}

		return fLayer;
	}

	static FbxGeometryElementNormal* CreateFbxMeshElementNormal(fbxsdk::FbxGeometryBase* fMesh)
	{
		auto fElementNormal = fMesh->CreateElementNormal();
		fElementNormal->SetMappingMode(FbxLayerElement::EMappingMode::eByControlPoint);
		fElementNormal->SetReferenceMode(FbxLayerElement::EReferenceMode::eDirect);
		fElementNormal->GetDirectArray().SetCount(fMesh->GetControlPointsCount());
		return fElementNormal;
	}

	static FbxGeometryElementMaterial* CreateFbxElementMaterial(fbxsdk::FbxMesh* fMesh)
	{
		// Technically not necessary to add, however 3ds Max adds this by default
		// so we do so as well to maintain compat
		auto fElementMaterial = fMesh->CreateElementMaterial();
		fElementMaterial->SetMappingMode(FbxLayerElement::EMappingMode::eByPolygon);
		fElementMaterial->SetReferenceMode(FbxLayerElement::EReferenceMode::eIndexToDirect);
		return fElementMaterial;
	}

	static FbxGeometryElementUV* CreateFbxMeshElementUV(fbxsdk::FbxMesh* fMesh, const char* name, int layer)
	{
		// 3ds Max requires every UV channel to be on its own layer (as these map to Map Channels)
		auto fLayer = GetFbxMeshLayer(fMesh, layer);
		auto fElementUV = (FbxGeometryElementUV*)fLayer->CreateLayerElementOfType(FbxLayerElement::EType::eUV);
		fElementUV->SetName(name);
		fElementUV->SetMappingMode(FbxLayerElement::EMappingMode::eByControlPoint);
		fElementUV->SetReferenceMode(FbxLayerElement::EReferenceMode::eDirect);
		fElementUV->GetDirectArray().SetCount(fMesh->GetControlPointsCount());
		return fElementUV;
	}

	static void ConvertVertexColorChannel(FbxMesh* fbxMesh, array<unsigned int>^ colors)
	{
		auto fbxElementColors = fbxMesh->CreateElementVertexColor();
		fbxElementColors->SetMappingMode(FbxLayerElement::EMappingMode::eByControlPoint);
		fbxElementColors->SetReferenceMode(FbxLayerElement::EReferenceMode::eDirect);
		fbxElementColors->GetDirectArray().SetCount(fbxMesh->GetControlPointsCount());
		for (int i = 0; i < fbxElementColors->GetDirectArray().GetCount(); i++)
			fbxElementColors->GetDirectArray().SetAt(i, FbxColor(1, 1, 1, 1));

		for (int i = 0; i < colors->Length; i++)
		{
			auto r = (colors[i] & 0xFF000000)>>24;
			auto g = (colors[i] & 0x00FF0000)>>16;
			auto b = (colors[i] & 0x0000FF00)>>8;
			auto a = (colors[i] & 0x000000FF);
			fbxElementColors->GetDirectArray().SetAt(i,
				FbxColor((double)r / 255.0, (double)g / 255.0,
					(double)b / 255.0, (double)a / 255.0));
		}
	}

	//static void ConvertVertexColorChannelASUVLayer(FbxMesh* fbxMesh, array<unsigned int>^ colors, const char* name, int layer)
	//{
	//	// 3ds Max requires every extra color channel to be a UV channel on its own layer (as these map to Map Channels)
	//	// TODO UDP3DSMAX = "MapChannel:3 = ColorChannel_3&cr;&lf;MapChannel:4 = ColorChannel_4&cr;&lf;"
	//	auto fLayer = GetFbxMeshLayer(fbxMesh, layer);
	//	auto fElementUV = (FbxGeometryElementUV*)fLayer->CreateLayerElementOfType(FbxLayerElement::EType::eUV);
	//	fElementUV->SetName(name);
	//	fElementUV->SetMappingMode(FbxLayerElement::EMappingMode::eByControlPoint);
	//	fElementUV->SetReferenceMode(FbxLayerElement::EReferenceMode::eDirect);
	//	fElementUV->GetDirectArray().SetCount(fbxMesh->GetControlPointsCount());

	//	for (int i = 0; i < colors->Length; i++)
	//	{
	//		auto r = (colors[i] & 0xFF000000) >> 24;
	//		auto g = (colors[i] & 0x00FF0000) >> 16;
	//		auto b = (colors[i] & 0x0000FF00) >> 8;
	//		auto a = (colors[i] & 0x000000FF);
	//		fElementUV->GetDirectArray().SetAt(i,
	//			FbxColor((double)r / 255.0, (double)g / 255.0,
	//				(double)b / 255.0, (double)a / 255.0));
	//	}
	//}

	static void ConvertTexCoordChannel(FbxMesh* fbxMesh, array<Vector2>^ texCoords, const char* name, int layer)
	{
		// 3ds Max requires every UV channel to be on its own layer (as these map to Map Channels)
		auto fbxElementUV = layer >= 0 ?
			(FbxGeometryElementUV*)GetFbxMeshLayer(fbxMesh, layer)->CreateLayerElementOfType(FbxLayerElement::EType::eUV) :
			fbxMesh->CreateElementUV(name);
		if (name)
			fbxElementUV->SetName(name);
		fbxElementUV->SetName(name);
		fbxElementUV->SetMappingMode(FbxLayerElement::EMappingMode::eByControlPoint);
		fbxElementUV->SetReferenceMode(FbxLayerElement::EReferenceMode::eDirect);
		fbxElementUV->GetDirectArray().SetCount(fbxMesh->GetControlPointsCount());

		for (int i = 0; i < texCoords->Length; i++)
			fbxElementUV->GetDirectArray().SetAt(i, FbxVector2(texCoords[i].X, 1.0f - texCoords[i].Y));
	}

	// Main conversion functions

	void FbxSdkModelPackExporter::ConvertModel(FbxNode* fbxParentNode,
		Node^ parentNode,
		Model^ model, int typeIndex)
	{
		throw gcnew FbxSdkModelPackExporterException("Converting model attachment not implemented.");
	}

	void FbxSdkModelPackExporter::ConvertNode(FbxNode* fbxParentNode,
		Node^ parentNode,
		Node^ node, int typeIndex)
	{
		throw gcnew FbxSdkModelPackExporterException("Converting node attachment not implemented.");
	}

	static FbxAMatrix ConvertToFbxAMatrix(Matrix4x4& m)
	{
		typedef float Matrix4x4Data[4][4];

		FbxAMatrix fm;
		for (int y = 0; y < 4; y++)
		{
			for (int x = 0; x < 4; x++)
				fm[y][x] = (*(Matrix4x4Data*)&m)[y][x];
		}

		return fm;
	}

	void FbxSdkModelPackExporter::ConvertMesh(FbxNode* fbxParentNode,
		Node^ parentNode,
		Mesh^ mesh, int typeIndex)
	{
		/*
			3DS Max is very picky about the ordering of layers & layer elements in meshes.
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
		*/

		mNodeIndexToFbxClusterLookup->Clear();

		auto fbxMeshNode = FbxNode::Create(mFbxScene, Utf8String(String::Format("{0}_mesh_{1}", parentNode->Name, typeIndex.ToString())).ToCStr());
		if (mModel->Bones != nullptr && mModel->Bones->Count > 0)
		{
			// Parenting meshes to nodes on animated models leads to weird results
			mFbxScene->GetRootNode()->AddChild(fbxMeshNode);
		}
		else
		{
			fbxParentNode->AddChild(fbxMeshNode);
		}

		// 3ds Max requires every node including nodes not used for skeletal animation to be in the bind pose, otherwise it is ignored entirely.
		mFbxBindPose->Add(fbxMeshNode, fbxMeshNode->EvaluateGlobalTransform());

		auto fbxMesh = FbxMesh::Create(fbxMeshNode, "");
		fbxMeshNode->SetNodeAttribute(fbxMesh);

		auto vertices = mesh->Vertices;
		auto normals = mesh->Normals;

		// Always transform the mesh, even when it doesn't have weights, to ensure the parent node matrix is applied.
		auto temp = mesh->Transform(parentNode, mModelNodes, mModel->Bones, true, true, Nullable<Matrix4x4>());
		vertices = temp.Item1;
		normals = temp.Item2;
		
		fbxMesh->InitControlPoints(vertices->Length);
		for (int i = 0; i < fbxMesh->GetControlPointsCount(); ++i)
			fbxMesh->SetControlPointAt(ConvertToFbxVector4(vertices[i]), i);

		if (normals)
		{
			auto fbxElementNormal = fbxMesh->CreateElementNormal();
			fbxElementNormal->SetMappingMode(FbxLayerElement::EMappingMode::eByControlPoint);
			fbxElementNormal->SetReferenceMode(FbxLayerElement::EReferenceMode::eDirect);
			fbxElementNormal->GetDirectArray().SetCount(fbxMesh->GetControlPointsCount());
			for (int i = 0; i < fbxElementNormal->GetDirectArray().GetCount(); ++i)
				fbxElementNormal->GetDirectArray().SetAt(i, ConvertToFbxVector4(normals[i]));
		}

		// Technically not necessary to add, however 3ds Max adds this by default
		// so we do so as well to maintain compat
		auto fbxElementMaterial = fbxMesh->CreateElementMaterial();
		fbxElementMaterial->SetMappingMode(FbxLayerElement::EMappingMode::eByPolygon);
		fbxElementMaterial->SetReferenceMode(FbxLayerElement::EReferenceMode::eIndexToDirect);

		if (mesh->ColorChannel0)
			ConvertVertexColorChannel(fbxMesh, mesh->ColorChannel0);

		if (mesh->ColorChannel1)
			ConvertVertexColorChannel(fbxMesh, mesh->ColorChannel1);

		if (mesh->TexCoordsChannel0)
			ConvertTexCoordChannel(fbxMesh, mesh->TexCoordsChannel0, "UVChannel_1", 0);

		if (mesh->TexCoordsChannel1)
			ConvertTexCoordChannel(fbxMesh, mesh->TexCoordsChannel1, "UVChannel_2", 1);

		if (mesh->TexCoordsChannel2)
			ConvertTexCoordChannel(fbxMesh, mesh->TexCoordsChannel2, "UVChannel_3", 2);

		auto fbxSkin = FbxSkin::Create(fbxMesh, "");
		fbxSkin->SetSkinningType(FbxSkin::EType::eLinear);
		fbxMesh->AddDeformer(fbxSkin);

		if (mesh->VertexWeights)
		{
			for (int i = 0; i < mesh->VertexWeights->Length; ++i)
			{
				auto vWeights = mesh->VertexWeights[i];
				for (int j = 0; j < vWeights.Weights->Length; ++j)
				{
					auto weight = vWeights.Weights[j];
					if (weight < 0.001) continue;

					auto boneIndex = vWeights.Indices[j];
					auto bone = mModel->Bones[boneIndex];
					auto nodeIndex = bone->NodeIndex;

					IntPtr fClusterPtr;
					FbxCluster* fbxCluster;
					if (!mNodeIndexToFbxClusterLookup->TryGetValue(nodeIndex, fClusterPtr))
					{
						auto fbxNode = (FbxNode*)mNodeIndexToFbxNodeLookup[nodeIndex].ToPointer();
						fbxCluster = FbxCluster::Create(mFbxScene, "");
						fbxCluster->SetLink(fbxNode);
						fbxCluster->SetLinkMode(FbxCluster::ELinkMode::eNormalize);

						// NOTE: DO NOT USE 'EvaluateGlobalTransform', IT IS BROKEN
						// AND DOES NOT ALWAYS RETURN THE CORRECT MATRIX!!!!
						auto worldTfm = mModelNodes[nodeIndex]->WorldTransform;
						fbxCluster->SetTransformLinkMatrix(ConvertToFbxAMatrix(worldTfm));
						fbxSkin->AddCluster(fbxCluster);
						mNodeIndexToFbxClusterLookup[nodeIndex] = (IntPtr)fbxCluster;
					}
					else
					{
						fbxCluster = (FbxCluster*)fClusterPtr.ToPointer();
					}

					fbxCluster->AddControlPointIndex(i, weight);
				}
			}
		}
		else if (mModel->Bones != nullptr && mModel->Bones->Count > 0)
		{
			// If this is an animated model, the mesh will be parented to the root in the scene
			// however it should still follow the transform of its actual parent
			// hence we add a skin here that binds all vertices to the parent.
			auto parentNodeIndex = mModelNodes->IndexOf(parentNode);
			IntPtr fClusterPtr;
			FbxCluster* fbxCluster;
			if (!mNodeIndexToFbxClusterLookup->TryGetValue(parentNodeIndex, fClusterPtr))
			{
				auto fbxNode = (FbxNode*)mNodeIndexToFbxNodeLookup[parentNodeIndex].ToPointer();
				fbxCluster = FbxCluster::Create(mFbxScene, "");
				fbxCluster->SetLink(fbxNode);
				fbxCluster->SetLinkMode(FbxCluster::ELinkMode::eNormalize);

				// NOTE: DO NOT USE 'EvaluateGlobalTransform', IT IS BROKEN
				// AND DOES NOT ALWAYS RETURN THE CORRECT MATRIX!!!!
				auto worldTfm = parentNode->WorldTransform;
				fbxCluster->SetTransformLinkMatrix(ConvertToFbxAMatrix(worldTfm));
				fbxSkin->AddCluster(fbxCluster);
				mNodeIndexToFbxClusterLookup[parentNodeIndex] = (IntPtr)fbxCluster;
			}
			else
			{
				fbxCluster = (FbxCluster*)fClusterPtr.ToPointer();
			}
			for (int i = 0; i < fbxMesh->GetControlPointsCount(); i++)
				fbxCluster->AddControlPointIndex(i, 1);
		}

		auto fbxMaterial = (FbxSurfaceMaterial*)mMaterialNameToFbxSurfaceMaterial[mesh->MaterialName].ToPointer();
		auto materialIndex = fbxMeshNode->AddMaterial(fbxMaterial);
		if (materialIndex < 0)
			throw gcnew FbxSdkModelPackExporterException(String::Format("Failed to add material '{0}' to mesh node '{1}'", mesh->MaterialName, parentNode->Name));

		for (int i = 0; i < mesh->Triangles->Length; ++i)
		{
			fbxMesh->BeginPolygon(materialIndex, -1, -1, false);
			fbxMesh->AddPolygon(mesh->Triangles[i].A);
			fbxMesh->AddPolygon(mesh->Triangles[i].B);
			fbxMesh->AddPolygon(mesh->Triangles[i].C);
			fbxMesh->EndPolygon();
		}
	}

	// TODO test & implement properly
	void FbxSdkModelPackExporter::ConvertCamera(FbxNode* fbxParentNode,
		Node^ parentNode,
		Camera^ camera, int typeIndex)
	{
		assert(typeIndex == 0);
		auto fbxCamera = FbxCamera::Create(mFbxScene, "");
		fbxParentNode->SetNodeAttribute(fbxCamera);

		// Set position
		fbxCamera->Position.Set(ConvertToFbxDouble3(camera->Position));

		// Set up vector
		fbxCamera->UpVector.Set(ConvertToFbxDouble3(camera->Up));

		// Set look-at point (target)
		Vector3 target = camera->Position + camera->Direction;
		fbxCamera->InterestPosition.Set(ConvertToFbxDouble3(target));

		// Set field of view
		fbxCamera->FieldOfView.Set(camera->FieldOfView);

		// Set aspect ratio
		fbxCamera->AspectWidth.Set(camera->AspectRatio);
		fbxCamera->AspectHeight.Set(1.0);

		// Set near and far planes
		fbxCamera->NearPlane.Set(camera->ClipPlaneNear);
		fbxCamera->FarPlane.Set(camera->ClipPlaneFar);

		// Set the camera format to custom to allow setting the aspect ratio
		fbxCamera->SetAspect(FbxCamera::eFixedResolution, camera->AspectRatio, 1.0);

		// Convert view matrix to FBX camera properties
		//FbxVector4 lEye = fbxCamera->Position;
		//FbxVector4 lCenter = ConvertToFbxVector4(target);
		//FbxVector4 lUp = fbxCamera->UpVector;
		//fbxCamera->LookAt(lEye, lCenter, lUp);
	}

	// TODO test & implement properly
	void FbxSdkModelPackExporter::ConvertLight(FbxNode* fbxParentNode,
		Node^ parentNode,
		Light^ light, int typeIndex)
	{
		auto fbxLight = FbxLight::Create(mFbxScene, "");
		fbxParentNode->SetNodeAttribute(fbxLight);

		// Set light type
		switch (light->Type)
		{
		case LightType::Point:
			fbxLight->LightType.Set(FbxLight::ePoint);
			break;
		case LightType::Spot:
			fbxLight->LightType.Set(FbxLight::eSpot);
			break;
		case LightType::Type1:
			fbxLight->LightType.Set(FbxLight::eDirectional);
			break;
		default:
			fbxLight->LightType.Set(FbxLight::ePoint);
			break;
		}

		// Set colors
		fbxLight->Color.Set(FbxDouble3(
			light->DiffuseColor.X,
			light->DiffuseColor.Y,
			light->DiffuseColor.Z
		));

		// Set intensity (assuming it's stored in the W component of DiffuseColor)
		//fbxLight->Intensity.Set(light->DiffuseColor.W * 100.0); // FBX uses percentage

		// Set attenuation
		fbxLight->EnableFarAttenuation.Set(true);
		fbxLight->FarAttenuationStart.Set(light->AttenuationStart);
		fbxLight->FarAttenuationEnd.Set(light->AttenuationEnd);

		// Set cone angle for spot lights
		if (light->Type == LightType::Spot)
		{
			fbxLight->InnerAngle.Set(light->AngleInnerCone);
			fbxLight->OuterAngle.Set(light->AngleOuterCone);
		}
	}

	void FbxSdkModelPackExporter::ConvertEpl(FbxNode* fbxParentNode,
		Node^ parentNode,
		Epl^ epl, int typeIndex)
	{
		//throw gcnew FbxSdkModelPackExporterException("Converting Epl attachment not implemented.");
	}

	void FbxSdkModelPackExporter::ConvertEplLeaf(FbxNode* fbxParentNode,
		Node^ parentNode,
		EplLeaf^ eplLeaf, int typeIndex)
	{
		//throw gcnew FbxSdkModelPackExporterException("Converting EplLeaf attachment not implemented.");
	}

	void FbxSdkModelPackExporter::ConvertMorph(FbxNode* fbxParentNode,
		Node^ parentNode,
		Morph^ morph, int typeIndex)
	{
		// TODO
		//throw gcnew FbxSdkModelPackExporterException("Converting EplLeaf attachment not implemented.");
		/*
		for ( int i = 0; i < mesh->BlendShapes->Length; i++ )
		{
			auto blendShape = mesh->BlendShapes[ i ];

			// Each shape will be stored in its own channel
			auto fChannel = FbxBlendShapeChannel::Create( work->Mesh, "" );
			work->BlendShape->AddBlendShapeChannel( fChannel );

			auto fShape = FbxShape::Create( work->Mesh, "" );
			fChannel->AddTargetShape( fShape );
			fShape->SetAbsoluteMode( true );

			// Convert vertices
			fShape->InitControlPoints( blendShape.Vertices->Length );
			ConvertPositionsToFbxControlPoints( fShape->GetControlPoints(), blendShape.Vertices );

			// Convert normals
			fShape->InitNormals( blendShape.Normals->Length );
			FbxLayerElementArrayTemplate<FbxVector4>* fNormalsArray;
			fShape->GetNormals( &fNormalsArray );
			auto fNormals = (FbxVector4*)fNormalsArray->GetLocked();
			ConvertPositionsToFbxControlPoints( fNormals, blendShape.Normals );
		}
		*/
	}

	void FbxSdkModelPackExporter::ConvertNode(FbxNode* fbxParentNode, Node^ node)
	{
		// Find node that was preallocated earlier.
		auto fbxNode = (FbxNode*)mNodeToFbxNodeLookup[node].ToPointer();

		// Setup transform
		FbxAMatrix m;
		m.SetQ(ConvertToFbxQuaternion(node->Rotation));
		fbxNode->LclRotation.Set(m.GetR());
		fbxNode->LclScaling.Set(ConvertToFbxDouble3(node->Scale));
		fbxNode->LclTranslation.Set(ConvertToFbxDouble3(node->Translation));
		fbxNode->SetPreferedAngle(fbxNode->LclRotation.Get());

		// Set parent
		fbxParentNode->AddChild(fbxNode);

		// Create Skeleton node attribute for this node
		auto fbxSkeleton = FbxSkeleton::Create(mFbxScene, "");

		// 3ds Max always sets the skeleton type to limb node
		fbxSkeleton->SetSkeletonType(FbxSkeleton::EType::eLimbNode);
		fbxNode->SetNodeAttribute(fbxSkeleton);

		// Add to bind pose
		mFbxBindPose->Add(fbxNode, fbxNode->EvaluateGlobalTransform());

		if (node->HasAttachments)
		{
			int typeIndices[(int)NodeAttachmentType::Morph + 1] = {};
			for each (auto attachment in node->Attachments)
			{
				int typeIndex = typeIndices[(int)attachment->Type]++;

				switch (attachment->Type)
				{
				case NodeAttachmentType::Model:
					ConvertModel(fbxNode, node, safe_cast<Model^>(attachment->GetValue()), typeIndex);
					break;

				case NodeAttachmentType::Node:
					ConvertNode(fbxNode, node, safe_cast<Node^>(attachment->GetValue()), typeIndex);
					break;

				case NodeAttachmentType::Mesh:
					ConvertMesh(fbxNode, node, safe_cast<Mesh^>(attachment->GetValue()), typeIndex);
					break;

				case NodeAttachmentType::Camera:
					ConvertCamera(fbxNode, node, safe_cast<Camera^>(attachment->GetValue()), typeIndex);
					break;

				case NodeAttachmentType::Light:
					ConvertLight(fbxNode, node, safe_cast<Light^>(attachment->GetValue()), typeIndex);
					break;

				case NodeAttachmentType::Epl:
					ConvertEpl(fbxNode, node, safe_cast<Epl^>(attachment->GetValue()), typeIndex);
					break;

				case NodeAttachmentType::EplLeaf:
					ConvertEplLeaf(fbxNode, node, safe_cast<EplLeaf^>(attachment->GetValue()), typeIndex);
					break;

				case NodeAttachmentType::Morph:
					ConvertMorph(fbxNode, node, safe_cast<Morph^>(attachment->GetValue()), typeIndex);
					break;
				default:
					throw gcnew FbxSdkModelPackExporterException(String::Format("Unsupported attachment type: {0}", attachment->Type.ToString()));
				}
			}
		}

		if (node->HasProperties)
		{
			//ConvertUserPropertiesToFbxProperties(node, fbxNode);

			// 3DS Max supports 'User Defined Properties' which are serialized as a string property named UDP3DSMAX. 
			ConvertUserPropertiesToUDP3DSMAXProperty(fbxNode, node);
		}

		if (node->HasChildren)
		{
			for each (auto childNode in node->Children)
				ConvertNode(fbxNode, childNode);
		}
	}

	FbxScene* FbxSdkModelPackExporter::ConvertToFbxScene(ModelPack^ modelPack)
	{
		mModelPack = modelPack;
		mModel = modelPack->Model;
		mModelNodes = gcnew List<Node^>(mModel->Nodes);

		// Create FBX scene
		mFbxScene = FbxScene::Create(mFbxManager, "");
		if (!mFbxScene)
			throw gcnew Exception("Failed to create scene");

		auto& fbxGlobalSettings = mFbxScene->GetGlobalSettings();
		fbxGlobalSettings.SetAxisSystem(FbxAxisSystem::DirectX);
		fbxGlobalSettings.SetSystemUnit(FbxSystemUnit::m);

		// Export textures
		System::IO::Directory::CreateDirectory(mTextureBaseDirectoryPath);
		for each (auto texture in modelPack->Textures)
		{
			auto texturePath = System::IO::Path::Combine(mTextureBaseDirectoryPath, texture.Key);
			System::IO::File::WriteAllBytes(texturePath, texture.Value->Data);
		}

		// Create materials
		for each (auto material in modelPack->Materials->Values)
		{
			auto fbxMaterial = FbxSurfacePhong::Create(mFbxScene, Utf8String(material->Name).ToCStr());
			fbxMaterial->ShadingModel.Set("Phong");

			if (material->DiffuseMap)
			{
				auto texturePath = System::IO::Path::Combine(mTextureBaseRelativeDirectoryPath, material->DiffuseMap->Name);

				// Create & connect file texture to material diffuse
				IntPtr fbxTexturePtr;
				FbxFileTexture* fbxTexture;
				if (!mTextureNameToFbxFileTexture->TryGetValue(texturePath, fbxTexturePtr))
				{
					fbxTexture = FbxFileTexture::Create(mFbxScene, "Bitmaptexture");
					fbxTexture->SetFileName(Utf8String(texturePath).ToCStr());

					// 3ds Max sets these by default
					fbxTexture->UVSet.Set("UVChannel_1");
					fbxTexture->UseMaterial.Set(true);

					mTextureNameToFbxFileTexture[texturePath] = (IntPtr)fbxTexture;
				}
				else
				{
					fbxTexture = (FbxFileTexture*)fbxTexturePtr.ToPointer();
				}


				fbxMaterial->Diffuse.ConnectSrcObject(fbxTexture);
			}

			mMaterialNameToFbxSurfaceMaterial[material->Name] = (IntPtr)fbxMaterial;
		}

		// 3ds Max a bind pose. The name is taken from it as well.
		mFbxBindPose = FbxPose::Create(mFbxScene, "BIND_POSES");
		if (!mFbxBindPose)
			throw gcnew FbxSdkModelPackExporterException("Failed to create bind pose");
		mFbxBindPose->SetIsBindPose(true);
		mFbxScene->AddPose(mFbxBindPose);

		// First, preallocate an FBX node for every node in order to handle
		// references to other nodes down the tree gracefully.
		for each (auto node in mModelNodes)
		{
			auto fbxNode = FbxNode::Create(mFbxScene, Utf8String(node->Name).ToCStr());
			mNodeToFbxNodeLookup->Add(node, (IntPtr)fbxNode);
			mNodeIndexToFbxNodeLookup->Add(mModelNodes->IndexOf(node), (IntPtr)fbxNode);
		}

		// Now, recursively convert all the children of the root node.
		// The root node itself is generated and is always identity, so omit that from the output.
		for each (auto node in mModel->RootNode->Children)
			ConvertNode(mFbxScene->GetRootNode(), node);

		return mFbxScene;
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
		mPath = path;
		mFileName = System::IO::Path::GetFileNameWithoutExtension(path);
		mBaseDirectoryPath = System::IO::Path::GetDirectoryName(path);
		mTextureBaseDirectoryPath = System::IO::Path::Combine(mBaseDirectoryPath, String::Format("{0}_Textures", mFileName));
		mTextureBaseRelativeDirectoryPath = mTextureBaseDirectoryPath->Substring(mBaseDirectoryPath->Length);

		// Create IO settings
		auto fIos = FbxIOSettings::Create(mFbxManager, IOSROOT);
		fIos->SetBoolProp(EXP_FBX_MATERIAL, true);
		fIos->SetBoolProp(EXP_FBX_TEXTURE, true);
		fIos->SetBoolProp(EXP_FBX_EMBEDDED, false);
		fIos->SetBoolProp(EXP_FBX_SHAPE, true);
		fIos->SetBoolProp(EXP_FBX_GOBO, true);
		fIos->SetBoolProp(EXP_FBX_ANIMATION, true);
		fIos->SetBoolProp(EXP_FBX_GLOBAL_SETTINGS, true);
		mFbxManager->SetIOSettings(fIos);

		// Create scene
		auto fbxScene = ConvertToFbxScene(modelPack);
		ExportFbxScene(fbxScene, path, false);
	}

	void FbxSdkModelPackExporter::ExportFbxScene(FbxScene* fbxScene, String^ path, bool isAscii)
	{
		// Create exporter
		auto fbxExporter = FbxExporter::Create(mFbxManager, "");
		if (!fbxExporter->SetFileExportVersion(FBX_2014_00_COMPATIBLE))
			throw gcnew FbxSdkModelPackExporterException("Failed to set FBX export version");

		// Initialize exporter
		int pFileFormat = -1;
		if (isAscii)
		{
			int lFormatIndex, lFormatCount = mFbxManager->GetIOPluginRegistry()->GetWriterFormatCount();
			for (lFormatIndex = 0; lFormatIndex < lFormatCount; lFormatIndex++)
			{
				if (mFbxManager->GetIOPluginRegistry()->WriterIsFBX(lFormatIndex))
				{
					FbxString lDesc = mFbxManager->GetIOPluginRegistry()->GetWriterFormatDescription(lFormatIndex);
					if (lDesc.Find("ascii") >= 0)
					{
						pFileFormat = lFormatIndex;
						break;
					}
				}
			}
			if (pFileFormat == -1)
				throw gcnew FbxSdkModelPackExporterException("Failed to find FBX ASCII export format.");
		}

		if (!fbxExporter->Initialize(Utf8String(path).ToCStr(), pFileFormat, mFbxManager->GetIOSettings()))
		{
			auto errorMsg = gcnew String(fbxExporter->GetStatus().GetErrorString());
			throw gcnew FbxSdkModelPackExporterException("Failed to initialize FBX exporter: " + errorMsg);
		}

		// Convert to Z up
		//FbxAxisSystem::Max.DeepConvertScene(fbxScene);

		// Export scene
		fbxExporter->Export(fbxScene);

		// Destroy exporter
		fbxExporter->Destroy();
	}
}