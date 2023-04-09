using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenTK.Graphics.OpenGL;



namespace TomogramVisualizer
{
  public static class Bin
  {
    public static int X, Y, Z;

    public static short[] array;

    

    public static void readBIN(string path)
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

        for (int i = 0; i < arraySize; i++)
        {
          array[i] = reader.ReadInt16();
        }

        int k = array[300];
      }
    }
    //public static int X, Y, Z;
    //public static short[]? array;
    //
    //public static void readBin(string path)
    //{
    //    if (File.Exists(path))
    //    {
    //        BinaryReader reader = new BinaryReader(File.Open(path, FileMode.Open));
    //        X = reader.ReadInt32();
    //        Y = reader.ReadInt32();
    //        Z = reader.ReadInt32();
    //        int arraySize = X * Y * Z;
    //        array = new short[arraySize];
    //        for (int i = 0; i < arraySize; i++)
    //        {
    //            array[i] = reader.ReadInt16();
    //        }
    //    }
    //}
  }
  public static class View
  {
    static int VBOtexture; //хранит номер текстуры в памяти видеокарты
    static Bitmap textureImage;

    public static int min = 0;
    public static int width = 255;

        public static void SetupView(int width, int height)
        {
            GL.ShadeModel(ShadingModel.Smooth);
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();
            GL.Ortho(0, Bin.X, 0, Bin.Y, -1, 1);
            GL.Viewport(0, 0, width, height);
        }
    public static Color TransferFunction(short value)
    {

      
      int max = min + width;
      
      int newVal = Math.Clamp((value - min) * 255 / (max - min), 0, 255);

      return Color.FromArgb(newVal, newVal, newVal);
    }
    public static void DrawQuads(int layerNumber)
    {
      GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
      GL.Begin(BeginMode.Quads);
      for (int x_coord = 0; x_coord < Bin.X - 1; x_coord++)
        for (int y_coord = 0; y_coord < Bin.Y - 1; y_coord++)
        {
          short value;
          value = Bin.array[x_coord + y_coord * Bin.X + layerNumber * Bin.X * Bin.Y];
          GL.Color3(TransferFunction(value));
          GL.Vertex2(x_coord, y_coord);
          value = Bin.array[x_coord + (y_coord + 1) * Bin.X + layerNumber * Bin.X * Bin.Y];
          GL.Color3(TransferFunction(value));
          GL.Vertex2(x_coord, y_coord + 1);
          value = Bin.array[(x_coord + 1) + (y_coord + 1) * Bin.X + layerNumber * Bin.X * Bin.Y];
          GL.Color3(TransferFunction(value));
          GL.Vertex2(x_coord + 1, y_coord + 1);
          value = Bin.array[(x_coord + 1) + y_coord * Bin.X + layerNumber * Bin.X * Bin.Y];
          GL.Color3(TransferFunction(value));
          GL.Vertex2(x_coord + 1, y_coord);
        }
      GL.End();
    }

    public static void Load2DTexture()
    {
      // связывает текстуру, делает ее активной, указывает ее тип
      GL.BindTexture(TextureTarget.Texture2D, VBOtexture);

      BitmapData data = textureImage.LockBits(new System.Drawing.Rectangle(0, 0, textureImage.Width, textureImage.Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

      // загружает текстуру в память видеокарты
      GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, data.Width, data.Height, 0, OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);

      textureImage.UnlockBits(data);

      GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
      
      GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
      
      ErrorCode Er = GL.GetError();
      
      string str = Er.ToString();
    }

    // генерирует изображение из томограммы при помощи созданной Transfer Function
    public static void generateTextureImage(int layerNumber)
    {
      textureImage = new Bitmap(Bin.X, Bin.Y);
      for(int i = 0; i < Bin.X; ++i)
      {
        for(int j = 0; j < Bin.Y; ++j)
        {
          int pixelNumber = i + j * Bin.X + layerNumber * Bin.X * Bin.Y;
          textureImage.SetPixel(i, j, TransferFunction(Bin.array[pixelNumber]));
        }
      }
    }
    // включает 2d - текстурирование,выбирает текстуру и рисует один прямоугольник с наложенной текстурой, выключает 2d - текстурирование 
    public static void DrawTexture()
    {
      GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
      GL.Enable(EnableCap.Texture2D);
      GL.BindTexture(TextureTarget.Texture2D, VBOtexture);

      GL.Begin(BeginMode.Quads);

      GL.Color3(Color.White);
      GL.TexCoord2(0f,0f);
      GL.Vertex2(0,0);
      GL.TexCoord2(0f, 1f);
      GL.Vertex2(0, Bin.Y);
      GL.TexCoord2(1f, 1f);
      GL.Vertex2(Bin.X,Bin.Y);
      GL.TexCoord2(1f, 0f);
      GL.Vertex2(Bin.X, 0);

      GL.End();

      GL.Disable(EnableCap.Texture2D);
    }
    
    
  }
}
