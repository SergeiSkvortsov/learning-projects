using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace OptionalProject
{
    /// <remarks>
    /// A number tile
    /// </remarks>
    class NumberTile
    {
        #region Fields

        // original length of each side of the tile
        int originalSideLength;

        // whether or not this tile is the correct number
        bool isCorrectNumber;

        // drawing support
        Texture2D texture;
        Rectangle drawRectangle;
        Rectangle sourceRectangle;

        // Increment 5: field for blinking tile texture
        Texture2D blinkingTileTexture;

        // Increment 5: field for current texture
        Texture2D currentTexture;

        // blinking support
        const int TotalBlinkMilliseconds = 4000;
        int elapsedBlinkMilliseconds = 0;
        const int FrameBlinkMilliseconds = 500;
        int elapsedFrameMilliseconds = 0;

        // Increment 4: fields for shrinking support
        const int TotalShrinkMilliseconds = 1000;
        int elapsedShrinkMilliseconds = 0;

        // Increment 4: fields to keep track of visible, blinking, and shrinking
        bool isVisible = true;
        bool isBlinking = false;
        bool isShrinking = false;

        // Increment 4: fields for click support
        bool clickStarted = false;
        bool buttonReleased = true;

        // Increment 5: sound effect field
        SoundEffect sound;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="contentManager">the content manager</param>
        /// <param name="center">the center of the tile</param>
        /// <param name="sideLength">the side length for the tile</param>
        /// <param name="number">the number for the tile</param>
        /// <param name="correctNumber">the correct number</param>
        public NumberTile(ContentManager contentManager, Vector2 center, int sideLength,
            int number, int correctNumber)
        {
            // set original side length field
            this.originalSideLength = sideLength;

            // load content for the tile and create draw rectangle
            LoadContent(contentManager, number);
            drawRectangle = new Rectangle((int)center.X - sideLength / 2,
                 (int)center.Y - sideLength / 2, sideLength, sideLength);

            // set isCorrectNumber flag
            isCorrectNumber = number == correctNumber;

            // Increment 5: load sound effect field to correct or incorrect sound effect
            // based on whether or not this tile is the correct number
            if (isCorrectNumber)
            {
                sound = contentManager.Load<SoundEffect>("audio\\explosion");
            }
            else
            {
                sound = contentManager.Load<SoundEffect>("audio\\loser");
            }
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Updates the tile based on game time and mouse state
        /// </summary>
        /// <param name="gameTime">the current GameTime</param>
        /// <param name="mouse">the current mouse state</param>
        /// <return>true if the correct number was guessed, false otherwise</return>
        public bool Update(GameTime gameTime, MouseState mouse)
        {

            // Increments 4 and 5: add code for shrinking and blinking support
            if (isBlinking)
            {
                elapsedBlinkMilliseconds += gameTime.ElapsedGameTime.Milliseconds;
                if (elapsedBlinkMilliseconds >= TotalBlinkMilliseconds)
                {
                    isBlinking = false;
                    isVisible = false;
                    return true;
                }
                else
                {
                    elapsedFrameMilliseconds += gameTime.ElapsedGameTime.Milliseconds;
                    if (elapsedFrameMilliseconds >= FrameBlinkMilliseconds && sourceRectangle.X == texture.Width / 2)
                    {
                        sourceRectangle.X = 0;
                        elapsedFrameMilliseconds = 0;
                    }
                    else if (elapsedFrameMilliseconds >= FrameBlinkMilliseconds && sourceRectangle.X == 0)
                    {
                        sourceRectangle.X = texture.Width / 2;
                        elapsedFrameMilliseconds = 0;
                    }
                }
            }
            else if (isShrinking)
            {
                elapsedShrinkMilliseconds += gameTime.ElapsedGameTime.Milliseconds;
                float newSideLength = (float)originalSideLength * (float)(TotalShrinkMilliseconds - elapsedShrinkMilliseconds) / TotalShrinkMilliseconds;
                if (newSideLength > 0)
                {
                    drawRectangle.Width = (int)newSideLength;
                    drawRectangle.Height = (int)newSideLength;
                }
                else
                {
                    isVisible = false;
                }
            }
            else
            {
                // Increment 4: add code to highlight/unhighlight the tile
                // check for mouse over button
                if (drawRectangle.Contains(mouse.X, mouse.Y))
                {
                    // highlight button
                    sourceRectangle.X = texture.Width / 2;

                    // check for click started on button
                    if (mouse.LeftButton == ButtonState.Pressed &&
                        buttonReleased)
                    {
                        clickStarted = true;
                        buttonReleased = false;
                    }
                    else if (mouse.LeftButton == ButtonState.Released)
                    {
                        buttonReleased = true;

                        // if click finished on button, change game state
                        if (clickStarted)
                        {
                            if (isCorrectNumber)
                            {
                                isBlinking = true;
                                currentTexture = blinkingTileTexture;
                                sourceRectangle.X = 0;
                            }
                            else
                            {
                                isShrinking = true;
                            }
                            sound.Play();
                            clickStarted = false;
                        }
                    }
                }
                else
                {
                    sourceRectangle.X = 0;
                }
            }

            // Increment 5: play sound effect

            // if we get here, return false
            return false;
        }

        /// <summary>
        /// Draws the number tile
        /// </summary>
        /// <param name="spriteBatch">the SpriteBatch to use for the drawing</param>
        public void Draw(SpriteBatch spriteBatch)
        {
            // Increments 3, 4, and 5: draw the tile
            if (isVisible)
            {
                spriteBatch.Draw(currentTexture, drawRectangle, sourceRectangle, Color.White);
            }
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Loads the content for the tile
        /// </summary>
        /// <param name="contentManager">the content manager</param>
        /// <param name="number">the tile number</param>
        private void LoadContent(ContentManager contentManager, int number)
        {
            // convert the number to a string
            string numberString = ConvertIntToString(number);

            // Increment 3: load content for the tile and set source rectangle
            texture = contentManager.Load<Texture2D>("graphics\\" + numberString);
            sourceRectangle = new Rectangle(drawRectangle.X, drawRectangle.Y, texture.Width/2, texture.Height);

            // Increment 5: load blinking tile texture
            blinkingTileTexture = contentManager.Load<Texture2D>("graphics\\blinking" + numberString);

            // Increment 5: set current texture
            currentTexture = texture;
        }

        /// <summary>
        /// Converts an integer to a string for the corresponding number
        /// </summary>
        /// <param name="number">the integer to convert</param>
        /// <returns>the string for the corresponding number</returns>
        private String ConvertIntToString(int number)
        {
            switch (number)
            {
                case 1:
                    return "one";
                case 2:
                    return "two";
                case 3:
                    return "three";
                case 4:
                    return "four";
                case 5:
                    return "five";
                case 6:
                    return "six";
                case 7:
                    return "seven";
                case 8:
                    return "eight";
                case 9:
                    return "nine";
                default:
                    throw new Exception("Unsupported number for number tile");
            }

        }

        #endregion
    }
}
