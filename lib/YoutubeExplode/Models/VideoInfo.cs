using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YoutubeExplode.Models.ClosedCaptions;
using YoutubeExplode.Models.MediaStreams;

namespace YoutubeExplode.Models
{
    public class VideoInfo
    {
        public Video Video { set; get; }
        public Channel Channel { set; get; }
        public MediaStreamInfoSet Media { set; get; }
        public ClosedCaptionTrackInfo CaptionTrack { set; get; }
    }
}
