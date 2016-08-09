
using System.Diagnostics.Eventing.Reader;
using ECS_Engine.Engine.Component.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ECS_Engine.Engine.Component
{
    public class HeightmapChunkComponent : IComponent
    {
        public float[,] HeightData { get; set; }
        public Color[] Colors { get; set; }
        public VertexPositionColorNormal[] Vertices { get; set; }
        public int ChunkX { get; set; }
        public int[] Indices { get; set; }
        public int ChunkY { get; set; }
        public int ChunkHeight { get; set; }
        public int ChunkWidth { get; set; }

        public HeightmapChunkComponent(Color[] colors, int chunkWidth, int chunkHeight)
        {

            ChunkWidth = chunkWidth;
            ChunkHeight = chunkHeight;
            Colors = colors;
            HeightData = new float[ChunkWidth, ChunkHeight];

            for (var x = 0; x < ChunkWidth; x++)
            {
                for (var y = 0; y < ChunkHeight; y++)
                {
                    HeightData[x, y] = Colors[x + y*ChunkWidth].R/8f;
                }
            }
            CreateVertices();
            CreateIndices();
            CreateNormals();
        }

        private void CreateVertices()
        {
            var minHeight = float.MaxValue;
            var maxHeight = float.MinValue;
            for (var x = 0; x < ChunkWidth; x++)
            {
                for (var y = 0; y < ChunkHeight; y++)
                {
                    if (HeightData[x, y] < minHeight)
                        minHeight = HeightData[x, y];
                    if (HeightData[x, y] > maxHeight)
                        maxHeight = HeightData[x, y];
                }
            }

            Vertices = new VertexPositionColorNormal[ChunkWidth*ChunkHeight];

            for (var x = 0; x < ChunkWidth; x++)
            {
                for (var y = 0; y < ChunkHeight; y++)
                {
                    Vertices[x + y*ChunkWidth].Position = new Vector3(x, HeightData[x, y], -y);

                    if (HeightData[x, y] < minHeight + (maxHeight - minHeight)/4)
                        Vertices[x + y*ChunkWidth].Color = Color.SandyBrown;
                    else if (HeightData[x, y] < minHeight + (maxHeight - minHeight)*2/4)
                        Vertices[x + y*ChunkWidth].Color = Color.BurlyWood;
                    else if (HeightData[x, y] < minHeight + (maxHeight - minHeight)*3/4)
                        Vertices[x + y*ChunkWidth].Color = Color.SandyBrown;
                    else
                        Vertices[x + y*ChunkWidth].Color = Color.Chocolate;
                }
            }
        }

        private void CreateIndices()
        {
            Indices = new int[(ChunkWidth - 1)*(ChunkHeight - 1)*6];
            var counter = 0;

            for (var x = 0; x < ChunkHeight - 1; x++)
            {
                for (var y = 0; y < ChunkWidth - 1; y++)
                {
                    var lowerLeft = x + y*ChunkWidth;
                    var lowerRight = (x + 1) + y*ChunkWidth;
                    var topLeft = x + (y + 1)*ChunkWidth;
                    var topRight = (x + 1) + (y + 1)*ChunkWidth;

                    Indices[counter++] = topLeft;
                    Indices[counter++] = lowerRight;
                    Indices[counter++] = lowerLeft;

                    Indices[counter++] = topLeft;
                    Indices[counter++] = topRight;
                    Indices[counter++] = lowerRight;
                }
            }
        }

        private void CreateNormals()
        {
            for (var i = 0; i < Vertices.Length; i++)
                Vertices[i].Normal = new Vector3(0, 0, 0);


            for (var i = 0; i < Indices.Length/3; i++)
            {
                var index1 = Indices[i*3];
                var index2 = Indices[i*3 + 1];
                var index3 = Indices[i*3 + 2];

                var side1 = Vertices[index1].Position - Vertices[index3].Position;
                var side2 = Vertices[index1].Position - Vertices[index2].Position;
                var normal = Vector3.Cross(side1, side2);

                Vertices[index1].Normal += normal;
                Vertices[index2].Normal += normal;
                Vertices[index3].Normal += normal;

            }

            for (var i = 0; i < Vertices.Length; i++)
                Vertices[i].Normal.Normalize();
        }
    }
}
