using System;
using System.Collections.Generic;
using System.Text;

namespace Buildersoft.Andy.X.Utilities.Random
{
    public static class RandomGenerator
    {
        public static int GetRandomSharedReader(int min, int countReaders)
        {
            System.Random random = new System.Random();
            return random.Next(min, countReaders);
        }
    }
}
