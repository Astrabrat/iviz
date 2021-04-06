﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Iviz.App;
using Iviz.Controllers;
using Iviz.Core;
using Iviz.Msgs;
using Iviz.Msgs.RosgraphMsgs;
using Iviz.Resources;
using Iviz.Ros;
using Iviz.XmlRpc;
using JetBrains.Annotations;
using Newtonsoft.Json;
using TMPro;
using Unity.Collections;
using UnityEngine;
using Logger = Iviz.Core.Logger;
#if UNITY_WSA
using Microsoft.MixedReality.Toolkit;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.XR.WSA;

#endif

namespace Iviz.Hololens
{
#if UNITY_WSA
    sealed class HololensManager : MonoBehaviour, ISettingsManager
    {
        static readonly Pose InfinitePose = new Pose(new Vector3(5000, 5000, 5000), Quaternion.identity);

        static HololensManager instance;
        static MarkerResourcePool resourcePool;

        public static MarkerResourcePool ResourcePool =>
            resourcePool ?? (resourcePool = new MarkerResourcePool(instance.gameObject));

        [SerializeField] FloorFrameObject floorFrame = null;
        [SerializeField] GameObject floorHelper = null;
        [SerializeField] HololensHandMenuObject hololensHandMenu = null;
        [SerializeField] GameObject consoleLog = null;
        [SerializeField] TMP_Text consoleText = null;
        [SerializeField] HololensMarkerMenu markerMenu = null;

        FrameNode node;
        TfFrame leftPalm;

        float leftHandScale = 1;
        Pose leftPalmPose;

        static TfFrame RootFrame => TfListener.RootFrame;
        static IEnumerable<ModuleData> ModuleDatas => ModuleListPanel.Instance.ModuleDatas;
        static string MyId => ConnectionManager.Connection.MyId;

        void Awake()
        {
            instance = this;
        }

        void Start()
        {
            if (ModuleListPanel.Initialized)
            {
                Initialize();
            }
            else
            {
                ModuleListPanel.InitFinished += Initialize;
            }
        }

        void Initialize()
        {
            ThreadPool.SetMinThreads(25, 20);

            try
            {
                Logger.Debug("Hololens Manager: Initializing!");
                Settings.SettingsManager = this;
                Settings.ScreenshotManager = new HololensScreenshotManager();


                ModuleListPanel.InitFinished -= Initialize;

                if (!UnityEngine.Application.isEditor)
                {
                    floorHelper.gameObject.SetActive(false);
                }

                floorFrame.OkClicked += StartWorld;


                ModuleListPanel.Instance.MenuDialog = markerMenu;

                //StartLog();
                StartRosConnection();
                StartHandMenu();
                //StartPalms();
                StartOriginPlaceMode();
            }
            catch (Exception e)
            {
                Debug.Log(e);
                Logger.Error("Error initializing Hololens Manager", e);
            }
        }

        readonly Queue<string> messageQueue = new Queue<string>();
        readonly StringBuilder builder = new StringBuilder();
        bool queueHasChanged;

        void StartLog()
        {
            void AddToQueue(string s)
            {
                messageQueue.Enqueue(s);
                queueHasChanged = true;
                if (messageQueue.Count > 100)
                {
                    messageQueue.Dequeue();
                }
            }

            Logger.LogInternal += AddToQueue;
            ConnectionManager.LogMessageArrived += (in Log message) =>
            {
                string messageTime = message.Header.Stamp == default
                    ? ""
                    : message.Header.Stamp.ToDateTime().ToLocalTime().ToString("HH:mm:ss");

                string messageLevel;
                switch ((LogLevel) message.Level)
                {
                    case LogLevel.Debug:
                        messageLevel = "[DEBUG]";
                        break;
                    case LogLevel.Error:
                        messageLevel = "[ERROR]";
                        break;
                    case LogLevel.Fatal:
                        messageLevel = "[FATAL]";
                        break;
                    case LogLevel.Info:
                        messageLevel = "[INFO]";
                        break;
                    case LogLevel.Warn:
                        messageLevel = "[WARN]";
                        break;
                    default:
                        messageLevel = "[?]";
                        break;
                }

                string messageString = $"<b>[{messageTime}] {messageLevel} [{message.Name}]:</b> {message.Msg}";
                AddToQueue(messageString);
            };

            GameThread.EverySecond += () =>
            {
                if (!consoleLog.activeSelf)
                {
                    return;
                }

                if (!queueHasChanged)
                {
                    return;
                }

                builder.Length = 0;
                foreach (string msg in messageQueue)
                {
                    if (msg == null)
                    {
                        Logger.Debug("HololensManager: Got null message!");
                        continue;
                    }

                    const int maxSize = 300;
                    if (msg.Length > maxSize)
                    {
                        builder.Append(msg.Substring(0, maxSize)).Append(" ... +").Append(msg.Length - maxSize)
                            .AppendLine(" chars");
                    }
                    else
                    {
                        builder.AppendLine(msg);
                    }
                }

                consoleText.text = builder.ToString();
                queueHasChanged = false;
            };
        }

        static void StartRosConnection()
        {
#if UNITY_EDITOR
            const string myUri = "http://141.3.59.11:7613";
            const string myId = "/iviz_win_hololens";
#else
            const string myUri = "http://141.3.59.45:7613";
            //const string myUri = "http://192.168.20.5:7613";
            const string myId = "/iviz_hololens";
#endif
            const string masterUri = "http://141.3.59.5:11311";
            //const string masterUri = "http://192.168.20.7:11311";
            ModuleListPanel.Instance.SetConnectionData(masterUri, myUri, myId);
        }

        void StartPalms()
        {
            leftPalm = TfListener.GetOrCreateFrame($"{MyId}/controller/left_palm");
            leftPalm.transform.localScale = 0.05f * Vector3.one;

            leftPalm = TfListener.GetOrCreateFrame($"{MyId}/controller/left_hand");
            node = FrameNode.Instantiate("Hololens");
            node.Parent = leftPalm;

            leftPalmPose = new Pose(Vector3.up * 1f, Quaternion.identity);
            leftPalm.SetPose(leftPalmPose);
            leftPalm.ParentCanChange = false;

            LeftHandScale = 0.1f;
        }

        void StartHandMenu()
        {
            hololensHandMenu.SetSide(new[]
            {
                new HololensMenuEntry("Add Robot", OnHandMenuAddRobot),
                new HololensMenuEntry("Add Topic", OnHandMenuAddTopic),
                new HololensMenuEntry("Remove Module", OnHandMenuRemoveModule),
                new HololensMenuEntry("Extras", OnExtras),
            });
        }

        void OnHandMenuAddRobot()
        {
            void MenuListClick(string robotName)
            {
                var robotConfig = new SimpleRobotConfiguration
                {
                    SavedRobotName = robotName,
                    AttachedToTf = true
                };
                ModuleListPanel.Instance.CreateModule(Resource.ModuleType.Robot, configuration: robotConfig);
            }

            hololensHandMenu.SetPalm(Resource.GetRobotNames().Select(
                robotName => new HololensMenuEntry(robotName, () => MenuListClick(robotName))));
        }

        void OnHandMenuAddTopic()
        {
            void MenuListClick(string topic, string type)
            {
                ModuleData data = ModuleListPanel.Instance.CreateModuleForTopic(topic, type);
                if (data.ModuleType == Resource.ModuleType.PointCloud)
                {
                    PointCloudConfiguration config = new PointCloudConfiguration
                    {
                        IntensityChannel = "rgba",
                        PointSize = 0.01f
                    };
                    data.UpdateConfiguration(JsonConvert.SerializeObject(config),
                        new[] {nameof(config.IntensityChannel), nameof(config.PointSize)});
                }
            }

            hololensHandMenu.SetPalm(AddTopicDialogData.GetTopicCandidates().Select(
                topic => new HololensMenuEntry(topic.Topic, () => MenuListClick(topic.Topic, topic.Type))));
        }

        void OnHandMenuRemoveModule()
        {
            void MenuListClick(ModuleData data)
            {
                ModuleListPanel.Instance.RemoveModule(data);
            }

            hololensHandMenu.SetPalm(
                ModuleDatas
                    .Where(data => data.ModuleType != Resource.ModuleType.TF)
                    .Select(data => new HololensMenuEntry(data.Description, () => MenuListClick(data))));
        }

        void OnExtras()
        {
            HololensMenuEntry[] entries =
            {
                new HololensMenuEntry("Set Fixed Frame", SetFixedFrame),
                new HololensMenuEntry("Set World Scale", SetWorldScale),
                new HololensMenuEntry("Reposition Origin", StartOriginPlaceMode),
                new HololensMenuEntry("Toggle TF Visibility", ToggleTfVisibility),
                //new HololensMenuEntry("Reset All", ResetAll),
                new HololensMenuEntry("Show Canvas", ShowConsoleLog),
                //new HololensMenuEntry("Print Listener Status", PrintListenerStatus),
                //new HololensMenuEntry("Reconnect", Reconnect),
            };
            hololensHandMenu.SetPalm(entries);
        }

        void SetFixedFrame()
        {
            void MenuListClick(string frameId)
            {
                TfListener.FixedFrameId = frameId;
            }

            const int maxLevel = 3;
            List<TfFrame> candidates = new List<TfFrame>();

            void AddFrame(TfFrame frame, int level)
            {
                candidates.Add(frame);
                if (level == maxLevel)
                {
                    return;
                }

                foreach (TfFrame child in frame.Children)
                {
                    AddFrame(child, level + 1);
                }
            }


            AddFrame(TfListener.OriginFrame, 0);
            hololensHandMenu.SetPalm(Enumerable.Select(candidates,
                frame => new HololensMenuEntry(frame.Id, () => MenuListClick(frame.Id))));
        }

        void SetWorldScale()
        {
            float[] scales = {1, 0.5f, 0.25f, 0.1f, 0.05f, 0.025f, 0.01f};

            void MenuListClick(float scale)
            {
                TfListener.RootFrame.transform.localScale = scale * Vector3.one;
            }

            hololensHandMenu.SetPalm(Enumerable.Select(scales,
                scale => new HololensMenuEntry(scale.ToString(BuiltIns.Culture), () => MenuListClick(scale))));
        }

        static void ToggleTfVisibility()
        {
            TfListener.Instance.FramesVisible = !TfListener.Instance.FramesVisible;
        }

        static void ResetAll()
        {
            ModuleListPanel.Instance.ResetAllModules();
        }

        void ShowConsoleLog()
        {
            Vector3 cameraPosition = Settings.MainCamera.transform.position;
            Vector3 cameraForward = Settings.MainCamera.transform.forward;
            Vector3 position = cameraPosition + cameraForward * 2;
            position.y = cameraPosition.y - 0.4f;

            consoleLog.transform.SetPositionAndRotation(position,
                Quaternion.LookRotation(new Vector3(cameraForward.x, 0, cameraForward.z)));
            consoleLog.SetActive(true);
        }

        void PrintListenerStatus()
        {
            StringBuilder str = new StringBuilder();
            var modules = ModuleDatas.OfType<ListenerModuleData>();
            foreach (var data in modules)
            {
                ListenerController controller = (ListenerController) data.Controller;
                var listener = controller.Listener;
                str.Append(listener.Topic).Append(listener.Subscribed ? " (On): " : " (Off): ")
                    .Append(listener.NumPublishers).Append(" publishers").AppendLine();
            }

            Logger.Info(str.ToString());
        }

        void Reconnect()
        {
            ConnectionManager.Connection.Disconnect();
        }

        void StartOriginPlaceMode()
        {
            if (RootFrame.gameObject.TryGetComponent<WorldAnchor>(out var component))
            {
                Destroy(component);
            }

            RootFrame.transform.SetPose(InfinitePose);
            floorFrame.gameObject.SetActive(true);

            CoreServices.SpatialAwarenessSystem.Enable();
        }

        void StartWorld()
        {
            floorFrame.gameObject.SetActive(false);
            RootFrame.Transform.SetPose(floorFrame.OriginPose);

            WriteAnchor(floorFrame.OriginPose);

            CoreServices.SpatialAwarenessSystem.Disable();
        }

        public float LeftHandScale
        {
            get => leftHandScale;
            set
            {
                leftHandScale = value;
                leftPalm.transform.localScale = value * Vector3.one;
            }
        }

        void OnDestroy()
        {
            if (node != null)
            {
                node.Parent = null;
                Destroy(node.gameObject);
            }
        }


        public class MarkerResourcePool
        {
            static readonly Info<GameObject> ControlAsset =
                new Info<GameObject>("Hololens Assets/HololensControl");

            readonly Queue<GameObject> markers = new Queue<GameObject>();
            readonly GameObject root;

            public MarkerResourcePool(GameObject parent)
            {
                root = parent.transform.parent.Find("HololensResourcePool").gameObject;
                root.SetActive(false);
                CreateMarkerPool();
            }

            void CreateMarkerPool()
            {
                for (int i = 0; i < root.transform.childCount; i++)
                {
                    markers.Enqueue(root.transform.GetChild(i).gameObject);
                }
            }

            public GameObject GetOrCreate(Transform parent = null)
            {
                if (markers.Count == 0)
                {
                    Debug.Log("NEW!!");
                    return ControlAsset.Instantiate(parent);
                }

                GameObject obj = markers.Dequeue();
                //obj.SetActive(true);
                obj.transform.SetParentLocal(parent);
                obj.transform.SetLocalPose(Pose.identity);
                return obj;
            }

            public void Dispose(GameObject obj)
            {
                //obj.SetActive(false);
                obj.transform.parent = root.transform;
                obj.transform.SetPose(Pose.identity);
                obj.transform.localScale = Vector3.one;
                markers.Enqueue(obj);
            }
        }

        public QualityType QualityInView
        {
            get => QualityType.VeryLow;
            set => Logger.Debug($"{this}: Ignoring view quality.");
        }

        public QualityType QualityInAr
        {
            get => QualityType.VeryLow;
            set => Logger.Debug($"{this}: Ignoring AR quality.");
        }

        public int NetworkFrameSkip
        {
            get => config.NetworkFrameSkip;
            set
            {
                config.NetworkFrameSkip = value;
                GameThread.NetworkFrameSkip = value;
            }
        }

        public int TargetFps
        {
            get => config.TargetFps;
            set
            {
                config.TargetFps = value;
                UnityEngine.Application.targetFrameRate = value;
            }
        }

        public Color BackgroundColor
        {
            get => Color.black;
            set => Logger.Debug($"{this}: Ignoring background color.");
        }

        public int SunDirection
        {
            get => config.SunDirection;
            set
            {
                config.SunDirection = value;
                if (MainLight != null)
                {
                    MainLight.transform.rotation = Quaternion.Euler(90 + value, 0, 0);
                }
            }
        }

        public SettingsConfiguration Config
        {
            get => config;
            set
            {
                BackgroundColor = value.BackgroundColor;
                SunDirection = value.SunDirection;
                NetworkFrameSkip = value.NetworkFrameSkip;
                QualityInAr = value.QualityInAr;
                QualityInView = value.QualityInView;
                TargetFps = value.TargetFps;
            }
        }

        public bool SupportsView => false;
        public bool SupportsAR => true;

        readonly SettingsConfiguration config = new SettingsConfiguration
        {
            QualityInAr = QualityType.VeryLow,
            QualityInView = QualityType.VeryLow,
        };

        static readonly string[] QualityInViewOptions = { };

        static readonly string[] QualityInArOptions = {"Very Low"};

        public IEnumerable<string> QualityLevelsInView => QualityInViewOptions;
        public IEnumerable<string> QualityLevelsInAR => QualityInArOptions;

        Light mainLight;

        Light MainLight => mainLight != null
            ? mainLight
            : (mainLight = GameObject.Find("MainLight")?.GetComponent<Light>());


        bool WriteAnchor(in Pose newWorldPose)
        {
            if (RootFrame.gameObject.TryGetComponent<WorldAnchor>(out var component))
            {
                Destroy(component);
            }

            ;

            RootFrame.Transform.SetPose(newWorldPose);
            RootFrame.gameObject.AddComponent<WorldAnchor>();
            return true;
        }
    }


#else
    class HololensManager : MonoBehaviour
    {
        public class MarkerResourcePool
        {
            public GameObject GetOrCreate(Transform parent = null)
            {
                throw new NotSupportedException();
            }

            public void Dispose(GameObject obj)
            {
                throw new NotSupportedException();
            }
        }    
        
        public static MarkerResourcePool ResourcePool => throw new NotSupportedException();
    }
#endif
}