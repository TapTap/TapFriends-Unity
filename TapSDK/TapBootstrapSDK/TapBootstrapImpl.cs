using System;
using System.Collections.Generic;
using TapTap.Common;

namespace TapTap.Bootstrap
{
    public class TapBootstrapImpl : ITapBootstrap
    {
        private TapBootstrapImpl()
        {
            EngineBridge.GetInstance().Register(TapBootstrapConstants.TAP_BOOTSTARP_CLZ,
                TapBootstrapConstants.TAP_BOOTSTARP_IMPL);
        }

        private static volatile TapBootstrapImpl _sInstance;

        private static readonly object Locker = new object();

        public static TapBootstrapImpl GetInstance()
        {
            lock (Locker)
            {
                if (_sInstance == null)
                {
                    _sInstance = new TapBootstrapImpl();
                }
            }

            return _sInstance;
        }

        public void Init(TapConfig config)
        {
            var command = new Command.Builder()
                .Service(TapBootstrapConstants.TAP_BOOTSTARP_SERVICE)
                .Method("initWithConfig")
                .Args("initWithConfig", config.ToJson())
                .CommandBuilder();
            
            EngineBridge.GetInstance().CallHandler(command);

            TapCommon.SetXua();
        }

        public void Login(LoginType loginType, string[] permissions)
        {
            var command = new Command.Builder()
                .Service(TapBootstrapConstants.TAP_BOOTSTARP_SERVICE)
                .Method("login")
                .Args("login", (int) loginType)
                .Args("permissions", permissions)
                .CommandBuilder();
            EngineBridge.GetInstance().CallHandler(command);
        }

        public void Login(string[] permissions)
        {
            var command = new Command.Builder()
                .Service(TapBootstrapConstants.TAP_BOOTSTARP_SERVICE)
                .Method("login")
                .Args("login", (int) LoginType.TAPTAP)
                .Args("permissions", permissions)
                .CommandBuilder();
            EngineBridge.GetInstance().CallHandler(command);
        }

        public void Bind(LoginType loginType, string[] permissions)
        {
            var command = new Command.Builder()
                .Service(TapBootstrapConstants.TAP_BOOTSTARP_SERVICE)
                .Method("bind")
                .Args("bind", (int) loginType)
                .Args("permissions", permissions).CommandBuilder();

            EngineBridge.GetInstance().CallHandler(command);
        }

        public void GetTestQualification(Action<bool, TapError> action)
        {
            var command = new Command.Builder()
                .Service(TapBootstrapConstants.TAP_BOOTSTARP_SERVICE)
                .Method("getTestQualification")
                .Callback(true)
                .OnceTime(true)
                .CommandBuilder();

            EngineBridge.GetInstance().CallHandler(command, result =>
            {
                if (!CheckResultLegal(result))
                {
                    action(false, TapError.UndefinedError());
                    return;
                }

                if (!(Json.Deserialize(result.content) is Dictionary<string, object> dic))
                {
                    action(false, TapError.UndefinedError());
                    return;
                }

                action(SafeDictionary.GetValue<int>(dic, "userTestQualification") == 1,
                    TapError.SafeConstructorTapError(SafeDictionary.GetValue<string>(dic, "error")));
            });
        }

        public void RegisterUserStatusChangedListener(ITapUserStatusChangedListener listener)
        {
            var command = new Command.Builder()
                .Service(TapBootstrapConstants.TAP_BOOTSTARP_SERVICE)
                .Method("registerUserStatusChangedListener")
                .Callback(true)
                .OnceTime(false)
                .CommandBuilder();

            EngineBridge.GetInstance().CallHandler(command, result =>
            {
                if (!CheckResultLegal(result))
                {
                    return;
                }

                var wrapper = new TapUserStatusWrapper(result.content);
                var error = TapError.SafeConstructorTapError(wrapper.wrapper);
                if (wrapper.userStatusCallbackCode == 1)
                {
                    listener.OnLogout(error);
                    return;
                }

                listener.OnBind(error);
            });
        }

        public void RegisterLoginResultListener(ITapLoginResultListener listener)
        {
            var command = new Command.Builder()
                .Service(TapBootstrapConstants.TAP_BOOTSTARP_SERVICE)
                .Method("registerLoginResultListener")
                .OnceTime(false)
                .Callback(true)
                .CommandBuilder();

            EngineBridge.GetInstance().CallHandler(command, result =>
            {
                if (!CheckResultLegal(result))
                {
                    listener.OnLoginError(new TapError(TapErrorCode.ERROR_CODE_BRIDGE_EXECUTE,
                        "TapSDK RegisterLoginResultListener Error!"));
                    return;
                }

                var wrapper = new TapLoginWrapper(result.content);
                switch (wrapper.loginCallbackCode)
                {
                    case 0:
                        listener.OnLoginSuccess(new AccessToken(wrapper.wrapper));
                        return;
                    case 1:
                        listener.OnLoginCancel();
                        return;
                    default:
                        listener.OnLoginError(TapError.SafeConstructorTapError(wrapper.wrapper));
                        break;
                }
            });
        }

        public void GetUser(Action<TapUser, TapError> action)
        {
            var command = new Command.Builder()
                .Service(TapBootstrapConstants.TAP_BOOTSTARP_SERVICE)
                .Method("getUser")
                .OnceTime(true)
                .Callback(true)
                .CommandBuilder();

            EngineBridge.GetInstance().CallHandler(command, result =>
            {
                if (!CheckResultLegal(result))
                {
                    action(null, new TapError(TapErrorCode.ERROR_CODE_BRIDGE_EXECUTE, "TapSDK GetUser Error!"));
                    return;
                }

                var wrapper = new TapUserInfoWrapper(result.content);

                if (wrapper.getUserInfoCode == 0)
                {
                    action(new TapUser(wrapper.wrapper), null);
                    return;
                }

                action(null, TapError.SafeConstructorTapError(wrapper.wrapper));
            });
        }

        public void GetDetailUser(Action<TapUserDetail, TapError> action)
        {
            var command = new Command.Builder()
                .Service(TapBootstrapConstants.TAP_BOOTSTARP_SERVICE)
                .Method("getUserDetails")
                .OnceTime(true)
                .Callback(true)
                .CommandBuilder();

            EngineBridge.GetInstance().CallHandler(command, result =>
            {
                if (!CheckResultLegal(result))
                {
                    action(null, new TapError(TapErrorCode.ERROR_CODE_BRIDGE_EXECUTE, "TapSDK GetDetailInfo Error!"));
                    return;
                }

                var detailInfoWrapper = new TapUserDetailInfoWrapper(result.content);
                if (detailInfoWrapper.getUserDetailInfoCode == 0)
                {
                    action(new TapUserDetail(detailInfoWrapper.wrapper), null);
                    return;
                }

                action(null, TapError.SafeConstructorTapError(detailInfoWrapper.wrapper));
            });
        }

        public void GetAccessToken(Action<AccessToken, TapError> action)
        {
            var command = new Command.Builder()
                .Service(TapBootstrapConstants.TAP_BOOTSTARP_SERVICE)
                .Method("getCurrentToken")
                .OnceTime(true)
                .Callback(true)
                .CommandBuilder();

            EngineBridge.GetInstance().CallHandler(command, result =>
            {
                if (!CheckResultLegal(result))
                {
                    action(null, new TapError(TapErrorCode.ERROR_CODE_BRIDGE_EXECUTE, "TapSDK GetAccessToken Error!"));
                    return;
                }

                action(new AccessToken(result.content), null);
            });
        }

        public void Logout()
        {
            var command = new Command.Builder()
                .Service(TapBootstrapConstants.TAP_BOOTSTARP_SERVICE)
                .Method("logout")
                .CommandBuilder();

            EngineBridge.GetInstance().CallHandler(command);
        }

        public void OpenUserCenter()
        {
            // var command = new Command.Builder()
            //     .Service(TapBootstrapConstants.TAP_BOOTSTARP_SERVICE)
            //     .Method("openUserCenter")
            //     .CommandBuilder();
            //
            // EngineBridge.GetInstance().CallHandler(command);
        }

        public void SetPreferLanguage(TapLanguage tapLanguage)
        {
            var command = new Command.Builder()
                .Service(TapBootstrapConstants.TAP_BOOTSTARP_SERVICE)
                .Method("setPreferredLanguage")
                .Args("preferredLanguage", (int) tapLanguage)
                .CommandBuilder();

            EngineBridge.GetInstance().CallHandler(command);
        }

        private bool CheckResultLegal(Result result)
        {
            if (result == null)
            {
                return false;
            }

            if (result.code != Result.RESULT_SUCCESS)
            {
                return false;
            }

            return !string.IsNullOrEmpty(result.content);
        }
    }
}