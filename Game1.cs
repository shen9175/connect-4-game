using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace connect4
{

    public struct position
    {
       public Point pos;
       public int color;
       public bool occupied;
    };
     public struct stepmove
     {
         public Point pos;
         public double score;
     }
     struct animation
     {
         public bool animationfinish;
         public Point pos;
     }

    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Texture2D ball,redball;
        Texture2D blank;
        SpriteFont font;
        MouseState oldmousestate, currentmousestate;
        position[,] board;
        Point move;
        Point start, end;
        animation ani;
        int Maxlevel;
        int column, row,player,delay_timer,delay_time,drop,accerlate_time;
        bool illegalmove,win,draw;
        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            graphics.PreferredBackBufferWidth = 1024;
            graphics.PreferredBackBufferHeight = 768;
            graphics.ApplyChanges();

        }


        protected override void Initialize()
        {
            start.X = -1; start.Y = -1;
            end.X = -1; end.Y = -1;
            Maxlevel = 7;
            board = new position[7, 6];
            column = Window.ClientBounds.Width / 8;
            row = Window.ClientBounds.Height / 7;
            illegalmove = false;
            win = false;
            draw = false;
            ani.animationfinish = true ;
            ani.pos.X = -1; ani.pos.Y = -1;
            player = 1;
            delay_timer = 0;
            delay_time = 25;
            drop = 0;
            accerlate_time = 0;
            for (int i = 0; i < 7; i++)
            {
                for (int j = 0; j < 6; j++)
                {
                    board[i, j].pos.X = i;
                    board[i, j].pos.Y = j;
                    board[i, j].occupied = false;
                }
            }

            base.Initialize();
        }


        protected override void LoadContent()
        {

            spriteBatch = new SpriteBatch(GraphicsDevice);
            ball = Content.Load<Texture2D>("ball") as Texture2D;
            redball = Content.Load<Texture2D>("goodball") as Texture2D;
            //bar = Content.Load<Texture2D>("bar") as Texture2D;
            blank = new Texture2D(GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
            blank.SetData(new[] { Color.White });
            font = Content.Load<SpriteFont>("SpriteFont1");
            IsMouseVisible = true;

        }


        protected override void UnloadContent()
        {

        }


        protected override void Update(GameTime gameTime)
        {

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();


            oldmousestate = currentmousestate;
            currentmousestate = Mouse.GetState();

            if (illegalmove)
            {
                if (oldmousestate.LeftButton == ButtonState.Pressed && currentmousestate.LeftButton == ButtonState.Released)
                    illegalmove = false;
            }
            else if (!win&&!draw&&ani.animationfinish)
            {
                if (player != 1)
                {
                    updateboard(hminimax(Maxlevel));
                }
                else if (oldmousestate.LeftButton == ButtonState.Pressed && currentmousestate.LeftButton == ButtonState.Released)
                {
                    Point click = new Point(currentmousestate.X, currentmousestate.Y);
                    if (click.Y >= 0 && click.Y <= Window.ClientBounds.Height)
                    {
                        if (click.X >= 1 * column - 50 && click.X <= 1 * column + 50)
                        {
                            updateboard(0);
                        }
                        else if (click.X >= 2 * column - 50 && click.X <= 2 * column + 50)
                        {
                            updateboard(1);
                        }
                        else if (click.X >= 3 * column - 50 && click.X <= 3 * column + 50)
                        {
                            updateboard(2);
                        }
                        else if (click.X >= 4 * column - 50 && click.X <= 4 * column + 50)
                        {
                            updateboard(3);
                        }
                        else if (click.X >= 5 * column - 50 && click.X <= 5 * column + 50)
                        {
                            updateboard(4);
                        }
                        else if (click.X >= 6 * column - 50 && click.X <= 6 * column + 50)
                        {
                            updateboard(5);
                        }
                        else if (click.X >= 7 * column - 50 && click.X <= 7 * column + 50)
                        {
                            updateboard(6);
                        }
                    }
                }
            }
            base.Update(gameTime);
        }


        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.SandyBrown);


            spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend);
            

            if (ani.animationfinish == false)
            {
                Color c;
                if(player==1)
                    c=Color.Blue;
                else
                    c=Color.Red;
               delay_timer += gameTime.ElapsedGameTime.Milliseconds;
                if(delay_timer>delay_time)
                {
                delay_timer-=delay_time;

                    if (drop <= (move.Y + 1) * row - 50)
                    {
                        drop += accerlate_time * 10;
                        spriteBatch.Draw(redball, new Rectangle((move.X + 1) * column - 50, drop, 100, 100), c);
                        accerlate_time++;
                    }
                    else
                    {
                        spriteBatch.Draw(redball, new Rectangle((move.X + 1) * column - 50, (move.Y + 1) * row - 50, 100, 100), c);
                        drop = 0;
                        accerlate_time = 0;
                        ani.animationfinish = true;
                        if (!win)
                        {
                            if (player == 1)
                                player = 2;
                            else
                                player = 1;
                        }
                    }
                
                }

            }
            for (int i = 0; i < 7; i++)
            {
                for (int j = 0; j < 6; j++)
                {
                    if (board[i, j].occupied == true)
                    {
                        Color c;
                        if (board[i, j].color == 1)
                            c = Color.Blue;
                        else
                            c = Color.Red;
                        if (ani.animationfinish == false)
                        {
                            if (ani.pos.X == i && ani.pos.Y == j)
                                spriteBatch.Draw(ball, new Rectangle((i + 1) * column - 10, (j + 1) * row - 10, 20, 20), Color.DarkOrange);
                            else
                                spriteBatch.Draw(redball, new Rectangle((i + 1) * column - 50, (j + 1) * row - 50, 100, 100), c);

                        }
                        else
                            spriteBatch.Draw(redball, new Rectangle((i + 1) * column - 50, (j + 1) * row - 50, 100, 100), c);
                    }
                    else
                        spriteBatch.Draw(ball, new Rectangle((i + 1) * column - 10, (j + 1) * row - 10, 20, 20), Color.DarkOrange);
                }
            }
            if (!win && !draw&&!illegalmove)
            {
                if (player == 1)
                    spriteBatch.DrawString(font, "Blue move:", new Vector2(300, 720), Color.Red);
                else
                    spriteBatch.DrawString(font, "Red move:", new Vector2(300, 720), Color.Red);
            }
            if (win)
            {
                if (ani.animationfinish == true)
                {
                        DrawLine(spriteBatch, blank, 5, Color.HotPink, new Vector2((start.X + 1) * column, (start.Y + 1) * row), new Vector2((end.X + 1) * column, (end.Y + 1) * row));
                }
                if(player==1)
                    spriteBatch.DrawString(font, "Blue Win!", new Vector2(300, 720), Color.Red);
                else
                    spriteBatch.DrawString(font, "Red Win!", new Vector2(300, 720), Color.Red);
            }
            if(draw)
                spriteBatch.DrawString(font, "Draw Game!", new Vector2(300, 720), Color.Red);
            if (illegalmove)
            {
                spriteBatch.DrawString(font, "Illegal Move!", new Vector2(300, 720), Color.Red);
            }

            //DrawLine(spriteBatch, blank, 1,Color.HotPink, new Vector2(0, 720), new Vector2(1024, 720));
            spriteBatch.End();
            base.Draw(gameTime);
        }

        public void updateboard(int col)
        {
            if (isDraw(board))
                draw = true;
            else
            {


                if (board[col, 0].occupied == true)
                    illegalmove = true;
                else
                {

                    int i;
                    for (i = 5; i >= 0; i--)
                    {
                        if (board[col, i].occupied == false)
                            break;
                    }
                    board[col, i].occupied = true;
                    board[col, i].color = player;
                    ani.animationfinish = false;
                    ani.pos.X = col;
                    ani.pos.Y = i;
                    move.X = col;
                    move.Y = i;
                   // if (isWin(col, i, board))
                    if(winstate(board,player))
                        win = true;

                }
            }
        }
/*
        public bool isWin(int col, int row, position[,] state)
        {
            int connect = 1;
            //check right:
            for (int i = 1; i <= 3; i++)
            {
                if(col+i>6)
                    break;
                else
                {
                    if (state[col + i, row].occupied == false)
                        break;
                    else
                    {
                        if (state[col + i, row].color == player)
                            connect++;
                        else
                            break;
                    }

                }
            }
            if (connect == 4)
                return true;

            //check left:
            for (int i = 1; i <= 3; i++)
            {
                if (col - i < 0)
                    break;
                else
                {
                    if (state[col - i, row].occupied == true)
                    {
                        if (state[col - i, row].color == player)
                            connect++;
                        else
                            break;
                    }
                    else
                        break;
                }
            }
            if (connect == 4)
                return true;

            connect = 1;
            //check up:
            for (int i = 1; i <= 3; i++)
            {
                if (row - i < 0)
                    break;
                else
                {
                    if (state[col, row - i].occupied == true)
                    {
                        if (state[col, row - i].color == player)
                            connect++;
                        else
                            break;
                    }
                    else
                        break;
                }
            }
            if (connect == 4)
                return true;
           //check down:
            for (int i = 1; i <= 3; i++)
            {
                if (row + i > 5)
                    break;
                else
                {
                    if (state[col, row + i].occupied == true)
                    {
                        if (state[col, row + i].color == player)
                            connect++;
                        else
                            break;
                    }
                    else
                        break;
                }
            }
            if (connect == 4)
                return true;

            connect = 1;
            //check upright:
            for (int i = 1; i <= 3; i++)
            {
                if (col + i > 6 || row - i < 0)
                    break;
                else
                {
                    if (state[col + i, row - i].occupied == true)
                    {
                        if (state[col + i, row - i].color == player)
                            connect++;
                        else
                            break;
                    }
                    else
                        break;
                }
            }
            if (connect == 4)
                return true;
            //check leftbottom:
            for (int i = 1; i <= 3; i++)
            {
                if (col - i < 0 || row + i > 5)
                    break;
                else
                {
                    if (state[col - i, row + i].occupied == true)
                    {
                        if (state[col - i, row + i].color == player)
                            connect++;
                        else
                            break;
                    }
                    else
                        break;
                }

            }
            if (connect == 4)
                return true;
            connect = 1;

            //check upleft:
            for (int i = 1; i <= 3; i++)
            {
                if (col - i < 0 || row - i < 0)
                    break;
                else
                {
                    if (state[col - i, row - i].occupied == true)
                    {
                        if (state[col - i, row - i].color == player)
                            connect++;
                        else
                            break;
                    }
                    else
                        break;
                }
            }
            if (connect == 4)
                return true;
            //check bottomright:
            for (int i = 1; i <= 3; i++)
            {
                if (col + i > 6 || row + i > 5)
                    break;
                else
                {
                    if (state[col + i, row + i].occupied == true)
                    {
                        if (state[col + i, row + i].color == player)
                            connect++;
                        else
                            break;
                    }
                    else
                        break;
                }
            }
            if (connect == 4)
                return true;

            return false;
        }*/
        public bool isDraw(position[,] state)
        {
            int sum = 0;
            for (int i = 0; i < 7; i++)
            {
                if (state[i, 0].occupied == true)
                    sum++;
            }
            if (sum == 7)
                return true;
            else
                return false;
        }
        public void DrawLine(SpriteBatch batch, Texture2D blank,
              float width, Color color, Vector2 point1, Vector2 point2)
        {
            float angle = (float)Math.Atan2(point2.Y - point1.Y, point2.X - point1.X);
            float length = Vector2.Distance(point1, point2);

            batch.Draw(blank, point1, null, color,
                       angle, Vector2.Zero, new Vector2(length, width),
                       SpriteEffects.None, 0);
        }

        public int hminimax(int depth)
        {
            
            position[,] state = new position[7, 6];
            for (int i = 0; i < 7; i++)
            {
                for (int j = 0; j < 6; j++)
                {
                    state[i,j].pos.X=board[i, j].pos.X;
                    state[i, j].pos.Y=board[i, j].pos.Y;
                    state[i, j].color = board[i, j].color;
                    state[i, j].occupied=board[i, j].occupied;
                }
            }
            //state[,] is a thinking board for minimax algorithm
            //v contains each action return move(position and score), r is for the final return value
            //1 is for human
           // double v = MaxValue(state, double.MinValue, double.MaxValue,depth);
            stepmove v,r;
            r.score = double.MaxValue;
            r.pos.X = -1;
            r.pos.Y = -1;
            List<stepmove> actionlist=new List<stepmove>();
            Action(state, ref actionlist);

            foreach (stepmove a in actionlist)
            {
                v = MaxValue(state, a,double.MinValue, double.MaxValue, 1);
                if (v.score <= r.score)
                    r = v;
            }

            return r.pos.X;
        }
        public stepmove MaxValue(position[,] s, stepmove next,double alpha, double beta,int depth)
        {

            position[,] state1 = new position[7, 6];
            for (int i = 0; i < 7; i++)
            {
                for (int j = 0; j < 6; j++)
                {
                    state1[i, j].pos.X = s[i, j].pos.X;
                    state1[i, j].pos.Y = s[i, j].pos.Y;
                    state1[i, j].color = s[i, j].color;
                    state1[i, j].occupied = s[i, j].occupied;
                }
            }

            result(state1,next,2);
            stepmove v;
            v.score= double.MinValue;
            v.pos = next.pos;
            if (cutoff(state1, depth, 2))
            {
                v.score = evaluation(state1, 2);
                return v;
            }
            List<stepmove> actionlist = new List<stepmove>();
            Action(state1, ref actionlist);
            foreach(stepmove a in actionlist)
            {
                v.score=Math.Max(v.score,MinValue(state1,a,alpha,beta,depth+1).score);
                if(v.score>=beta)
                    return v;
                alpha=Math.Max(alpha,v.score);
            }
            return v;
        }
        public stepmove MinValue(position[,] s, stepmove next,double alpha, double beta, int depth)
        {
            position[,] state2 = new position[7, 6];
            for (int i = 0; i < 7; i++)
            {
                for (int j = 0; j < 6; j++)
                {
                    state2[i, j].pos.X = s[i, j].pos.X;
                    state2[i, j].pos.Y = s[i, j].pos.Y;
                    state2[i, j].color = s[i, j].color;
                    state2[i, j].occupied = s[i, j].occupied;
                }
            }
            result(state2, next, 1);
            stepmove v;
            v.score = double.MaxValue;
            v.pos = next.pos;
            if (cutoff(state2, depth, 1))
            {
                v.score = evaluation(state2, 1);
                return v;
            }
            List<stepmove> actionlist = new List<stepmove>();
            Action(state2, ref actionlist);
            foreach (stepmove a in actionlist)
            {
                v.score = Math.Min(v.score, MaxValue(state2, a, alpha, beta, depth + 1).score);
                if (v.score <= alpha)
                    return v;
                beta = Math.Min(beta, v.score);
            }
            return v;
        }

        public void Action(position[,] state, ref List<stepmove> r)
        {
           // List<stepmove> r = new List<stepmove>();
            
            for (int i = 0; i < 7; i++)
            {
                for (int j = 5; j >= 0; j--)
                {
                    if (!state[i, j].occupied)
                    {
                        stepmove temp = new stepmove();
                        temp.pos = state[i, j].pos;
                        temp.score = double.MinValue;
                        r.Add(temp);
                        break;
                    }
                }
            }
           // return r;

        }
        public void result(position[,] state, stepmove a,int pcolor)
        {
            
            state[a.pos.X, a.pos.Y].pos = a.pos;
            state[a.pos.X, a.pos.Y].occupied = true;
            state[a.pos.X, a.pos.Y].color = pcolor;
            //return state;
        }
        public double evaluation(position[,] state, int pcolor)
        {
            if (winstate(state, pcolor))
            {
                if (pcolor == 2)//computer
                    return double.MinValue;
                else
                    return double.MaxValue;
            }
            return score(state, 2) - score(state, 1);
        }
        public double score(position[,] state,int pcolor)
        {
            double score = 0;
            double scoreTwo = 10;
            double scoreThree = 40;

            for (int i = 0; i < 7; i++)
            {
                for (int j = 0; j < 6; j++)
                {
                    if (state[i, j].occupied == true)
                    {
                        //Vertical 2
                        if (j < 6 - 2)
                        {
                            if ((state[i, j + 1].color == pcolor) && (state[i, j + 2].color == pcolor))
                                score += scoreTwo;
                        }

                        //Vertical 2~
                        if (j >= 2)
                        {
                            if ((state[i, j - 1].color == pcolor) && (state[i, j - 2].color == pcolor))
                                score += scoreTwo;
                        }

                        //Horizontal 2
                        if (i < 7 - 2)
                        {
                            if ((state[i + 1, j].color == pcolor) && (state[i + 2, j].color == pcolor))
                                score += scoreTwo;

                            //Diagonal 2
                            if (j < 6 - 2)
                            {
                                if ((state[i + 1, j + 1].color == pcolor) && (state[i + 2, j + 2].color == pcolor))
                                    score += scoreTwo;
                            }

                            if (j >= 2)
                            {
                                if ((state[i + 1, j - 1].color == pcolor) && (state[i + 2, j - 2].color == pcolor))
                                    score += scoreTwo;
                            }
                        }

                        //Horizontal 2~
                        if (i >= 2)
                        {
                            if ((state[i - 1, j].color == pcolor) && (state[i - 2, j].color == pcolor))
                                score += scoreTwo;

                            //Diagonal 2~
                            if (j < 6 - 2)
                            {
                                if ((state[i - 1, j + 1].color == pcolor) && (state[i - 2, j + 2].color == pcolor))
                                    score += scoreTwo;
                            }

                            if (j >= 2)
                            {
                                if ((state[i - 1, j - 1].color == pcolor) && (state[i - 2, j - 2].color == pcolor))
                                    score += scoreTwo;
                            }
                        }

                        //Vertical 3
                        if (j < 6 - 3)
                        {
                            if ((state[i, j + 1].color == pcolor) && (state[i, j + 2].color == pcolor) && (state[i, j + 3].color == pcolor))
                                score += scoreThree;
                        }

                        //Vertical 3~
                        if (j >= 3)
                        {
                            if ((state[i, j - 1].color == pcolor) && (state[i, j - 2].color == pcolor) && (state[i, j - 3].color == pcolor))
                                score += scoreThree;
                        }

                        //Horizontal 3
                        if (i < 7 - 3)
                        {
                            if ((state[i + 1, j].color == pcolor) && (state[i + 2, j].color == pcolor) && (state[i + 3, j].color == pcolor))
                                score += scoreThree;

                            //Diagonal 3
                            if (j < 6 - 3)
                            {
                                if ((state[i + 1, j + 1].color == pcolor) && (state[i + 2, j + 2].color == pcolor) && (state[i + 3, j + 3].color == pcolor))
                                    score += scoreThree;
                            }

                            if (j >= 3)
                            {
                                if ((state[i + 1, j - 1].color == pcolor) && (state[i + 2, j - 2].color == pcolor) && (state[i + 3, j - 3].color == pcolor))
                                    score += scoreThree;
                            }
                        }

                        //Horizontal 3~
                        if (i >= 3)
                        {
                            if ((state[i - 1, j].color == pcolor) && (state[i - 2, j].color == pcolor) && (state[i - 3, j].color == pcolor))
                                score += scoreThree;

                            //Diagonal 3~
                            if (j < 6 - 3)
                            {
                                if ((state[i - 1, j + 1].color == pcolor) && (state[i - 2, j + 2].color == pcolor) && (state[i - 3, j + 3].color == pcolor))
                                    score += scoreThree;
                            }

                            if (j >= 3)
                            {
                                if ((state[i - 1, j - 1].color == pcolor) && (state[i - 2, j - 2].color == pcolor) && (state[i - 3, j - 3].color == pcolor))
                                    score += scoreThree;
                            }
                        }
                    }
                }
            }

            return score; 
        }

        public bool winstate(position[,] state, int pcolor)
        {
            //Horizontal
            for (int i = 0; i < 7; i++)
            {
                for (int j = 0; j < 6 - 3; j++)
                {
                    if (state[i,j].occupied&&(state[i, j].color == pcolor) && state[i,j+1].occupied&&(state[i, j+1].color == pcolor) && state[i,j+2].occupied&&(state[i, j+2].color == pcolor) && state[i,j+3].occupied&&(state[i, j+3].color == pcolor))
                    { start.X = i; start.Y = j; end.X = i; end.Y = j + 3; return true; }
                }
            }

            //Vertical
            for (int i = 0; i < 7 - 3; i++)
            {
                for (int j = 0; j < 6; j++)
                {
                    if (state[i,j].occupied&&(state[i, j].color == pcolor) && state[i+1,j].occupied&&(state[i+1, j].color == pcolor) && state[i+2,j].occupied&&(state[i+2, j].color == pcolor) && state[i+3,j].occupied&&(state[i+3, j].color == pcolor))
                    { start.X = i; start.Y = j; end.X = i+3; end.Y = j; return true; }
                }
            }

            //Diagonal
            for (int i = 0; i < 7 - 3; i++)
            {
                for (int j = 0; j < 6 - 3; j++)
                {
                    if (state[i, j].occupied && (state[i, j].color == pcolor) && state[i+1, j + 1].occupied && (state[i+1, j + 1].color == pcolor) && state[i+2, j + 2].occupied && (state[i+2, j + 2].color == pcolor) && state[i+3, j + 3].occupied && (state[i+3, j + 3].color == pcolor))
                    { start.X = i; start.Y = j; end.X = i+3; end.Y = j + 3; return true; }
                }
            }

            //Diagonal
            for (int i = 0; i < 7 - 3; i++)
            {
                for (int j = 3; j < 6; j++)
                {
                    if (state[i, j].occupied && (state[i, j].color == pcolor) && state[i + 1, j - 1].occupied && (state[i + 1, j - 1].color == pcolor) && state[i + 2, j - 2].occupied && (state[i + 2, j - 2].color == pcolor) && state[i + 3, j - 3].occupied && (state[i + 3, j - 3].color == pcolor))
                    { start.X = i; start.Y = j; end.X = i+3; end.Y = j - 3; return true; }
                }
            }

            return false;
        }
        public bool cutoff(position[,] state, int depth,int pcolor)
        {
            if(winstate(state,pcolor))
                return true;
            else if(depth>Maxlevel)
                return true;
            else
                return false;
        }
    }
}
