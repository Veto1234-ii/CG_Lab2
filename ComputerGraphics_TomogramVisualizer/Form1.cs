using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;



namespace TomogramVisualizer
{
  public partial class Form1 : Form
  {
    //������� � �������������� ������
    //Bin bin;

    // ����� �� ��������� ��������� ���� �� ��������� ������
    bool loaded = false;

    int currentLayer = 0;
    bool needReload = false;
    public Form1()
    {
      InitializeComponent();
    }

    private void �������ToolStripMenuItem_Click(object sender, EventArgs e)
    {

      OpenFileDialog dialog = new OpenFileDialog();

      if (dialog.ShowDialog() == DialogResult.OK)
      {
        string str = dialog.FileName;


        //bin.readBIN(str);
        Bin.readBIN(str);


        View.SetupView(glControl1.Width, glControl1.Height);

        loaded = true;

        glControl1.Invalidate();

        trackBar1.Maximum = Bin.Z - 1;
      }
    }

    private void glControl1_Paint(object sender, PaintEventArgs e)
    {
      if (loaded)
      {
        //View.DrawQuads(currentLayer);
        if (needReload)
        {
          View.generateTextureImage(currentLayer);
          View.Load2DTexture();
          needReload = false;
        }
        View.DrawTexture();
        glControl1.SwapBuffers();
      }
    }

    private void trackBar1_Scroll(object sender, EventArgs e)
    {
      currentLayer = trackBar1.Value;
      glControl1.Invalidate();
      needReload = true;
    }

    

  }
}