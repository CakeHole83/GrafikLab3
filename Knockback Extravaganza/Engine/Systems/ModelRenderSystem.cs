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

        public void DrawModelsWithEffects(GraphicsDeviceManager graphicsDevice, ComponentManager componentManager)
        {
            Dictionary<Entity, IComponent> cam = componentManager.GetComponents<CameraComponent>();
            CameraComponent camera = (CameraComponent)cam.First().Value;
            Dictionary<Entity, IComponent> shadowComponents = componentManager.GetComponents<ShadowComponent>();
            ShadowComponent shadow = (ShadowComponent)shadowComponents.First().Value;

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
                                        //LÖS SÅ ATT DEN INTE GÅR IGENOM SHADOW EFFEKT. Mesh.draw() kommer att cpa för att man har modeller med flera effekter där shadow effekten inte är satt än.
                                        part.Effect = effect.Key;
                                        if(part.Effect.Name != "Effects/ShadowMapping") {
                                            foreach (EffectTechnique technique in part.Effect.Techniques)
                                            {
                                                part.Effect.CurrentTechnique = part.Effect.Techniques[technique.Name];
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
                                                pass.Apply();
                                            }
                                        }
                                    }
                                }
                                mesh.Draw();
                            }

                        }

                    }
                }
            }
            DrawShadows(shadow, componentManager, graphicsDevice.GraphicsDevice);
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
                                        if (part.Effect.Name != "Effects/ShadowMapping")
                                        {
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

        public Matrix CreateLightViewProjectionMatrix(ShadowComponent shadow, CameraComponent camera)
        {
            shadow.LightRotation = Matrix.CreateLookAt(Vector3.Zero,
                                                       -shadow.LightDirection,
                                                       Vector3.Up);

            shadow.FrustumCorners = camera.CameraFrustum.GetCorners();

            for (int i = 0; i < shadow.FrustumCorners.Length; i++)
            {
                shadow.FrustumCorners[i] = Vector3.Transform(shadow.FrustumCorners[i], shadow.LightRotation);
            }

            BoundingBox lightBox = BoundingBox.CreateFromPoints(shadow.FrustumCorners);

            Vector3 boxSize = lightBox.Max - lightBox.Min;
            Vector3 halfBoxSize = boxSize * 0.5f;

            Vector3 lightPosition = lightBox.Min + halfBoxSize;
            lightPosition.Z = lightBox.Min.Z;

            lightPosition = Vector3.Transform(lightPosition,
                                              Matrix.Invert(shadow.LightRotation));

            Matrix lightView = Matrix.CreateLookAt(lightPosition,
                                                   lightPosition - shadow.LightDirection,
                                                   Vector3.Up);

            Matrix lightProjection = Matrix.CreateOrthographic(boxSize.X, boxSize.Y,
                                                               -boxSize.Z, boxSize.Z);

            return lightView * lightProjection;
        }

        public void DrawShadows(ShadowComponent shadow, ComponentManager componentManager, GraphicsDevice graphicsDevice)
        {

            Dictionary<Entity, IComponent> cam = componentManager.GetComponents<CameraComponent>();
            CameraComponent camera = (CameraComponent)cam.First().Value;

            shadow.LightViewProjection = CreateLightViewProjectionMatrix(shadow, camera);

            graphicsDevice.BlendState = BlendState.Opaque;
            graphicsDevice.DepthStencilState = DepthStencilState.Default;

            CreateShadowMap(shadow, componentManager, graphicsDevice);
        }

        public void CreateShadowMap(ShadowComponent shadow, ComponentManager componentManager, GraphicsDevice graphicsDevice)
        {
            graphicsDevice.SetRenderTarget(shadow.ShadowRenderTarget);
            //graphicsDevice.Clear(Color.White);

            Dictionary<Entity, IComponent> modelEntities = componentManager.GetComponents<ModelComponent>();
            foreach (KeyValuePair<Entity, IComponent> modelC in modelEntities)
            {
                ModelComponent model = (ModelComponent)modelC.Value;
                if (model.ShadowsOn && model.CastShadowOn)
                {
                    DrawShadowModel(modelC, shadow, componentManager, true);
                }
            }
            graphicsDevice.SetRenderTarget(null);

            //graphicsDevice.Clear(Color.CornflowerBlue);
            graphicsDevice.SamplerStates[1] = SamplerState.PointClamp;
            foreach(KeyValuePair<Entity, IComponent> modelC in modelEntities)
            {
                ModelComponent model = (ModelComponent)modelC.Value;
                if (model.ShadowsOn)
                {
                    DrawShadowModel(modelC, shadow, componentManager, false);
                }
            }
            shadow.SpriteBatch.Begin(0, BlendState.Opaque, SamplerState.PointClamp, null, null);
            shadow.SpriteBatch.Draw(shadow.ShadowRenderTarget, new Rectangle(0, 0, 128, 128), Color.White);
            shadow.SpriteBatch.End();

            graphicsDevice.Textures[0] = null;
            graphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;
        }

        public void DrawShadowModel(KeyValuePair<Entity, IComponent> modelC, ShadowComponent shadow, ComponentManager componentManager, bool createShadowMap)
        {
            string techniqueName = createShadowMap ? "CreateShadowMap" : "DrawWithShadowMap";

            ModelComponent model = (ModelComponent)modelC.Value;

            Matrix[] transforms = new Matrix[model.Model.Bones.Count];
            model.Model.CopyAbsoluteBoneTransformsTo(transforms);

            Dictionary<Entity, IComponent> cam = componentManager.GetComponents<CameraComponent>();
            CameraComponent camera = (CameraComponent)cam.First().Value;

            TransformComponent cameraTransC = componentManager.GetComponent<TransformComponent>(cam.First().Key);
            EffectComponent effectComponent = componentManager.GetComponent<EffectComponent>(modelC.Key);

            ModelTransformComponent meshTransform = componentManager.GetComponent<ModelTransformComponent>(modelC.Key);
            TransformComponent transform = componentManager.GetComponent<TransformComponent>(cam.First().Key);

            foreach (ModelMesh mesh in model.Model.Meshes)
            {
                // Loop over effects in the mesh
                foreach (Effect effect in mesh.Effects)
                {
                    if (effect.Name == "Effects/ShadowMapping")
                    {
                        effect.Parameters["World"].SetValue(meshTransform.GetTranform(mesh.Name).World * transforms[mesh.ParentBone.Index] * transform.World);
                        effect.Parameters["View"].SetValue(camera.View);
                        effect.Parameters["Projection"].SetValue(camera.Projection);
                        // Set the currest values for the effect
                        effect.CurrentTechnique = effect.Techniques[techniqueName];
                        effect.Parameters["LightDirection"].SetValue(shadow.LightDirection);
                        effect.Parameters["LightViewProj"].SetValue(shadow.LightViewProjection);

                        if (!createShadowMap)
                            effect.Parameters["ShadowMap"].SetValue(shadow.ShadowRenderTarget);
                    }
                }
                // Kan inte m¨åla ut per effekt. Kolla så att det endast körs en mesh.Draw per modell. 
                mesh.Draw();
            }

        }
    }
}