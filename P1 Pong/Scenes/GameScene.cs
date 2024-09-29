using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using P1_Pong;
using P1_Pong.UI;

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
    private Vector2 ballPos;
    private Rectangle ballRect;
    private Paddle previousPaddle;

    private UI _GameUi;
    private UI _GameOverUi;
    private UI _CurrentUi;
    
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
        
        SpriteFont font = Content.Load<SpriteFont>("Sprites/UI/SpriteFont");
        
        _CurrentUi = new UI(new UI.UiElement[0]);
        int paddleWidth = 10;
        _paddles = new Paddle[2];
        _paddles[0] = new PlayerPaddle(
            "player 1",
            this,
            _paddleTex,
            new Vector2(20, _game.Window.ClientBounds.Height/2 - 100/2),
            new Vector2(paddleWidth, 100),
            Keys.W,
            Keys.S,
            font
            );
        _paddles[1] = new PlayerPaddle(
            "player 2",
            this,
            _paddleTex,
            new Vector2(_game.Window.ClientBounds.Width - (20 + paddleWidth), _game.Window.ClientBounds.Height/2 - 100/2),
            new Vector2(paddleWidth, 100),
            Keys.Up,
            Keys.Down,
            font
        );
        int ballsize = 10;
        
        // Texture2D fontTexture = Content.Load<Texture2D>("Sprites/UI/1234Font");
        
        
        ballPos = new Vector2(_game.Window.ClientBounds.Width/2 -ballsize/2 ,_game.Window.ClientBounds.Height/2 - ballsize/2);
        ballRect = new (ballPos.ToPoint(), new Point(ballsize));
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
        
        //Draw UI
        _CurrentUi.Draw(spriteBatch);
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
            if (p.LifeCount > 0)
            {
                p.Update(gameTime, _game.Window);
            }
            
        }
        
        //Ball logic (2P)
        int ballspeed = gameTime.ElapsedGameTime.Milliseconds / 3;
        ballPos += (ballDir * ballspeed);
        ballRect.Location = ballPos.ToPoint();
        
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
        
        if (ballRect.X < 0)
        {
            //Right point
            _paddles[0].LifeCount -= 1;
            if (_paddles[1].LifeCount <= 0)
            {
                GameOver(_paddles[0].Name);
            }
            else
            {
                ResetBall();
            }
            
        }

        if (ballRect.X + ballRect.Width > _game.Window.ClientBounds.Width)
        {
            //Left point
            _paddles[1].LifeCount -= 1;
            if (_paddles[1].LifeCount <= 0)
            {
                GameOver(_paddles[1].Name);
            }
            else
            {
                ResetBall();
            }
        }
        
        
    }

    private void GameOver(string winPlayer)
    {
        ballPos = new Vector2(_game.Window.ClientBounds.Width / 2, _game.Window.ClientBounds.Height / 2);
        ballDir = Vector2.Zero;
        Rectangle gameOverMessageRect = new (Point.Zero, new Point(_game.Window.ClientBounds.Width, _game.Window.ClientBounds.Height));
        SpriteFont font = Content.Load<SpriteFont>("Sprites/UI/SpriteFontLarge");
        UI.TextElement textGameOver = new($"{winPlayer} wins", font, gameOverMessageRect);
        UI.UiElement[] endGameElems = { textGameOver };
        _CurrentUi = new UI(endGameElems);
    }
    private void ResetBall()
    {
        ballDir = Vector2.Zero;
        ballPos = new Vector2(_game.Window.ClientBounds.Width / 2, _game.Window.ClientBounds.Height / 2);
        ballRect.Location = ballPos.ToPoint();
        _countdown = true;
        previousPaddle = null;
    }

    private void RandomizeBall()
    {
        if (new Random().Next(2) == 1)
        {
            //Go left
            ballDir.X = -1;
        }
        else
        {
            ballDir.X = 1;
        }
        ballDir.Y = new Random().Next(2) == 1 ? 1 : -1;
    }
}