using ECS_Engine.Engine.Component.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECS_Engine.Engine.Component.Shaders
{
    public class ShadowComponent : IComponent
    {
        public Vector3 LightDirection { get; set; } = new Vector3(-0.3333333f, 0.6666667f, 0.6666667f);
        public Matrix LightViewProjection { get; set; }
        public Matrix LightRotation { get; set; }
        public Vector3[] FrustumCorners { get; set; }
        public RenderTarget2D ShadowRenderTarget { get; set; }
        public SpriteBatch SpriteBatch { get; set; }
        public bool CastShadow { get; set; } = true;
    }
}
