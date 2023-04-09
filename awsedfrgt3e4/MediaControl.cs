using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Media;
using System.Windows.Media;
using System.Windows;
using System.Windows.Controls;
using System.Collections;
using System.Windows.Shapes;

namespace awsedfrgt3e4.src
{
    internal class MediaControl
    {
        public void PlaySC()
        {
            try
            {
                System.IO.Stream str = Properties.Resources.MacosScreenShot;
                SoundPlayer player = new SoundPlayer(str);
                player.Play();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        public void PlayNext()
        {
            try
            {
                System.IO.Stream str = Properties.Resources.Next;
                SoundPlayer player = new SoundPlayer(str);
                player.Play();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
