using System;

namespace CrappyPrizm.Tools.Crypto
{
    internal static partial class Curve25519
    {
        private class Long10
        {
            #region Var
            private readonly long[] Digits;

            public bool IsOverflow => this[0] > P26 - 19 && (this[1] & this[3] & this[5] & this[7] & this[9]) == P25 && (this[2] & this[4] & this[6] & this[8]) == P26 || this[9] > P25;

            public int IsNegative => (int)(((IsOverflow || (this[9] < 0)) ? 1 : 0) ^ (this[0] & 1));

            private const int Size = 10; 
            #endregion

            #region Init
            public Long10() => Digits = new long[Size];

            public Long10(params long[] digits)
            {
                Digits = new long[Size];
                int max = Math.Max(digits.Length, Size);
                for (int i = 0; i < max; ++i)
                    Digits[i] = digits[i];
            }
            #endregion

            #region Functions
            public long this[int index]
            {
                get => Digits[index];
                set => Digits[index] = value;
            }

            public long[] GetDigits()
            {
                long[] digits = new long[Size];
                Array.Copy(Digits, digits, Size);
                return digits;
            }

            public static Long10 operator+(Long10 a, Long10 b)
            {
                Long10 result = new Long10();
                for (int i = 0; i < Size; ++i)
                    result[i] = a[i] + b[i];
                return result;
            }

            public static Long10 operator-(Long10 a, Long10 b)
            {
                Long10 result = new Long10();
                for (int i = 0; i < Size; ++i)
                    result[i] = a[i] - b[i];
                return result;
            }
            #endregion
        }

        private static void Unpack(Long10 x, byte[] m)
        {
            x[0] = ((m[0] & 0xFF)) | ((m[1] & 0xFF)) << 8 |
                    (m[2] & 0xFF) << 16 | ((m[3] & 0xFF) & 3) << 24;
            x[1] = ((m[3] & 0xFF) & ~3) >> 2 | (m[4] & 0xFF) << 6 |
                    (m[5] & 0xFF) << 14 | ((m[6] & 0xFF) & 7) << 22;
            x[2] = ((m[6] & 0xFF) & ~7) >> 3 | (m[7] & 0xFF) << 5 |
                    (m[8] & 0xFF) << 13 | ((m[9] & 0xFF) & 31) << 21;
            x[3] = ((m[9] & 0xFF) & ~31) >> 5 | (m[10] & 0xFF) << 3 |
                    (m[11] & 0xFF) << 11 | ((m[12] & 0xFF) & 63) << 19;
            x[4] = ((m[12] & 0xFF) & ~63) >> 6 | (m[13] & 0xFF) << 2 |
                    (m[14] & 0xFF) << 10 | (m[15] & 0xFF) << 18;
            x[5] = (m[16] & 0xFF) | (m[17] & 0xFF) << 8 |
                    (m[18] & 0xFF) << 16 | ((m[19] & 0xFF) & 1) << 24;
            x[6] = ((m[19] & 0xFF) & ~1) >> 1 | (m[20] & 0xFF) << 7 |
                    (m[21] & 0xFF) << 15 | ((m[22] & 0xFF) & 7) << 23;
            x[7] = ((m[22] & 0xFF) & ~7) >> 3 | (m[23] & 0xFF) << 5 |
                    (m[24] & 0xFF) << 13 | ((m[25] & 0xFF) & 15) << 21;
            x[8] = ((m[25] & 0xFF) & ~15) >> 4 | (m[26] & 0xFF) << 4 |
                    (m[27] & 0xFF) << 12 | ((m[28] & 0xFF) & 63) << 20;
            x[9] = ((m[28] & 0xFF) & ~63) >> 6 | (m[29] & 0xFF) << 2 |
                    (m[30] & 0xFF) << 10 | (m[31] & 0xFF) << 18;
        }

        private static void Pack(Long10 x, byte[] m)
        {
            int ld = (x.IsOverflow ? 1 : 0) - (x[9] < 0 ? 1 : 0);
            int ud = ld * -(P25 + 1);
            ld *= 19;
            long t = ld + x[0] + (x[1] << 26);
            m[0] = (byte)t;
            m[1] = (byte)(t >> 8);
            m[2] = (byte)(t >> 16);
            m[3] = (byte)(t >> 24);
            t = (t >> 32) + (x[2] << 19);
            m[4] = (byte)t;
            m[5] = (byte)(t >> 8);
            m[6] = (byte)(t >> 16);
            m[7] = (byte)(t >> 24);
            t = (t >> 32) + (x[3] << 13);
            m[8] = (byte)t;
            m[9] = (byte)(t >> 8);
            m[10] = (byte)(t >> 16);
            m[11] = (byte)(t >> 24);
            t = (t >> 32) + (x[4] << 6);
            m[12] = (byte)t;
            m[13] = (byte)(t >> 8);
            m[14] = (byte)(t >> 16);
            m[15] = (byte)(t >> 24);
            t = (t >> 32) + x[5] + (x[6] << 25);
            m[16] = (byte)t;
            m[17] = (byte)(t >> 8);
            m[18] = (byte)(t >> 16);
            m[19] = (byte)(t >> 24);
            t = (t >> 32) + (x[7] << 19);
            m[20] = (byte)t;
            m[21] = (byte)(t >> 8);
            m[22] = (byte)(t >> 16);
            m[23] = (byte)(t >> 24);
            t = (t >> 32) + (x[8] << 12);
            m[24] = (byte)t;
            m[25] = (byte)(t >> 8);
            m[26] = (byte)(t >> 16);
            m[27] = (byte)(t >> 24);
            t = (t >> 32) + ((x[9] + ud) << 6);
            m[28] = (byte)t;
            m[29] = (byte)(t >> 8);
            m[30] = (byte)(t >> 16);
            m[31] = (byte)(t >> 24);
        }

        private static void Copy(Long10 o, Long10 i)
        {
            o[0] = i[0]; o[1] = i[1];
            o[2] = i[2]; o[3] = i[3];
            o[4] = i[4]; o[5] = i[5];
            o[6] = i[6]; o[7] = i[7];
            o[8] = i[8]; o[9] = i[9];
        }

        private static void Set(Long10 o, int i)
        {
            o[0] = i; o[1] = 0;
            o[2] = 0; o[3] = 0;
            o[4] = 0; o[5] = 0;
            o[6] = 0; o[7] = 0;
            o[8] = 0; o[9] = 0;
        }

        private static void Add(Long10 xy, Long10 x, Long10 y)
        {
            xy[0] = x[0] + y[0]; xy[1] = x[1] + y[1];
            xy[2] = x[2] + y[2]; xy[3] = x[3] + y[3];
            xy[4] = x[4] + y[4]; xy[5] = x[5] + y[5];
            xy[6] = x[6] + y[6]; xy[7] = x[7] + y[7];
            xy[8] = x[8] + y[8]; xy[9] = x[9] + y[9];
        }

        private static void Sub(Long10 xy, Long10 x, Long10 y)
        {
            xy[0] = x[0] - y[0]; xy[1] = x[1] - y[1];
            xy[2] = x[2] - y[2]; xy[3] = x[3] - y[3];
            xy[4] = x[4] - y[4]; xy[5] = x[5] - y[5];
            xy[6] = x[6] - y[6]; xy[7] = x[7] - y[7];
            xy[8] = x[8] - y[8]; xy[9] = x[9] - y[9];
        }

        private static Long10 MulSmall(Long10 xy, Long10 x, long y)
        {
            long t = (x[8] * y);
            xy[8] = (t & ((1 << 26) - 1));
            t = (t >> 26) + (x[9] * y);
            xy[9] = (t & ((1 << 25) - 1));
            t = 19 * (t >> 25) + (x[0] * y);
            xy[0] = (t & ((1 << 26) - 1));
            t = (t >> 26) + (x[1] * y);
            xy[1] = (t & ((1 << 25) - 1));
            t = (t >> 25) + (x[2] * y);
            xy[2] = (t & ((1 << 26) - 1));
            t = (t >> 26) + (x[3] * y);
            xy[3] = (t & ((1 << 25) - 1));
            t = (t >> 25) + (x[4] * y);
            xy[4] = (t & ((1 << 26) - 1));
            t = (t >> 26) + (x[5] * y);
            xy[5] = (t & ((1 << 25) - 1));
            t = (t >> 25) + (x[6] * y);
            xy[6] = (t & ((1 << 26) - 1));
            t = (t >> 26) + (x[7] * y);
            xy[7] = (t & ((1 << 25) - 1));
            t = (t >> 25) + xy[8];
            xy[8] = (t & ((1 << 26) - 1));
            xy[9] += (t >> 26);
            return xy;
        }

        private static Long10 Mul(Long10 xy, Long10 x, Long10 y)
        {
            long t = (x[0] * y[8]) + (x[2] * y[6]) + (x[4] * y[4]) + (x[6] * y[2]) +
                    (x[8] * y[0]) + 2 * ((x[1] * y[7]) + (x[3] * y[5]) +
                    (x[5] * y[3]) + (x[7] * y[1])) + 38 *
                    (x[9] * y[9]);
            xy[8] = (t & ((1 << 26) - 1));
            t = (t >> 26) + (x[0] * y[9]) + (x[1] * y[8]) + (x[2] * y[7]) +
                    (x[3] * y[6]) + (x[4] * y[5]) + (x[5] * y[4]) +
                    (x[6] * y[3]) + (x[7] * y[2]) + (x[8] * y[1]) +
                    (x[9] * y[0]);
            xy[9] = (t & ((1 << 25) - 1));
            t = (x[0] * y[0]) + 19 * ((t >> 25) + (x[2] * y[8]) + (x[4] * y[6])
                    + (x[6] * y[4]) + (x[8] * y[2])) + 38 *
                    ((x[1] * y[9]) + (x[3] * y[7]) + (x[5] * y[5]) +
                            (x[7] * y[3]) + (x[9] * y[1]));
            xy[0] = (t & ((1 << 26) - 1));
            t = (t >> 26) + (x[0] * y[1]) + (x[1] * y[0]) + 19 * ((x[2] * y[9])
                    + (x[3] * y[8]) + (x[4] * y[7]) + (x[5] * y[6]) +
                    (x[6] * y[5]) + (x[7] * y[4]) + (x[8] * y[3]) +
                    (x[9] * y[2]));
            xy[1] = (t & ((1 << 25) - 1));
            t = (t >> 25) + (x[0] * y[2]) + (x[2] * y[0]) + 19 * ((x[4] * y[8])
                    + (x[6] * y[6]) + (x[8] * y[4])) + 2 * (x[1] * y[1])
                    + 38 * ((x[3] * y[9]) + (x[5] * y[7]) +
                    (x[7] * y[5]) + (x[9] * y[3]));
            xy[2] = (t & ((1 << 26) - 1));
            t = (t >> 26) + (x[0] * y[3]) + (x[1] * y[2]) + (x[2] * y[1]) +
                    (x[3] * y[0]) + 19 * ((x[4] * y[9]) + (x[5] * y[8]) +
                    (x[6] * y[7]) + (x[7] * y[6]) +
                    (x[8] * y[5]) + (x[9] * y[4]));
            xy[3] = (t & ((1 << 25) - 1));
            t = (t >> 25) + (x[0] * y[4]) + (x[2] * y[2]) + (x[4] * y[0]) + 19 *
                    ((x[6] * y[8]) + (x[8] * y[6])) + 2 * ((x[1] * y[3]) +
                    (x[3] * y[1])) + 38 *
                    ((x[5] * y[9]) + (x[7] * y[7]) + (x[9] * y[5]));
            xy[4] = (t & ((1 << 26) - 1));
            t = (t >> 26) + (x[0] * y[5]) + (x[1] * y[4]) + (x[2] * y[3]) +
                    (x[3] * y[2]) + (x[4] * y[1]) + (x[5] * y[0]) + 19 *
                    ((x[6] * y[9]) + (x[7] * y[8]) + (x[8] * y[7]) +
                            (x[9] * y[6]));
            xy[5] = (t & ((1 << 25) - 1));
            t = (t >> 25) + (x[0] * y[6]) + (x[2] * y[4]) + (x[4] * y[2]) +
                    (x[6] * y[0]) + 19 * (x[8] * y[8]) + 2 * ((x[1] * y[5]) +
                    (x[3] * y[3]) + (x[5] * y[1])) + 38 *
                    ((x[7] * y[9]) + (x[9] * y[7]));
            xy[6] = (t & ((1 << 26) - 1));
            t = (t >> 26) + (x[0] * y[7]) + (x[1] * y[6]) + (x[2] * y[5]) +
                    (x[3] * y[4]) + (x[4] * y[3]) + (x[5] * y[2]) +
                    (x[6] * y[1]) + (x[7] * y[0]) + 19 * ((x[8] * y[9]) +
                    (x[9] * y[8]));
            xy[7] = (t & ((1 << 25) - 1));
            t = (t >> 25) + xy[8];
            xy[8] = (t & ((1 << 26) - 1));
            xy[9] += (t >> 26);
            return xy;
        }

        private static Long10 Sqr(Long10 x2, Long10 x)
        {
            long t = (x[4] * x[4]) + 2 * ((x[0] * x[8]) + (x[2] * x[6])) + 38 *
                    (x[9] * x[9]) + 4 * ((x[1] * x[7]) + (x[3] * x[5]));
            x2[8] = (t & ((1 << 26) - 1));
            t = (t >> 26) + 2 * ((x[0] * x[9]) + (x[1] * x[8]) + (x[2] * x[7]) +
                    (x[3] * x[6]) + (x[4] * x[5]));
            x2[9] = (t & ((1 << 25) - 1));
            t = 19 * (t >> 25) + (x[0] * x[0]) + 38 * ((x[2] * x[8]) +
                    (x[4] * x[6]) + (x[5] * x[5])) + 76 * ((x[1] * x[9])
                    + (x[3] * x[7]));
            x2[0] = (t & ((1 << 26) - 1));
            t = (t >> 26) + 2 * (x[0] * x[1]) + 38 * ((x[2] * x[9]) +
                    (x[3] * x[8]) + (x[4] * x[7]) + (x[5] * x[6]));
            x2[1] = (t & ((1 << 25) - 1));
            t = (t >> 25) + 19 * (x[6] * x[6]) + 2 * ((x[0] * x[2]) +
                    (x[1] * x[1])) + 38 * (x[4] * x[8]) + 76 *
                    ((x[3] * x[9]) + (x[5] * x[7]));
            x2[2] = (t & ((1 << 26) - 1));
            t = (t >> 26) + 2 * ((x[0] * x[3]) + (x[1] * x[2])) + 38 *
                    ((x[4] * x[9]) + (x[5] * x[8]) + (x[6] * x[7]));
            x2[3] = (t & ((1 << 25) - 1));
            t = (t >> 25) + (x[2] * x[2]) + 2 * (x[0] * x[4]) + 38 *
                    ((x[6] * x[8]) + (x[7] * x[7])) + 4 * (x[1] * x[3]) + 76 *
                    (x[5] * x[9]);
            x2[4] = (t & ((1 << 26) - 1));
            t = (t >> 26) + 2 * ((x[0] * x[5]) + (x[1] * x[4]) + (x[2] * x[3]))
                    + 38 * ((x[6] * x[9]) + (x[7] * x[8]));
            x2[5] = (t & ((1 << 25) - 1));
            t = (t >> 25) + 19 * (x[8] * x[8]) + 2 * ((x[0] * x[6]) +
                    (x[2] * x[4]) + (x[3] * x[3])) + 4 * (x[1] * x[5]) +
                    76 * (x[7] * x[9]);
            x2[6] = (t & ((1 << 26) - 1));
            t = (t >> 26) + 2 * ((x[0] * x[7]) + (x[1] * x[6]) + (x[2] * x[5]) +
                    (x[3] * x[4])) + 38 * (x[8] * x[9]);
            x2[7] = (t & ((1 << 25) - 1));
            t = (t >> 25) + x2[8];
            x2[8] = (t & ((1 << 26) - 1));
            x2[9] += (t >> 26);
            return x2;
        }

        private static void Recip(Long10 y, Long10 x, int sqrtassist)
        {
            Long10 t0 = new Long10();
            Long10 t1 = new Long10();
            Long10 t2 = new Long10();
            Long10 t3 = new Long10();
            Long10 t4 = new Long10();

            Sqr(t1, x);
            Sqr(t2, t1);
            Sqr(t0, t2);
            Mul(t2, t0, x);
            Mul(t0, t2, t1);
            Sqr(t1, t0);
            Mul(t3, t1, t2);
            Sqr(t1, t3);
            Sqr(t2, t1);
            Sqr(t1, t2);
            Sqr(t2, t1);
            Sqr(t1, t2);
            Mul(t2, t1, t3);
            Sqr(t1, t2);
            Sqr(t3, t1);
            for (int i = 1; i < 5; i++)
            {
                Sqr(t1, t3);
                Sqr(t3, t1);
            }
            Mul(t1, t3, t2);
            Sqr(t3, t1);
            Sqr(t4, t3);
            for (int i = 1; i < 10; i++)
            {
                Sqr(t3, t4);
                Sqr(t4, t3);
            }
            Mul(t3, t4, t1);
            for (int i = 0; i < 5; i++)
            {
                Sqr(t1, t3);
                Sqr(t3, t1);
            }
            Mul(t1, t3, t2);
            Sqr(t2, t1);
            Sqr(t3, t2);
            for (int i = 1; i < 25; i++)
            {
                Sqr(t2, t3);
                Sqr(t3, t2);
            }
            Mul(t2, t3, t1);
            Sqr(t3, t2);
            Sqr(t4, t3);
            for (int i = 1; i < 50; i++)
            {
                Sqr(t3, t4);
                Sqr(t4, t3);
            }
            Mul(t3, t4, t2);
            for (int i = 0; i < 25; i++)
            {
                Sqr(t4, t3);
                Sqr(t3, t4);
            }
            Mul(t2, t3, t1);
            Sqr(t1, t2);
            Sqr(t2, t1);
            if (sqrtassist != 0)
            {
                Mul(y, x, t2);
            }
            else
            {
                Sqr(t1, t2);
                Sqr(t2, t1);
                Sqr(t1, t2);
                Mul(y, t1, t0);
            }
        }

        private static void Sqrt(Long10 x, Long10 u)
        {
            Long10 v = new Long10();
            Long10 t1 = new Long10();
            Long10 t2 = new Long10();
            Add(t1, u, u);
            Recip(v, t1, 1);
            Sqr(x, v);
            Mul(t2, t1, x);
            t2[0]--;
            Mul(t1, v, t2);
            Mul(x, u, t1);
        }

        private static void MontPrep(Long10 t1, Long10 t2, Long10 ax, Long10 az)
        {
            Add(t1, ax, az);
            Sub(t2, ax, az);
        }

        private static void MontAdd(Long10 t1, Long10 t2, Long10 t3, Long10 t4, Long10 ax, Long10 az, Long10 dx)
        {
            Mul(ax, t2, t3);
            Mul(az, t1, t4);
            Add(t1, ax, az);
            Sub(t2, ax, az);
            Sqr(ax, t1);
            Sqr(t1, t2);
            Mul(az, t1, dx);
        }

        private static void MontDbl(Long10 t1, Long10 t2, Long10 t3, Long10 t4, Long10 bx, Long10 bz)
        {
            Sqr(t1, t3);
            Sqr(t2, t4);
            Mul(bx, t1, t2);
            Sub(t2, t1, t2);
            MulSmall(bz, t2, 121665);
            Add(t1, t1, bz);
            Mul(bz, t1, t2);
        }

        private static void XToY2(Long10 t, Long10 y2, Long10 x)
        {
            Sqr(t, x);
            MulSmall(y2, x, 486662);
            Add(t, t, y2);
            t[0]++;
            Mul(y2, t, x);
        }
    }
}
