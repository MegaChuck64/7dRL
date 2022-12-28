using Microsoft.Xna.Framework.Graphics;

namespace Engine.Core;

public interface IComponent
{
    public GameObject Owner { get; set; }
    public bool IsActive { get; set; }

    public void Start();
    public void Update(float dt);
    public void Draw(SpriteBatch sb);
}