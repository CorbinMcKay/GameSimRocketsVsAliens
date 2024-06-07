using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
//using Microsoft.Xna.Framework.Storage;
using System.Diagnostics;//to access Debug

namespace Project1
{
    public enum cState { Dead, Rocket, Alien, Bonus, BonusAlien, BonusRocket, DangerObject, Animate, Barrier};
   

    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        public SpriteBatch spriteBatch;

      
        SpriteFont Font1, Font2, Font3, Font4;
        // in the pipeline program, right click on Content, select add new item
        // and select SpriteFont Description.  And then save.
        //
        //   see http://msdn.microsoft.com/en-us/library/bb447673.aspx
        //   for more details

        int maxThings;  // number of creatures
        int currentRockets, currentAliens, currentBonus, timesPlayed;
 
        Color bgColor;
        float screenWidth, screenHeight;
        Random rand;
        Creature[] thingArray; // creature array

        // variables to hold images
        protected Texture2D rocketTexture, alienTexture, bonusTexture, bonusRocket, bonusAlien, blackHole, animationTexture, barrierTexture;
        

        // variables to hold audio
        private SoundEffect mutateAudio, victoryAudio, deathAudio, powerupAudio;
        private Song gameMusic;

        // time variables
        long elapsedTime;
        double eTime, elapsed;

        // variables for comments
        protected String[] comments = {"Watch out!", "Zap!"};
        Vector2 commentPosition, commentPosition2;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            graphics.IsFullScreen = false;
            graphics.PreferredBackBufferHeight = 800;
            graphics.PreferredBackBufferWidth = 1600;
            IsFixedTimeStep = false;

            // create new random number generator
            rand = new Random((int)DateTime.Now.Ticks);

            maxThings = 100;  // set number of things
            thingArray = new Creature[maxThings];
            int numThings = 0;
            timesPlayed = 0;
            
            screenWidth = (float)(Window.ClientBounds.Width);
            screenHeight = (float)(Window.ClientBounds.Height);


            // spawn Zombies 
            for (int i = 0; i < 6; i++)
            {
                float randX = rand.Next(0, 1600);
                float randY = rand.Next(0, 800);
                thingArray[numThings] = new Creature(this, randX, randY, cState.Alien);
                Components.Add(thingArray[numThings++]);
                currentAliens++;
            }

            // spawn Humans
            for (int i = 0; i < 6; i++)
            {
                float randX = rand.Next(0, 1600);
                float randY = rand.Next(0, 800);
                thingArray[numThings] = new Creature(this, randX, randY, cState.Rocket);
                Components.Add(thingArray[numThings++]);
                currentRockets++;
            }

            // spawn bonus
            thingArray[numThings] = new Creature(this, 500.0f, 100.0f, cState.Bonus);
            Components.Add(thingArray[numThings++]);
            currentBonus++;
            thingArray[numThings] = new Creature(this, 500.0f, 500.0f, cState.Bonus);
            Components.Add(thingArray[numThings++]);
            currentBonus++;
            // thingArray[numThings] = new Creature(this, 1000.0f, 100.0f, cState.Bonus);
            // Components.Add(thingArray[numThings++]);
            // currentBonus++;
            // thingArray[numThings] = new Creature(this, 1000.0f, 500.0f, cState.Bonus);
            // Components.Add(thingArray[numThings++]);
            // currentBonus++;


            // spawn black holes
            thingArray[numThings] = new Creature(this, 750.0f, 300.0f, cState.DangerObject);
            Components.Add(thingArray[numThings++]);
            thingArray[numThings] = new Creature(this, 1400.0f, 100.0f, cState.DangerObject);
            Components.Add(thingArray[numThings++]);
            thingArray[numThings] = new Creature(this, 350.0f, 725.0f, cState.DangerObject);
            Components.Add(thingArray[numThings++]);



            // spawn barriers
            thingArray[numThings] = new Creature(this, 500.0f, 300.0f, cState.Barrier);
            Components.Add(thingArray[numThings++]);
            thingArray[numThings] = new Creature(this, 1100.0f, 350.0f, cState.Barrier);
            Components.Add(thingArray[numThings++]);
            thingArray[numThings] = new Creature(this, 750.0f, 100.0f, cState.Barrier);
            Components.Add(thingArray[numThings++]);
            thingArray[numThings] = new Creature(this, 675.0f, 550.0f, cState.Barrier);
            Components.Add(thingArray[numThings++]);
            thingArray[numThings] = new Creature(this, 100.0f, 100.0f, cState.Barrier);
            Components.Add(thingArray[numThings++]);
            thingArray[numThings] = new Creature(this, 1300.0f, 600.0f, cState.Barrier);
            Components.Add(thingArray[numThings++]);
            thingArray[numThings] = new Creature(this, 100.0f, 550.0f, cState.Barrier);
            Components.Add(thingArray[numThings++]);
            thingArray[numThings] = new Creature(this, 900.0f, 725.0f, cState.Barrier);
            Components.Add(thingArray[numThings++]);
            thingArray[numThings] = new Creature(this, 1100.0f, 10.0f, cState.Barrier);
            Components.Add(thingArray[numThings++]);



            while (numThings < maxThings)
            {  // and a bunch of dead ones
                float randX = rand.Next(0, 1600);
                float randY = rand.Next(0, 800);
                thingArray[numThings] = new Creature(this, randX, randY, cState.Dead);
                Components.Add(thingArray[numThings++]);
            }


        }

        protected override void Initialize()
        {
 
            //Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            screenWidth = (float)(Window.ClientBounds.Width);
            screenHeight = (float)(Window.ClientBounds.Height);
            bgColor = new Color (000, 000, 000); // set to your background color

            // load text fonts
            Font1 = Content.Load<SpriteFont>("AlienVRocket");
            Font2 = Content.Load<SpriteFont>("fpsfont");
            Font3 = Content.Load<SpriteFont>("spriteComments");
            Font4 = Content.Load<SpriteFont>("victory");

            // load sprite images
            rocketTexture = Content.Load<Texture2D>("images/Rocket");
            alienTexture = Content.Load<Texture2D>("images/Alien");
            bonusTexture = Content.Load<Texture2D>("images/Bonus");
            bonusRocket = Content.Load<Texture2D>("images/RocketV2");
            bonusAlien = Content.Load<Texture2D>("images/AlienV2");
            blackHole = Content.Load<Texture2D>("images/blackhole");
            barrierTexture = Content.Load<Texture2D>("images/Barrier");

            // load animation 
            animationTexture = Content.Load<Texture2D>("images/animation");

            // load audio 
            mutateAudio = Content.Load<SoundEffect>("Audio/mutation");
            victoryAudio = Content.Load<SoundEffect>("Audio/victory");
            deathAudio = Content.Load<SoundEffect>("Audio/death");
            powerupAudio = Content.Load<SoundEffect>("Audio/powerup");
            gameMusic = Content.Load<Song>("Audio/music");

            // Play game music
            MediaPlayer.Volume = 0.05f;
            MediaPlayer.IsRepeating = true;
            MediaPlayer.Play(gameMusic);


            Debug.WriteLine("Texture Width " + rocketTexture.Width);
            Debug.WriteLine("Texture Height " + rocketTexture.Height);


            //Debug code for illustration purposes.
            Debug.WriteLine("Window Width " + Window.ClientBounds.Width);
            Debug.WriteLine("Window Height " + Window.ClientBounds.Height);
            Debug.WriteLine("IsFixedTimeStep " + IsFixedTimeStep);
            Debug.WriteLine("TargetElapsedTime " + TargetElapsedTime);
          
            base.Initialize();
        }


        protected override void LoadContent()
        {
        }

        protected override void UnloadContent()
        {
            //No unload code needed.
        }


        protected override void Update(GameTime gameTime)
        {
            // get elapsed time in seconds
            elapsedTime = gameTime.ElapsedGameTime.Ticks;
            eTime = (double)elapsedTime / (double)TimeSpan.TicksPerSecond;
            IsFixedTimeStep = true;
            TargetElapsedTime = System.TimeSpan.FromSeconds(1d / 60);
            elapsed += (double)elapsedTime / (double)TimeSpan.TicksPerSecond;


            // reset everybody
            for (int it = 0; it < maxThings; it++)
            {
                thingArray[it].reset();
            }
            // compare everybody
            for (int it = 0; it < maxThings; it++)
            {
                for (int ot = 0; ot < maxThings; ot++)
                {
                    if (it != ot)
                        thingArray[ot].compare(thingArray[it]);
                }
            }
            // calculate separation direction
            for (int it = 0; it < maxThings; it++)
            {
                thingArray[it].separate();
            }


            GetInput(gameTime);  // get user input

            //The following statement is always required.
            base.Update(gameTime);
        }


        protected void GetInput(GameTime gameTime)
        {
            KeyboardState keyboardState = Keyboard.GetState();
            if (keyboardState.IsKeyDown(Keys.Escape))
            { // id escape key pressed, then exit
                this.Exit();
            }
        }


        protected override void Draw(GameTime gameTime)
        {

            GraphicsDevice.Clear(bgColor);

            spriteBatch.Begin();
   
            base.Draw(gameTime);  // draw everything else in the game


            int displayRockets = getCurrentRockets();
            int displayAliens = getCurrentAliens();
            Vector2 displayCommentPosition = getCommentPosition();
            Vector2 displayCommentPosition2 = getCommentPosition2();

            // draw fonts to screen
            double fps = 1.0 / eTime;  // calculate and display frame rate
            spriteBatch.DrawString(Font2, "fps " + fps.ToString("f6"), new Vector2(10, 10), Color.White);

            // draw title
            spriteBatch.DrawString(Font1, "Rockets: " + displayRockets.ToString() + "  Aliens: " + displayAliens.ToString(), new Vector2(625, 10), Color.LightSeaGreen);

            // draw comments
            spriteBatch.DrawString(Font3, comments[0], displayCommentPosition, Color.Red);
            spriteBatch.DrawString(Font3, comments[1], displayCommentPosition2, Color.Red);

            // draw winner and time survived once there are no remaining rockets or aliens
            if (displayRockets == 0)
            {
                elapsed -= (double)elapsedTime / (double)TimeSpan.TicksPerSecond;
                elapsed = Math.Round(elapsed, 2);
                spriteBatch.DrawString(Font4, "                       Aliens WIN!  \r\n  Time Survived: " + elapsed.ToString() + " seconds", new Vector2(350, 350), Color.LightSeaGreen);
                MediaPlayer.Pause();
                // prevent endless audio loop 
                if (timesPlayed < 1)
                {
                    playVictory();
                    timesPlayed++;
                }
            }
            if (displayAliens == 0)
            {
                elapsed -= (double)elapsedTime / (double)TimeSpan.TicksPerSecond;
                elapsed = Math.Round(elapsed, 2);
                spriteBatch.DrawString(Font4, "                     Rockets WIN! \r\n Time Survived: " + elapsed.ToString() + " seconds", new Vector2(350, 350), Color.LightSeaGreen);
                MediaPlayer.Pause();
                if (timesPlayed < 1)
                {
                    playVictory();
                    timesPlayed++;
                }

            }

            spriteBatch.End();

        }

        // a whole lot of functions
        public float getScreenWidth()
        {
            return screenWidth;
        }

        public float getScreenHeight()
        {
            return screenHeight;
        }

        public Random getRand()
        {
            return rand;
        }

        public SpriteBatch getSpriteBatch()
        {
            return spriteBatch;
        }

        public Texture2D getHumanTexture()
        {
            return rocketTexture;
        }

        public Texture2D getZombieTexture()
        {
            return alienTexture;
        }

        public Texture2D getBonusTexture()
        {
            return bonusTexture;
        }

        public Texture2D getBonusRocket()
        {
            return bonusRocket;
        }

        public Texture2D getBonusAlien()
        {
            return bonusAlien;
        }

        public Texture2D getDangerObject()
        {
            return blackHole;
        }

        public Texture2D getAnimationTexture()
        {
            return animationTexture;
        }

        public Texture2D getBarrierTexture()
        {
            return barrierTexture;
        }
        public int getCurrentRockets()
        {
            return currentRockets;
        }

        public int setCurrentRocketsMinus()
        {
            currentRockets -= 1;
            return currentRockets;
        }
        
        public int setCurrentAliensMinus()
        {
            currentAliens -= 1;
            return currentAliens;
        }
        public int setCurrentAliensAdd()
        {
            currentAliens += 1;
            return currentAliens;
        }
        public int getCurrentAliens()
        {
            return currentAliens;
        }

        public int getCurrentBonus()
        {
            return currentBonus;
        }
        public int setCurrentBonus(int x)
        {
            currentBonus += x;
            return currentBonus;
        }
        public Vector2 getCommentPosition()
        {
            return commentPosition;
        }

        public Vector2 setCommentPosition(Vector2 x)
        {
            commentPosition = x;
            return commentPosition;
        }


        public Vector2 getCommentPosition2()
        {
            return commentPosition2;
        }

        public Vector2 setCommentPosition2(Vector2 x)
        {
            commentPosition2 = x;
            return commentPosition2;
        }

        public void playMutate()
        {
            mutateAudio.Play(volume: 0.05f, pitch: 0.0f, pan: 0.0f);
        }

        public void playVictory()
        {
            victoryAudio.Play(volume: 0.05f, pitch: 0.0f, pan: 0.0f);
        }

        public void playDeath()
        {
            deathAudio.Play(volume: 0.05f, pitch: 0.0f, pan: 0.0f);
        }

        public void playBonus()
        {
            powerupAudio.Play(volume: 0.05f, pitch: 0.0f, pan: 0.0f);
        }


    }//End class
}//End namespace
