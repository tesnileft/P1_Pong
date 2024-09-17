using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using P1_Pong;
using TestLib.Helper;

public class GameScreen : Scene
{
    ContentManager _content;
    Game _game;
    
    private Vector2 ballDir;
    private Rectangle ballRect = new (new Point(0,0),new Point(30, 30));
    

    public GameScreen(ContentManager content, Game game)
    {
        _content = content;
        _game = game;
    }
    public void LoadContent(ContentManager content, Game game)
    {
        
    }
    public void Initialize()
    {
        new Paddle();
    }
    
    public override void Draw(SpriteBatch spriteBatch)
    {
        
    }
    public override void Update(GameTime gameTime)
    {
        
    
    }

    class Paddle
    {
        
    }
    
}
public class Scene
{
    HashSet<GameObject> _objects;
    public virtual void Init()
    {
        //Load all your stuff here
    }
    public virtual void Draw(SpriteBatch spriteBatch)
    {

    }

    public virtual void Update(GameTime gameTime)
    {


    }
}

public class MenuScreen : Scene
{
        private UI homeUI;
        private List<CpuParticleManager> _particles;
        
        void Init(Texture2D texture)
        {
            _particles = new List<CpuParticleManager>();
            _particles.Add(new CpuParticleManager(texture));
        }

        public MenuScreen(UI.Button[] buttons, Texture2D tex)
        {
            homeUI = new (buttons);
            Init(tex);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            homeUI.Draw(spriteBatch);
            
            foreach (var particleSystem in _particles)
            {
                particleSystem.Draw(spriteBatch);
            }
        }

        public override void Update(GameTime gameTime)
        {
            homeUI.Update(gameTime);
                
            foreach (CpuParticleManager manager in _particles)
            {
                manager.Update(gameTime);
            }
        }
    }

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
            if (Mouse.GetState().LeftButton == ButtonState.Pressed && _mousePrev.LeftButton == ButtonState.Released && _buttonClickable)
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