using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using ECS_Engine.Engine.Component.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ECS_Engine.Engine.Component
{
    public class HeightmapComponent:IComponent
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public HeightmapChunkComponent[] HeightmapChunkComponents { get; set; }
        public VertexPositionColorNormal[] Vertices { get; set; }
        public int[] Indices { get; set; }
        public int ChunkWidth { get; set; }
        public int ChunkHeight { get; set; }
        public int NumberOfChunksPerRow { get; set; }
        public HeightmapComponent(Texture2D texture, int numberOfChunksPerRow)
        {
            Width = texture.Width;
            Height = texture.Height;
            NumberOfChunksPerRow = numberOfChunksPerRow;
            ChunkHeight = (texture.Height-1)/numberOfChunksPerRow;
            ChunkWidth = (texture.Width-1)/numberOfChunksPerRow;
            HeightmapChunkComponents = new HeightmapChunkComponent[NumberOfChunksPerRow*NumberOfChunksPerRow];
            SplitTextureIntoChunks(texture);

        }
        public void SplitTextureIntoChunks(Texture2D texture)
        {
            var chunkIndex = 0;
        
     

            for (var x = 0; x < Width - ChunkWidth; x += ChunkWidth)
            {
                for (var y = 0; y < Height - ChunkHeight; y += ChunkHeight)
                {
                    var colors = new Color[(ChunkWidth+1)*(ChunkHeight+1)];
                    
                    texture.GetData(0,new Rectangle(x, y, ChunkWidth+1, ChunkHeight+1), colors, 0, (ChunkWidth+1)*(ChunkHeight+1));
                    HeightmapChunkComponents[chunkIndex] = new HeightmapChunkComponent(colors, ChunkWidth+1, ChunkHeight+1)
                    {
                        ChunkX = x,
                        ChunkY = y
                    };
                
                    chunkIndex++;
                }
            }
        }
        /*
        public void GetVertices()
        {
            List<VertexPositionColorNormal> tempVertices = new List<VertexPositionColorNormal>();
            foreach (HeightmapChunkComponent chunk in HeightmapChunkComponents)
            {
                tempVertices.AddRange(chunk.Vertices);
            }
            Vertices = tempVertices.ToArray();
        }



            int startV = 0;

            foreach (HeightmapChunkComponent chunk in HeightmapChunkComponents)
            {
                for (int i = 0; i < chunk.Vertices.Length; i++)
                {
                    chunk.Vertices[i] = Vertices[startV + i];
                }
                startV += chunk.Vertices.Length;
            }
        }*/


    }
}

