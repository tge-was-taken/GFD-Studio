using AtlusGfdFramework;
using System;

namespace AtlusGfdEditor.Gui.WrapperTreeNodes
{
    static class WrapperTreeNodeFactory
    {
        public static GfdResourceBundleWrapperTreeNode CreateWrapper(GfdResourceBundle resourceBundle)
        {
            // Create resource bundle node, to serve as a root
            var resourceBundleNode = new GfdResourceBundleWrapperTreeNode(resourceBundle);

            // Get all resources, and add each to the node
            var resources = resourceBundle.GetResources();

            foreach (var res in resources)
            {
                switch (res.Type)
                {
                    case GfdResourceType.AnimationList:
                        resourceBundleNode.Nodes.Add(CreateWrapper(res as GfdAnimationList));
                        break;
                    case GfdResourceType.TextureDictionary:
                        resourceBundleNode.Nodes.Add(CreateWrapper(res as GfdTextureDictionary));
                        break;
                    case GfdResourceType.MaterialDictionary:
                        resourceBundleNode.Nodes.Add(CreateWrapper(res as GfdMaterialDictionary));
                        break;
                    case GfdResourceType.Scene:
                        resourceBundleNode.Nodes.Add(CreateWrapper(res as GfdScene));
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }

            return resourceBundleNode;
        }

        private static GfdAnimationListWrapperTreeNode CreateWrapper(GfdAnimationList animationList)
        {
            return null;
        }

        private static GfdTextureDictionaryWrapperTreeNode CreateWrapper(GfdTextureDictionary textureDictionary)
        {
            var textureDicNode = new GfdTextureDictionaryWrapperTreeNode(textureDictionary);
            textureDicNode.Name = textureDicNode.Text = "Textures";

            foreach (var item in textureDictionary.Values)
            {
                var textureNode = new GfdTextureWrapperTreeNode(item);
                textureDicNode.Nodes.Add(textureNode);
            }

            return textureDicNode;
        }

        private static GfdMaterialDictionaryWrapperTreeNode CreateWrapper(GfdMaterialDictionary materialDictionary)
        {
            return null;
        }

        private static GfdSceneWrapperTreeNode CreateWrapper(GfdScene scene)
        {
            return null;
        }
    }
}
