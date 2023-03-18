using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Threading.Tasks;

using OpenTK;
using OpenTK.Graphics;

using OpenTK.Graphics.OpenGL;
using System.IO;
using System.Drawing.Drawing2D;

namespace Tomogram_visualizer
{
  class Bin
  {
    public static int X, Y, Z;

    public static short[] array;

    public Bin() { }

    public void readBIN(string path)
    {

      if (File.Exists(path))
      {
        BinaryReader reader = new BinaryReader(File.Open(path, FileMode.Open));

        //size of tomogram
        X = reader.ReadInt32();
        Y = reader.ReadInt32();
        Z = reader.ReadInt32();

        //data array
        int arraySize = X * Y * Z;
        array = new short[arraySize];

        for(int i = 0; i < arraySize; i++)
        {
          array[i] = reader.ReadInt16();
        }

      }
    }
  }

  class View
  {
        //данная функция настраивает окно вывода
        //Smooth - интерполирование цветов
        //Инициализация матрицы
        //Ортогональное проецирование массива данных в окно вывода
        //Вывод в окно OpenTK

        public static int clamp(int value, int min, int max)
        {
            if (value < min)
                return min;
            if (value > max)
                return max;

            return value;
        }

        //настраивает окно вывода
        public static void SetupView(int width, int height)
        {
            //интерполирование цветов
            GL.ShadeModel(ShadingModel.Smooth);

            GL.MatrixMode(MatrixMode.Projection);

            //инициализация матрицы проекции, устанавливаем ее равной матрице тождественного преобразования
            GL.LoadIdentity();

            //задаем матрицу
            //ортогональное проецирование массива данных томограммы в окно вывода,
            //которое попутно преобразует размеры массива в
            //канонический видимый объем(CVV)

            GL.Ortho(0, Bin.X, 0, Bin.Y,- 1, 1);

            //разрешение синтезируемого изображения равно размеру окна OpenTK
            GL.Viewport(0, 0, width, height);

        }


    //TF - функция перевода значения плотностей томограммы в цвет
        static Color TransferFunction(short value)
        {

            int min = 0;
            int max = 2000;
            int newVal = clamp((value - min) * 255 / (max - min), 0, 255);
             return Color.FromArgb(255, newVal, newVal, newVal);

        }

        //отрисовка четырехугольника
        //Из томограммы извлекаются значения в 4 ячейках.
        //Эти значения заносятся в цвет вершин четырехугольника,
        //и данный четырехугольник визуализируется
        public static void DrawQuads(int layerNumber)
        {

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.Begin(BeginMode.Quads);


            for(int x_coord = 0; x_coord < Bin.X - 1; x_coord++)
            {

                for(int y_coord = 0; y_coord < Bin.Y - 1; y_coord++)
                {

                    short value;
                    //1 вершина
                    value = Bin.array[x_coord + y_coord * Bin.X + layerNumber * Bin.X * Bin.Y];
                    GL.Color3(TransferFunction(value));
                    GL.Vertex2(x_coord, y_coord);

                    //2 вершина
                    value = Bin.array[x_coord + (y_coord + 1) * Bin.X + layerNumber * Bin.X * Bin.Y];
                    GL.Color3(TransferFunction(value));
                    GL.Vertex2(x_coord, y_coord+1);

                    //3 вершина
                    value = Bin.array[x_coord + 1 + (y_coord + 1) * Bin.X + layerNumber * Bin.X * Bin.Y];
                    GL.Color3(TransferFunction(value));
                    GL.Vertex2(x_coord + 1, y_coord + 1);

                    //4 вершина
                    value = Bin.array[x_coord + 1 + y_coord * Bin.X + layerNumber * Bin.X * Bin.Y];
                    GL.Color3(TransferFunction(value));
                    GL.Vertex2(x_coord + 1, y_coord);
                }
            GL.End();
        }
    }
  }
}
