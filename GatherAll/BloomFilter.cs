using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
namespace BlogGather
{

        public class BloomFilter
        {
            /// <summary>  
            /// BitArray��������ڴ�飬��C/C++�п�ʹ��BITMAP���  
            /// </summary>  
            private static BitArray bitArray = null;

            private int size = -1;

            /// <summary>  
            /// ���캯������ʼ�������ڴ�  
            /// </summary>  
            /// <param name="size">������ڴ��С,���뱣֤��2����</param>  
            public BloomFilter(int size)
            {
                if (size % 2 == 0)
                {
                    bitArray = new BitArray(size, false);
                    this.size = size;
                }
                else
                {
                    throw new Exception("����ĳ���,���ܱ�2����");
                }
            }

            /// <summary>  
            /// ��str����Bloomfilter����Ҫ��HASH���ҵ�ָ��λ����true  
            /// </summary>  
            /// <param name="str">�ַ���</param>  
            public void Add(string str)
            {
                int[] offsetList = getOffset(str);
                if (offsetList != null)
                {
                    put(offsetList[0]);
                    put(offsetList[1]);
                }
                else
                {
                    throw new Exception("�ַ�������Ϊ��");
                }
            }

            /// <summary>  
            /// �жϸ��ַ����Ƿ��ظ�  
            /// </summary>  
            /// <param name="str">�ַ���</param>  
            /// <returns>true�ظ���֮��false</returns>  
            public Boolean Contains(string str)
            {
                int[] offsetList = getOffset(str);
                if (offsetList != null)
                {
                    if ((get(offsetList[0]) == true) && (get(offsetList[1]) == true))
                    {
                        return true;
                    }

                }
                return false;
            }

            /// <summary>  
            /// �����ڴ��ָ��λ��״̬  
            /// </summary>  
            /// <param name="offset">λ��</param>  
            /// <returns>״̬ΪTRUE����FALSE ΪTRUE��ռ��</returns>  
            private Boolean get(int offset)
            {
                return bitArray[offset];
            }

            /// <summary>  
            /// �ı�ָ��λ��״̬  
            /// </summary>  
            /// <param name="offset">λ��</param>  
            /// <returns>�ı�ɹ�����TRUE���򷵻�FALSE</returns>  
            private Boolean put(int offset)
            {
                //try  
                //{  
                if (bitArray[offset])
                {
                    return false;
                }
                bitArray[offset] = true;
                //}  
                //catch (Exception e)  
                //{  
                // Console.WriteLine(offset);  
                //}  
                return true;
            }

            public int[] getOffset(string str)
            {
                if (String.IsNullOrEmpty(str) != true)
                {
                    int[] offsetList = new int[2];
                    string tmpCode = Hash(str).ToString();
                    int hashCode = Hash2(tmpCode);
                    int offset = Math.Abs(hashCode % (size / 2) - 1);
                    offsetList[0] = offset;
                    hashCode = Hash3(str);
                    offset = size - Math.Abs(hashCode % (size / 2)) - 1;
                    offsetList[1] = offset;
                    return offsetList;
                }
                return null;
            }
            /// <summary>  
            /// �ڴ���С  
            /// </summary>  
            public int Size
            {
                get { return size; }
            }

            /// <summary>  
            /// ��ȡ�ַ���HASHCODE  
            /// </summary>  
            /// <param name="val">�ַ���</param>  
            /// <returns>HASHCODE</returns>  
            private int Hash(string val)
            {
                return val.GetHashCode();
            }

            /// <summary>  
            /// ��ȡ�ַ���HASHCODE  
            /// </summary>  
            /// <param name="val">�ַ���</param>  
            /// <returns>HASHCODE</returns>  
            private int Hash2(string val)
            {
                int hash = 0;

                for (int i = 0; i < val.Length; i++)
                {
                    hash += val[i];
                    hash += (hash << 10);
                    hash ^= (hash >> 6);
                }
                hash += (hash << 3);
                hash ^= (hash >> 11);
                hash += (hash << 15);
                return hash;
            }

            /// <summary>  
            /// ��ȡ�ַ���HASHCODE  
            /// </summary>  
            /// <param name="val">�ַ���</param>  
            /// <returns>HASHCODE</returns>  
            private int Hash3(string str)
            {
                long hash = 0;

                for (int i = 0; i < str.Length; i++)
                {
                    if ((i & 1) == 0)
                    {
                        hash ^= ((hash << 7) ^ str[i] ^ (hash >> 3));
                    }
                    else
                    {
                        hash ^= (~((hash << 11) ^ str[i] ^ (hash >> 5)));
                    }
                }
                unchecked
                {
                    return (int)hash;
                }
            }

        }  
        //static void Main(string[] args)
        //{
        //    System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch();
        //    BloomFilter bf = new BloomFilter(10485760);
        //    const int maxNum = 100000;
        //    double count = 0;

        //    watch.Reset();
        //    watch.Start();
        //    for (int i = 0; i < maxNum; i++)
        //    {
        //        if (bf.Contains(i.ToString()) != true)
        //        {
        //            bf.Add(i.ToString());
        //        }
        //        else
        //        {
        //            //Console.Write("������ײ����:");  
        //            //Console.WriteLine(i);  
        //            count++;
        //        }
        //    }
        //    watch.Stop();
        //    Console.WriteLine("��ײ����:" + (count / (double)maxNum * 100) + "%");
        //    Console.Write("����ʱ��:");
        //    Console.WriteLine(watch.ElapsedMilliseconds.ToString() + "ms");  
        //}
 
}
