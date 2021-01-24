using ExileCore.Shared.Attributes;
using ExileCore.Shared.Interfaces;
using ExileCore.Shared.Nodes;
using SharpDX;

namespace HeistIcons.Main
{
    public class Settings : ISettings
    {
        public Settings ()
        {
            Enable = new ToggleNode(false);
        }

        [Menu("Map icon size")]
        public RangeNode<int> MapIconSize { get; set; } = new RangeNode<int>(32, 10, 100);

        [Menu("Draw icon to world")]
        public ToggleNode WorldIcon { get; set; } = new ToggleNode(true);

        [Menu("World icon size")]
        public RangeNode<int> WorldIconSize { get; set; } = new RangeNode<int>(120, 10, 220);

        [Menu("Text")]
        public ToggleNode TextEnable { get; set; } = new ToggleNode(true);

        [Menu("Use default font")]
        public ToggleNode UseDefaultText { get; set; } = new ToggleNode(false);

        [Menu("Text background width multiplier")]
        public RangeNode<float> BackgroundWidth { get; set; } = new RangeNode<float>(1.5f, 0, 2);

        [Menu("Text background height multiplier")]
        public RangeNode<float> BackgroundHeight { get; set; } = new RangeNode<float>(1.4f, 0, 2);

        [Menu("Text color")]
        public ColorNode TextColor { get; set; } = new ColorNode(Color.White);

        [Menu("Text background color")]
        public ColorNode TextBackgroundColor { get; set; } = new ColorNode(Color.DarkGreen);

        [Menu("Text border color")]
        public ColorNode TextBorderColor { get; set; } = new ColorNode(Color.Black);

        [Menu("Enable multithreading")]
        public ToggleNode MultiThreading { get; set; } = new ToggleNode(true);

        public ToggleNode Enable { get; set; }
    }
}
