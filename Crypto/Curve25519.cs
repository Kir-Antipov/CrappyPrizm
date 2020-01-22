using System;

namespace CrappyPrizm.Crypto
{

    internal static partial class Curve25519
    {
        #region Var
        private const int P25 = 33554431;
        private const int P26 = 67108863;

        private static readonly byte[] Order = 
        {
            237, 211, 245, 92,
            26,  99,  18,  88,
            214, 156, 247, 162,
            222, 249, 222, 20,
            0,   0,   0,   0,
            0,   0,   0,   0,
            0,   0,   0,   0,
            0,   0,   0,   16
        };

        private static readonly byte[] OrderTimes8 = 
        {
            104, 159, 174, 231,
            210, 24,  147, 192,
            178, 230, 188, 23,
            245, 206, 247, 166,
            0,   0,   0,   0,
            0,   0,   0,   0,
            0,   0,   0,   0,
            0,   0,   0,   128
        };

        private static readonly Long10 Base2Y = new Long10(39999547, 18689728, 59995525, 1648697, 57546132, 24010086, 19059592, 5425144, 63499247, 16420658);
        private static readonly Long10 BaseR2Y = new Long10(5744, 8160848, 4790893, 13779497, 35730846, 12541209, 49101323, 30047407, 40071253, 6226132);
        #endregion


        public static void Clamp(byte[] k)
        {
            k[31] &= 0x7F;
            k[31] |= 0x40;
            k[0] &= 0xF8;
        }

        public static void Keygen(byte[] P, byte[]? s, byte[] k)
        {
            Clamp(k);
            Core(P, s, k, null);
        }

        public static byte[] Curve(byte[] k, byte[] P)
        {
            byte[] z = new byte[32];
            Core(z, null, k, P);
            return z;
        }

        public static bool Sign(byte[] v, byte[] h, byte[] x, byte[] s)
        {
            byte[] h1 = new byte[32];
            byte[] x1 = new byte[32];
            byte[] tmp1 = new byte[64];
            byte[] tmp2 = new byte[64];

            Cpy32(h1, h);
            Cpy32(x1, x);

            byte[] tmp3 = new byte[32];
            DivMod(tmp3, h1, 32, Order, 32);
            DivMod(tmp3, x1, 32, Order, 32);

            MulaSmall(v, x1, 0, h1, 32, -1);
            MulaSmall(v, v, 0, Order, 32, 1);

            Mula32(tmp1, v, s, 32, 1);
            DivMod(tmp2, tmp1, 64, Order, 32);

            int w, i;
            for (w = 0, i = 0; i < 32; i++)
                w |= v[i] = tmp1[i];
            return w != 0;
        }

        public static void Verify(byte[] Y, byte[] v, byte[] h, byte[] P)
        {
            byte[] d = new byte[32];
            Long10[] p = new[] { new Long10(), new Long10() };
            Long10[] s = new[] { new Long10(), new Long10() };
            Long10[] yx = new[] { new Long10(), new Long10(), new Long10() };
            Long10[] yz = new[] { new Long10(), new Long10(), new Long10() };
            Long10[] t1 = new[] { new Long10(), new Long10(), new Long10() };
            Long10[] t2 = new[] { new Long10(), new Long10(), new Long10() };

            int vi = 0;
            int hi = 0;
            int di = 0;
            int nvh = 0;
            int i, j, k;

            Set(p[0], 9);
            Unpack(p[1], P);

            XToY2(t1[0], t2[0], p[1]);
            Sqrt(t1[0], t2[0]);
            j = t1[0].IsNegative;
            t2[0][0] += 39420360;
            Mul(t2[1], Base2Y, t1[0]);
            Sub(t1[j], t2[0], t2[1]);
            Add(t1[1 - j], t2[0], t2[1]);
            Copy(t2[0], p[1]);
            t2[0][0] -= 9;
            Sqr(t2[1], t2[0]);
            Recip(t2[0], t2[1], 0);
            Mul(s[0], t1[0], t2[0]);
            Sub(s[0], s[0], p[1]);
            s[0][0] -= 9 + 486662;
            Mul(s[1], t1[1], t2[0]);
            Sub(s[1], s[1], p[1]);
            s[1][0] -= 9 + 486662;
            MulSmall(s[0], s[0], 1);
            MulSmall(s[1], s[1], 1);

            for (i = 0; i < 32; i++)
            {
                vi = (vi >> 8) ^ (v[i] & 0xFF) ^ ((v[i] & 0xFF) << 1);
                hi = (hi >> 8) ^ (h[i] & 0xFF) ^ ((h[i] & 0xFF) << 1);
                nvh = ~(vi ^ hi);
                di = (nvh & (di & 0x80) >> 7) ^ vi;
                di ^= nvh & (di & 0x01) << 1;
                di ^= nvh & (di & 0x02) << 1;
                di ^= nvh & (di & 0x04) << 1;
                di ^= nvh & (di & 0x08) << 1;
                di ^= nvh & (di & 0x10) << 1;
                di ^= nvh & (di & 0x20) << 1;
                di ^= nvh & (di & 0x40) << 1;
                d[i] = (byte)di;
            }

            di = ((nvh & (di & 0x80) << 1) ^ vi) >> 8;

            Set(yx[0], 1);
            Copy(yx[1], p[di]);
            Copy(yx[2], s[0]);
            Set(yz[0], 0);
            Set(yz[1], 1);
            Set(yz[2], 1);

            vi = 0;
            hi = 0;

            for (i = 32; i-- != 0;)
            {
                vi = (vi << 8) | (v[i] & 0xFF);
                hi = (hi << 8) | (h[i] & 0xFF);
                di = (di << 8) | (d[i] & 0xFF);

                for (j = 8; j-- != 0;)
                {
                    MontPrep(t1[0], t2[0], yx[0], yz[0]);
                    MontPrep(t1[1], t2[1], yx[1], yz[1]);
                    MontPrep(t1[2], t2[2], yx[2], yz[2]);

                    k = ((vi ^ vi >> 1) >> j & 1)
                            + ((hi ^ hi >> 1) >> j & 1);
                    MontDbl(yx[2], yz[2], t1[k], t2[k], yx[0], yz[0]);

                    k = (di >> j & 2) ^ ((di >> j & 1) << 1);
                    MontAdd(t1[1], t2[1], t1[k], t2[k], yx[1], yz[1],
                            p[di >> j & 1]);

                    MontAdd(t1[2], t2[2], t1[0], t2[0], yx[2], yz[2],
                            s[((vi ^ hi) >> j & 2) >> 1]);
                }
            }

            k = (vi & 1) + (hi & 1);
            Recip(t1[0], yz[k], 0);
            Mul(t1[1], yx[k], t1[0]);

            Pack(t1[1], Y);
        }

        public static bool IsCanonicalSignature(byte[] v)
        {
            byte[] vCopy = new byte[32];
            Array.Copy(v, vCopy, vCopy.Length);
            byte[] tmp = new byte[32];
            DivMod(tmp, vCopy, 32, Order, 32);
            for (int i = 0; i < 32; i++)
                if (v[i] != vCopy[i])
                    return false;
            return true;
        }

        public static bool IsCanonicalPublicKey(byte[] publicKey)
        {
            if (!(publicKey is { Length: 32 }))
                return false;

            Long10 publicKeyUnpacked = new Long10();
            Unpack(publicKeyUnpacked, publicKey);
            byte[] publicKeyCopy = new byte[32];
            Pack(publicKeyUnpacked, publicKeyCopy);
            for (int i = 0; i < 32; i++)
                if (publicKeyCopy[i] != publicKey[i])
                    return false;
            return true;
        }

        private static void Cpy32(byte[] d, byte[] s)
        {
            for (int i = 0; i < 32; i++)
                d[i] = s[i];
        }

        private static int MulaSmall(byte[] p, byte[] q, int m, byte[] x, int n, int z)
        {
            int v = 0;
            for (int i = 0; i < n; ++i)
            {
                v += (q[i + m] & 0xFF) + z * (x[i] & 0xFF);
                p[i + m] = (byte)v;
                v >>= 8;
            }
            return v;
        }

        private static int Mula32(byte[] p, byte[] x, byte[] y, int t, int z)
        {
            const int n = 31;
            int w = 0;
            int i = 0;
            for (; i < t; i++)
            {
                int zy = z * (y[i] & 0xFF);
                w += MulaSmall(p, p, i, x, n, zy) +
                        (p[i + n] & 0xFF) + zy * (x[n] & 0xFF);
                p[i + n] = (byte)w;
                w >>= 8;
            }
            p[i + n] = (byte)(w + (p[i + n] & 0xFF));
            return w >> 8;
        }

        private static void DivMod(byte[] q, byte[] r, int n, byte[] d, int t)
        {
            int rn = 0;
            int dt = ((d[t - 1] & 0xFF) << 8);
            if (t > 1)
                dt |= (d[t - 2] & 0xFF);
            while (n-- >= t)
            {
                int z = (rn << 16) | ((r[n] & 0xFF) << 8);
                if (n > 0)
                    z |= (r[n - 1] & 0xFF);
                z /= dt;
                rn += MulaSmall(r, r, n - t + 1, d, t, -z);
                q[n - t + 1] = (byte)((z + rn) & 0xFF);
                MulaSmall(r, r, n - t + 1, d, t, -rn);
                rn = (r[n] & 0xFF);
                r[n] = 0;
            }
            r[t - 1] = (byte)rn;
        }

        private static int NumSize(byte[] x, int n)
        {
            while (n-- != 0 && x[n] == 0)
                ;
            return n + 1;
        }

        private static byte[] Egcd32(byte[] x, byte[] y, byte[] a, byte[] b)
        {
            int an, bn = 32, qn, i;
            for (i = 0; i < 32; i++)
                x[i] = y[i] = 0;
            x[0] = 1;
            an = NumSize(a, 32);
            if (an == 0)
                return y;
            byte[] temp = new byte[32];
            while (true)
            {
                qn = bn - an + 1;
                DivMod(temp, b, bn, a, an);
                bn = NumSize(b, bn);
                if (bn == 0)
                    return x;
                Mula32(y, x, temp, qn, -1);

                qn = an - bn + 1;
                DivMod(temp, a, an, b, bn);
                an = NumSize(a, an);
                if (an == 0)
                    return y;
                Mula32(x, y, temp, qn, -1);
            }
        }

        private static void Core(byte[] Px, byte[]? s, byte[] k, byte[]? Gx)
        {
            Long10
                    dx = new Long10(),
                    t1 = new Long10(),
                    t2 = new Long10(),
                    t3 = new Long10(),
                    t4 = new Long10();
            Long10[]
                    x = new Long10[] { new Long10(), new Long10() },
                    z = new Long10[] { new Long10(), new Long10() };
            int i, j;

            if (Gx != null)
                Unpack(dx, Gx);
            else
                Set(dx, 9);

            Set(x[0], 1);
            Set(z[0], 0);

            Copy(x[1], dx);
            Set(z[1], 1);

            for (i = 32; i-- != 0;)
            {
                if (i == 0)
                {
                    i = 0;
                }
                for (j = 8; j-- != 0;)
                {
                    int bit1 = (k[i] & 0xFF) >> j & 1;
                    int bit0 = ~(k[i] & 0xFF) >> j & 1;
                    Long10 ax = x[bit0];
                    Long10 az = z[bit0];
                    Long10 bx = x[bit1];
                    Long10 bz = z[bit1];

                    MontPrep(t1, t2, ax, az);
                    MontPrep(t3, t4, bx, bz);
                    MontAdd(t1, t2, t3, t4, ax, az, dx);
                    MontDbl(t1, t2, t3, t4, bx, bz);
                }
            }

            Recip(t1, z[0], 0);
            Mul(dx, x[0], t1);
            Pack(dx, Px);

            if (s != null)
            {
                XToY2(t2, t1, dx);
                Recip(t3, z[1], 0);
                Mul(t2, x[1], t3);
                Add(t2, t2, dx);
                t2[0] += 9 + 486662;
                dx[0] -= 9;
                Sqr(t3, dx);
                Mul(dx, t2, t3);
                Sub(dx, dx, t1);
                dx[0] -= 39420360;
                Mul(t1, dx, BaseR2Y);
                if (t1.IsNegative != 0)
                    Cpy32(s, k);
                else
                    MulaSmall(s, OrderTimes8, 0, k, 32, -1);

                byte[] temp1 = new byte[32];
                byte[] temp2 = new byte[64];
                byte[] temp3 = new byte[64];
                Cpy32(temp1, Order);
                Cpy32(s, Egcd32(temp2, temp3, s, temp1));
                if ((s[31] & 0x80) != 0)
                    MulaSmall(s, s, 0, Order, 32, 1);
            }
        }
    }
}