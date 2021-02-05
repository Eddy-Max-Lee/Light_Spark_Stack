using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Light_Spot_Center_Stack
{
    public partial class Form1 : Form
    {
         private static Dictionary<string, Bitmap> DICT_BMP= new Dictionary<string, Bitmap>();
        private static Bitmap STACKED;


        public Form1()
        {
            InitializeComponent();
           // textBox1.KeyDown += btn_stack_Click;
        }
        #region Tool
        //====Tool Start========================
        private Bitmap StackBitmap(Dictionary<string, Bitmap> dict) {

            var first = dict.First();
          //  Bitmap stacked =  first.Value;
            Bitmap stacked = new Bitmap(first.Value.Width, first.Value.Height);

           double[,] R_stacking = new double[first.Value.Width, first.Value.Height];
            double[,] G_stacking = new double[first.Value.Width, first.Value.Height];
            double[,] B_stacking = new double[first.Value.Width, first.Value.Height];




            //1. 相加
            foreach (KeyValuePair<string, Bitmap> pair in dict) {
                Bitmap ori_bmp = new Bitmap(pair.Value.Width, pair.Value.Height);
             
                ori_bmp = pair.Value;



                for (int Pixel_x = 0; Pixel_x < ori_bmp.Width; Pixel_x++)//一直是0->input_image.Width
                {
                    for (int Pixel_y = 0; Pixel_y < ori_bmp.Height; Pixel_y++)//初始值為now_y掃到now_y+20
                    {
                        Color color_new =ori_bmp.GetPixel(Pixel_x, Pixel_y);
                        int Red = color_new.R;
                        int Green = color_new.G;
                        int Blue = color_new.B;

                        //          R_stacking[Pixel_x, Pixel_y] = R_stacking[Pixel_x, Pixel_y] + Red>0?Red:0;
                    
                            R_stacking[Pixel_x, Pixel_y] = R_stacking[Pixel_x, Pixel_y] + (Red > 10 ? Red : 0);
                            G_stacking[Pixel_x, Pixel_y] = G_stacking[Pixel_x, Pixel_y] + (Green > 10 ? Green : 0);
                            B_stacking[Pixel_x, Pixel_y] = B_stacking[Pixel_x, Pixel_y] + (Blue > 10 ? Blue : 0);

                   
                    }
                }
            }
            //2. 畫圓心 格式對才能畫
            if (checkBox1.Checked) {
                foreach (KeyValuePair<string, Bitmap> pair in dict)
                {
                    Bitmap ori_bmp = new Bitmap(pair.Value.Width, pair.Value.Height);
                    string ori_name = pair.Key;
                    ori_bmp = pair.Value;

                    char sp0 = '(';
                    char sp2 = ')';
                    char sp3 = '_';

                    string center_string = ori_name.Split(sp0)[1].Split(sp2)[0];//()誇號中間的串
                    int centerX = Convert.ToInt32(center_string.Split(sp3)[0]);
                    int centerY = Convert.ToInt32(center_string.Split(sp3)[1]);
                    int thick = 5;


                    for (int Pixel_x = 0; Pixel_x < ori_bmp.Width; Pixel_x++)
                    {
                        for (int Pixel_y = 0; Pixel_y < ori_bmp.Height; Pixel_y++)
                        {
                            if (Math.Abs(Pixel_x - centerX) < thick && Math.Abs(Pixel_y - centerY) < thick)
                            {
                                R_stacking[Pixel_x, Pixel_y] = 255;
                                G_stacking[Pixel_x, Pixel_y] = 0;
                                B_stacking[Pixel_x, Pixel_y] = 0;
                            }
                        }
                    }
                }

            }
         

            //3. 將疊圖的值存入stack
            for (int Pixel_x = 0; Pixel_x < first.Value.Width; Pixel_x++)//一直是0->input_image.Width
            {
                for (int Pixel_y = 0; Pixel_y < first.Value.Height; Pixel_y++)//初始值為now_y掃到now_y+20
                {
                    int Red = Convert.ToInt32(R_stacking[Pixel_x, Pixel_y] <=255? R_stacking[Pixel_x, Pixel_y ]:255 );
                    int Green = Convert.ToInt32(G_stacking[Pixel_x, Pixel_y] <= 255 ? G_stacking[Pixel_x, Pixel_y] : 255); ;
                    int Blue = Convert.ToInt32(B_stacking[Pixel_x, Pixel_y] <= 255 ? B_stacking[Pixel_x, Pixel_y] : 255);

                    Color color_stack = Color.FromArgb(255,Red, Green, Blue);
                    stacked.SetPixel(Pixel_x, Pixel_y, color_stack);
                }
            }

            return stacked;
        }



        //===Tool End=======================
        #endregion
        private void btn_load_Click(object sender, EventArgs e)
        {

            // open file dialog   
            FolderBrowserDialog open = new FolderBrowserDialog();
            // image filters  
           // open.Filter = "Image Files(*.jpg; *.jpeg; *.gif; *.bmp)|*.jpg; *.jpeg; *.gif; *.bmp";
            if (open.ShowDialog() == DialogResult.OK)
            {
                DirectoryInfo dir0 = new DirectoryInfo(open.SelectedPath);

                FileInfo[] files = dir0.GetFiles();
                StringBuilder sb = new StringBuilder();
                textBox1.Text=open.SelectedPath;

                foreach (FileInfo file in files)
                {
                //    sb.Append("<img src='' data-original='_img/bookImg/" + bookNo + "/" + file.Name + "' style='width:100%;'  />");
                    if(   file.Name.Contains("_stamped.bmp"))  cb_list.Items.Add(file.Name);
                }
             //   bookImg.InnerHtml = sb.ToString();



        //        char sp1 = '\\';
           //     char sp2 = '.';
              //  string name_buff = open.FileName.Split(sp1)[open.FileName.Split(sp1).Length - 1];
               // tb_lens.Text = name_buff.Split(sp2)[0];


            }
        }

        private void textBox1_MouseDown(object sender, MouseEventArgs e)
        {
            textBox1.Text = "";
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            cb_list.Items.Clear();

            if (e.KeyCode == Keys.Enter)
            {

                DirectoryInfo dir0 = new DirectoryInfo(textBox1.Text);

                FileInfo[] files = dir0.GetFiles();
                StringBuilder sb = new StringBuilder();
                foreach (FileInfo file in files)
                {
                    if (checkBox1.Checked)
                    {
                        if (file.Name.Contains(")_stamped.bmp"))
                            cb_list.Items.Add(file.Name);
                    }
                    else {
                        cb_list.Items.Add(file.Name);
                    }

                }
                // textBox1.KeyDown += btn_stack_Click;

                cb_list.Text = "已選取路徑"+ textBox1.Text;
            }
        }


       
        private void btn_stack_Click(object sender, EventArgs e)
        {
            DICT_BMP.Clear();
            for (int i = 0; i < cb_list.Items.Count; i++) {
                //  string path ="@"+ textBox1.Text + cb_list.Items[i].ToString());
                string path =  textBox1.Text+"\\"+ cb_list.Items[i].ToString();
                 Bitmap bmp = new Bitmap(path);
                
                DICT_BMP.Add(cb_list.Items[i].ToString(), bmp);
                
               // pbx_.Image = bmp;
                //Thread.Sleep(500);
            }

            STACKED = StackBitmap(DICT_BMP);
            pbx_.Image = STACKED;
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void btn_save_Click(object sender, EventArgs e)
        {
            SaveFileDialog dialog = new SaveFileDialog();
          dialog.FileName = "Name" + ".bmp";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                int width = Convert.ToInt32(pbx_.Width);
                int height = Convert.ToInt32(pbx_.Height);
                Bitmap bmp = new Bitmap(width, height);
              //  pbx_.DrawToBitmap(bmp, new Rectangle(0, 0, width, height));
                bmp = (Bitmap)pbx_.Image;
                STACKED.Save(dialog.FileName, ImageFormat.Bmp);
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
           
        }
    }
}
