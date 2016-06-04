using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace BOXING
{
    public class AccData
    {
        public Double mX;
        public Double mY;
        public Double mZ;
        public Double mG;
        public DateTimeOffset mTime;

        public AccData()
        {
            mX = mY = mZ = mG = 0.0;
            mTime = new DateTimeOffset(DateTime.Now);
        }

        public AccData(Double x, Double y, Double z, Double g, DateTimeOffset t)
        {
            mX = x;
            mY = y;
            mZ = z;
            mG = g;
            mTime = t;
        }
    }
}
