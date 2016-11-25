using ServiceStack.Redis;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using System.Linq;
using System.Threading.Tasks;

namespace JeuDeMemoire
{

    public partial class Form1 : Form
    {
        private PictureBox[] pictureBoxes;
        private Bitmap[] pictures;
        private Image[] images = new Image[48];
        private Dictionary<int, Boolean> boxesStatus = new Dictionary<int, Boolean>();
        private PictureBox pictureBoxRecentlyAdded;
        private int time = 0;
        private int nbDiscoveredPhoto = 0;
        private RedisClient redisClient = new RedisClient("localhost");

        public Form1()
        {
            InitializeComponent();
            tableLayoutPanel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            Initialise();
            Console.WriteLine("pictureBox.Width: " + pictureBoxes[0].Width);
            Console.WriteLine(redisClient.GetAllEntriesFromHash("scores").Count);
            
            //handleVictory();
        }
        public void Initialise()
        {
            pictureBoxes = new PictureBox[48] {pictureBox1, pictureBox2, pictureBox3, pictureBox4, pictureBox5, pictureBox6, pictureBox7, pictureBox8,
            pictureBox9, pictureBox10, pictureBox11, pictureBox12, pictureBox13, pictureBox14, pictureBox15, pictureBox16, pictureBox17,
            pictureBox18, pictureBox19, pictureBox20, pictureBox21, pictureBox22, pictureBox23, pictureBox24, pictureBox25, pictureBox26,
            pictureBox27, pictureBox28, pictureBox29, pictureBox30, pictureBox31, pictureBox32, pictureBox33, pictureBox34, pictureBox35,
            pictureBox36, pictureBox37, pictureBox38, pictureBox39, pictureBox40, pictureBox41, pictureBox42, pictureBox43, pictureBox44,
            pictureBox45, pictureBox46, pictureBox47, pictureBox48};
            try
            {
                pictures = new Bitmap[24]  {new Bitmap(global::JeuDeMemoire.Resources.Resource1.carte0),
                    new Bitmap(global::JeuDeMemoire.Resources.Resource1.carte1), new Bitmap(global::JeuDeMemoire.Resources.Resource1.carte2),
                new Bitmap(global::JeuDeMemoire.Resources.Resource1.carte3), new Bitmap(global::JeuDeMemoire.Resources.Resource1.carte4),
                    new Bitmap(global::JeuDeMemoire.Resources.Resource1.carte5), new Bitmap(global::JeuDeMemoire.Resources.Resource1.carte6),
                new Bitmap(global::JeuDeMemoire.Resources.Resource1.carte7), new Bitmap(global::JeuDeMemoire.Resources.Resource1.carte8),
                    new Bitmap(global::JeuDeMemoire.Resources.Resource1.carte9), new Bitmap(global::JeuDeMemoire.Resources.Resource1.carte10),
                new Bitmap(global::JeuDeMemoire.Resources.Resource1.carte11), new Bitmap(global::JeuDeMemoire.Resources.Resource1.carte12),
                    new Bitmap(global::JeuDeMemoire.Resources.Resource1.carte13), new Bitmap(global::JeuDeMemoire.Resources.Resource1.carte14),
                new Bitmap(global::JeuDeMemoire.Resources.Resource1.carte15), new Bitmap(global::JeuDeMemoire.Resources.Resource1.carte16),
                    new Bitmap(global::JeuDeMemoire.Resources.Resource1.carte17), new Bitmap(global::JeuDeMemoire.Resources.Resource1.carte18),
                new Bitmap(global::JeuDeMemoire.Resources.Resource1.carte19), new Bitmap(global::JeuDeMemoire.Resources.Resource1.carte20),
                    new Bitmap(global::JeuDeMemoire.Resources.Resource1.carte21), new Bitmap(global::JeuDeMemoire.Resources.Resource1.carte22),
                new Bitmap(global::JeuDeMemoire.Resources.Resource1.carte23)};
            }
            catch(Exception e)
            {
                Console.WriteLine(e.StackTrace);
            }
            for (int i = 0; i < pictureBoxes.Length; i++)
            {
                pictureBoxes[i].Enabled = false;
            }
        }
        public void FillingImageBoxes()
        {
            int randomInt = new int();
            Random rnd = new Random();
            Dictionary<int, Boolean> filledBoxes = new Dictionary<int, Boolean>();
            for(int o = 0; o < 48; o++)
            {
                filledBoxes.Add(o, false);
                Console.WriteLine(filledBoxes[o]);
            }
            //first box
            randomInt = rnd.Next(0, 48);
            
            //pictureBoxes[randomInt].Image = pictures[0];
            images[randomInt] = pictures[0];
            filledBoxes[randomInt] = true;

            //second box
            randomInt = rnd.Next(0, 48);
            while (filledBoxes[randomInt] == true)
            {
                randomInt = rnd.Next(0, 48);
            }
            //pictureBoxes[randomInt].Image = pictures[0];
            images[randomInt] = pictures[0];
            filledBoxes[randomInt] = true;
            for (int p = 1; p < 24; p++)
            {
                //first box     
                randomInt = rnd.Next(0, 48);
                while (filledBoxes[randomInt] == true)
                {
                    randomInt = rnd.Next(0, 48);
                }
                //pictureBoxes[randomInt].Image = pictures[p];
                images[randomInt] = pictures[p];
                filledBoxes[randomInt] = true;

                //second box
                randomInt = rnd.Next(0, 48);
                while (filledBoxes[randomInt] == true)
                {
                    randomInt = rnd.Next(0, 48);
                }
                //pictureBoxes[randomInt].Image = pictures[p];
                images[randomInt] = pictures[p];
                filledBoxes[randomInt] = true;
            }

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if(button1.Text == "Start")
            {
                for (int i = 0; i < pictureBoxes.Length; i++)
                {
                    pictureBoxes[i].Image = new Bitmap(global::JeuDeMemoire.Properties.Resources.dos);
                    pictureBoxes[i].Enabled = true;
                }
                time = 0;
                timer1.Start();
                button1.Text = "Stop";
                Console.WriteLine("Started");
                FillingImageBoxes();
                button2.Enabled = true;
            }
            else
            {
                timer1.Stop();
                button1.Text = "Start";
                Console.WriteLine("Stopped");
                for (int i = 0; i < pictureBoxes.Length; i++)
                {
                    pictureBoxes[i].Image = new Bitmap(global::JeuDeMemoire.Properties.Resources.dos);
                    pictureBoxes[i].Enabled = false;
                }
                button2.Enabled = false;
            }
        }



        public void ClickPictureBox(object sender, EventArgs e)
        {
            System.Media.SoundPlayer player = new System.Media.SoundPlayer(global::JeuDeMemoire.Resources.Resource1.click);
            player.Play();

            int range = getPictureBoxRange((PictureBox)sender);
            Console.WriteLine(range);
            ((PictureBox)sender).Image = (Bitmap)images.GetValue(range);

            if (pictureBoxRecentlyAdded == null)
            {
                Console.WriteLine("first photo");
                pictureBoxRecentlyAdded = (PictureBox)sender;
                pictureBoxRecentlyAdded.Enabled = false;
            }
            else
            {
                Console.WriteLine("second photo");
                if (((PictureBox)sender).Image.Equals(pictureBoxRecentlyAdded.Image))
                {
                    Console.WriteLine("the same photos");
                    ((PictureBox)sender).Enabled = false;
                    nbDiscoveredPhoto += 2;
                    if(nbDiscoveredPhoto == 48)
                    {
                        handleVictory();                        
                    }
                    Console.WriteLine("nbDiscoveredPhoto: " + nbDiscoveredPhoto);
                }
                else
                {
                    Console.WriteLine("not the same photos");
                    ((PictureBox)sender).Image = (Bitmap)images.GetValue(range);
                    //Console.WriteLine(((Image)images.GetValue(range)));
                    pictureBoxRecentlyAdded.Image = new Bitmap(global::JeuDeMemoire.Properties.Resources.dos);
                    ((PictureBox)sender).Image = new Bitmap(global::JeuDeMemoire.Properties.Resources.dos);
                    pictureBoxRecentlyAdded.Enabled = true;
                    //((PictureBox)sender).Enabled = false;
                }
                pictureBoxRecentlyAdded = null;
            }
        }

        private void handleVictory()
        {
            timer1.Stop();
            button2.Enabled = false;
            button1.Text = "Start";
            System.Media.SoundPlayer player = new System.Media.SoundPlayer(global::JeuDeMemoire.Resources.Resource1.You_win);
            player.Play();
            string username = InputBox.ShowDialog("You win!", "Type your name please");
            redisClient.SetEntryInHash("scores", username, "" + time);
        }

        
        private void Common_MouseHover(object sender, EventArgs e)
        {
            System.Media.SoundPlayer player = new System.Media.SoundPlayer(global::JeuDeMemoire.Resources.Resource1.click);
            player.Play();
        }

        public int getPictureBoxRange(PictureBox pictureBox)
        {
            for(int i = 0; i < pictureBoxes.Length; i++)
            {
                if(pictureBoxes[i].Name == pictureBox.Name)
                {
                    return i;
                }
            }
            return -1;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            time++;
            labelTime.Text = ""+time;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if(button2.Text == "Pause")
            {
                timer1.Stop();
                button2.Text = "Resume";
                for (int i = 0; i < pictureBoxes.Length; i++)
                {
                    pictureBoxes[i].Enabled = false;
                }
            }
            else
            {
                timer1.Start();
                button2.Text = "Pause";
                for (int i = 0; i < pictureBoxes.Length; i++)
                {
                    pictureBoxes[i].Enabled = true;
                }
            }
        }

        private void bestScoreToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DisplayScores.ShowScores();
        }
    }



    public static class InputBox
    {
        public static string ShowDialog(string text, string caption)
        {
            Form prompt = new Form();
            prompt.Width = 500;
            prompt.Height = 150;
            prompt.FormBorderStyle = FormBorderStyle.FixedDialog;
            prompt.Text = caption;
            prompt.StartPosition = FormStartPosition.CenterScreen;
            Label textLabel = new Label() { Left = 50, Top = 20, Text = text };
            TextBox textBox = new TextBox() { Left = 50, Top = 50, Width = 400 };
            Button confirmation = new Button() { Text = "Ok", Left = 350, Width = 100, Top = 70, DialogResult = DialogResult.OK };
            confirmation.Click += (sender, e) => { prompt.Close(); };
            prompt.Controls.Add(textBox);
            prompt.Controls.Add(confirmation);
            prompt.Controls.Add(textLabel);
            prompt.AcceptButton = confirmation;

            return prompt.ShowDialog() == DialogResult.OK ? textBox.Text : "";
        }
    }

    public static class DisplayScores
    {
        private static RedisClient redisClient = new RedisClient("localhost");
        public static void ShowScores()
        {
            Form form = new Form();
            form.Width = 500;
            form.Height = 300;
            
            form.FormBorderStyle = FormBorderStyle.FixedDialog;
            form.Text = "Best time";
            form.MaximizeBox = false;
            form.StartPosition = FormStartPosition.CenterScreen;

            ListView list = new ListView();
            list.View = View.Details;
            list.Columns.Add("Username", 100);
            list.Columns.Add("Time", 100);

            Dictionary<String, String> scores =  redisClient.GetAllEntriesFromHash("scores");
            ListViewItem item;
            String[] scoreEntry = new String[2];

            Dictionary<String, int> sortedScores = new Dictionary<string, int>();
            foreach (KeyValuePair<String, String> entry in scores)
            {
                sortedScores.Add(entry.Key, Int32.Parse(entry.Value));
            }

            var items = from pair in sortedScores
                        orderby pair.Value descending
                        select pair;

            foreach (var it in items.OrderBy(i => i.Value))
            {
                Console.WriteLine(it);
                scoreEntry[0] = it.Key;
                scoreEntry[1] = "" + it.Value;
                item = new ListViewItem(scoreEntry);
                list.Items.Add(item);
            }
            form.Controls.Add(list);
            list.Dock = DockStyle.Fill;
            form.Show();
        }
    }

}
