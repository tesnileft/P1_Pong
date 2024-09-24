using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace P1_Pong;

public abstract class Paddle
{
    protected GameScreen _parent;
    protected Texture2D _sprite;
    public Rectangle Rect;
    protected int _moveSpeed = 5;
    public bool Axis;
    public void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(_sprite, Rect, Color.White);
    }

    public abstract void Update(GameTime gameTime, GameWindow window);
    
    void SetSpeed(int speed)
    {
        _moveSpeed = speed;
    }
}

public class PlayerPaddle : Paddle
{
    private Keys _leftkey;
    private Keys _rightkey;
    
    public PlayerPaddle(GameScreen parent, Texture2D sprite, Vector2 pos, Vector2 size, Keys left, Keys right, bool onXaxis = false)
    {
        _parent = parent;
        Rect = new(pos.ToPoint(), size.ToPoint());
        _sprite = sprite;
        Axis = onXaxis;
        _leftkey = left;
        _rightkey = right;
    }
    
    public override void Update(GameTime gameTime, GameWindow window)
    {
        //Check keydowns
        if (Keyboard.GetState().IsKeyDown(_leftkey))
        {
            if (!Axis)
            {
                Rect.Y -= _moveSpeed;
            }
            else
            {
                Rect.X += _moveSpeed;
            }
                    
        }
            
        if (Keyboard.GetState().IsKeyDown(_rightkey))
        {
            if (!Axis)
            {
                Rect.Y += _moveSpeed;
            }
            else
            {
                Rect.X -= _moveSpeed;
            }
        }
        //Constrain to playable space
        if (Rect.Y < 0)
        {
            Rect.Y = 0;
        }

        if (Rect.Y + Rect.Height > window.ClientBounds.Height)
        {
            Rect.Y = window.ClientBounds.Height - Rect.Height;
        }
    }
    
}

public class AiPaddle : Paddle
{
    public AiPaddle(GameScreen parent, Texture2D sprite, Vector2 pos, Vector2 size, bool onXaxis = false)
    {
        _parent = parent;
        Rect = new(pos.ToPoint(), size.ToPoint());
        _sprite = sprite;
        Axis = onXaxis;
        
    }

    public override void Update(GameTime gameTime, GameWindow window)
    {
        //Woo complex AI stuff wowowowowowowowowowow
        
    }
    
}