using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using P1_Pong;

public class Scene
{
    HashSet<GameObject> _objects;
    public virtual void Init()
    {
        //Load all your stuff here
    }
    public virtual void Draw(SpriteBatch spriteBatch)
    {
        foreach (GameObject o in _objects)
        {
            
        }
    }

    public virtual void Update(GameTime gameTime)
    {
        //Stuff gets done here
    }
}



