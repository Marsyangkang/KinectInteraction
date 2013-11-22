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
using Microsoft.Kinect;
using Microsoft.Kinect.Toolkit;
using Microsoft.Kinect.Toolkit.Controls;
using Microsoft.Kinect.Toolkit.Interaction;


namespace InteractionTest
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
     
        /// <summary>
        /// 创建一个KinectSensorChooserUI对象,来指示当前Kinect的工作状态，
        /// 提示用户Kinect传感器是否工作正常，比如是否断线，是否插到了错误的USB接口上了等等
        /// </summary>
        private KinectSensorChooser sensorChooser;

        /// <summary>
        /// 窗体主函数
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            //加载初始化方法
            Loaded += OnLoaded;
        }

        /// <summary>
        /// 窗体运行时加载方法
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            this.sensorChooser = new KinectSensorChooser();
            this.sensorChooser.KinectChanged += SensorChooserOnKinectChanged;
            this.sensorChooserUi.KinectSensorChooser = this.sensorChooser;
            this.sensorChooser.Start();
             

            //填充scrollView中的圆
            for (int i = 1; i < 20; i++)
            {
                //Kinect Interaction圆形按钮
                var button = new KinectCircleButton
                {
                    Content = i,
                    Height = 200
                };

                int i1 = i;
                //给按钮注册事件，按下给出弹出框提示
                button.Click +=
                    (o, args) => MessageBox.Show("You clicked button #" + i1);

                scrollContent.Children.Add(button);
            }
        }

        /// <summary>
        /// 传感器的状态发生改变触发事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void SensorChooserOnKinectChanged(object sender, KinectChangedEventArgs args)
        {
            //处理初始的Kinect传感器拔掉，连上新的传感器并开启深度和骨骼数据流的情景
            bool error = false;//是否为初始的Kinect传感器
            if (args.OldSensor != null)
            {
                try
                {
                    args.OldSensor.DepthStream.Range = DepthRange.Default;
                    args.OldSensor.SkeletonStream.EnableTrackingInNearRange = false;
                    args.OldSensor.DepthStream.Disable();
                    args.OldSensor.SkeletonStream.Disable();
                }
                catch (InvalidOperationException)
                {
                    // KinectSensor might enter an invalid state while enabling/disabling streams or stream features.
                    // E.g.: sensor might be abruptly unplugged.
                    error = true;
                }
            }

            if (args.NewSensor != null)
            {
                try
                {
                    //开启传感器深度数据数据流
                    args.NewSensor.DepthStream.Enable(DepthImageFormat.Resolution640x480Fps30);
                    //开启骨骼数据流
                    args.NewSensor.SkeletonStream.Enable();

                    try
                    {
                        //默认使用近景模式，K4W可使用近景模式，xbox不支持近景模式
                        args.NewSensor.DepthStream.Range = DepthRange.Near;
                        args.NewSensor.SkeletonStream.EnableTrackingInNearRange = true;
                        args.NewSensor.SkeletonStream.TrackingMode = SkeletonTrackingMode.Seated;
                    }
                    catch (InvalidOperationException)
                    {
                        // Non Kinect for Windows devices do not support Near mode, so reset back to default mode.
                        args.NewSensor.DepthStream.Range = DepthRange.Default;
                        args.NewSensor.SkeletonStream.EnableTrackingInNearRange = false;
                    }
                }
                catch (InvalidOperationException)
                {
                    // KinectSensor might enter an invalid state while enabling/disabling streams or stream features.
                    // E.g.: sensor might be abruptly unplugged.
                    error = true;
                   
                }
                try{
                    if(!error)
                    {
                        //设置kinectRegion的KinectSensor属性到获取的kinectSensor上
                        kinectRegion.KinectSensor = args.NewSensor; 
                    }
                }
                catch(Exception)
                {
                    throw;
                }
            }
        }

        /// <summary>
        /// KinectTileButton按钮监听事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonOnClick(object sender, RoutedEventArgs e)
        {
            //按钮被按下给出弹出框提示
            MessageBox.Show("You clicked me!!!");
        }

    }
}
