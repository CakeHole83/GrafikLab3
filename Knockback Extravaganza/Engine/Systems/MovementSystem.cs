using ECS_Engine.Engine.Systems.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECS_Engine.Engine.Managers;
using Microsoft.Xna.Framework;
using ECS_Engine.Engine.Component.Interfaces;
using ECS_Engine.Engine.Component;

namespace ECS_Engine.Engine.Systems
{
    public class MovementSystem : IUpdateSystem
    {
        public void Update(GameTime gametime, ComponentManager componentManager)
        {
            //HandleInput(gametime, componentManager);
            HandleInputAssignment2(gametime, componentManager);
        }

        public void HandleInputAssignment2(GameTime gametime, ComponentManager componentManager)
        {
            Dictionary<Entity, IComponent> keyboardComponents = componentManager.GetComponents<KeyBoardComponent>();
            foreach (KeyValuePair<Entity, IComponent> component in keyboardComponents)
            {
                KeyBoardComponent keyboardComp = (KeyBoardComponent)component.Value;
                TransformComponent transformC = componentManager.GetComponent<TransformComponent>(component.Key);
                MovementComponent movementComp = componentManager.GetComponent<MovementComponent>(component.Key);

                foreach (KeyValuePair<string, BUTTON_STATE> actionState in keyboardComp.ActionStates)
                {
                    if (actionState.Key.Equals("Forward") && actionState.Value.Equals(BUTTON_STATE.PRESSED))
                    {
                        transformC.Position += transformC.Forward * movementComp.Speed;

                    }
                    if (actionState.Key.Equals("Forward") && actionState.Value.Equals(BUTTON_STATE.HELD))
                    {
                        movementComp.Speed += movementComp.Speed * (float)gametime.ElapsedGameTime.TotalSeconds * movementComp.Acceleration;
                        if (movementComp.Speed > 30)
                            movementComp.Speed = 30;
                        transformC.Position += transformC.Forward * movementComp.Speed;
                    }
                    if (actionState.Key.Equals("Backward") && actionState.Value.Equals(BUTTON_STATE.PRESSED))
                    {
                        transformC.Position += transformC.Forward * -movementComp.Speed;
                    }
                    if (actionState.Key.Equals("Backward") && actionState.Value.Equals(BUTTON_STATE.HELD))
                    {
                        movementComp.Speed += movementComp.Speed * (float)gametime.ElapsedGameTime.TotalSeconds * movementComp.Acceleration;
                        if (movementComp.Speed > 20)
                            movementComp.Speed = 20;
                        transformC.Position += transformC.Forward * -movementComp.Speed;
                    }
                    if (actionState.Key.Equals("Right") && actionState.Value.Equals(BUTTON_STATE.HELD))
                    {
                        movementComp.Speed += movementComp.Speed * (float)gametime.ElapsedGameTime.TotalSeconds * movementComp.Acceleration;
                        if (movementComp.Speed > 20)
                            movementComp.Speed = 20;
                        transformC.Position += transformC.Right * movementComp.Speed;
                    }
                    if (actionState.Key.Equals("Left") && actionState.Value.Equals(BUTTON_STATE.HELD))
                    {
                        movementComp.Speed += movementComp.Speed * (float)gametime.ElapsedGameTime.TotalSeconds * movementComp.Acceleration;
                        if (movementComp.Speed > 20)
                            movementComp.Speed = 20;
                        transformC.Position += transformC.Right * -movementComp.Speed;
                    }

                    if (actionState.Key.Equals("TurnRight") && actionState.Value.Equals(BUTTON_STATE.PRESSED) || (actionState.Key.Equals("TurnRight") && actionState.Value.Equals(BUTTON_STATE.HELD)))
                    {
                        transformC.Rotation += new Vector3(0, -.1f, 0);
                    }
                    if (actionState.Key.Equals("TurnLeft") && actionState.Value.Equals(BUTTON_STATE.PRESSED) || (actionState.Key.Equals("TurnLeft") && actionState.Value.Equals(BUTTON_STATE.HELD)))
                    {
                        transformC.Rotation += new Vector3(0, .1f, 0);
                    }
                    if (actionState.Key.Equals("Up") && actionState.Value.Equals(BUTTON_STATE.PRESSED) || (actionState.Key.Equals("Up") && actionState.Value.Equals(BUTTON_STATE.HELD)))
                    {
                        transformC.Position += transformC.Up * movementComp.Speed;
                    }
                    if (actionState.Key.Equals("Down") && actionState.Value.Equals(BUTTON_STATE.PRESSED) || (actionState.Key.Equals("Down") && actionState.Value.Equals(BUTTON_STATE.HELD)))
                    {
                        transformC.Position += transformC.Up * -movementComp.Speed;
                    }
                    if (actionState.Key.Equals("LookUp") && actionState.Value.Equals(BUTTON_STATE.PRESSED) || (actionState.Key.Equals("LookUp") && actionState.Value.Equals(BUTTON_STATE.HELD)))
                    {
                        transformC.Rotation += new Vector3(0.025f, 0, 0);
                    }
                    if (actionState.Key.Equals("LookDown") && actionState.Value.Equals(BUTTON_STATE.PRESSED) || (actionState.Key.Equals("LookDown") && actionState.Value.Equals(BUTTON_STATE.HELD)))
                    {
                        transformC.Rotation += new Vector3(-0.025f, 0, 0);
                    }

                }
                //Dictionary<Entity, IComponent> keyboardComponents = componentManager.GetComponents<KeyBoardComponent>();
                //foreach (KeyValuePair<Entity, IComponent> component in keyboardComponents)
                //{
                //    KeyBoardComponent keyboardComp = (KeyBoardComponent)component.Value;
                //    TransformComponent transformC = componentManager.GetComponent<TransformComponent>(component.Key);
                //    MovementComponent movementComp = componentManager.GetComponent<MovementComponent>(component.Key);

                //    foreach (KeyValuePair<string, BUTTON_STATE> actionState in keyboardComp.ActionStates)
                //    {
                //        if (actionState.Key.Equals("Forward") && actionState.Value.Equals(BUTTON_STATE.PRESSED))
                //        {
                //            transformC.Position += transformC.Forward * movementComp.Speed;

                //        }
                //        if (actionState.Key.Equals("Forward") && actionState.Value.Equals(BUTTON_STATE.HELD))
                //        {
                //            movementComp.Speed += movementComp.Speed * (float)gametime.ElapsedGameTime.TotalSeconds * movementComp.Acceleration;
                //            if (movementComp.Speed > 30)
                //                movementComp.Speed = 30;
                //            transformC.Position += transformC.Forward * movementComp.Speed; 
                //        }
                //        if (actionState.Key.Equals("Backward") && actionState.Value.Equals(BUTTON_STATE.PRESSED))
                //        {
                //            transformC.Position += transformC.Forward * -movementComp.Speed;
                //        }
                //        if (actionState.Key.Equals("Backward") && actionState.Value.Equals(BUTTON_STATE.HELD))
                //        {
                //            movementComp.Speed += movementComp.Speed * (float)gametime.ElapsedGameTime.TotalSeconds * movementComp.Acceleration;
                //            if (movementComp.Speed > 20)
                //                movementComp.Speed = 20;
                //            transformC.Position += transformC.Forward * -movementComp.Speed;
                //        }
                //        if (actionState.Key.Equals("Right") && actionState.Value.Equals(BUTTON_STATE.HELD))
                //        {
                //            movementComp.Speed += movementComp.Speed * (float)gametime.ElapsedGameTime.TotalSeconds * movementComp.Acceleration;
                //            if (movementComp.Speed > 20)
                //                movementComp.Speed = 20;
                //            transformC.Position += transformC.Right * movementComp.Speed;
                //        }
                //        if (actionState.Key.Equals("Left") && actionState.Value.Equals(BUTTON_STATE.HELD))
                //        {
                //            movementComp.Speed += movementComp.Speed * (float)gametime.ElapsedGameTime.TotalSeconds * movementComp.Acceleration;
                //            if (movementComp.Speed > 20)
                //                movementComp.Speed = 20;
                //            transformC.Position += transformC.Right * -movementComp.Speed;
                //        }

                //        if (actionState.Key.Equals("TurnRight") && actionState.Value.Equals(BUTTON_STATE.PRESSED) || (actionState.Key.Equals("TurnRight") && actionState.Value.Equals(BUTTON_STATE.HELD)))
                //        {
                //            transformC.Rotation += new Vector3(0, -.1f, 0);
                //        }
                //        if (actionState.Key.Equals("TurnLeft") && actionState.Value.Equals(BUTTON_STATE.PRESSED) || (actionState.Key.Equals("TurnLeft") && actionState.Value.Equals(BUTTON_STATE.HELD)))
                //        {
                //            transformC.Rotation += new Vector3(0, .1f, 0);
                //        }
                //        if (actionState.Key.Equals("Up") && actionState.Value.Equals(BUTTON_STATE.PRESSED) || (actionState.Key.Equals("Up") && actionState.Value.Equals(BUTTON_STATE.HELD)))
                //        {
                //            transformC.Position += transformC.Up * movementComp.Speed;
                //        }
                //        if (actionState.Key.Equals("Down") && actionState.Value.Equals(BUTTON_STATE.PRESSED) || (actionState.Key.Equals("Down") && actionState.Value.Equals(BUTTON_STATE.HELD)))
                //        {
                //            transformC.Position += transformC.Up * -movementComp.Speed;
                //        }
                //        if (actionState.Key.Equals("LookUp") && actionState.Value.Equals(BUTTON_STATE.PRESSED) || (actionState.Key.Equals("LookUp") && actionState.Value.Equals(BUTTON_STATE.HELD)))
                //        {
                //            transformC.Rotation += new Vector3(0.025f, 0, 0);
                //        }
                //        if (actionState.Key.Equals("LookDown") && actionState.Value.Equals(BUTTON_STATE.PRESSED) || (actionState.Key.Equals("LookDown") && actionState.Value.Equals(BUTTON_STATE.HELD)))
                //        {
                //            transformC.Rotation += new Vector3(-0.025f, 0, 0);
                //        }

                //    }

                //}
            }
        }
        public void HandleInput(GameTime gametime, ComponentManager componentManager)
        {
            Dictionary<Entity, IComponent> kComponents = componentManager.GetComponents<KeyBoardComponent>();


            foreach (KeyValuePair<Entity, IComponent> component in kComponents)
            {
                KeyBoardComponent keyboardComp = (KeyBoardComponent)component.Value;
                TransformComponent tc = componentManager.GetComponent<TransformComponent>(component.Key);
                MovementComponent mc = componentManager.GetComponent<MovementComponent>(component.Key);
                PhysicsComponent pc = componentManager.GetComponent<PhysicsComponent>(component.Key);
                ModelTransformComponent modelC = componentManager.GetComponent<ModelTransformComponent>(component.Key);

                foreach (KeyValuePair<string, BUTTON_STATE> actionState in keyboardComp.ActionStates)
                {
                    if (actionState.Key.Equals("Forward") && actionState.Value.Equals(BUTTON_STATE.PRESSED))
                    {

                        var position = tc.Position += tc.Forward * mc.Speed;

                        var result = InsideMapControll(position, componentManager);
                        tc.Position = result;
                        GetYCoordinate(gametime, componentManager, tc);

                        var meshtransform = modelC.GetTranform("Body");
                        meshtransform.Rotation += tc.Forward * mc.Speed;
                    }
                    if (actionState.Key.Equals("Forward") && actionState.Value.Equals(BUTTON_STATE.HELD))
                    {
                        mc.Speed += (float)gametime.ElapsedGameTime.TotalSeconds * mc.Speed * mc.Acceleration;
                        if (mc.Speed > 3)
                            mc.Speed = 3;

                        var position = tc.Position += tc.Forward * mc.Speed;
                        var result = InsideMapControll(position, componentManager);
                        tc.Position = result;
                        GetYCoordinate(gametime, componentManager, tc);

                        var meshtransform = modelC.GetTranform("Body");
                        meshtransform.Rotation += tc.Forward * mc.Speed;
                    }

                    if (actionState.Key.Equals("Forward") && actionState.Value.Equals(BUTTON_STATE.RELEASED))
                    {
                        mc.Speed = 1;
                        //GetYCoordinate(gametime, componentManager, tc);
                    }

                    if (actionState.Key.Equals("Backward") && actionState.Value.Equals(BUTTON_STATE.PRESSED))
                    {
                        var position = tc.Position += tc.Forward * -mc.Speed;
                        var result = InsideMapControll(position, componentManager);
                        tc.Position = result;
                        GetYCoordinate(gametime, componentManager, tc);
               
                        var meshtransform = modelC.GetTranform("Body");
                        meshtransform.Rotation += tc.Forward * mc.Speed;

                    }
                    if (actionState.Key.Equals("Backward") && actionState.Value.Equals(BUTTON_STATE.HELD))
                    {
                        mc.Speed += (float)gametime.ElapsedGameTime.TotalSeconds * mc.Speed * mc.Acceleration;
                        if (mc.Speed > 1.5f)
                            mc.Speed = 1.5f;

                        var position = tc.Position += tc.Forward * -mc.Speed;
                        var result = InsideMapControll(position,componentManager);
                        tc.Position = result;
                        GetYCoordinate(gametime, componentManager, tc);

                        var meshtransform = modelC.GetTranform("Body");
                        meshtransform.Rotation += tc.Forward * mc.Speed;
                    }
                    if (actionState.Key.Equals("Backward") && actionState.Value.Equals(BUTTON_STATE.RELEASED))
                    {
                        mc.Speed = 2;
                        //GetYCoordinate(gametime, componentManager, tc);
                    }
                    if (actionState.Key.Equals("Right") && actionState.Value.Equals(BUTTON_STATE.PRESSED) || (actionState.Key.Equals("Right") && actionState.Value.Equals(BUTTON_STATE.HELD)))
                    {
                        tc.Rotation += new Vector3(0, -.1f, 0);
                    }
                    if (actionState.Key.Equals("Left") && actionState.Value.Equals(BUTTON_STATE.PRESSED) || (actionState.Key.Equals("Left") && actionState.Value.Equals(BUTTON_STATE.HELD)))
                    {
                        tc.Rotation += new Vector3(0, .1f, 0);
                    }

                }

            }
        }

        public Vector3 InsideMapControll(Vector3 position, ComponentManager componentManager)
        {
            Dictionary<Entity, IComponent> hComponents = componentManager.GetComponents<HeightmapComponent>();
            foreach (var heightMapEntity in hComponents)
            {
                var heightmapComponent =
                    componentManager.GetComponent<HeightmapComponent>(heightMapEntity.Key);

                if (position.X < 0)
                    position.X = 0;
                if (position.X > heightmapComponent.Width)
                    position.X = heightmapComponent.Width-1;
                if (position.Z > 0)
                    position.Z = 0;
                if (position.Z < -heightmapComponent.Height)
                    position.Z = -heightmapComponent.Height+1;
            }
            return position;
        }

        public void GetYCoordinate(GameTime gametime, ComponentManager componentManager, TransformComponent transformComponent)
        {
            Dictionary<Entity, IComponent> hComponents = componentManager.GetComponents<HeightmapComponent>();
            foreach (KeyValuePair<Entity, IComponent> heightMapPair in hComponents)
            {
                HeightmapComponent heightmapComponent = componentManager.GetComponent<HeightmapComponent>(heightMapPair.Key);
                foreach (HeightmapChunkComponent chunk in heightmapComponent.HeightmapChunkComponents)
                {
                    if (transformComponent.Position.X >= chunk.ChunkX &&
                    transformComponent.Position.Z * -1 >= chunk.ChunkY &&
                    transformComponent.Position.X <= (chunk.ChunkX + chunk.ChunkWidth) &&
                    transformComponent.Position.Z * -1 <= (chunk.ChunkY + chunk.ChunkHeight))
                    {
                        transformComponent.Position = new Vector3((transformComponent.Position.X),
                            chunk.HeightData[(int)transformComponent.Position.X - chunk.ChunkX, (((int)transformComponent.Position.Z *-1)- chunk.ChunkY)], transformComponent.Position.Z);
                    }
                }
                    
            }
        }
    }
}
