using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace P1_Pong;


public abstract class Paddle
{
    protected GameScreen _parent;
    protected Texture2D _sprite;
    public Rectangle Rect;
    protected int _moveSpeed = 8;
    public bool Axis;
    private UI.UI.TextElement _livesDisplay;
    public int LifeCount = 3;
    Color spriteColor = Color.White;
    
    Vector2 lifeOffset = new (10,20);

    protected Paddle(GameScreen parent, Texture2D sprite, Vector2 pos, Vector2 size, SpriteFont livesFont, bool onXaxis = false)
    {
        _parent = parent;
        Rect = new(pos.ToPoint(), size.ToPoint());
        this._sprite = sprite;
        Axis = onXaxis;
    
        _livesDisplay = new(LifeCount.ToString(),livesFont, Rect);
    }
    public void Draw(SpriteBatch spriteBatch)
    {
        if (LifeCount == 0)
        {
            spriteColor = Color.Gray;
        }
        spriteBatch.Draw(_sprite, Rect, spriteColor);
        _livesDisplay.Text = LifeCount.ToString();
        _livesDisplay.Position = Rect.Location + lifeOffset.ToPoint();
        _livesDisplay.Draw(spriteBatch);
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
    
    public PlayerPaddle(GameScreen parent, Texture2D sprite, Vector2 pos, Vector2 size, Keys left, Keys right, SpriteFont livesFont, bool onXaxis = false) : base(parent, sprite, pos, size, livesFont, onXaxis)
    {
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
    public AiPaddle(GameScreen parent, Texture2D sprite, Vector2 pos, Vector2 size, SpriteFont livesFont, bool onXaxis = false) : base(parent, sprite,  pos, size, livesFont, onXaxis)
    {
        
        
    }

    public override void Update(GameTime gameTime, GameWindow window)
    {
        //Woo complex AI stuff wowowowowowowowowowow
        
    }
}