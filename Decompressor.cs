/* Decompressor.cs
 * Author: Leo Chen
*/

using Ksu.Cis300.BitIO;
using Ksu.Cis300.ImmutableBinaryTrees;
using Ksu.Cis300.PriorityQueueLibrary;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ksu.Cis300.HuffmanTrees
{
    /// <summary>
    /// Static class to implement the decompressor methods.
    /// </summary>
    public static class Decompressor
    {

        private static string ReadBits(int bits, BitInputStream input)
        {
            string read = input.ReadBits(bits);
            if (read.Length != bits)
            {
                throw new IOException("Unexpected end of compressed data.");
            }
            else
            {
                return read;
            }
        }


        public static void Decompress(BitInputStream compress, Stream write)
        {
            if (compress == null || write == null)
            {
                throw new ArgumentNullException();
            }
            else
            {            
                long length = Convert.ToInt64(ReadBits(Constants.BitsInFileLength, compress), Constants.Radix);
                if (length != 0)
                {
                    // Step 1 - Read number of non-leaves in the HuffmanTree and compute total number of nodes.
                    long nonLeaves = Convert.ToInt64(ReadBits(Constants.BitsInNonLeaves, compress), Constants.Radix);
                    long totalNodes = nonLeaves + nonLeaves + 1;

                    // Step 2 - Populate BinaryTreeNode[] with leaves or non-leaves
                    BinaryTreeNode<byte>[] trees = new BinaryTreeNode<byte>[totalNodes];

                    for (int i = 0; i < trees.Length; i++)
                    {
                        if (ReadBits(1, compress) == Constants.Leaf)
                        {
                            byte data = (byte)Convert.ToInt32(ReadBits(Constants.BitsInByte, compress), Constants.Radix);
                            trees[i] = new BinaryTreeNode<byte>(data, null, null);
                        }
                        else
                        {
                            int leftIndex = Convert.ToInt32(ReadBits(Constants.BitsInNodeNumber, compress), Constants.Radix);
                            if (leftIndex >= i)
                            {
                                throw new IOException("The Huffman tree is improperly formed.");
                            }
                            int rightIndex = i - 1;
                            trees[i] = new BinaryTreeNode<byte>(4, trees[leftIndex], trees[rightIndex]);
                        }
                    }

                    // Step 3 - Need to trace out path.
                    while (write.Length < length)
                    {
                        BinaryTreeNode<byte> root = trees[trees.Length - 1];
                        while (root.LeftChild != null)
                        {
                            if (ReadBits(1, compress) == Constants.Left)
                            {
                                root = root.LeftChild;
                            }
                            else
                            {
                                root = root.RightChild;
                            }
                        }
                        write.WriteByte(root.Data);
                    }
                }
            }
        }
    }
}
