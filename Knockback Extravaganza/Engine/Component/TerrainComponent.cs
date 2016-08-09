using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECS_Engine.Engine.Component.Interfaces;
using Microsoft.Xna.Framework.Graphics;

namespace ECS_Engine.Engine.Component
{
    public class TerrainComponent: IComponent
    {
        public float[,] VertexHeight { get; set; }
        public float MaxHeight { get; set; }
        public Texture2D HeightMap { get; set; }
        public float CellSize { get; set; }
        public int Width { get; set; }
        public int Length { get; set; }
        public int Height { get; set; }
    }
}
