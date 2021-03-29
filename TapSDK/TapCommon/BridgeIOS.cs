﻿
using UnityEngine;
using System;
using System.Runtime.InteropServices;
using System.Collections.Concurrent;

namespace TapCommon
{
    public class BridgeIOS : IBridge
    {
        private static BridgeIOS sInstance = new BridgeIOS();

        private ConcurrentDictionary<string, Action<Result>> dic;

        public static BridgeIOS GetInstance()
        {
            return sInstance;
        }

        private BridgeIOS()
        {
            dic = new ConcurrentDictionary<string, Action<Result>>();
        }

        public ConcurrentDictionary<string, Action<Result>> GetConcurrentDictionary()
        {
            return dic;
        }

        private delegate void EngineBridgeDelegate(string result);
        [AOT.MonoPInvokeCallbackAttribute(typeof(EngineBridgeDelegate))]
        static void engineBridgeDelegate(string resultJson)
        {

            Result result = new Result(resultJson);

            ConcurrentDictionary<string, Action<Result>> actionDic = BridgeIOS.GetInstance().GetConcurrentDictionary();

            Action<Result> action = null;

            if (actionDic != null && actionDic.ContainsKey(result.callbackId))
            {
                action = actionDic[result.callbackId];
            }

            if (action != null)
            {
                action(result);
                if (result.onceTime && BridgeIOS.GetInstance().GetConcurrentDictionary().TryRemove(result.callbackId, out Action<Result> outAction))
                {
                    Debug.Log($"TapSDK resolved current Action:{result.callbackId}");
                }
            }
            Debug.Log($"TapSDK iOS BridgeAction last Count:{BridgeIOS.GetInstance().GetConcurrentDictionary().Count}");
        }

        public void Register(string serviceClz, string serviceImp)
        {
            //IOS无需注册
        }

        public void Call(Command command)
        {
            callHandler(command.ToJSON());
        }

        public void Call(Command command, Action<Result> action)
        {
            if (command.callback && !string.IsNullOrEmpty(command.callbackId))
            {
                if (!dic.ContainsKey(command.callbackId))
                {
                    dic.GetOrAdd(command.callbackId, action);
                }
            }
            registerHandler(command.ToJSON(), engineBridgeDelegate);
        }

    [DllImport("__Internal")]
    private static extern void callHandler(string command);

    [DllImport("__Internal")]
    private static extern void registerHandler(string command,EngineBridgeDelegate engineBridgeDelegate);
    }
}