using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace GameOfNougtsAndCrossesLib
{
    public enum CellStatus { None, Cross, Nougth }
    public class Cell
    {
        public CellStatus Status { get; set; }
        public string UId { get; set; }

        public Cell(CellStatus status, string uid)
        { 
            Status = status;
            UId = uid;
        }

        public Cell(Stream stream)
        {
            Cell cell = Load(stream).Result;
            Status = cell.Status;
            UId = cell.UId;
        }

        private static async Task<Cell> Load(Stream stream)
        {
            //Получаем CellStatus
            byte[] buffer = new byte[4];
            await stream.ReadAsync(buffer, 0, buffer.Length);
            CellStatus cellStatus = (CellStatus)BitConverter.ToInt32(buffer, 0);

            //Получаем размер Uid
            buffer = new byte[4];
            await stream.ReadAsync(buffer, 0, buffer.Length);

            //Получаем строку uid
            buffer = new byte[BitConverter.ToInt32(buffer, 0)];
            await stream.ReadAsync(buffer, 0, buffer.Length);
            string uId = Encoding.Unicode.GetString(buffer);
            return new Cell(cellStatus, uId);
        }

        public byte[] Send()
        {
            MemoryStream stream = new MemoryStream();
            BinaryWriter writer = new BinaryWriter(stream);
            //Отсылаем CellStatus
            writer.Write(BitConverter.GetBytes((int)Status));

            //Отсылаем Uid
            byte[] buf = Encoding.Unicode.GetBytes(UId);
            writer.Write(buf.Length);
            writer.Write(buf);
            return stream.ToArray();
        }

        public void Draw(Button button)
        {
            Image img = new Image();
            img.Height = 25;
            img.Width = 25;

            if (Status == CellStatus.Cross)
            {
                img.Source = new BitmapImage(new Uri(@"..\..\..\Images\Cross.png"));
                button.IsEnabled = false;
            }
            else if (Status == CellStatus.Nougth)
            {
                img.Source = new BitmapImage(new Uri(@"..\..\..\Images\Nougth.png"));
                button.IsEnabled = false;
            }
            else
                return;

            StackPanel stackPnl = new StackPanel();
            stackPnl.Orientation = Orientation.Horizontal;
            stackPnl.Margin = new Thickness(10);
            stackPnl.Children.Add(img);
            button.Content = stackPnl;
        }

    }
}