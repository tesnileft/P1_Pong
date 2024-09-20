using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;

using MonoGame.OpenGL;
using P1_Pong;
using TestLib.Helper;


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
public class GameScreen : Scene
{
    readonly public ContentManager Content;
    Game _game;

    private Texture2D _ballTex;
    private Texture2D _paddleTex;
    private Texture2D _weede;
    private bool _countdown = true;
    private int coundownMs = 3000;
    private Vector2 ballDir;
    private Rectangle ballRect;
    
    Paddle[] _paddles;
    

    public GameScreen(Game game)
    {
        Content = game.Content;
        _game = game;
        
        LoadContent();
    }
    public void LoadContent()
    {
        //_ballTex = Content.Load<Texture2D>("Ball");
        //_paddleTex = Content.Load<Texture2D>("Paddle");
        _weede = Content.Load<Texture2D>("Weede");
        
        //2 player game
        //Temp textures
        _paddleTex = _weede;
        _ballTex = _weede;
        
        _paddles = new Paddle[2];
        _paddles[0] = new Paddle(
            this,
            _paddleTex,
            new Vector2(20, _game.Window.ClientBounds.Height/2 - 100/2),
            new Vector2(10, 100),
            Keys.W,
            Keys.S
            );
        _paddles[1] = new Paddle(
            this,
            _paddleTex,
            new Vector2(_game.Window.ClientBounds.Width - 20, _game.Window.ClientBounds.Height/2 - 100/2),
            new Vector2(10, 100),
            Keys.Up,
            Keys.Down
        );
        int ballsize = 30;
        ballRect = new (new Point(_game.Window.ClientBounds.Width/2 -ballsize/2 ,_game.Window.ClientBounds.Height/2 - ballsize/2),new Point(ballsize));
        RandomizeBall();
    }
    
    public override void Draw(SpriteBatch spriteBatch)
    {
        Rectangle fullscreenRect = new( new Point(0,0),new Point(_game.Window.ClientBounds.Width,_game.Window.ClientBounds.Height));
        //Fullscreen weede :sunglassesvery3dcool:
        // spriteBatch.Draw(_weede, fullscreenRect, Color.White);
    
        //Draw the paddles. Peddls. Pad els. 
        foreach (Paddle p in _paddles)
        {
            p.Draw(spriteBatch);
        }
        //Draw ball 
        spriteBatch.Draw(_ballTex, ballRect, Color.White);
        
        
        
    }
    public override void Update(GameTime gameTime)
    {
        
        if (_countdown)
        {
            coundownMs -= gameTime.ElapsedGameTime.Milliseconds;
            if (coundownMs <= 0)
            {
                _countdown = false;
                coundownMs = 3000;
                RandomizeBall();
            }

            return;
        }
        
        //Paddle logic
        foreach (Paddle p in _paddles)
        {
            p.Update(gameTime, _game.Window);
        }
        
        //Ball logic (2P)
        int ballspeed = gameTime.ElapsedGameTime.Milliseconds / 3;
        ballRect.Location += (ballDir * ballspeed).ToPoint();
        if (ballRect.Y < 0 || ballRect.Y + ballRect.Height > _game.Window.ClientBounds.Height)
        {
            //Invert Y direction bc we hit the top/bottom of the screen
            ballDir.Y *= -1;
            //This wouldn't ever cause problems clearly
        }
        foreach (Paddle p in _paddles)
        {
            if (ballRect.Intersects(p.Rect))
            {
                //Do angle based on where center is?
                ballDir.X *= -1;
            }
            
        }

        if (ballRect.X < 0)
        {
            //Right point
            
            ResetBall();
        }

        if (ballRect.X + ballRect.Width > _game.Window.ClientBounds.Width)
        {
            //Left point
            
            ResetBall();
        }
        
        
    }

    private void ResetBall()
    {
        ballDir = Vector2.Zero;
        ballRect.Location = new Point(_game.Window.ClientBounds.Width / 2, _game.Window.ClientBounds.Height / 2);
        _countdown = true;
    }

    private void RandomizeBall()
    {
        if (new Random().Next(1) == 1)
        {
            //Go right
            ballDir.X = -1;
        }
        else
        {
            ballDir.X = 1;
        }
        ballDir.Y = new Random().Next(1) == 1 ? 1 : -1;
        
    }

    class Paddle
    {
        private GameScreen _parent;
        private Texture2D _sprite;
        public Rectangle Rect;

        private Keys _leftkey;
        private Keys _rightkey;
        
        private bool _axis;
        

        public Paddle(GameScreen parent, Texture2D sprite, Vector2 pos, Vector2 size, Keys left, Keys right, bool onXaxis = false)
        {
            _parent = parent;
            Rect = new(pos.ToPoint(), size.ToPoint());
            _sprite = sprite;
            _axis = onXaxis;
            _leftkey = left;
            _rightkey = right;
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_sprite, Rect, Color.White);
        }

        public void Update(GameTime gameTime, GameWindow window)
        {
            //Check keydowns
            int movespeed = 5;
            if (Keyboard.GetState().IsKeyDown(_leftkey))
            {
                if (!_axis)
                {
                    Rect.Y -= movespeed;
                }
                else
                {
                    Rect.X += movespeed;
                }
                    
            }
            
            if (Keyboard.GetState().IsKeyDown(_rightkey))
            {
                if (!_axis)
                {
                    Rect.Y += movespeed;
                }
                else
                {
                    Rect.X -= movespeed;
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