using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;



namespace Tomogram_visualizer
{

  public partial class Form1 : Form
  {
    //создаем и инициализируем объект
    Bin bin;
    
    // чтобы не запускать отрисовку пока не загружены данные
    bool loaded = false;

    public Form1()
    {
      InitializeComponent();
    }

        private void открытьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();

            if(dialog.ShowDialog() == DialogResult.OK)
            {
                string str = dialog.FileName;

                bin.readBIN(str);

                View.SetupView(glControl1.Width, glControl1.Height);

                loaded = true;

                glControl1.Invalidate();


            }
        }

        private void glControl1_Paint(object sender, PaintEventArgs e)
        {
            if (loaded)
            {
                View.DrawQuads(0);
                glControl1.SwapBuffers();
            }
        }
    }
}
