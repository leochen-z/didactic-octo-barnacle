/* Constants.cs
 * Author: Leo Chen
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ksu.Cis300.HuffmanTrees
{
    /// <summary>
    /// Defines constants to be used for the other static classes.
    /// </summary>
    public static class Constants
    {
        /// <summary>
        /// Indicates the number of bits in the file length field.
        /// </summary>
        public static readonly int BitsInFileLength = 63;

        /// <summary>
        /// Indicates the number of bits in the field giving the number of non-leaves.
        /// </summary>
        public static readonly int BitsInNonLeaves = 8;

        /// <summary>
        /// Indicates the string to indicate that a huffman tree node is a leaf.
        /// </summary>
        public static readonly string Leaf = "0";

        /// <summary>
        /// Indicates the string to indicate that a huffman tree node is a non-leaf.
        /// </summary>
        public static readonly string NonLeaf = "1";

        /// <summary>
        /// Indicate the number of bits in a byte.
        /// </summary>
        public static readonly int BitsInByte = 8;

        /// <summary>
        /// Indictaes the number of bits in a node number.
        /// </summary>
        public static readonly int BitsInNodeNumber = 9;

        /// <summary>
        /// Indicates the radix for a bit string.
        /// A bit string encodes a value in base 2.
        /// </summary>
        public static readonly int Radix = 2;

        /// <summary>
        /// Indicates the string to denote a left child.
        /// </summary>
        public static readonly string Left = "0";

        /// <summary>
        /// Indicates the string to denote a right child.
        /// </summary>
        public static readonly string Right = "1";

    }
}
