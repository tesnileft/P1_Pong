using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace P1_Pong.UI;

public class UI
{
    private UiElement[] _elements;

    public UI(UiElement[] elements)
    {
        _elements = elements;
    }
    public void Update(GameTime gameTime)
    {
        foreach (UiElement b in _elements)
        {
            b.Update(gameTime);
        }
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        foreach (UiElement b in _elements)
        {
            b.Draw(spriteBatch);
        }
    }

    public abstract class UiElement
    {
        public Point Position;
        public Point Size;

        void SetPos(Point newPosition)
        {
            Position = newPosition;
        }
        public abstract void Draw(SpriteBatch spriteBatch);
        public abstract void Update(GameTime gameTime);
    }

    public class TextElement : UiElement
    {
        public string Text { get; set; }
        public SpriteFont Font;

        public TextElement(string text, SpriteFont font, Rectangle rect)
        {
            Text = text;
            Font = font;
            Size = rect.Size;
            Position = rect.Location;
        }

        public override void Update(GameTime gameTime)
        {
            //
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.DrawString(Font, Text, Position.ToVector2(), Color.White);
        }

    }
    

    public class Button : UiElement
    {
        bool _buttonHover;
        private bool _clicked;
        private bool _buttonClickable = true;
        private string _textureName;
        Texture2D _buttonTexture;
        Texture2D _highlightTexture;
        MouseState _mousePrev = Mouse.GetState();
        
        public event EventHandler ButtonDown;
        
        public Button(Vector2 positionV, Vector2 sizeV, Texture2D texture, Texture2D highlightTexture) : this(positionV,
            sizeV, texture)
        {

            this._highlightTexture = highlightTexture;
        }

        public Button(Vector2 positionV, Vector2 sizeV, Texture2D texture)
        {
            this.Position = positionV.ToPoint();
            this.Size = sizeV.ToPoint();
            this._buttonTexture = texture;
        }

        public Button(Point position, Point size, Texture2D texture)
        {
            this.Position = position;
            this.Size = size;
            _buttonTexture = texture;
        }

        public void Load(Game game)
        {
            _buttonTexture = game.Content.Load<Texture2D>(_textureName);
        }

        public override void Update(GameTime gameTime)
        {
            CheckHover(Mouse.GetState().Position);
            if (Mouse.GetState().LeftButton == ButtonState.Pressed && _mousePrev.LeftButton == ButtonState.Released && _buttonClickable && _buttonHover)
            {
                //Yay! button got clicked
                ButtonDown?.Invoke(this, EventArgs.Empty);
            }
            _mousePrev = Mouse.GetState();
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_buttonTexture, new Rectangle(Position, Size), Color.White);
            if (_buttonHover && _highlightTexture != null)
            {
                spriteBatch.Draw(_highlightTexture, new Rectangle(Position, Size), Color.White);
            }
        }

        bool CheckHover(Point mousePos)
        {
            if (mousePos.X > Position.X && mousePos.X < Position.X + Size.X && mousePos.Y > Position.Y &&
                mousePos.Y < Position.Y + Size.Y)
            {
                //The mouse is on the button!!!
                _buttonHover = true;
            }
            else
            {
                _buttonHover = false;
            }

            return _buttonHover;
        }
    }
}