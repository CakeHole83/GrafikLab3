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
                                        pass.Apply();
                                    }
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
            DrawShadowmapModels(graphicsDevice.GraphicsDevice, componentManager);
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

        void DrawShadowmapModels(GraphicsDevice graphicsDevice, ComponentManager componentManager)
        {
            Dictionary<Entity, IComponent> camEntities = componentManager.GetComponents<CameraComponent>();
            CameraComponent camera = (CameraComponent)camEntities.First().Value;

            Dictionary<Entity, IComponent> shadowComponents = componentManager.GetComponents<ShadowComponent>();

            foreach (KeyValuePair<Entity, IComponent> shadows in shadowComponents)
            {
                ShadowComponent shadow = (ShadowComponent)shadows.Value;
                //Detta skall endast göras en gång och inte för varje skugga. 
                //Det handlar enbart om att se hur kamerans position är kontra ljussättningen för att veta var skuggor skall ritas ut.
                //Det enda som skall köras för varje model är DrawShadowModel
                //Det vi vill göra är att där sätta en loop runt DrawShadowModel och där rita ut varje model som har en skugga.
                //Övriga funktioner och variabler beror inte på shadowcomponenten och bör därför endast köras en gång per gametick.
                shadow.LightViewProjection = CreateLightViewProjectionMatrix(shadow, camera);

                graphicsDevice.BlendState = BlendState.Opaque;
                graphicsDevice.DepthStencilState = DepthStencilState.Default;

                CreateShadowMap(componentManager, shadow, graphicsDevice);

                shadow.SpriteBatch.Begin(0, BlendState.Opaque, SamplerState.PointClamp, null, null);
                shadow.SpriteBatch.Draw(shadow.ShadowRenderTarget, new Rectangle(0, 0, 128, 128), Color.White);
                shadow.SpriteBatch.End();

                graphicsDevice.Textures[0] = null;
                graphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;
            }

        }
        public Matrix CreateLightViewProjectionMatrix(ShadowComponent shadow, CameraComponent camera)
        {
            shadow.LightRotation = Matrix.CreateLookAt(Vector3.Zero,
                                                       -shadow.LightDirection,
                                                       Vector3.Up);
            //Detta måste rättas till. Camera Frustum beräkningen känns inte korrekt.
            //Måste se över variablerna i cameran och bestämma var uträkningen skall ske.
            BoundingFrustum camFrustrum = new BoundingFrustum(Matrix.Identity);
            camFrustrum.Matrix = camera.View * camera.Projection;
            shadow.FrustumCorners = camFrustrum.GetCorners();

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

        void CreateShadowMap(ComponentManager componentManager, ShadowComponent shadow, GraphicsDevice graphicsDevice)
        {
            Dictionary<Entity, IComponent> modelComponents = componentManager.GetComponents<ModelComponent>();
            Dictionary<Entity, IComponent> cam = componentManager.GetComponents<CameraComponent>();
            CameraComponent camera = (CameraComponent)cam.First().Value;


            foreach (var modelComponent in modelComponents)
            {
                ModelComponent model = (ModelComponent)modelComponent.Value;
                //Plocka ut entiteterna istället så du kan använda dem för att ta fram transform comp per entity med nyckeln
                if (model.ShadowsOn)
                {
                    shadow.ShadowRenderTarget = new RenderTarget2D(graphicsDevice,
                                                    2048,
                                                    2048,
                                                    false,
                                                    SurfaceFormat.Single,
                                                    DepthFormat.Depth24);
                    graphicsDevice.SetRenderTarget(shadow.ShadowRenderTarget);
                    graphicsDevice.Clear(Color.White);
                    DrawShadowModel(componentManager, modelComponent, shadow, camera, true);
                    graphicsDevice.SetRenderTarget(null);
                    graphicsDevice.Clear(Color.CornflowerBlue);
                    graphicsDevice.SamplerStates[1] = SamplerState.PointClamp;
                }
                
                
                DrawShadowModel(componentManager, modelComponent, shadow, camera, false);
            }

        }

        void DrawShadowModel(ComponentManager componentManager, KeyValuePair<Entity, IComponent> modelComponent, ShadowComponent shadow, CameraComponent camera, bool createShadowMap)
        {
            //Det är här modellerna påverkas utav effekten. Sätt alltså effekten i en egen komponent som är påslagen hela tiden.
            //Låt modellerna påverkas här men de har ingenting med värdena att göra.
            string techniqueName = createShadowMap ? "CreateShadowMap" : "DrawWithShadowMap";

            ModelTransformComponent modelTransform = componentManager.GetComponent<ModelTransformComponent>(modelComponent.Key);
            TransformComponent transformComponent = componentManager.GetComponent<TransformComponent>(modelComponent.Key);
            ModelComponent model = (ModelComponent)modelComponent.Value;
            EffectComponent effectComponent = componentManager.GetComponent<EffectComponent>(modelComponent.Key);

            Matrix[] transforms = new Matrix[model.Model.Bones.Count];
            model.Model.CopyAbsoluteBoneTransformsTo(transforms);

            // Loop over meshs in the model
            foreach (ModelMesh mesh in model.Model.Meshes)
            {

                    // Loop over effects in the mesh
                    foreach (var effect in effectComponent.Effects)
                    {
                        Effect part = effect.Key;
                        if (part.Name == "Effects/ShadowMapping")
                        {
                            part.CurrentTechnique = part.Techniques[techniqueName];

                            part.Parameters["World"].SetValue(
                                modelTransform.GetTranform(mesh.Name).World * transforms[mesh.ParentBone.Index] * transformComponent.World);
                            part.Parameters["View"].SetValue(camera.View);
                            part.Parameters["Projection"].SetValue(camera.Projection);
                            
                            part.Parameters["LightDirection"].SetValue(shadow.LightDirection);
                            part.Parameters["LightViewProj"].SetValue(shadow.LightViewProjection);

                            if (!createShadowMap)
                                part.Parameters["ShadowMap"].SetValue(shadow.ShadowRenderTarget);
                        }
                    }
                
                mesh.Draw();
            }
        }
    }
}
