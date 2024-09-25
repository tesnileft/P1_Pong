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
    private Scene menuScreen;
    private Scene gameScreen;
    private Scene currentScene;
    private MouseState mousePrev;
    private EventHandler mouseClick;
    
    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
        Window.AllowUserResizing = false;
    }
    protected override void Initialize()
    {
        Window.Title = "Pong";
        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        //Initialize Menu UI
        menuScreen = new MenuScreen(this);
        
        currentScene = menuScreen;
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
            Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();
        
        currentScene.Update(gameTime);
        
        //This does nothing anymore 
        backgroundhue = (float)Math.Sin(gameTime.TotalGameTime.TotalSeconds) / 2 + 0.5f;
        
        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.Black);
        
        _spriteBatch.Begin(samplerState: SamplerState.PointClamp);
        currentScene.Draw(_spriteBatch);
        _spriteBatch.End();
        
        base.Draw(gameTime);
    }

    //Grrr i need a scene manager >:/ (I do not need a scene manager)
    public void ChangeScene(Scene scene)
    {
        currentScene = scene;
    }
}




