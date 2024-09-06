using System;
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
    private MenuScreen menuScreen;
    
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
        menuScreen = new(buttons);
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
            Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();
        
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

public class Ui
{
    public virtual void Draw(SpriteBatch spriteBatch)
    {

    }

    public virtual void Update(GameTime gameTime)
    {


    }

    public class Button
    {
        bool buttonHover = false;
        private bool buttonClickable = true;
        Texture2D buttonTexture;
        Texture2D highlightTexture = null;
        Point position;
        Point size;
        public event EventHandler ButtonClicked;
        
        public Button(Vector2 positionV, Vector2 sizeV, Texture2D texture, Texture2D highlightTexture) : this(positionV,
            sizeV, texture)
        {

            this.highlightTexture = highlightTexture;
        }

        public Button(Vector2 positionV, Vector2 sizeV, Texture2D texture)
        {
            this.position = positionV.ToPoint();
            this.size = sizeV.ToPoint();
            this.buttonTexture = texture;
        }

        public Button(Point position, Point size, Texture2D texture)
        {
            this.position = position;
            this.size = size;
            buttonTexture = texture;
        }

        public void Update()
        {
            if (CheckHover(Mouse.GetState().Position) && Mouse.GetState().LeftButton == ButtonState.Pressed)
            {
                ButtonClicked?.Invoke(this, EventArgs.Empty);
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(buttonTexture, new Rectangle(position, size), Color.White);
            if (buttonHover && highlightTexture != null)
            {
                spriteBatch.Draw(highlightTexture, new Rectangle(position, size), Color.White);
            }
        }

        bool CheckHover(Point mousePos)
        {
            if (mousePos.X > position.X && mousePos.X < position.X + size.X && mousePos.Y > position.Y &&
                mousePos.Y < position.Y + size.Y)
            {
                //The mouse is on the button!!!
                buttonHover = true;
            }
            else
            {
                buttonHover = false;
            }

            return buttonHover;
        }
    }
}

public class MenuScreen : Ui
    {
        private Button[] _buttons;

        public MenuScreen(Texture2D texture)
        {
            _buttons = new Button[1];
            _buttons[0] = new Button(Vector2.Zero, new Vector2(40, 40), texture, null);

        }

        public MenuScreen(Button[] buttons)
        {
            _buttons = buttons;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            foreach (Button b in _buttons)
            {
                b.Draw(spriteBatch);
            }
        }

        public override void Update(GameTime gameTime)
        {
            foreach (Button b in _buttons)
            {
                b.Update();
            }
        }
    }


