using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Media;
using System.Drawing.Imaging;
using System.Drawing;
using System.IO;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using NAudio.Wave.SampleProviders;

namespace voice_recorder
{
    public partial class Form1 : Form
    {
        private string path;
        private readonly SoundPlayer _soundPlayer = new SoundPlayer();
        private string fileName = string.Empty;
        //private WaveOutEvent outputDevise;
        //private AudioFileReader audioFile;
        public Form1()
        {
            InitializeComponent();
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void button4_Click(object sender, EventArgs e)
        {
            List<NAudio.Wave.WaveInCapabilities> sources = new List<NAudio.Wave.WaveInCapabilities>();
            for (int i = 0; i < NAudio.Wave.WaveIn.DeviceCount; i++)
            {
                sources.Add(NAudio.Wave.WaveIn.GetCapabilities(i));
            }

            sourseList.Items.Clear();

            foreach (var source in sources)
            {
                ListViewItem item = new ListViewItem(source.ProductName);
                item.SubItems.Add(new ListViewItem.ListViewSubItem(item, source.Channels.ToString()));
                sourseList.Items.Add(item);
            }
        }
        NAudio.Wave.WaveIn sourceStream = null;
        NAudio.Wave.WaveFileWriter waveWriter = null;
        NAudio.Wave.DirectSoundOut waveOut = null;

        private void button2_Click(object sender, EventArgs e)
        {
            System.Drawing.Drawing2D.GraphicsPath myPath = new System.Drawing.Drawing2D.GraphicsPath();
            myPath.AddEllipse(0, 0, button1.Width, button1.Height);

            Region myRegion = new Region(myPath);
            button1.Region = myRegion;
            if (sourseList.SelectedItems.Count == 0) return;

            SaveFileDialog save = new SaveFileDialog();
            save.Filter = "Wave Filter (*.wav)|*.wav;";
            if (save.ShowDialog() != System.Windows.Forms.DialogResult.OK) return;

            int deviceNimber = sourseList.SelectedItems[0].Index;

            sourceStream = new NAudio.Wave.WaveIn();
            sourceStream.DeviceNumber = deviceNimber;
            sourceStream.WaveFormat = new NAudio.Wave.WaveFormat(44100, NAudio.Wave.WaveIn.GetCapabilities(deviceNimber).Channels);
            sourceStream.DataAvailable += new EventHandler<NAudio.Wave.WaveInEventArgs>(sourceStream_DataAvailable);
            waveWriter = new NAudio.Wave.WaveFileWriter(save.FileName, sourceStream.WaveFormat);

            waveOut = new NAudio.Wave.DirectSoundOut();
            waveOut.Play();


            sourceStream.StartRecording();
            waveOut.Play();
        }
        private void sourceStream_DataAvailable(object sender, NAudio.Wave.WaveInEventArgs e)
        {
            if (waveWriter == null) return;

            waveWriter.WriteData(e.Buffer, 0, e.BytesRecorded);
            waveWriter.Flush();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (waveOut != null)
            {
                waveOut.Stop();
                waveOut.Dispose();
                waveOut = null;
            }

            if (sourceStream != null)
            {
                sourceStream.StopRecording();
                sourceStream.Dispose();
                sourceStream = null;
            }
            if (waveWriter != null)
            {
                waveWriter.Dispose();
                waveWriter = null;
            }
            if (_soundPlayer != null)
            {
                _soundPlayer.Stop();
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderPicker = new FolderBrowserDialog(); // открываем папку
            if (folderPicker.ShowDialog() == DialogResult.OK) // если выбрали
            {
                listView1.Items.Clear(); // очищаем наш список

                listView1.MouseDoubleClick += OnMouseDoubleClickEvent;

                string[] files = Directory.GetFiles(folderPicker.SelectedPath); // берем все файлы
                path = folderPicker.SelectedPath;
                foreach (string file in files) // пробегаем по списку
                {
                    var extension = Path.GetExtension(file); // берем расширение
                    if (extension != null && extension.Contains("wav")) // если wav 
                    {
                        FileInfo fileInfo = new FileInfo(file);
                        ListViewItem listViewItem = new ListViewItem(fileInfo.Name);
                        listViewItem.SubItems.Add(fileInfo.FullName);

                        listView1.Items.Add(listViewItem);
                    }
                }

            }
        }
        private void OnMouseDoubleClickEvent(object sender, MouseEventArgs e)
        {
            var selItem = listView1.SelectedItems[0];
            string a = path + "\\" + selItem.Text.ToString();
            _soundPlayer.SoundLocation = a;
            _soundPlayer.Play();

        }
    }
}