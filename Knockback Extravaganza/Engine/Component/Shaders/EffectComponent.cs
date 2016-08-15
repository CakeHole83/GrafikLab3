using ECS_Engine.Engine.Component.Interfaces;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECS_Engine.Engine.Component.Shaders
{
    public class EffectComponent : IComponent
    {  
        public Texture2D Normalmap { get; set; }
        public TextureCube SkyboxTexture { get; set; }
        public Dictionary<Effect, List<string>> Effects { get; set; }        
    }
}
