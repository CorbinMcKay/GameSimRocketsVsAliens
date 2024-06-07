using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Diagnostics;//to access Debug
using Microsoft.Xna.Framework.Input;



namespace Project1
{
    public class Creature : DrawableGameComponent
    {

        protected cState creatureState;  // state
        protected Texture2D humanTexture, zombieTexture, bonusTexture, bonusRocket, bonusAlien, blackHole, animationTexture, barrierTexture;
        protected Rectangle drawRect, drawRect2;
        KeyboardState prevKeyboardState;
        int currentRockets, currentAliens, gameState;

        protected SpriteBatch spriteBatch;

        protected Vector2 Position, Direction, hideComment;


        protected float speed;
        protected int dFrame;  // frames since new direction
        protected int framesPerDir;  // number of frames to spend in each direction

        protected Game1 game;
        protected Random rand;
        protected float screenWidth, screenHeight;

        // variable for distance comparison
        int collide = 4000;
        float closest = 25000;
        float separationDistance = 25000;
        float alignmentDistance = 200000;
        float cohesionDistance = 500000;

        // direction vectors and probably an unnecessary amount of variables for neighbor evaluation
        Vector2 C, D, A, S, V, BA;
        Vector2 s_allNeighbors, s_avgNeighbors, a_allNeighbors, a_avgNeighbors, c_allNeighbors, c_avgNeighbors, ba_allNeighbors, ba_avgNeighbors;
        int s_numNeighbors, a_numNeighbors, c_numNeighbors, ba_numNeighbors;


        // variables to calculate source rectangle
        protected Rectangle sourceRectangle;
        float elapsed;
        float delay = 100f;
        int frames = 0;



        public Creature(Game1 theGame, float ix, float iy, cState state)
            : base(theGame)
        {
            Position = new Vector2(ix, iy);

            Direction = new Vector2();

            creatureState = state;

            game = theGame;

        }


        public cState getState()
        {
            return creatureState;
        }

        public void setState(cState state)
        {
            creatureState = state;
        }



        public void reset()
        {
            s_numNeighbors = 0;
            s_allNeighbors = Vector2.Zero;
            a_numNeighbors = 0;
            a_allNeighbors = Vector2.Zero;
            c_numNeighbors = 0;
            c_allNeighbors = Vector2.Zero;
            ba_numNeighbors = 0;
            ba_allNeighbors = Vector2.Zero;
        }

        public void compare(Creature a)
        {
            float ax = a.Position.X;
            float ay = a.Position.Y;
            float bx = Position.X;
            float by = Position.Y;
            Vector2 p = Position;
            Vector2 z = a.Position;
            Vector2 x = a.Direction;
            V = p - z;
            Vector2 B = z - p;
            Vector2 D = Direction;
            Vector2 O;
            float creatureDistance = (ax - bx) * (ax - bx) + (ay - by) * (ay - by);


            if (getState() == cState.Rocket && a.getState() == cState.Alien || getState() == cState.Rocket && a.getState() == cState.BonusAlien)
            {
                // detects collision and turns entity into zombie
                if (creatureDistance < collide)
                {
                    setState(cState.Alien);
                    game.setCurrentRocketsMinus();
                    game.setCurrentAliensAdd();
                    game.setCommentPosition2(a.Position);
                    game.playMutate();

                }

                // detect closest zombie
                else if (creatureDistance < closest)
                {
                    closest = creatureDistance;
                    V.Normalize();
                    Direction = V * speed;
                }


                if (creatureDistance < 50000)
                {
                    game.setCommentPosition(Position);
                }
                // hide comments when not in use
                else
                {
                    game.setCommentPosition2(hideComment);
                    game.setCommentPosition(hideComment);
                }


            }


        
            

            if (getState() == cState.Rocket && a.getState() == cState.Rocket)
            {
                // if other zombies are within neighbor distance, add other humans position and increment number of near neighbors
                if (creatureDistance < separationDistance)
                {
                    s_allNeighbors += z;
                    s_numNeighbors += 1;
                }

                // 
                if (creatureDistance < alignmentDistance)
                {
                    a_allNeighbors += x;
                    a_numNeighbors += 1;
                }

                if (creatureDistance < cohesionDistance)
                {
                    c_allNeighbors += z;
                    c_numNeighbors += 1;
                }


            }

            // A rocket in the bonus state evaluartes nearby aliens
            if (getState() == cState.BonusRocket && a.getState() == cState.Alien || getState() == cState.BonusRocket && a.getState() == cState.BonusAlien)
            {
                if (creatureDistance < 100000)
                {
                    ba_allNeighbors += z;
                    ba_numNeighbors += 1;
                }
            }


            // Rockets and Aliens will go toward bonus if within range
            if (getState() == cState.Rocket && a.getState() == cState.Bonus || getState() == cState.Alien && a.getState() == cState.Bonus)
            {
                if (creatureDistance < 40000)
                {
                    B.Normalize();
                    Direction = B * speed;
                }
            }

            // detect if rocket or alien has collided with bonus. Rocket or Alien is switched to its bonus state, and the bonus is returned dead after collision
            if (getState() == cState.Rocket && a.getState() == cState.Bonus)
            {
                if (creatureDistance < collide)
                {
                    setState(cState.BonusRocket);
                    a.setState(cState.Dead);
                    game.playBonus();
                    game.setCurrentBonus(-1);
                }
            }

            if (getState() == cState.Alien && a.getState() == cState.Bonus)
            {
                if (creatureDistance < collide)
                {
                    setState(cState.BonusAlien);
                    a.setState(cState.Dead);
                    game.playBonus();
                    game.setCurrentBonus(-1);
                }
            }

            // a rocketship with bonus can kill aliens upon collision
            if (getState() == cState.BonusRocket && a.getState() == cState.Alien)
            {
                if (creatureDistance < collide)
                {
                    a.setState(cState.Animate);
                    game.playDeath();
                    game.setCurrentAliensMinus();
                }
            }

            // attempt to avoid black hole
            if (getState() == cState.Rocket && a.getState() == cState.DangerObject || getState() == cState.Alien && a.getState() == cState.DangerObject || getState() == cState.BonusRocket && a.getState() == cState.DangerObject)
            {
                if (creatureDistance < 15000)
                {
                    V.Normalize();
                    O = 0.6f * D + 0.4f * V;
                    O.Normalize();
                    Direction = O * speed;
                }
            }

            // I gave aliens in the bonus state better collision avoidance with black holes due to its speed
            if (getState() == cState.BonusAlien && a.getState() == cState.DangerObject)
            {
                if (creatureDistance < 15000)
                {
                    V.Normalize();
                    O = 0.3f * D + 0.7f * V;
                    O.Normalize();
                    Direction = O * speed;
                }
            }

            // detect black hole collision 
            if (getState() == cState.Rocket && a.getState() == cState.DangerObject || getState() == cState.BonusRocket && a.getState() == cState.DangerObject)
            {
                if (creatureDistance < 5500)
                {
                    setState(cState.Animate);
                    game.playDeath();
                    game.setCurrentRocketsMinus();
                }
            }

            if (getState() == cState.Alien && a.getState() == cState.DangerObject || getState() == cState.BonusAlien && a.getState() == cState.DangerObject)
            {
                if (creatureDistance < 5500)
                {
                    setState(cState.Animate);
                    game.playDeath();
                    game.setCurrentAliensMinus();
                }
            }


            // Rockets and aliens cannot penetrate barriers
            if (getState() == cState.Alien && a.getState() == cState.Barrier || getState() == cState.BonusAlien && a.getState() == cState.Barrier)
            {
                if (creatureDistance < 5500)
                {
                    V.Normalize();
                    Direction = V * speed;
                }
            }

            if (getState() == cState.Rocket && a.getState() == cState.Barrier || getState() == cState.BonusRocket && a.getState() == cState.Barrier)
            {
                if (creatureDistance < 5500)
                {
                    V.Normalize();
                    Direction = V * speed;
                }
            }

            // If a rocket and alien in bonus states collide, they both lose their bonus state and deflect in opposite directions
            if (getState() == cState.BonusRocket && a.getState() == cState.BonusAlien)
            {
                if (creatureDistance < collide)
                {
                    V.Normalize();
                    Direction = V * speed;
                    a.Direction = -Direction;
                    setState(cState.Rocket);
                    a.setState(cState.Alien);
                    game.playMutate();
                }
            }

            // One bonus item is always active in game. If all bonus items are obtained, a bonus will spawn on screen. 
            if (game.getCurrentBonus() == 0)
            {
                if (getState() == cState.Dead)
                {
                    setState(cState.Bonus);
                    float randX = rand.Next(0, 1552);
                    float randY = rand.Next(0, 752);
                    Position.X = randX;
                    Position.Y = randY;
                    game.setCurrentBonus(1);
                }
            }



        }

        public void separate()
        {
            Vector2 H = Position; // current human
            D = Direction;  // existing direction
            Vector2 N, J, K;
            if (getState() == cState.Rocket)
            {
                if (s_numNeighbors > 0) // separation
                {
                    s_avgNeighbors = s_allNeighbors / s_numNeighbors;
                    S = H - s_avgNeighbors;
                    S.Normalize();

                }
                if (a_numNeighbors > 0) // alignment
                {
                    a_avgNeighbors = a_allNeighbors / a_numNeighbors;
                    A = a_avgNeighbors;
                    A.Normalize();


                }

                if (c_numNeighbors > 0) // cohesion
                {
                    c_avgNeighbors = c_allNeighbors / c_numNeighbors;
                    C = c_avgNeighbors - H;
                    C.Normalize();


                }

                N = 0.7f * D + 0.1f * S + 0.1f * A + 0.1f * C;
                N.Normalize();
                Direction = N * speed;


            }

            // Rockets in the bonus state head toward average position of aliens
            else if (getState() == cState.BonusRocket)
            {
                if (ba_numNeighbors > 0)
                {
                    ba_avgNeighbors = ba_allNeighbors / ba_numNeighbors;
                    BA = ba_avgNeighbors - H;
                    BA.Normalize();
                    J = 0.7f * D + 0.3f * BA;
                    J.Normalize();
                    Direction = J * speed;

                }
               
            }
            else
            {
                K = D;
                K.Normalize();
                Direction = K * speed;
            }

        }

        public override void Initialize()
        {
            base.Initialize();
        }


        protected override void LoadContent()
        {
            speed = 5.0f;  // set to your speed
            dFrame = 0;
            framesPerDir = 150;  // set to your frames Per Direction
            rand = game.getRand();

            Direction.Y = (float)rand.NextDouble() - 0.5f;
            Direction.X = (float)rand.NextDouble() - 0.5f;
            Direction *= speed;
            gameState = 0;

            hideComment.X = -1000.0f;
            hideComment.Y = -1000.0f;


            spriteBatch = game.getSpriteBatch();
            screenWidth = game.getScreenWidth();
            screenHeight = game.getScreenHeight();

            // load textures
            humanTexture = game.getHumanTexture();
            zombieTexture = game.getZombieTexture();
            bonusTexture = game.getBonusTexture();
            bonusRocket = game.getBonusRocket();
            bonusAlien = game.getBonusAlien();
            blackHole = game.getDangerObject();
            animationTexture = game.getAnimationTexture();
            barrierTexture = game.getBarrierTexture();


            // different sizes for sprites
            drawRect = new Rectangle((int)Position.X, (int)Position.Y, 64, 64); 
            drawRect2 = new Rectangle((int)Position.X, (int)Position.Y, 80, 80); 
            

            base.LoadContent();
        }

        public void kill()
        {
            creatureState = cState.Dead;
        }

        public Vector2 getPosition()
        {
            return Position;
        }

        public override void Update(GameTime gameTime)
        {
            if (creatureState == cState.Dead) return;

            // animation loop, plays animation and then switches state to dead
            if (creatureState == cState.Animate)
            {
                elapsed += (float)gameTime.ElapsedGameTime.TotalMilliseconds;

                if (elapsed >= delay)
                {
                    if (frames >= 3)
                    {
                        frames = 0;
                        kill();
                    }
                    else
                    {
                        frames++;
                    }
                    elapsed = 0;
                }
                sourceRectangle = new Rectangle(64 * frames, 0,  64, 64);
                Position -= Direction;
            }


            // keep the sprites within screen boundaries
            if ((Position.X + 48 > screenWidth) && (Direction.X > 0.0f))
            {
                Direction.X = -Direction.X;
            }

            if ((Position.X < 0) && (Direction.X < 0.0f))
            {
                Direction.X = -Direction.X;
            }

            if ((Position.Y + 48 > screenHeight) && (Direction.Y > 0.0f))
            {
                Direction.Y = -Direction.Y;
            }

            if ((Position.Y < 0) && (Direction.Y < 0.0f))
            {
                Direction.Y = -Direction.Y;
            }

           
            drawRect.X = (int)Position.X;
            drawRect.Y = (int)Position.Y;
            
            Position += Direction;

            // keep the bonus, black hole, and asteroids in fixed position
            if (creatureState == cState.Bonus || creatureState == cState.DangerObject || creatureState == cState.Barrier)
            {
                Position -= Direction;
            }

           
           

            // top remaining rockets or aliens motion once the game is won by rockets or aliens
            currentRockets = game.getCurrentRockets();
            currentAliens = game.getCurrentAliens();

            if (currentRockets == 0 || currentAliens == 0)
            {
                if (creatureState == cState.Rocket || creatureState == cState.Alien || creatureState == cState.BonusRocket )
                {
                    Position -= Direction;
                }
                else if (creatureState == cState.BonusAlien)
                {
                    Position -= Direction;
                   
                }
            }

            updateKeyboardInput();


            closest = 25000;
            separationDistance = 25000;
            alignmentDistance = 200000;
            cohesionDistance = 500000;

        }

        void updateKeyboardInput()
        {

            KeyboardState keyboardState = Keyboard.GetState();


            if (gameState == 0)
            {

                if ((keyboardState.IsKeyDown(Keys.D)) && (!prevKeyboardState.IsKeyDown(Keys.D)))
                {
                    gameState = 1;
                }
            }

            // after D is pressed, the game is paused
            if (gameState == 1)
            {
                if (getState() == cState.Rocket || getState() == cState.BonusRocket)
                {
                    Position -= Direction;
                }

                if (getState() == cState.Alien || getState() == cState.BonusAlien)
                {
                    Position -= Direction;
                }

                // pressing s resumes the game
                if ((keyboardState.IsKeyDown(Keys.S)) && (!prevKeyboardState.IsKeyDown(Keys.S)))
                {
                    gameState = 0;
                }

            }

            // hold f to fastfoward 3x
            if ((keyboardState.IsKeyDown(Keys.F)))
            {

                if (creatureState == cState.BonusAlien)
                {
                    speed = 30.0f;
                }

                else if (creatureState == cState.BonusRocket)
                {
                    speed = 21.0f;
                }
                else
                { 
                    speed = 15.0f; 
                }

            }

            // set speeds if fastfoward feature is not used
            else
            {
                if (creatureState == cState.BonusAlien)
                {
                    speed = 10.0f;
                }

                else if (creatureState == cState.BonusRocket)
                {
                    speed = 7.0f;
                }
                else
                {
                    speed = 5.0f;
                }
            }


           


        }

        public override void Draw(GameTime gameTime)
        {


            switch (creatureState)
            {
                case cState.Dead:
                    return; // don't draw anything.

                case cState.Rocket:
                    spriteBatch.Draw(humanTexture, drawRect, Color.White);
                    break;

                case cState.Alien:
                    spriteBatch.Draw(zombieTexture, drawRect, Color.White);
                    break;


                case cState.Bonus:
                    spriteBatch.Draw(bonusTexture, drawRect, Color.White);
                    break;

                case cState.BonusRocket:
                    spriteBatch.Draw(bonusRocket, drawRect, Color.White);
                    break;

                case cState.BonusAlien:
                    spriteBatch.Draw(bonusAlien, drawRect, Color.White);
                    break;

                case cState.DangerObject:
                    spriteBatch.Draw(blackHole, drawRect2, Color.White);
                    break;


                case cState.Animate:
                    spriteBatch.Draw(animationTexture, drawRect, sourceRectangle, Color.White);
                    break;

                case cState.Barrier:
                    spriteBatch.Draw(barrierTexture, drawRect2, Color.White);
                    break;


            }

        }
    }
}
