using ECS_Engine.Engine.Component.Interfaces;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECS_Engine.Engine.Component.Shaders
{
    public class SkyboxShaderComponent : IComponent
    {

        public Model Skybox { get; set; }
        public TextureCube SkyboxTexture { get; set; }
        public Effect SkyboxEffect { get; set; }
        public float size { get; set; }
        
    }
}
