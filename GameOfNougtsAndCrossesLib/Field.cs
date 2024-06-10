using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace GameOfNougtsAndCrossesLib
{
    public class Field : IEnumerable<Cell>
    {
        public static int FieldHeigth { get; } = 25;
        public static int FieldWidth { get; } = 25;
        public static int WinStatus { get; } = 5;
        private Cell[,] Field_ { get; } = new Cell[FieldHeigth, FieldWidth];

        public Cell this[int row, int col] { get => Field_[row, col]; set { Field_[row, col] = value; } }

        public Field(Cell[,] field_)
        {
            Field_ = field_;
        }

        public Field() { }

        public void SynchronizeWithPlayer(Field playerField)
        {
            for (int i = 0; i < FieldHeigth; i++)
                for (int j = 0; j < FieldWidth; j++)
                    if ((playerField[i, j].Status == CellStatus.Cross || playerField[i, j].Status == CellStatus.Nougth)
                        && Field_[i, j].Status == CellStatus.None)
                        Field_[i, j].Status = playerField[i, j].Status;
        }

        public Field(Stream stream)
        {
            for (int i = 0; i < FieldHeigth; i++)
                for (int j = 0; j < FieldWidth; j++)
                    Field_[i, j] = new Cell(stream);
        }
        public byte[] Send()
        {
            MemoryStream stream = new MemoryStream();
            BinaryWriter writer = new BinaryWriter(stream);
            for (int i = 0; i < FieldHeigth; i++)
                for (int j = 0; j < FieldWidth; j++)
                    writer.Write(Field_[i, j].Send());
            return stream.ToArray();
        }

        public Cell FindCell(Button btn)
        {
            for (int i = 0; i < FieldHeigth; i++)
                for (int j = 0; j < FieldWidth; j++)
                    if (Field_[i, j].UId == btn.Uid)
                        return Field_[i, j];
            return null;
        }

        public void DrawField(ListView listView)
        {
            for (int i = 0; i < FieldHeigth; i++)
                for (int j = 0; j < FieldWidth; j++)
                    Field_[i, j].Draw((Button)listView.Items[i * FieldWidth + j]);
        }

        //Метод создает поле для сервера, синхронизируется с клиентом
        public static Field CreateField()
        {
            Field field = new Field();
            for (int row = 0; row < FieldWidth; row++)
                for (int col = 0; col < FieldWidth; col++)
                    field[row, col] = new Cell(CellStatus.None, $"({row};{col})");
            return field;
        }

        public IEnumerator<Cell> GetEnumerator() => (IEnumerator<Cell>)Field_.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => Field_.GetEnumerator();

        private GameStatus CheckDraw()
        {
            int count = 0;
            for (int row = 0; row < FieldHeigth; row++)
                for (int col = 0; col < FieldWidth; col++)
                    if (Field_[row, col].Status != CellStatus.None)
                        count++;
            if (count == FieldWidth * FieldHeigth)
                return GameStatus.Draw;
            return GameStatus.Start;
        }

        public GameStatus CheckGameStatus()
        {
            if (Check(CellStatus.Cross, GameStatus.VictoryOfCrosses) == GameStatus.VictoryOfCrosses)
                return GameStatus.VictoryOfCrosses;
            else if (Check(CellStatus.Nougth, GameStatus.VictoryOfNougth) == GameStatus.VictoryOfNougth)
                return GameStatus.VictoryOfNougth;
            else if (CheckDraw() == GameStatus.Draw)
                return GameStatus.Draw;
            return GameStatus.Start;
        }

        private GameStatus Check(CellStatus status, GameStatus win)
        {
            for (int row = 0; row < FieldHeigth; row++)
            {
                for (int col = 0; col < FieldWidth; col++)
                {
                    if (Field_[row, col].Status == status)
                    {
                        if (win == CheckHorisontalVictory(row, col, status, win) 
                            || win == CheckVerticalVictory(row, col, status, win)
                            || win == CheckRirthDiagonalVictory(row, col, status, win)
                            || win == CheckLeftDiagonalVictory(row, col, status, win))
                            return win;
                    }
                }
            }
            return GameStatus.Start;
        }

        public GameStatus CheckHorisontalVictory(int row, int col, CellStatus status, GameStatus win)
        {
            int count = 0;
            for (int j = col; j < FieldWidth; j++)
            {
                if (Field_[row, j].Status == status)
                    count++;
                else if (Field_[row, col].Status == CellStatus.None)
                    break;
            }

            if (count == WinStatus)
                return win;

            return GameStatus.Start;
        }
        public GameStatus CheckVerticalVictory(int row, int col, CellStatus status, GameStatus win)
            => CheckVictory(row, col, status, win, 0);

        public GameStatus CheckRirthDiagonalVictory(int row, int col, CellStatus status, GameStatus win)
            => CheckVictory(row, col, status, win, 1);

        public GameStatus CheckLeftDiagonalVictory(int row, int col, CellStatus status, GameStatus win)
            => CheckVictory(row, col, status, win, -1);

        private GameStatus CheckVictory(int row, int col, CellStatus status, GameStatus win, int newDegree)
        {
            int count = 0;
            int requiredСell = col;
            for (int i = row; i < FieldHeigth; i++)
            {
                if (!(requiredСell >= 0 && requiredСell < FieldWidth))
                    break;

                if (Field_[i, requiredСell].Status == status)
                    count++;
                else if (Field_[row, col].Status == CellStatus.None)
                    break;

                requiredСell += newDegree;
            }

            if (count == WinStatus)
                return win;

            return GameStatus.Start;
        }
    }
}
