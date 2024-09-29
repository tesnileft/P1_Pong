using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using P1_Pong;
using P1_Pong.Scenes;
using P1_Pong.UI;
using TestLib.Helper;

public class MenuScreen : Scene
{
        private UI homeUI;
        private List<CpuParticleManager> _particles;
        
        void InitParticles(Texture2D texture)
        {
            _particles = new List<CpuParticleManager>();
            _particles.Add(new CpuParticleManager(texture));
        }

        public MenuScreen(PongGame Game)
        {
            ContentManager Content = Game.Content;
            List<UiElement> uiElements = new ();
            int windowX = Game.Window.ClientBounds.Width;
            int windowY = Game.Window.ClientBounds.Height;
            
            Texture2D splash = Content.Load<Texture2D>("Sprites/UI/MenuSplash");
            ImageElement splashImage = new ImageElement(splash, new Rectangle(150, 80, 500, 200));
            uiElements.Add(splashImage);
            //2 Player start button
            Texture2D twoPlayerSprite = Content.Load<Texture2D>("Sprites/UI/2P_sprite");
            Texture2D highlightSpriteLarge = Content.Load<Texture2D>(@"Sprites/UI/Highlight");
            Button twopButton = new (
                new Vector2(windowX/2-50, 400) ,
                new Vector2(100), 
                twoPlayerSprite,
                highlightSpriteLarge
                );
            twopButton.ButtonDown += (obj, args) =>
            {
                Console.WriteLine("Main menu button clicked"); 
                Game.ChangeScene(new GameScreen(Game));
            
            };
            uiElements.Add(twopButton);
            
            //Git link button
            Texture2D highlightSpriteSmall = Content.Load<Texture2D>(@"Sprites/UI/Highlight_small");
            Texture2D gitSprite = Content.Load<Texture2D>(@"Sprites/UI/Git");
            Button gitButton = new(
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
            uiElements.Add(gitButton);
            
            //Exit button
            Texture2D exitSprite = Content.Load<Texture2D>(@"Sprites/UI/X");
            Button exitButton = new(
                new Vector2(windowX - 40, 0), //Top right corner
                new Vector2(40),
                exitSprite,
                highlightSpriteSmall
            );
            exitButton.ButtonDown += (sender, args) =>
            {
                Game.Exit(); //Bah bye!! -Famous plumber Mario Mario
            };
            uiElements.Add(exitButton);
            
            //Settings button
            Texture2D settingsSprite = Content.Load<Texture2D>(@"Sprites/UI/Settings");
            Button settingsButton = new(
                new Vector2(windowX - 120, windowY - 40), //Top right corner
                new Vector2(40),
                settingsSprite,
                highlightSpriteSmall
            );
            settingsButton.ButtonDown += (sender, args) =>
            {
                Game.ChangeScene(new SettingsScene(Game));
            };
            uiElements.Add(settingsButton);
            //And give everything to the UI to handle
            homeUI = new (uiElements.ToArray());
            //Silly particle emitter that follows your mouse, not at all important but cool
            InitParticles(highlightSpriteLarge);
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