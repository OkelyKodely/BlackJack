using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

/// <summary>
/// This is the main type for your game
/// </summary>
public class Game1 : Microsoft.Xna.Framework.Game
{
    String winnerText;

    GraphicsDeviceManager graphics;

    SpriteFont spriteFont, spriteFont2;
        
    KeyboardState _currentKeyboardState;
        
    KeyboardState _previousKeyboardState;
        
    SpriteBatch spriteBatch;
        
    Texture2D stayBtn, hitBtn, dealBtn;
        
    MouseState _previousMouseState, _currentMouseState;
        
    Rectangle sizeStayBtn, sizeHitBtn, sizeDealBtn, sizeShuffle, sizeS;
        
    DeckOfCards deckOfCards;
        
    List<Card> house;
    
    List<Card> player;
        
    Boolean starting = true;

    Boolean dealt = false;
        
    Boolean evaluateReconcile = true;
    
    Boolean stay = false;
    
    Boolean houseDone = false;
        
    GifAnimation.GifAnimation rainbow, shuffle, s;
        
    int houseCardsValue;
        
    int playerCardsValue;
        
    int currentCard = -1;
        
    int totalAmount = 975;
        
    int amount = 25;

    int total;
        
    int placeHouse;
    
    int placePlayer;

    int countHouseAces = 0;

    int countPlayerAces = 0;

    int elpsed = 0, elpsedlast = 0;

    public Game1()
    {
        this.IsMouseVisible = true;

        graphics = new GraphicsDeviceManager(this);
  
        graphics.PreferredBackBufferWidth = 1160;
        
        graphics.PreferredBackBufferHeight = 600;
            
        Content.RootDirectory = "Content";
    }

    //This below is required paste it somewhere.
    protected static void GetMBResult(IAsyncResult r)
    {
        int? b = Guide.EndShowMessageBox(r);
    }

    /// <summary>
    /// Allows the game to perform any initialization it needs to before starting to run.
    /// This is where it can query for any required services and load any non-graphic
    /// related content.  Calling base.Initialize will enumerate through any components
    /// and initialize them as well.
    /// </summary>
    protected override void Initialize()
    {
        // TODO: Add your initialization logic here
        base.Initialize();
        
        SpriteBatchEx.GraphicsDevice = GraphicsDevice;
    }

    /// <summary>
    /// LoadContent will be called once per game and is the place to load
    /// all of your content.
    /// </summary>
    protected override void LoadContent()
    {
        // Create a new SpriteBatch, which can be used to draw textures.
        spriteBatch = new SpriteBatch(GraphicsDevice);

        // TODO: use this.Content to load your game content here
        rainbow = Content.Load<GifAnimation.GifAnimation>("rainbow");

        shuffle = Content.Load<GifAnimation.GifAnimation>("shuffle");

        s = Content.Load<GifAnimation.GifAnimation>("s");

        sizeShuffle = new Rectangle(0, 0, 0, 0);

        dealBtn = Content.Load<Texture2D>("deal");
        
        hitBtn = Content.Load<Texture2D>("hit");
        
        stayBtn = Content.Load<Texture2D>("stay");
        
        spriteFont = Content.Load<SpriteFont>("SpriteFont1");

        spriteFont2 = Content.Load<SpriteFont>("SpriteFont2");

        _currentMouseState = Mouse.GetState();
        
        _previousMouseState = _currentMouseState;

        deckOfCards = new DeckOfCards();
        
        house = new List<Card>();
        
        player = new List<Card>();

        this.stay = true;
        
        this.houseDone = true;
    }

    /// <summary>
    /// UnloadContent will be called once per game and is the place to unload
    /// all content.
    /// </summary>
    protected override void UnloadContent()
    {
        // TODO: Unload any non ContentManager content here
    }

    /// <summary>
    /// Allows the game to run logic such as updating the world,
    /// checking for collisions, gathering input, and playing audio.
    /// </summary>
    /// <param name="gameTime">Provides a snapshot of timing values.</param>
    protected override void Update(GameTime gameTime)
    {
        _previousMouseState = _currentMouseState;
        
        _currentMouseState = Mouse.GetState();

        // Allows the game to exit
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
            this.Exit();

        rainbow.Update(gameTime.ElapsedGameTime.Ticks);

        shuffle.Update(gameTime.ElapsedGameTime.Ticks);

        s.Update(gameTime.ElapsedGameTime.Ticks);

        this.elpsed = (int)(gameTime.TotalGameTime.Ticks);

        int interval = this.elpsed - this.elpsedlast;

        if (interval > 57350987 && sizeShuffle.Width != 0)
        {
            sizeShuffle = new Rectangle(0, 0, 0, 0);

            sizeS = new Rectangle(0, 0, 0, 0);

            Deal();
        }

        // Before handling input
        _currentKeyboardState = Keyboard.GetState();

        if (_currentKeyboardState.IsKeyDown(Keys.F) && _previousKeyboardState.IsKeyUp(Keys.F))
        {
            this.graphics.ToggleFullScreen();
        }

        if (!dealt)
        {
            if (_currentKeyboardState.IsKeyDown(Keys.B) && _previousKeyboardState.IsKeyUp(Keys.B))
            {
                if (this.totalAmount > 0)
                {
                    this.amount += 25;

                    this.totalAmount -= 25;

                    if (this.totalAmount < 0)
                    {
                        this.totalAmount += 25;

                        this.amount -= 25;
                    }

                    ShowAmountBet(this.amount);

                    ShowTotalAmount(this.totalAmount);
                }
            }
            else if (_currentKeyboardState.IsKeyDown(Keys.U) && _previousKeyboardState.IsKeyUp(Keys.U))
            {
                this.amount -= 25;

                if (this.amount >= 0)
                {
                    this.totalAmount += 25;

                    if (this.amount < 25)
                    {
                        this.totalAmount -= 25;

                        this.amount = 25;
                    }
                }

                ShowAmountBet(this.amount);

                ShowTotalAmount(this.totalAmount);
            }
        }

        // After handling input
        _previousKeyboardState = Keyboard.GetState();

        // TODO: Add your update logic here
        // Recognize a single click of the left mouse button
        if (_previousMouseState.LeftButton == ButtonState.Released && _currentMouseState.LeftButton == ButtonState.Pressed)
        {
            // Check if the mouse position is inside the rectangle
            var mousePosition = new Point(_currentMouseState.X, _currentMouseState.Y);

            Rectangle someRectangle = new Rectangle(22, 330, 45, 61);
                
            Rectangle area = someRectangle;
                
            if (area.Contains(mousePosition) && !dealt)
            {

                DealWithIt(gameTime.ElapsedGameTime.Ticks);
                
            }
        }

        Play();

        if (this.stay == true && this.evaluateReconcile)
        {
            HousePlay();

            if (this.houseDone == true)
            {
                Reconcile();
            }
        }
        else if (!this.evaluateReconcile)
        {
            Reconcile();
        }
            
        base.Update(gameTime);
    }

    private void HousePlay()
    {
        HouseHit();
    }

    private void Play()
    {
        sizeDealBtn = new Rectangle(22, 330, 45, 61);

        sizeHitBtn = new Rectangle(12, 420, 65, 81);

        sizeStayBtn = new Rectangle(12, 510, 65, 81);

        if (_previousMouseState.LeftButton == ButtonState.Released && _currentMouseState.LeftButton == ButtonState.Pressed)
        {
            // Check if the mouse position is inside the rectangle
            var mousePosition = new Point(_currentMouseState.X, _currentMouseState.Y);

            Rectangle someRectangle = new Rectangle(22, 420, 45, 61);
                
            Rectangle area = someRectangle;
                
            if (area.Contains(mousePosition) && !this.Window.Title.Contains("~"))
            {
                Hit();
            }
        }

        if (_previousMouseState.LeftButton == ButtonState.Released && _currentMouseState.LeftButton == ButtonState.Pressed)
        {
            // Check if the mouse position is inside the rectangle
            var mousePosition = new Point(_currentMouseState.X, _currentMouseState.Y);

            Rectangle someRectangle = new Rectangle(22, 510, 45, 61);
                
            Rectangle area = someRectangle;
                
            if (area.Contains(mousePosition))
            {
                this.stay = true;
                
                this.evaluateReconcile = true;
            }
        }
    }

    private void HouseHit()
    {
        this.total = 0;

        for(int i=0; i<house.Count; i++)
        {
            total += house[i].getRank();
            if (house[i].getRank() == 1)
            {
                ++countHouseAces;
                total += 10;
            }
            if (house[i].getRank() == 11)
            {
                total -= 1;
            }
            if (house[i].getRank() == 12)
            {
                total -= 2;
            }
            if (house[i].getRank() == 13)
            {
                total -= 3;
            }
        }

        if (total > 16)
        {
            while (true)
            {
                if (this.total > 21)
                {
                    if (this.countHouseAces > 0)
                    {
                        --this.countHouseAces;
                        total -= 10;
                    }
                    if (this.total < 22)
                        break;
                    if (this.countHouseAces == 1 && total > 21)
                    {
                        --this.countHouseAces;
                        total -= 10;
                        break;
                    }
                    if (this.countHouseAces <= 1)
                        break;
                }
                else
                {
                    break;
                }
            }

            this.houseDone = false;
        }

        if (total < 17 || (total > 16 && this.house.Count > 2 && total < this.playerCardsValue && this.playerCardsValue < 22))
        {
            placeHouse += 25;

            ++currentCard;
        
            if (currentCard == 52)
                currentCard = 0;

            Card kard = deckOfCards.getCards()[currentCard];

            house.Add(kard);

            total += kard.getRank();
            if (kard.getRank() == 1)
            {
                total += 10;
                this.countHouseAces++;
            }
            if (kard.getRank() == 11)
            {
                total -= 1;
            }
            if (kard.getRank() == 12)
            {
                total -= 2;
            }
            if (kard.getRank() == 13)
            {
                total -= 3;
            }
        }

        this.houseCardsValue = total;

        if (total > 16)
        {
            while (true)
            {
                if (this.houseCardsValue > 21)
                {
                    if (this.countHouseAces > 0)
                    {
                        --this.countHouseAces;
                        this.houseCardsValue -= 10;
                    }
                    if (this.houseCardsValue < 22)
                        break;
                    if (this.countHouseAces == 1 && this.houseCardsValue > 21)
                    {
                        --this.countHouseAces;
                        this.houseCardsValue -= 10;
                        break;
                    }
                    if (this.countHouseAces <= 1)
                        break;
                }
                else
                {
                    break;
                }
            }

            this.houseDone = false;
        }

        total = this.houseCardsValue;

        if (total > 16 && total < 22 && !(this.houseCardsValue > 16 && this.house.Count > 2 && this.houseCardsValue < this.playerCardsValue && this.playerCardsValue < 22))
        {
            this.houseDone = true;
        }

        if (total > 21)
        {
            this.houseDone = true;
        }
    }

    private void Hit()
    {
        if (!dealt)
            return;

        placePlayer += 25;

        ++currentCard;

        if (currentCard == 52)
            currentCard = 0;

        Card kard = deckOfCards.getCards()[currentCard];

        player.Add(kard);

        this.playerCardsValue = 0;

        this.countPlayerAces = 0;

        for (int i = 0; i < player.Count; i++)
        {
            Card card = player[i];
            
            this.playerCardsValue += card.getRank();
            
            if (card.getRank() == 1)
            {
                this.playerCardsValue += 10;
                ++this.countPlayerAces;
            }
            if (card.getRank() == 11)
            {
                this.playerCardsValue -= 1;
            }
            if (card.getRank() == 12)
            {
                this.playerCardsValue -= 2;
            }
            if (card.getRank() == 13)
            {
                this.playerCardsValue -= 3;
            }
        }
        while (true)
        {
            if (this.playerCardsValue > 21)
            {
                if (this.countPlayerAces > 0)
                {
                    --this.countPlayerAces;
                    this.playerCardsValue -= 10;
                }
                if (this.playerCardsValue < 22)
                    break;
                if (this.countPlayerAces == 1 && this.playerCardsValue > 21)
                {
                    --this.countPlayerAces;
                    this.playerCardsValue -= 10;
                    break;
                }
                else if (this.countPlayerAces <= 1)
                    break;
            }
            else
            {
                break;
            }
        }
        if (this.playerCardsValue > 21)
        {
            this.stay = true;
            
            this.houseDone = true;
                
            this.evaluateReconcile = false;
        }
        else
        {
            this.evaluateReconcile = true;
        }
    }

    private void Reconcile()
    {
        sizeDealBtn = new Rectangle(12, 330, 65, 81);

        sizeHitBtn = new Rectangle(22, 420, 45, 61);

        sizeStayBtn = new Rectangle(22, 510, 45, 61);

        //this.houseCardsValue = this.total;

        if (this.houseCardsValue > this.playerCardsValue && this.houseCardsValue < 22)
        {
            this.winnerText = "Winner is House~";

            if (!this.starting && houseDone)
            {
                this.totalAmount -= 25;

                this.amount = 25;

                houseDone = false;
            }
        }
        else if (this.playerCardsValue > 21 && this.houseCardsValue < 22)
        {
            this.winnerText = "Winner is House~";

            if (!this.starting && houseDone)
            {
                this.totalAmount -= 25;

                this.amount = 25;

                houseDone = false;
            }
        }
        else if (this.playerCardsValue > this.houseCardsValue && this.playerCardsValue < 22)
        {
            this.winnerText = "Winner is Player~";

            if(this.stay || this.houseDone)
                this.totalAmount += this.amount + this.amount;

            if(!this.starting && houseDone)
                this.amount = 25;

            if (!this.starting && houseDone)
                this.totalAmount -= 25;

            this.stay = false;
            
            this.houseDone = false;
        }
        else if (this.houseCardsValue > 21 && this.playerCardsValue < 22)
        {
            this.winnerText = "Winner is Player~";
                
            if (this.stay || this.houseDone)
                this.totalAmount += this.amount + this.amount;

            if (!this.starting && houseDone)
                this.amount = 25;

            if (!this.starting && houseDone)
                this.totalAmount -= 25;

            this.stay = false;
            
            this.houseDone = false;
        }
        else
        {
            if (this.houseCardsValue == this.playerCardsValue)
            {
                this.winnerText = "Push~";
            }

            if (this.stay || this.houseDone)
                this.totalAmount += this.amount;

            if (!this.starting && houseDone)
                this.amount = 25;

            if (!this.starting && houseDone)
                this.totalAmount -= 25;

            this.stay = false;

            this.houseDone = false;
        }

        this.ShowAmountBet(this.amount);

        this.ShowTotalAmount(this.totalAmount);

        this.evaluateReconcile = false;

        if (this.starting)
        {
            this.winnerText = "";
        }

        if (this.totalAmount <= 0 && this.amount <= 0)
                
            this.totalAmount = 1000;

        if (this.amount == 25 && this.totalAmount == -25)
        {
            this.amount = 0;

            this.totalAmount = 0;
        }

        this.dealt = false;
    }

    private void DealWithIt(long elapsed)
    {
        if (!(this.amount >= 25 && this.totalAmount >= 0))
        {
            return;
        }

        this.starting = false;

        this.dealt = true;

        this.stay = false;

        this.houseDone = false;

        this.evaluateReconcile = true;

        placeHouse = 0;

        placePlayer = 0;

        this.houseCardsValue = 0;

        this.playerCardsValue = 0;

        winnerText = "Playing now...";

        house.Clear();

        player.Clear();

        if (this.currentCard < 8)
        {
            this.elpsedlast = this.elpsed;

            this.sizeShuffle = new Rectangle(100, 240, 240, 160);

            this.sizeS = new Rectangle(500, 100, 440, 660);

            this.ShuffleIt(elapsed);

            this.currentCard = 8;
        }
        else
        {
            Deal();
        }
    }

    private void ShuffleIt(long elapsed)
    {
        //for (int i = 0; i < 6; i++)
            Shuffle(elapsed);
    }

    private void Shuffle(long elapsed)
    {
        Card[] oldCards = this.deckOfCards.getCards();

        Card[] newCards = FisherYates.Shuffle(oldCards);

        this.deckOfCards.setCards(newCards);
    }

    private void Deal()
    {
        Card kard;
        ++currentCard;
        
        if (currentCard == 52)
            currentCard = 0;
        
        kard = deckOfCards.getCards()[currentCard];
        house.Add(kard);

        ++currentCard;
        
        if (currentCard == 52)
            currentCard = 0;
        
        kard = deckOfCards.getCards()[currentCard];
        house.Add(kard);

        Card mard;
        ++currentCard;
        
        if (currentCard == 52)
            currentCard = 0;
        
        mard = deckOfCards.getCards()[currentCard];
        player.Add(mard);

        ++currentCard;
        
        if (currentCard == 52)
            currentCard = 0;
        
        mard = deckOfCards.getCards()[currentCard];
        
        player.Add(mard);

        this.houseCardsValue = 0;
        
        this.playerCardsValue = 0;

        for (int i = 0; i < house.Count; i++)
        {
            Card card = house[i];
            
            this.houseCardsValue += card.getRank();
            
            if (card.getRank() == 1)
            {
                this.houseCardsValue += 10;
            }
            if (card.getRank() == 11)
            {
                this.houseCardsValue -= 1;
            }
            if (card.getRank() == 12)
            {
                this.houseCardsValue -= 2;
            }
            if (card.getRank() == 13)
            {
                this.houseCardsValue -= 3;
            }
        }

        for (int i = 0; i < player.Count; i++)
        {
            Card card = player[i];
            
            this.playerCardsValue += card.getRank();
            
            if (card.getRank() == 1)
            {
                this.playerCardsValue += 10;
            }
            if (card.getRank() == 11)
            {
                this.playerCardsValue -= 1;
            }
            if (card.getRank() == 12)
            {
                this.playerCardsValue -= 2;
            }
            if (card.getRank() == 13)
            {
                this.playerCardsValue -= 3;
            }
        }

        if (this.house[0].getRank() == 1 && this.house[1].getRank() == 1)
        {
            if (this.houseCardsValue == 22)
            {
                this.houseCardsValue = this.houseCardsValue - 10;
            }
        }

        if (this.player[0].getRank() == 1 && this.player[1].getRank() == 1)
        {
            if (this.playerCardsValue == 22)
            {
                this.playerCardsValue = this.playerCardsValue - 10;
            }
        }    
    }

    private void ShowAmountBet(int amount)
    {
        this.Window.Title = "Bet Amount: $" + amount;
    }

    private void ShowTotalAmount(int amount)
    {
        this.Window.Title = this.Window.Title + ", Total Amount: $" + amount;
    }
        
    /// <summary>
    /// This is called when the game should draw itself.
    /// </summary>
    /// <param name="gameTime">Provides a snapshot of timing values.</param>
    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.YellowGreen);

        spriteBatch.Begin();
        Texture2D rect = new Texture2D(graphics.GraphicsDevice, 90, 600);

        Color[] data = new Color[90 * 600];
        for (int i = 0; i < data.Length; ++i) data[i] = Color.White;
        rect.SetData(data);

        Vector2 coor = new Vector2(0, 0);
        spriteBatch.Draw(rainbow.GetTexture(), new Rectangle(0, 0, 1160, 600), Color.White);
        spriteBatch.Draw(rect, coor, Color.White);
        spriteBatch.Draw(stayBtn, sizeStayBtn, Color.White);
        spriteBatch.Draw(hitBtn, sizeHitBtn, Color.White);
        spriteBatch.Draw(dealBtn, sizeDealBtn, Color.White);

        SpriteBatchEx.DrawLine(spriteBatch, new Vector2(100, 10), new Vector2(250, 10), Color.White, 1);
        SpriteBatchEx.DrawLine(spriteBatch, new Vector2(100, 160), new Vector2(250, 160), Color.White, 1);
        SpriteBatchEx.DrawLine(spriteBatch, new Vector2(100, 10), new Vector2(100, 160), Color.White, 1);
        SpriteBatchEx.DrawLine(spriteBatch, new Vector2(250, 10), new Vector2(250, 160), Color.White, 1);

        SpriteBatchEx.DrawLine(spriteBatch, new Vector2(100 + 151, 10), new Vector2(250 + 151, 10), Color.White, 1);
        SpriteBatchEx.DrawLine(spriteBatch, new Vector2(100 + 151, 160), new Vector2(250 + 151, 160), Color.White, 1);
        SpriteBatchEx.DrawLine(spriteBatch, new Vector2(100 + 151, 10), new Vector2(100 + 151, 160), Color.White, 1);
        SpriteBatchEx.DrawLine(spriteBatch, new Vector2(250 + 151, 10), new Vector2(250 + 151, 160), Color.White, 1);

        SpriteBatchEx.DrawLine(spriteBatch, new Vector2(100 + 301, 10), new Vector2(250 + 301, 10), Color.White, 1);
        SpriteBatchEx.DrawLine(spriteBatch, new Vector2(100 + 301, 160), new Vector2(250 + 301, 160), Color.White, 1);
        SpriteBatchEx.DrawLine(spriteBatch, new Vector2(100 + 301, 10), new Vector2(100 + 301, 160), Color.White, 1);
        SpriteBatchEx.DrawLine(spriteBatch, new Vector2(250 + 301, 10), new Vector2(250 + 301, 160), Color.White, 1);

        SpriteBatchEx.DrawLine(spriteBatch, new Vector2(100 + 451, 10), new Vector2(250 + 451, 10), Color.White, 1);
        SpriteBatchEx.DrawLine(spriteBatch, new Vector2(100 + 451, 160), new Vector2(250 + 451, 160), Color.White, 1);
        SpriteBatchEx.DrawLine(spriteBatch, new Vector2(100 + 451, 10), new Vector2(100 + 451, 160), Color.White, 1);
        SpriteBatchEx.DrawLine(spriteBatch, new Vector2(250 + 451, 10), new Vector2(250 + 451, 160), Color.White, 1);

        SpriteBatchEx.DrawLine(spriteBatch, new Vector2(100 + 601, 10), new Vector2(250 + 601, 10), Color.White, 1);
        SpriteBatchEx.DrawLine(spriteBatch, new Vector2(100 + 601, 160), new Vector2(250 + 601, 160), Color.White, 1);
        SpriteBatchEx.DrawLine(spriteBatch, new Vector2(100 + 601, 10), new Vector2(100 + 601, 160), Color.White, 1);
        SpriteBatchEx.DrawLine(spriteBatch, new Vector2(250 + 601, 10), new Vector2(250 + 601, 160), Color.White, 1);

        SpriteBatchEx.DrawLine(spriteBatch, new Vector2(100 + 751, 10), new Vector2(250 + 751, 10), Color.White, 1);
        SpriteBatchEx.DrawLine(spriteBatch, new Vector2(100 + 751, 160), new Vector2(250 + 751, 160), Color.White, 1);
        SpriteBatchEx.DrawLine(spriteBatch, new Vector2(100 + 751, 10), new Vector2(100 + 751, 160), Color.White, 1);
        SpriteBatchEx.DrawLine(spriteBatch, new Vector2(250 + 751, 10), new Vector2(250 + 751, 160), Color.White, 1);

        SpriteBatchEx.DrawLine(spriteBatch, new Vector2(100 + 901, 10), new Vector2(250 + 901, 10), Color.White, 1);
        SpriteBatchEx.DrawLine(spriteBatch, new Vector2(100 + 901, 160), new Vector2(250 + 901, 160), Color.White, 1);
        SpriteBatchEx.DrawLine(spriteBatch, new Vector2(100 + 901, 10), new Vector2(100 + 901, 160), Color.White, 1);
        SpriteBatchEx.DrawLine(spriteBatch, new Vector2(250 + 901, 10), new Vector2(250 + 901, 160), Color.White, 1);

        SpriteBatchEx.DrawLine(spriteBatch, new Vector2(100, 410), new Vector2(250, 410), Color.White, 1);
        SpriteBatchEx.DrawLine(spriteBatch, new Vector2(100, 560), new Vector2(250, 560), Color.White, 1);
        SpriteBatchEx.DrawLine(spriteBatch, new Vector2(100, 410), new Vector2(100, 560), Color.White, 1);
        SpriteBatchEx.DrawLine(spriteBatch, new Vector2(250, 410), new Vector2(250, 560), Color.White, 1);

        SpriteBatchEx.DrawLine(spriteBatch, new Vector2(100 + 151, 410), new Vector2(250 + 151, 410), Color.White, 1);
        SpriteBatchEx.DrawLine(spriteBatch, new Vector2(100 + 151, 560), new Vector2(250 + 151, 560), Color.White, 1);
        SpriteBatchEx.DrawLine(spriteBatch, new Vector2(100 + 151, 410), new Vector2(100 + 151, 560), Color.White, 1);
        SpriteBatchEx.DrawLine(spriteBatch, new Vector2(250 + 151, 410), new Vector2(250 + 151, 560), Color.White, 1);

        SpriteBatchEx.DrawLine(spriteBatch, new Vector2(100 + 301, 410), new Vector2(250 + 301, 410), Color.White, 1);
        SpriteBatchEx.DrawLine(spriteBatch, new Vector2(100 + 301, 560), new Vector2(250 + 301, 560), Color.White, 1);
        SpriteBatchEx.DrawLine(spriteBatch, new Vector2(100 + 301, 410), new Vector2(100 + 301, 560), Color.White, 1);
        SpriteBatchEx.DrawLine(spriteBatch, new Vector2(250 + 301, 410), new Vector2(250 + 301, 560), Color.White, 1);

        SpriteBatchEx.DrawLine(spriteBatch, new Vector2(100 + 451, 410), new Vector2(250 + 451, 410), Color.White, 1);
        SpriteBatchEx.DrawLine(spriteBatch, new Vector2(100 + 451, 560), new Vector2(250 + 451, 560), Color.White, 1);
        SpriteBatchEx.DrawLine(spriteBatch, new Vector2(100 + 451, 410), new Vector2(100 + 451, 560), Color.White, 1);
        SpriteBatchEx.DrawLine(spriteBatch, new Vector2(250 + 451, 410), new Vector2(250 + 451, 560), Color.White, 1);

        SpriteBatchEx.DrawLine(spriteBatch, new Vector2(100 + 601, 410), new Vector2(250 + 601, 410), Color.White, 1);
        SpriteBatchEx.DrawLine(spriteBatch, new Vector2(100 + 601, 560), new Vector2(250 + 601, 560), Color.White, 1);
        SpriteBatchEx.DrawLine(spriteBatch, new Vector2(100 + 601, 410), new Vector2(100 + 601, 560), Color.White, 1);
        SpriteBatchEx.DrawLine(spriteBatch, new Vector2(250 + 601, 410), new Vector2(250 + 601, 560), Color.White, 1);

        SpriteBatchEx.DrawLine(spriteBatch, new Vector2(100 + 751, 410), new Vector2(250 + 751, 410), Color.White, 1);
        SpriteBatchEx.DrawLine(spriteBatch, new Vector2(100 + 751, 560), new Vector2(250 + 751, 560), Color.White, 1);
        SpriteBatchEx.DrawLine(spriteBatch, new Vector2(100 + 751, 410), new Vector2(100 + 751, 560), Color.White, 1);
        SpriteBatchEx.DrawLine(spriteBatch, new Vector2(250 + 751, 410), new Vector2(250 + 751, 560), Color.White, 1);

        SpriteBatchEx.DrawLine(spriteBatch, new Vector2(100 + 901, 410), new Vector2(250 + 901, 410), Color.White, 1);
        SpriteBatchEx.DrawLine(spriteBatch, new Vector2(100 + 901, 560), new Vector2(250 + 901, 560), Color.White, 1);
        SpriteBatchEx.DrawLine(spriteBatch, new Vector2(100 + 901, 410), new Vector2(100 + 901, 560), Color.White, 1);
        SpriteBatchEx.DrawLine(spriteBatch, new Vector2(250 + 901, 410), new Vector2(250 + 901, 560), Color.White, 1);

        int counter = 100;
        int width = 150;
        if (this.starting)
        {
            width = 0;
            this.houseCardsValue = 0;
        }
        for (int i = 0; i < house.Count && house.Count != 1; i++)
        {
            Texture2D txtr = Content.Load<Texture2D>(house[i].getSuit() + "-" + house[i].getRank());
            spriteBatch.Draw(txtr, new Rectangle(counter, 10, width, 150), Color.White);
            counter += 151;
        }
        spriteBatch.DrawString(spriteFont, "House:" + houseCardsValue, new Vector2(100, 200), Color.White);
        int cunter = 100;
        for (int i = 0; i < player.Count; i++)
        {
            Texture2D txtr = Content.Load<Texture2D>(player[i].getSuit() + "-" + player[i].getRank());
            spriteBatch.Draw(txtr, new Rectangle(cunter, 410, width, 150), Color.White);
            cunter += 151;
        }
        spriteBatch.DrawString(spriteFont, "Player:" + playerCardsValue, new Vector2(100, 560), Color.White);
        spriteBatch.DrawString(spriteFont, this.winnerText, new Vector2(340, 210), Color.White);
        spriteBatch.DrawString(spriteFont, this.Window.Title, new Vector2(340, 240), Color.White);
        spriteBatch.DrawString(spriteFont, "B l a c k  J a c k", new Vector2(440, 280), Color.White);
        spriteBatch.DrawString(spriteFont2, "Press [B]", new Vector2(5, 10), Color.YellowGreen);
        spriteBatch.DrawString(spriteFont2, "To Raise", new Vector2(5, 50), Color.YellowGreen);
        spriteBatch.DrawString(spriteFont2, "Bet", new Vector2(5, 90), Color.YellowGreen);
        spriteBatch.DrawString(spriteFont2, "Press [U]", new Vector2(5, 170), Color.YellowGreen);
        spriteBatch.DrawString(spriteFont2, "To Lower", new Vector2(5, 210), Color.YellowGreen);
        spriteBatch.DrawString(spriteFont2, "Bet", new Vector2(5, 250), Color.YellowGreen);
        spriteBatch.Draw(shuffle.GetTexture(), sizeShuffle, Color.Wheat);
        spriteBatch.Draw(s.GetTexture(), sizeS, Color.White);
        spriteBatch.End();

        base.Draw(gameTime);
    }
}

public class Card
{
    private int suit = 1;
        
    private int rank = 1;

    public static int DIAMOND = 1;

    public static int HEART = 2;
        
    public static int CLUB = 3;
        
    public static int SPADE = 4;

    public Card(int suit, int rank)
    {
        this.suit = suit;

        this.rank = rank;
    }

    public void setSuit(int suit)
    {
        this.suit = suit;
    }

    public void setRank(int rank)
    {
        this.rank = rank;
    }

    public int getSuit()
    {
        return this.suit;
    }

    public int getRank()
    {
        return this.rank;
    }
}

public class DeckOfCards
{
    public static int NCARDS = 52;
    
    private Card[] deckOfCards;         // Contains all 52 cards
    
    /* ---------------------------------------------------
        The constructor method: make 52 cards in a deck
	--------------------------------------------------- */
    public DeckOfCards( )
    {
        /* =================================================================   
        First: create the array                                             
        ================================================================= */
           	deckOfCards = new Card[ NCARDS ];   // Very important !!!              
                                            // We must crate the array first ! 

        /* =================================================================
        Next: initialize all 52 card objects in the newly created array
        ================================================================= */
           	int i = 0;
    
           	for ( int suit = Card.DIAMOND; suit <= Card.SPADE; suit++ )           
           	for ( int rank = 1; rank <= 13; rank++ )
           	deckOfCards[i++] = new Card(suit, rank);  // Put card in
                                                        // position i
    }

    public void setCards(Card[] cards)
    {
        this.deckOfCards = cards;
    }

    public Card[] getCards()
    {
        return this.deckOfCards;
    }

}

/// <summary>
/// Fisher-Yates algorithm used for the card shuffle mechanism
/// </summary>
static public class FisherYates
{
    static Random r = new Random();

    static public Card[] Shuffle(Card[] deck)
    {
        for (int n = deck.Length - 1; n > 0; --n)
        {
            int k = r.Next(n + 1);
    
            int tempsuit = deck[n].getSuit();
            
            int temp = deck[n].getRank();
            
            deck[n].setRank(deck[k].getRank());
            
            deck[k].setRank(temp);
            
            deck[n].setSuit(deck[k].getSuit());
            
            deck[k].setSuit(tempsuit);
        }

        return deck;
    }
}

static class SpriteBatchEx
{
    /// <summary>
    /// Draws a single line. 
    /// Require SpriteBatch.Begin() and SpriteBatch.End()
    /// </summary>
    public static void DrawLine(this SpriteBatch spriteBatch, Vector2 begin, Vector2 end, Color color, int width = 1)
    {
        Rectangle r = new Rectangle((int)begin.X, (int)begin.Y, (int)(end - begin).Length() + width, width);
 
        Vector2 v = Vector2.Normalize(begin - end);
        
        float angle = (float)Math.Acos(Vector2.Dot(v, -Vector2.UnitX));
        
        if (begin.Y > end.Y) angle = MathHelper.TwoPi - angle;
        
        spriteBatch.Draw(TexGen.White, r, null, color, angle, Vector2.Zero, SpriteEffects.None, 0);
    }

    /// <summary>
    /// The graphics device, set this before drawing lines
    /// </summary>
    public static GraphicsDevice GraphicsDevice;

    static class TexGen
    {
        static Texture2D white = null;
        /// <summary>
        /// Returns a single pixel white texture, if it doesn't exist, it creates one
        /// </summary>
        /// <exception cref="System.Exception">Please set the SpriteBatchEx.GraphicsDevice to your graphicsdevice before drawing lines.</exception>
        public static Texture2D White
        {
            get
            {
                if (white == null)
                {
                    if (SpriteBatchEx.GraphicsDevice == null)
                        throw new Exception("Please set the SpriteBatchEx.GraphicsDevice to your GraphicsDevice before drawing lines.");
        
                    white = new Texture2D(SpriteBatchEx.GraphicsDevice, 1, 1);
                    
                    Color[] color = new Color[1];
                    
                    color[0] = Color.White;
                    
                    white.SetData<Color>(color);
                }

                return white;
            }
        }
    }
}