using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEditor.Experimental.GraphView;
using SkillEditor.Data;
using Animancer;

namespace SkillEditor.Editor
{
    public class AnimancerNode : SkillNodeBase<AnimancerNodeData>
    {
        protected override string GetNodeTitle() => "Animancer动画";
        protected override float GetNodeWidth() => 1020;

        protected override void CreateContent() {
            CreateAnimationConfigSection();
            CreateTimelineSection();
        }

        private void CreateAnimationConfigSection() {
            var container = new VisualElement {
                style =
                {
                    backgroundColor = new Color(56f / 255f, 56f / 255f, 56f / 255f),
                    borderTopLeftRadius = 8,
                    borderTopRightRadius = 8,
                    borderBottomLeftRadius = 8,
                    borderBottomRightRadius = 8,
                    paddingLeft = 8,
                    paddingRight = 8,
                    paddingTop = 6,
                    paddingBottom = 6,
                    marginTop = 8
                }
            };

            // === 第一行：Animancer资源拖拽 ===
            var row1 = new VisualElement {
                style = { flexDirection = FlexDirection.Row, alignItems = Align.Center, marginBottom = 4 }
            };

            var objField = new ObjectField("Animancer资源") {
                objectType = typeof(Animancer.AnimancerTransitionAssetBase),
                value = TypedData?.Data
            };
            objField.style.flexGrow = 0;
            objField.style.width = 300;
            objField.labelElement.style.minWidth = 60;

            row1.Add(objField);

            container.Add(row1);


            mainContainer.Add(container);
        }

        private void CreateTimelineSection() {
            // 时间轴整个区域
            _timelineContainer = new VisualElement {
                name = "TimelineSection",
                style =
                {
                    backgroundColor = new Color(56f / 255f, 56f / 255f, 56f / 255f),
                    borderTopLeftRadius = 8,
                    borderTopRightRadius = 8,
                    borderBottomLeftRadius = 8,
                    borderBottomRightRadius = 8,
                    paddingLeft = 8,
                    paddingRight = 8,
                    paddingTop = 8,
                    paddingBottom = 8,
                    marginTop = 8,
                    minWidth = 1004
                }
            };

            // 创建Timeline视图
            _timelineView = new TimelineView();
            _timelineView.style.display = _timelineSectionFolded ? DisplayStyle.None : DisplayStyle.Flex;
            _timelineView.OnDataChanged += NotifyDataChanged;
            _timelineView.OnAddButtonClicked += OnTimelineAddClicked;
            _timelineContainer.Add(_timelineView);

            mainContainer.Add(_timelineContainer);

            // 初始化Timeline
            RefreshTimeline();
        }

        private void ToggleTimelineSection() {
            _timelineSectionFolded = !_timelineSectionFolded;
            if (_timelineView != null)
                _timelineView.style.display = _timelineSectionFolded ? DisplayStyle.None : DisplayStyle.Flex;
        }

        private void OnTimelineAddClicked() {
            var menu = new GenericMenu();
            menu.AddItem(new GUIContent("时间效果"), false, () =>
            {
                if (TypedData == null) return;
                if (TypedData.timeEffects == null)
                    TypedData.timeEffects = new List<TimeEffectData>();

                TypedData.timeEffects.Add(new TimeEffectData());

                if (_timelineSectionFolded)
                    ToggleTimelineSection();

                _timelineView?.AddNewTrack(false);
                RefreshPorts();
                NotifyDataChanged();
            });
            menu.AddItem(new GUIContent("时间Cue"), false, () =>
            {
                if (TypedData == null) return;
                if (TypedData.timeCues == null)
                    TypedData.timeCues = new List<TimeCueData>();

                TypedData.timeCues.Add(new TimeCueData());

                if (_timelineSectionFolded)
                    ToggleTimelineSection();

                _timelineView?.AddNewTrack(true);
                RefreshPorts();
                NotifyDataChanged();
            });
            menu.ShowAsContext();
        }

        private void RefreshTimeline() {
            if (_timelineView == null || TypedData == null) return;

            _timelineView.Initialize(TypedData, () =>
            {
                var port = TimelinePort.Create<Edge>(Orientation.Horizontal, Direction.Output, Port.Capacity.Multi, typeof(float));
                return port;
            });

            RefreshPorts();

            // 绑定播放指示器事件
            BindPlaybackIndicator();
        }

        /// <summary>
        /// 绑定播放指示器事件
        /// </summary>
        private void BindPlaybackIndicator() {
            if (_timelineView == null) return;

            var indicator = _timelineView.GetPlaybackIndicator();
            if (indicator != null) {
                indicator.OnSeekToFrame -= OnPlaybackSeek;
                indicator.OnSeekToFrame += OnPlaybackSeek;
            }
        }

        /// <summary>
        /// 播放指示器拖拽跳转回调
        /// </summary>
        private void OnPlaybackSeek(int frame) {
            /*
            if (_spineRenderer == null || !_spineRenderer.IsInitialized) return;
            _spineRenderer.SeekToFrame(frame);
            RepaintPreview();
            */
        }

        /// <summary>
        /// 根据端口标识符查找输出端口（支持普通端口和Timeline端口）
        /// </summary>
        public override Port FindOutputPortByIdentifier(string portIdentifier) {
            if (_timelineView != null) {
                var port = _timelineView.FindPortByIdentifier(portIdentifier);
                if (port != null) return port;
            }

            return base.FindOutputPortByIdentifier(portIdentifier);
        }

        // Timeline视图
        private TimelineView _timelineView;
        private VisualElement _timelineContainer;
        private bool _timelineSectionFolded = false;

        public AnimancerNode(Vector2 position) : base(NodeType.Animancer, position) { }
    }
}
