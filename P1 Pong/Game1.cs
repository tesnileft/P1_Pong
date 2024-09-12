using System;
using System.Collections.Generic;
using System.Reflection;
using System.Xml.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using Microsoft.Xna.Framework.Content;
using TestLib.Helper;
namespace P1_Pong;

public class Game1 : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    private float backgroundhue = 0;    
    private Texture2D playButtonTex;
    private Ui menuScreen;
    private Ui gameScreen;
    private MouseState mousePrev;
    private EventHandler mouseClick;
    
    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        // TODO: Add your initialization logic here
        
        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        playButtonTex = Content.Load<Texture2D>(@"Play");
        Ui.Button[] buttons = new Ui.Button[1];
        buttons[0] = new Ui.Button(
            new Vector2(Window.ClientBounds.Width/2-50, 0) ,
            new Vector2(100), 
            playButtonTex, 
            Content.Load<Texture2D>(@"Highlight")
            );
        menuScreen = new MenuScreen(buttons, Content.Load<Texture2D>(@"Highlight"));
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
            Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();
        
        if(mousePrev.LeftButton == ButtonState.Released && Mouse.GetState().LeftButton == ButtonState.Pressed )
        {
           mouseClick?.Invoke(this, EventArgs.Empty); 
        }
        mousePrev = Mouse.GetState();
        
        menuScreen.Update(gameTime);
        // TODO: Add your update logic here
        backgroundhue = (float)Math.Sin(gameTime.TotalGameTime.TotalSeconds) / 2 + 0.5f;
        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.Black);
        
        _spriteBatch.Begin(samplerState: SamplerState.PointClamp);
        menuScreen.Draw(_spriteBatch);
        _spriteBatch.End();
        base.Draw(gameTime);
    }
}




