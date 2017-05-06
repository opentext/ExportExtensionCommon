using System;

namespace ExportExtensionCommon
{
    /// This class is used to get the next available file name if numbering
    /// is used for conflict handling. The point is that we do not want to
    /// search linearily for the next avaiable number (1, 2, 3, ...) but to
    /// apply some binary search for performance reasons.
    /// 
    /// In order to apply the algorithm independent from the CMIS system
    /// and to be able to unit test it the function to check the existance
    /// of a given file is delegated to a function.

    // the delegate
    public delegate bool DNFN_existsFuncion (string filename, int number);

    public class DocumentNameFindNumber
    {
        public DNFN_existsFuncion Exists { get; set; }
        public DocumentNameFindNumber() { }
        public DocumentNameFindNumber(DNFN_existsFuncion e) { Exists = e; }

        private string basename;

        // The algorithm has to two phases. In phase one we search an upper bound.
        // There is an upper bound of 8^8 = 16,777,216. We probe the upper bound
        // in steps 1, 8, 64, 512,...
        public int GetNextFileName(string basename)
        {
            this.basename = basename;
            int maxNumber = (int)Math.Pow(8.0,8.0);
            int low = 1;
            int probe = 1;
            while (probe <= maxNumber)
            {
                if (Exists(basename, probe))
                {
                    low = probe;
                    probe *= 8;
                    continue;
                }
                break;
            }
            if (probe > maxNumber) throw (new Exception("more than " + maxNumber + "files in folder"));
            return findByBinarySearch(low, probe);
        }

        private int findByBinarySearch(int low, int high)
        {
            if (low == high) return low;
            int between = (low + high) / 2;
            if (Exists(basename, between))
                return findByBinarySearch(between+1, high);
             else
                return findByBinarySearch(low, between);
        }
    }
}
