namespace TapBootstrap
{
    public interface ITapUserStatusChangedListener
    {
        void OnLogout(TapError error);

        void OnBind(TapError error);
    }
}