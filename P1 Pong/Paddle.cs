using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TestLib.Helper;
namespace P1_Pong;


public abstract class Paddle
{
    protected GameScreen _parent;
    protected Texture2D _sprite;
    public Rectangle Rect;
    protected int _moveSpeed = 8;

    protected Vector2 facingVec;
    public FacingDir Directional;
    private TextElement _livesDisplay;
    public int LifeCount = 3;
    Color spriteColor = Color.White;
    public string Name = "player";
    
    Vector2 lifeOffset = new (15,20);

    public enum FacingDir
    {
        Up = 1,
        Right = 2,
        Down = 3,
        Left = 4
    }
    
    protected Paddle(string name, GameScreen parent, Texture2D sprite, Vector2 pos, Vector2 size, SpriteFont livesFont, FacingDir dir)
    {
        Name = name;
        _parent = parent;
        Rect = new(pos.ToPoint(), size.ToPoint());
        this._sprite = sprite;
        Directional = dir;
        switch (Directional)
        {
            case FacingDir.Up:
                lifeOffset = new(Rect.Width / 2, 15);
                break;
            case FacingDir.Right:
                lifeOffset = new(-15, Rect.Height / 2);
                break;
            case FacingDir.Left:
                lifeOffset = new(Rect.Width + 15, Rect.Height / 2);
                break;
        }
            
        
    
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
    
    public PlayerPaddle(string name, GameScreen parent, Texture2D sprite, Vector2 pos, Vector2 size, Keys left, Keys right, SpriteFont livesFont, FacingDir facing) 
        : base(name, parent, sprite, pos, size, livesFont, facing)
    {
        _leftkey = left;
        _rightkey = right;
    }
    
    public override void Update(GameTime gameTime, GameWindow window)
    {
        //Check keydowns and move the paddle
        if (Keyboard.GetState().IsKeyDown(_leftkey))
        {
            switch (Directional)
            {
                case FacingDir.Up:
                    
                    break;
                case FacingDir.Right:
                    Rect.Y -= _moveSpeed;
                    break;
                case FacingDir.Down:
                    
                    break;
                case FacingDir.Left:
                    Rect.Y -= _moveSpeed;
                    break;
            }
        }
            
        if (Keyboard.GetState().IsKeyDown(_rightkey))
        {
            switch (Directional)
            {
                case FacingDir.Up:
                    
                    break;
                case FacingDir.Right:
                    Rect.Y += _moveSpeed;
                    break;
                case FacingDir.Down:
                    
                    break;
                case FacingDir.Left:
                    Rect.Y += _moveSpeed;
                    break;
            }
        }
        //Constrain to playable space
        switch (Directional)
        {
            case FacingDir.Left:
            case FacingDir.Right:
                if (Rect.Y < 0)
                {
                    Rect.Y = 0;
                }

                if (Rect.Y + Rect.Height > window.ClientBounds.Height)
                {
                    Rect.Y = window.ClientBounds.Height - Rect.Height;
                }
                break;
            case FacingDir.Down:
            case FacingDir.Up:
                    //TODO constrain left/right movement
                break;
        }
        
    }
}

public class AiPaddle : Paddle
{
    public AiPaddle(string name, GameScreen parent, Texture2D sprite, Vector2 pos, Vector2 size, SpriteFont livesFont,
        FacingDir dir) : base(name, parent, sprite,  pos, size, livesFont, dir)
    {
        
        
    }

    public override void Update(GameTime gameTime, GameWindow window)
    {
        //Woo complex AI stuff wowowowowowowowowowow
        
    }
}