namespace Excel.Core.BinaryFormat
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    internal class XlsRootDirectory
    {
        private readonly List<XlsDirectoryEntry> m_entries;
        private readonly XlsDirectoryEntry m_root;

        public XlsRootDirectory(XlsHeader hdr)
        {
            XlsStream stream = new XlsStream(hdr, hdr.RootDirectoryEntryStart, false, null);
            byte[] src = stream.ReadStream();
            List<XlsDirectoryEntry> list = new List<XlsDirectoryEntry>();
            for (int i = 0; i < src.Length; i += 0x80)
            {
                byte[] dst = new byte[0x80];
                Buffer.BlockCopy(src, i, dst, 0, dst.Length);
                list.Add(new XlsDirectoryEntry(dst, hdr));
            }
            this.m_entries = list;
            for (int j = 0; j < list.Count; j++)
            {
                XlsDirectoryEntry entry = list[j];
                if ((this.m_root == null) && (entry.EntryType == STGTY.STGTY_ROOT))
                {
                    this.m_root = entry;
                }
                if (entry.ChildSid != uint.MaxValue)
                {
                    entry.Child = list[(int) entry.ChildSid];
                }
                if (entry.LeftSiblingSid != uint.MaxValue)
                {
                    entry.LeftSibling = list[(int) entry.LeftSiblingSid];
                }
                if (entry.RightSiblingSid != uint.MaxValue)
                {
                    entry.RightSibling = list[(int) entry.RightSiblingSid];
                }
            }
            stream.CalculateMiniFat(this);
        }

        public XlsDirectoryEntry FindEntry(string EntryName)
        {
            XlsDirectoryEntry entry2;
            using (List<XlsDirectoryEntry>.Enumerator enumerator = this.m_entries.GetEnumerator())
            {
                while (true)
                {
                    if (enumerator.MoveNext())
                    {
                        XlsDirectoryEntry current = enumerator.Current;
                        if (!string.Equals(current.EntryName, EntryName, StringComparison.CurrentCultureIgnoreCase))
                        {
                            continue;
                        }
                        entry2 = current;
                    }
                    else
                    {
                        return null;
                    }
                    break;
                }
            }
            return entry2;
        }

        public ReadOnlyCollection<XlsDirectoryEntry> Entries =>
            this.m_entries.AsReadOnly();

        public XlsDirectoryEntry RootEntry =>
            this.m_root;
    }
}

