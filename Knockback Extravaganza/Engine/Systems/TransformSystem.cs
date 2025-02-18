﻿using ECS_Engine.Engine.Systems.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECS_Engine.Engine.Managers;
using Microsoft.Xna.Framework;
using ECS_Engine.Engine.Component.Interfaces;
using ECS_Engine.Engine.Component;

namespace ECS_Engine.Engine.Systems {
    public class TransformSystem : IUpdateSystem {
        public void Update(GameTime gametime, ComponentManager componentManager) {
            Dictionary<Entity, IComponent> components = componentManager.GetComponents<TransformComponent>();
            if(components != null){
                foreach(KeyValuePair<Entity, IComponent> component in components) {
                    TransformComponent transform = componentManager.GetComponent<TransformComponent>(component.Key);
                    Quaternion rotation = Quaternion.CreateFromYawPitchRoll(transform.Rotation.Y, transform.Rotation.X, transform.Rotation.Z);

                    transform.World = Matrix.CreateScale(transform.Scale) * Matrix.CreateFromQuaternion(rotation) * Matrix.CreateTranslation(transform.Position);
                    transform.Forward = GetLocalDir(Vector3.Forward, rotation);
                    transform.Right = GetLocalDir(Vector3.Right, rotation);
                    transform.Up = GetLocalDir(Vector3.Up, rotation);

                }
            }

            Dictionary<Entity, IComponent> ModelComponents = componentManager.GetComponents<ModelTransformComponent>();
            if (components != null) {
                foreach (KeyValuePair<Entity, IComponent> component in ModelComponents) {
                    ModelTransformComponent transform = componentManager.GetComponent<ModelTransformComponent>(component.Key);
                    foreach (MeshTransform mesh in transform.GetTransforms().Values) {
                        Quaternion rotation = Quaternion.CreateFromYawPitchRoll(mesh.Rotation.Y, mesh.Rotation.X, mesh.Rotation.Z);

                        mesh.World = Matrix.CreateScale(mesh.Scale) * Matrix.CreateFromQuaternion(rotation) * Matrix.CreateTranslation(mesh.Position);
  
                        mesh.Forward = GetLocalDir(Vector3.Forward, rotation);
                        mesh.Right = GetLocalDir(Vector3.Right, rotation);
                        mesh.Up = GetLocalDir(Vector3.Up, rotation);
                    }

                }
            }
    }

        private Vector3 GetLocalDir(Vector3 dir, Quaternion rotation) {
            Vector3 result = Vector3.Transform(dir, rotation);
            result.Normalize();
            return result;
        }
    }
}
