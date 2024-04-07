/* UserInterface.cs
 * Author: Rod Howell
 */
using Ksu.Cis300.PriorityQueueLibrary;
using Ksu.Cis300.ImmutableBinaryTrees;
using KansasStateUniversity.TreeViewer2;
using Ksu.Cis300.BitIO;

namespace Ksu.Cis300.HuffmanTrees
{
    /// <summary>
    /// A GUI for a program that builds and displays Huffman trees.
    /// </summary>
    public partial class UserInterface : Form
    {
        /// <summary>
        /// Suffix used for compressed file names.
        /// </summary>
        private const string _suffix = ".cmp";

        /// <summary>
        /// Constructs the GUI.
        /// </summary>
        public UserInterface()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Handles a Click event on the "Compress a File" button.
        /// </summary>
        /// <param name="sender">The object signaling the event.</param>
        /// <param name="e">Information on the event.</param>
        private void CompressFileClick(object sender, EventArgs e)
        {
            if (uxOpenDialogUncompressed.ShowDialog() == DialogResult.OK)
            {
                uxSaveDialogCompressed.FileName = uxOpenDialogUncompressed.FileName + _suffix;
                if (uxSaveDialogCompressed.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        using (FileStream input = File.OpenRead(uxOpenDialogUncompressed.FileName))
                        {
                            using (BitOutputStream output = new BitOutputStream(uxSaveDialogCompressed.FileName))
                            {
                                Compressor.Compress(input, output);
                            }
                        }
                        MessageBox.Show("Compressed File Written.");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.ToString());
                    }
                }
            }
        }

        /// <summary>
        /// Handles a Click event on the "Decompress a File" button.
        /// </summary>
        /// <param name="sender">The object signaling the event.</param>
        /// <param name="e">Information on the event.</param>
        private void DecompressFileClick(object sender, EventArgs e)
        {
            if (uxOpenDialogCompressed.ShowDialog() == DialogResult.OK)
            {
                uxSaveDialogUncompressed.FileName = uxOpenDialogCompressed.FileName.Substring(0, uxOpenDialogCompressed.FileName.Length - _suffix.Length);
                if (uxSaveDialogUncompressed.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        using (BitInputStream input = new BitInputStream(uxOpenDialogCompressed.FileName))
                        {
                            using (FileStream output = File.Create(uxSaveDialogUncompressed.FileName))
                            {
                                Decompressor.Decompress(input, output);
                            }
                        }
                        MessageBox.Show("Decompressed File Written.");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.ToString());
                    }
                }
            }
        }
    }
}