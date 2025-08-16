using System.Diagnostics;
using System.Xml.Serialization;

namespace AD.Story
{
    [System.Serializable]
    public class StoryBackgroundCG : StoryEntry
    {
        /****** Public Members ******/

        public enum AnimationType
        {
            Appear,
            Disappear,
            Move,
            Shake,
            ZoomIn,
            ZoomOut,
            AnimtationTypeCount
        }

        public enum TargetPositionType
        {
            Center = 0,
            Left,
            Right,
            TargetPositionTypeCount
        }

        public ChapterType          Chapter             => _chapter;
        public string               ImageName           => _imageName;
        public AnimationType        Animation           => _animation;
        public float                AnimationDuration   => _animationDuration;
        public TargetPositionType   TargetPosition      => _targetPosition;

        public override bool        IsSavePoint => false;
        public override EntryType   Type        => EntryType.BackgroundCG;

        public StoryBackgroundCG()
        {
            
        }

        public StoryBackgroundCG(ChapterType chapter, string imageName, AnimationType animation = AnimationType.Appear, float animationDuration = 1.0f, TargetPositionType targetPosition = TargetPositionType.Center)
        {
            Debug.Assert(ChapterType.ChapterTypeCount != chapter, "ChapterType must be set to a valid chapter.");
            Debug.Assert(false == string.IsNullOrEmpty(imageName), "ImageName must not be empty or null.");
            Debug.Assert(AnimationType.AnimtationTypeCount != animation, "AnimationType must be set to a valid animation type.");
            Debug.Assert(0 < animationDuration, "AnimationDuration must be greater than zero.");

            _chapter            = chapter;
            _imageName          = imageName;
            _animation          = animation;
            _animationDuration  = animationDuration;
            _targetPosition     = targetPosition;
        }


        /****** Private Members ******/

        [XmlAttribute("Chapter")]           private     ChapterType         _chapter            = ChapterType.ChapterTypeCount;
        [XmlAttribute("ImageName")]         private     string              _imageName          = "";
        [XmlAttribute("Animation")]         private     AnimationType       _animation          = AnimationType.AnimtationTypeCount;
        [XmlAttribute("AnimationDuration")] private     float               _animationDuration  = 1.0f;
        [XmlAttribute("TargetPosition")]    private     TargetPositionType  _targetPosition     = TargetPositionType.TargetPositionTypeCount;
    }
}