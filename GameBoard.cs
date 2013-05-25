using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;

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

            for (int i = 0; i < board.Count; i++)
                for (int j = 0; j < board.Count; j++)
                    board[i][j].Draw(g);

            g.ResetTransform();
        }

        private HashSet<Gem> GetGemsToBeDestroyed(){
            HashSet<Gem> gemsToBeDestroyed = new HashSet<Gem>();
            int rowCount = 1, colCount = 1;
            //Row checking
            for (int i = 0; i < board.Count; i++)
                for (int j = 1; j < board.Count; j++ ) {
                    if (!board[i][j].name.Equals(GemName.Nothing))
                        if (board[i][j].name.Equals(board[i][j - 1].name)) {
                            rowCount++;
                            if (j < board.Count - 1)
                                continue;
                            j++;
                        }
                    if (rowCount >= 3)
                        while (rowCount > 0) {
                            gemsToBeDestroyed.Add(board[i][j - rowCount]);
                            rowCount--;
                        }
                    rowCount = 1;
                }
            //Col checking
            for (int j = 0; j < board.Count; j++)
                for (int i = 1; i < board.Count; i++) {
                    if (!board[i][j].name.Equals(GemName.Nothing))
                        if (board[i][j].name.Equals(board[i - 1][j].name)) {
                            colCount++;
                            if (i < board.Count - 1)
                                continue;
                            i++;
                        }
                    if (colCount >= 3)
                        while (colCount > 0) {
                            gemsToBeDestroyed.Add(board[i - colCount][j]);
                            colCount--;
                        }
                    colCount = 1;
                }
            return gemsToBeDestroyed;
        }

        private void FillNothingWithNewGems() { 
            //Scan the board upwards, if there is an empty cell, move gems that are above that cell down
            for (int i = board.Count - 1; i >= 0; i--)
                for (int j = 0; j < board.Count; j++) {
                    if (!board[i][j].name.Equals(GemName.Nothing))
                        continue;

                    int tempRow = i;
                    int tempCount = 0;
                    while (tempRow >= 0 && board[tempRow--][j].name.Equals(GemName.Nothing))
                        tempCount++;

                    for (tempRow = i - tempCount; tempRow >= 0; tempRow--)
                    {
                        board[tempRow][j].SwitchPlaceWith(board[tempRow + tempCount][j]);
                    }
                }
            //create new random gems
            HashSet<Gem> nothingList = new HashSet<Gem>();
            for (int i = 0; i < board.Count; i++)
                for (int j = 0; j < board.Count; j++)
                    if (board[i][j].name == GemName.Nothing)
                        nothingList.Add(board[i][j]);
            SetNewGemsFromList(nothingList);
            //Check for matching again
            CheckAndDoMatching();
        }

        private void SetNewGemsFromList(HashSet<Gem> nothingList)
        {
            foreach (Gem gem in nothingList)
                gem.name = (GemName)random.Next(6);
            for (decimal scale = 0.1m; scale <= 1; scale += 0.1m)
            {
                foreach (Gem gem in nothingList)
                    gem.SetScaleParameter((float)scale);
                System.Threading.Thread.Sleep(GameMain.frameRate);
            }
        }

        public bool CheckAndDoMatching() {
            HashSet<Gem> gemsToBeDestroyed = GetGemsToBeDestroyed();
            if (gemsToBeDestroyed.Count == 0 || gemsToBeDestroyed == null)
                return false;

            for (decimal scale = 1.0m; scale > 0; scale -= 0.1m)
            {
                foreach (Gem gem in gemsToBeDestroyed)
                    gem.SetScaleParameter((float)scale);
                System.Threading.Thread.Sleep(GameMain.frameRate);
            }
            ResetZeroScaleToNothing();
            FillNothingWithNewGems();
            return true;
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

        private void ResetZeroScaleToNothing() { 
            foreach(List<Gem> list in board)
                foreach (Gem gem in list) {
                    if (gem.scaleParameter.X < 1f)
                        gem.ResetToNothing();
                }
        }
    }
}
