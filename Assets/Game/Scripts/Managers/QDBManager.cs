using System.Collections.Generic;

public static class QDBManager
{
    public static List<FileData> FileList = new List<FileData> ();
    public class FileData
    {
        public string filename;
        public string modified_time;
    }
}
