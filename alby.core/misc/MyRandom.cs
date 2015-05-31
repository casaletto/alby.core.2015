using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Security.Cryptography;

namespace alby.core.misc
{
    public class MyRandom
    {
        protected byte GetByte()
        {
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            byte[] byteArray = new byte[1];

            rng.GetBytes( byteArray );
            return byteArray[0] ;
        }

		//
		// return a random number from 0 to 255, further limited by min  and max
		//
        public int GetSmallInt( int min, int max )
        {
            while (true)
            {
                byte b = this.GetByte();
                int n = Convert.ToInt32(b);

                if ( min <= n && n <= max )
                    return n ;
            }
        }

    } // end class
}
