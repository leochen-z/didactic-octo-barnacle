/* Compressor.cs
 * Author: Leo Chen
*/

using Ksu.Cis300.BitIO;
using Ksu.Cis300.ImmutableBinaryTrees;
using Ksu.Cis300.PriorityQueueLibrary;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ksu.Cis300.HuffmanTrees
{
    /// <summary>
    /// Static class to implement the compressor methods.
    /// </summary>
    public static class Compressor
    {
        /// <summary>
        /// Helps faciliate padding to get a string to its proper length.
        /// </summary>
        private static readonly char _padding = '0';

        /// <summary>
        /// The length of the frequency table.
        /// </summary>
        private const int _frequencyTableLength = 256;

        /// <summary>
        /// Gets the frequency table for the given file.
        /// </summary>
        /// <param name="fn">The name of the file to read.</param>
        /// <returns>The frequency table for the file named fn.</returns>
        private static long[] GetFrequencyTable(Stream fn)
        {
            long[] table = new long[_frequencyTableLength];
            int b;
            while ((b = fn.ReadByte()) != -1)
            {
                table[b]++;
            }
            return table;
        }

        /// <summary>
        /// Converts the given value to a bit string that is at least the given minimum length.
        /// </summary>
        /// <param name="val">Value to be converted.</param>
        /// <param name="minLength">The minimum length.</param>
        /// <returns>A bit string.</returns>
        private static string ToBitString(long val, int minLength)
        {
            return Convert.ToString(val, Constants.Radix).PadLeft(minLength, _padding);
        }

        /// <summary>
        /// Method to write to the Huffman Tree.
        /// </summary>
        /// <param name="tree">The Huffman Tree.</param>
        /// <param name="output">BitOutputStream.</param>
        /// <param name="nodesWritten">Number of nodes written so far.</param>
        /// <returns>An int giving the node number of the root of the given tree.</returns>
        private static int WriteTree(BinaryTreeNode<byte> tree, BitOutputStream output, int nodesWritten)
        {
            if (tree.LeftChild == null)
            {
                output.WriteBits(Constants.Leaf);
                output.WriteBits(ToBitString(tree.Data, Constants.BitsInByte));
                return nodesWritten;
            }
            else
            {
                int leftNodesWritten = WriteTree(tree.LeftChild, output, nodesWritten);
                int rightNodesWritten = WriteTree(tree.RightChild, output, leftNodesWritten + 1);
                output.WriteBits(Constants.NonLeaf);
                output.WriteBits(ToBitString(leftNodesWritten, Constants.BitsInNodeNumber));
                return rightNodesWritten + 1;
            }
        }


        /// <summary>
        /// Gets the variable-width encodings of the given tree.
        /// </summary>
        /// <param name="tree">A tree.</param>
        /// <param name="s">StringBuilder.</param>
        /// <param name="encodings">Path.</param>
        private static void GetEncodings(BinaryTreeNode<byte> tree, StringBuilder s, string[] encodings)
        {
            if (tree.LeftChild == null)
            {
                encodings[tree.Data] = s.ToString();
            }
            else
            {
                s.Append(Constants.Left);
                GetEncodings(tree.LeftChild, s, encodings);
                s.Length--;

                s.Append(Constants.Right);
                GetEncodings(tree.RightChild, s, encodings);
                s.Length--;
            }

        }

        /// <summary>
        /// Method responsible for compressing the given input file to the given output file.
        /// </summary>
        /// <param name="compress">Stream giving data to be compressed.</param>
        /// <param name="write">Compressed data to be written.</param>
        public static void Compress(Stream compress, BitOutputStream write)
        {
            if (compress == null || write == null)
            {
                throw new ArgumentNullException();
            }
            else
            {
                long length = compress.Length;
                write.WriteBits(ToBitString(length, Constants.BitsInFileLength)); // Format of Compressed Files Step 1
                if (length != 0)
                {
                    // Step 1 & 2
                    MinPriorityQueue<long, BinaryTreeNode<byte>> queue = GetLeaves(GetFrequencyTable(compress));

                    // Step 3
                    // Format of Compressed Files Step 2
                    // Writes the number of non-leaves which is the number of leaves minus one.
                    write.WriteBits(ToBitString(queue.Count - 1, Constants.BitsInNonLeaves));

                    // Step 4
                    BinaryTreeNode<byte> tree = BuildHuffmanTree(queue);

                    // Step 5 
                    // Format of Compressed Files Step 3
                    WriteTree(tree, write, 0); 

                    // Step 6
                    string[] encodings = new string[_frequencyTableLength];
                    StringBuilder s = new StringBuilder();
                    GetEncodings(tree, s, encodings);

                    // Step 7
                    compress.Seek(0, SeekOrigin.Begin);

                    // Step 8
                    int b;
                    while ((b = compress.ReadByte()) != -1)
                    {
                        if (encodings[b] != null && encodings[b].Length != 0)
                        {
                            write.WriteBits(encodings[b]);
                        }
                    }
                }
            }
        }



        /// <summary>
        /// Gets the leaves of a Huffman tree for the given frequency table.
        /// </summary>
        /// <param name="table">The frequency table for a file.</param>
        /// <returns>A min-priority queue containing the leaves of a Huffman tree
        /// for the given frequency table. The priority of each leaf is the frequency 
        /// of its Data from the table.</returns>
        private static MinPriorityQueue<long, BinaryTreeNode<byte>> GetLeaves(long[] table)
        {
            MinPriorityQueue<long, BinaryTreeNode<byte>> q = new();
            for (int i = 0; i < table.Length; i++)
            {
                if (table[i] > 0)
                {
                    q.Add(table[i], new BinaryTreeNode<byte>((byte)i, null, null));
                }
            }
            return q;
        }

        /// <summary>
        /// Builds a Huffman tree from the given leaves.
        /// </summary>
        /// <param name="q">Contains the leaves of the Huffman tree with priorities set to the
        /// byte frequencies.</param>
        /// <returns>The Huffman tree.</returns>
        private static BinaryTreeNode<byte> BuildHuffmanTree(MinPriorityQueue<long, BinaryTreeNode<byte>> q)
        {
            while (q.Count > 1)
            {
                long p1 = q.MinPriority;
                BinaryTreeNode<byte> t1 = q.RemoveMinPriorityElement();
                long p2 = q.MinPriority;
                BinaryTreeNode<byte> t2 = q.RemoveMinPriorityElement();
                q.Add(p1 + p2, new BinaryTreeNode<byte>(0, t1, t2));
            }
            return q.RemoveMinPriorityElement();
        }

    }
}
