using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace Project_Evo_Neural_Network_Prototype_1
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private Texture2D textBoxTexture;
        private NNMaker[,] NeuralNet = new NNMaker[500, 5];
        private NNManager NeuralManager = new NNManager();
        private NoiseMaker noiseMaker = new NoiseMaker();
        private float[][] perlinNoise;
        private float[,] numberOfCells;
        private Random myRandom = new Random();
        private const int SIZE_OF_CELL = 10;
        private bool showMap = false;
        private Movable[] creature = new Movable[500];
        private float[] inputs = new float[5];
        private int[] layers = new int[5];

        // These two are VERY simplistic counters to "limit" how long a creature is alive until it is killed off or reproduces
        private int frameCounter = 0;
        private int secondsCounter = 0;


        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            _graphics.PreferredBackBufferWidth = 1000;
            _graphics.PreferredBackBufferHeight = 1000;
            _graphics.ApplyChanges();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            textBoxTexture = Content.Load<Texture2D>("textRectangle");

            numberOfCells = new float[Window.ClientBounds.Width / SIZE_OF_CELL, Window.ClientBounds.Height / SIZE_OF_CELL];

            for(int i = 0; i < layers.Length; i++)
            {
                layers[i] = 5;
            }

            for (int i = 0; i < NeuralNet.GetUpperBound(0); i++)
            {
                for(int j = 0; j < NeuralNet.GetUpperBound(1); j++)
                {
                    NeuralNet[i, j] = new NNMaker(layers);
                }
            }

            for (int i = 0; i < creature.Length; i++)
            {
                creature[i] = new Movable(new Rectangle(myRandom.Next(0, numberOfCells.GetUpperBound(0)) * SIZE_OF_CELL, myRandom.Next(0, numberOfCells.GetUpperBound(1)) * SIZE_OF_CELL, SIZE_OF_CELL, SIZE_OF_CELL), textBoxTexture);
            }

            // TODO: use this.Content to load your game content here
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here

            if (Keyboard.GetState().IsKeyDown(Keys.Enter))
            {
                showMap = true;

                perlinNoise = noiseMaker.GeneratePerlinhNoise(noiseMaker.GenerateWhiteNoise(Window.ClientBounds.Width, Window.ClientBounds.Height, myRandom.Next(0, 99999999)), 4);

                for (int i = 0; i <= numberOfCells.GetUpperBound(0); i++)
                {
                    for (int j = 0; j <= numberOfCells.GetUpperBound(1); j++)
                    {
                        numberOfCells[i, j] = perlinNoise[i][j];
                    }
                }

            }

            if(showMap == true)
            {

                for(int i = 0; i < inputs.Length; i++)
                {
                    inputs[i] = 1;
                }

                if (frameCounter % 20 == 0)
                {

                    for (int i = 0; i < creature.Length - 1; i++)
                    {
                        for (int j = 0; j < layers.Length - 1; j++)
                        {
                            for (int k = 0; k < NeuralNet[i, j].FeedForward(inputs).GetUpperBound(0); k++)
                            {
                                if (creature[i].position.X / SIZE_OF_CELL + 1 >= numberOfCells.GetUpperBound(0))
                                {
                                    creature[i].MoveLeft(SIZE_OF_CELL);
                                }

                                else if (creature[i].position.X / SIZE_OF_CELL - 1 <= 0)
                                {
                                    creature[i].MoveRight(SIZE_OF_CELL);
                                }

                                else if (creature[i].position.Y / SIZE_OF_CELL - 1 <= 0)
                                {
                                    creature[i].MoveDown(SIZE_OF_CELL);
                                }

                                else if(creature[i].position.Y / SIZE_OF_CELL + 1 >= numberOfCells.GetUpperBound(1))
                                {
                                    creature[i].MoveUp(SIZE_OF_CELL);
                                }

                                else
                                {

                                    if (NeuralNet[i, 0].FeedForward(inputs)[k] >= 0)
                                    {
                                        if (numberOfCells[creature[i].position.X / SIZE_OF_CELL + 1, creature[i].position.Y / SIZE_OF_CELL] >= 0.95 && numberOfCells[creature[i].position.X / SIZE_OF_CELL + 1, creature[i].position.Y / SIZE_OF_CELL] <= 1)
                                        {
                                            creature[i].MoveLeft(SIZE_OF_CELL);
                                        }

                                        else
                                        {
                                            creature[i].MoveRight(SIZE_OF_CELL);
                                        }

                                    }

                                    if (NeuralNet[i, 1].FeedForward(inputs)[k] >= 0)
                                    {
                                        if (numberOfCells[creature[i].position.X / SIZE_OF_CELL - 1, creature[i].position.Y / SIZE_OF_CELL] >= 0.95 && numberOfCells[creature[i].position.X / SIZE_OF_CELL - 1, creature[i].position.Y / SIZE_OF_CELL] <= 1)
                                        {
                                            creature[i].MoveRight(SIZE_OF_CELL);
                                        }

                                        else
                                        {
                                            creature[i].MoveLeft(SIZE_OF_CELL);
                                        }
                                    }

                                    if (NeuralNet[i, 2].FeedForward(inputs)[k] >= 0)
                                    {
                                        if (numberOfCells[creature[i].position.X / SIZE_OF_CELL, creature[i].position.Y / SIZE_OF_CELL - 1] >= 0.95 && numberOfCells[creature[i].position.X / SIZE_OF_CELL, creature[i].position.Y / SIZE_OF_CELL - 1] <= 1)
                                        {
                                            creature[i].MoveDown(SIZE_OF_CELL);
                                        }

                                        else
                                        {
                                            creature[i].MoveUp(SIZE_OF_CELL);
                                        }
                                    }

                                    if (NeuralNet[i, 3].FeedForward(inputs)[k] >= 0)
                                    {
                                        if (numberOfCells[creature[i].position.X / SIZE_OF_CELL, creature[i].position.Y / SIZE_OF_CELL + 1] >= 0.95 && numberOfCells[creature[i].position.X / SIZE_OF_CELL, creature[i].position.Y / SIZE_OF_CELL + 1] <= 1)
                                        {
                                            creature[i].MoveUp(SIZE_OF_CELL);
                                        }

                                        else
                                        {
                                            creature[i].MoveDown(SIZE_OF_CELL);
                                        }
                                    }
                                }
                            }
                        }

                        if (secondsCounter >= 10)
                        {
                            if (creature[i].position.X / SIZE_OF_CELL > numberOfCells.GetUpperBound(0) / 4 * 3)
                            {
                                NeuralNet[i, 0].fitness = 1;
                            }
                        }
                    }

                }


                if (secondsCounter % 10 == 0 && frameCounter % 60 == 0)
                {

                    for (int i = 0; i < creature.GetUpperBound(0); i++)
                    {
                        if (NeuralNet[i, 0].fitness >= 1)
                        {
                            bool reproduced = false;

                            if (reproduced == false)
                            {
                                for (int j = 0; j < creature.GetUpperBound(0); j++)
                                {
                                    if (NeuralNet[j, 0].fitness < 1)
                                    {
                                        NeuralNet[j, 0] = NeuralNet[i, 0];
                                        NeuralNet[j, 1] = NeuralNet[i, 1];
                                        NeuralNet[j, 2] = NeuralNet[i, 2];
                                        NeuralNet[j, 3] = NeuralNet[i, 3];

                                        NeuralNet[j, 0].Mutate(100, 10);
                                        NeuralNet[j, 1].Mutate(100, 10);
                                        NeuralNet[j, 2].Mutate(100, 10);
                                        NeuralNet[j, 3].Mutate(100, 10);

                                        reproduced = true;
                                        j = creature.GetUpperBound(0);
                                    }
                                }
                            }
                        }

                        creature[i].position = new Rectangle(myRandom.Next(0, numberOfCells.GetUpperBound(0)) * SIZE_OF_CELL, myRandom.Next(0, numberOfCells.GetUpperBound(1)) * SIZE_OF_CELL, SIZE_OF_CELL, SIZE_OF_CELL);

                    }

                    for(int i = 0; i < NeuralNet.GetUpperBound(0); i++)
                    {
                        NeuralNet[i, 0].fitness = 0;
                    }

                }
            }

            frameCounter++;

            if(frameCounter >= 60)
            {
                secondsCounter++;
                frameCounter = 0;
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.White);

            // TODO: Add your drawing code here
            _spriteBatch.Begin();

            if (showMap == true)
            {
                for (int i = 0; i <= numberOfCells.GetUpperBound(0); i++)
                {
                    for (int j = 0; j <= numberOfCells.GetUpperBound(1); j++)
                    {
                        if (numberOfCells[i, j] < 0.5 && numberOfCells[i, j] > 0.4)
                        {
                            _spriteBatch.Draw(textBoxTexture, new Rectangle(SIZE_OF_CELL * i, SIZE_OF_CELL * j, SIZE_OF_CELL, SIZE_OF_CELL), Color.CornflowerBlue);
                        }
                        else if(numberOfCells[i, j] <= 0.4 && numberOfCells[i, j] > 0.3)
                        {
                            _spriteBatch.Draw(textBoxTexture, new Rectangle(SIZE_OF_CELL * i, SIZE_OF_CELL * j, SIZE_OF_CELL, SIZE_OF_CELL), Color.Blue);
                        }
                        else if(numberOfCells[i, j] <= 0.3)
                        {
                            _spriteBatch.Draw(textBoxTexture, new Rectangle(SIZE_OF_CELL * i, SIZE_OF_CELL * j, SIZE_OF_CELL, SIZE_OF_CELL), Color.DarkBlue);
                        }
                        else if (numberOfCells[i, j] >= 0.5 && numberOfCells[i, j] < 0.8)
                        {
                            _spriteBatch.Draw(textBoxTexture, new Rectangle(SIZE_OF_CELL * i, SIZE_OF_CELL * j, SIZE_OF_CELL, SIZE_OF_CELL), Color.Beige);
                        }
                        else if (numberOfCells[i, j] >= 0.95 && numberOfCells[i, j] <= 1)
                        {
                            _spriteBatch.Draw(textBoxTexture, new Rectangle(SIZE_OF_CELL * i, SIZE_OF_CELL * j, SIZE_OF_CELL, SIZE_OF_CELL), Color.DarkSlateGray);
                        }
                        else
                        {
                            _spriteBatch.Draw(textBoxTexture, new Rectangle(SIZE_OF_CELL * i, SIZE_OF_CELL * j, SIZE_OF_CELL, SIZE_OF_CELL), Color.White);
                        }
                    }
                }

                for(int i = 0; i < creature.Length; i++)
                {
                    _spriteBatch.Draw(creature[i].texture, creature[i].position, Color.Red);
                }

            }

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
