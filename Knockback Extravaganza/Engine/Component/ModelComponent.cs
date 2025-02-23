﻿using ECS_Engine.Engine.Component.Interfaces;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace ECS_Engine.Engine.Component {
    public class ModelComponent : IComponent{
        public Model Model { get; set; }
        public Matrix[] Meshes { get; set; }
        public Texture2D Texture { get; set; }
        public bool HasTransparentMesh { get; set; } = false;
        public bool CastShadowOn { get; set; } = true;
        public bool ShadowsOn { get; set; } = true;
        public ModelComponent() {
            Texture = null;
        }
    }
}
