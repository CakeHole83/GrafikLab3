﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ECS_Engine;
using ECS_Engine.Engine.Managers;
using ECS_Engine.Engine.Systems.Interfaces;
using ECS_Engine.Engine;
using ECS_Engine.Engine.Component;
using ECS_Engine.Engine.Systems;
using System.Collections.Generic;
using ECS_Engine.Engine.Component.Interfaces;
using System;
using GameEngine;
using System.Linq;
using ECS_Engine.Engine.Component.Shaders;

namespace Assignment3
{

    public class Game1 : ECSEngine
    {


        public Game1() : base()
        {

        }


        protected override void Initialize()
        {
            InitializeSystems();
            base.Initialize();
        }

        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            CreateEntities();

        }

        protected override void UnloadContent()
        {

        }

        public void CreateEntities()
        {
            //Camera target
            Entity targetEntity = new Entity();

            TransformComponent tranformCompTarget = new TransformComponent
            {
                Position = new Vector3(0, 0, 0),
                Rotation = new Vector3(0, 0, 0)
            };

            MovementComponent movementCompTarget = new MovementComponent
            {
                Speed = 5f,
                Acceleration = 1.5f
            };
            KeyBoardComponent keyboardCompTarget = new KeyBoardComponent();
            keyboardCompTarget.AddKeyToAction("Forward", Keys.W);
            keyboardCompTarget.AddKeyToAction("Backward", Keys.S);
            keyboardCompTarget.AddKeyToAction("Left", Keys.A);
            keyboardCompTarget.AddKeyToAction("Right", Keys.D);
            keyboardCompTarget.AddKeyToAction("Up", Keys.Space);
            keyboardCompTarget.AddKeyToAction("Down", Keys.LeftControl);
            keyboardCompTarget.AddKeyToAction("TurnRight", Keys.Right);
            keyboardCompTarget.AddKeyToAction("TurnLeft", Keys.Left);
            keyboardCompTarget.AddKeyToAction("LookUp", Keys.Up);
            keyboardCompTarget.AddKeyToAction("LookDown", Keys.Down);

            componentManager.AddComponent(targetEntity, movementCompTarget);
            componentManager.AddComponent(targetEntity, keyboardCompTarget);
            componentManager.AddComponent(targetEntity, tranformCompTarget);

            //Camera

            Entity cameraEntity = new Entity();
            CameraComponent cameraC = new CameraComponent
            {
                AvatarPosition = new Vector3(0f, 0, -0),
                Offset = new Vector3(0, 0, 0),
                FieldOfView = MathHelper.PiOver4,
                AspectRatio = 2,
                FarPlaneDistace = 1000000,
                NearPlaneDistace = 10,
                Up = Vector3.Up,

            };
         
            ChaseCameraComponent chaseCameraComp = new ChaseCameraComponent
            {
                Target = targetEntity,
                Offset = new Vector3(0, 0, 0.1f)
            };
            TransformComponent transformCompCamera = new TransformComponent
            {
                Position = new Vector3(0f, 0, -0),
                Rotation = new Vector3(0, 0, 0)
            };

            componentManager.AddComponent(cameraEntity, chaseCameraComp);
            componentManager.AddComponent(cameraEntity, cameraC);
            componentManager.AddComponent(cameraEntity, transformCompCamera);


            //Helicopter Texture shading
            var sphereEntity = new Entity();
            var sphereModelC = new ModelComponent
            {
                Model = Content.Load<Model>("Chopper"),
                Texture = Content.Load<Texture2D>("HelicopterTextureMap"),
            };
            var sphereEffectC = new EffectComponent
            {
                Effects = new Dictionary<Effect, List<string>>(),
            };
            sphereEffectC.Effects.Add(Content.Load<Effect>("Effects/TextureShader"), new List<string> { "WorldInverseTranspose", "ModelTexture", "ViewVector", "CameraPosition" });

            var sphereModelTransC = new ModelTransformComponent(sphereModelC.Model);

            var sphereTransformC = new TransformComponent()
            {
                Position = new Vector3(-1200, 80, 1200),
                Rotation = new Vector3(0, 0, 0),
                Scale = new Vector3(50)
            };
            componentManager.AddComponent(sphereEntity, sphereTransformC);
            componentManager.AddComponent(sphereEntity, sphereModelC);
            componentManager.AddComponent(sphereEntity, sphereModelTransC);
            componentManager.AddComponent(sphereEntity, sphereEffectC);

            //Zeppelin Diffuse shading
            var zeppE = new Entity();
            var zeppModelC = new ModelComponent
            {
                Model = Content.Load<Model>("Zeppelin_NT"),
            };
            var zeppEffectC = new EffectComponent
            {
                Effects = new Dictionary<Effect, List<string>>(),
            };

            zeppEffectC.Effects.Add(Content.Load<Effect>("Effects/Diffuse"), new List<string> { "AmbientColor", "AmbientIntensity", "WorldInverseTranspose" });
            ModelTransformComponent modeltransC = new ModelTransformComponent(zeppModelC.Model);

            var zeppTransformC = new TransformComponent()
            {
                Position = new Vector3(600, 300, 800),
                Rotation = new Vector3(0, 0, 0),
                Scale = new Vector3(.05f)
            };

            componentManager.AddComponent(zeppE, zeppModelC);
            componentManager.AddComponent(zeppE, zeppTransformC);
            componentManager.AddComponent(zeppE, modeltransC);
            componentManager.AddComponent(zeppE, zeppEffectC);

            //Nordstern Specular shader
            var nordE = new Entity();
            var nordModelC = new ModelComponent
            {
                Model = Content.Load<Model>("Nordstern"),
            };
            var nordEffectC = new EffectComponent
            {
                Effects = new Dictionary<Effect, List<string>>(),
            };
            nordEffectC.Effects.Add(Content.Load<Effect>("Effects/Specular"), new List<string> { "AmbientColor", "AmbientIntensity", "WorldInverseTranspose", "ViewVector", "CameraPosition" });

            var nordmodelTransC = new ModelTransformComponent(nordModelC.Model);

            var nordTransformC = new TransformComponent()
            {
                Position = new Vector3(-800, 200, 900),
                Rotation = new Vector3(0, MathHelper.PiOver2, 0),
                Scale = new Vector3(.1f)
            };

            componentManager.AddComponent(nordE, nordModelC);
            componentManager.AddComponent(nordE, nordTransformC);
            componentManager.AddComponent(nordE, nordmodelTransC);
            componentManager.AddComponent(nordE, nordEffectC);

            //Hangar Transparent shading
            var hangarE = new Entity();
            var hangarModelC = new ModelComponent
            {
                Model = Content.Load<Model>("moffett-hangar2"),
                HasTransparentMesh = true
            };

            var hangarEffectC = new EffectComponent
            {
                Effects = new Dictionary<Effect, List<string>>(),
            };
            hangarEffectC.Effects.Add(Content.Load<Effect>("Effects/Transparency"), new List<string> { "AmbientColor", "AmbientIntensity", "WorldInverseTranspose", "ViewVector", "ModelTexture", "Transparency" });

            var hangarModelTransC = new ModelTransformComponent(hangarModelC.Model);

            var hangarTransformC = new TransformComponent()
            {
                Position = new Vector3(1350, 0, -700),
                Rotation = new Vector3(0, MathHelper.PiOver2, 0),
                Scale = new Vector3(.05f)
            };

            componentManager.AddComponent(hangarE, hangarTransformC);
            componentManager.AddComponent(hangarE, hangarModelC);
            componentManager.AddComponent(hangarE, hangarModelTransC);
            componentManager.AddComponent(hangarE, hangarEffectC);

            var old1Entity = new Entity();
            var old1ModelC = new ModelComponent
            {
                Model = Content.Load<Model>("moffett-old-building-a"),
                HasTransparentMesh = true
            };

            var old1ModelTransC = new ModelTransformComponent(old1ModelC.Model);

            var old1EffectC = new EffectComponent
            {
                Effects = new Dictionary<Effect, List<string>>(),
            };
            old1EffectC.Effects.Add(Content.Load<Effect>("Effects/Transparency"), new List<string> { "AmbientColor", "AmbientIntensity", "WorldInverseTranspose", "ViewVector", "ModelTexture", "Transparency" });


            var old1TransformC = new TransformComponent()
            {
                Position = new Vector3(500, 0, -1400),
                Rotation = new Vector3(0, 0, 0),
                Scale = new Vector3(.4f)
            };

            componentManager.AddComponent(old1Entity, old1ModelTransC);
            componentManager.AddComponent(old1Entity, old1ModelC);
            componentManager.AddComponent(old1Entity, old1TransformC);
            componentManager.AddComponent(old1Entity, old1EffectC);

            //Bumpmap helicopter
            var old2Entity = new Entity();
            var old2ModelC = new ModelComponent
            {
                Model = Content.Load<Model>("Chopper"),

            };

            var old2EffectC = new EffectComponent
            {
                Effects = new Dictionary<Effect, List<string>>(),
                Normalmap = Content.Load<Texture2D>("normalmap/HelicopterNormalMap")
            };
            old2EffectC.Effects.Add(Content.Load<Effect>("Effects/NormalMap"), new List<string> { "AmbientColor", "AmbientIntensity", "WorldInverseTranspose", "ViewVector", "ModelTexture", "NormalMap", "CameraPosition" });

            var old2ModelTransC = new ModelTransformComponent(old2ModelC.Model);

            var old2TransformC = new TransformComponent()
            {
                Position = new Vector3(-600, 100, -1200),
                Rotation = new Vector3(0, 0, 0),
                Scale = new Vector3(50)
            };

            componentManager.AddComponent(old2Entity, old2ModelTransC);
            componentManager.AddComponent(old2Entity, old2ModelC);
            componentManager.AddComponent(old2Entity, old2TransformC);
            componentManager.AddComponent(old2Entity, old2EffectC);

            //Skybox
            var skyboxE = new Entity();

            var skyboxC = new SkyboxShaderComponent
            {
                size = 10000f,
                Skybox = Content.Load<Model>("Skyboxes/Cube"),
                SkyboxTexture = Content.Load<TextureCube>("Skyboxes/Islands"),
                SkyboxEffect = Content.Load<Effect>("Effects/Skybox")
            };

            componentManager.AddComponent(skyboxE, skyboxC);

            //ShadowMapping cube
            var shadowMapEntity = new Entity();
            shadowMapEntity.Tag = "shadowmap";

            ShadowComponent shadow = new ShadowComponent();
            componentManager.AddComponent(shadowMapEntity, shadow);

           
        }

        public void InitializeSystems()
        {
            systemManager.AddSystem(new KeyBoardSystem());
            systemManager.AddSystem(new MovementSystem());
            systemManager.AddSystem(new TransformSystem());
            systemManager.AddSystem(new CameraSystem());
            systemManager.AddSystem(new ModelRenderSystem());
            systemManager.AddSystem(new ChaseCameraSystem());

        }
    }
}