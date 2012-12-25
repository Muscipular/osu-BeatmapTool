using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using QuartzTypeLib;

namespace BeatmapTool.Utils
{
    public class Mp3Player
    {
        public enum MediaStatus { None, Stopped, Paused, Running };
        private FilgraphManager m_objFilterGraph = null;
        private IBasicAudio m_objBasicAudio = null;
        private IMediaEvent m_objMediaEvent = null;
        private IMediaEventEx m_objMediaEventEx = null;
        private IMediaPosition m_objMediaPosition = null;
        private IMediaControl m_objMediaControl = null;

        private MediaStatus m_currentStatus = MediaStatus.None;

        public IMediaEventEx MediaEventEx
        {
            get { return m_objMediaEventEx; }
        }

        public MediaStatus CurrentStatus
        {
            get { return m_currentStatus; }
            set { m_currentStatus = value; }
        }

        public int Volume
        {
            get { return m_objBasicAudio.Volume; }
            set { m_objBasicAudio.Volume = value; }
        }

        public double Duration
        {
            get
            {
                if (m_objMediaPosition != null)
                {
                    return m_objMediaPosition.Duration;
                }
                else
                {
                    return 0.0;
                }
            }
        }

        public double CurrentPosition
        {
            get
            {
                if (m_objMediaPosition != null)
                {
                    return m_objMediaPosition.CurrentPosition;
                }
                else
                {
                    return 0.0;
                }
            }

            set
            {
                if (m_objMediaPosition != null)
                {
                    m_objMediaPosition.CurrentPosition = value;
                }
            }
        }

        /// <summary>
        /// 初始化播放
        /// </summary>
        /// <param name="fileName">文件名称（全路径）</param>
        public void Open(string fileName)
        {
            m_objFilterGraph = new FilgraphManager();
            m_objFilterGraph.RenderFile(fileName);

            m_objBasicAudio = m_objFilterGraph as IBasicAudio;
            m_objMediaEvent = m_objFilterGraph as IMediaEvent;

            m_objMediaEventEx = m_objFilterGraph as IMediaEventEx;

            m_objMediaPosition = m_objFilterGraph as IMediaPosition;

            m_objMediaControl = m_objFilterGraph as IMediaControl;
        }


        /// <summary>
        /// 播放
        /// </summary>
        public void Play()
        {
            if (this.CurrentPosition == this.Duration)
                this.CurrentPosition = 0;
            m_objMediaControl.Run();
            m_currentStatus = MediaStatus.Running;
        }

        /// <summary>
        /// 暂停
        /// </summary>
        public void Pause()
        {
            m_objMediaControl.Pause();
            m_currentStatus = MediaStatus.Paused;
        }

        /// <summary>
        /// 停止
        /// </summary>
        public void Stop()
        {
            m_objMediaControl.Stop();
            m_objMediaPosition.CurrentPosition = 0;
            m_currentStatus = MediaStatus.Stopped;
        }

        /// <summary>
        /// 清理资源
        /// </summary>
        public void Dispose()
        {
            if (m_objMediaControl != null)
                m_objMediaControl.Stop();

            m_currentStatus = MediaStatus.Stopped;

            if (m_objMediaEventEx != null)
                m_objMediaEventEx.SetNotifyWindow(0, 0, 0);

            if (m_objMediaControl != null) m_objMediaControl = null;
            if (m_objMediaPosition != null) m_objMediaPosition = null;
            if (m_objMediaEventEx != null) m_objMediaEventEx = null;
            if (m_objMediaEvent != null) m_objMediaEvent = null;
            if (m_objBasicAudio != null) m_objBasicAudio = null;

            //System.Runtime.InteropServices.Marshal.ReleaseComObject(m_objFilterGraph);

            if (m_objFilterGraph != null) m_objFilterGraph = null;

            //System.Runtime.InteropServices.Marshal.ReleaseComObject
            //QuartzTypeLib.FilgraphManagerClass fm = new FilgraphManagerClass();
        }
    }
}
