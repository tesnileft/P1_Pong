using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace TestLib.Helper;


public class CpuParticleManager
{
    private double _timeSinceEmit = 0;
    bool _isLooping = false;
    private float _frameDuration;
    private Texture2D _particleTex;
    HashSet<Particle> _particles;
    class Particle
    {
        public Vector2 Position;
        public Vector2 Velocity;
        public double Lifetime;
        
        
        public Particle(Vector2 position, Vector2 velocity, double lifetime)
        {
            this.Position = position;
            this.Velocity = velocity;
            this.Lifetime = lifetime;
            
        }
    }

    void Emit(Vector2 position)
    {
        Random rand = new Random();
        float scale = 1;
        Vector2 velocity = new Vector2((float)rand.NextDouble()*(2*scale) - scale,(float)rand.NextDouble()*(2*scale) - scale);
        double lifetime = 1;
        
        _particles.Add(
            new Particle(
                position,
                velocity,
                lifetime
                ));
    }

    void BurstEmit()
    {
        
    }
    
    public void Update(GameTime gameTime)
    {
        //Emitting particles
        _timeSinceEmit += gameTime.ElapsedGameTime.TotalSeconds;
        if (_timeSinceEmit > 0.01)
        {
            Emit(Mouse.GetState().Position.ToVector2());
            _timeSinceEmit = 0;
        }
        
        
        HashSet<Particle> removeParticles = new HashSet<Particle>();
        foreach (Particle particle in _particles)
        {
            
            particle.Lifetime -= gameTime.ElapsedGameTime.TotalSeconds;
            if (particle.Lifetime < 0)
            {
                removeParticles.Add(particle);
            }
            
            particle.Position += particle.Velocity;
            
            
        }
        _particles.ExceptWith(removeParticles);
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        foreach (var particle in _particles)
        {
            spriteBatch.Draw(_particleTex, new Rectangle(particle.Position.ToPoint(), new Point(10)), Color.White);
        }
    }
    
    public CpuParticleManager(Texture2D texture)
    {
        this._particles = new HashSet<Particle>();
        this._particleTex = texture;
    }
}
