using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace REEL.PoseAnimation
{
    public class RobotTransformController : Singleton<RobotTransformController>
    {

        public MoccaFaceAnniTest face;
        public JointSet[] jointInfo;
        public bool breath = false;
        public int cnt = 0;
        public string currentGesture;

        string _gesture;
        float duration = -1;

        bool _breathe_enable = false;

        private float rotSpeed = 360f;
        private float rotSpeedPercentage = 0.8f;

        // Y,    Z,      Z,   Y,    X, Z
        float[] zeroAngle = new float[8] { 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f };
        float[] baseAngle = new float[8] { 45f, -45f, -45f, -45f, 45f, 45f, 0f, 0f };
        float[] DIRECTION = new float[8] { 1f, 1f, -1f, -1f, -1f, -1f, -1f, -1f };

        float[] OFFSET = new float[] { -90f, 0f, 0f, 90f, 0f, 0f, 0f, 0f, 0f };
        //     float[][] hiList = {
        //            // Time,   Left Arm,          Right Arm,         Head
        //new float[9] {  1.5f,        45, -45, -45,       -60, 60, 38,         0, 20      },
        //         new float[9] {  0.7f,        45, -45, -45,       -60, 80, 34,         0, 20     },
        //         new float[9] {  0.7f,        45, -45, -45,       -60, 60, 38,         0, 20     },
        //         new float[9] {  0.7f,        45, -45, -45,       -60, 80, 34,         0, 20     },
        //         new float[9] {  1.5f,        45, -45, -45,       45, 45, 45,        0, 20    }
        //     };
        public float[][] baseList = { new float[9] { 0.8f, 45f, -45f, -45f, -45f, 45f, 45f, 0f, 0f }, };

        float[][] helloList = {
                     // Time,   Left Arm,          Right Arm,          Head
            new float[9] {  1.5f,       -60f, -60f, -38f,   60f, 60f, 38f,      0f, 20f   },
            new float[9] {  0.7f,       -40f, -80f, -34f,   40f, 80f, 34f,      0f, 20f   },
            new float[9] {  0.7f,       -40f, -60f, -38f,   40f, 60f, 38f,      0f, 20f   },
            new float[9] {  0.7f,       -40f, -80f, -34f,   40f, 80f, 34f,      0f, 20f   },

        };
        float[][] angryList = {
                     // Time,   Left Arm,           Right Arm,         Head
             new float[9] {  0.8f,        -20, -70, -24,      20, 70, 24,         0, -2      },
            new float[9] {  0.8f,        40, -60, -24,       -40, 60, 24,        0, -2      },
            new float[9] {  0.8f,        -20, -70, -24,      20, 70, 24,         0, -2      },

        };
        float[][] sadList = {
                         // Time,    Left Arm,           Right Arm,          Head
            new float[9] {  1.2f,       -24, -70, -55,      42, 70, 55,         0, 5   },
            new float[9] {  0.5f,       -42, -70, -55,      24, 70, 55,         0, 5   },
            new float[9] {  0.5f,       -24, -70, -55,      42, 70, 55,         0, 5   },
            new float[9] {  0.5f,       -42, -70, -55,      24, 70, 55,         0, 5   },

        };

        float[][] okList = {
                         // Time,    Left Arm,           Right Arm,          Head
            new float[9] {  0.6f,       45, -45, -45,       -45, 45, 45,        0, -10    },
            new float[9] {  0.6f,       45, -45, -45,       -45, 45, 45,        0, 20     },
            new float[9] {  0.6f,       45, -45, -45,       -45, 45, 45,        0,  -10   },
            new float[9] {  0.6f,       45, -45, -45,       -45, 45, 45,        0, 20     },
            new float[9] {  0.6f,       45, -45, -45,       -45, 45, 45,        0, -10    },
            new float[9] {  0.6f,       45, -45, -45,       -45, 45, 45,        0, 20     },

        };
        float[][] noList = {
                         // Time,    Left Arm,           Right Arm,          Head
            new float[9] {  0.7f,       45, -45, -45,       -45, 45, 45,        -20, 10 },
            new float[9] {  0.7f,       45, -45, -45,       -45, 45, 45,         20, 10 },
            new float[9] {  0.7f,       45, -45, -45,       -45, 45, 45,        -20, 10 },
            new float[9] {  0.7f,       45, -45, -45,       -45, 45, 45,         20, 10 },

        };

        float[][] wrongList =
        {
            new float[9] {  1.1f,       -20, -45, -45,    -50, 35, 40,         0, 10  },
            new float[9] {  0.9f,        50, -35, -40,     20, 45, 45,         0, 10  },
            new float[9] {  0.9f,       -20, -45, -45,    -50, 35, 40,         0, 10  },
            new float[9] {  0.9f,        50, -35, -40,     20, 45, 45,         0, 10  },

        };

        float[][] happyList = {
                            // Time,    Left Arm,           Right Arm,          Head
            new float[9] {  1.6f,       -25, -75, -45,      25, 75, 45,          0, -10 },
            new float[9] {  0.8f,       -25, -75, -45,      25, 75, 45,         -14, -2 },
            new float[9] {  0.8f,       -25, -75, -45,      25, 75, 45,          0, -10 },
            new float[9] {  0.8f,       -25, -75, -45,      25, 75, 45,         -14, -2 },

        };
        float[][] refuseList = {
                            // Time,    Left Arm,           Right Arm,          Head
            new float[9] {  0.9f,       55, -38, -15,       -55, 38, 15,         25, -13   },
            new float[9] {  0.9f,       50, -35, -40,       -50, 35, 40,        -25, -13   },

        };
        float[][] smileList = new float[4][] {
                            // Time,    Left Arm,           Right Arm,          Head
            new float[9] {  1.5f,       -31,-85,-25,        31,85,25,           0,-7   },
            new float[9] {  0.5f,       -28,-89,-30,       28,89,30,          0,-9   },
            new float[9] {  0.5f,       -31,-85,-25,        31,85,25,           0,-7   },
            new float[9] {  0.5f,       -28,-89,-30,       28,89,30,          0,-9   },

        };
        float[][] takenabackList = new float[2][] {
                            // Time,    Left Arm,           Right Arm,          Head
            new float[9] {  0.7f,        32, -28, -5,       -32, 23, 5,         0,-18   },
            new float[9] {  1.2f,        32, -28, -5,       -32, 23, 5,         0,-18   },

        };
        float[][] tellList = new float[4][] {
                            // Time,    Left Arm,           Right Arm,          Head
            new float[9] {  0.9f,       4,-58,-50,          -4,58,50 ,           0,-12      },
            new float[9] {  0.5f,       4,-60,-60,          -4,60,60,            0,-12      },
            new float[9] {  0.5f,       4,-58,-50,          -4,58,50,            0,-12      },
            new float[9] {  0.5f,       4,-60,-60,          -4,60,60,            0,-12      },

        };
        float[][] sleepyList = new float[4][] {
                            // Time,    Left Arm,           Right Arm,          Head
            new float[9] {  0.4f,       40,-55,-15,         -40,55,15,           0,-18      },
            new float[9] {  1.4f,       40,-55,-15,         -40,55, 15,           0,18       },
            new float[9] {  0.4f,       40,-55,-15,         -40,55, 15,           0,-18      },
            new float[9] {  1.4f,       40,-55,-15,         -40,55, 15,           0,18      },

        };
        float[][] thinkingList = new float[5][] {
                            // Time,    Left Arm,           Right Arm,          Head
            new float[9] {  1.5f,       40.5f,-60,-16,         30,75,50,           -17,-13     },
            new float[9] {  0.8f,       40.5f, -60,-16,         30,75, 50,          -29,8       },
            new float[9] {  0.8f,       40.5f, -60,-16,         30,75,50 ,           1,17       },
            new float[9] {  0.8f,       40.5f, -60,-16,         30,75,50   ,        -10,-16     },
            new float[9] {  0.8f,       40.5f, -60,-16,         15,75,50   ,         1,-2       },

        };
        float[][] excellentList = new float[4][] {
                            // Time,    Left Arm,           Right Arm,          Head
            new float[9] {  1.2f,       -68, -29, -17,        68, 29, 17,      0, 5 },
            new float[9] {  0.9f,       -55, -65, -27,        55, 65, 27,      0, 5 },
            new float[9] {  0.9f,       -68, -29, -17,        68, 29, 17,      0, 5 },
            new float[9] {  0.9f,       -55, -65, -27,        55, 65, 27,      0, 5 },

        };
        float[][] shakeList = {
                            // Time,    Left Arm,           Right Arm,          Head
            new float[9] {  0.8f,       45,-45,-45,        10,45,45,            0,10  },
            new float[9] {  0.5f,       45,-45,-45,        -30,45,40,           0,10  },
            new float[9] {  0.5f,       45,-45,-45,        10,45,45,            0,10  },
            new float[9] {  0.5f,       45,-45,-45,        -30,45,40,           0,10   },

        };
        float[][] stretchList = {
                            // Time,    Left Arm,           Right Arm,          Head
            new float[9] {  2.2f,       0, 0, 0,            0, 0, 0,            0, -13  },

        };
        float[][] hugList = new float[2][] {
                            // Time,    Left Arm,           Right Arm,          Head
            new float[9] {  0.5f,       20, -65, -40,         -20, 65, 40 ,     0,20},
            new float[9] {  1.1f,       20, -65, -40,         -20, 65, 40 ,     0,20},

        };
        float[][] fearList = new float[4][] {
                            // Time,    Left Arm,           Right Arm,          Head
            new float[9] {  0.8f,       10, -70, -50,      -10, 70, 50 ,        0, 0 },
            new float[9] {  0.6f,       10 , -70, -70,      -10, 70, 70 ,        0, 0 },
            new float[9] {  0.8f,       10, -70, -50,      -10, 70, 50 ,        0, 0 },
            new float[9] {  0.6f,       10 , -70, -70,      -10, 70, 70 ,        0, 0 },

        };
        float[][] bowList = {
                            // Time,    Left Arm,           Right Arm,          Head
            new float[9] {  0.5f,       65, -25, -55,       -65, 25, 55,         0, -19 },

        };
        float[][] attentionList = {
                            // Time,    Left Arm,           Right Arm,          Head
         new float[9] {  0.5f,       50, -45, -15,       -50, 45, 15,         0, 5 },

        };
        float[][] bestList = {
                            // Time,    Left Arm,           Right Arm,          Head
            new float[9] {  0.9f,       12,-65,-57,         12,72,57,           0,10  },
            new float[9] {  0.5f,       -12,-72,-57,        -12,65,57,          0,10},
            new float[9] {  0.5f,       12,-65,-57,         12,72,57,           0,20   },
            new float[9] {  0.5f,       -12,-72,-57,        -12,65,57,          0,10},

        };
        float[][] leftNeckList = {
                            // Time,    Left Arm,           Right Arm,          Head
            new float[9] {  1.2f,       45, -45, -45,       -45, 45, 45 ,       40, 40 },

        };
        float[][] rightNeckList = {
                            // Time,    Left Arm,           Right Arm,          Head
            new float[9] {  1.2f,       45, -45, -45,       -45, 45, 45 ,       -40, 40 },

        };
        float[][] forwardList = {
                            // Time,    Left Arm,           Right Arm,          Head
            new float[9] {  1f,         45, -45, -45,       -45, 45, 45,        0, 10 },

        };
        //float[][] coachList = {
        //                    // Time,    Left Arm,           Right Arm,          Head
        //    new float[9] {  0.5f,       45, -45, -45,       -45, 45, 45,        0, 10 },
        //    new float[9] {  1f,         45, -45, -45,       -45, 45, 45 ,       0, 20 }
        //};
        //float[][] requireList = {
        //                    // Time,    Left Arm,           Right Arm,          Head
        //    new float[9] {  1f,         45, -45, -45,       -45, 45, 45,        0, 10 },
        //    new float[9] {  1f,         45, -45, -45,       -45, 45, 45 ,       0, 20 }
        //};
        //float[][] resetList = {
        //                    // Time,    Left Arm,           Right Arm,          Head
        //    new float[9] {  0.5f,       45, -45, -45,       -45, 45, 45,        0, 10 },
        //    new float[9] {  1f,         45, -45, -45,       -45, 45, 45 ,       0, 20 }
        //};
        //float[][] exchangedList = {
        //                    // Time,    Left Arm,           Right Arm,          Head
        //    new float[9] {  1f,         45, -45, -45,       -45, 45, 45,        0, 10 },
        //    new float[9] {  1f,         45, -45, -45,       -45, 45, 45 ,       0, 20 }
        //};
        float[][] headTiltList = {
                            // Time,    Left Arm,           Right Arm,          Head
            new float[9] {  0.8f,       45, -45, -45,       -45, 45, 45,        31, -12 },

        };
        float[][] headBackList = {
                            // Time,    Left Arm,           Right Arm,          Head
            new float[9] {  1.2f,       45, -45, -45,       -45, 45, 45,        19, 71 },

        };
        float[][] headUpList = {
                            // Time,    Left Arm,           Right Arm,          Head
            new float[9] {  0.8f,       45, -45, -45,       -45, 45, 45,        0, 30 },

        };
        float[][] headRightList = {
                            // Time,    Left Arm,           Right Arm,          Head
         new float[9] {  0.8f,       45, -45, -45,       -45, 45, 45, -26, 7 },

        };
        float[][] headLeftList = {
                            // Time,    Left Arm,           Right Arm,          Head
            new float[9] {  0.8f,       45,-45,-45,         -45,45,45,          26,7 },

        };
        float[][] headDownList = {
                            // Time,    Left Arm,           Right Arm,          Head
            new float[9] {  0.8f,       45, -45, -45,       -45, 45, 45,        0, -7 },

        };
        float[][] headList = new float[2][] {
                            // Time,    Left Arm,           Right Arm,          Head
            new float[9] {  0.8f,       45,-45,-45,         -45,45,45,          0,-18  },
            new float[9] {  1.6f,       45,-45,-45,         -45,45,45,          0,18   },

        };

        float[][] armsUpList = {
                            // Time,    Left Arm,           Right Arm,          Head
            new float[9] {  0.8f,       -40, -45, -45,      40, 45, 45,         0, 10 },

        };
        float[][] armsUpRightList = {
                            // Time,    Left Arm,           Right Arm,          Head
         new float[9] {  0.8f,       45, -45, -45,       40, 45, 45,         0, 10 },

        };
        float[][] armsUpLeftList = {
                            // Time,    Left Arm,           Right Arm,          Head
         new float[9] {  0.8f,       -40, -45, -45,      -45, 45, 45,        0, 10 },

        };
        float[][] armsDownList = {
                            // Time,    Left Arm,           Right Arm,          Head
         new float[9] {  0.8f,       45, -45, -50,       -45, 45, 50,         0, 10 },

        };
        float[][] armsDownRightList = {
                            // Time,    Left Arm,           Right Arm,          Head
         new float[9] {  0.8f,       45, -45, -45,      -45, 45, 50,         0, 10 },

        };
        float[][] armsDownLeftList = {
                            // Time,    Left Arm,           Right Arm,          Head
         new float[9] {  0.8f,      45, -45, -50,       -45, 45, 45,         0, 10 },

        };
        float[][] armsUpdownList = new float[4][] {
                            // Time,    Left Arm,           Right Arm,          Head
            new float[9] {  0.8f,       -40,-45,-45,        40,45,45,           0,10  },
            new float[9] {  0.8f,       45, -45, -50,       -45, 45, 50,           0,10  },
            new float[9] {  0.8f,       -40,-45,-45,        40,45,45,           0,10  },
            new float[9] {  0.8f,       45, -45, -50,       -45, 45, 50,           0,10  },

        };
        float[][] armsUpdownRightList = new float[4][] {
                            // Time,    Left Arm,           Right Arm,          Head
            new float[9] {  0.8f,       45,-45,-45,         40,45,45,          0,10  },
            new float[9] {  0.8f,       45,-45,-45,         -45, 45, 50,           0,10  },
            new float[9] {  0.8f,       45,-45,-45,         40,45,45,          0,10  },
            new float[9] {  0.8f,       45,-45,-45,         -45, 45, 50,           0,10  },

        };
        float[][] armsUpdownLeftList = new float[4][] {
                            // Time,    Left Arm,           Right Arm,          Head
            new float[9] {  0.8f,       -40,-45,-45,        -45, 45, 45,          0,10  },
            new float[9] {  0.8f,       45, -45, -50,         -45, 45, 45,           0,10  },
            new float[9] {  0.8f,       -40,-45,-45,        -45, 45, 45,          0,10  },
            new float[9] {  0.8f,       45, -45, -50,         -45, 45, 45,           0,10  },

        };
        float[][] armsInList = {
            new float[9] {  0.8f,       10.5f, -60.3f, -60.4f,      -10.5f, 60.3f, 60.4f,         0, 9}
        };

        float[][] armsInRightList = {
                            // Time,    Left Arm,           Right Arm,          Head
         new float[9] {  0.8f,       40.5f, -40.5f, -40.5f,     -10.5f, 60.3f, 60.4f,         0, 9},

        };

        float[][] armsInLeftList = {
                            // Time,    Left Arm,           Right Arm,          Head
         new float[9] {  0.8f,      10.5f, -60.3f, -60.4f,      -40.5f, 40.5f, 40.5f,         0, 9},

        };
        float[][] spreadoutList = new float[4][] {
                            // Time,    Left Arm,           Right Arm,          Head
            new float[9] {  0.7f,       45,-41,-89,         -45,41,89,          0,10  },
            new float[9] {  0.7f,       45,-41,-89,         -45,41,89,          0,10  },
            new float[9] {  0.7f,       45,-41,-89,         -45,41,89,          0,10  },
            new float[9] {  0.7f,       45,-41,-89,         -45,41,89,          0,10   }
        };
        float[][] spreadoutRightList = new float[4][] {
                            // Time,    Left Arm,           Right Arm,          Head
            new float[9] {  0.7f,       45,-45,-45,         -45,41,89,          0,10  },
            new float[9] {  0.7f,       45,-45,-45,         -45,41,89,          0,10  },
            new float[9] {  0.7f,       45,-45,-45,         -45,41,89,          0,10  },
            new float[9] {  0.7f,       45,-45,-45,         -45,41,89,          0,10   }
        };
        float[][] spreadoutLeftList = new float[4][] {
                            // Time,    Left Arm,           Right Arm,          Head
            new float[9] {  0.7f,       45,-41,-89,         -45,45,45,          0,10  },
            new float[9] {  0.7f,       45,-41,-89,         -45,45,45,          0,10  },
            new float[9] {  0.7f,       45,-41,-89,         -45,45,45,          0,10  },
            new float[9] {  0.7f,       45,-41,-89,         -45,45,45,          0,10   }
        };
        float[][] armsForwardList = {
                            // Time,    Left Arm,           Right Arm,          Head
            new float[9] {  1.0f,       0, -90, 0,          0, 90, 0,           0, 10},

        };
        float[][] armsForwardRightList = {
                            // Time,    Left Arm,           Right Arm,          Head
         new float[9] {  1.0f,       45, -45, -45,       0, 90, 0,           0, 10},

        };
        float[][] armsForwardLeftList = {
                            // Time,    Left Arm,           Right Arm,          Head
         new float[9] {  1.0f,       0, -90, 0,          -45, 45, 45,           0, 10},

        };
        float[][] armsFrontList = {
                            // Time,    Left Arm,           Right Arm,          Head
            new float[9] {  0.8f,       -40, -45, -45,      40, 45, 45,        0, 10},

        };
        float[][] armsFrontRightList = {
                            // Time,    Left Arm,           Right Arm,          Head
         new float[9] {  0.8f,       45, -45, -45,       40, 45, 45,        0, 10},

        };
        float[][] armsFrontLeftList = {
                            // Time,    Left Arm,           Right Arm,          Head
            new float[9] {  0.8f,       -40, -45, -45,      -45, 45, 45,        0, 10},

        };
        float[][] armsSwingList = {
                            // Time,    Left Arm,           Right Arm,          Head
            new float[9] {  1.0f,        0, -79.1f, -37.38f,        0, 46.8f, 14.4f,          0, 9},
            new float[9] {  1.0f,        0, -46.8f, -14.4f,         0, 79.1f, 37.38f,       0, 9},
            new float[9] {  1.0f,        0, -79.1f, -37.38f,        0, 46.8f, 14.4f,           0, 9},
            new float[9] {  1.0f,        0, -46.8f, -14.4f,         0, 79.1f, 37.38f,       0, 9},

        };
        float[][] armsSwingRightList = {
                            // Time,    Left Arm,           Right Arm,          Head
            new float[9] {  1.0f,        40.5f, -40.5f, -40.5f,       0, 46.8f, 14.4f,           0, 9},
            new float[9] {  1.0f,        40.5f, -40.5f, -40.5f,       0, 79.1f, 37.38f,           0, 9},
            new float[9] {  1.0f,        40.5f, -40.5f, -40.5f,       0, 46.8f, 14.4f,           0, 9},
            new float[9] {  1.0f,        40.5f, -40.5f, -40.5f,       0, 79.1f, 37.38f,           0, 9},

        };
        float[][] armsSwingLeftList = {
                            // Time,    Left Arm,           Right Arm,          Head
            new float[9] {  1.0f,       0, -46.8f, -14.4f,         -40.5f, 40.5f, 40.5f,         0, 9},
            new float[9] {  1.0f,       0, -79.1f, -37.38f,         -40.5f, 40.5f, 40.5f,          0, 9},
            new float[9] {  1.0f,       0, -46.8f, -14.4f,         -40.5f, 40.5f, 40.5f,         0, 9},
            new float[9] {  1.0f,       0, -79.1f, -37.38f,         -40.5f, 40.5f, 40.5f,          0, 9},

        };

        float[][] armsRightList = {
                            // Time,    Left Arm,           Right Arm,          Head
            new float[9] {  1.0f,       0, -88.2f, -27.9f,        0, 46.8f, 14.4f,         0, 9},

        };

        float[][] armsRightRightList = {
                            // Time,    Left Arm,           Right Arm,          Head
         new float[9] {  1.0f,       45, -45, -45,        0, 52, 16,          0, 10},

        };



        float[][] armsLeftList = {
                            // Time,    Left Arm,           Right Arm,          Head
            new float[9] {  1.0f,        0, -46.8f, -14.4f,       0, 88.2f, 27.9f,          0, 9},

        };

        float[][] armsLeftRightList = {
                            // Time,    Left Arm,           Right Arm,          Head
         new float[9] {  1.0f,        40.5f, -40.5f, -40.5f,      0, 90f, 60f,         0, 9},

        };
        float[][] armsRightLeftList = {
                            // Time,    Left Arm,           Right Arm,          Head
         new float[9] {  1.0f,       0, -90f, -60f,        -40.5f, 40.5f, 40.5f,          0, 9},
        };


        float[][] armsLeftLeftList = {
                            // Time,    Left Arm,           Right Arm,          Head
         new float[9] {  1.0f,       0, -52, -16,        -45, 45, 45,          0, 10 },

        };

        float[][] nodList = {
                            // Time,    Left Arm,           Right Arm,          Head
            new float[9] {  0.4f,       45f, -45f, -45f,    -45f, 45f, 45f,     -10f, -10f    },
            new float[9] {  0.4f,       45f, -45f, -45f,    -45f, 45f, 45f,     10f, -10f    },
            new float[9] {  0.4f,       45f, -45f, -45f,    -45f, 45f, 45f,     -10f, -10f    },
            new float[9] {  0.4f,       45f, -45f, -45f,    -45f, 45f, 45f,     10f, -10f    },


        };
        float[][] nodRightList = new float[1][] {
                            // Time,    Left Arm,           Right Arm,          Head
            new float[9] {  0.4f,       45f, -45f, -45f,    -45f, 45f, 45f,     -10f, -10f    },

        };
        float[][] nodLeftList = new float[1][] {
                     // Time,   Left Arm,          Right Arm,         Head
            new float[9] {  0.4f,       45f, -45f, -45f,    -45f, 45f, 45f,     10f, -10f    },

        };

        float[][] breathing_active = new float[][] {
                            // Time,   Left Arm,          Right Arm,         Head
            //new float[9] {  0.0f,       45f, -45f, -45f, 45f, 45f, 45f, 0f, 10f },
            new float[9] {  1.2f,       35f, -35f, -35f,    -35f, 35f, 35f,     0f, 5f },
            new float[9] {  1.2f,       45f, -45f, -45f,    -45f, 45f, 45f,     0f, 0f },
        };

        float[][] breathing_inactive = new float[][] {
                         // Time,   Left Arm,              Right Arm,         Head
            //new float[9] {  0.0f,       45f, -45f, -45f, 45f, 45f, 45f, 0f, 10f },
            //new float[9] {  1.2f,       40f, -40f, -40f,    -40f, 40f, 40f,    0f, -2f },
            //new float[9] {  1.2f,       45f, -45f, -45f,    -45f, 45f, 45f,    0f, 4f },
            new float[9] {  1.2f,       40f, -40f, -40f,    -40f, 40f, 40f,    0f, 0f },
            new float[9] {  1.2f,       45f, -45f, -45f,    -45f, 45f, 45f,    0f, 0f },
        };

        float[][] hurrayList = {
            new float[9] { 0.6f,     -70f, -60f, -25f,    70f, 60f, 25f,   0f, 18f },
            new float[9] { 0.6f,     50f, -35f, -15f,     -50f, 35f, 15f,   0f, 0f },
            new float[9] { 0.6f,     -70f, -60f, -25f,    70f, 60f, 25f,   0f, 18f },
            new float[9] { 0.6f,     50f, -35f, -15f,     -50f, 35f, 15f,   0f, 0f },

        };

        float[][] hi_v2List = {
            new float[9] { 0.6f,    -70f, -48.5f, -19.2f,     -50f, 35f, 32f,   0f, 13f },
            new float[9] { 0.6f,    -55f, -60f, -43f,       -50f, 37f, 27f,   0f, 0f },
            new float[9] { 0.6f,    -70f, -48.5f, -19.2f,     -50f, 35f, 32f,   0f, 13f },
            new float[9] { 0.6f,    -55f, -60f, -43f,       -50f, 37f, 27f,   0f, 0f },
        };

        float[][] handShakingList = {
            new float[9] { 0.6f,    58f, -40f, -3f,    15f, 65f, 20.7f,    0f, 12f },
            new float[9] { 0.6f,    58f, -40f, -3f,    -16f, 65f, 24.7f,     -3f, 4f },
            new float[9] { 0.6f,    58f, -40f, -3f,    15f, 65f, 20.7f,    0f, 12f },
            new float[9] { 0.6f,    58f, -40f, -3f,    -16f, 65f, 24.7f,     -3f, 4f },
        };

        float[][] excitingList = {
            new float[9] { 0.6f,    -47.6f, -38.1f, -85f,   53.1f, 7.6f, 12.5f,     -11.3f, 6.3f },
            new float[9] { 0.6f,    -29.8f, -38.1f, 0f,     64.7f, 36.7f, 65f,    12.8f, 4f },
            new float[9] { 0.6f,    -47.6f, -38.1f, -85f,   53.1f, 7.6f, 12.5f,     -11.3f, 6.3f },
            new float[9] { 0.6f,    -29.8f, -38.1f, 0f,     64.7f, 36.7f, 65f,    12.8f, 4f },
        };

        float[][] banggaBanggaList = {
            new float[9] { 0.6f,    -50f, -56f, -4f,  49.7f, 49.7f, 23.4f,    -4f, 5f },
            new float[9] { 0.6f,    -45f, -69f, -45f,   45f, 67f, 45f,      -1f, 2f },
            new float[9] { 0.6f,    -50f, -56f, -4f,  49.7f, 49.7f, 23.4f,    -4f, 5f },
            new float[9] { 0.6f,    -45f, -69f, -45f,   45f, 67f, 45f,      -1f, 2f },
        };

        float[][] sleepy_v2List = {
            new float[9] { 1.0f,    42.4f, -54.5f, -18.6f,    -42.4f, 54.5f, 20.2f,   17.7f, -15.0f },
            new float[9] { 0.5f,    35f, -63.8f, -15.3f,  -35f, 64.7f, 17f,     18.4f, 11f },
            new float[9] { 1.0f,    42.4f, -54.5f, -18.6f,    -42.4f, 54.5f, 20.2f,   17.7f, -15.0f },
            new float[9] { 0.5f,    35f, -63.8f, -15.3f,  -35f, 64.7f, 17f,     18.4f, 11f },
        };

        float[][] sad_v2List = {
            new float[9] { 0.5f,    -40f, -70f, -55f,     40f, 70f, 55f,   0f, -3f },
            new float[9] { 0.5f,    -40f, -70f, -65f,   40f, 70f, 65f,      0f, -13f },
            new float[9] { 0.5f,    -40f, -70f, -55f,     40f, 70f, 55f,   0f, -3f },
            new float[9] { 0.5f,    -40f, -70f, -65f,   40f, 70f, 65f,      0f, -13f },
        };

        float[][] rightPointList = {
            new float[9] { 0.6f,    51.9f, -52f, -29.7f,    54.4f, 53.4f, 20.9f,    -25.4f, 8f },
            new float[9] { 0.5f,    51.9f, -52f, -29.7f,    65.7f, 40.7f, 26.8f,    -25.3f, 11.9f },
            new float[9] { 0.6f,    51.9f, -52f, -29.7f,    54.4f, 53.4f, 20.9f,    -25.4f, 8f },
            new float[9] { 0.5f,    51.9f, -52f, -29.7f,    65.7f, 40.7f, 26.8f,    -25.3f, 11.9f },
        };

        float[][] politeGreetingList = {
            new float[9] { 0.7f,    39.6f, -38.16f, -80.1f,    -32.22f, 41.76f, 80.1f,     -1.71f, 4.23f },
            new float[9] { 0.9f,    39.6f, -45.81f, -80.1f,    -30.24f, 53.64f, 80.1f,     -1.53f, -13.5f },
            new float[9] { 0.7f,    39.6f, -38.16f, -80.1f,    -32.22f, 41.76f, 80.1f,     -1.71f, 4.23f },
            new float[9] { 1.0f,    39.6f, -40.5f,   -80.1f,    -40.5f,   40.5f,   80.1f,     0f,    0f},
        };

        float[][] ok_v2List = {
            new float[9] { 0.6f,    40.6f, -55.5f, -20.8f,  -40f, 58.3f, 14.7f,     -3.5f, 24.5f },
            new float[9] { 0.6f,    40.6f, -55.5f, -20.8f,  -40f, 58.3f, 14.7f,     -1.5f, -15.0f },
            new float[9] { 0.6f,    40.6f, -55.5f, -20.8f,  -40f, 58.3f, 14.7f,     -3.5f, 24.5f },
            new float[9] { 0.6f,    40.6f, -55.5f, -20.8f,  -40f, 58.3f, 14.7f,     -1.5f, -15.0f },
        };

        float[][] no_v2List = {
            new float[9] { 0.6f,    40.6f, -55.5f, -20.8f, -40f, 58.3f, 14.7f,     25.9f, 11.2f },
            new float[9] { 0.6f,    40.6f, -55.5f, -20.8f, -40f, 58.3f, 14.7f,     -30f, 12.4f },
            new float[9] { 0.6f,    40.6f, -55.5f, -20.8f, -40f, 58.3f, 14.7f,     25.9f, 11.2f },
            new float[9] { 0.6f,    40.6f, -55.5f, -20.8f, -40f, 58.3f, 14.7f,     -30f, 12.4f },
        };

        float[][] leftPointList = {
            new float[9] { 0.6f,    -63.2f, -63.2f, -6.4f,  -45.9f, 56f, 20.9f,     20.4f, -2.1f },
            new float[9] { 0.5f,    -56.4f, -65f, -14.6f,   -45.9f, 56f, 20.9f,     20.8f, -1.6f },
            new float[9] { 0.6f,    -63.2f, -63.2f, -6.4f,  -45.9f, 56f, 20.9f,     20.4f, -2.1f },
            new float[9] { 0.5f,    -56.4f, -65f, -14.6f,   -45.9f, 56f, 20.9f,     20.8f, -1.6f },
        };

        float[][] surprisedList = {
            new float[9] { 0.5f,    -61.1f, -63.5f, -28.6f,   61.1f, 63.5f, 28.6f,    0.2f, 21.2f },
        };

        float[][] armsForwardBackList = new float[6][] {
                     // Time,   Left Arm,          Right Arm,         Head
            new float[9] {  0.9f,       -20f, -45f, -45f,   20f, 45f, 45f,     0f, 25f    },
            new float[9] {  0.4f,       -20f, -75f, -55f,   20f, 75f, 55f,     0f, 25f   },
            new float[9] {  0.4f,       -20f, -45f, -45f,   20f, 45f, 45f,     0f, 25f   },
            new float[9] {  0.4f,       -20f, -75f, -55f,   20f, 75f, 55f,     0f, 25f   },
            new float[9] {  0.4f,       -20f, -45f, -45f,   20f, 45f, 45f,     0f, 25f   },
            new float[9] {  0.4f,       -20f, -75f, -55f,   20f, 75f, 55f,     0f, 25f   },

        };
        float[][] armsForwardBackRightList = new float[6][] {
                     // Time,   Left Arm,          Right Arm,         Head
            new float[9] {  0.9f,       45f, -45f, -45f,   20f, 45f, 45f,     0f, 25f    },
            new float[9] {  0.4f,       45f, -45f, -45f,   20f, 75f, 55f,     0f, 25f   },
            new float[9] {  0.4f,       45f, -45f, -45f,   20f, 45f, 45f,     0f, 25f   },
            new float[9] {  0.4f,       45f, -45f, -45f,   20f, 75f, 55f,     0f, 25f   },
            new float[9] {  0.4f,       45f, -45f, -45f,   20f, 45f, 45f,     0f, 25f   },
            new float[9] {  0.4f,       45f, -45f, -45f,   20f, 75f, 55f,     0f, 25f   },

        };
        float[][] armsForwardBackLeftList = new float[6][] {
                     // Time,   Left Arm,          Right Arm,         Head
            new float[9] {  0.9f,       -20f, -45f, -45f,   -45f, 45f, 45f,     0f, 25f    },
            new float[9] {  0.4f,       -20f, -75f, -55f,   -45f, 45f, 45f,     0f, 25f   },
            new float[9] {  0.4f,       -20f, -45f, -45f,   -45f, 45f, 45f,     0f, 25f   },
            new float[9] {  0.4f,       -20f, -75f, -55f,   -45f, 45f, 45f,     0f, 25f   },
            new float[9] {  0.4f,       -20f, -45f, -45f,   -45f, 45f, 45f,     0f, 25f   },
            new float[9] {  0.4f,       -20f, -75f, -55f,   -45f, 45f, 45f,     0f, 25f   },

        };

        float[][] complimentList = new float[][]
        {
            new float[9] {  0.4f,       -25f, -57f, -45f,     25f, 57f, 45f,   1f, 21f    },
            new float[9] {  0.4f,       -25f, -83f, -45f,      25f, 83f, 45f,  1f,0f     },
            new float[9] {  0.4f,       -25f, -57f, -45f,     25f, 57f, 45f,   1f, 21f    },
            new float[9] {  0.4f,       -25f, -83f, -45f,      25f, 83f, 45f,  1f,0f     },
            new float[9] {  0.4f,       -25f, -57f, -45f,     25f, 57f, 45f,   1f, 21f    },
            new float[9] {  0.4f,       -25f, -83f, -45f,      25f, 83f, 45f,  1f,0f     }
        };

        float[][] attention_v2List = new float[][]
        {
            new float[9] {  0.4f,       48f, -60f, -24f,      -44f, 60f, 29f,    2f, 20f    }
        };

        float[][] movement_shyList = {
            new float[9] { 0.4f,       46.35f, -53.64f, -6.03f,    -43.02f, 54.99f, 8.19f,     -17.19f, -8.28f },
            new float[9] { 0.4f,       38.43f, -47.52f, 0f,        -43.83f, 48.87f, 1.53f,     -13.95f, -7.56f },
            new float[9] { 0.4f,       46.35f, -53.64f, -6.03f,    -43.02f, 54.99f, 8.19f,     -17.19f, -8.28f },
            new float[9] { 0.4f,       38.43f, -47.52f, 0f,        -43.83f, 48.87f, 1.53f,     -13.95f, -7.56f },
        };

        float[][] armsMirrorList = {
            new float[9] { 0.4f, 0f, -73.35f, -19.98f, 0f, 73.8f, 25.02f, 0f, 0f },
            new float[9] { 0.4f, 0f, -13.86f, 0f, 0f, 10.71f, 0f, 0f, 0f },
            new float[9] { 0.4f, 0f, -73.35f, -19.98f, 0f, 73.8f, 25.02f, 0f, 0f },
            new float[9] { 0.4f, 0f, -13.86f, 0f, 0f, 10.71f, 0f, 0f, 0f },
        };

        float[][] movement_angryList = {
            new float[9] { 0.5f, 13.14f, -47.97f, -52.83f, -19.35f, 40.95f, 61.83f, 0f, -6.48f },
            new float[9] { 0.5f, 25.38f, -51.23f, -67.34f, -25.38f, 51.23f, 67.34f, 0f, -8.91f },
            new float[9] { 0.4f, 13.14f, -47.97f, -52.83f, -19.35f, 40.95f, 61.83f, 0f, -6.48f },
            new float[9] { 0.4f, 44.73f, -57.6f, -3.78f, -44.73f, 57.6f, 3.78f, 0f, 8.28f },
        };


        float[][] movement_disgustList = {
            new float[9] { 0.4f, -0.9f, -73.8f, 0f, 26.28f, 78.21f, 0f, -5.76f, 5.04f },
            new float[9] { 0.4f, -25.38f, -73.8f, 0f, 0f, 77.76f, 0f, 6.03f, 5.04f },
            new float[9] { 0.4f, -0.9f, -73.8f, 0f, 26.28f, 78.21f, 0f, -5.76f, 5.04f },
            new float[9] { 0.4f, -25.38f, -73.8f, 0f, 0f, 77.76f, 0f, 6.03f, 5.04f },
        };


        float[][] movement_fearList = {
            new float[9] { 0.4f, -47.16f, -65.88f, -62.37f, 36.81f, 60.21f, 66.24f, 17.91f, -15.75f },
            new float[9] { 0.4f, -36.81f, -60.21f, -66.24f, 47.16f, 65.88f, 62.37f, -17.91f, -15.75f },
            new float[9] { 0.4f, -47.16f, -65.88f, -62.37f, 36.81f, 60.21f, 66.24f, 17.91f, -15.75f },
            new float[9] { 0.4f, -36.81f, -60.21f, -66.24f, 47.16f, 65.88f, 62.37f, -17.91f, -15.75f },
        };


        float[][] rightArm_12List =
        {
            new float[9] {  0.7f,       45f, -45f, -45f,    0f, 90f, 0f,     0f, 0f    },
        };

        float[][] rightArm_1List =
        {
            new float[9] { 0.7f,       45f, -45f, -45f,    0f, 60f, 0f,     0f, 0f    },
        };

        float[][] rightArm_2List =
        {
            new float[9] { 0.7f,       45f, -45f, -45f,    0f, 30f, 0f,     0f, 0f    },
        };

        float[][] rightArm_3List =
        {
            new float[9] { 0.7f,       45f, -45f, -45f,    0f, 0f, 0f,     0f, 0f    },
        };

        float[][] rightArm_9List =
        {
            new float[9] { 0.7f,       45f, -45f, -45f,    0f, 90f, 90f,     0f, 0f    },
        };

        float[][] rightArm_10List =
        {
            new float[9] { 0.7f,       45f, -45f, -45f,    0f, 90f, 60f,     0f, 0f    },
        };

        float[][] rightArm_11List =
        {
            new float[9] { 0.7f,       45f, -45f, -45f,    0f, 90f, 30f,     0f, 0f    },
        };


        float[][] leftArm_12List =
        {
            new float[9] { 0.7f,       0f, -90f, 0f,  -45f, 45f, 45f,     0f, 0f    },
        };

        float[][] leftArm_1List =
        {
            new float[9] { 0.7f,       0f, -90f, -30f,  -45f, 45f, 45f,     0f, 0f    },
        };

        float[][] leftArm_2List =
        {
            new float[9] { 0.7f,       0f, -90f, -60f,  -45f, 45f, 45f,     0f, 0f    },
        };

        float[][] leftArm_3List =
        {
            new float[9] { 0.7f,       0f, -90f, -90f,  -45f, 45f, 45f,     0f, 0f    },
        };

        float[][] leftArm_9List =
        {
            new float[9] { 0.7f,       0f, 0f, 0f,   -45f, 45f, 45f,    0f, 0f    },
        };

        float[][] leftArm_10List =
        {
            new float[9] { 0.7f,       0f, -30f, 0f,   -45f, 45f, 45f,      0f, 0f    },
        };

        float[][] leftArm_11List =
        {
            new float[9] { 0.7f,       0f, -60f, 0f,    -45f, 45f, 45f,     0f, 0f    },
        };


        float[][] head_12List =
        {
            new float[9] { 0.7f,       45f, -45f, -45f,    -45f, 45f, 45f,     0f, 0f    },
        };

        float[][] head_1List =
        {
            new float[9] { 0.7f,       45f, -45f, -45f,    -45f, 45f, 45f,     -30f, 0f    },
        };

        float[][] head_2List =
        {
            new float[9] { 0.7f,       45f, -45f, -45f,    -45f, 45f, 45f,     -60f, 0f    },
        };

        float[][] head_3List =
        {
            new float[9] { 0.7f,       45f, -45f, -45f,    -45f, 45f, 45f,     -90f, 0f    },
        };

        float[][] head_9List =
        {
            new float[9] { 0.7f,       45f, -45f, -45f,    -45f, 45f, 45f,     90f, 0f    },
        };

        float[][] head_10List =
        {
            new float[9] { 0.7f,       45f, -45f, -45f,    -45f, 45f, 45f,     60f, 0f    },
        };

        float[][] head_11List =
        {
            new float[9] { 0.7f,       45f, -45f, -45f,    -45f, 45f, 45f,     30f, 0f    },
        };

        float[][] arms_12List =
        {
            new float[9] { 0.7f,        0f, -90f, 0f,     0f, 90f, 0f,        0f, 0f },
        };
        float[][] arms_1List =
        {
            new float[9] { 0.7f,        0f, -90f, -30f,     0f, 60f, 0f,        0f, 0f },
        };
        float[][] arms_2List =
        {
            new float[9] { 0.7f,        0f, -90f, -60f,     0f, 30f, 0f,        0f, 0f },
        };
        float[][] arms_3List =
        {
            new float[9] { 0.7f,        0f, -90f, -90f,     0f, 30f, 0f,        0f, 0f },
        };
        float[][] arms_9List =
        {
            new float[9] { 0.7f,        0f, 0f, 0f,     0f, 90f, 90f,        0f, 0f },
        };
        float[][] arms_10List =
        {
            new float[9] { 0.7f,        0f, -30f, 0f,     0f, 90f, 60f,        0f, 0f },
        };
        float[][] arms_11List =
        {
            new float[9] { 0.7f,        0f, -60f, 0f,     0f, 90f, 30f,        0f, 0f },
        };






        public Dictionary<string, float[][]> motionTable;
        IEnumerator currentAnimation = null;
        private bool isPlaying = false;
        private bool isBreathActive = true;

        private readonly float playMotionDelayTime = 1f;

        Queue<MotionAnimInfo> animationQueue = new Queue<MotionAnimInfo>();

        private IEnumerator Start()
        {
            InitMotionTable();

            //yield return StartCoroutine(TestAllMotion());
            yield return StartCoroutine(GestureProcess(politeGreetingList));
            //StartCoroutine(GestureProcess(breathing_active));

            //float[][] test_;
            //motionTable.TryGetValue("오른팔-상", out test_);
            //if(test_ != null)
            //{
            //    Debug.Log("있어용");
            //}

            // if (isBreathActive) PlayMotion("breathing_active");
        }

        //public void ClickedNextButton()
        //{
        //    string keys = keyInput.text;
        //    float[][] motion = motionTable[keys];
        //    int Length = motion.Length;
        //    Debug.Log("길이 : " + Length);
            
        //    for (int i = 0; i < 8; i++)
        //        SetAngle(i, motion[cnt][i+1]*1.2f);
        //    cnt++;
        //    if (cnt > Length-1)
        //    {
        //        keyInput.text = null;
        //        cnt = 0;
        //        Debug.Log("#########끝#######");
        //    }
        //}

        public void SetBreathActiveState(string message)
        {
            isBreathActive = Convert.ToBoolean(message);
        }

        public void PlayMotion(string motion)
        {
            //// 흘끗보기 동작은 우선적으로 재생.
            //if (motion.Contains("nod"))
            //{
            //    if (isPlaying)
            //    {
            //        StopAllCoroutines();
            //        isPlaying = false;
            //    }

            //    StartCoroutine(PlayMotionCoroutine(motion));
            //    return;
            //}

            if (!isBreathActive && !motion.Contains("breathing"))
            {
                StartCoroutine("DelayPlayMotion", motion);
                return;
            }
            else
            {
                if (isPlaying)
                {
                    StopCoroutine("GestureProcess");
                    StopCoroutine("DelayPlayMotion");
                    isPlaying = false;
                }

                //Debug.Log("Play Motion: " + motion);
                StartCoroutine(PlayMotionCoroutine(motion));
                return;

            }

            //animationQueue.Enqueue(new MotionAnimInfo(motion, PlayMotionCoroutine(motion)));
            //if (!isPlaying && animationQueue.Count > 0)
            //{
            //    StartCoroutine(animationQueue.Dequeue().motionCoroutine);
            //}
        }

        IEnumerator DelayPlayMotion(string motion)
        {
            yield return new WaitForSeconds(playMotionDelayTime);
            PlayMotion(motion);
        }

        IEnumerator TestAllMotion()
        {
            foreach (KeyValuePair<string, float[][]> motion in motionTable)
            {
                Debug.Log(motion.Key);
                yield return PlayMotionCoroutine(motion.Key);
            }
        }

        public void SetZeroPos()
        {
            for (int i = 0; i < 6; i++)
            {
                SetAngle(i, zeroAngle[i]);
            }
        }

        public IEnumerator SetBasePos()
        {
            float basePoseTime = 1f;

            for (int ix = 0; ix < jointInfo.Length; ++ix)
            {
                //jointInfo[ix].SetBaseAngle();
                //SetAngle(ix, baseAngle[ix]);
                SetAngleLerp(ix, baseAngle[ix], basePoseTime);
            }

            yield return new WaitForSeconds(basePoseTime);

            if (StateUpdater.isCallingADV)
                StateUpdater.isCallingADV = false;
        }

        //public bool PlayMotion(string gesture)
        public IEnumerator PlayMotionCoroutine(string gesture)
        {
            float[][] gestureInfo;
            if (motionTable.TryGetValue(gesture, out gestureInfo))
            {
                currentGesture = gesture;
                yield return StartCoroutine("GestureProcess", gestureInfo);
                currentAnimation = null;
                //return true;
            }
            else
            {
                currentGesture = string.Empty;
                currentAnimation = null;
                //return false;
            }
        }

        public float[][] keyMotionTable(string keys)
        {
            if (motionTable.ContainsKey(keys))
            {
                return motionTable[keys];
            }
            else
                return null;

        }

        void InitMotionTable()
        {
            motionTable = new Dictionary<string, float[][]>();
            //motionTable.Add("hi", hiList);  // 삭제.
            motionTable.Add("hello", helloList);
            motionTable.Add("angry", angryList);
            motionTable.Add("sad", sadList);
            motionTable.Add("머리고개얼굴목-상하반복", okList);
            motionTable.Add("팔양팔두팔양쪽팔-전후반복", armsForwardBackList);
            motionTable.Add("오른팔-전후반복", armsForwardBackRightList);
            motionTable.Add("왼팔-전후반복", armsForwardBackLeftList);

            motionTable.Add("머리고개얼굴목-좌우반복", noList);
            motionTable.Add("wrong", wrongList);
            motionTable.Add("happy", happyList);
            motionTable.Add("nod", nodList);
            motionTable.Add("nodRight", nodRightList);
            motionTable.Add("nodLeft", nodLeftList);
            motionTable.Add("breathing_active", breathing_active);
            motionTable.Add("breathing_inactive", breathing_inactive);
            motionTable.Add("refuse", refuseList);
            motionTable.Add("smile", smileList);
            motionTable.Add("takenaback", takenabackList);
            motionTable.Add("tell", tellList);
            motionTable.Add("sleep", sleepyList);
            motionTable.Add("thinking", thinkingList);
            motionTable.Add("전신-(규모 등에)놀람", excellentList);  // 박수.
            motionTable.Add("stretch", stretchList);
            motionTable.Add("전신-허그", hugList);
            //motionTable.Add("fear", fearList);  // 새로 만들기.
            motionTable.Add("bow", bowList);
            motionTable.Add("attention", attentionList);
            motionTable.Add("best", bestList);
            motionTable.Add("leftNeck", leftNeckList);
            motionTable.Add("rightNeck", rightNeckList);
            motionTable.Add("머리고개얼굴목-전", forwardList);
            //motionTable.Add("coach", coachList);  // 삭제.
            //motionTable.Add("require", requireList);  // 삭제.
            //motionTable.Add("reset", resetList);  // 삭제.
            //motionTable.Add("exchanged", exchangedList);  // 삭제.
            motionTable.Add("shake", shakeList);

            motionTable.Add("전신-만세", hurrayList);
            motionTable.Add("hi_v2", hi_v2List);
            motionTable.Add("전신-악수", handShakingList);
            motionTable.Add("전신-기쁨", excitingList);
            motionTable.Add("전신-팔로인사", banggaBanggaList);
            motionTable.Add("전신-졸림", sleepy_v2List);
            motionTable.Add("전신-슬픔", sad_v2List);
            motionTable.Add("right_point", rightPointList);
            motionTable.Add("전신-고개숙여인사", politeGreetingList);
            motionTable.Add("전신-긍정", ok_v2List);
            motionTable.Add("전신-부정", no_v2List);
            motionTable.Add("left_point", leftPointList);
            motionTable.Add("전신-(소리 등에)놀람", surprisedList);
            motionTable.Add("전신-집중", attention_v2List);
            motionTable.Add("전신-칭찬", complimentList);

            motionTable.Add("headTilt", headTiltList);
            motionTable.Add("머리고개얼굴목-상-후", headBackList);
            motionTable.Add("머리고개얼굴목-상-전", headUpList);
            motionTable.Add("머리고개얼굴목-우", headRightList);
            motionTable.Add("머리고개얼굴목-좌", headLeftList);
            motionTable.Add("전신-회피", headLeftList);
            motionTable.Add("머리고개얼굴목-하", headDownList);
            motionTable.Add("head", headList);

            motionTable.Add("팔양팔두팔양쪽팔-상", armsUpList);
            motionTable.Add("오른팔-상", armsUpRightList);
            motionTable.Add("왼팔-상", armsUpLeftList);
            motionTable.Add("팔양팔두팔양쪽팔-하", armsDownList);
            motionTable.Add("오른팔-하", armsDownRightList);
            motionTable.Add("왼팔-하", armsDownLeftList);
            motionTable.Add("팔양팔두팔양쪽팔-상하반복", armsUpdownList);
            motionTable.Add("오른팔-상하반복", armsUpdownRightList);
            motionTable.Add("왼팔-상하반복", armsUpdownLeftList);
            motionTable.Add("팔양팔두팔양쪽팔-안(몸)쪽", armsInList);
            motionTable.Add("오른팔-안(몸)쪽", armsInRightList);
            motionTable.Add("왼팔-안(몸)쪽", armsInLeftList);
            motionTable.Add("spreadout", spreadoutList);
            motionTable.Add("spreadoutRight", spreadoutRightList);
            motionTable.Add("spreadoutLeft", spreadoutLeftList);
            motionTable.Add("팔양팔두팔양쪽팔-전", armsForwardList);
            motionTable.Add("오른팔-전", armsForwardRightList);
            motionTable.Add("왼팔-전", armsForwardLeftList);
            motionTable.Add("armsFront", armsFrontList);
            motionTable.Add("armsFrontRight", armsFrontRightList);
            motionTable.Add("armsFrontLeft", armsFrontLeftList);
            motionTable.Add("팔양팔두팔양쪽팔-좌우반복", armsSwingList);
            motionTable.Add("팔양팔두팔양쪽팔-좌우대칭", armsMirrorList);
            motionTable.Add("오른팔-좌우반복", armsSwingRightList);
            motionTable.Add("왼팔-좌우반복", armsSwingLeftList);
            motionTable.Add("팔양팔두팔양쪽팔-우-같이", armsRightList);
            motionTable.Add("오른팔-우", armsRightRightList);
            motionTable.Add("왼팔-우", armsRightLeftList);
            motionTable.Add("팔양팔두팔양쪽팔-좌-같이", armsLeftList);
            motionTable.Add("오른팔-좌", armsLeftRightList);
            motionTable.Add("왼팔-좌", armsLeftLeftList);
            motionTable.Add("전신-두려움", movement_fearList);
            motionTable.Add("전신-부끄러움", movement_shyList);
            motionTable.Add("전신-혐오싫음", movement_disgustList);
            motionTable.Add("전신-화남", movement_angryList);
            motionTable.Add("전신-생각", thinkingList);
            motionTable.Add("전신-기본자세", breathing_active);


            motionTable.Add("오른팔-DEG-0도", rightArm_12List);
            motionTable.Add("오른팔-DEG-30도", rightArm_1List);
            motionTable.Add("오른팔-DEG-60도", rightArm_2List);
            motionTable.Add("오른팔-DEG-90도", rightArm_3List);
            motionTable.Add("오른팔-DEG--90도", rightArm_9List);
            motionTable.Add("오른팔-DEG--60도", rightArm_10List);
            motionTable.Add("오른팔-DEG--30도", rightArm_11List);

            motionTable.Add("왼팔-DEG-0도", leftArm_12List);
            motionTable.Add("왼팔-DEG--30도", leftArm_11List);
            motionTable.Add("왼팔-DEG--60도", leftArm_10List);
            motionTable.Add("왼팔-DEG--90도", leftArm_9List);
            motionTable.Add("왼팔-DEG-30도", leftArm_1List);
            motionTable.Add("왼팔-DEG-60도", leftArm_2List);
            motionTable.Add("왼팔-DEG-90도", leftArm_3List);


            motionTable.Add("머리고개얼굴목-DEG-0도", head_12List);
            motionTable.Add("머리고개얼굴목-DEG-30도", head_1List);
            motionTable.Add("머리고개얼굴목-DEG-60도", head_2List);
            motionTable.Add("머리고개얼굴목-DEG-90도", head_3List);
            motionTable.Add("머리고개얼굴목-DEG--30도", head_11List);
            motionTable.Add("머리고개얼굴목-DEG--60도", head_10List);
            motionTable.Add("머리고개얼굴목-DEG--90도", head_9List);

            motionTable.Add("팔양팔두팔양쪽팔-DEG-0도", arms_12List);
            motionTable.Add("팔양팔두팔양쪽팔-DEG-30도", arms_1List);
            motionTable.Add("팔양팔두팔양쪽팔-DEG-60도", arms_2List);
            motionTable.Add("팔양팔두팔양쪽팔-DEG-90도", arms_3List);
            motionTable.Add("팔양팔두팔양쪽팔-DEG--30도", arms_11List);
            motionTable.Add("팔양팔두팔양쪽팔-DEG--60도", arms_10List);
            motionTable.Add("팔양팔두팔양쪽팔-DEG--90도", arms_9List);


        }

        float GetDurationToFirstAngle(float[] motionInfo)
        {
            float maxDegree = 0f;
            for (int ix = 0; ix < jointInfo.Length; ++ix)
            {
                float degree = jointInfo[ix].GetTargetAngleAxis(motionInfo[ix + 1]);
                maxDegree = Mathf.Max(maxDegree, degree);
            }

            return maxDegree;
        }

        public IEnumerator GestureProcess(float[][] motionInfo)
        {
            
            isPlaying = true;

            for (int ix = 0; ix < motionInfo.Length; ++ix)
            {
                // 첫 번째를 제외한 나머지 각도 설정인 경우, 각도 데이터에 있는 시간 값 사용.
                float rotDuration = motionInfo[ix][0];
                //Debug.Log(motionInfo[ix][8]);
                // 첫 번째 각도 설정인 경우, 최대 회전 각도를 기반으로 회전 시간 계산.
                if (ix == 0 && motionInfo.Length != 1 && MotionCoaching.repeat == false)
                {
                    float maxDegree = GetDurationToFirstAngle(motionInfo[0]);
                    if (maxDegree < 90f)
                        maxDegree += 300f;
                    rotDuration = maxDegree / (rotSpeed * rotSpeedPercentage);
                    //Debug.Log("Max Degree: " + maxDegree + " , Rot Duration: " + rotDuration);
                }

                for (int jx = 0; jx < jointInfo.Length; ++jx)
                {
                    // For Debug.
                    //if (jx == 0)
                    //{
                    //    StartCoroutine(jointInfo[jx].SetAngleLerp(motionInfo[ix][jx + 1], motionInfo[ix][0], true));
                    //    continue;
                    //}


                    if (jx == 7)
                    {
                        StartCoroutine(jointInfo[jx].SetAngleLerp((motionInfo[ix][jx + 1]), rotDuration));
                        // Debug.Log(jx + ": " + motionInfo[ix][jx + 1]);
                        continue;
                    }


                    StartCoroutine(jointInfo[jx].SetAngleLerp((motionInfo[ix][jx + 1]), rotDuration));
                    // Debug.Log(jx + ": " + motionInfo[ix][jx + 1]);
                }

                //float waitTime = motionInfo[ix][0];

                yield return new WaitForSeconds(rotDuration);

            }

            // yield return StartCoroutine(SetBasePos());

            isPlaying = false;

            //Debug.Log("Motion Finished, queue count: " + animationQueue.Count);
            if (animationQueue.Count > 0)
            {
                MotionAnimInfo info = animationQueue.Dequeue();
                //Debug.Log("Play Next motion: " + info.motion);
                StartCoroutine(info.motionCoroutine);
            }
            else if (animationQueue.Count == 0)
            {
                if (breath)
                {
                    //bool isActiveMode = WebSurvey.Instance.GetBehaviorMode == WebSurvey.Mode.Active;
                    string brethingName = isBreathActive ? "breathing_active" : "breathing_inactive";
                    PlayMotion(brethingName);
                }
            }
            //face.Clear();
            //breatheEnable = true;
        }

        IEnumerator WaitUntilNextCoroutine(float time)
        {
            float elapsedTime = 0f;
            while (elapsedTime <= time)
            {
                elapsedTime += Time.deltaTime;
                yield return null;
            }
        }

        IEnumerator TestExecutor()
        {
            for (int ix = 0; ix < 5; ++ix)
            {
                yield return StartCoroutine(Test());
            }
        }

        float waitTime = 0.1f;
        IEnumerator Test()
        {
            for (int ix = 1; ix < 11; ++ix)
            {
                //SetAngle(6, -ix * 10f);
                SetAngle(7, ix * 10f);

                yield return new WaitForSeconds(waitTime);
            }

            yield return new WaitForSeconds(.5f);
            SetBasePos();
        }

        public void SetAngle(int jointId, float angle)
        {
            jointInfo[jointId].SetAngle(angle);
        }

        public void SetAngleLerp(int jointId, float angle, float duration)
        {
            StartCoroutine(jointInfo[jointId].SetAngleLerp(angle, duration));
        }

        public float GetAngle(int JointID)
        {
            return jointInfo[JointID].GetAngle();
        }

        class MotionAnimInfo
        {
            public string motion;
            public IEnumerator motionCoroutine;

            public MotionAnimInfo() { }
            public MotionAnimInfo(string motion, IEnumerator coroutine)
            {
                this.motion = motion;
                motionCoroutine = coroutine;
            }
        }
    }
}