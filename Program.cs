

using System.Security.Cryptography;

namespace testRecursive
{
    public static class Recursive
    {
        static void Main(string[] args)
        {
            /*var folders = new[] { "Folder1","Folder2","Folder2/ChildFolder","Folder2/ChildFolder2","Folder2/ChildFolder3",
                "Folder2/ChildFolder3/Folderrrr", "Folder2/ChildFolder3/Hi"
            };*/
           /* string directory = args[0];

            if (Directory.Exists(directory))
            {
                var dir = new DirectoryInfo(directory);
                // Get the FileInfo objects for every file in the directory.
                FileInfo[] files = dir.GetFiles();

                using (SHA256 mySHA256 = SHA256.Create())
                {

                }
            }*/

            var root = new Dir("Root");
            foreach (var folder in args[0])
            {
                var tmp = folder.ToString();
                BuildTree(tmp, root);
            }
        }

        private static void BuildTree(string path, Dir parent)
        {
            if (parent == null) return;

            if (path.Contains("/"))
            {
                var dir = path.Substring(0, path.IndexOf("/"));
                var newPath = path.Substring(dir.Length + 1);
                Dir addNodeTo;
                if (!parent.Contains(dir))
                {
                    var newParent = new Dir(dir);
                    parent.Dirs.Add(newParent);
                    addNodeTo = newParent;
                    Console.WriteLine(newParent);
                }
                else
                {
                    addNodeTo = parent.Get(dir);
                }
                BuildTree(newPath, addNodeTo);
            }
            else
            {
                if (!parent.Contains(path))
                    parent.Dirs.Add(new Dir(path));
            }


        }

        public class Dir
        {
            public string Name { get; private set; }
            public string Hash { get; set; }
            public bool Read { get; set; }
            public bool Write { get; set; }
            public List<Dir> Dirs { get; private set; }

            public Dir(string name)
            {
                Name = name;
                Dirs = new List<Dir>();
            }

            public bool Contains(string name)
            {
                return Dirs.Any(d => d.Name.Equals(name));
            }

            public Dir Get(string name)
            {
                return Dirs.FirstOrDefault(d => d.Name.Equals(name));
            }

            public override string ToString()
            {
                return Name;
            }
        }
    }
}