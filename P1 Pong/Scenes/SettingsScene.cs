global using P1_Pong.UI;
using System.Collections.Generic;
using System.Net.Mime;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using P1_Pong;

namespace P1_Pong.Scenes;

public class SettingsScene: Scene
{
    UI.UI _settings;

    public SettingsScene(PongGame game)
    {
        List<UiElement> elements = new();
        
        game.Content.Load<Texture2D>(@"Sprites\IF I HAD ONE");
        SpriteFont font = game.Content.Load<SpriteFont>(@"Sprites\UI\SpriteFontLarge");
        TextElement elem = new(
            "here is where i would put my settings menu",
            font,
            new Rectangle(new Point(200), Point.Zero)
        );
        Rectangle window = game.Window.ClientBounds;
        Texture2D homeSprite = game.Content.Load<Texture2D>("Sprites/UI/Home");
        Texture2D highlightSpriteSmall = game.Content.Load<Texture2D>("Sprites/UI/Highlight_small");
        Button goHomeButton = new(
            new Vector2(0,  0), 
            new Vector2(40),
            homeSprite,
            highlightSpriteSmall
        );
        goHomeButton.ButtonDown += (sender, args) =>
        {
            game.ChangeScene(new MenuScreen(game));
        };
        elements.Add(goHomeButton);
        
        elements.Add(elem);
        _settings = new (elements.ToArray());
    }
    public override void Draw(SpriteBatch spriteBatch)
    {
        _settings.Draw(spriteBatch);
    }

    public override void Update(GameTime gameTime)
    {
        _settings.Update(gameTime);
    }
}