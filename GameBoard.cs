using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Gemstones.Board
{
    public class GameBoard
    {
        public static readonly int size = 8;
        public static readonly int cellEdge = 70;
        public static readonly Rectangle playField = new Rectangle(0, 0, cellEdge * size, cellEdge * size);
        public static readonly Point TranslationParameter = new Point(100, 100);

        public Gemstones.Scene.InGame inGame { get; private set; }
        public Gem currentlySelectedGem;
        private Random random;
        public List<List<Gem>> board { get; private set; }
        //public Point selected;

        public GameBoard(Gemstones.Scene.InGame inGame) {
            this.inGame = inGame;
            random = new Random();
            InitializeBoard();
        }

        public void Draw(Graphics g) {
            g.TranslateTransform(TranslationParameter.X, TranslationParameter.Y);
            //Draw board's border
            g.DrawRectangle(new Pen(Brushes.Gold, 5), playField);
            g.FillRectangle(new SolidBrush(Color.FromArgb(20, 20, 20)), playField);

            try
            {
                foreach (List<Gem> rowList in board)
                    foreach (Gem gem in rowList)
                        gem.Draw(g);
            }
            catch (InvalidOperationException) { }

            g.ResetTransform();
        }

        public bool CheckAndDoMatching(Gem src, Gem dst) {
            bool result;
            result = src.HasMatching();
            result |= dst.HasMatching();
            ResetZeroScaleToNothing();
            return result;
        }

        public Point GetTranslatedCursor() {
            Point cursor = GameMain.cursorLocation;
            cursor.X -= TranslationParameter.X;
            cursor.Y -= TranslationParameter.Y;
            return cursor;
        }

        public Gem GetCurrentGem() {
            foreach(List<Gem> list in board)
                foreach(Gem gem in list)
                    if (Helpers.IsWithin(gem.bounds, GetTranslatedCursor()))
                        return gem;
            return null;
        }

        public void InitializeBoard() {
            board = new List<List<Gem>>();

            for (int i = 0; i < size; i++)
            {
                List<Gem> rowList = new List<Gem>();
                for (int j = 0; j < size; j++)
                {
                    Rectangle bounds = new Rectangle(playField.X + Gem.size * j, playField.Y + Gem.size * i, Gem.size, Gem.size);
                    Point index = new Point(i, j);
                    GemName tempRand;
                    //Create a gem that does not create a triplet with adjacent gems
                    do
                    {
                        tempRand = (GemName)random.Next(6);
                        if (i < 2 && j < 2)
                            break;
                    } while ((j >= 2 && tempRand.Equals(rowList[j - 1].name) && tempRand.Equals(rowList[j - 2].name))
                         || (i >= 2 && tempRand.Equals(board[i - 1][j].name) && tempRand.Equals(board[i - 2][j].name)));
                    Gem gem = new Gem(this, tempRand, bounds, index);
                    rowList.Add(gem);
                }
                board.Add(rowList);
            }
        }

        public void ResetZeroScaleToNothing() { 
            foreach(List<Gem> list in board)
                foreach (Gem gem in list) {
                    if (gem.scaleParameter.X < 1f)
                        gem.ResetToNothing();
                }
        }
    }
}
