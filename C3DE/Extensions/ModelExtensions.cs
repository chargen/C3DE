﻿using C3DE.Components.Rendering;
using C3DE.Graphics.Geometries;
using C3DE.Graphics.Materials;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace C3DE.Extensions
{
    public static class ModelExtensions
    {
        public static GameObject ToMeshRenderers(this Model model, Scene scene)
        {
            var boneTransforms = new Matrix[model.Bones.Count];
            model.CopyAbsoluteBoneTransformsTo(boneTransforms);

            var gameObject = new GameObject("Model");
            scene.Add(gameObject);

            foreach (ModelMesh mesh in model.Meshes)
            {
                var meshPartIndex = 0;

                var parent = new GameObject(mesh.Name);
                parent.Transform.Parent = gameObject.Transform;

                var matrix = boneTransforms[mesh.ParentBone.Index];
                Vector3 position;
                Quaternion rotation;
                Vector3 scale;

                matrix.Decompose(out scale, out rotation, out position);

                parent.Transform.Position = position;
                parent.Transform.Rotation = rotation.ToEuler();
                parent.Transform.LocalScale = scale;

                foreach (var part in mesh.MeshParts)
                {
                    var effect = (BasicEffect)part.Effect;
                    var material = new StandardMaterial(scene);
                    material.Texture = effect.Texture;
                    material.DiffuseColor = new Color(effect.DiffuseColor.X, effect.DiffuseColor.Y, effect.DiffuseColor.Z);
                    material.SpecularColor = new Color(effect.SpecularColor.X, effect.SpecularColor.Y, effect.SpecularColor.Z);
                    material.Shininess = effect.SpecularPower;
                    material.EmissiveColor = new Color(effect.EmissiveColor.X, effect.EmissiveColor.Y, effect.EmissiveColor.Z);

                    var child = new GameObject($"{mesh.Name}_{meshPartIndex}");
                    var renderer = child.AddComponent<MeshRenderer>();
                    renderer.material = material;
                    
                    var geometry = new Geometry();
                    geometry.VertexBuffer = part.VertexBuffer;
                    geometry.IndexBuffer = part.IndexBuffer;

                    renderer.Geometry = geometry;

                    child.Transform.Parent = parent.Transform;
                }
            }

            return gameObject;
        }
    }
}
