using ECS_Engine.Engine.Component;
using ECS_Engine.Engine.Component.Interfaces;
using ECS_Engine.Engine.Managers;
using ECS_Engine.Engine.Systems.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace ECS_Engine.Engine
{
    public class HeightmapSystem : ISystem
    {
        private GraphicsDevice Graphics;

        public HeightmapSystem(ComponentManager componentManager, GraphicsDevice graphics)
        {
            Initialize(componentManager);
            Graphics = graphics;
        }

        public void Initialize(ComponentManager componentManager)
        {

            Dictionary<Entity, IComponent> components = componentManager.GetComponents<HeightMapComponent>();

            if (components != null)
            {
                foreach (KeyValuePair<Entity, IComponent> comp in components)
                {
                    HeightMapComponent hc = componentManager.GetComponent<HeightMapComponent>(comp.Key);
                    Entity entity = comp.Key;
                    CreateHeightData(comp.Key, hc);
                    CreateVertices(comp.Key,componentManager, hc);
                    CreateIndices(comp.Key, componentManager, hc);
                    GenerateeNormals(comp.Key, componentManager, hc);
                    CreateBuffers(comp.Key,componentManager, hc);
                }
            }
        }

        private void CreateBuffers(Entity e, ComponentManager cpm, HeightMapComponent hc)
        {
            Dictionary<Entity, IComponent> components = cpm.GetComponents<VertexComponent<VertexPositionColorNormal>>();
            if (components != null)
            {
                foreach (KeyValuePair<Entity, IComponent> comp in components)
                {
                    var vertexC = cpm.GetComponent<VertexComponent<VertexPositionColorNormal>>(comp.Key);
                    var vertexBuffer = new VertexBufferComponent<VertexPositionColorNormal>(vertexC.Vertices, Graphics);
                    cpm.AddComponent(e, vertexBuffer);

                    var indexC = cpm.GetComponent<VertexIndexComponent<VertexPositionColorNormal>>(comp.Key);
                    var indexBuffer = new VertexIndexBufferComponent<VertexPositionColorNormal>(vertexC.Vertices, indexC.Indices, Graphics);


                    cpm.AddComponent(e, indexBuffer);
                }
            }

        }

        private void CreateVertices(Entity e, ComponentManager cpm, HeightMapComponent hmc)
        {
            var vertexC = new VertexComponent<VertexPositionColorNormal>();
           
            var minHeight = float.MaxValue;
            var maxHeight = float.MinValue;
            for (var x = 0; x < hmc.Width; x++)
            {
                for (var y = 0; y < hmc.Height; y++)
                {
                    if (hmc.HeightData[x, y] < minHeight)
                        minHeight = hmc.HeightData[x, y];
                    if (hmc.HeightData[x, y] > maxHeight)
                        maxHeight = hmc.HeightData[x, y];
                }
            }
            vertexC.Vertices = new VertexPositionColorNormal[hmc.Width * hmc.Height];
            //hmc.Vertices = new HeightMapComponent.VertexPositionColorNormal[hmc.Width * hmc.Height];
            for (var x = 0; x < hmc.Width; x++)
            {
                for (var y = 0; y < hmc.Height; y++)
                {
                     vertexC.Vertices[x + y * hmc.Width].Position = new Vector3(x, hmc.HeightData[x, y], -y);

                    if (hmc.HeightData[x, y] < minHeight + (maxHeight - minHeight) / 4)
                        vertexC.Vertices[x + y * hmc.Width].Color = Color.SandyBrown;
                    else if (hmc.HeightData[x, y] < minHeight + (maxHeight - minHeight) * 2 / 4)
                        vertexC.Vertices[x + y * hmc.Width].Color = Color.BurlyWood;
                    else if (hmc.HeightData[x, y] < minHeight + (maxHeight - minHeight) * 3 / 4)
                        vertexC.Vertices[x + y * hmc.Width].Color = Color.SandyBrown;
                    else
                        vertexC.Vertices[x + y * hmc.Width].Color = Color.BurlyWood;
                }
            }
            cpm.AddComponent(e, vertexC);
        }

        private void CreateIndices(Entity e, ComponentManager cpm, HeightMapComponent hmc)
        {
            var indexC = new VertexIndexComponent<VertexPositionColorNormal>();
            indexC.Indices = new int[(hmc.Width - 1) * (hmc.Height - 1) * 6];
            var i = 0;
            for (var y = 0; y < hmc.Height - 1; y++)
            {
                for (var x = 0; x < hmc.Width - 1; x++)
                {
                    var lowerLeft = x + y * hmc.Width;
                    var lowerRight = (x + 1) + y * hmc.Width;
                    var topLeft = x + (y + 1) * hmc.Width;
                    var topRight = (x + 1) + (y + 1) * hmc.Width;

                    indexC.Indices[i++] = topLeft;
                    indexC.Indices[i++] = lowerRight;
                    indexC.Indices[i++] = lowerLeft;

                    indexC.Indices[i++] = topLeft;
                    indexC.Indices[i++] = topRight;
                    indexC.Indices[i++] = lowerRight;
                }
            }
            cpm.AddComponent(e, indexC);
        }

        private void GenerateeNormals(Entity e, ComponentManager cpm, HeightMapComponent hmc)
        {
            Dictionary<Entity, IComponent> components = cpm.GetComponents<VertexIndexComponent<VertexPositionColorNormal>>();
            if (components != null)
            {
                foreach (KeyValuePair<Entity, IComponent> comp in components)
                {
                    var indexC = cpm.GetComponent<VertexIndexComponent<VertexPositionColorNormal>>(comp.Key);
                    var vertexC = cpm.GetComponent<VertexComponent<VertexPositionColorNormal>>(comp.Key);
                    for (var i = 0; i < vertexC.Vertices.Length; i++)
                        vertexC.Vertices[i].Normal = new Vector3(0, 0, 0);

                    for (var i = 0; i < indexC.Indices.Length / 3; i++)
                    {
                        var index1 = indexC.Indices[i * 3];
                        var index2 = indexC.Indices[i * 3 + 1];
                        var index3 = indexC.Indices[i * 3 + 2];

                        var side1 = vertexC.Vertices[index1].Position - vertexC.Vertices[index3].Position;
                        var side2 = vertexC.Vertices[index1].Position - vertexC.Vertices[index2].Position;
                        var normal = Vector3.Cross(side1, side2);
                        normal.Normalize();

                        vertexC.Vertices[index1].Normal += normal;
                        vertexC.Vertices[index2].Normal += normal;
                        vertexC.Vertices[index3].Normal += normal;
                    }

                    for (var i = 0; i < vertexC.Vertices.Length; i++)
                        vertexC.Vertices[i].Normal.Normalize();
                }
            }
        }

        private void CreateHeightData(Entity e, HeightMapComponent hmc)
        {
            hmc.Width = hmc.Texture.Width;
            hmc.Height = hmc.Texture.Height;

            var heightMapColors = new Color[hmc.Width * hmc.Height];
            hmc.Texture.GetData(heightMapColors);

            hmc.HeightData = new float[hmc.Width, hmc.Height];
            for (var x = 0; x < hmc.Width; x++)
                for (var y = 0; y < hmc.Height; y++)
                    hmc.HeightData[x, y] = heightMapColors[x + y * hmc.Width].R / 4f;
        }
    }
}
