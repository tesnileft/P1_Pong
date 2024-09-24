using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
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
        //Stuff gets drawn here
    }

    public virtual void Update(GameTime gameTime)
    {
        //Stuff gets done here
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
        
        //For the 2 player game
        _paddleTex = Content.Load<Texture2D>("Paddle_default");
        //Temp texture
        _ballTex = _weede;
        
        _paddles = new Paddle[2];
        _paddles[0] = new PlayerPaddle(
            this,
            _paddleTex,
            new Vector2(20, _game.Window.ClientBounds.Height/2 - 100/2),
            new Vector2(10, 100),
            Keys.W,
            Keys.S
            );
        _paddles[1] = new PlayerPaddle(
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
        //I removed him he no longer is real :<
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
                //Do collision!!
                //If it hits the top, should bounce back up, and invert x direction
                Vector2 ballCenter = ballRect.Center.ToVector2();
                Vector2 paddleCenter = p.Rect.Center.ToVector2();
                
                
                // Normal (vertical paddle) is 0
                double angleBall = p.Axis ? 1 : Math.Atan(  double.Abs(ballCenter.Y - paddleCenter.Y) / double.Abs(ballCenter.X - paddleCenter.X)) * 180 / Math.PI;
                
                double paddleX = Double.Abs(paddleCenter.X - p.Rect.Location.X);
                double paddleY = Double.Abs(paddleCenter.Y - p.Rect.Location.Y);
                
                double paddleAngle = Math.Atan(paddleY/paddleX) * 180 / Math.PI;
  
                Console.WriteLine($"Collision!!");
                Console.WriteLine($"Paddle corner angle: {paddleAngle}");
                Console.WriteLine($"Ball impact angle: {angleBall}");
                
                Vector2 paddleDiff = paddleCenter - ballCenter;
                //Naively reflect
                if (angleBall > paddleAngle)
                {
                    //Side reflect
                    ballDir.Y *= (ballCenter.Y < paddleCenter.Y ? -1 : 1);
                    ballDir.X *= -1;
                    //TODO get the ball out of the paddle
                }
                else
                {
                    //Front reflect
                    ballDir.X *= -1;
                    ballDir.Y = (float)(angleBall / 80) * (paddleCenter.Y> ballCenter.Y ? -1 : 1);
                    //Console.WriteLine($"Ball impact angle: {paddleCenter.Y > ballCenter.Y}");
                }
                
            }
            
        }

        //Todo give ppl lives or sthn :/
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