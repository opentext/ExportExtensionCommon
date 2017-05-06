using System;
using System.Collections.ObjectModel;

namespace ExportExtensionCommon
{
    public class SIEEBatch : Collection<SIEEDocument>
    {
        public bool ExportSucceeded()
        {
            foreach (SIEEDocument doc in this) { if (!doc.Succeeded) return false; }
            return true;
        }

        public string ErrorMessage()
        {
            string msg = "";
            foreach (SIEEDocument doc in this)
            {
                if (!doc.Succeeded) 
                {
                    if (msg != "") msg += Environment.NewLine;
                    msg += doc.ErrorMsg;
                }
            }
            return msg;
        }

        public override string ToString()
        {
            string res = "";
            foreach (SIEEDocument d in this) { res += d.ToString() + Environment.NewLine + Environment.NewLine; }
            return res;
        }
    }
}
