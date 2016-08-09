using ECS_Engine.Engine.Component.Interfaces;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECS_Engine.Engine.Component.Shaders
{
    public class CubemapComponent : IComponent
    {
        public RenderTargetCube renderTargetCube { get; set; }
        public RenderTarget2D renderTarget { get; set; }
        public TextureCube textureCube { get; set; }
        public bool RenderTextCube { get; set; }

    }
}
