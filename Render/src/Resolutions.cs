using System.Drawing;

namespace Qkmaxware.Rendering {
    
/// <summary>
/// Common resolution sizes
/// </summary>
public static class Resolutions {
    /// <summary>
    /// Resolutions for application icons
    /// </summary>
    public static class Icon {
        /// <summary>
        /// 16x16 Resolution
        /// </summary>
        public static readonly Size Web = new Size(16, 16);
        /// <summary>
        /// 32x32 Resolution
        /// </summary>
        public static readonly Size Taskbar = new Size(32, 32);
        /// <summary>
        /// 96x96 Resolution
        /// </summary>
        public static readonly Size Desktop = new Size(96, 96);
    }
    /// <summary>
    /// Resolutions common on Instagram
    /// </summary>
    public static class Instagram {
        /// <summary>
        /// 110x110 Resolution
        /// </summary>
        public static readonly Size ProfilePhoto = new Size(110, 110);
    }
    /// <summary>
    /// Resolutions common on Facebook
    /// </summary>
    public static class Facebook {
        /// <summary>
        /// 160x160 Resolution
        /// </summary>
        public static readonly Size ProfilePhoto = new Size(160, 160);
        /// <summary>
        /// 1640x624 Resolution
        /// </summary>
        public static readonly Size CoverPhoto = new Size(1640, 624);
    }
    /// <summary>
    /// Resolutions common on Youtube
    /// </summary>
    public static class YouTube {
        /// <summary>
        /// 800x800 Resolution
        /// </summary>
        public static readonly Size ProfilePhoto = new Size(800, 800);
        /// <summary>
        /// 1280x720 Resolution
        /// </summary>
        public static readonly Size ThumbnailPhoto = new Size(1280, 720);
        /// <summary>
        /// 2560x1440 Resolution
        /// </summary>
        public static readonly Size CoverPhoto = new Size(2560, 1440);
    }
    /// <summary>
    /// Resolutions common on Twitter
    /// </summary>
    public static class Twitter {
        /// <summary>
        /// 400x400 Resolution
        /// </summary>
        public static readonly Size ProfilePhoto = new Size(400, 400);
        /// <summary>
        /// 1500x1500 Resolution
        /// </summary>
        public static readonly Size CoverPhoto = new Size(1500, 1500);
    }
    /// <summary>
    /// Resolutions common on LinkedIn
    /// </summary>
    public static class LinkedIn {
        /// <summary>
        /// 400x400 Resolution
        /// </summary>
        public static readonly Size ProfilePhoto = new Size(400, 400);
        /// <summary>
        /// 646x220 Resolution
        /// </summary>
        public static readonly Size CoverPhoto = new Size(646, 220);
    }
    /// <summary>
    /// Resolutions common on Pinterest
    /// </summary>
    public static class Pinterest {
        /// <summary>
        /// 165x165 Resolution
        /// </summary>
        public static readonly Size ProfilePhoto = new Size(165, 165);
    }
    /// <summary>
    /// Resolutions with a 4 x 3 aspect ratio
    /// </summary>
    public static class Aspect4x3 {
        /// <summary>
        /// 640x480 Resolution
        /// </summary>
        public static readonly Size Display480p = new Size(640,480);
        /// <summary>
        /// 800x600 Resolution
        /// </summary>
        public static readonly Size Display600p = new Size(800,600);
        /// <summary>
        /// 960x720 Resolution
        /// </summary>
        public static readonly Size Display720p = new Size(960,720);
        /// <summary>
        /// 1024x768 Resolution
        /// </summary>
        public static readonly Size Display768p = new Size(1024,768);
        /// <summary>
        /// 1280x960 Resolution
        /// </summary>
        public static readonly Size Display960p = new Size(1280,960);
        /// <summary>
        /// 1400x1050 Resolution
        /// </summary>
        public static readonly Size Display1050p = new Size(1400,1050);
        /// <summary>
        /// 1400x1080 Resolution
        /// </summary>
        public static readonly Size Display1080p = new Size(1440,1080); 
        /// <summary>
        /// 1600x1200 Resolution
        /// </summary>
        public static readonly Size Display1200p = new Size(1600,1200);
        /// <summary>
        /// 1856x1392 Resolution
        /// </summary>
        public static readonly Size Display1392p = new Size(1856,1392);
        /// <summary>
        /// 1920x1440 Resolution
        /// </summary>
        public static readonly Size Display1440p = new Size(1920,1440);
        /// <summary>
        /// 2048x1536 Resolution
        /// </summary>
        public static readonly Size Display1536p = new Size(2048,1536);
    }
    /// <summary>
    /// Resolutions with a 16 x 10 aspect ratio
    /// </summary>
    public static class Aspect16x10 {
        /// <summary>
        /// 1280x800 Resolution
        /// </summary>
        public static readonly Size Display800p = new Size(1280,800);
        /// <summary>
        /// 1440x900 Resolution
        /// </summary>
        public static readonly Size Display900p = new Size(1440,900);
        /// <summary>
        /// 1680x1050 Resolution
        /// </summary>
        public static readonly Size Display1050p = new Size(1680,1050);
        /// <summary>
        /// 1920x1200 Resolution
        /// </summary>
        public static readonly Size Display1200p = new Size(1920,1200);
        /// <summary>
        /// 2560x1600 Resolution
        /// </summary>
        public static readonly Size Display1600p = new Size(2560,1600);
    }
    /// <summary>
    /// Resolutions with a 16 x 9 aspect ratio
    /// </summary>
    public static class Aspect16x9 {
        /// <summary>
        /// 1024x576 Resolution
        /// </summary>
        public static readonly Size Display576p = new Size(1024,576);
        /// <summary>
        /// 1152x648 Resolution
        /// </summary>
        public static readonly Size Display648p = new Size(1152,648);
        /// <summary>
        /// 1280x720 Resolution
        /// </summary>
        public static readonly Size Display720p = new Size(1280,720);
        /// <summary>
        /// 1366x768 Resolution
        /// </summary>
        public static readonly Size Display768p = new Size(1366,768);
        /// <summary>
        /// 1600x900 Resolution
        /// </summary>
        public static readonly Size Display900p = new Size(1600,900);
        /// <summary>
        /// 1920x1080 Resolution
        /// </summary>
        public static readonly Size Display1080p = new Size(1920,1080);
        /// <summary>
        /// 2560x1440 Resolution
        /// </summary>
        public static readonly Size Display1440p = new Size(2560,1440);
        /// <summary>
        /// 3840x2160 Resolution
        /// </summary>
        public static readonly Size Display2160p = new Size(3840,2160);
    }
}

}