using _7dRL.RoomCreation;
using Engine.Core;
using Engine.Utilities;
using Microsoft.Xna.Framework.Graphics;
using SharpDX.Direct2D1.Effects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _7dRL.Spells;

public abstract class Spool : IComponent
{
    public GameObject Owner { get; set; }
    public bool IsActive { get; set; }

    public Map Map { get; set; }
    public SpriteAtlas Atlas { get; set; }
    public Spool(Map map, SpriteAtlas atlas, GameObject owner)
    {
        Owner = owner;
        IsActive = true;
        Map = map;
        Atlas = atlas;

    }

    public void Start()
    {
    }

    public void Update(float dt)
    {
    }
    public void Draw(SpriteBatch sb)
    {
    }

    public abstract void Cast();
}