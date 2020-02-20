using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel;
using System.Threading;
using System.Timers;

namespace Beginer.ChristmasLight
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// Timer Implementation
    public partial class MainWindow : Window
    {
        private Ellipse[] ellipse_object_array = new Ellipse[9];
        private Brush[] brushes = { Brushes.White, Brushes.Red };
        private Brush clonnedBrush;
        
        private bool isEven = false;
        private BackgroundWorker backgroundWorker;

        const string START_LIGHTING = "Start Lighting";
        const string STOP_LIGHTING = "Stop Lighting";

        const byte MIN_OPACITY = 0;
        const byte MAX_OPACITY = 100;

        private System.Timers.Timer timer;
        private int milliseconds;
        private int seconds;
        private bool isTimeOver = false;
        private int count = 0;
        private int opacity = 10;
        const int TIMER_INTERVAL=500;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void christmass_light_window_Loaded(object sender, RoutedEventArgs e)
        {
            ellipse_object_array[0] = ellipse_1;
            ellipse_object_array[1] = ellipse_2;
            ellipse_object_array[2] = ellipse_3;
            ellipse_object_array[3] = ellipse_4;
            ellipse_object_array[4] = ellipse_5;
            ellipse_object_array[5] = ellipse_6;
            ellipse_object_array[6] = ellipse_7;
            ellipse_object_array[7] = ellipse_8;
            ellipse_object_array[8] = ellipse_9;

            backgroundWorker = new BackgroundWorker();

            backgroundWorker.WorkerReportsProgress = true;
            backgroundWorker.WorkerSupportsCancellation = true;

            backgroundWorker.DoWork += worker_DoWork;
            backgroundWorker.ProgressChanged += worker_ProgressChanged;
            backgroundWorker.RunWorkerCompleted += worker_WorkCompleted;

            var br = typeof(Brushes).GetProperties().Select(s => new CustomBrushes{ CustomBrush = s.GetValue(null) as Brush,
                getMethod=s.GetGetMethod(),
                BrushName=s.Name}).ToList();
            colour_combo.ItemsSource = br;

        }
        /// <summary>
        /// Controls the start and stop of lighting.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void christmas_light_button_Click(object sender, RoutedEventArgs e)
        {
            var sender_button = sender as Button;         

            if (!backgroundWorker.IsBusy)
            {
                sender_button.Content = STOP_LIGHTING;
                backgroundWorker.RunWorkerAsync();
                SetTimer();
            }
            else
            {
                sender_button.Content = START_LIGHTING;
                backgroundWorker.CancelAsync();
            }
            
        }

        /// <summary>
        /// Animation is being done based on the odd even index of array:-ellipse_object_array
        /// </summary>
        private void Animate()
        {
            if (isEven)
            {
                for (int i = 1; i < 9; i += 2)
                {
                    ellipse_object_array[i].Fill = brushes[1];
                }
                for (int i = 0; i < 9; i += 2)
                {
                    ellipse_object_array[i].Fill = brushes[0];
                }
                isEven = false;
            }
            else
            {
                for (int i = 1; i < 9; i += 2)
                {
                    ellipse_object_array[i].Fill = brushes[0];
                }
                for (int i = 0; i < 9; i += 2)
                {
                    ellipse_object_array[i].Fill = brushes[1];
                }
                isEven = true;
            }
            
        }

        /// <summary>
        /// A timer is set, which ticks by the specified interval.
        /// 
        /// </summary>
        private void SetTimer()
        {
            timer = new System.Timers.Timer(TIMER_INTERVAL);
            timer.Elapsed += onTimedEvent;
            timer.Enabled = true;
            isTimeOver = false;
        }

        /// <summary>
        /// The set timer is removed along with the onTimed eventHandler.
        /// </summary>
        private void removeTimer()
        {
            timer.Elapsed -= onTimedEvent;
            isTimeOver = true;
            count = 0;
        }

        /// <summary>
        /// The event handler ticks at the specified interval and keeps check of if time is over.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        private void onTimedEvent(object source,ElapsedEventArgs e)
        {
            count += 1;
            if (count*TIMER_INTERVAL>=milliseconds)
            {
                isTimeOver = true;
                count = 0;
            }
        }
        /// <summary>
        /// Animation is being done asynchronously.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void worker_DoWork(object sender,DoWorkEventArgs e)
        {
            var backGroundWorker_ = sender as BackgroundWorker;
            
            for (int i = 0; !isTimeOver;i++)
            {
                backGroundWorker_.ReportProgress(i);
                Thread.Sleep(500);
                if (backGroundWorker_.CancellationPending)
                {
                    e.Cancel = true;
                    return;
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void worker_ProgressChanged(object sender,ProgressChangedEventArgs e)
        {
            Animate();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Sender"></param>
        /// <param name="e"></param>
        void worker_WorkCompleted(object Sender,RunWorkerCompletedEventArgs e)
        {
            christmass_light_button.Content = START_LIGHTING;
            removeTimer();
        }
        /// <summary>
        /// Keeps check of color value change in comboBox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void colour_combo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var _c = sender as ComboBox;

            var cb = _c.SelectedItem as CustomBrushes;
            brushes[1] = cb.getMethod.Invoke(new SolidColorBrush(),new object[] { }) as SolidColorBrush;
            if (brushes[1].IsFrozen)
            {
                clonnedBrush = brushes[1].Clone();
                clonnedBrush.Opacity = opacity;
                brushes[1] = clonnedBrush;
            }
            
        }
        /// <summary>
        /// Keeps a check of 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lighting_duration_TextChanged(object sender, TextChangedEventArgs e)
        {
            var _t = sender as TextBox;
            try
            {
                seconds = int.Parse(_t.Text);
                
            }
            catch (Exception)
            {

                seconds = 1;
            }
            milliseconds = seconds * 1000;
        }

        /// <summary>
        /// Changes intensity value after any change to the intensity is made 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lighting_intensity_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox temp_textBox;
            temp_textBox = sender as TextBox;
            try
            {
                opacity = Int16.Parse(temp_textBox.Text);
                if (brushes[1].IsFrozen)
                {
                    clonnedBrush = brushes[1].Clone();
                    clonnedBrush.Opacity = opacity/100;
                    brushes[1] = clonnedBrush;
                }
                else
                {
                    double fl;
                    fl = double.Parse(temp_textBox.Text) / 100;
                    brushes[1].Opacity = fl;
                }

            }
            catch (Exception)
            {
                                
            }
        }

        /// <summary>
        /// Changes the value of intensity by 1 mark.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lighting_intensity_KeyDown(object sender, KeyEventArgs e)
        {
            TextBox temp_textBox;
            int obtained_number;

            temp_textBox = sender as TextBox;
            try
            {
                obtained_number = int.Parse(temp_textBox.Text);
            }
            catch (Exception)
            {
                return;
            }
            switch (e.Key)
            {
                case Key.Up:
                    if (obtained_number < MAX_OPACITY)
                    {
                        obtained_number += 1;
                        temp_textBox.Text = obtained_number.ToString();
                    }
                    break;
                case Key.Down:
                    if (obtained_number > MIN_OPACITY)
                    {
                        obtained_number -= 1;
                        temp_textBox.Text = obtained_number.ToString();
                    }
                    break;
                default:
                    break;
            }
        }
    }
    /// <summary>
    /// A composite datatype
    /// </summary>
    class CustomBrushes
    {
        public string BrushName { get; set; }
        public Brush CustomBrush { get; set; }
        public System.Reflection.MethodInfo getMethod { get; set; }
    }
}
