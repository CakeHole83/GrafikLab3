using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Schema;
using ECS_Engine.Engine.Managers;
using ECS_Engine;
using ECS_Engine.Engine.Component;
using ECS_Engine.Engine.Component.Interfaces;
using ECS_Engine.Engine.Systems.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ECS_Engine.Engine.Systems
{
    public class TerrainSystem : ISystem
    {
        public TerrainSystem(ComponentManager componentManager)
        {

            Dictionary<Entity, IComponent> components = componentManager.GetComponents<TerrainComponent>();
            foreach (KeyValuePair<Entity, IComponent> c in components)
            {
                var terrainComponent = componentManager.GetComponent<TerrainComponent>(c.Key);
                //Kalla på alla get funktioner som ligger under för att sätta componenterna.(Endast en component i vårt fall)
            }

        }

        private void getHeights(TerrainComponent terrainComponent)
        {
            Color[] heightMapData = new Color[terrainComponent.Width*terrainComponent.Length];
            terrainComponent.HeightMap.GetData<Color>(heightMapData);

            terrainComponent.VertexHeight = new float[terrainComponent.Width, terrainComponent.Length];

            for (int y = 0; y < terrainComponent.Length; y++)
            {
                for (int x = 0; x < terrainComponent.Width; x++)
                {
                    float amt = heightMapData[y*terrainComponent.Width + x].R;
                    amt /= 255.0f;
                    terrainComponent.VertexHeight[x, y] = amt*terrainComponent.Height;
                }
            }
        }

        private void createCertices(TerrainComponent terrainComponent)
        {
            VertexPositionNormalTexture[] vertices = new VertexPositionNormalTexture[terrainComponent.Width*terrainComponent.Length];

        }

    }
}
