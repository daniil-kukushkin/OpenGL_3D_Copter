using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace _3D_Copter_emulator
{
    class CopterModel
    {
        public int countMotors;
        public double[] centerCoord = new double[3];
        public double[] angles = new double[3];
        public CopterModel(int countMotors_)
        {
            countMotors = countMotors_;
            for (int i = 0; i < 3; i++ )
                centerCoord[i] = angles[i] = 0;
        }
    }
}
