using ECS_Engine.Engine.Component;
using ECS_Engine.Engine.Component.Interfaces;
using ECS_Engine.Engine.Component.Shaders;
using ECS_Engine.Engine.Managers;
using ECS_Engine.Engine.Systems.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECS_Engine.Engine.Systems
{
    public class ModelRenderSystem : IRenderSystem
    {
        SortedList<float, Entity> transparentEntities = new SortedList<float, Entity>();
        public void Render(GameTime gameTime, GraphicsDeviceManager graphicsDevice, ComponentManager componentManager)
        {
            Dictionary<Entity, IComponent> cameraValuePairs = componentManager.GetComponents<CameraComponent>();
            CameraComponent cameraC = (CameraComponent)cameraValuePairs.First().Value;
//            CubemapComponent cubeMapC = (CubemapComponent)cubeMapKeyValuePairs.First().Value;
//            Dictionary<Entity, IComponent> cubeMapKeyValuePairs = componentManager.GetComponents<CubemapComponent>();
            //if (cubeMapC != null)
            //    CreateCubeMap(cubeMapC, cameraC, componentManager, graphicsDevice);
            DrawSkybox(graphicsDevice.GraphicsDevice, componentManager);

            Dictionary<Entity, IComponent> cam = componentManager.GetComponents<CameraComponent>();
            CameraComponent camera = (CameraComponent)cam.First().Value;
            foreach (KeyValuePair<Entity, IComponent> camE in cam)
            {
                TransformComponent cameraTransC = componentManager.GetComponent<TransformComponent>(camE.Key);

                Dictionary<Entity, IComponent> components = componentManager.GetComponents<ModelComponent>();
                foreach (KeyValuePair<Entity, IComponent> component in components)
                {
                    ModelComponent model = (ModelComponent)component.Value;
                    ModelTransformComponent meshTransform = componentManager.GetComponent<ModelTransformComponent>(component.Key);
                    TransformComponent transform = componentManager.GetComponent<TransformComponent>(component.Key);
                    EffectComponent effectC = componentManager.GetComponent<EffectComponent>(component.Key);
                    if (effectC == null)
                    {
                        if (meshTransform != default(ModelTransformComponent) && transform != default(TransformComponent))
                        {
                            Matrix[] transforms = new Matrix[model.Model.Bones.Count()];
                            model.Model.CopyAbsoluteBoneTransformsTo(transforms);

                            foreach (ModelMesh mesh in model.Model.Meshes)
                            {
                                foreach (BasicEffect effect in mesh.Effects)
                                {
                                    CheckForTexture(model, effect);
                                    effect.EnableDefaultLighting();
                                    effect.View = camera.View;
                                    effect.Projection = camera.Projection;
                                    effect.World = meshTransform.GetTranform(mesh.Name).World * transforms[mesh.ParentBone.Index] * transform.World;
                                }
                                mesh.Draw();
                            }
                        }
                        else if (transform != default(TransformComponent))
                        {

                            Matrix[] transforms = new Matrix[model.Model.Bones.Count()];
                            model.Model.CopyAbsoluteBoneTransformsTo(transforms);
                            foreach (ModelMesh mesh in model.Model.Meshes)
                            {
                                foreach (BasicEffect effect in mesh.Effects)
                                {
                                    CheckForTexture(model, effect);
                                    effect.EnableDefaultLighting();
                                    effect.View = camera.View;
                                    effect.Projection = camera.Projection;
                                    effect.World = transforms[mesh.ParentBone.Index] * transform.World;
                                }
                                mesh.Draw();
                            }
                        }
                    }
                }
            }
            DrawModelsWithEffects(graphicsDevice, componentManager);
        }

        //public void CreateCubeMap(CubemapComponent cubeMapComponent, CameraComponent cameraComponent, ComponentManager componentManager, GraphicsDeviceManager graphicsDevice)
        //{
        //    Dictionary<Entity, IComponent> cam = componentManager.GetComponents<CameraComponent>();
        //    CameraComponent camera = (CameraComponent)cam.First().Value;
        //    foreach (KeyValuePair<Entity, IComponent> camE in cam)
        //    {
        //        TransformComponent cameraTransC = componentManager.GetComponent<TransformComponent>(camE.Key);

        //        Dictionary<Entity, IComponent> models = componentManager.GetComponents<CubemapComponent>();
        //        foreach (KeyValuePair<Entity, IComponent> keyvaluepair in models)
        //        {
        //            TransformComponent transC = componentManager.GetComponent<TransformComponent>(keyvaluepair.Key);
        //            Matrix originalViewMatrix = cameraComponent.View;
        //            Matrix originalProjectionMatrix = cameraComponent.Projection;
       
        //            var proj = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(90), 1, cameraComponent.NearPlaneDistace, cameraComponent.FarPlaneDistace);

        //            cameraComponent.Projection = proj;
        //            cubeMapComponent.RenderTextCube = false;
                 

        //            for (int i = 0; i < 6; i++)
        //            {
        //                CubeMapFace cubeMapFace = (CubeMapFace)i;
        //                switch (cubeMapFace)
        //                {
        //                    case CubeMapFace.NegativeX:
        //                        {
        //                            cameraComponent.View = Matrix.CreateLookAt(transC.Position, Vector3.Left + transC.Position, Vector3.Up);
        //                            break;
        //                        }
        //                    case CubeMapFace.PositiveY:
        //                        {
        //                            cameraComponent.View = Matrix.CreateLookAt(transC.Position, Vector3.Down + transC.Position, Vector3.Forward);
        //                            break;
        //                        }
        //                    case CubeMapFace.NegativeZ:
        //                        {
        //                            cameraComponent.View = Matrix.CreateLookAt(transC.Position, Vector3.Backward + transC.Position, Vector3.Up);
        //                            break;
        //                        }
        //                    case CubeMapFace.PositiveX:
        //                        {
        //                            cameraComponent.View = Matrix.CreateLookAt(transC.Position, Vector3.Right + transC.Position, Vector3.Up);
        //                            break;
        //                        }
        //                    case CubeMapFace.NegativeY:
        //                        {
        //                            cameraComponent.View = Matrix.CreateLookAt(transC.Position, Vector3.Up + transC.Position, Vector3.Backward);
        //                            break;
        //                        }
        //                    case CubeMapFace.PositiveZ:
        //                        {
        //                            cameraComponent.View = Matrix.CreateLookAt(transC.Position, Vector3.Forward + transC.Position, Vector3.Up);
        //                            break;
        //                        }
        //                    default:
        //                        break;
        //                }

        //                graphicsDevice.GraphicsDevice.SetRenderTarget(cubeMapComponent.renderTargetCube, cubeMapFace);

        //                DrawModelsWithEffects(graphicsDevice, componentManager);
        //                DrawSkybox(graphicsDevice.GraphicsDevice, componentManager);
        //            }
        //            graphicsDevice.GraphicsDevice.SetRenderTarget(null);
        //            cubeMapComponent.environmentMap = cubeMapComponent.renderTargetCube;
        //            cameraComponent.View = originalViewMatrix;
        //            cameraComponent.Projection = originalProjectionMatrix;
        //            cubeMapComponent.RenderTextCube = true;
        //        }
        //    }
        //}

        public void DrawModelsWithEffects(GraphicsDeviceManager graphicsDevice, ComponentManager componentManager)
        {
            Dictionary<Entity, IComponent> cam = componentManager.GetComponents<CameraComponent>();
            CameraComponent camera = (CameraComponent)cam.First().Value;
            foreach (KeyValuePair<Entity, IComponent> camE in cam)
            {
                TransformComponent cameraTransC = componentManager.GetComponent<TransformComponent>(camE.Key);

                Dictionary<Entity, IComponent> components = componentManager.GetComponents<EffectComponent>();
                foreach (KeyValuePair<Entity, IComponent> component in components)
                {
                    EffectComponent effectC = (EffectComponent)component.Value;
                    ModelComponent model = componentManager.GetComponent<ModelComponent>(component.Key);
                    ModelTransformComponent meshTransform = componentManager.GetComponent<ModelTransformComponent>(component.Key);
                    TransformComponent transform = componentManager.GetComponent<TransformComponent>(component.Key);
//                    CubemapComponent cubeMapComponent = componentManager.GetComponent<CubemapComponent>(component.Key);
                    if (model.HasTransparentMesh == true)
                        transparentEntities.Add(Vector3.Distance(cameraTransC.Position, transform.Position), component.Key);

                    if (effectC != null && model.HasTransparentMesh == false)
                    {
                        Matrix[] transforms = new Matrix[model.Model.Bones.Count];
                        model.Model.CopyAbsoluteBoneTransformsTo(transforms);

                        foreach (ModelMesh mesh in model.Model.Meshes)
                        {
                            foreach (ModelMeshPart part in mesh.MeshParts)
                            {
                                foreach (EffectPass pass in part.Effect.CurrentTechnique.Passes)
                                {
                                    foreach (var effect in effectC.Effects)
                                    {
                                        part.Effect = effect.Key;
                                        part.Effect.Parameters["World"].SetValue(meshTransform.GetTranform(mesh.Name).World * transforms[mesh.ParentBone.Index] * transform.World);
                                        part.Effect.Parameters["View"].SetValue(camera.View);
                                        part.Effect.Parameters["Projection"].SetValue(camera.Projection);

                                        foreach (var parameter in effect.Value)
                                        {
                                            switch (parameter)
                                            {
                                                case "AmbientColor":
                                                    part.Effect.Parameters["AmbientColor"].SetValue(Color.Green.ToVector4());
                                                    break;
                                                case "AmbientIntensity":
                                                    part.Effect.Parameters["AmbientIntensity"].SetValue(0.5f);
                                                    break;
                                                case "WorldInverseTranspose":
                                                    Matrix worldInverseTransposeMatrix =
                                                    Matrix.Transpose(Matrix.Invert(mesh.ParentBone.Transform * meshTransform.GetTranform(mesh.Name).World));
                                                    part.Effect.Parameters["WorldInverseTranspose"].SetValue(worldInverseTransposeMatrix);
                                                    break;
                                                case "ViewVector":
                                                    part.Effect.Parameters["ViewVector"].SetValue(camera.ViewVector);
                                                    break;
                                                case "NormalMap":
                                                    part.Effect.Parameters["NormalMap"].SetValue(effectC.Normalmap);
                                                    break;
                                                case "ModelTexture":
                                                    part.Effect.Parameters["ModelTexture"].SetValue(model.Texture);
                                                    break;
                                                case "SkyboxTexture":
                                                    part.Effect.Parameters["SkyboxTexture"].SetValue(effectC.SkyboxTexture);
                                                    break;
                                                case "CameraPosition":
                                                    part.Effect.Parameters["CameraPosition"].SetValue(cameraTransC.Position);
                                                    break;
                                                default:
                                                    break;
                                            }
                                        }

                                    }
                                    pass.Apply();
                                }
                                mesh.Draw();
                            }
                        }
                    }
                    if (effectC != null && model.HasTransparentMesh == false)
                    {
                        Matrix[] transforms = new Matrix[model.Model.Bones.Count];
                        model.Model.CopyAbsoluteBoneTransformsTo(transforms);

                        foreach (ModelMesh mesh in model.Model.Meshes)
                        {
                            foreach (ModelMeshPart part in mesh.MeshParts)
                            {
                                foreach (EffectPass pass in part.Effect.CurrentTechnique.Passes)
                                {
                                    foreach (var effect in effectC.Effects)
                                    {
                                        part.Effect = effect.Key;
                                        part.Effect.Parameters["World"].SetValue(meshTransform.GetTranform(mesh.Name).World * transforms[mesh.ParentBone.Index] * transform.World);
                                        part.Effect.Parameters["View"].SetValue(camera.View);
                                        part.Effect.Parameters["Projection"].SetValue(camera.Projection);

                                        foreach (var parameter in effect.Value)
                                        {
                                            switch (parameter)
                                            {
                                                case "WorldInverseTranspose":
                                                    Matrix worldInverseTransposeMatrix =
                                                    Matrix.Transpose(Matrix.Invert(mesh.ParentBone.Transform * meshTransform.GetTranform(mesh.Name).World));
                                                    part.Effect.Parameters["WorldInverseTranspose"].SetValue(worldInverseTransposeMatrix);
                                                    break;
                                                //case "SkyboxTexture":
                                                //    part.Effect.Parameters["SkyboxTexture"].SetValue(cubeMapComponent.environmentMap);
                                                //    break;
                                                case "CameraPosition":
                                                    part.Effect.Parameters["CameraPosition"].SetValue(cameraTransC.Position);
                                                    break;
                                                default:
                                                    break;
                                            }
                                        }

                                    }
                                    pass.Apply();
                                }
                                mesh.Draw();
                            }
                        }
                    }
                }
            }
            DrawTransparentModels(graphicsDevice, componentManager, transparentEntities);
            transparentEntities.Clear();
        }

        public void DrawTransparentModels(GraphicsDeviceManager graphicsDevice, ComponentManager componentManager, SortedList<float, Entity> transparentEntities)
        {
            Dictionary<Entity, IComponent> cam = componentManager.GetComponents<CameraComponent>();
            CameraComponent camera = (CameraComponent)cam.First().Value;
            var orderedList = transparentEntities.OrderByDescending(k => k.Key);
            foreach (KeyValuePair<Entity, IComponent> camE in cam)
            {
                TransformComponent cameraTransC = componentManager.GetComponent<TransformComponent>(camE.Key);
                foreach (var entity in orderedList)
                {
                    EffectComponent effectC = componentManager.GetComponent<EffectComponent>(entity.Value);
                    ModelComponent model = componentManager.GetComponent<ModelComponent>(entity.Value);
                    ModelTransformComponent meshTransform = componentManager.GetComponent<ModelTransformComponent>(entity.Value);
                    TransformComponent transform = componentManager.GetComponent<TransformComponent>(entity.Value);

                    if (effectC != null && model.HasTransparentMesh == true)
                    {
                        Matrix[] transforms = new Matrix[model.Model.Bones.Count];
                        model.Model.CopyAbsoluteBoneTransformsTo(transforms);

                        foreach (ModelMesh mesh in model.Model.Meshes)
                        {
                            foreach (ModelMeshPart part in mesh.MeshParts)
                            {
                                foreach (EffectPass pass in part.Effect.CurrentTechnique.Passes)
                                {
                                    foreach (var effect in effectC.Effects)
                                    {
                                        part.Effect = effect.Key;

                                        part.Effect.Parameters["World"].SetValue(
                                              meshTransform.GetTranform(mesh.Name).World * transforms[mesh.ParentBone.Index] * transform.World);
                                        part.Effect.Parameters["View"].SetValue(camera.View);
                                        part.Effect.Parameters["Projection"].SetValue(camera.Projection);

                                        foreach (var parameter in effect.Value)
                                        {
                                            switch (parameter)
                                            {
                                                case "AmbientColor":
                                                    part.Effect.Parameters["AmbientColor"].SetValue(Color.Green.ToVector4());
                                                    break;
                                                case "AmbientIntensity":
                                                    part.Effect.Parameters["AmbientIntensity"].SetValue(0.5f);
                                                    break;
                                                case "WorldInverseTranspose":
                                                    Matrix worldInverseTransposeMatrix =
                                                    Matrix.Transpose(Matrix.Invert(mesh.ParentBone.Transform * meshTransform.GetTranform(mesh.Name).World));
                                                    part.Effect.Parameters["WorldInverseTranspose"].SetValue(worldInverseTransposeMatrix);
                                                    break;
                                                case "ViewVector":
                                                    part.Effect.Parameters["ViewVector"].SetValue(camera.ViewVector);
                                                    break;
                                                case "NormalMap":
                                                    part.Effect.Parameters["NormalMap"].SetValue(effectC.Normalmap);
                                                    break;
                                                case "ModelTexture":
                                                    part.Effect.Parameters["ModelTexture"].SetValue(model.Texture);
                                                    break;
                                                case "SkyboxTexture":
                                                    part.Effect.Parameters["SkyboxTexture"].SetValue(effectC.SkyboxTexture);

                                                    break;
                                                case "CameraPosition":
                                                    part.Effect.Parameters["CameraPosition"].SetValue(cameraTransC.Position);
                                                    break;
                                                case "Transparency":
                                                    part.Effect.Parameters["Transparency"].SetValue(0.5f);
                                                    graphicsDevice.GraphicsDevice.BlendState = BlendState.AlphaBlend;
                                                    break;
                                                default:
                                                    break;
                                            }
                                        }
                                    }
                                    pass.Apply();
                                }
                                mesh.Draw();
                                graphicsDevice.GraphicsDevice.BlendState = BlendState.Opaque;
                            }
                        }
                    }
                }
            }
        }

        private void CheckForTexture(ModelComponent model, BasicEffect effect)
        {
            if (model.Texture != null)
            {
                effect.TextureEnabled = true;
                effect.Texture = model.Texture;
            }
        }

        public void DrawSkybox(GraphicsDevice graphics, ComponentManager componentManager)
        {
            Dictionary<Entity, IComponent> camEntities = componentManager.GetComponents<CameraComponent>();
            CameraComponent camera = (CameraComponent)camEntities.First().Value;

            Dictionary<Entity, IComponent> skyboxEntities = componentManager.GetComponents<SkyboxShaderComponent>();
            SkyboxShaderComponent skyboxC = (SkyboxShaderComponent)skyboxEntities.First().Value;
            foreach (KeyValuePair<Entity, IComponent> camE in camEntities)
            {

                TransformComponent cameraTransC = componentManager.GetComponent<TransformComponent>(camE.Key);

                var rs = new RasterizerState { CullMode = CullMode.CullClockwiseFace };
                graphics.RasterizerState = rs;


                foreach (EffectPass pass in skyboxC.SkyboxEffect.CurrentTechnique.Passes)
                {
                    foreach (ModelMesh mesh in skyboxC.Skybox.Meshes)
                    {
                        foreach (ModelMeshPart part in mesh.MeshParts)
                        {
                            part.Effect = skyboxC.SkyboxEffect;
                            part.Effect.Parameters["World"].SetValue(
                                Matrix.CreateScale(skyboxC.size) * Matrix.CreateTranslation(cameraTransC.Position));
                            part.Effect.Parameters["View"].SetValue(camera.View);
                            part.Effect.Parameters["Projection"].SetValue(camera.Projection);
                            part.Effect.Parameters["SkyBoxTexture"].SetValue(skyboxC.SkyboxTexture);
                            part.Effect.Parameters["CameraPosition"].SetValue(cameraTransC.Position);

                            graphics.BlendState = BlendState.AlphaBlend;
                        }

                        mesh.Draw();
                        graphics.BlendState = BlendState.Opaque;
                    }
                }
                var rs2 = new RasterizerState();
                rs2.CullMode = CullMode.CullCounterClockwiseFace;
                graphics.RasterizerState = rs2;

            }
        }
    }
}