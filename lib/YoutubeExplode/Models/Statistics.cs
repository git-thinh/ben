using ProtoBuf;
using YoutubeExplode.Internal;

namespace YoutubeExplode.Models
{
    /// <summary>
    /// User activity statistics.
    /// </summary>
    [ProtoContract]
    public class Statistics
    {
        /// <summary>
        /// View count.
        /// </summary>
        [ProtoMember(1)]
        public long ViewCount { get; set; }

        /// <summary>
        /// Like count.
        /// </summary>
        [ProtoMember(2)]
        public long LikeCount { get; set; }

        /// <summary>
        /// Dislike count.
        /// </summary>
        [ProtoMember(3)]
        public long DislikeCount { get; set; }

        /// <summary>
        /// Average user rating in stars (1 star to 5 stars).
        /// </summary>
        [ProtoMember(4)]
        public double AverageRating
        {
            get
            {
                if (LikeCount + DislikeCount == 0) return 0;
                return 1 + 4.0 * LikeCount / (LikeCount + DislikeCount);
            }
            set { }
        }

        public Statistics() { }

        /// <summary />
        public Statistics(long viewCount, long likeCount, long dislikeCount)
        {
            ViewCount = viewCount.GuardNotNegative(nameof(viewCount));
            LikeCount = likeCount.GuardNotNegative(nameof(likeCount));
            DislikeCount = dislikeCount.GuardNotNegative(nameof(dislikeCount));
        }
    }
}