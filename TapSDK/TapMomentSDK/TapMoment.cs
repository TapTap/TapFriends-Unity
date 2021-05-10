using System;
using System.Collections.Generic;

namespace TapTap.Moment
{
    public class TapMoment
    {
        public static void SetCallback(Action<int, string> callback)
        {
            MomentImpl.GetInstance().SetCallback(callback);
        }

        public static void Init(string clientId)
        {
            MomentImpl.GetInstance().Init(clientId);
        }

        public static void Init(string clientId, bool isCN)
        {
            MomentImpl.GetInstance().Init(clientId, isCN);
        }

        public static void Open(Orientation config)
        {
            MomentImpl.GetInstance().Open(config);
        }

        public static void Publish(Orientation config, string[] imagePaths, string content)
        {
            MomentImpl.GetInstance().Publish(config, imagePaths, content);
        }

        public static void PublishVideo(Orientation config, string[] videoPaths, string[] imagePaths, string title,
            string desc)
        {
            MomentImpl.GetInstance().PublishVideo(config, videoPaths, imagePaths, title, desc);
        }

        public static void PublishVideo(Orientation config, string[] videoPaths, string title, string desc)
        {
            MomentImpl.GetInstance().PublishVideo(config, videoPaths, title, desc);
        }

        public static void FetchNotification()
        {
            MomentImpl.GetInstance().FetchNotification();
        }

        public static void DirectlyOpen(Orientation orientation, string page, Dictionary<string, object> extras)
        {
            MomentImpl.GetInstance().DirectlyOpen(orientation, page, extras);
        }

        public static void Close()
        {
            MomentImpl.GetInstance().Close();
        }

        public static void Close(string title, string desc)
        {
            MomentImpl.GetInstance().Close(title, desc);
        }

        public static void SetGameScreenAutoRotate(bool auto)
        {
            MomentImpl.GetInstance().SetUseAutoRotate(auto);
        }
    }
}