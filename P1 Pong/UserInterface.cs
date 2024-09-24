using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace P1_Pong;

public class UI
{
    private Button[] _buttons;

    public UI(Button[] buttons)
    {
        _buttons = buttons;
    }
    public void Update(GameTime gameTime)
    {
        foreach (Button b in _buttons)
        {
            b.Update();
        }
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        foreach (Button b in _buttons)
        {
            b.Draw(spriteBatch);
        }
    }
    public class Button
    {
        bool _buttonHover;
        private bool _clicked;
        private bool _buttonClickable = true;
        private string _textureName;
        Texture2D _buttonTexture;
        Texture2D _highlightTexture;
        MouseState _mousePrev = Mouse.GetState();
        Point _position;
        Point _size;
        public event EventHandler ButtonDown;
        
        public Button(Vector2 positionV, Vector2 sizeV, Texture2D texture, Texture2D highlightTexture) : this(positionV,
            sizeV, texture)
        {

            this._highlightTexture = highlightTexture;
        }

        public Button(Vector2 positionV, Vector2 sizeV, Texture2D texture)
        {
            this._position = positionV.ToPoint();
            this._size = sizeV.ToPoint();
            this._buttonTexture = texture;
        }

        public Button(Point position, Point size, Texture2D texture)
        {
            this._position = position;
            this._size = size;
            _buttonTexture = texture;
        }

        public void Load(Game game)
        {
            _buttonTexture = game.Content.Load<Texture2D>(_textureName);
        }

        public void Update()
        {
            CheckHover(Mouse.GetState().Position);
            if (Mouse.GetState().LeftButton == ButtonState.Pressed && _mousePrev.LeftButton == ButtonState.Released && _buttonClickable && _buttonHover)
            {
                //Yay! button got clicked
                ButtonDown?.Invoke(this, EventArgs.Empty);
            }
            _mousePrev = Mouse.GetState();
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_buttonTexture, new Rectangle(_position, _size), Color.White);
            if (_buttonHover && _highlightTexture != null)
            {
                spriteBatch.Draw(_highlightTexture, new Rectangle(_position, _size), Color.White);
            }
        }

        bool CheckHover(Point mousePos)
        {
            if (mousePos.X > _position.X && mousePos.X < _position.X + _size.X && mousePos.Y > _position.Y &&
                mousePos.Y < _position.Y + _size.Y)
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