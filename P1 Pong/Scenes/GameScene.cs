using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using P1_Pong;
using P1_Pong.UI;

public class GameScreen : Scene
{
    readonly public ContentManager Content;
    PongGame _game;
    
    private Texture2D _ballTex;
    private Texture2D _paddleTex;
    private Texture2D _weede;
    private bool _countdown = true;
    private int _coundownMs = 1000;
    private Vector2 ballDir;
    private Vector2 ballPos;
    private Rectangle ballRect;
    private Paddle previousPaddle;

    private UI _GameUi;
    private UI _GameOverUi;
    private UI _CurrentUi;
    
    Paddle[] _paddles;

    public GameScreen(PongGame game)
    {
        Content = game.Content;
        _game = game;
        LoadContent();
    }
    public void LoadContent()
    {
        //Load all required textures and UI
        _weede = Content.Load<Texture2D>("Sprites/Weede");
        _paddleTex = Content.Load<Texture2D>("Sprites/Paddle_default");
        _ballTex =  Content.Load<Texture2D>("Sprites/Ball");
        SpriteFont font = Content.Load<SpriteFont>("Sprites/UI/SpriteFont");
        _CurrentUi = new UI(new UiElement[0]);
        int paddleWidth = 16;
        int paddleHeight = 160;
        int borderDistance = 40;
        
        Load2P();
        //For the 2 player game
        void Load2P()
        {
            //Initialize the paddles
            _paddles = new Paddle[2];
            _paddles[0] = new PlayerPaddle(
                "player 1",
                this,
                _paddleTex,
                new Vector2(borderDistance, _game.Window.ClientBounds.Height/2 - 100/2),
                new Vector2(paddleWidth, paddleHeight),
                Keys.W,
                Keys.S,
                font,
                Paddle.FacingDir.Right
            );
            _paddles[1] = new PlayerPaddle(
                "player 2",
                this,
                _paddleTex,
                new Vector2(_game.Window.ClientBounds.Width - (borderDistance + paddleWidth), _game.Window.ClientBounds.Height/2 - 100/2),
                new Vector2(paddleWidth, paddleHeight),
                Keys.Up,
                Keys.Down,
                font,
                Paddle.FacingDir.Left
            );
        }
        //Initialize the ball
        int ballsize = 20;
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
        
        //Draw the current UI
        _CurrentUi.Draw(spriteBatch);
    }
    public override void Update(GameTime gameTime)
    {
        //Handle the little logic pause between ball respawning, just so you can breathe for a second
        if (_countdown)
        {
            _coundownMs -= gameTime.ElapsedGameTime.Milliseconds;
            if (_coundownMs <= 0)
            {
                _countdown = false;
                _coundownMs = 800;
                RandomizeBall();
            }
            return;
        }
        
        _CurrentUi.Update(gameTime);
        
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
                double angleBall;
                if (p.Directional == Paddle.FacingDir.Left || p.Directional == Paddle.FacingDir.Right)
                {
                    angleBall = Math.Atan(double.Abs(ballCenter.Y - paddleCenter.Y) / double.Abs(ballCenter.X - paddleCenter.X)) * 180 / Math.PI;
                }
                else
                {
                    angleBall = 1;
                }
                
                
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
            if (_paddles[0].LifeCount <= 0)
            {
                GameOver(_paddles[1].Name);
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
                GameOver(_paddles[0].Name);
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
        
        //End of game UI
        Rectangle window = _game.Window.ClientBounds;
        List<UiElement> endGameElems = new();
        
        //"win" text
        SpriteFont font = Content.Load<SpriteFont>("Sprites/UI/SpriteFontLarge");
        string wintext = $"{winPlayer} wins";
        Rectangle gameOverMessageRect = new(
            new Point(window.Width / 2 - (int)font.MeasureString(wintext).X / 2, window.Height / 2),
            Point.Zero);
        TextElement textGameOver = new(wintext, font, gameOverMessageRect);
        endGameElems.Add(textGameOver);
        
        //Back home button
        Texture2D homeSprite = Content.Load<Texture2D>("Sprites/UI/Home");
        Texture2D highlightSpriteSmall = Content.Load<Texture2D>("Sprites/UI/Highlight_small");
        Button goHomeButton = new(
                new Vector2(200,  window.Height / 2), 
                new Vector2(40),
                homeSprite,
                highlightSpriteSmall
            );
        goHomeButton.ButtonDown += (sender, args) =>
        {
            _game.ChangeScene(new MenuScreen(_game));
        };
        endGameElems.Add(goHomeButton);
        
        //Exit button
        Texture2D exitSprite = Content.Load<Texture2D>("Sprites/UI/X");
        Button exitButton = new(
            new Vector2(window.Width - 240, window.Height / 2), 
            new Vector2(40),
            exitSprite,
            highlightSpriteSmall
        );
        exitButton.ButtonDown += (sender, args) =>
        {
            _game.Exit();
        };
        endGameElems.Add(exitButton);
        
        //Reset/try button
        Texture2D retrySprite = Content.Load<Texture2D>("Sprites/UI/X");
        Button retryButton = new(
            new Vector2(window.Width - 240, window.Height / 2 + 80), 
            new Vector2(40),
            retrySprite,
            highlightSpriteSmall
        );
        retryButton.ButtonDown += (sender, args) =>
        {
            _game.ChangeScene(new GameScreen(_game));
        };
        endGameElems.Add(retryButton);
        
        _CurrentUi = new UI(endGameElems.ToArray());
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