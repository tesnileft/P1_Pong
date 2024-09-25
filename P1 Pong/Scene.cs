using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
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
    private int coundownMs = 1000;
    private Vector2 ballDir;
    private Rectangle ballRect;
    private Paddle previousPaddle;
    private Dictionary<Paddle, int> _scoreDict;
    
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
        _weede = Content.Load<Texture2D>("Sprites/Weede");
        
        //For the 2 player game
        _paddleTex = Content.Load<Texture2D>("Sprites/Paddle_default");
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
        _scoreDict = new ();
        foreach (var paddle in _paddles)
        {
            //Start at 3 life
            _scoreDict.Add(paddle, 3);
        }
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
                coundownMs = 800;
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
            if (ballRect.Intersects(p.Rect) && previousPaddle != p)
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
                }
                else
                {
                    //Front reflect
                    ballDir.X *= -1;
                    ballDir.Y = (float)(angleBall / 80) * (paddleCenter.Y> ballCenter.Y ? -1 : 1);
                    //Console.WriteLine($"Ball impact angle: {paddleCenter.Y > ballCenter.Y}");
                }
                previousPaddle = p;
            }
            
        }

        //Todo give ppl lives or sthn :/
        if (ballRect.X < 0)
        {
            //Right point
            _scoreDict[_paddles[0]] -= 1;
            ResetBall();
        }

        if (ballRect.X + ballRect.Width > _game.Window.ClientBounds.Width)
        {
            //Left point
            _scoreDict[_paddles[1]] -= 1;
            ResetBall();
        }
        
        
    }

    private void ResetBall()
    {
        ballDir = Vector2.Zero;
        ballRect.Location = new Point(_game.Window.ClientBounds.Width / 2, _game.Window.ClientBounds.Height / 2);
        _countdown = true;
        previousPaddle = null;
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

        public MenuScreen(Game1 Game)
        {
            ContentManager Content = Game.Content;
            List<UI.Button> buttons = new List<UI.Button>();
            int windowX = Game.Window.ClientBounds.Width;
            int windowY = Game.Window.ClientBounds.Height;
            
            Texture2D twoPlayerSprite = Content.Load<Texture2D>("Sprites/UI/2P_sprite");
            Texture2D highlightSpriteLarge = Content.Load<Texture2D>(@"Sprites/UI/Highlight");
            UI.Button twopButton = new (
                new Vector2(windowX/2-50, 200) ,
                new Vector2(100), 
                twoPlayerSprite,
                highlightSpriteLarge
                );
            twopButton.ButtonDown += (obj, args) =>
            {
                Console.WriteLine("Main menu button clicked"); 
                Game.ChangeScene(new GameScreen(Game));
            
            };
            buttons.Add(twopButton);
            
            Texture2D highlightSpriteSmall = Content.Load<Texture2D>(@"Sprites/UI/Highlight_small");
            Texture2D gitSprite = Content.Load<Texture2D>(@"Sprites/UI/Git");
            UI.Button gitButton = new(
                new Vector2(windowX - 40, windowY - 40), //Bottom right corner
                new Vector2(40),
                gitSprite,
                highlightSpriteSmall
            );
            gitButton.ButtonDown += (sender, args) =>
            {
                string url = "https://github.com/tesnileft/P1_Pong";
                try
                {
                    Process.Start(url);
                }
                catch
                {
                    // hack because of this: https://github.com/dotnet/corefx/issues/10361
                    if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                    {
                        url = url.Replace("&", "^&");
                        Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
                    }
                    else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                    {
                        Process.Start("xdg-open", url);
                    }
                    else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                    {
                        Process.Start("open", url);
                    }
                    else
                    {
                        throw;
                    }
                }
            };
            buttons.Add(gitButton);
            
            homeUI = new (buttons.ToArray());
            Init(highlightSpriteLarge);
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