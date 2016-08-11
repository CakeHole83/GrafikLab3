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
        const int shadowMapWidthHeight = 2048;
        public int ShadowMapWidthHeight{get;}
        Vector3 lightDirection = new Vector3(-0.3333333f, 0.6666667f, 0.6666667f);
        public Vector3 LightDirection { get { return lightDirection; } set { lightDirection = value; } }
        public Matrix LightViewProjection { get; set; }
        public Matrix LightRotation { get; set; }
        public Vector3[] FrustumCorners { get; set; }
        public RenderTarget2D ShadowRenderTarget { get; set; }
        public Effect ShadowEffect { get; set; }
    }
}
