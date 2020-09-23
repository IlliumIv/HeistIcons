using ExileCore.Shared.Attributes;
using ExileCore.Shared.Interfaces;
using ExileCore.Shared.Nodes;
using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeistIcons
{
    public class HeistIconsSettings : ISettings
    {
        public HeistIconsSettings ()
        {
            Enable = new ToggleNode(false);
        }

        [Menu("Map Icon Size")]
        public RangeNode<int> MapIconSize { get; set; } = new RangeNode<int>(32, 10, 100);

        [Menu("Draw icon to world")]
        public ToggleNode WorldIcon { get; set; } = new ToggleNode(true);

        [Menu("World Icon Size")]
        public RangeNode<int> WorldIconSize { get; set; } = new RangeNode<int>(110, 10, 220);

        [Menu("Text")]
        public ToggleNode TextEnable { get; set; } = new ToggleNode(false);

        [Menu("Text Color")]
        public ColorNode TextColor { get; set; } = new ColorNode(Color.Black);

        [Menu("Text Background Color")]
        public ColorNode TextBackgroundColor { get; set; } = new ColorNode(Color.White);

        [Menu("Text Border Color")]
        public ColorNode TextBorderColor { get; set; } = new ColorNode(Color.Black);

        public ToggleNode Enable { get; set; }
    }
}
