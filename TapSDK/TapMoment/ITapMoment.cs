using System;

namespace TapMoment
{
    public interface ITapMoment
    {
        void SetCallback(Action<int, string> callback);

        void Init(string clientId);

        void Init(string clientId, bool isCN);

        void Open(Orientation orientation);

        void Publish(Orientation orientation, string[] imagePaths, string content);

        void PublishVideo(Orientation orientation, string[] videoPaths, string[] imagePaths, string title,
            string desc);

        void PublishVideo(Orientation orientation, string[] videoPaths, string title, string desc);

        void FetchNotification();

        void Close();

        void Close(string title, string content);

        void SetUseAutoRotate(bool useAuto);

        void OpenSceneEntry(Orientation orientation, string sceneId);

        void OpenUserCenter(Orientation orientation, string userId);
    }
}